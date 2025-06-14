using SmartClinic.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public interface IDoctorService
    {
        Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto createDto);
        Task<DoctorDto> GetDoctorByIdAsync(Guid id);
        Task<List<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto> UpdateDoctorAsync(Guid id, DoctorUpdateDto updateDto);
        Task<bool> DeleteDoctorAsync(Guid id);
        Task<DoctorAvailabilityDto> GetDoctorAvailabilityAsync(Guid doctorId, DateTime date);
    }
}