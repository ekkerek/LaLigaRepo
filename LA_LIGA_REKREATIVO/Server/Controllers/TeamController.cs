using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Server.Services;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [Route("api/team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;
        private readonly ITeamStatsService _teamStatsService;
        public TeamController(LaLigaContext context, IMapper mapper, ITeamStatsService teamStatsService)
        {
            _context = context;
            _mapper = mapper;
            _teamStatsService = teamStatsService;
        }

        // GET: api/<TeamController>
        [HttpGet]
        public IEnumerable<Team> Get()
        {
            return _context.Teams.ToList();
        }

        // GET api/<TeamController>/5
        [HttpGet("{id}")]
        public TeamDto Get(int id)
        {
            Team? team = _context.Teams.Include(x => x.Leagues).FirstOrDefault(x => x.Id == id);
            var teamDto = _mapper.Map<TeamDto>(team);
            return teamDto;
        }

        [HttpPost("addNew")]
        [Authorize]
        public void AddNew([FromBody] TeamDto teamDto)
        {
            Team team = _mapper.Map<Team>(teamDto);
            team.Leagues.Clear();
            var leagues = _context.Leagues.Where(x => teamDto.Leagues.Select(x => x.Id).Contains(x.Id));
            foreach (var league in leagues)
            {
                team.Leagues.Add(league);
            }
            var entry = _context.Teams.Add(team);

            if (entry != null)
            {
                _context.SaveChanges();
            }
        }

        // PUT api/<TeamController>/5
        [HttpPut("{id}")]
        [Authorize]
        public void Put(int id, [FromBody] TeamDto teamDto)
        {
            var updatedTeam = _context.Teams.Include(x => x.Leagues).FirstOrDefault(x => x.Id == id);
            updatedTeam.Name = teamDto.Name;
            updatedTeam.ParticipantOf = teamDto.ParticipantOf;
            updatedTeam.LogoSrc = teamDto.LogoSrc;
            Team team = _mapper.Map<Team>(teamDto);
            updatedTeam.Leagues.Clear();
            var leagues = _context.Leagues.Where(x => teamDto.Leagues.Select(x => x.Id).Contains(x.Id));
            foreach (var league in leagues)
            {
                updatedTeam.Leagues.Add(_mapper.Map<League>(league));
            }

            _context.Teams.Update(updatedTeam);
            _context.SaveChanges();
        }


        [HttpPost("delete")]
        [Authorize]
        public void Delete([FromBody] int teamId)
        {
            _context.Teams.FirstOrDefault(x => x.Id == teamId).IsDeleted = true;
            _context.SaveChanges();
        }

        // GET api/<TeamController>/5
        [HttpGet("getTeamsByLeague/{leagueId}")]
        public List<TeamDto> GetTeamsByLeague(int leagueId)
        {
            var teams = _context.Teams.Include(x => x.Leagues).Where(x => x.Leagues.Select(x => x.Id).Contains(leagueId));
            var teamsDto = _mapper.Map<List<TeamDto>>(teams);
            return teamsDto;
        }

        // GET api/<TeamController>/5
        [HttpGet("getTeamStats/{teamId}")]
        public TeamStatsDto GetTeamStats(int teamId)
        {
            var matches = _context.Matches.Include(x => x.League).Include(x => x.Summaries).Where(x => x.League.IsActive && ((x.HomeTeamId == teamId || x.AwayTeamId == teamId) && (x.Players.Count() > 0) || x.IsOfficialResult)).ToList();

            var team = _context.Teams.Include(x => x.Leagues).FirstOrDefault(x => x.Id == teamId);

            var teamStatsDto = new TeamStatsDto();
            teamStatsDto.Team = _mapper.Map<TeamDto>(team);
            teamStatsDto.Matches = _mapper.Map<List<MatchDto>>(matches.Where(x => x.HomeTeamId == teamId || x.AwayTeamId == teamId));
            teamStatsDto.GamePlayed = matches.Count(x => x.HomeTeamId == teamId || x.AwayTeamId == teamId);
            teamStatsDto.Goals = matches.Where(x => x.HomeTeamId == teamId).Select(x => x.HomeTeamGoals).Sum() +
                                 matches.Where(x => x.AwayTeamId == teamId).Select(x => x.AwayTeamGoals).Sum();
            teamStatsDto.GoalsConceded = matches.Where(x => x.HomeTeamId == teamId).Select(x => x.AwayTeamGoals).Sum() +
                                 matches.Where(x => x.AwayTeamId == teamId).Select(x => x.HomeTeamGoals).Sum();
            teamStatsDto.Wins = matches.Count(x => x.HomeTeamId == teamId && x.HomeTeamGoals > x.AwayTeamGoals) +
                                matches.Count(x => x.AwayTeamId == teamId && x.HomeTeamGoals < x.AwayTeamGoals);
            teamStatsDto.Losts = matches.Count(x => x.HomeTeamId == teamId && x.HomeTeamGoals < x.AwayTeamGoals) +
                                matches.Count(x => x.AwayTeamId == teamId && x.HomeTeamGoals > x.AwayTeamGoals);
            teamStatsDto.Draws = matches.Count(x => x.HomeTeamId == teamId && x.HomeTeamGoals == x.AwayTeamGoals) +
                                matches.Count(x => x.AwayTeamId == teamId && x.HomeTeamGoals == x.AwayTeamGoals);
            teamStatsDto.TotalPoints = teamStatsDto.Wins * 3 + teamStatsDto.Draws;
            teamStatsDto.PointsByCoefficient = (teamStatsDto.Wins * 3 + teamStatsDto.Draws) * teamStatsDto.Team.Leagues.FirstOrDefault().Coefficient;

            return teamStatsDto;
        }

        [HttpGet("getCommonStanding")]
        public async Task<List<TeamStatsDto>> GetCommonStanding()
        {
            return _teamStatsService.GetCommonStanding();
        }

        [HttpGet("getStandingsByLeague/{id}")]
        public async Task<List<TeamStatsDto>> GetStandingsByLeague(int id)
        {
            return _teamStatsService.GetStandingsByLeague(id);
        }

        [HttpGet("getTeamStatsForNonActiveLeagues/{id}")]
        public List<TeamStatsHistoryDto> GetTeamStatsForNonActiveLeagues(int id)
        {
            return _teamStatsService.GetTeamStatsForNonActiveLeagues(id);
        }
    }
}
