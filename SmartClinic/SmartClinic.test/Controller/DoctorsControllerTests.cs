using Moq;
using SmartClinic.Controllers;
using SmartClinic.Models.DTOs;
using SmartClinic.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SmartClinic.Test.Controllers
{
    public class DoctorsControllerTests
    {
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly Mock<IAppointmentService> _appointmentServiceMock;
        private readonly Mock<ILogger<DoctorsController>> _loggerMock;
        private readonly DoctorsController _controller;

        public DoctorsControllerTests()
        {
            _doctorServiceMock = new Mock<IDoctorService>();
            _appointmentServiceMock = new Mock<IAppointmentService>();
            _loggerMock = new Mock<ILogger<DoctorsController>>();
            _controller = new DoctorsController(_appointmentServiceMock.Object, _doctorServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateDoctor_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var createDto = new DoctorCreateDto { Specialization = "Cardiology" };
            var doctorDto = new DoctorDto { DoctorId = Guid.NewGuid(), Specialization = "Cardiology" };
            _doctorServiceMock.Setup(s => s.CreateDoctorAsync(createDto)).ReturnsAsync(doctorDto);

            // Act
            var result = await _controller.CreateDoctor(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(DoctorsController.GetDoctorById), createdResult.ActionName);
            Assert.Equal(doctorDto, createdResult.Value);
        }

        [Fact]
        public async Task GetDoctorById_ReturnsNotFound_WhenDoctorNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _doctorServiceMock.Setup(s => s.GetDoctorByIdAsync(id)).ThrowsAsync(new ArgumentException("Doctor not found."));

            // Act
            var result = await _controller.GetDoctorById(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}