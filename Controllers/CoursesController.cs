using AutoMapper;
using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.Models.Core.Repositories;
using CourseLibrary.Api.Models.DTOs.CourseDtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            this._courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

            this._mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CoursesDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var courses = _courseLibraryRepository.GetCourses(authorId);

            return Ok(_mapper.Map<IEnumerable<CoursesDto>>(courses));
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        public ActionResult<CoursesDto> GetSingleCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var course = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (course == null)
                return NotFound();

            return Ok(_mapper.Map<CoursesDto>(course));
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
    }
}
