using AutoMapper;
using SmartClinic.Models.DTOs;
using SmartClinic.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public class PatientHistoryService : IPatientHistoryService
    {
        private readonly IMedicalHistoryRepository _medicalHistoryRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientHistoryService> _logger;

        public PatientHistoryService(IMedicalHistoryRepository medicalHistoryRepository, IPatientRepository patientRepository, IMapper mapper, ILogger<PatientHistoryService> logger)
        {
            _medicalHistoryRepository = medicalHistoryRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<MedicalHistoryDto>> GetMedicalHistoryByPatientIdAsync(Guid patientId)
        {
            _logger.LogDebug("Fetching medical history for PatientId: {PatientId}", patientId);

            if (!await _patientRepository.PatientExistsAsync(patientId))
                throw new ArgumentException("Patient not found.");

            var history = await _medicalHistoryRepository.GetMedicalHistoryByPatientIdAsync(patientId);
            return _mapper.Map<List<MedicalHistoryDto>>(history);
        }
    }
}