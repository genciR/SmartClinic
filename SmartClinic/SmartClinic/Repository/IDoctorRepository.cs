using SmartClinic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Repository
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetDoctorByIdAsync(Guid id);
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task AddDoctorAsync(Doctor doctor);
        Task UpdateDoctorAsync(Doctor doctor);
        Task<bool> DeleteDoctorAsync(Guid id);
    }
}