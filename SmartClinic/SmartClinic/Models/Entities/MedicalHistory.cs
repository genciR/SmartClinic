using System;
using System.ComponentModel.DataAnnotations;

namespace SmartClinic.Models.Entities
{
    public class MedicalHistory
    {
        [Key]
        public Guid HistoryId { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public Guid AppointmentId { get; set; }

        public string? Diagnosis { get; set; }

        public string? Prescriptions { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
        public Appointment? Appointment { get; set; }
    }
}