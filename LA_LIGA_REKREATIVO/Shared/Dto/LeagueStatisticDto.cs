namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class LeagueStatisticDto
    {
        public int TotalMatches { get; set; }
        public int NumberOfPlayers { get; set; }
        public int TotalGoals { get; set; }
        public int TotalAssists { get; set; }
        public decimal GoalsPerMatch { get; set; }
        public decimal AssistsPerMatch { get; set; }
    }
}
