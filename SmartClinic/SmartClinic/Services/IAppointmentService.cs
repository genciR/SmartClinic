using SmartClinic.Models.DTOs;

namespace SmartClinic.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> ScheduleAppointmentAsync(AppointmentCreateDto createDto);
    }
}
