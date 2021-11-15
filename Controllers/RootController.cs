using CourseLibrary.Api.Models.DTOs.Base_Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [ResponseCache(CacheProfileName = "240SecondsCacheProfile")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "Root")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link("Root", null),
                "self",
                "GET"));

            links.Add(new LinkDto(Url.Link("GetAuthors", null),
                "authors",
                "GET"));

            links.Add(new LinkDto(Url.Link("CreateAuhtor", null),
                "create_author",
                "POST"));

            return Ok(links);
            
        }
    }
}
