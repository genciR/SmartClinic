using SmartClinic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Repository
{
    public interface IMedicalHistoryRepository
    {
        Task<MedicalHistory?> GetMedicalHistoryByIdAsync(Guid historyId);
        Task<List<MedicalHistory>> GetMedicalHistoryByPatientIdAsync(Guid patientId);
        Task AddMedicalHistoryAsync(MedicalHistory history);
        Task UpdateMedicalHistoryAsync(MedicalHistory history);
        Task<bool> DeleteMedicalHistoryAsync(Guid historyId);
    }
}