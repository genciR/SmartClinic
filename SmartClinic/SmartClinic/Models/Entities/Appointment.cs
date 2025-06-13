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
    public enum AppointmentType
    {
        Standard,    // 30 minutes
        Extended,    // 60 minutes
        Emergency    // 15 minutes
    }
    public class Appointment
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public AppointmentType Type { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}