using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Authorization;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.User;
using BCrypt.Net;
using PWAConverter.Helpers;

namespace PWAConverter.Services
{
    public class UserService : IUserService
    {
        private PWAConverterContext _dataContext;
        private IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _dataContext.Users.SingleOrDefault(x => x.Email == model.Email);

            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            // authentication successful
            var response = _mapper.Map<AuthenticateResponse>(user);
            response.Token = _jwtUtils.GenerateToken(user);
            return response;
        }

        public void Delete(int id)
        {
            var user = GetUser(id);
            _dataContext.Users.Remove(user);
            _dataContext.SaveChanges();
        }

        public IEnumerable<User> GetAll()
        {
            return _dataContext.Users;
        }

        public User GetById(int id)
        {
            return GetUser(id);
        }

        public void Register(RegisterRequest model)
        {
            // validate
            if (_dataContext.Users.Any(x => x.Email == model.Email))
                throw new AppException("Email " + model.Email + " is already registered.");

            // map model to new user object
            var user = _mapper.Map<User>(model);
           
            user.PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt();
            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password,user.PasswordSalt);

            // save user
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();
        }
        private User GetUser(int id)
        {
            var user = _dataContext.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
    }
}
