using AutoMapper;
using SmartClinic.Models.DTOs;
using SmartClinic.Models.Entities;
using SmartClinic.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(IDoctorRepository doctorRepository, IAppointmentRepository appointmentRepository, IMapper mapper, ILogger<DoctorService> logger)
        {
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto createDto)
        {
            _logger.LogDebug("Creating doctor with Specialization: {Specialization}", createDto.Specialization);

            var doctor = _mapper.Map<Doctor>(createDto);
            doctor.DoctorId = Guid.NewGuid();
            doctor.UserId = Guid.NewGuid(); // Placeholder; adjust based on user creation

            await _doctorRepository.AddDoctorAsync(doctor);
            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> GetDoctorByIdAsync(Guid id)
        {
            _logger.LogDebug("Fetching doctor with DoctorId: {DoctorId}", id);

            var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
            if (doctor == null)
                throw new ArgumentException("Doctor not found.");

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<List<DoctorDto>> GetAllDoctorsAsync()
        {
            _logger.LogDebug("Fetching all doctors");

            var doctors = await _doctorRepository.GetAllDoctorsAsync();
            return _mapper.Map<List<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto> UpdateDoctorAsync(Guid id, DoctorUpdateDto updateDto)
        {
            _logger.LogDebug("Updating doctor with DoctorId: {DoctorId}", id);

            var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
            if (doctor == null)
                throw new ArgumentException("Doctor not found.");

            _mapper.Map(updateDto, doctor);
            await _doctorRepository.UpdateDoctorAsync(doctor);
            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<bool> DeleteDoctorAsync(Guid id)
        {
            _logger.LogDebug("Deleting doctor with DoctorId: {DoctorId}", id);

            return await _doctorRepository.DeleteDoctorAsync(id);
        }

        public async Task<DoctorAvailabilityDto> GetDoctorAvailabilityAsync(Guid doctorId, DateTime date)
        {
            _logger.LogDebug("Fetching availability for DoctorId: {DoctorId} on Date: {Date}", doctorId, date);

            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                throw new ArgumentException("Doctor not found.");

            var utcDate = date.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(date.Date, DateTimeKind.Utc)
                : date.Date.ToUniversalTime();

            if (!doctor.WorkingDays.Contains(utcDate.DayOfWeek))
                return new DoctorAvailabilityDto { Date = utcDate, AvailableSlots = new List<TimeSlotDto>() };

            var appointments = await _appointmentRepository.GetAppointmentsByDoctorAndDateAsync(doctorId, utcDate);
            var availableSlots = GenerateAvailableSlots(doctor, utcDate, appointments);

            return new DoctorAvailabilityDto
            {
                Date = utcDate,
                AvailableSlots = availableSlots
            };
        }

        private List<TimeSlotDto> GenerateAvailableSlots(Doctor doctor, DateTime date, List<Appointment> appointments)
        {
            var slots = new List<TimeSlotDto>();
            var startHour = TimeSpan.FromHours(9); // Default working hours
            var endHour = TimeSpan.FromHours(17);
            var breakStart = TimeSpan.FromHours(12);
            var breakEnd = TimeSpan.FromHours(13);

            var standardExtendedCount = appointments.Count(a => a.Status != AppointmentStatus.Cancelled && (a.Type == AppointmentType.Standard || a.Type == AppointmentType.Extended));
            var emergencyCount = appointments.Count(a => a.Status != AppointmentStatus.Cancelled && a.Type == AppointmentType.Emergency);

            for (var time = startHour; time < endHour; time += TimeSpan.FromMinutes(15))
            {
                if (time >= breakStart && time < breakEnd)
                    continue;

                var slotStart = date.Add(time);
                if (slotStart < DateTime.UtcNow)
                    continue;

                // Standard (30 min)
                if (standardExtendedCount < 8)
                {
                    var slotEnd = slotStart.AddMinutes(30);
                    if (slotEnd <= date.Add(endHour) && !appointments.Any(a => a.Status != AppointmentStatus.Cancelled && a.StartTime < slotEnd && a.EndTime > slotStart))
                    {
                        slots.Add(new TimeSlotDto { StartTime = slotStart, EndTime = slotEnd });
                    }
                }

                // Extended (60 min)
                if (standardExtendedCount < 8 && time < endHour - TimeSpan.FromMinutes(60))
                {
                    var slotEnd = slotStart.AddMinutes(60);
                    if (slotEnd <= date.Add(endHour) && !appointments.Any(a => a.Status != AppointmentStatus.Cancelled && a.StartTime < slotEnd && a.EndTime > slotStart))
                    {
                        slots.Add(new TimeSlotDto { StartTime = slotStart, EndTime = slotEnd });
                    }
                }

                // Emergency (15 min)
                if (emergencyCount < 2)
                {
                    var slotEnd = slotStart.AddMinutes(15);
                    if (slotEnd <= date.Add(endHour) && !appointments.Any(a => a.Status != AppointmentStatus.Cancelled && a.StartTime < slotEnd && a.EndTime > slotStart))
                    {
                        slots.Add(new TimeSlotDto { StartTime = slotStart, EndTime = slotEnd });
                    }
                }
            }

            return slots.OrderBy(s => s.StartTime).ToList();
        }
    }
}