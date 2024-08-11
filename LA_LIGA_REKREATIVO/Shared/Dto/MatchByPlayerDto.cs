using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class MatchByPlayerDto : MatchByTeamDto
    {
        public int TotalPoints { get; set; }
    }
}
