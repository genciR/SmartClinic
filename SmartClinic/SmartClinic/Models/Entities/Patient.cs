using System;
using System.ComponentModel.DataAnnotations;

namespace SmartClinic.Models.Entities
{
    public class Patient
    {
        [Key]
        public Guid PatientId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public User? User { get; set; }
    }
}