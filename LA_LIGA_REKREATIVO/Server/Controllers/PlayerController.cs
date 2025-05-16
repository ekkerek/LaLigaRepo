using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Server.Services;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _memoryCache;

        public PlayerController(LaLigaContext context, IMapper mapper, IPlayerStatsService playerStatsService, IMemoryCache memoryCache)
        {
            _context = context;
            _mapper = mapper;
            _playerStatsService = playerStatsService;
            _memoryCache = memoryCache;
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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

        [HttpGet("getPlayerStatsForNonActiveLeagues/{id}")]
        public List<PlayerStatsHistoryDto> GetPlayerStatsForNonActiveLeagues(int id)
        {
            return _playerStatsService.GetPlayerStatsForNonActiveLeagues(id);
        }

        [HttpGet("getplayersstats/{leagueId}")]
        public IEnumerable<PlayerStatsDto> GetPlayersStats(int leagueId)
        {
            var cacheData = _memoryCache.Get<IEnumerable<PlayerStatsDto>>($"playerstatsbyleague-{leagueId}");
            if (cacheData != null)
            {
                return cacheData;
            }
            var expirationTime = DateTimeOffset.Now.AddDays(5);
            cacheData = _playerStatsService.GetPlayersStats(leagueId);
            _memoryCache.Set($"playerstatsbyleague-{leagueId}", cacheData, expirationTime);
            return cacheData;
        }

        [HttpGet("getplayersstatsoverall")]
        public IEnumerable<PlayerStatsDto> GetPlayersStatsOverall()
        {
            var cacheData = _memoryCache.Get<IEnumerable<PlayerStatsDto>>("getplayersstatsoverall");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddDays(5);
            cacheData = _playerStatsService.GetPlayersStatsOverall(); ;//await _dbContext.Products.ToListAsync();
            _memoryCache.Set("getplayersstatsoverall", cacheData, expirationTime);
            return cacheData;
        }

        [HttpGet("getDreamTeam/{leagueId}")]
        public IEnumerable<PlayerStatsDto> GetDreamTeam(int leagueId)
        {
            var cacheData = _memoryCache.Get<IEnumerable<PlayerStatsDto>>($"getDreamTeam-{leagueId}");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddDays(7);
            cacheData = _playerStatsService.GetDreamTeamByLeague(leagueId); //await _dbContext.Products.ToListAsync();
            _memoryCache.Set($"getDreamTeam-{leagueId}", cacheData, expirationTime);
            return cacheData;
        }

        [HttpGet("getDreamTeamByLeagueRound/{leagueId}/{round}")]
        public IEnumerable<PlayerStatsDto> GetDreamTeamByLeagueRound(int leagueId, int round)
        {
            var cacheData = _memoryCache.Get<IEnumerable<PlayerStatsDto>>($"getDreamTeamRound-{leagueId}-{round}");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddDays(7);
            cacheData = _playerStatsService.GetDreamTeamByLeagueRound(leagueId, round); //await _dbContext.Products.ToListAsync();
            _memoryCache.Set($"getDreamTeamRound-{leagueId}-{round}", cacheData, expirationTime);
            return cacheData;
        }

        [HttpGet("get2ndDreamTeam/{leagueId}")]
        public IEnumerable<PlayerStatsDto> Get2ndDreamTeam(int leagueId)
        {
            var cacheData = _memoryCache.Get<IEnumerable<PlayerStatsDto>>($"get2ndDreamTeam-{leagueId}");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddDays(7);
            cacheData = _playerStatsService.Get2ndDreamTeamByLeague(leagueId); //await _dbContext.Products.ToListAsync();
            _memoryCache.Set($"get2ndDreamTeam-{leagueId}", cacheData, expirationTime);
            return cacheData;
        }

        [HttpGet("getDreamTeamOverall")]
        public IEnumerable<PlayerStatsDto> GetDreamTeamOverall()
        {
            var cacheData = _memoryCache.Get<IEnumerable<PlayerStatsDto>>("getDreamTeamOverall");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddDays(7);
            cacheData = _playerStatsService.GetDreamTeamOverall();
            _memoryCache.Set("getDreamTeamOverall", cacheData, expirationTime);
            return cacheData;
        }

        [HttpGet("get2ndDreamTeamOverall")]
        public IEnumerable<PlayerStatsDto> Get2ndDreamTeamOverall()
        {
            var cacheData = _memoryCache.Get<IEnumerable<PlayerStatsDto>>("get2ndDreamTeamOverall");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddDays(7);
            cacheData = _playerStatsService.Get2ndDreamTeamOverall();
            _memoryCache.Set("get2ndDreamTeamOverall", cacheData, expirationTime);
            return cacheData;
        }

        [HttpGet("getTeamPlayers/{teamId}")]
        public IEnumerable<PlayerStatsDto> GetTeamPlayers(int teamId)
        {
            return _playerStatsService.GetTeamPlayers(teamId);
        }

        [HttpGet("getTopGoalscorer")]
        public IEnumerable<PlayerStatsDto> GetTopGoalscorer()
        {
            var cacheData = _memoryCache.Get<IEnumerable<PlayerStatsDto>>("getTopGoalscorer");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddDays(5);
            cacheData = _playerStatsService.GetTopGoalscorer();
            _memoryCache.Set("getTopGoalscorer", cacheData, expirationTime);
            return cacheData;
        }

        [HttpGet("getTopAssitent")]
        public IEnumerable<PlayerStatsDto> GetTopAssitent()
        {
            var cacheData = _memoryCache.Get<IEnumerable<PlayerStatsDto>>("getTopAssitent");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddDays(10);
            cacheData = _playerStatsService.GetTopAssitent();
            _memoryCache.Set("getTopAssitent", cacheData, expirationTime);
            return cacheData;
        }
    }
}
