using System;
using System.Collections.Generic;

namespace SmartClinic.Models.DTOs
{
    public class DoctorAvailabilityDto
    {
        public DateTime Date { get; set; }
        public List<TimeSlotDto> AvailableSlots { get; set; }
    }
}