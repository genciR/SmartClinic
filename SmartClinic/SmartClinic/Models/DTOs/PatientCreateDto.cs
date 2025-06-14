using System;

namespace SmartClinic.Models.DTOs
{
    public class PatientCreateDto
    {
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}