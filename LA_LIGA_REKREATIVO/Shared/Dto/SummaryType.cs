using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public enum SummaryType
    {
        [Display(Name = "Goal 5 pts")]
        Goal,
        [Display(Name = "Assist 2 pts")]
        Assist,
        [Display(Name = "Own Goal -3 pts")]
        OwnGoal,
        [Display(Name = "Yellow Card -2 pts")]
        YellowCards,
        [Display(Name = "Red Card -7 pts")]
        RedCards,
        [Display(Name = "Missed Penalty -3 pts")]
        MissedPenalty,
        [Display(Name = "Missed 10 meter -2 pts")]
        Missed10meter,
        [Display(Name = "Clean Sheet GK 5 pts")]
        CleanSheetGK,
        [Display(Name = "Four Saves GK 1 pts")]
        FourSavesGK,
        [Display(Name = "Saved From Penalty GK 3 pts")]
        SavedFromPenaltyGK,
        [Display(Name = "Saved From 10 meter GK 2 pts")]
        SavedFrom10meterGK

    }
}
