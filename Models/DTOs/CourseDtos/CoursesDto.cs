using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Models.DTOs.CourseDtos
{
    public class CoursesDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AuthorId { get; set; }
    }
}
