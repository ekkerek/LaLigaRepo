namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class LeagueStatsDto
    {
        public LeagueStatsDto()
        {
            TeamStats = new();
        }
        public List<TeamStatsDto> TeamStats { get; set; }
        public LeagueDto League { get; set; }
    }
}
