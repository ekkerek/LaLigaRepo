using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public enum PlayOffRound
    {
        [Display(Name = "šesnaestina finala")]
        RoundOf32,
        [Display(Name = "osimna finala")]
        RoundOf16,
        [Display(Name = "četvrtfinale")]
        QuarterFinals,
        [Display(Name = "polufinale")]
        SemiFinals,
        [Display(Name = "Finale")]
        Final
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .FirstOrDefault()?
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name ?? enumValue.ToString();
        }
    }
}
