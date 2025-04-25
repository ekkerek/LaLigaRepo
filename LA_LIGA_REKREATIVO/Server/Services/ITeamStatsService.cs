using LA_LIGA_REKREATIVO.Shared.Dto;

namespace LA_LIGA_REKREATIVO.Server.Services
{
    public interface ITeamStatsService
    {
        List<TeamStatsDto> GetStandingsByLeague(int id);
        List<TeamStatsDto> GetCommonStanding();
        List<TeamStatsHistoryDto> GetTeamStatsForNonActiveLeagues(int teamId);
    }
}
