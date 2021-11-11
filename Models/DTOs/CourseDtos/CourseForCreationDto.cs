using CourseLibrary.Api.Models.DTOs.Base_Dtos;
using CourseLibrary.Api.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models.DTOs.CourseDtos
{
    public class CourseForCreationDto : CourseForManipulationDto //: IValidatableObject
    {
       /* public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Title == Description)
            {
                yield return new ValidationResult("Title and Description cannot be the same",
                    new[] { "CourseForCreationDto" });
            }
        }
       */
    }
}
