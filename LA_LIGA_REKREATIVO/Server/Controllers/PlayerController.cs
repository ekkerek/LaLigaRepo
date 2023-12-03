using AutoMapper;
using LA_LIGA_REKREATIVO.Client.Server;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Shared.Dto;
using LA_LIGA_REKREATIVO.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;
        public PlayerController(LaLigaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<PlayerController>
        [HttpGet]
        public IEnumerable<PlayerDto> Get()
        {
            var players = _context.Players.Include(x=> x.Team).ToList();
            var playersDto = _mapper.Map<List<PlayerDto>>(players);
            return playersDto;
        }

        // GET api/<PlayerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PlayerController>
        [HttpPost]
        public void Post([FromBody] PlayerDto playerDto)
        {
            var team = _context.Teams.FirstOrDefault(r => r.Id == playerDto.TeamId);
            //playerDto.Team = _mapper.Map<TeamDto>(rl);

            var newPlayer = _mapper.Map<Player>(playerDto);
            newPlayer.Team = team;

            var entry = _context.Players.Add(newPlayer);
            if (entry != null)
            {
                _context.SaveChanges();
            }
        }

        // PUT api/<PlayerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PlayerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("getplayerstats/{id}")]
        public PlayerStatsDto GetPlayerStats(int id)
        {
            var player = _context.Players.Include(x => x.Team).Include(x=> x.Matches).ThenInclude(x=> x.Summaries).FirstOrDefault(x=> x.Id == id);
            var playerStats = player.Matches.Select(x => x.Summaries);
            PlayerStatsDto returnPlayer = new PlayerStatsDto();
            foreach (var stats in playerStats)
            {
                foreach (var stat in stats)
                {
                    var aa = stat.Type switch
                    {
                        SummaryType.Assist => returnPlayer.Assists += 1,
                        SummaryType.Goal => returnPlayer.Goals += 1,
                        SummaryType.GoalsFrom10meter => returnPlayer.GoalsFrom10meter += 1,
                        SummaryType.GoalsFromPenalty => returnPlayer.GoalsFromPenalty += 1,
                        SummaryType.OwnGoal => returnPlayer.OwnGoals += 1,
                        SummaryType.RedCards => returnPlayer.RedCards += 1,
                        SummaryType.YellowCards => returnPlayer.YellowCards += 1,
                    };
                }
            }

            returnPlayer.Player = _mapper.Map<PlayerDto>(player);
            return returnPlayer;
        }
    }
}
