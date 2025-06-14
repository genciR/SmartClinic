using Moq;
using SmartClinic.Data;
using SmartClinic.Models.Entities;
using SmartClinic.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace SmartClinic.test.Repository
{
    public class AppointmentRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly AppointmentRepository _appointmentRepository;

        public AppointmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _appointmentRepository = new AppointmentRepository(_context);
        }

        [Fact]
        public async Task AddAppointmentAsync_SuccessfulAdd_ReturnsNoException()
        {
            // Arrange
            var appointment = new Appointment { AppointmentId = Guid.NewGuid(), StartTime = DateTime.UtcNow };

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _appointmentRepository.AddAppointmentAsync(appointment));
            Assert.Null(exception);
        }

        [Fact]
        public async Task GetAppointmentsByDoctorAndDateAsync_ValidInput_ReturnsAppointments()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var date = DateTime.UtcNow.Date;
            var appointments = new List<Appointment>
            {
                new Appointment { AppointmentId = Guid.NewGuid(), DoctorId = doctorId, StartTime = date.AddHours(9) }
            };
            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _appointmentRepository.GetAppointmentsByDoctorAndDateAsync(doctorId, date);

            // Assert
            Assert.Single(result);
            Assert.Equal(doctorId, result[0].DoctorId);
        }
    }
}