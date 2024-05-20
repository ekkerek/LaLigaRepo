using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var leagues = _context.Leagues.Include(x => x.Teams);
            return _mapper.Map<List<LeagueDto>>(leagues).OrderByDescending(x => x.Id);
        }

        // GET api/<LeagueController>/5
        [HttpGet("{id}")]
        public LeagueDto Get(int id)
        {
            var league = _context.Leagues.Include(x => x.Teams).FirstOrDefault(x => x.Id == id);
            return _mapper.Map<LeagueDto>(league);
        }

        [HttpGet("getLeaguesIncudeOverall")]
        public IEnumerable<LeagueDto> GetLeaguesIncudeOverall()
        {
            var leagues = _context.Leagues.Include(x => x.Teams);
            var overroalLeague = new LeagueDto { Id = 10000, Coefficient = 1, Name = "OverallLeague", Year = 2024 };
            var leaguesDto = _mapper.Map<List<LeagueDto>>(leagues);
            leaguesDto.Add(overroalLeague);
            return leaguesDto.OrderByDescending(x => x.Id);
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
        public void Put(int id, [FromBody] LeagueDto league)
        {
            var updatedLeague = _context.Leagues.FirstOrDefault(x => x.Id == id);
            updatedLeague.Name = league.Name;
            updatedLeague.Year = league.Year;
            updatedLeague.Coefficient = league.Coefficient;
            _context.Leagues.Update(updatedLeague);
            _context.SaveChanges();
        }

        [HttpPost("delete")]
        public void Delete([FromBody] LeagueDto league)
        {
            _context.Leagues.FirstOrDefault(x => x.Id == league.Id).IsDeleted = true;
            _context.SaveChanges();
        }

        //[HttpPost("addTeams")]
        //public void AddTeams([FromBody] LeagueDto league)
        //{
        //    var l = _context.Leagues.FirstOrDefault(x => x.Id == league.Id);
        //    //var teams = _mapper.Map<List<Team>>(league.Teams);
        //    var tttteams = _context.Teams.Where(x => league.Teams.Select(x => x.Id).Contains(x.Id)).ToList();

        //    foreach (var t in tttteams)
        //    {
        //        l.Teams.Add(t);
        //    }
        //    _context.SaveChanges();
        //}


    }
}
