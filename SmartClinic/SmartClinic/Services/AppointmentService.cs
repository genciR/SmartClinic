using SmartClinic.Models.DTOs;
using SmartClinic.Models.Entities;
using SmartClinic.Repositories;
using SmartClinic.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly int _maxAppointmentsPerDay = 8;
        private readonly int _maxEmergencyAppointmentsPerDay = 2;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository, ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _logger = logger;
        }

        public async Task<AppointmentDto> ScheduleAppointmentAsync(AppointmentCreateDto createDto)
        {
            _logger.LogDebug("Scheduling appointment for DoctorId: {DoctorId}, PatientId: {PatientId}, StartTime: {StartTime}", createDto.DoctorId, createDto.PatientId, createDto.StartTime);

            // Ensure StartTime is UTC
            var startTime = createDto.StartTime.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(createDto.StartTime, DateTimeKind.Utc)
                : createDto.StartTime.ToUniversalTime();

            var doctor = await _doctorRepository.GetDoctorByIdAsync(createDto.DoctorId);
            if (doctor == null)
                throw new ArgumentException("Doctor not found.");

            if (!await _appointmentRepository.PatientExistsAsync(createDto.PatientId))
                throw new ArgumentException("Patient not found.");

            var dayOfWeek = startTime.DayOfWeek;
            if (!doctor.WorkingDays.Contains((DayOfWeek)(int)dayOfWeek))
                throw new ArgumentException("Doctor is not available on this day.");

            var time = startTime.TimeOfDay;
            if (time < TimeSpan.FromHours(9) || time >= TimeSpan.FromHours(17) || (time >= TimeSpan.FromHours(12) && time < TimeSpan.FromHours(13)))
                throw new InvalidOperationException("Appointment time is outside working hours or during break.");

            TimeSpan duration = createDto.Type switch
            {
                AppointmentType.Standard => TimeSpan.FromMinutes(30),
                AppointmentType.Extended => TimeSpan.FromMinutes(60),
                AppointmentType.Emergency => TimeSpan.FromMinutes(15),
                _ => throw new ArgumentException("Invalid appointment type.")
            };
            var endTime = startTime.Add(duration);

            var existingAppointments = await _appointmentRepository.GetAppointmentsByDoctorAndDateAsync(createDto.DoctorId, startTime);
            if (existingAppointments.Any(a => a.StartTime < endTime && a.EndTime > startTime))
                throw new InvalidOperationException("Requested time slot is already booked.");

            var standardExtendedCount = existingAppointments.Count(a => a.Type == AppointmentType.Standard || a.Type == AppointmentType.Extended);
            var emergencyCount = existingAppointments.Count(a => a.Type == AppointmentType.Emergency);
            if ((createDto.Type == AppointmentType.Standard || createDto.Type == AppointmentType.Extended) && standardExtendedCount >= 8)
                throw new InvalidOperationException("Doctor has reached maximum standard/extended appointments.");
            if (createDto.Type == AppointmentType.Emergency && emergencyCount >= 2)
                throw new InvalidOperationException("Doctor has reached maximum emergency appointments.");

            if (existingAppointments.Any(a => a.PatientId == createDto.PatientId))
                throw new InvalidOperationException("Patient already has an appointment with this doctor.");

            var appointment = new Appointment
            {
                AppointmentId = Guid.NewGuid(),
                PatientId = createDto.PatientId,
                DoctorId = createDto.DoctorId,
                StartTime = startTime,
                EndTime = endTime,
                Status = AppointmentStatus.Scheduled,
                Type = createDto.Type,
                Notes = createDto.Notes
            };

            await _appointmentRepository.AddAppointmentAsync(appointment);
            _logger.LogInformation("Appointment created: {AppointmentId}", appointment.AppointmentId);

            return new AppointmentDto
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status,
                Type = appointment.Type,
                Notes = appointment.Notes
            };
        }

    }
}