using System;

namespace SmartClinic.Models.DTOs
{
    public class MedicalHistoryDto
    {
        public Guid HistoryId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid AppointmentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Notes { get; set; }
    }
}