using AutoMapper;
using CourseLibrary.Api.ActionAttributes;
using CourseLibrary.Api.Helpers;
using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.Models.Core.Repositories;
using CourseLibrary.Api.Models.DTOs.AuthorDtos;
using CourseLibrary.Api.Models.DTOs.Base_Dtos;
using CourseLibrary.Api.ResourcesParameters;
using CourseLibrary.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
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
    //[ResponseCache(CacheProfileName = "60SecondsCacheProfile")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyExistenceChecker _propertyExistenceChecker;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper,
            IPropertyMappingService propertyMappingService, IPropertyExistenceChecker propertyExistenceChecker)
        {
            this._courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

            this._mapper = mapper ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));

            this._propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));

            this._propertyExistenceChecker = propertyExistenceChecker ??
                throw new ArgumentNullException(nameof(propertyExistenceChecker));
        }

        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.author.full+json",
            "application/vnd.marvin.author.full.hateoas+json",
            "application/vnd.marvin.author.friendly+json",
            "application/vnd.marvin.author.friendly.hateoas+json")]
        public IActionResult GetAuthors(
        [FromQuery]
        AuthorsResourceParameters authorsParameters,
        [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyMappingService.ValidMappingExistsFor<AuthorsDto, Author>
                (authorsParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertyExistenceChecker.TypeHasProperties<AuthorsDto>(authorsParameters.Fields))
                return BadRequest();



            var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsParameters);

            var pagingMetaData = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                totalPages = authorsFromRepo.TotalPages,
                currentPage = authorsFromRepo.CurrentPage
            };

            Response.Headers.Add("X-Paginations",
                JsonSerializer.Serialize(pagingMetaData,
                    new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas",
                StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForAuthors(authorsParameters, authorsFromRepo.HasNext,
                                    authorsFromRepo.HasPrevious);
            }

            var isFullAuthor = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            if(isFullAuthor == "vnd.marvin.author.full")
            {

                var shappedFullAuthorData = _mapper.Map<IEnumerable<AuthorFullDto>>(authorsFromRepo)
                                                                        .ShapeData(authorsParameters.Fields);

                if (includeLinks)
                {
                    if (!_propertyExistenceChecker.FieldsHasIdProperty(authorsParameters.Fields))
                        return BadRequest();

                    var shappedDataWithLinks = shappedFullAuthorData.Select(author =>
                    {
                        var authorsAsDictionary = author as IDictionary<string, object>;
                        var linksForAuthor = CreateLinksForAuthor((Guid)authorsAsDictionary["Id"], null);
                        authorsAsDictionary.Add("links", linksForAuthor);

                        return authorsAsDictionary;
                    });

                    var linkedResourceToReturn = new
                    {
                        value = shappedDataWithLinks,
                        links
                    };

                    return Ok(linkedResourceToReturn);

                }

                return Ok(shappedFullAuthorData);
            }

            var shappedData = _mapper.Map<IEnumerable<AuthorsDto>>(authorsFromRepo)
                                                                        .ShapeData(authorsParameters.Fields);

            if (includeLinks)
            {
                if (!_propertyExistenceChecker.FieldsHasIdProperty(authorsParameters.Fields))
                    return BadRequest();

                var shappedDataWithLinks = shappedData.Select(author =>
                {
                    var authorsAsDictionary = author as IDictionary<string, object>;
                    var linksForAuthor = CreateLinksForAuthor((Guid)authorsAsDictionary["Id"], null);
                    authorsAsDictionary.Add("links", linksForAuthor);

                    return authorsAsDictionary;
                });

                var linkedResourceToReturn = new
                {
                    value = shappedDataWithLinks,
                    links
                };

                return Ok(linkedResourceToReturn);

            }

            return Ok(_mapper.Map<IEnumerable<AuthorsDto>>(authorsFromRepo)
                                                                        .ShapeData(authorsParameters.Fields));
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.author.full+json",
            "application/vnd.marvin.author.full.hateoas+json",
            "application/vnd.marvin.author.friendly+json",
            "application/vnd.marvin.author.friendly.hateoas+json")]
        public IActionResult GetAuthor(Guid authorId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
                return BadRequest();

            if (!_propertyExistenceChecker.TypeHasProperties<AuthorsDto>(fields))
                return BadRequest();

            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
                return NotFound();

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas",
                StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForAuthor(authorId, fields);
            }

            var isFullAuthor = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            if (isFullAuthor == "vnd.marvin.author.full")
            {
                var linkedFullResourceToReturn = _mapper.Map<AuthorFullDto>(authorFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                    linkedFullResourceToReturn.Add("links", links);

                return Ok(linkedFullResourceToReturn);

            }

            var linkedResourceToReturn = _mapper.Map<AuthorsDto>(authorFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
                linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateAuhtorWithDateOfDeath")]
        [RequestHeaderMatchesTypeAttribute("Content-Type",
            "application/vnd.marvin.authorforcreationwithdateofdeath+json")]
        [Consumes("application/vnd.marvin.authorforcreationwithdateofdeath+json")]
        public ActionResult<AuthorsDto> CreateAuhtorWithDateOfDeath(AuthorWithDateOfDeathForCreationDto author)
        {
            var authorForCreation = _mapper.Map<Author>(author);

            _courseLibraryRepository.AddAuthor(authorForCreation);
            _courseLibraryRepository.Save();

            var authorDto = _mapper.Map<AuthorsDto>(authorForCreation);

            var links = CreateLinksForAuthor(authorDto.Id, null);

            var linkedResourceToReturn = authorDto.ShapeData(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor",
                new { authorId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

    [HttpPost(Name = "CreateAuhtor")]
    [RequestHeaderMatchesType("Content-Type"," application/json",
            "application/vnd.marvin.authorforcreation+json")]
    [Consumes("application/json",
            "application/vnd.marvin.authorforcreation+json")]
    public ActionResult<AuthorsDto> CreateAuthor(AuthorForCreationDto author)
    {
        var authorForCreation = _mapper.Map<Author>(author);

        _courseLibraryRepository.AddAuthor(authorForCreation);
        _courseLibraryRepository.Save();

        var authorDto = _mapper.Map<AuthorsDto>(authorForCreation);

        var links = CreateLinksForAuthor(authorDto.Id, null);

        var linkedResourceToReturn = authorDto.ShapeData(null) as IDictionary<string, object>;
        linkedResourceToReturn.Add("links", links);

        return CreatedAtRoute("GetAuthor",
            new { authorId = linkedResourceToReturn["Id"] },
            linkedResourceToReturn);
    }



    [HttpOptions]
    public IActionResult GetAuthorOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST");
        return Ok();
    }

    [HttpPut("{authorId}", Name = "UpdateAuthor")]
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

    [HttpPatch("{authorId}", Name = "UpdateAuhtorPartially")]
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

        if (!TryValidateModel(authorToUpdate))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(authorToUpdate, author);

        _courseLibraryRepository.UpdateAuthor(author);
        _courseLibraryRepository.Save();

        return NoContent();
    }

    [HttpDelete("{authorId}", Name = "DeleteAuthor")]
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

        return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
    }

    private string CreateAuthorResourceUri(AuthorsResourceParameters authorsResourceParameters,
        ResourcePagingUriType uriType)
    {
        switch (uriType)
        {
            case ResourcePagingUriType.nextPage:
                return Url.Link("GetAuthors", new
                {
                    fields = authorsResourceParameters.Fields,
                    orderBy = authorsResourceParameters.OrderBy,
                    pageSize = authorsResourceParameters.PageSize,
                    pageNumber = authorsResourceParameters.PageNumber + 1,
                    mainCategory = authorsResourceParameters.mainCategory,
                    searchQuery = authorsResourceParameters.searchQuery
                });

            case ResourcePagingUriType.prevPage:
                return Url.Link("GetAuthors", new
                {
                    fields = authorsResourceParameters.Fields,
                    orderBy = authorsResourceParameters.OrderBy,
                    pageSize = authorsResourceParameters.PageSize,
                    pageNumber = authorsResourceParameters.PageNumber - 1,
                    mainCategory = authorsResourceParameters.mainCategory,
                    searchQuery = authorsResourceParameters.searchQuery
                });

            case ResourcePagingUriType.current:
            default:
                return Url.Link("GetAuthors", new
                {
                    fields = authorsResourceParameters.Fields,
                    orderBy = authorsResourceParameters.OrderBy,
                    pageSize = authorsResourceParameters.PageSize,
                    pageNumber = authorsResourceParameters.PageNumber,
                    mainCategory = authorsResourceParameters.mainCategory,
                    searchQuery = authorsResourceParameters.searchQuery
                });
        }

    }
    private IEnumerable<LinkDto> CreateLinksForAuthor(Guid authorId, string fields)
    {
        var links = new List<LinkDto>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            links.Add(new LinkDto(Url.Link("GetAuthor", new { authorId }),
                "self",
                "GET"));
        }
        else
        {
            links.Add(new LinkDto(Url.Link("GetAuthor", new { authorId, fields }),
                "self",
                "GET"));
        }

        links.Add(new LinkDto(Url.Link("DeleteAuthor", new { authorId }),
            "delete_author",
            "DELETE"));

        links.Add(new LinkDto(Url.Link("UpdateAuthor", new { authorId }),
            "update_author",
            "PUT"));

        links.Add(new LinkDto(Url.Link("UpdateAuhtorPartially", new { authorId }),
            "update_author_partially",
            "PATCH"));


        links.Add(new LinkDto(Url.Link("GetCoursesForAuthor", new { authorId }),
            "courses",
            "GET"));

        links.Add(new LinkDto(Url.Link("CreateCourseForAuthor", new { authorId }),
            "create_course_for_author",
            "POST"));

        return links;
    }

    private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParameters parameters,
        bool hasNext,
        bool hasPerv)
    {
        var links = new List<LinkDto>();

        links.Add(new LinkDto(CreateAuthorResourceUri(parameters, ResourcePagingUriType.current),
            "self",
            "GET"));

        links.Add(new LinkDto(Url.Link("CreateAuhtor", null),
            "create_author",
            "POST"));

        if (hasNext)
            links.Add(new LinkDto(CreateAuthorResourceUri(parameters, ResourcePagingUriType.nextPage),
                "NextPage",
                "GET"));

        if (hasPerv)
            links.Add(new LinkDto(CreateAuthorResourceUri(parameters, ResourcePagingUriType.prevPage),
                "PreviousPage",
                "GET"));

        return links;
    }
}
}
