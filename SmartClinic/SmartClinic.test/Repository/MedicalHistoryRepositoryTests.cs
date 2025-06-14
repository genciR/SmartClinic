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
    public class MedicalHistoryRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly MedicalHistoryRepository _medicalHistoryRepository;

        public MedicalHistoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _medicalHistoryRepository = new MedicalHistoryRepository(_context);
        }

        [Fact]
        public async Task AddMedicalHistoryAsync_SuccessfulAdd_ReturnsNoException()
        {
            // Arrange
            var history = new MedicalHistory { HistoryId = Guid.NewGuid(), PatientId = Guid.NewGuid() };
            ArgumentNullException.ThrowIfNull(history);

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _medicalHistoryRepository.AddMedicalHistoryAsync(history));
            Assert.Null(exception);
        }

        [Fact]
        public async Task GetMedicalHistoryByPatientIdAsync_ValidInput_ReturnsMedicalHistory()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var histories = new List<MedicalHistory>
            {
                new MedicalHistory { HistoryId = Guid.NewGuid(), PatientId = patientId }
            };
            await _context.MedicalHistories.AddRangeAsync(histories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _medicalHistoryRepository.GetMedicalHistoryByPatientIdAsync(patientId);

            // Assert
            Assert.Single(result);
            Assert.Equal(patientId, result[0].PatientId);
        }
    }
}