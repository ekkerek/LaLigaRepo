namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class LeagueDto
    {
        public LeagueDto()
        {
            Teams = new List<TeamDto>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public List<TeamDto> Teams { get; set; }

        public void AddNewTeams(List<TeamDto> teams)
        {
            Teams.AddRange(teams);
        }
    }

}
