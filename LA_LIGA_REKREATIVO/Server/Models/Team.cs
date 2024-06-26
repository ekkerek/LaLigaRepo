﻿namespace LA_LIGA_REKREATIVO.Server.Models
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
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Player> Players { get; set; }
        public ICollection<League> Leagues { get; set; }
    }
}
