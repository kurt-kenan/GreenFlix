﻿namespace GreenFlix.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // şifre hashli tutulacak
        public bool IsAdmin { get; set; }
    }

}
