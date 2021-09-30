using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieApi.DTOs;
using MovieApi.Entities;
using MovieApi.Filters;
using MovieApi.Helpers;
using MovieApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GenresController(ILogger<GenresController> logger, ApplicationDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        //[ResponseCache(Duration = 60)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[ServiceFilter(typeof(MyActionFilter))]
        public async Task<ActionResult<List<GenreDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            //_logger.LogInformation("Getting all genres");
            var queryable = _context.Genres.AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);

            var genres = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();

            return _mapper.Map<List<GenreDTO>>(genres);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
            return _mapper.Map<List<GenreDTO>>(genres);
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<GenreDTO>> Get(int Id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == Id);

            if (genre == null)
            {
                return NotFound();
            }

            return _mapper.Map<GenreDTO>(genre);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
        {
            var genre = _mapper.Map<Genre>(genreCreationDTO);
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int Id ,[FromBody] GenreCreationDTO genreCreationDTO)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == Id);
            if (genre == null)
            {
                return NotFound();
            }
            genre = _mapper.Map(genreCreationDTO, genre);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Genres.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Genre() { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
