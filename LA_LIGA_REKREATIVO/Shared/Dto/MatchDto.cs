﻿using System.ComponentModel.DataAnnotations;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class MatchDto
    {
        public MatchDto()
        {
            Summaries = new();
            Players = new();
            HomeTeam = new();
            AwayTeam = new();
            League = new();
        }

        public int Id { get; set; }
        public DateTime GameTime { get; set; }
        [Required]
        public int GameRound { get; set; }
        public string GamePlace { get; set; }
        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }
        public TeamDto HomeTeam { get; set; }
        public TeamDto AwayTeam { get; set; }
        public List<SummaryDto> Summaries { get; set; }
        public List<PlayerDto> Players { get; set; }
        public LeagueDto League { get; set; }
    }
}
