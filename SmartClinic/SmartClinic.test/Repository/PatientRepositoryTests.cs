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
    public class PatientRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly PatientRepository _patientRepository;

        public PatientRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _patientRepository = new PatientRepository(_context);
        }

        [Fact]
        public async Task PatientExistsAsync_PatientExists_ReturnsTrue()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patients = new List<Patient> { new Patient { PatientId = patientId } };
            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();

            // Act
            var result = await _patientRepository.PatientExistsAsync(patientId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PatientExistsAsync_PatientDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            // Act
            var result = await _patientRepository.PatientExistsAsync(patientId);

            // Assert
            Assert.False(result);
        }
    }
}