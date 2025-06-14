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
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IMedicalHistoryRepository> _medicalHistoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<AppointmentService>> _loggerMock;
        private readonly AppointmentService _appointmentService;

        public AppointmentServiceTests()
        {
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _doctorRepositoryMock = new Mock<IDoctorRepository>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _medicalHistoryRepositoryMock = new Mock<IMedicalHistoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<AppointmentService>>();
            _appointmentService = new AppointmentService(_appointmentRepositoryMock.Object, _doctorRepositoryMock.Object, _patientRepositoryMock.Object, _medicalHistoryRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ScheduleAppointmentAsync_SuccessfulScheduling_ReturnsAppointmentDto()
        {
            // Arrange
            var createDto = new AppointmentCreateDto
            {
                DoctorId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                StartTime = new DateTime(2025, 6, 16, 9, 0, 0, DateTimeKind.Utc), // Explicit UTC
                Type = AppointmentType.Standard
            };
            var doctor = new Doctor { DoctorId = createDto.DoctorId, WorkingDays = new List<DayOfWeek> { DayOfWeek.Monday } };
            var appointment = new Appointment { AppointmentId = Guid.NewGuid(), StartTime = createDto.StartTime, EndTime = createDto.StartTime.AddMinutes(30) };
            var appointmentDto = new AppointmentDto { AppointmentId = appointment.AppointmentId };

            // Use It.IsAny<> to match any date, ensuring the mock captures the service's call
            _doctorRepositoryMock.Setup(r => r.GetDoctorByIdAsync(createDto.DoctorId)).ReturnsAsync(doctor);
            _patientRepositoryMock.Setup(r => r.PatientExistsAsync(createDto.PatientId)).ReturnsAsync(true);
            _appointmentRepositoryMock.Setup(r => r.GetAppointmentsByDoctorAndDateAsync(createDto.DoctorId, It.IsAny<DateTime>())).ReturnsAsync(new List<Appointment>()); // Match any date
            _mapperMock.Setup(m => m.Map<Appointment>(createDto)).Returns(appointment);
            _mapperMock.Setup(m => m.Map<AppointmentDto>(appointment)).Returns(appointmentDto);
            _appointmentRepositoryMock.Setup(r => r.AddAppointmentAsync(appointment)).Returns(Task.CompletedTask);

            // Act
            var result = await _appointmentService.ScheduleAppointmentAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointmentDto.AppointmentId, result.AppointmentId);
            _appointmentRepositoryMock.Verify(r => r.GetAppointmentsByDoctorAndDateAsync(createDto.DoctorId, It.IsAny<DateTime>()), Times.Once()); // Verify call
            _appointmentRepositoryMock.Verify(r => r.AddAppointmentAsync(appointment), Times.Once());
        }
    }
}