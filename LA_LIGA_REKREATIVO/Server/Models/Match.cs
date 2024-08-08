using LA_LIGA_REKREATIVO.Shared.Dto;

namespace LA_LIGA_REKREATIVO.Server.Models
{
    public class Match
    {
        public Match()
        {
            Summaries = new List<Summary>();
            Players = new List<Player>();
        }

        public int Id { get; set; }
        public DateTime GameTime { get; set; }
        public int GameRound { get; set; }
        public string? GamePlace { get; set; }
        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsOfficialResult { get; set; }
        public bool IsMatchSuspended { get; set; }
        public int? HomeTeamNegativePoints { get; set; }
        public int? AwayTeamNegativePoints { get; set; }
        public bool IsPlayOff { get; set; }
        public PlayOffRound? PlayOffRound { get; set; }
        public ICollection<Summary> Summaries { get; set; }
        public ICollection<Player> Players { get; set; }
        public League League { get; set; }
    }
}
