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
    
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            return Ok(response);
        }

      
        [HttpPost("validate")]
        public IActionResult Validate(string token)
        {
             _userService.Validate(token);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest model)
        {
            _userService.Register(model);
            return Ok(new { message = "Registration successful" });
        }
        
        [HttpGet("getAll")]
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

            [HttpDelete]
            public IActionResult Delete()
            {   
                _userService.Delete();
                return Ok(new { message = "User deleted successfully" });
            }
            [HttpPut]
            public IActionResult Update(UpdateRequest model)
            {
                _userService.Update(model);
                return Ok(new { message = "User updated successfully" });
            }
        }
    }

