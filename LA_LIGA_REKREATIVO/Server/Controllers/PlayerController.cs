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
            var player = _context.Players.Include(x => x.Team).Include(x => x.Matches).ThenInclude(x => x.Summaries).FirstOrDefault(x => x.Id == id);
            var playerStats = player.Matches.Select(x => x.Summaries);
            PlayerStatsDto returnPlayer = new PlayerStatsDto();
            returnPlayer.TotalMatches = player.Matches.Count();
            returnPlayer.Wins = player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals > x.AwayTeamGoals) +
                                player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals > x.HomeTeamGoals);

            returnPlayer.Draws = player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals == x.AwayTeamGoals) +
                                player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals == x.HomeTeamGoals);

            returnPlayer.Losts = player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals < x.AwayTeamGoals) +
                                player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals < x.HomeTeamGoals);

            foreach (var stats in playerStats)
            {
                foreach (var stat in stats)
                {
                    var aa = stat.Type switch
                    {
                        SummaryType.Assist => returnPlayer.Assists += 1,
                        SummaryType.Goal => returnPlayer.Goals += 1,
                        SummaryType.GoalFrom10meter => returnPlayer.GoalsFrom10meter += 1,
                        SummaryType.GoalFromPenalty => returnPlayer.GoalsFromPenalty += 1,
                        SummaryType.OwnGoal => returnPlayer.OwnGoals += 1,
                        SummaryType.RedCards => returnPlayer.RedCards += 1,
                        SummaryType.YellowCards => returnPlayer.YellowCards += 1,
                    };
                }
            }

            returnPlayer.Goals = returnPlayer.Goals + returnPlayer.GoalsFrom10meter + returnPlayer.GoalsFromPenalty;

            returnPlayer.GoalsPerMatch = (double)returnPlayer.Goals / (double)returnPlayer.TotalMatches;
            returnPlayer.WinPerMatch = (double)returnPlayer.Wins / (double)returnPlayer.TotalMatches * 100.0;
            returnPlayer.Player = _mapper.Map<PlayerDto>(player);
            return returnPlayer;
        }

        [HttpGet("getplayersstats/{leagueId}/{count}")]
        public List<PlayerStatsDto> GetPlayersStats(int leagueId, int count)
        {
            var kk = _context.Players.Include(x => x.Team).ThenInclude(x => x.Leagues).AsNoTracking();
            var playerIdsByLeague = kk.Where(x => x.Team.Leagues.Select(x => x.Id).Contains(leagueId)).Select(x => x.Id).ToList();

            List<PlayerStatsDto> playersStats = new();
            foreach (var playerId in playerIdsByLeague)
            {
                var player = _context.Players.Include(x => x.Team).Include(x => x.Matches).ThenInclude(x => x.Summaries).FirstOrDefault(x => x.Id == playerId);
                // delete if we dont need team name on dream team or other details area
                var team = _context.Teams.FirstOrDefault(x => x.Id == player.Team.Id);
                var playerStats = player.Matches.Select(x => x.Summaries);
                PlayerStatsDto returnPlayer = new PlayerStatsDto();
                returnPlayer.TotalMatches = player.Matches.Count();
                returnPlayer.Wins = player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals > x.AwayTeamGoals) +
                                    player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals > x.HomeTeamGoals);

                returnPlayer.Draws = player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals == x.AwayTeamGoals) +
                                    player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals == x.HomeTeamGoals);

                returnPlayer.Losts = player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals < x.AwayTeamGoals) +
                                    player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals < x.HomeTeamGoals);

                foreach (var stats in playerStats)
                {
                    foreach (var stat in stats)
                    {
                        var aa = stat.Type switch
                        {
                            SummaryType.Assist => returnPlayer.Assists += 1,
                            SummaryType.Goal => returnPlayer.Goals += 1,
                            SummaryType.GoalFrom10meter => returnPlayer.GoalsFrom10meter += 1,
                            SummaryType.GoalFromPenalty => returnPlayer.GoalsFromPenalty += 1,
                            SummaryType.OwnGoal => returnPlayer.OwnGoals += 1,
                            SummaryType.RedCards => returnPlayer.RedCards += 1,
                            SummaryType.YellowCards => returnPlayer.YellowCards += 1,
                        };

                        var bb = stat.Type switch
                        {
                            SummaryType.Assist => returnPlayer.TotalPoints += 3,
                            SummaryType.Goal => returnPlayer.TotalPoints += 5,
                            SummaryType.GoalFrom10meter => returnPlayer.TotalPoints += 3,
                            SummaryType.GoalFromPenalty => returnPlayer.TotalPoints += 2,
                            SummaryType.OwnGoal => returnPlayer.TotalPoints -= 3,
                            SummaryType.RedCards => returnPlayer.TotalPoints -= 5,
                            SummaryType.YellowCards => returnPlayer.TotalPoints -= 2,
                        };
                    }
                }

                returnPlayer.Goals = returnPlayer.Goals + returnPlayer.GoalsFrom10meter + returnPlayer.GoalsFromPenalty;

                returnPlayer.GoalsPerMatch = returnPlayer.Goals / returnPlayer.TotalMatches;
                returnPlayer.WinPerMatch = (double)returnPlayer.Wins / (double)returnPlayer.TotalMatches * 100.0;
                returnPlayer.Player = _mapper.Map<PlayerDto>(player);
                // delete if we dont need team name on dream team or other details area
                returnPlayer.Team = _mapper.Map<TeamDto>(team);
                //return returnPlayer;
                playersStats.Add(returnPlayer);
            }
            return playersStats.OrderByDescending(x => x.TotalPoints).Take(count).ToList();

        }

        [HttpGet("getDreamTeam/{leagueId}")]
        public List<PlayerStatsDto> GetDreamTeam(int leagueId)
        {
            var kk = _context.Players.Include(x => x.Team).ThenInclude(x => x.Leagues).AsNoTracking();
            var playerIdsByLeague = kk.Where(x => x.Team.Leagues.Select(x => x.Id).Contains(leagueId)).Select(x => x.Id).ToList();

            List<PlayerStatsDto> playersStats = new();
            foreach (var playerId in playerIdsByLeague)
            {
                var player = _context.Players.Include(x => x.Team).Include(x => x.Matches).ThenInclude(x => x.Summaries).ThenInclude(x => x.Player).FirstOrDefault(x => x.Id == playerId);
                // delete if we dont need team name on dream team or other details area
                var team = _context.Teams.FirstOrDefault(x => x.Id == player.Team.Id);
                var playerStats = player.Matches.SelectMany(x => x.Summaries).Where(x => x.Player.Id == player.Id).ToList();
                PlayerStatsDto returnPlayer = new PlayerStatsDto();
                returnPlayer.TotalMatches = player.Matches.Count();
                returnPlayer.Wins = player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals > x.AwayTeamGoals) +
                                    player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals > x.HomeTeamGoals);

                returnPlayer.Draws = player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals == x.AwayTeamGoals) +
                                    player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals == x.HomeTeamGoals);

                returnPlayer.Losts = player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals < x.AwayTeamGoals) +
                                    player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals < x.HomeTeamGoals);

                foreach (var stat in playerStats)
                {
                    var aa = stat.Type switch
                    {
                        SummaryType.Assist => returnPlayer.Assists += 1,
                        SummaryType.Goal => returnPlayer.Goals += 1,
                        SummaryType.GoalFrom10meter => returnPlayer.GoalsFrom10meter += 1,
                        SummaryType.GoalFromPenalty => returnPlayer.GoalsFromPenalty += 1,
                        SummaryType.OwnGoal => returnPlayer.OwnGoals += 1,
                        SummaryType.RedCards => returnPlayer.RedCards += 1,
                        SummaryType.YellowCards => returnPlayer.YellowCards += 1,
                    };

                    var bb = stat.Type switch
                    {
                        SummaryType.Assist => returnPlayer.TotalPoints += 3,
                        SummaryType.Goal => returnPlayer.TotalPoints += 5,
                        SummaryType.GoalFrom10meter => returnPlayer.TotalPoints += 3,
                        SummaryType.GoalFromPenalty => returnPlayer.TotalPoints += 2,
                        SummaryType.OwnGoal => returnPlayer.TotalPoints -= 3,
                        SummaryType.RedCards => returnPlayer.TotalPoints -= 5,
                        SummaryType.YellowCards => returnPlayer.TotalPoints -= 2,
                    };
                }

                returnPlayer.Goals = returnPlayer.Goals + returnPlayer.GoalsFrom10meter + returnPlayer.GoalsFromPenalty;
                if (returnPlayer.TotalMatches > 0)
                    returnPlayer.GoalsPerMatch = returnPlayer.Goals / returnPlayer.TotalMatches;
                else
                    returnPlayer.GoalsPerMatch = 0;

                if (returnPlayer.TotalMatches > 0)
                    returnPlayer.WinPerMatch = (double)returnPlayer.Wins / (double)returnPlayer.TotalMatches * 100.0;
                else
                    returnPlayer.WinPerMatch = 0;

                returnPlayer.Player = _mapper.Map<PlayerDto>(player);
                // delete if we dont need team name on dream team or other details area
                returnPlayer.Team = _mapper.Map<TeamDto>(team);
                //return returnPlayer;
                playersStats.Add(returnPlayer);
            }
            return playersStats.OrderByDescending(x => x.TotalPoints).ToList();

        }
    }
}
