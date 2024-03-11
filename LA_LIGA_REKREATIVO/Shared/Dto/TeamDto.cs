namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class TeamDto
    {
        public TeamDto()
        {
            Players = new();
            Leagues = new();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ParticipantOf { get; set; }
        public string? LogoSrc { get; set; }
        public List<LeagueDto> Leagues { get; set; }

        public List<PlayerDto> Players { get; set; }
    }
}
