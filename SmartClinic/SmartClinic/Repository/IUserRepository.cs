using SmartClinic.Models.Entities;
using System;
using System.Threading.Tasks;

namespace SmartClinic.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
    }
}