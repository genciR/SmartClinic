using Microsoft.EntityFrameworkCore;
using SmartClinic.Data;
using SmartClinic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClinic.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid id)
        {
            return await _context.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(Guid doctorId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);
            return await _context.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorId == doctorId && a.StartTime >= startOfDay && a.StartTime < endOfDay)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            try
            {
                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to add appointment.", ex);
            }
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            try
            {
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to update appointment.", ex);
            }
        }
    }
}