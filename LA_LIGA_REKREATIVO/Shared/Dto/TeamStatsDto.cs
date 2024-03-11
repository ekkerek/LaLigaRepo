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
        public int TotalPoints { get; set; }
        public TeamDto Team { get; set; }
    }
}
