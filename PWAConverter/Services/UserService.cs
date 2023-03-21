using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Authorization;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.User;
using BCrypt.Net;
using PWAConverter.Helpers;
using System.Security.Claims;
using System.Dynamic;

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

        public Guid GetMyId()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return Guid.Parse(result);
        }

        public string Authenticate(AuthenticateRequest model)
        {
            var user = _dataContext.Users.SingleOrDefault(x => x.Email == model.Email);

            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            // authentication successful
            var response = _mapper.Map<AuthenticateResponse>(user);
            response.Token = _jwtUtils.CreateToken(user);
            return response.Token;
        }

        public void Validate(string token)
        {
            _jwtUtils.ValidateToken(token);
        }
        public void Delete()
        {
            Guid id = GetMyId();
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
        public void Update(UpdateRequest model)
        {
            Guid id = GetMyId();
            var user = GetUser(id);
           
            // validate old password and mail
            if (model.Email != user.Email || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                throw new AppException("Email or password is not correct.");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.NewPassword))
                user.PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt();
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword,user.PasswordSalt);

            // copy model to user and save
            _mapper.Map(model, user);
            _dataContext.Users.Update(user);
            _dataContext.SaveChanges();
        }

      
    }
}
