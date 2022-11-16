using AutoMapper;
using AutoMapper.QueryableExtensions;
using EfCoreMovies.DTOs;
using EfCoreMovies.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EfCoreMovies.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MoviesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await _context.Movies
                .Include(m=>m.Genres)
                .Include(m=>m.CinemaHalls)
                    .ThenInclude(ch => ch.Cinema)
                .Include(m=> m.MoviesActors)
                    .ThenInclude(ma=> ma.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if(movie == null)
                return NotFound();

            var movieDTO = _mapper.Map<MovieDTO>(movie);

            movieDTO.Cinemas.DistinctBy(c => c.Id);

            return movieDTO;
        }

        [HttpGet("automapper/{id:int}")]
        public async Task<ActionResult<MovieDTO>> GetWithAutoMapper(int id)
        {
            var movieDTO = await _context.Movies
                .ProjectTo<MovieDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movieDTO == null)
                return NotFound();


            movieDTO.Cinemas.DistinctBy(c => c.Id);

            return movieDTO;
        }

        [HttpGet("lazyloand/{id:int}")]
        public async Task<ActionResult<MovieDTO>> GetLazyLoading(int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);

            if(movie is null)
            {
                return NotFound(); 
            }

            var movieDTO = _mapper.Map<MovieDTO>(movie);

            movieDTO.Cinemas = movieDTO.Cinemas.DistinctBy(x => x.Id).ToList();

            return movieDTO;

        }

        [HttpGet("groupedByCinema")]
        public async Task<ActionResult> GetGroupedByCinema()
        {            
            var groupedMovies = await _context.Movies.GroupBy(m => m.InCinemas).Select(g => new
            {
                InCinemas = g.Key,
                Count = g.Count(),
                Movies = g.ToList()
            }).ToListAsync();

            return Ok(groupedMovies);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> Filter([FromQuery] MovieFilterDTO movieFilterDTO)
        {
            var moviesQueryable = _context.Movies.AsQueryable();

            if(!string.IsNullOrEmpty(movieFilterDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(movieFilterDTO.Title));
            }

            if (movieFilterDTO.InCinemas)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InCinemas);
            }

            if (movieFilterDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if(movieFilterDTO.GenreId != 0)

            {
                moviesQueryable = moviesQueryable.Where(m => m.Genres.Select(g => g.Id).Contains(movieFilterDTO.GenreId));
            }

            var movies = await moviesQueryable.Include(m => m.Genres).ToListAsync();


            return Ok(movies); 
        }
    }
}
