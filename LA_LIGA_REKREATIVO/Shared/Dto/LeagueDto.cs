﻿namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class LeagueDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string? Logo { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public double Coefficient { get; set; }
    }
}
