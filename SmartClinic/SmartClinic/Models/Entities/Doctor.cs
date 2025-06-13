using System;
using System.Collections.Generic;

namespace SmartClinic.Models.Entities
{
    public class Doctor
    {
        public Guid DoctorId { get; set; } // Primary Key, FK to User.Id
        public Guid UserId { get; set; } // FK to User
        public User? User { get; set; } // Navigation property
        public string Specialization { get; set; }
        public List<DayOfWeek> WorkingDays { get; set; } = new List<DayOfWeek>();
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? BreakStartTime { get; set; } // Nullable
        public TimeSpan? BreakEndTime { get; set; }
    }
}