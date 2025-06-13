namespace SmartClinic.Models.Entities
{
    public class Patient
    {
        public Guid PatientId { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
