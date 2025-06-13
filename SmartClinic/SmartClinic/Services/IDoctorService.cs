using SmartClinic.Models.DTOs;

public interface IDoctorService
{
    Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto createDto);
    Task<DoctorDto?> GetDoctorByIdAsync(Guid id);
    Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
    Task<DoctorDto?> UpdateDoctorAsync(Guid doctorId, DoctorUpdateDto updateDto);
    Task<bool> DeleteDoctorAsync(Guid id);
    Task<DoctorAvailabilityDto> GetDoctorAvailabilityAsync(Guid doctorId, DateTime date);
}