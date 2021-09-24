using AutoMapper;
using MovieApi.DTOs;
using MovieApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Helpers
{
    public class AutoMappterProfiles : Profile
    {
        public AutoMappterProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();

        }
    }
}
