﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class MatchesByGameTimeDto
    {
        public MatchesByGameTimeDto()
        {
            Matches = new();
        }

        public DateTime GameTime { get; set; }
        public List<MatchDto> Matches { get; set; }
    }
}
