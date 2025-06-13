using SmartClinic.Models.Entities;

namespace SmartClinic.Models.DTOs
{
    public class AppointmentCreateDto
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public AppointmentType Type { get; set; }
        public string? Notes { get; set; }
    }
}
