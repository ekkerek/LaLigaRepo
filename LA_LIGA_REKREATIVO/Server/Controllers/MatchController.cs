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
    public class MatchController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;
        public MatchController(LaLigaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<MatchController>
        [HttpGet]
        public IEnumerable<MatchDto> Get()
        {
            var matches = _context.Matches.ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(_context.Matches);
            foreach (var match in matchesDto)
            {
                var homeTeamId = matches.FirstOrDefault(x => x.Id == match.Id).HomeTeamId;
                var awayTeamId = matches.FirstOrDefault(x => x.Id == match.Id).AwayTeamId;
                var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                match.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                match.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
            }
            return matchesDto;
        }


        [HttpPost("getByDate")]
        public IEnumerable<MatchDto> GetByDate([FromBody] DateTime date)
        {
            var aa = date.Date.ToUniversalTime();
            //var bb = _context.Matches.Last();
            //var cc = bb.GameTime.ToUniversalTime();
            var matches = _context.Matches.Where(x => x.GameTime.ToUniversalTime().Date == date.ToUniversalTime().Date).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(matches);
            foreach (var match in matchesDto)
            {
                var homeTeamId = matches.FirstOrDefault(x => x.Id == match.Id).HomeTeamId;
                var awayTeamId = matches.FirstOrDefault(x => x.Id == match.Id).AwayTeamId;
                var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                match.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                match.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
            }
            return matchesDto;
        }

        // GET api/<MatchController>/5
        [HttpGet("{id}")]
        public MatchDto Get(int id)
        {
            return _mapper.Map<List<MatchDto>>(_context.Matches).FirstOrDefault();
        }

        // POST api/<MatchController>
        [HttpPost]
        public void Post([FromBody] MatchDto match)
        {
            var mappedMatch = _mapper.Map<Match>(match);

            //map players
            var players = _context.Players.Include(x => x.Team).Where(x => match.Players.Select(x => x.Id).Contains(x.Id)).ToList();
            mappedMatch.Players = players;

            //map summaries
            foreach (var summary in mappedMatch.Summaries)
                summary.Player = players.FirstOrDefault(x => x.Id == summary.Player.Id);

            mappedMatch.League = _context.Leagues.FirstOrDefault(x => x.Id == match.League.Id);

            var entry = _context.Matches.Add(mappedMatch);
            if (entry != null)
            {
                _context.SaveChanges();
            }
        }

        // PUT api/<MatchController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MatchController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
