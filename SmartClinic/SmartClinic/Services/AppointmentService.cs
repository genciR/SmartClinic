using AutoMapper;
using SmartClinic.Models.DTOs;
using SmartClinic.Models.Entities;
using SmartClinic.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicalHistoryRepository _medicalHistoryRepository;
        private readonly IMapper _mapper;
        private readonly int _maxAppointmentsPerDay = 8;
        private readonly int _maxEmergencyAppointmentsPerDay = 2;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IMedicalHistoryRepository medicalHistoryRepository, IMapper mapper, ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _medicalHistoryRepository = medicalHistoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AppointmentDto> ScheduleAppointmentAsync(AppointmentCreateDto createDto)
        {
            _logger.LogDebug("Scheduling appointment for DoctorId: {DoctorId}, PatientId: {PatientId}, StartTime: {StartTime}",
                createDto.DoctorId, createDto.PatientId, createDto.StartTime);

            var startTime = createDto.StartTime.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(createDto.StartTime, DateTimeKind.Utc)
                : createDto.StartTime.ToUniversalTime();

            var doctor = await _doctorRepository.GetDoctorByIdAsync(createDto.DoctorId);
            if (doctor == null)
                throw new ArgumentException("Doctor not found.");

            if (!await _patientRepository.PatientExistsAsync(createDto.PatientId))
                throw new ArgumentException("Patient not found.");

            var dayOfWeek = startTime.DayOfWeek;
            if (!doctor.WorkingDays.Contains(dayOfWeek))
                throw new ArgumentException("Doctor is not available on this day.");

            var startHour = TimeSpan.FromHours(9); // Hardcoded working hours
            var endHour = TimeSpan.FromHours(17);
            var breakStart = TimeSpan.FromHours(12);
            var breakEnd = TimeSpan.FromHours(13);

            var timeOfDay = startTime.TimeOfDay; // TimeSpan
            if (timeOfDay < startHour || timeOfDay >= endHour ||
                (timeOfDay >= breakStart && timeOfDay < breakEnd))
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
            if (existingAppointments.Any(a => a.StartTime < endTime && a.EndTime > startTime && a.Status != AppointmentStatus.Cancelled))
                throw new InvalidOperationException("Requested time slot is already booked.");

            var standardExtendedCount = existingAppointments.Count(a => a.Type == AppointmentType.Standard || a.Type == AppointmentType.Extended);
            var emergencyCount = existingAppointments.Count(a => a.Type == AppointmentType.Emergency);
            if ((createDto.Type == AppointmentType.Standard || createDto.Type == AppointmentType.Extended) && standardExtendedCount >= _maxAppointmentsPerDay)
                throw new InvalidOperationException("Doctor has reached maximum standard/extended appointments.");
            if (createDto.Type == AppointmentType.Emergency && emergencyCount >= _maxEmergencyAppointmentsPerDay)
                throw new InvalidOperationException("Doctor has reached maximum emergency appointments.");

            if (existingAppointments.Any(a => a.PatientId == createDto.PatientId))
                throw new InvalidOperationException("Patient already has an appointment with this doctor.");

            var appointment = _mapper.Map<Appointment>(createDto);
            appointment.AppointmentId = Guid.NewGuid();
            appointment.StartTime = startTime;
            appointment.EndTime = endTime;
            appointment.Status = AppointmentStatus.Scheduled;

            await _appointmentRepository.AddAppointmentAsync(appointment);
            _logger.LogInformation("Appointment created: {AppointmentId}", appointment.AppointmentId);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<List<AppointmentDto>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            _logger.LogDebug("Fetching appointments for PatientId: {PatientId}", patientId);

            if (!await _patientRepository.PatientExistsAsync(patientId))
                throw new ArgumentException("Patient not found.");

            var appointments = await _appointmentRepository.GetAppointmentsByPatientIdAsync(patientId);
            return _mapper.Map<List<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto> UpdateAppointmentStatusAsync(AppointmentStatusUpdateDto updateDto)
        {
            _logger.LogDebug("Updating status for AppointmentId: {AppointmentId} to {Status}",
                updateDto.AppointmentId, updateDto.Status);

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(updateDto.AppointmentId);
            if (appointment == null)
                throw new ArgumentException("Appointment not found.");

            if (updateDto.Status == AppointmentStatus.Cancelled)
            {
                if (appointment.StartTime < DateTime.UtcNow.AddHours(24))
                    throw new InvalidOperationException("Cannot cancel appointment less than 24 hours before start time.");
            }

            appointment.Status = updateDto.Status;
            await _appointmentRepository.UpdateAppointmentAsync(appointment);

            if (updateDto.Status == AppointmentStatus.Completed)
            {
                var medicalHistory = new MedicalHistory
                {
                    HistoryId = Guid.NewGuid(),
                    PatientId = appointment.PatientId,
                    DoctorId = appointment.DoctorId,
                    AppointmentId = appointment.AppointmentId,
                    CreatedAt = DateTime.UtcNow
                };
                await _medicalHistoryRepository.AddMedicalHistoryAsync(medicalHistory);
                _logger.LogInformation("Medical history created for AppointmentId: {AppointmentId}", appointment.AppointmentId);
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }
    }
}