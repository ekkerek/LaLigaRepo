using LA_LIGA_REKREATIVO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class PlayerStatsDto
    {
        public int Id { get; set; }
        public int Season { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int OwnGoals { get; set; }
        public double GoalsPerMatch { get; set; }
        public double WinPerMatch { get; set; }

        //Discipline
        public int YellowCards { get; set; }
        public int RedCards { get; set; }

        //Penals
        public int GoalsFromPenalty { get; set; }
        public int GoalsFrom10meter { get; set; }

        public PlayerDto Player { get; set; }
    }
}
