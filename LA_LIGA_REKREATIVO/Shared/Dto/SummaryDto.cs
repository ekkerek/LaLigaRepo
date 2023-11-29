namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class SummaryDto
    {
        public int Id { get; set; }
        public int Time { get; set; }
        public PlayerDto Player { get; set; }
        public SummaryType Type
        {
            get; set;
        }
    }
}
