using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Authorization;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.User;
using BCrypt.Net;
using PWAConverter.Helpers;
using System.Security.Claims;

namespace PWAConverter.Services
{
    public class UserService : IUserService
    {
        private PWAConverterContext _dataContext;
        private IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(PWAConverterContext dataContext, IJwtUtils jwtUtils, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetMyId()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return result;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _dataContext.Users.SingleOrDefault(x => x.Email == model.Email);

            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            // authentication successful
            var response = _mapper.Map<AuthenticateResponse>(user);
            response.Token = _jwtUtils.CreateToken(user);
            return response;
        }

        public void Delete(Guid id)
        {
            var user = GetUser(id);
            _dataContext.Users.Remove(user);
            _dataContext.SaveChanges();
        }

        public IEnumerable<User> GetAll()
        {
            return _dataContext.Users;
        }

        public User GetById(Guid id)
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
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, user.PasswordSalt);
                
                // save user
                _dataContext.Users.Add(user);
                _dataContext.SaveChanges();
            
            
        }
        private User GetUser(Guid id)
        {
            var user = _dataContext.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
        public void Update(Guid id, UpdateRequest model)
        {
            var user = GetUser(id);

            // validate
            if (model.Email != user.Email && _dataContext.Users.Any(x => x.Email == model.Email))
                throw new AppException("Email is not valid.");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                user.PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt();
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password,user.PasswordSalt);

            // copy model to user and save
            _mapper.Map(model, user);
            _dataContext.Users.Update(user);
            _dataContext.SaveChanges();
        }

      
    }
}
