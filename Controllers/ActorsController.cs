using AutoMapper;
using AutoMapper.QueryableExtensions;
using EfCoreMovies.DTOs;
using EfCoreMovies.Entities;
using EfCoreMovies.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreMovies.Controllers
{
    [ApiController]
    [Route("api/actor")]
    public class ActorsController: ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ActorsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ActorDTO>> Get(int page = 1, int recordToTake = 2)
        {
            return await _context.Actors
                .OrderBy(g=>g.Name)
                .ProjectTo<ActorDTO>(_mapper.ConfigurationProvider)
                .Paginate(page, recordToTake)
                .ToListAsync();
        }

        [HttpGet("ids")]
        public async Task<IEnumerable<int>> GetIds()
        {
            return await _context.Actors.Select(a => a.Id).ToListAsync();
        }
    }
}
  