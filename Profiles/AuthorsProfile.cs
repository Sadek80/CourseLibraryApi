using AutoMapper;
using CourseLibrary.Api.Helpers;
using CourseLibrary.Api.Models.Core.Domain;
using CourseLibrary.Api.Models.DTOs.AuthorDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Author, AuthorsDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));

            CreateMap<AuthorForCreationDto, Author>();
            CreateMap<AuthorForUpdateDto, Author>();
            CreateMap<Author, AuthorForUpdateDto>();

        }
    }
}
