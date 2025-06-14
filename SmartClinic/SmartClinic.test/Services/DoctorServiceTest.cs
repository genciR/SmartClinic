using AutoMapper;
using Moq;
using SmartClinic.Models.DTOs;
using SmartClinic.Models.Entities;
using SmartClinic.Repository;
using SmartClinic.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;

namespace SmartClinic.test.Services
{
    public class DoctorServiceTests
    {
        private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<DoctorService>> _loggerMock;
        private readonly DoctorService _doctorService;

        public DoctorServiceTests()
        {
            _doctorRepositoryMock = new Mock<IDoctorRepository>();
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<DoctorService>>();
            _doctorService = new DoctorService(_doctorRepositoryMock.Object, _appointmentRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetDoctorAvailabilityAsync_DoctorNotWorkingDay_ReturnsEmptySlots()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var date = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc); // Explicit UTC
            var doctor = new Doctor { DoctorId = doctorId, WorkingDays = new List<DayOfWeek> { DayOfWeek.Monday } };
            var appointments = new List<Appointment>();

            _doctorRepositoryMock.Setup(r => r.GetDoctorByIdAsync(doctorId)).ReturnsAsync(doctor);
            _appointmentRepositoryMock.Setup(r => r.GetAppointmentsByDoctorAndDateAsync(doctorId, date.Date)).ReturnsAsync(appointments);

            // Act
            var result = await _doctorService.GetDoctorAvailabilityAsync(doctorId, date);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.AvailableSlots);
            Assert.Equal(date.Date, result.Date); // Compare Date only, ignoring time
        }
    }
}