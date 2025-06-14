using Microsoft.EntityFrameworkCore;
using SmartClinic.Data;
using SmartClinic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Repository
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Doctor?> GetDoctorByIdAsync(Guid id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DoctorId == id);
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddDoctorAsync(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            try
            {
                await _context.Doctors.AddAsync(doctor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to add doctor.", ex);
            }
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            try
            {
                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to update doctor.", ex);
            }
        }

        public async Task<bool> DeleteDoctorAsync(Guid id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return false;

            try
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to delete doctor.", ex);
            }
        }
    }
}