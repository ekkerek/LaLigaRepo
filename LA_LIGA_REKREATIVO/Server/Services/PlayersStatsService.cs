using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.EntityFrameworkCore;

namespace LA_LIGA_REKREATIVO.Server.Services
{
    public class PlayersStatsService : IPlayerStatsService
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;
        public PlayersStatsService(LaLigaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<PlayerStatsDto> GetStatsByGoal(int leagueId)
        {
            var playersStats = GetPlayersStatsByLeague(leagueId).OrderByDescending(x => x.Goals).ToList();
            SetOrderId(playersStats);
            return playersStats;
        }

        public List<PlayerStatsDto> GetStatsByAssists(int leagueId)
        {
            var playersStats = GetPlayersStatsByLeague(leagueId).OrderByDescending(x => x.Assists).ToList();
            SetOrderId(playersStats);
            return playersStats;
        }

        public List<PlayerStatsDto> GetDreamTeamByLeague(int leagueId)
        {
            var playersStats = GetPlayersStatsByLeague(leagueId).Where(x => !x.Player.IsGk).OrderByDescending(x => x.TotalPoints).Take(4).ToList();
            playersStats.Add(GetBestGkByLeague(leagueId));
            SetOrderId(playersStats);
            return playersStats;
        }

        public List<PlayerStatsDto> GetDreamTeamOverall()
        {
            var playersStats = GetPlayersStatsOverall().Where(x => !x.Player.IsGk).OrderByDescending(x => x.TotalPoints).Take(4).ToList();
            playersStats.Add(GetBestGkOverall());
            SetOrderId(playersStats);
            return playersStats;
        }

        public List<PlayerStatsDto> Get2ndDreamTeamByLeague(int leagueId)
        {
            var playersStats = GetPlayersStatsByLeague(leagueId)
                                .Where(x => !x.Player.IsGk)
                                .OrderByDescending(x => x.TotalPoints)
                                .Skip(4)
                                .Take(4)
                                .ToList();
            playersStats.Add(Get2ndBestGkByLeague(leagueId));
            return playersStats;
        }

        public List<PlayerStatsDto> Get2ndDreamTeamOverall()
        {
            var playersStats = GetPlayersStatsOverall()
                                    .Where(x => !x.Player.IsGk)
                                    .OrderByDescending(x => x.TotalPoints)
                                    .Skip(4)
                                    .Take(4)
                                    .ToList();
            playersStats.Add(Get2ndBestGkOverall());
            return playersStats;
        }

        public List<PlayerStatsDto> GetPlayersStats(int leagueId)
        {
            var playersStats = GetPlayersStatsByLeague(leagueId).OrderByDescending(x => x.TotalPoints).ToList();
            SetOrderId(playersStats);
            return playersStats;
        }

        private void SetOrderId(List<PlayerStatsDto> playerStats)
        {
            int orderId = 1;
            playerStats.ForEach(x => { x.OrderId = orderId; orderId++; });
        }

        public List<PlayerStatsDto> GetPlayersStatsOverall()
        {
            var playerIds = _context.Players.AsNoTracking().Select(x => x.Id).ToList();
            List<PlayerStatsDto> playersStats = new();
            foreach (var playerId in playerIds)
            {
                PlayerStatsDto returnPlayer = new();
                returnPlayer = GetPlayerStats(playerId);
                returnPlayer.TotalPoints = (double)RecalculatePlayerPointsForOverallDreamTeam(playerId);
                playersStats.Add(returnPlayer);
            }
            return playersStats.ToList();
        }

        private List<PlayerStatsDto> GetPlayersStatsByLeague(int leagueId)
        {
            var players = _context.Players.Include(x => x.Team).ThenInclude(x => x.Leagues).AsNoTracking();
            var playerIdsByLeague = players.Where(x => x.Team.Leagues.Select(x => x.Id).Contains(leagueId)).Select(x => x.Id).ToList();

            List<PlayerStatsDto> playersStats = new();
            foreach (var playerId in playerIdsByLeague)
            {
                PlayerStatsDto returnPlayer = new();
                returnPlayer = GetPlayerStats(playerId, leagueId);
                playersStats.Add(returnPlayer);
            }
            return playersStats.ToList();
        }

        private PlayerStatsDto GetBestGkByLeague(int leagueId)
        {
            return GetPlayersStatsByLeague(leagueId).Where(x => x.Player.IsGk)
                                          .OrderByDescending(x => x.TotalPoints)
                                          .FirstOrDefault();
        }

        private PlayerStatsDto GetBestGkOverall()
        {
            return GetPlayersStatsOverall().Where(x => x.Player.IsGk)
                                           .OrderByDescending(x => x.TotalPoints)
                                           .FirstOrDefault();
        }

        private PlayerStatsDto Get2ndBestGkByLeague(int leagueId)
        {
            return GetPlayersStatsByLeague(leagueId).Where(x => x.Player.IsGk)
                                          .OrderByDescending(x => x.TotalPoints)
                                          .Skip(1)
                                          .FirstOrDefault();
        }

        private PlayerStatsDto Get2ndBestGkOverall()
        {
            return GetPlayersStatsOverall().Where(x => x.Player.IsGk)
                                           .OrderByDescending(x => x.TotalPoints)
                                           .Skip(1)
                                           .FirstOrDefault();
        }

        public List<PlayerStatsDto> GetTeamPlayers(int teamId)
        {
            var players = _context.Players.Include(x => x.Team).AsNoTracking();
            var playerIdsByLeague = players.Where(x => x.Team.Id == teamId).Select(x => x.Id).ToList();

            List<PlayerStatsDto> playersStats = new();
            foreach (var playerId in playerIdsByLeague)
            {
                PlayerStatsDto returnPlayer = new();
                returnPlayer = GetPlayerStats(playerId);
                playersStats.Add(returnPlayer);
            }
            return playersStats.ToList();
        }

        public PlayerStatsDto GetPlayerStatsForOverallDreamTeam(int id)
        {
            var player = _context.Players.Include(x => x.Team).Include(x => x.Matches).ThenInclude(x => x.Summaries).ThenInclude(x => x.Player).FirstOrDefault(x => x.Id == id);
            var playerStats = player.Matches.Select(x => x.Summaries);
            PlayerStatsDto returnPlayer = new PlayerStatsDto();
            returnPlayer.TotalMatches = player.Matches.Count();
            returnPlayer.Wins = CalculatePlayerWins(player);//player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals > x.AwayTeamGoals) +
                                                            //player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals > x.HomeTeamGoals);

            returnPlayer.Draws = CalculatePlayerDraws(player); //player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals == x.AwayTeamGoals) +
                                                               //player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals == x.HomeTeamGoals);

            returnPlayer.Losts = CalculatePlayerLosts(player);//player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals < x.AwayTeamGoals) +
                                                              //player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals < x.HomeTeamGoals);

            foreach (var stats in playerStats)
            {
                foreach (var stat in stats)
                {
                    if (stat.Player.Id == id)
                    {
                        var aa = stat.Type switch
                        {
                            SummaryType.Goal => returnPlayer.Goals += 1,
                            SummaryType.Assist => returnPlayer.Assists += 1,
                            SummaryType.OwnGoal => returnPlayer.OwnGoals += 1,
                            SummaryType.YellowCards => returnPlayer.YellowCards += 1,
                            SummaryType.RedCards => returnPlayer.RedCards += 1,
                            SummaryType.MissedPenalty => returnPlayer.MissedPenalty += 1,
                            SummaryType.Missed10meter => returnPlayer.Missed10meter += 1,
                            SummaryType.CleanSheetGK => returnPlayer.CleanSheetGK += 1,
                            SummaryType.FourSavesGK => returnPlayer.FourSavesGK += 1,
                            SummaryType.SavedFromPenaltyGK => returnPlayer.SavedFromPenaltyGK += 1,
                            SummaryType.SavedFrom10meterGK => returnPlayer.SavedFrom10meterGK += 1
                        };

                        var bb = stat.Type switch
                        {
                            SummaryType.Goal => returnPlayer.TotalPoints += 5,
                            SummaryType.Assist => returnPlayer.TotalPoints += 2,
                            SummaryType.OwnGoal => returnPlayer.TotalPoints -= 3,
                            SummaryType.YellowCards => returnPlayer.TotalPoints -= 2,
                            SummaryType.RedCards => returnPlayer.TotalPoints -= 7,
                            SummaryType.MissedPenalty => returnPlayer.TotalPoints -= 3,
                            SummaryType.Missed10meter => returnPlayer.TotalPoints -= 2,
                            SummaryType.CleanSheetGK => returnPlayer.TotalPoints += 5,
                            SummaryType.FourSavesGK => returnPlayer.TotalPoints += 1,
                            SummaryType.SavedFromPenaltyGK => returnPlayer.TotalPoints += 3,
                            SummaryType.SavedFrom10meterGK => returnPlayer.TotalPoints += 2
                        };
                    }
                }
            }

            returnPlayer.TotalPoints += returnPlayer.TotalMatches * 2; // match attened
            returnPlayer.GoalsPerMatch = returnPlayer.Goals == 0 ? 0 :
                (double)returnPlayer.Goals / (double)returnPlayer.TotalMatches;
            returnPlayer.WinPerMatch = returnPlayer.Wins == 0 ? 0 :
                (double)returnPlayer.Wins / (double)returnPlayer.TotalMatches * 100.0;
            returnPlayer.Player = _mapper.Map<PlayerDto>(player);
            returnPlayer.Team = _mapper.Map<TeamDto>(player.Team);
            return returnPlayer;
        }

        public PlayerStatsDto GetPlayerStats(int id, int leagueId = 0)
        {
            var playerStats = Enumerable.Empty<ICollection<Summary>>();
            var player = _context.Players.Include(x => x.Team).Include(x => x.Matches).ThenInclude(x => x.Summaries).ThenInclude(x => x.Player).Include(x => x.Matches).ThenInclude(x => x.League).FirstOrDefault(x => x.Id == id);
            PlayerStatsDto returnPlayer = new PlayerStatsDto();
            if (leagueId != 0)
            {
                playerStats = player.Matches.Where(x => x.League.Id == leagueId).Select(x => x.Summaries);
                returnPlayer.TotalMatches = player.Matches.Where(x => x.League.Id == leagueId).Count();
            }
            else
            {
                playerStats = player.Matches.Select(x => x.Summaries);
                returnPlayer.TotalMatches = player.Matches.Count();
            }

            if (returnPlayer.TotalMatches == 0)
            {   var testNewPlayerStats = new PlayerStatsDto();
                testNewPlayerStats.Player = _mapper.Map<PlayerDto>(player);
                testNewPlayerStats.Team = _mapper.Map<TeamDto>(player.Team);
                return testNewPlayerStats;
            }

            returnPlayer.Wins = CalculatePlayerWins(player);//player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals > x.AwayTeamGoals) +
                                                            //player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals > x.HomeTeamGoals);

            returnPlayer.Draws = CalculatePlayerDraws(player); //player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals == x.AwayTeamGoals) +
                                                               //player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals == x.HomeTeamGoals);

            returnPlayer.Losts = CalculatePlayerLosts(player);//player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals < x.AwayTeamGoals) +
                                                              //player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals < x.HomeTeamGoals);

            foreach (var stats in playerStats)
            {
                foreach (var stat in stats)
                {
                    if (stat.Player.Id == id)
                    {
                        var aa = stat.Type switch
                        {
                            SummaryType.Goal => returnPlayer.Goals += 1,
                            SummaryType.Assist => returnPlayer.Assists += 1,
                            SummaryType.OwnGoal => returnPlayer.OwnGoals += 1,
                            SummaryType.YellowCards => returnPlayer.YellowCards += 1,
                            SummaryType.RedCards => returnPlayer.RedCards += 1,
                            SummaryType.MissedPenalty => returnPlayer.MissedPenalty += 1,
                            SummaryType.Missed10meter => returnPlayer.Missed10meter += 1,
                            SummaryType.CleanSheetGK => returnPlayer.CleanSheetGK += 1,
                            SummaryType.FourSavesGK => returnPlayer.FourSavesGK += 1,
                            SummaryType.SavedFromPenaltyGK => returnPlayer.SavedFromPenaltyGK += 1,
                            SummaryType.SavedFrom10meterGK => returnPlayer.SavedFrom10meterGK += 1
                        };

                        var bb = stat.Type switch
                        {
                            SummaryType.Goal => returnPlayer.TotalPoints += 5,
                            SummaryType.Assist => returnPlayer.TotalPoints += 2,
                            SummaryType.OwnGoal => returnPlayer.TotalPoints -= 3,
                            SummaryType.YellowCards => returnPlayer.TotalPoints -= 2,
                            SummaryType.RedCards => returnPlayer.TotalPoints -= 7,
                            SummaryType.MissedPenalty => returnPlayer.TotalPoints -= 3,
                            SummaryType.Missed10meter => returnPlayer.TotalPoints -= 2,
                            SummaryType.CleanSheetGK => returnPlayer.TotalPoints += 5,
                            SummaryType.FourSavesGK => returnPlayer.TotalPoints += 1,
                            SummaryType.SavedFromPenaltyGK => returnPlayer.TotalPoints += 3,
                            SummaryType.SavedFrom10meterGK => returnPlayer.TotalPoints += 2
                        };
                    }
                }
            }

            returnPlayer.TotalPoints += returnPlayer.TotalMatches * 2; // match attened
            returnPlayer.GoalsPerMatch = returnPlayer.Goals == 0 ? 0 :
                (double)returnPlayer.Goals / (double)returnPlayer.TotalMatches;
            returnPlayer.WinPerMatch = returnPlayer.Wins == 0 ? 0 :
                (double)returnPlayer.Wins / (double)returnPlayer.TotalMatches * 100.0;
            returnPlayer.Player = _mapper.Map<PlayerDto>(player);
            returnPlayer.Team = _mapper.Map<TeamDto>(player.Team);
            return returnPlayer;
        }

        public IEnumerable<PlayerStatsDto> GetTopGoalscorer()
        {
            var playerIds = _context.Players.AsNoTracking().Select(x => x.Id).ToList();
            List<PlayerStatsDto> playersStats = new();
            foreach (var playerId in playerIds)
            {
                PlayerStatsDto returnPlayer = new();
                returnPlayer = GetPlayerStats(playerId);
                playersStats.Add(returnPlayer);
            }
            return playersStats.OrderByDescending(x => x.Goals).Take(5).ToList();
        }

        public IEnumerable<PlayerStatsDto> GetTopAssitent()
        {
            var playerIds = _context.Players.AsNoTracking().Select(x => x.Id).ToList();
            List<PlayerStatsDto> playersStats = new();
            foreach (var playerId in playerIds)
            {
                PlayerStatsDto returnPlayer = new();
                returnPlayer = GetPlayerStats(playerId);
                playersStats.Add(returnPlayer);
            }
            return playersStats.OrderByDescending(x => x.Assists).Take(5).ToList();
        }

        public decimal RecalculatePlayerPointsForOverallDreamTeam(int id)
        {
            var player = _context.Players.Include(x => x.Team).Include(x => x.Matches).ThenInclude(x => x.Summaries).ThenInclude(x => x.Player).Include(x => x.Matches).ThenInclude(x => x.League).FirstOrDefault(x => x.Id == id);
            decimal totalPointsMulitlieToCoefficinet = 0;

            foreach (var match in player.Matches)
            {
                foreach (var summary in match.Summaries)
                {
                    if (summary.Player.Id == id)
                    {
                        var bb = summary.Type switch
                        {
                            SummaryType.Goal => totalPointsMulitlieToCoefficinet += 5 * match.League.Coefficient,
                            SummaryType.Assist => totalPointsMulitlieToCoefficinet += 2 * match.League.Coefficient,
                            SummaryType.OwnGoal => totalPointsMulitlieToCoefficinet -= 3 * match.League.Coefficient,
                            SummaryType.YellowCards => totalPointsMulitlieToCoefficinet -= 2 * match.League.Coefficient,
                            SummaryType.RedCards => totalPointsMulitlieToCoefficinet -= 7 * match.League.Coefficient,
                            SummaryType.MissedPenalty => totalPointsMulitlieToCoefficinet -= 3 * match.League.Coefficient,
                            SummaryType.Missed10meter => totalPointsMulitlieToCoefficinet -= 2 * match.League.Coefficient,
                            SummaryType.CleanSheetGK => totalPointsMulitlieToCoefficinet += 5 * match.League.Coefficient,
                            SummaryType.FourSavesGK => totalPointsMulitlieToCoefficinet += 1 * match.League.Coefficient,
                            SummaryType.SavedFromPenaltyGK => totalPointsMulitlieToCoefficinet += 3 * match.League.Coefficient,
                            SummaryType.SavedFrom10meterGK => totalPointsMulitlieToCoefficinet += 2 * match.League.Coefficient
                        };
                    }
                }
                totalPointsMulitlieToCoefficinet += 2 * match.League.Coefficient;
            }
            return totalPointsMulitlieToCoefficinet;
        }

        private int CalculatePlayerWins(Player player)
        {
            return player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals > x.AwayTeamGoals) +
                   player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals > x.HomeTeamGoals);
        }

        private int CalculatePlayerLosts(Player player)
        {
            return player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals < x.AwayTeamGoals) +
                   player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals < x.HomeTeamGoals);
        }

        private int CalculatePlayerDraws(Player player)
        {
            return player.Matches.Count(x => x.HomeTeamId == player.Team.Id && x.HomeTeamGoals == x.AwayTeamGoals) +
                   player.Matches.Count(x => x.AwayTeamId == player.Team.Id && x.AwayTeamGoals == x.HomeTeamGoals);
        }
    }
}
