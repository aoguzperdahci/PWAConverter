using PWAConverter.Entities;
using PWAConverter.Models.Auth;
using PWAConverter.Models.User;

namespace PWAConverter.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdatePasswordAsync(Guid id, UpdatePasswordRequest model);
        Task<bool> UpdateEmailAsync(Guid id, UpdateEmailRequest model);
    }
}
