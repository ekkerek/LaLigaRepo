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
        public string? Picture { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool IsGk { get; set; }
        public Team Team { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
