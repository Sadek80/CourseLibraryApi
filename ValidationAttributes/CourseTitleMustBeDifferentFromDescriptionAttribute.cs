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
            var objectInstance = validationContext.ObjectInstance;
            var course = validationContext.ObjectInstance;

            if (course is IEnumerable<CourseForManipulationDto>)
            {
                foreach (var c in course as IEnumerable<CourseForManipulationDto>)
                {
                    if (c.Title == c.Description)
                    {
                        return new ValidationResult(ErrorMessage,
                            new[] { "CourseForManipulationDto" });
                    }
                }
            }
            else
            {
                var singleCourse = (CourseForManipulationDto) course;
                if (singleCourse.Title == singleCourse.Description)
                {
                    return new ValidationResult(ErrorMessage,
                        new[] { "CourseForManipulationDto" });
                }
            }

            return ValidationResult.Success;
        }
    }
}
