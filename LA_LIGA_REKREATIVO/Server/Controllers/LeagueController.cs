using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;

        public LeagueController(LaLigaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<LeagueController>
        [HttpGet]
        public IEnumerable<LeagueDto> Get()
        {
            return _mapper.Map<List<LeagueDto>>(_context.Leagues);
        }

        // GET api/<LeagueController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LeagueController>
        [HttpPost]
        public void Post([FromBody] LeagueDto league)
        {
            _context.Leagues.Add(_mapper.Map<League>(league));
            _context.SaveChanges();
        }

        // PUT api/<LeagueController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LeagueController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("addTeams")]
        public void AddTeams([FromBody] LeagueDto league)
        {
            var l = _context.Leagues.FirstOrDefault(x => x.Id == league.Id);
            //var teams = _mapper.Map<List<Team>>(league.Teams);
            var tttteams = _context.Teams.Where(x => league.Teams.Select(x => x.Id).Contains(x.Id)).ToList();

            foreach (var t in tttteams)
            {
                l.Teams.Add(t);
            }
            _context.SaveChanges();
        }
    }
}
