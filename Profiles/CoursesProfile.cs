using AutoMapper;
using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.Models.DTOs.CourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Course, CoursesDto>();
            CreateMap<CourseForCreationDto, Course>();
            CreateMap<CourseForUpdateDto, Course>();
            CreateMap<Course, CourseForUpdateDto>();
        }
    }
}
