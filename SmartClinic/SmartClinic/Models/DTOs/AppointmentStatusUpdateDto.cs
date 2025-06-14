using System;
using SmartClinic.Models.Entities;

namespace SmartClinic.Models.DTOs
{
    public class AppointmentStatusUpdateDto
    {
        public Guid AppointmentId { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}