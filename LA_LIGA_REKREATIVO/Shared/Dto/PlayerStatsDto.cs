namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class PlayerStatsDto
    {
        //General
        public int TotalMatches { get; set; }
        public double TotalPoints { get; set; }
        public int Wins { get; set; }
        public int Losts { get; set; }
        public int Draws { get; set; }

        //Game
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int OwnGoals { get; set; }
        public double GoalsPerMatch { get; set; }
        public double WinPerMatch { get; set; }

        //Discipline
        public int YellowCards { get; set; }
        public int RedCards { get; set; }

        //Penals
        public int MissedPenalty { get; set; }
        public int Missed10meter { get; set; }

        //
        public int CleanSheetGK { get; set; }
        public int FourSavesGK { get; set; }
        public int SavedFromPenaltyGK { get; set; }
        public int SavedFrom10meterGK { get; set; }

        public PlayerDto Player { get; set; }
        public TeamDto? Team { get; set; }

        public int OrderId { get; set; }
    }

    public class PlayerStatsHistoryDto : PlayerStatsDto
    {
        public int Year { get; set; }
    }
}
