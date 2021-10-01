using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs;
using MovieApi.Entities;
using MovieApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [ApiController]
    [Route("api/movietheaters")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class MovieTheatersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MovieTheatersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieTheaterDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.MovieTheaters.AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var entities = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();

            return _mapper.Map<List<MovieTheaterDTO>>(entities);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieTheaterDTO>> Get(int Id)
        {
            var movieTheater = await _context.MovieTheaters.FirstOrDefaultAsync(x => x.Id == Id);

            if (movieTheater == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieTheaterDTO>(movieTheater);
        }

        [HttpPost]
        public async Task<ActionResult> Post(MovieTheaterCreationDTO movieTheaterCreationDTO)
        {
            var movieTheater = _mapper.Map<MovieTheater>(movieTheaterCreationDTO);
            _context.Add(movieTheater);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int Id,MovieTheaterCreationDTO movieTheaterCreationDTO)
        {
            var movieTheater = await _context.MovieTheaters.FirstOrDefaultAsync(x => x.Id == Id);

            if (movieTheater == null)
            {
                return NotFound();
            }

            movieTheater = _mapper.Map(movieTheaterCreationDTO, movieTheater);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var exists = await _context.MovieTheaters.AnyAsync(x => x.Id == Id);
            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Genre() { Id = Id });
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
