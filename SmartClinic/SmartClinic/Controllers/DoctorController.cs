using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DoctorsController> _logger;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;

        public DoctorsController(ILogger<DoctorsController> logger, IDoctorService doctorService, IAppointmentService appointmentService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _doctorService = doctorService;
            _appointmentService = appointmentService;
        }

        // POST: api/doctors
        [HttpPost]
        public async Task<ActionResult<DoctorDto>> CreateDoctor([FromBody] DoctorCreateDto createDto)
        {
            var doctor = await _doctorService.CreateDoctorAsync(createDto);
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.DoctorId }, doctor);
        }

        // GET: api/doctors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(Guid id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();
            return Ok(doctor);
        }
        [HttpGet("{id}/availability")]
        public async Task<ActionResult<DoctorAvailabilityDto>> GetDoctorAvailability(Guid id, [FromQuery] DateTime date)
        {
            try
            {
                var availability = await _doctorService.GetDoctorAvailabilityAsync(id, date);
                return Ok(availability);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        // GET: api/doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        // PUT: api/doctors/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<DoctorDto>> UpdateDoctor(Guid id, [FromBody] DoctorUpdateDto updateDto)
        {
            var doctor = await _doctorService.UpdateDoctorAsync(id, updateDto);
            if (doctor == null)
                return NotFound();
            return Ok(doctor);
        }

        // DELETE: api/doctors/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            var success = await _doctorService.DeleteDoctorAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
        [HttpPost("{doctorId:guid}/appointments")]
        public async Task<ActionResult<AppointmentDto>> ScheduleAppointment(Guid doctorId, [FromBody] AppointmentCreateDto createDto)
        {
            _logger.LogDebug("Received request for doctorId: {DoctorId}, patientId: {PatientId}, startTime: {StartTime}, Kind: {Kind}",
         doctorId, createDto.PatientId, createDto.StartTime, createDto.StartTime.Kind);
            if (createDto.DoctorId != doctorId)
            {
                _logger.LogWarning("Doctor ID mismatch: URL={UrlDoctorId}, Body={BodyDoctorId}", doctorId, createDto.DoctorId);
                return BadRequest("Doctor ID in URL and body must match.");
            }

            try
            {
                var appointment = await _appointmentService.ScheduleAppointmentAsync(createDto);
                _logger.LogInformation("Appointment created: {AppointmentId}", appointment.AppointmentId);
                return CreatedAtAction(nameof(GetDoctorAvailability), new { id = doctorId, date = appointment.StartTime.Date }, appointment);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Conflict: {Message}", ex.Message);
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating appointment");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}