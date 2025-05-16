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

        public List<PlayerStatsDto> GetDreamTeamByLeagueRound(int leagueId, int round, PlayOffRound? playOffRound = null)
        {
            List<Match>? matches = null;
            // Get all matches for the specified league and round
            if (playOffRound is null)
            {
                matches = _context.Matches.Include(x => x.League)
                                          .Include(x => x.Summaries)
                                              .ThenInclude(s => s.Player)
                                                .ThenInclude(x => x.Team)
                                          .Where(x => x.League.Id == leagueId && x.GameRound == round)
                                          .ToList();
            }
            else
            {
                matches = _context.Matches.Include(x => x.League)
                                          .Include(x => x.Summaries)
                                              .ThenInclude(s => s.Player)
                                                .ThenInclude(x => x.Team)
                                          .Where(x => x.League.Id == leagueId && x.PlayOffRound == playOffRound)
                                          .ToList();
            }

            var teams = _context.Teams;

            // Get all field players and GKs
            var summaries = matches.SelectMany(m => m.Summaries);

            var players = summaries
                .Select(s => s.Player)
                .Where(x => !x.IsGk)
                .Distinct()
                .ToList();

            var goalkeepers = summaries
                .Select(s => s.Player)
                .Where(x => x.IsGk)
                .Distinct()
                .ToList();

            // Calculate total points for field players
            var allFieldStats = players
                .Select(player => new PlayerStatsDto
                {
                    Player = new PlayerDto
                    {
                        Id = player.Id,
                        FirstName = player.FirstName,
                        LastName = player.LastName,
                        Picture = player.Picture
                    },
                    TotalPoints = CalculateTotalPoints(player.Id, matches) + 2,
                    Team = _mapper.Map<TeamDto>(teams.FirstOrDefault(x => x.Id == player.Team.Id))
                })
                .OrderByDescending(p => p.TotalPoints)
                .Take(8)
                .ToList();

            // Calculate total points for GKs
            var allGkStats = goalkeepers
                .Select(gk => new PlayerStatsDto
                {
                    Player = new PlayerDto
                    {
                        Id = gk.Id,
                        FirstName = gk.FirstName,
                        LastName = gk.LastName,
                        IsGk = true,
                        Picture = gk.Picture
                    },
                    TotalPoints = CalculateTotalPoints(gk.Id, matches),
                    Team = _mapper.Map<TeamDto>(teams.FirstOrDefault(x => x.Id == gk.Team.Id))
                })
                .OrderByDescending(p => p.TotalPoints)
                .Take(2)
                .ToList();

            // Split into groups
            var first4Players = allFieldStats.Take(4);
            var next4Players = allFieldStats.Skip(4).Take(4);
            var firstGk = allGkStats.Take(1);
            var secondGk = allGkStats.Skip(1).Take(1);

            // Final order: 4 players + best GK + 4 players + second-best GK
            var dreamTeam = new List<PlayerStatsDto>();
            dreamTeam.AddRange(first4Players);
            dreamTeam.AddRange(firstGk);
            dreamTeam.AddRange(next4Players);
            dreamTeam.AddRange(secondGk);

            return dreamTeam;
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
            return playersStats.OrderByDescending(x => x.TotalPoints).ToList();
        }

        public PlayerStatsDto GetPlayerStats(int id, int leagueId = 0)
        {
            var playerStats = Enumerable.Empty<ICollection<Summary>>();

            var player = _context.Players
                                    .Include(x => x.Team)
                                    .Include(x => x.Matches.Where(m => m.League.IsActive)) // Fetch only active league matches
                                        .ThenInclude(x => x.Summaries)
                                            .ThenInclude(x => x.Player)
                                    .Include(x => x.Matches.Where(m => m.League.IsActive)) // Fetch only active league matches
                                        .ThenInclude(x => x.League)
                                    .FirstOrDefault(x => x.Id == id);

            PlayerStatsDto returnPlayer = new PlayerStatsDto();
            if (leagueId != 0)
            {
                playerStats = player.Matches.Where(x => x.League.IsActive && x.League.Id == leagueId).Select(x => x.Summaries);
                returnPlayer.TotalMatches = player.Matches.Where(x => x.League.IsActive && x.League.Id == leagueId).Count();
            }
            else
            {
                playerStats = player.Matches.Where(x => x.League.IsActive).Select(x => x.Summaries);
                returnPlayer.TotalMatches = player.Matches.Where(x => x.League.IsActive).Count();
            }

            if (returnPlayer.TotalMatches == 0)
            {
                var testNewPlayerStats = new PlayerStatsDto();
                testNewPlayerStats.Player = _mapper.Map<PlayerDto>(player);
                testNewPlayerStats.Team = _mapper.Map<TeamDto>(player.Team);
                return testNewPlayerStats;
            }

            returnPlayer.Wins = CalculatePlayerWins(player);

            returnPlayer.Draws = CalculatePlayerDraws(player);

            returnPlayer.Losts = CalculatePlayerLosts(player);

            UpdatePlayerStats(returnPlayer, playerStats, id);

            returnPlayer.TotalPoints = CalculateTotalPoints(id, player.Matches);

            returnPlayer.TotalPoints += returnPlayer.TotalMatches * 2; // match attened // TODO: move it to CalculateTotalPoints method
            returnPlayer.GoalsPerMatch = returnPlayer.Goals == 0 ? 0 :
                (double)returnPlayer.Goals / (double)returnPlayer.TotalMatches;
            returnPlayer.WinPerMatch = returnPlayer.Wins == 0 ? 0 :
                (double)returnPlayer.Wins / (double)returnPlayer.TotalMatches * 100.0;
            returnPlayer.Player = _mapper.Map<PlayerDto>(player);
            returnPlayer.Team = _mapper.Map<TeamDto>(player.Team);
            return returnPlayer;
        }


        public List<PlayerStatsHistoryDto> GetPlayerStatsForNonActiveLeagues(int playerId)
        {
            var player = _context.Players
                .Include(x => x.Team)
                .Include(x => x.Matches.Where(m => !m.League.IsActive)) // Filter out active leagues
                    .ThenInclude(m => m.League)
                .Include(x => x.Matches.Where(m => !m.League.IsActive))
                    .ThenInclude(m => m.Summaries)
                        .ThenInclude(s => s.Player)
                .FirstOrDefault(x => x.Id == playerId);

            if (player == null || !player.Matches.Any())
                return new List<PlayerStatsHistoryDto>();

            return player.Matches
                .GroupBy(m => m.League.Year)
                .Select(g =>
                {
                    var stats = new PlayerStatsHistoryDto
                    {
                        Year = g.Key, // previously the dictionary key
                        TotalMatches = g.Count(),
                        Wins = CalculatePlayerWins(player), // optional: consider filtering by league group
                        Draws = CalculatePlayerDraws(player),
                        Losts = CalculatePlayerLosts(player),
                        TotalPoints = CalculateTotalPoints(playerId, g.ToList()) + 2 * g.Count(),
                        Goals = CalculatePlayerGoals(player),
                        Assists = CalculatePlayerAssists(player),
                        GoalsPerMatch = g.Any() ? (double)CalculatePlayerGoals(player) / g.Count() : 0,
                        WinPerMatch = g.Any() ? ((double)CalculatePlayerWins(player) / g.Count()) * 100.0 : 0,
                        Player = _mapper.Map<PlayerDto>(player),
                        Team = _mapper.Map<TeamDto>(player.Team),
                    };

                    return stats;
                })
                .ToList();
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

        private double CalculateTotalPoints(int playerId, ICollection<Match> matches)
        {
            return matches.SelectMany(x => x.Summaries)
                                        .Where(stat => stat.Player.Id == playerId)
                                        .Sum(stat => stat.Type switch
                                        {
                                            SummaryType.Goal => 5,
                                            SummaryType.Assist => 2,
                                            SummaryType.OwnGoal => -3,
                                            SummaryType.YellowCards => -2,
                                            SummaryType.RedCards => -7,
                                            SummaryType.MissedPenalty => -3,
                                            SummaryType.Missed10meter => -2,
                                            SummaryType.CleanSheetGK => 5,
                                            SummaryType.FourSavesGK => 1,
                                            SummaryType.SavedFromPenaltyGK => 3,
                                            SummaryType.SavedFrom10meterGK => 2,
                                            _ => 0
                                        });
        }

        private int CalculatePlayerGoals(Player player)
        {
            return player.Matches
                .SelectMany(m => m.Summaries)
                .Count(s => s.Player.Id == player.Id && s.Type == SummaryType.Goal);
        }

        private int CalculatePlayerAssists(Player player)
        {
            return player.Matches
                .SelectMany(m => m.Summaries)
                .Count(s => s.Player.Id == player.Id && s.Type == SummaryType.Assist);
        }

        private void UpdatePlayerStats(PlayerStatsDto playerStatsDto, IEnumerable<ICollection<Summary>> playerStats, int playerId)
        {
            foreach (var stats in playerStats)
            {
                foreach (var stat in stats)
                {
                    if (stat.Player.Id == playerId)
                    {
                        _ = stat.Type switch
                        {
                            SummaryType.Goal => playerStatsDto.Goals += 1,
                            SummaryType.Assist => playerStatsDto.Assists += 1,
                            SummaryType.OwnGoal => playerStatsDto.OwnGoals += 1,
                            SummaryType.YellowCards => playerStatsDto.YellowCards += 1,
                            SummaryType.RedCards => playerStatsDto.RedCards += 1,
                            SummaryType.MissedPenalty => playerStatsDto.MissedPenalty += 1,
                            SummaryType.Missed10meter => playerStatsDto.Missed10meter += 1,
                            SummaryType.CleanSheetGK => playerStatsDto.CleanSheetGK += 1,
                            SummaryType.FourSavesGK => playerStatsDto.FourSavesGK += 1,
                            SummaryType.SavedFromPenaltyGK => playerStatsDto.SavedFromPenaltyGK += 1,
                            SummaryType.SavedFrom10meterGK => playerStatsDto.SavedFrom10meterGK += 1,
                            _ => throw new NotImplementedException()
                        };
                    }
                }
            }
        }
    }
}


