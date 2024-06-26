﻿using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
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
            //var bestGk = GetBestGkOverall();
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
            //SetOrderId(playersStats);
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
            //var bestGk = GetBestGkOverall();
            //SetOrderId(playersStats);
            return playersStats;
        }

        public List<PlayerStatsDto> GetPlayersStats23(int leagueId)
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
                returnPlayer = GetPlayerStats(playerId);
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

        public PlayerStatsDto GetPlayerStats(int id)
        {
            var player = _context.Players.Include(x => x.Team).Include(x => x.Matches).ThenInclude(x => x.Summaries).ThenInclude(x => x.Player).FirstOrDefault(x => x.Id == id);
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
            //returnPlayer.TotalPoints += returnPlayer.Wins * 2; // team win
            returnPlayer.GoalsPerMatch = returnPlayer.Goals == 0 ? 0 :
                (double)returnPlayer.Goals / (double)returnPlayer.TotalMatches;
            returnPlayer.WinPerMatch = returnPlayer.Wins == 0 ? 0 :
                (double)returnPlayer.Wins / (double)returnPlayer.TotalMatches * 100.0;
            returnPlayer.Player = _mapper.Map<PlayerDto>(player);
            returnPlayer.Team = _mapper.Map<TeamDto>(player.Team);
            return returnPlayer;
        }
    }
}
