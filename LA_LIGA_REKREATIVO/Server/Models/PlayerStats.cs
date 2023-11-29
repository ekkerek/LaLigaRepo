namespace LA_LIGA_REKREATIVO.Server.Models
{
    public class PlayerStats
    {
        public int Id { get; set; }
        public int Season { get; set; }
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

        public Player Player { get; set; }
    }
}
