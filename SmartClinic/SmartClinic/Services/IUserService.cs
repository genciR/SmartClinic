using SmartClinic.Models.DTOs;
using System.Threading.Tasks;

namespace SmartClinic.Services
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(UserRegistrationDto registrationDto);
        Task<string> LoginAsync(UserLoginDto loginDto);
    }
}