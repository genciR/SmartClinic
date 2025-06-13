using Microsoft.EntityFrameworkCore;
using SmartClinic.Data;
using SmartClinic.Models.Entities;
using SmartClinic.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClinic.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _context.Appointments.FindAsync(appointmentId);
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(Guid doctorId, DateTime date)
        {
            var dateUtc = date.Kind == DateTimeKind.Unspecified
         ? DateTime.SpecifyKind(date.Date, DateTimeKind.Utc)
         : date.Date.ToUniversalTime();

            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                    && a.StartTime.Date == dateUtc
                    && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByPatientAndDateAsync(Guid patientId, DateTime date)
        {
            var dateUtc = date.Kind == DateTimeKind.Unspecified
        ? DateTime.SpecifyKind(date.Date, DateTimeKind.Utc)
        : date.Date.ToUniversalTime();
            return await _context.Appointments
                .Where(a => a.PatientId == patientId
                    && a.StartTime.Date == dateUtc
                    && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> PatientExistsAsync(Guid patientId)
        {
            return await _context.Patients.AnyAsync(p => p.PatientId == patientId);
        }
    }
}