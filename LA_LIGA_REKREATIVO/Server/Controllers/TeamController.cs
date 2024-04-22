using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;
        public TeamController(LaLigaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        [AllowAnonymous]
        [HttpPost("addNew")]
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
            //var pp = teamDto.Leagues.FirstOrDefault(x => x.Id)

            if (entry != null)
            {
                _context.SaveChanges();
            }
        }

        // PUT api/<TeamController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] TeamDto teamDto)
        {
            var updatedTeam = _context.Teams.Include(x=> x.Leagues).FirstOrDefault(x => x.Id == id);
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

        // DELETE api/<TeamController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // GET api/<TeamController>/5
        [HttpGet("getTeamsByLeague/{leagueId}")]
        public List<TeamDto> GetTeamsByLeague(int leagueId)
        {
            var teams = _context.Teams.Include(x => x.Leagues).Where(x => x.Leagues.Select(x => x.Id).Contains(leagueId));
            var teamsDto = _mapper.Map<List<TeamDto>>(teams);
            return teamsDto;
        }
    }
}
