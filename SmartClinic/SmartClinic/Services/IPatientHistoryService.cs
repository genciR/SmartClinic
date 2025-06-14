using SmartClinic.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public interface IPatientHistoryService
    {
        Task<List<MedicalHistoryDto>> GetMedicalHistoryByPatientIdAsync(Guid patientId);
    }
}