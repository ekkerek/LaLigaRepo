namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class TeamStatsDto
    {
        //public int Id { get; set; }
        //public int Season { get; set; }
        public int GamePlayed { get; set; }
        public int Wins { get; set; }
        public int Losts { get; set; }
        public int Draws { get; set; }
        public int Goals { get; set; }
        public int GoalsConceded { get; set; }
        public int GoalDifference { get; set; }
        public int TotalPoints { get; set; }
        public decimal PointsByCoefficient { get; set; }
        public TeamDto Team { get; set; }
        public int OrderId { get; set; }
        public List<MatchDto> Matches { get; set; }
    }

    public class TeamStatsHistoryDto : TeamStatsDto
    {
        public int Year { get; set; }
        public string? PlayoffRoundFinished { get; set; }
    }
}
