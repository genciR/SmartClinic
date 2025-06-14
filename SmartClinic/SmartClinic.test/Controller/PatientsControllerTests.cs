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

namespace SmartClinic.test.Controller
{
    public class PatientsControllerTests
    {
        private readonly Mock<IPatientHistoryService> _patientHistoryServiceMock;
        private readonly Mock<ILogger<PatientsController>> _loggerMock;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            _patientHistoryServiceMock = new Mock<IPatientHistoryService>();
            _loggerMock = new Mock<ILogger<PatientsController>>();
            _controller = new PatientsController(_patientHistoryServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetMedicalHistory_ReturnsOk_WhenHistoryFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var historyList = new List<MedicalHistoryDto> { new MedicalHistoryDto { HistoryId = Guid.NewGuid() } };
            _patientHistoryServiceMock.Setup(s => s.GetMedicalHistoryByPatientIdAsync(patientId)).ReturnsAsync(historyList);

            // Act
            var result = await _controller.GetMedicalHistory(patientId); // Corrected to use _controller

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<MedicalHistoryDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(historyList[0].HistoryId, returnValue[0].HistoryId);
        }

        [Fact]
        public async Task GetMedicalHistory_ReturnsNotFound_WhenPatientNotFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            _patientHistoryServiceMock.Setup(s => s.GetMedicalHistoryByPatientIdAsync(patientId)).ThrowsAsync(new ArgumentException("Patient not found."));

            // Act
            var result = await _controller.GetMedicalHistory(patientId); 

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}