using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.User;
using PWAConverter.Helpers;
using PWAConverter.Services.Interfaces;
using PWAConverter.Models.Auth;

namespace PWAConverter.Services.Concrete
{
    public class UserService : IUserService
    {
        private PWAConverterContext _dataContext;

        public UserService(PWAConverterContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dataContext.Users.FindAsync(id);
        }

        public async Task<bool> UpdatePasswordAsync(Guid id, UpdatePasswordRequest model)
        {
            var user = await GetByIdAsync(id);

            // validate old password
            if (user == null || BCrypt.Net.BCrypt.HashPassword(model.Password, user.PasswordSalt) != user.PasswordHash)
            {
                return false;
            }

            user.PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, user.PasswordSalt);

            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEmailAsync(Guid id, UpdateEmailRequest model)
        {
            var user = await GetByIdAsync(id);

            // validate old password
            if (user == null || BCrypt.Net.BCrypt.HashPassword(model.Password, user.PasswordSalt) != user.PasswordHash)
            {
                return false;
            }

            user.Email = model.NewEmail;
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
            return true;
        }
    }
}
