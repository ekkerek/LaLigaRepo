using System.ComponentModel.DataAnnotations;

namespace LA_LIGA_REKREATIVO.Server.Models
{
    public class Player
    {
        public Player()
        {
            Matches = new List<Match>();
        }
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Team Team { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
