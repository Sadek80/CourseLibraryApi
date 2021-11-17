using AutoMapper;
using CourseLibrary.Api.Helpers;
using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.Models.Core.Repositories;
using CourseLibrary.Api.Models.DTOs.AuthorDtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/authorcollections")]
    [ResponseCache(CacheProfileName = "60SecondsCacheProfile")]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorCollectionsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            this._courseLibraryRepository = courseLibraryRepository ?? 
                                                throw new ArgumentNullException(nameof(courseLibraryRepository));
            this._mapper = mapper ?? 
                              throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("({ids})", Name = "GetAuthorsCollection")]
        public ActionResult<IEnumerable<AuthorsDto>> GetAuthorsCollection(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            var authors = _courseLibraryRepository.GetAuthors(ids);

            if (ids.Count() != authors.Count())
                return NotFound();

            var authorsDto = _mapper.Map<IEnumerable<AuthorsDto>>(authors);

            return Ok(authorsDto);
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorsDto>> CreateAuthorCollection(
                                                            IEnumerable<AuthorForCreationDto> authors)
        {
            var authorsCollection = _mapper.Map<IEnumerable<Author>>(authors);

            foreach (var author in authorsCollection)
            {
                _courseLibraryRepository.AddAuthor(author);
            }
            _courseLibraryRepository.Save();

            var authorsDto = _mapper.Map<IEnumerable<AuthorsDto>>(authorsCollection);

            var ids = string.Join(",", authorsCollection.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorsCollection", new {ids = ids}, authorsDto);
        }
    }
}
