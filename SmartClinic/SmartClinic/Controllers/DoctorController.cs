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
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
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
    }
}