using Microsoft.EntityFrameworkCore;
using SmartClinic.Data;
using SmartClinic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Repository
{
    public class MedicalHistoryRepository : IMedicalHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalHistoryRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<MedicalHistory?> GetMedicalHistoryByIdAsync(Guid historyId)
        {
            return await _context.MedicalHistories
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.HistoryId == historyId);
        }

        public async Task<List<MedicalHistory>> GetMedicalHistoryByPatientIdAsync(Guid patientId)
        {
            return await _context.MedicalHistories
                .AsNoTracking()
                .Where(m => m.PatientId == patientId)
                .ToListAsync();
        }

        public async Task AddMedicalHistoryAsync(MedicalHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            try
            {
                await _context.MedicalHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to add medical history.", ex);
            }
        }

        public async Task UpdateMedicalHistoryAsync(MedicalHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            try
            {
                _context.MedicalHistories.Update(history);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to update medical history.", ex);
            }
        }

        public async Task<bool> DeleteMedicalHistoryAsync(Guid historyId)
        {
            var history = await _context.MedicalHistories.FindAsync(historyId);
            if (history == null)
                return false;

            try
            {
                _context.MedicalHistories.Remove(history);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Failed to delete medical history.", ex);
            }
        }
    }
}