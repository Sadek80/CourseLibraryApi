using CourseLibrary.Api.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models.DTOs.Base_Dtos
{
    [CourseTitleMustBeDifferentFromDescriptionAttribute(
        ErrorMessage = "The title must not be the same as the Description")]
    public abstract class CourseForManipulationDto
    {
        [Required(ErrorMessage = "The Title Must be Filled Out")]
        [MaxLength(100, ErrorMessage = "The tilte should not be more than 100 charachters")]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "The Description should not be more than 1500 charachters")]
        public virtual string Description { get; set; }
    }
}
