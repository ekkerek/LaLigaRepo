namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class TeamDto
    {
        public TeamDto()
        {
            Players = new List<PlayerDto>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ParticipantOf { get; set; }
        public string? LogoSrc { get; set; }

        public List<PlayerDto> Players { get; set; }
    }
}
