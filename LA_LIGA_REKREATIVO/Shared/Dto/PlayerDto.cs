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
        public bool IsGk { get; set; }

        private string fullName;
        public string FullName
        {
            get
            {
                if (fullName == null)
                {
                    fullName = GetFullName(); // slow
                }
                return fullName;
            }
            set { }
        }

        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }
    }
}
