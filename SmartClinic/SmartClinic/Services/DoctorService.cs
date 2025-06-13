using SmartClinic.Models.DTOs;
using SmartClinic.Models.Entities;
using SmartClinic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto createDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email,
                PasswordHash = "hashed_password", // Placeholder
                Role = Role.Doctor,
                CreatedAt = DateTime.UtcNow
            };

            var doctor = new Doctor
            {
                DoctorId = Guid.NewGuid(),
                UserId = user.Id,
                Specialization = createDto.Specialization,
                WorkingDays = createDto.WorkingDays,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                BreakStartTime = createDto.BreakStartTime,
                BreakEndTime = createDto.BreakEndTime,
                User = user
            };

            await _doctorRepository.AddDoctorAsync(doctor);

            return new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                Name = $"{user.FirstName} {user.LastName}",
                Specialization = doctor.Specialization,
                WorkingDays = doctor.WorkingDays,
                StartTime = doctor.StartTime,
                EndTime = doctor.EndTime,
                BreakStartTime = doctor.BreakStartTime,
                BreakEndTime = doctor.BreakEndTime
            };
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
            if (doctor == null)
                return null;

            return new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                Name = $"{doctor.User.FirstName} {doctor.User.LastName}",
                Specialization = doctor.Specialization,
                WorkingDays = doctor.WorkingDays,
                StartTime = doctor.StartTime,
                EndTime = doctor.EndTime,
                BreakStartTime = doctor.BreakStartTime,
                BreakEndTime = doctor.BreakEndTime
            };
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAllDoctorsAsync();
            return doctors.Select(d => new DoctorDto
            {
                DoctorId = d.DoctorId,
                Name = $"{d.User.FirstName} {d.User.LastName}",
                Specialization = d.Specialization,
                WorkingDays = d.WorkingDays,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                BreakStartTime = d.BreakStartTime,
                BreakEndTime = d.BreakEndTime
            });
        }

        public async Task<DoctorDto?> UpdateDoctorAsync(Guid id, DoctorUpdateDto updateDto)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
            if (doctor == null)
                return null;

            doctor.Specialization = updateDto.Specialization;
            doctor.WorkingDays = updateDto.WorkingDays;
            doctor.StartTime = updateDto.StartTime;
            doctor.EndTime = updateDto.EndTime;
            doctor.BreakStartTime = updateDto.BreakStartTime;
            doctor.BreakEndTime = updateDto.BreakEndTime;

            await _doctorRepository.UpdateDoctorAsync(doctor);

            return new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                Name = $"{doctor.User.FirstName} {doctor.User.LastName}",
                Specialization = doctor.Specialization,
                WorkingDays = doctor.WorkingDays,
                StartTime = doctor.StartTime,
                EndTime = doctor.EndTime,
                BreakStartTime = doctor.BreakStartTime,
                BreakEndTime = doctor.BreakEndTime
            };
        }

        public async Task<bool> DeleteDoctorAsync(Guid id)
        {
            return await _doctorRepository.DeleteDoctorAsync(id);
        }

        public async Task<DoctorAvailabilityDto> GetDoctorAvailabilityAsync(Guid doctorId, DateTime date)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");

            var dayOfWeek = date.DayOfWeek;
            if (!doctor.WorkingDays.Contains(dayOfWeek))
                return new DoctorAvailabilityDto { Date = date.Date, AvailableSlots = new List<TimeSlotDto>() };

            var appointments = await _doctorRepository.GetAppointmentsByDoctorAndDateAsync(doctorId, date);

            var slots = new List<TimeSlotDto>();
            var startDateTime = date.Date + doctor.StartTime;
            var endDateTime = date.Date + doctor.EndTime;
            var slotDuration = TimeSpan.FromMinutes(30);

            for (var current = startDateTime; current < endDateTime; current += slotDuration)
            {
                if (doctor.BreakStartTime.HasValue && doctor.BreakEndTime.HasValue)
                {
                    var breakStart = date.Date + doctor.BreakStartTime.Value;
                    var breakEnd = date.Date + doctor.BreakEndTime.Value;
                    if (current >= breakStart && current < breakEnd)
                        continue;
                }

                var slotEnd = current + slotDuration;
                bool isOverlapping = appointments.Any(a =>
                    a.StartTime < slotEnd && a.EndTime > current);

                if (!isOverlapping)
                {
                    slots.Add(new TimeSlotDto
                    {
                        StartTime = current,
                        EndTime = slotEnd
                    });
                }
            }

            return new DoctorAvailabilityDto
            {
                Date = date.Date,
                AvailableSlots = slots
            };
        }
    }
}