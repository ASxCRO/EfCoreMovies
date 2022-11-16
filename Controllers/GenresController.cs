using AutoMapper;
using EfCoreMovies.DTOs;
using EfCoreMovies.Entities;
using EfCoreMovies.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreMovies.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenresController: ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GenresController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<Genre>> Get(int page = 1, int recordToTake = 2)
        {
            return await _context.Genres.Paginate(page,recordToTake).ToListAsync();
        }

        [HttpGet("first")]
        public async Task<ActionResult<Genre>> GetFirst()
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(p => p.Name.StartsWith("x"));

            if(genre == null)
            {
                return NotFound();
            }

            return genre;
        }

        [HttpGet("filter")]
        public async Task<IEnumerable<Genre>> Filter(string name)
        {
            return await _context.Genres.Where(g => g.Name.Contains(name)).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(GenreCreationDTO genreCreationDTO)
        {
            _context.Add(_mapper.Map<Genre>(genreCreationDTO));
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("many")]
        public async Task<ActionResult> PostMany(GenreCreationDTO[] genreCreationDTO)
        {
            await _context.AddRangeAsync(_mapper.Map<Genre[]>(genreCreationDTO));
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
