using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models.DTOs.Base_Dtos
{
    public class AuthorForManipulationDto
    {
        [Required(ErrorMessage = "First Name must be filled out")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "First Name must be filled out")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of Birth must be filled out")]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required(ErrorMessage = "Main Category must be filled out")]
        public string MainCategory { get; set; }
    }
}
