namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Picture { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int TeamId { get; set; }
    }
}
