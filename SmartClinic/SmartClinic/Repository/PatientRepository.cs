using Microsoft.EntityFrameworkCore;
using SmartClinic.Data;
using SmartClinic.Models.Entities;
using System;
using System.Threading.Tasks;

namespace SmartClinic.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> PatientExistsAsync(Guid id)
        {
            return await _context.Patients.AnyAsync(p => p.PatientId == id);
        }

        public async Task AddPatientAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            try
            {
                await _context.Patients.AddAsync(patient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to add patient.", ex);
            }
        }
    }
}