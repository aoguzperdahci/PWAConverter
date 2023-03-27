using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Helpers;
using PWAConverter.Models.Auth;
using PWAConverter.Services.Interfaces;

namespace PWAConverter.Services.Concrete
{
    public class AuthService : IAuthService
    {
        private PWAConverterContext _dataContext;
        private IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;

        public AuthService(PWAConverterContext dataContext, IJwtUtils jwtUtils, IMapper mapper)
        {
            _dataContext = dataContext;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
        }

        public async Task<string?> AuthenticateAsync(AuthenticateRequest model)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.Email == model.Email);

            // validate
            if (user == null || BCrypt.Net.BCrypt.HashPassword(model.Password, user.PasswordSalt) != user.PasswordHash)
            {
                return null;
            }

            // authentication successful
            var token = _jwtUtils.CreateToken(user);
            return token;
        }
        public async Task<bool> RegisterAsync(RegisterRequest model)
        {
            // validate
            if (await _dataContext.Users.AnyAsync(x => x.Email == model.Email))
                return false;

            // map model to new user object
            var user = _mapper.Map<User>(model);

            user.PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt();
            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, user.PasswordSalt);

            // save user
            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return true;
        }

    }
}
