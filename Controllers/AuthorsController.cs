using AutoMapper;
using CourseLibrary.Api.Helpers;
using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.Models.Core.Repositories;
using CourseLibrary.Api.Models.DTOs.AuthorDtos;
using CourseLibrary.Api.ResourcesParameters;
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
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            this._courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

            this._mapper = mapper ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

        }

        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorsDto>> GetAuthors(
        [FromQuery]
        AuthorsResourceParameters authorsParameters)
        {
            var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsParameters);

            var nextPageLink = authorsFromRepo.HasNext ?
                CreateAuthorResourceUri(authorsParameters,
                    ResourcePagingUriType.nextPage) : null;

            var previousPageLink = authorsFromRepo.HasPrevious ?
                CreateAuthorResourceUri(authorsParameters,
                    ResourcePagingUriType.prevPage) : null;

            var pagingMetaData = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsParameters.PageSize,
                totalPages = authorsFromRepo.TotalPages,
                currentPage = authorsParameters.PageNumber,
                previousPageLink,
                nextPageLink,
            };

            Response.Headers.Add("X-Paginations",
                JsonSerializer.Serialize(pagingMetaData,
                    new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping}));

            return Ok(_mapper.Map<IEnumerable<AuthorsDto>>(authorsFromRepo));
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public ActionResult<AuthorsDto> GetAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
                return NotFound();

            return Ok(_mapper.Map<AuthorsDto>(authorFromRepo));
        }

        [HttpPost]
        public ActionResult<AuthorsDto> CreateAuthor(AuthorForCreationDto author)
        {
            var authorForCreation = _mapper.Map<Author>(author);

            _courseLibraryRepository.AddAuthor(authorForCreation);
            _courseLibraryRepository.Save();

            var authorDto = _mapper.Map<AuthorsDto>(authorForCreation);
            return CreatedAtRoute("GetAuthor", new { authorId = authorDto.Id }, authorDto);
        }

        [HttpOptions]
        public IActionResult GetAuthorOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }

        [HttpPut("{authorId}")]
        public IActionResult UpdateAuthor(Guid authorId, AuthorForUpdateDto authorForUpdate)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                var authorToCreate = _mapper.Map<Author>(authorForUpdate);
                authorToCreate.Id = authorId;

                _courseLibraryRepository.AddAuthor(authorToCreate);
                _courseLibraryRepository.Save();

                var authorToReturn = _mapper.Map<AuthorsDto>(authorToCreate);

                return CreatedAtRoute("GetAuthor", new { authorId = authorId }, authorToReturn);
            }

            var author = _courseLibraryRepository.GetAuthor(authorId);

            _mapper.Map(authorForUpdate, author);

            _courseLibraryRepository.UpdateAuthor(author);
            _courseLibraryRepository.Save();

            return NoContent();

        }

        [HttpPatch("{authorId}")]
        public IActionResult PartiallyUpdateAuthor(Guid authorId,
            JsonPatchDocument<AuthorForUpdateDto> patchDocument)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var author = _courseLibraryRepository.GetAuthor(authorId);
            var authorToUpdate = _mapper.Map<AuthorForUpdateDto>(author);

            patchDocument.ApplyTo(authorToUpdate, ModelState);

            if(!TryValidateModel(authorToUpdate))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(authorToUpdate, author);

            _courseLibraryRepository.UpdateAuthor(author);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpDelete("{authorId}")]
        public IActionResult DeleteAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();

            var author = _courseLibraryRepository.GetAuthor(authorId);

            _courseLibraryRepository.DeleteAuthor(author);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] 
        ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult) options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        private string CreateAuthorResourceUri(AuthorsResourceParameters authorsResourceParameters,
            ResourcePagingUriType uriType)
        {
            switch (uriType)
            {
                case ResourcePagingUriType.nextPage:
                    return Url.Link("GetAuthors", new 
                    { 
                        pageSize = authorsResourceParameters.PageSize,
                        pageNumber = authorsResourceParameters.PageNumber + 1,
                        mainCategory = authorsResourceParameters.mainCategory,
                        searchQuery = authorsResourceParameters.searchQuery
                    });

                case ResourcePagingUriType.prevPage:
                    return Url.Link("GetAuthors", new
                    {
                        pageSize = authorsResourceParameters.PageSize,
                        pageNumber = authorsResourceParameters.PageNumber - 1,
                        mainCategory = authorsResourceParameters.mainCategory,
                        searchQuery = authorsResourceParameters.searchQuery
                    });

                default:
                    return Url.Link("GetAuthors", new
                    {
                        pageSize = authorsResourceParameters.PageSize,
                        pageNumber = authorsResourceParameters.PageNumber,
                        mainCategory = authorsResourceParameters.mainCategory,
                        searchQuery = authorsResourceParameters.searchQuery
                    });
            }
        }
    }
}
