using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var matches = _context.Matches.Include(x => x.League).Include(x => x.Summaries).ThenInclude(x => x.Player).ToList();
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
            var match = _context.Matches.Include(x => x.League).Include(x => x.Players).Include(x => x.Summaries).ThenInclude(x => x.Player).FirstOrDefault(x => x.Id == id);
            var matchDto = _mapper.Map<MatchDto>(match);

            var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
            var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == match.AwayTeamId);
            matchDto.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
            matchDto.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
            return matchDto;
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
        public async void Put(int id, [FromBody] MatchDto match)
        {
            //var mappedMatch = _mapper.Map<Match>(match);
            var players = _context.Players.Include(x => x.Team).Where(x => match.Players.Select(x => x.Id).Contains(x.Id)).ToList();

            var matchToBeUpdate = _context.Matches.Include(x => x.Players)
                                                   .Include(x => x.Summaries)
                                                   .ThenInclude(x => x.Player)
                                                   .FirstOrDefault(x => x.Id == match.Id);
            matchToBeUpdate.Players.Clear();

            _context.Matches.Include(x => x.Players)
                            .FirstOrDefault(x => x.Id == match.Id)
                            .Players.Clear();

            foreach (var player in players)
            {
                if (match.Players.Select(x => x.Id).Contains(player.Id))
                {
                    matchToBeUpdate.Players.Add(player);
                }
            }

            //foreach (var summary in mappedMatch.Summaries)
            //    summary.Player = players.FirstOrDefault(x => x.Id == summary.Player.Id);
            var allPlayers = _context.Players.Include(x => x.Team).ToList();
            matchToBeUpdate.Summaries.Clear();
            _context.Matches.Include(x => x.Players).Include(x => x.Summaries).ThenInclude(x => x.Player).FirstOrDefault(x => x.Id == match.Id).Summaries.Clear();

            var summaries = _context.Summaries.Where(x => x.Match.Id == match.Id);
            foreach (var item in summaries)
            {
                _context.Summaries.Remove(item);
            }

            foreach (var summary in match.Summaries)
            {
                var sum = _mapper.Map<Summary>(summary);
                sum.Player = allPlayers.FirstOrDefault(x => x.Id == summary.Player.Id);
                matchToBeUpdate.Summaries.Add(sum);

            }

            matchToBeUpdate.League = _context.Leagues.FirstOrDefault(x => x.Id == match.League.Id);

            var entry = _context.Matches.Update(matchToBeUpdate);
            if (entry != null)
            {
                _context.SaveChanges();
            }
        }

        // DELETE api/<MatchController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
