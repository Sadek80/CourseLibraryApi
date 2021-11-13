﻿using AutoMapper;
using CourseLibrary.Api.Helpers;
using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.Models.Core.Repositories;
using CourseLibrary.Api.Models.DTOs.CourseDtos;
using CourseLibrary.Api.ResourcesParameters;
using CourseLibrary.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyExistenceChecker _propertyExistenceChecker;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper, 
            IPropertyMappingService propertyMappingService, IPropertyExistenceChecker propertyExistenceChecker)
        {
            this._courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

            this._mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));

            this._propertyMappingService = propertyMappingService ?? 
                throw new ArgumentNullException(nameof(propertyMappingService));

            this._propertyExistenceChecker = propertyExistenceChecker ?? 
                throw new ArgumentNullException(nameof(propertyExistenceChecker));
        }

        [HttpGet(Name = "GetCoursesForAuthor")]
        public ActionResult<IEnumerable<CoursesDto>> GetCoursesForAuthor(Guid authorId,
            [FromQuery] 
            BaseResourcesParameters parameters)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            if (!_propertyExistenceChecker.TypeHasProperties<CoursesDto>(parameters.Fields))
                return BadRequest();

            if(!_propertyMappingService.ValidMappingExistsFor<CoursesDto, Course>
                (parameters.OrderBy))
            {
                return BadRequest();
            }

            var courses = _courseLibraryRepository.GetCourses(authorId, parameters);

            var nexPageLink = courses.HasNext ? CreateCoursesResourceUri(parameters,
                                                            ResourcePagingUriType.nextPage) : null;

            var previousPageLink = courses.HasPrevious ? CreateCoursesResourceUri(parameters,
                                                            ResourcePagingUriType.prevPage) : null;

            var pagingMetaData = new
            {
                totalCount = courses.TotalCount,
                pageSize = courses.PageSize,
                totalPages = courses.TotalPages,
                currentPage = courses.CurrentPage,
                nexPageLink,
                previousPageLink
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagingMetaData,
                new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));

            return Ok(_mapper.Map<IEnumerable<CoursesDto>>(courses)
                                                           .ShapeData(parameters.Fields));
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        public ActionResult<CoursesDto> GetSingleCourseForAuthor(Guid authorId, Guid courseId, string fields)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            if (!_propertyExistenceChecker.TypeHasProperties<CoursesDto>(fields))
                return BadRequest();

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
                return NotFound();

            return Ok(_mapper.Map<CoursesDto>(course).ShapeData(fields));
        }

        [HttpPost]
        public ActionResult<CoursesDto> CreateCourseForAuthor(Guid authorId, CourseForCreationDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var courseForCreation = _mapper.Map<Course>(course);

            _courseLibraryRepository.AddCourse(authorId, courseForCreation);
            _courseLibraryRepository.Save();

            var courseDto = _mapper.Map<CoursesDto>(courseForCreation);

            return CreatedAtRoute("GetCourseForAuthor",
                new { authorId = authorId, courseId = courseForCreation.Id }, courseDto);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourse(Guid authorId, Guid courseId, CourseForUpdateDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var courseForUpdate = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseForUpdate == null)
            {
                var courseForCreation = _mapper.Map<Course>(course);
                courseForCreation.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseForCreation);
                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CoursesDto>(courseForCreation);

                return CreatedAtRoute("GetCourseForAuthor",
                    new { authorId, courseId = courseToReturn.Id }, courseToReturn);

            }

            _mapper.Map(course, courseForUpdate);

            _courseLibraryRepository.UpdateCourse(courseForUpdate);
            _courseLibraryRepository.Save();

            return NoContent();

        }

        [HttpPatch("{courseId}")]
        public IActionResult PartiallyUpdateCourse(Guid authorId, 
            Guid courseId, 
            JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var courseEntity = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseEntity == null)
            {
                var courseForUpserting = new CourseForUpdateDto();
                patchDocument.ApplyTo(courseForUpserting, ModelState);

                if(!TryValidateModel(courseForUpserting))
                {
                    return ValidationProblem(ModelState);
                }

                var courseToAdd = _mapper.Map<Course>(courseForUpserting);
                courseToAdd.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseToAdd);
                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CoursesDto>(courseToAdd);

                return CreatedAtRoute("GetCourseForAuthor",
                    new { authorId, courseId = courseToReturn.Id }, courseToReturn);
            }

            var courseForUpdate = _mapper.Map<CourseForUpdateDto>(courseEntity);

            patchDocument.ApplyTo(courseForUpdate, ModelState);

            if(!TryValidateModel(courseForUpdate))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(courseForUpdate, courseEntity);

            _courseLibraryRepository.UpdateCourse(courseEntity);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public IActionResult DeleteCourse(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var courseToDelete = _courseLibraryRepository.GetCourse(authorId, courseId);

            if(courseToDelete == null)
            {
                return NotFound();
            }

            _courseLibraryRepository.DeleteCourse(courseToDelete);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue]
            ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.
                GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        private string CreateCoursesResourceUri(BaseResourcesParameters parameters, 
            ResourcePagingUriType uriType)
        {
            switch (uriType)
            {
                case ResourcePagingUriType.nextPage:
                    return Url.Link("GetCoursesForAuthor", new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        searchQuery = parameters.searchQuery,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize
                    });

                case ResourcePagingUriType.prevPage:
                    return Url.Link("GetCoursesForAuthor", new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        searchQuery = parameters.searchQuery,
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize
                    });

                default:
                    return Url.Link("GetCoursesForAuthor", new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        searchQuery = parameters.searchQuery,
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize
                    });
            }
        }
    }
}
