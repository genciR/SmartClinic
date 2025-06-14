using SmartClinic.Models.Entities;
using System;
using System.Threading.Tasks;

namespace SmartClinic.Repository
{
    public interface IPatientRepository
    {
        Task<bool> PatientExistsAsync(Guid id);
        Task AddPatientAsync(Patient patient);
    }
}