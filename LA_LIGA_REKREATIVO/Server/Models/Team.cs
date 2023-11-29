using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LA_LIGA_REKREATIVO.Server.Models
{
    public class Team
    {
        public Team()
        {
            Players = new List<Player>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ParticipantOf { get; set; }
        public string? LogoSrc { get; set; }

        public ICollection<Player> Players { get; set; }
    }
}
