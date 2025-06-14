using SmartClinic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Repository
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetAppointmentByIdAsync(Guid id);
        Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(Guid doctorId, DateTime date);
        Task<List<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);
        Task AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
    }
}