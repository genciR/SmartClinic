using SmartClinic.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> ScheduleAppointmentAsync(AppointmentCreateDto createDto);
        Task<List<AppointmentDto>> GetAppointmentsByPatientIdAsync(Guid patientId);
        Task<AppointmentDto> UpdateAppointmentStatusAsync(AppointmentStatusUpdateDto updateDto);
    }
}