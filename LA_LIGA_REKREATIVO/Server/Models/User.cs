﻿namespace LA_LIGA_REKREATIVO.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public bool IsApporved { get; set; }
    }
}
