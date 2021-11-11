using CourseLibrary.Api.Models.DTOs.Base_Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models.DTOs.CourseDtos
{
    public class CourseForUpdateDto : CourseForManipulationDto
    {
        [Required(ErrorMessage = "Description must be filled out")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
