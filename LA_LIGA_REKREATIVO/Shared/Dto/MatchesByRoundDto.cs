using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class MatchesByRoundDto
    {
        public MatchesByRoundDto()
        {
            Matches = new();
        }

        public int Round { get; set; }
        public List<MatchDto> Matches { get; set; }
    }
}
