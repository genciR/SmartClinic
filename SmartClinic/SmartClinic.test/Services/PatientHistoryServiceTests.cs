using AutoMapper;
using Moq;
using SmartClinic.Models.DTOs;
using SmartClinic.Repository;
using SmartClinic.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using SmartClinic.Models.Entities;

namespace SmartClinic.Test.Services
{
    public class PatientHistoryServiceTests
    {
        private readonly Mock<IMedicalHistoryRepository> _medicalHistoryRepositoryMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PatientHistoryService>> _loggerMock;
        private readonly PatientHistoryService _patientHistoryService;

        public PatientHistoryServiceTests()
        {
            _medicalHistoryRepositoryMock = new Mock<IMedicalHistoryRepository>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PatientHistoryService>>();
            _patientHistoryService = new PatientHistoryService(_medicalHistoryRepositoryMock.Object, _patientRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetMedicalHistoryByPatientIdAsync_SuccessfulFetch_ReturnsMedicalHistoryList()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var historyList = new List<MedicalHistory> { new MedicalHistory { HistoryId = Guid.NewGuid() } };
            var historyDtoList = new List<MedicalHistoryDto> { new MedicalHistoryDto { HistoryId = historyList[0].HistoryId } };

            _patientRepositoryMock.Setup(r => r.PatientExistsAsync(patientId)).ReturnsAsync(true);
            _medicalHistoryRepositoryMock.Setup(r => r.GetMedicalHistoryByPatientIdAsync(patientId)).ReturnsAsync(historyList);
            _mapperMock.Setup(m => m.Map<List<MedicalHistoryDto>>(historyList)).Returns(historyDtoList);

            // Act
            var result = await _patientHistoryService.GetMedicalHistoryByPatientIdAsync(patientId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(historyDtoList[0].HistoryId, result[0].HistoryId);
            _medicalHistoryRepositoryMock.Verify(r => r.GetMedicalHistoryByPatientIdAsync(patientId), Times.Once());
        }

        [Fact]
        public async Task GetMedicalHistoryByPatientIdAsync_PatientNotFound_ThrowsArgumentException()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _patientRepositoryMock.Setup(r => r.PatientExistsAsync(patientId)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _patientHistoryService.GetMedicalHistoryByPatientIdAsync(patientId));
        }
    }
}