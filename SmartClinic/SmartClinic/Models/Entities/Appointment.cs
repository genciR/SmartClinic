using System;
using System.ComponentModel.DataAnnotations;

namespace SmartClinic.Models.Entities
{
    public enum AppointmentStatus
    {
        Scheduled,
        Cancelled,
        Completed
    }

    public class Appointment
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}