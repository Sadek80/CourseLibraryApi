using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models.DTOs.AuthorDtos
{
    public class AuthorWithDateOfDeathForCreationDto : AuthorForCreationDto
    {
        [Required(ErrorMessage = "Date of Death must be filled out")]
        public DateTimeOffset DateOfDeath { get; set; }
    }
}
