using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LA_LIGA_REKREATIVO.Shared.Models
{
    public enum SummaryType
    {
        Goal,
        Assist,
        OwnGoal,
        GoalsFromPenalty,
        GoalsFrom10meter,
        YellowCards,
        RedCards
    }

    public class Summary
    {
        public int Id { get; set; }
        public int Time { get; set; }
        public Player Player { get; set; }
        public Match Match { get; set; }
        public SummaryType Type { get; set; }
    }
}
