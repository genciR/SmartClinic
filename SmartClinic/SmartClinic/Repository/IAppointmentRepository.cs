using SmartClinic.Models.Entities;

namespace SmartClinic.Repository
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId);
        Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(Guid doctorId, DateTime date);
        Task<List<Appointment>> GetAppointmentsByPatientAndDateAsync(Guid patientId, DateTime date);
        Task AddAppointmentAsync(Appointment appointment);

        Task<bool> PatientExistsAsync(Guid patientId);
    }
}
