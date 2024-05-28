using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class LeagueMatches
    {
        public LeagueMatches()
        {
            Matches = new();
        }
        public LeagueDto LeagueDto { get; set; }
        public List<MatchDto> Matches { get; set; }
    }
}
