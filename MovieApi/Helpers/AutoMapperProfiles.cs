﻿using AutoMapper;
using MovieApi.DTOs;
using MovieApi.Entities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreationDTO, Actor>()
                .ForMember(x => x.Picture, options => options.Ignore());

            CreateMap<MovieTheater, MovieTheaterDTO>()
                .ForMember(x => x.Latitude, dto => dto.MapFrom(prop => prop.Location.Y))
                .ForMember(x => x.Longitude, dto => dto.MapFrom(prop => prop.Location.X));

            CreateMap<MovieTheaterCreationDTO, MovieTheater>()
                .ForMember(x => x.Location, x => x.MapFrom(dto => geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude))));

            CreateMap<MovieCreationDTO, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MovieTheatersMovies, options => options.MapFrom(MapMovieTheatersMovies))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieDTO>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MovieTheaters, options => options.MapFrom(MapMovieTheatersMovies))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMovieActors));

        }

        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();
            if (movieCreationDTO.GenresIds == null)
            {
                return result;
            }

            foreach (var id in movieCreationDTO.GenresIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }

            return result;
        }

        private List<MovieTheatersMovies> MapMovieTheatersMovies(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MovieTheatersMovies>();

            if (movieCreationDTO.MovieTheatersIds == null)
            {
                return result;
            }

            foreach (var id in movieCreationDTO.MovieTheatersIds)
            {
                result.Add(new MovieTheatersMovies() { MovieTheaterId = id });
            }

            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesActors>();

            if (movieCreationDTO.Actors == null)
            {
                return result;
            }

            foreach (var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { ActorId = actor.Id, Character = actor.Character });
            }

            return result;
        }

        private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDTO movieDto)
        {
            var result = new List<GenreDTO>();

            if (movie.MoviesGenres != null)
            {
                foreach (var genre in movie.MoviesGenres)
                {
                    result.Add(new GenreDTO() { Id = genre.GenreId, Name = genre.Genre.Name });
                }
            }
            return result;
        }

        private List<MovieTheaterDTO> MapMovieTheatersMovies(Movie movie, MovieDTO movieDto)
        {
            var result = new List<MovieTheaterDTO>();

            if (movie.MovieTheatersMovies != null)
            {
                foreach (var movieTheaterMovies in movie.MovieTheatersMovies)
                {
                    result.Add(new MovieTheaterDTO()
                    {
                        Id = movieTheaterMovies.MovieTheaterId,
                        Name = movieTheaterMovies.MovieTheater.Name,
                        Latitude = movieTheaterMovies.MovieTheater.Location.Y,
                        Longitude = movieTheaterMovies.MovieTheater.Location.X
                    });
                }
            }

            return result;
        }

        private List<ActorsMovieDTO> MapMovieActors(Movie movie, MovieDTO movieDto)
        {
            var result = new List<ActorsMovieDTO>();

            if (movie.MoviesActors != null)
            {
                foreach (var moviesActors in movie.MoviesActors)
                {
                    result.Add(new ActorsMovieDTO()
                    {
                        Id = moviesActors.ActorId,
                        Name = moviesActors.Actor.Name,
                        Character = moviesActors.Character,
                        Picture = moviesActors.Actor.Picture,
                        Order = moviesActors.Order
                    });
                }
            }

            return result;
        }
    }
}
