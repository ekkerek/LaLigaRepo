using LA_LIGA_REKREATIVO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class MatchDto
    {
        public int Id { get; set; }
        public DateTime GameTime { get; set; }
        public int GameRound { get; set; }
        public string GamePlace { get; set; }
        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }
        public TeamDto HomeTeam { get; set; }
        public TeamDto AwayTeam { get; set; }
        public List<SummaryDto> Summaries { get; set; }
        public List<PlayerDto> Players { get; set; }
    }
}
