﻿using System;

namespace SmartClinic.Models.Entities
{
    public enum Role
    {
        Patient,
        Doctor,
        Admin
    }

    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}