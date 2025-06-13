namespace SmartClinic.Models.DTOs
{
    public class DoctorDto
    {
        public Guid DoctorId { get; set; }
        public string Name { get; set; } 
        public string Specialization { get; set; }
        public List<DayOfWeek> WorkingDays { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }
    }
}
