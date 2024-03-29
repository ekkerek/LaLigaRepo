namespace LA_LIGA_REKREATIVO.Server.Models
{
    public class League
    {
        public League()
        {
            Teams = new List<Team>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string? Logo { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Team> Teams { get; set; }
    }
}
