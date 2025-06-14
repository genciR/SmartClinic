using AutoMapper;
using SmartClinic.Models.DTOs;
using SmartClinic.Models.Entities;
using SmartClinic.Repository;
using System;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> RegisterAsync(UserRegistrationDto registrationDto)
        {
            _logger.LogDebug("Registering user with Email: {Email}", registrationDto.Email);

            if (await _userRepository.GetUserByEmailAsync(registrationDto.Email) != null)
                throw new InvalidOperationException("Email already exists.");

            var user = _mapper.Map<User>(registrationDto);
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password);

            await _userRepository.AddUserAsync(user);

            if (registrationDto.Role == "Patient")
            {
                var patient = new Patient
                {
                    PatientId = Guid.NewGuid(),
                    UserId = user.Id,
                    DateOfBirth = DateTime.Now.AddYears(-30), // Placeholder
                    Gender = "Unknown",
                    Phone = "Unknown"
                };
                await _patientRepository.AddPatientAsync(patient);
            }
            else if (registrationDto.Role == "Doctor")
            {
                var doctor = new Doctor
                {
                    DoctorId = Guid.NewGuid(),
                    UserId = user.Id,
                    Specialization = "General",
                    WorkingDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday },
                    StartTime = TimeSpan.FromHours(9),
                    EndTime = TimeSpan.FromHours(17)
                };
                await _doctorRepository.AddDoctorAsync(doctor);
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> LoginAsync(UserLoginDto loginDto)
        {
            _logger.LogDebug("Logging in user with Email: {Email}", loginDto.Email);

            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            return "jwt-token-placeholder"; // Implement JWT
        }
    }
}