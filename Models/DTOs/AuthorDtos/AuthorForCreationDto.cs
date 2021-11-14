using CourseLibrary.Api.Models.DTOs.CourseDtos;
using CourseLibrary.Api.Models.DTOs.Base_Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models.DTOs.AuthorDtos
{
    public class AuthorForCreationDto : AuthorForManipulationDto
    {
        public ICollection<CourseForCreationDto> Courses { get; set; }
            = new List<CourseForCreationDto>();
    }
}
