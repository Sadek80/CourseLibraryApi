using CourseLibrary.Api.Models.DTOs;
using CourseLibrary.Api.Models.DTOs.Base_Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.ValidationAttributes
{
    public class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseForManipulationDto) validationContext.ObjectInstance;

            if(course.Title == course.Description)
            {
                return new ValidationResult(ErrorMessage,
                    new[] { "CourseForManipulationDto" });
            }

            return ValidationResult.Success;
        }
    }
}
