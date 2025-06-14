using Microsoft.AspNetCore.Mvc;
using SmartClinic.Models.DTOs;
using SmartClinic.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Controllers
{
    [Route("api/patients")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientHistoryService _patientHistoryService;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientHistoryService patientHistoryService, ILogger<PatientsController> logger)
        {
            _patientHistoryService = patientHistoryService;
            _logger = logger;
        }

        [HttpGet("{id:guid}/history")]
        public async Task<ActionResult<List<MedicalHistoryDto>>> GetMedicalHistory(Guid id)
        {
            try
            {
                var history = await _patientHistoryService.GetMedicalHistoryByPatientIdAsync(id);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching medical history for PatientId: {PatientId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}