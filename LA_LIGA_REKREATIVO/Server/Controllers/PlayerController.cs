using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Server.Services;
using LA_LIGA_REKREATIVO.Shared.Dto;
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
        private readonly IPlayerStatsService _playerStatsService;
        public PlayerController(LaLigaContext context, IMapper mapper, IPlayerStatsService playerStatsService)
        {
            _context = context;
            _mapper = mapper;
            _playerStatsService = playerStatsService;
        }

        // GET: api/<PlayerController>
        [HttpGet]
        public IEnumerable<PlayerDto> Get()
        {
            var players = _context.Players.Include(x => x.Team).ToList();
            var playersDto = _mapper.Map<List<PlayerDto>>(players);
            return playersDto;
        }

        // GET api/<PlayerController>/5
        [HttpGet("{id}")]
        public PlayerDto Get(int id)
        {
            var player = _context.Players.Include(x => x.Team).FirstOrDefault(x => x.Id == id);
            var playerDto = _mapper.Map<PlayerDto>(player);
            return playerDto;
        }

        // POST api/<PlayerController>
        [HttpPost]
        public void Post([FromBody] PlayerDto playerDto)
        {
            var team = _context.Teams.FirstOrDefault(r => r.Id == playerDto.TeamId);

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
        public void Put(int id, [FromBody] PlayerDto value)
        {
            var updatedPlayer = _context.Players.Include(x => x.Team).FirstOrDefault(x => x.Id == id);
            updatedPlayer.FirstName = value.FirstName;
            updatedPlayer.LastName = value.LastName;
            updatedPlayer.Picture = value.Picture;
            updatedPlayer.IsGk = value.IsGk;
            updatedPlayer.Team = _context.Teams.FirstOrDefault(x => x.Id == value.TeamId);
            _context.Players.Update(updatedPlayer);
            _context.SaveChanges();
        }

        [HttpPost("delete")]
        public void Delete([FromBody] PlayerDto player)
        {
            _context.Players.FirstOrDefault(x => x.Id == player.Id).IsDeleted = true;
            _context.SaveChanges();
        }

        [HttpGet("getplayerstats/{id}")]
        public PlayerStatsDto GetPlayerStats(int id)
        {
            return _playerStatsService.GetPlayerStats(id);
        }

        [HttpGet("getplayersstats/{leagueId}")]
        public List<PlayerStatsDto> GetPlayersStats(int leagueId)
        {
            return _playerStatsService.GetPlayersStats23(leagueId);
        }

        [HttpGet("getDreamTeam/{leagueId}")]
        public List<PlayerStatsDto> GetDreamTeam(int leagueId)
        {
            return _playerStatsService.GetDreamTeamByLeague(leagueId);
        }
    }
}
