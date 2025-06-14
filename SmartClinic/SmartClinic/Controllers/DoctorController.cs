using Microsoft.AspNetCore.Mvc;
using SmartClinic.Models.DTOs;
using SmartClinic.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClinic.Controllers
{
    [Route("api/doctors")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IAppointmentService appointmentService, IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<DoctorDto>> CreateDoctor([FromBody] DoctorCreateDto createDto)
        {
            try
            {
                var doctorDto = await _doctorService.CreateDoctorAsync(createDto);
                return CreatedAtAction(nameof(GetDoctorById), new { id = doctorDto.DoctorId }, doctorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<DoctorDto>>> GetAllDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all doctors");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DoctorDto>> GetDoctorById(Guid id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);
                return Ok(doctor);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctor with DoctorId: {DoctorId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<DoctorDto>> UpdateDoctor(Guid id, [FromBody] DoctorUpdateDto updateDto)
        {
            try
            {
                var doctor = await _doctorService.UpdateDoctorAsync(id, updateDto);
                return Ok(doctor);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor with DoctorId: {DoctorId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            try
            {
                var deleted = await _doctorService.DeleteDoctorAsync(id);
                if (!deleted)
                    return NotFound("Doctor not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor with DoctorId: {DoctorId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("{id:guid}/availability")]
        public async Task<ActionResult<DoctorAvailabilityDto>> GetDoctorAvailability(Guid id, [FromQuery] DateTime date)
        {
            try
            {
                var availability = await _doctorService.GetDoctorAvailabilityAsync(id, date);
                return Ok(availability);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching availability for DoctorId: {DoctorId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("{doctorId:guid}/appointments")]
        public async Task<ActionResult<AppointmentDto>> ScheduleAppointment(Guid doctorId, [FromBody] AppointmentCreateDto createDto)
        {
            if (doctorId != createDto.DoctorId)
                return BadRequest("Doctor ID mismatch.");

            try
            {
                var appointmentDto = await _appointmentService.ScheduleAppointmentAsync(createDto);
                return CreatedAtAction(nameof(ScheduleAppointment), new { doctorId, id = appointmentDto.AppointmentId }, appointmentDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling appointment for DoctorId: {DoctorId}", doctorId);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}