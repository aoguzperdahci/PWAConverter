using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PWAConverter.Helpers;
using PWAConverter.Models.User;
using PWAConverter.Services;

namespace PWAConverter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest model)
        {
            _userService.Register(model);
            return Ok(new { message = "Registration successful" });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

            [HttpGet("{id}")]
            public IActionResult GetById(Guid id)
            {
                var user = _userService.GetById(id);
                return Ok(user);
            }

            [HttpDelete("{id}")]
            public IActionResult Delete(Guid id)
            {
                _userService.Delete(id);
                return Ok(new { message = "User deleted successfully" });
            }
            [HttpPut("{id}")]
            public IActionResult Update(Guid id, UpdateRequest model)
            {
                _userService.Update(id, model);
                return Ok(new { message = "User updated successfully" });
            }
        }
    }
}
