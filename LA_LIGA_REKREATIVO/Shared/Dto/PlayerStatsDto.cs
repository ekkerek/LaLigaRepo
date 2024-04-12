namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class PlayerStatsDto
    {
        //public int Id { get; set; }
        //public int Season { get; set; }
        //General
        public int TotalMatches { get; set; }
        public int TotalPoints { get; set; }
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
        public int GoalsFromPenalty { get; set; }
        public int GoalsFrom10meter { get; set; }

        public PlayerDto Player { get; set; }
        public TeamDto Team { get; set; }
    }
}
