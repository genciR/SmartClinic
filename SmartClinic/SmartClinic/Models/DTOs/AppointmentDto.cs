using SmartClinic.Models.Entities;

namespace SmartClinic.Models.DTOs
{
    public class AppointmentDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public AppointmentType Type { get; set; }
        public string? Notes { get; set; }
    }
}
