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
        public ICollection<Team> Teams { get; set; }
    }
}
