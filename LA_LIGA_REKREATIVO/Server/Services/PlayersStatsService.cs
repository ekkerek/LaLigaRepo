﻿using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.EntityFrameworkCore;

namespace LA_LIGA_REKREATIVO.Server.Services
{
    public class PlayersStatsService
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
            var playersStats = GetPlayerStats(leagueId).OrderByDescending(x => x.Goals).ToList();
            SetOrderId(playersStats);
            return playersStats;
        }

        public List<PlayerStatsDto> GetStatsByAssists(int leagueId)
        {
            var playersStats = GetPlayerStats(leagueId).OrderByDescending(x => x.Assists).ToList();
            SetOrderId(playersStats);
            return playersStats;
        }

        private void SetOrderId(List<PlayerStatsDto> playerStats)
        {
            int orderId = 1;
            playerStats.ForEach(x => { x.OrderId = orderId; orderId++; });
        }

        private List<PlayerStatsDto> GetPlayerStats(int leagueId)
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
                            SummaryType.YellowCards => returnPlayer.YellowCards += 1
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

            return playersStats.ToList();

        }
    }
}