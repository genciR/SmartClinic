using Microsoft.EntityFrameworkCore;
using SmartClinic.Models.DTOs;
using SmartClinic.Data;
using SmartClinic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;

        public DoctorService(ApplicationDbContext context)
        {
            _context = context;
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
                BreakStartTime = createDto.BreakStartTime, // No cast needed
                BreakEndTime = createDto.BreakEndTime     // No cast needed
            };

            _context.Users.Add(user);
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

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

        public async Task<DoctorDto> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DoctorId == id);

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
            return await _context.Doctors
                .Include(d => d.User)
                .Select(d => new DoctorDto
                {
                    DoctorId = d.DoctorId,
                    Name = $"{d.User.FirstName} {d.User.LastName}",
                    Specialization = d.Specialization,
                    WorkingDays = d.WorkingDays,
                    StartTime = d.StartTime,
                    EndTime = d.EndTime,
                    BreakStartTime = d.BreakStartTime,
                    BreakEndTime = d.BreakEndTime
                })
                .ToListAsync();
        }

        public async Task<DoctorDto> UpdateDoctorAsync(Guid id, DoctorUpdateDto updateDto)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null)
                return null;

            doctor.Specialization = updateDto.Specialization;
            doctor.WorkingDays = updateDto.WorkingDays;
            doctor.StartTime = updateDto.StartTime;
            doctor.EndTime = updateDto.EndTime;
            doctor.BreakStartTime = updateDto.BreakStartTime;
            doctor.BreakEndTime = updateDto.BreakEndTime;

            await _context.SaveChangesAsync();

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
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return false;

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}