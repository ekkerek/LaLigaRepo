﻿using LA_LIGA_REKREATIVO.Shared;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Mvc;

namespace LA_LIGA_REKREATIVO.Server.Services
{
    public interface IPlayerStatsService
    {
        List<PlayerStatsDto> GetStatsByGoal(int leagueId);
        List<PlayerStatsDto> GetStatsByAssists(int leagueId);
        List<PlayerStatsDto> GetDreamTeamByLeague(int leagueId);
        PlayerStatsDto GetPlayerStats(int id);
        List<PlayerStatsDto> GetPlayersStats23(int leagueId);
    }
}
