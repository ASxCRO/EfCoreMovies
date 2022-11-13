using EfCoreMovies.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreMovies.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenresController: ControllerBase
    {
        private readonly AppDbContext _context;

        public GenresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Genre>> Get()
        {
            return await _context.Genres.ToListAsync();
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
    }
}
