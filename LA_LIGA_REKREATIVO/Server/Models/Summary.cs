using LA_LIGA_REKREATIVO.Shared.Dto;

namespace LA_LIGA_REKREATIVO.Server.Models;

public class Summary
{
    public int Id { get; set; }
    public int Time { get; set; }
    public bool IsDeleted { get; set; }
    public Player Player { get; set; }
    public Match Match { get; set; }
    public SummaryType Type { get; set; }
}
