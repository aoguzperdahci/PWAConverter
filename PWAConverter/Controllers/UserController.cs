using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PWAConverter.Entities;
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

        /// <summary>
        /// The method allows user to login the system and creates a token.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Token</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            return response == null ? NotFound() : Ok(response);

        }

        /// <summary>
        /// The method evaluates whether the token is valid or not.
        /// </summary>
        /// <param name="token">Token itself.</param>
        /// <returns>Success message.</returns>
        [HttpPost("validate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Object))]
        public IActionResult Validate(string token)
        {
            _userService.Validate(token);
            return Ok(new { message = "Validation successful" });
        }
        /// <summary>
        /// The method allows user to register the system.
        /// </summary>
        /// <param name="model">Email, password, name</param>
        /// <returns>Success message</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Object))]
        public IActionResult Register(RegisterRequest model)
        {
            _userService.Register(model);
            return Ok(new { message = "Registration successful" });
        }
        /// <summary>
        /// The method allows to get a list of all the users.
        /// </summary>
        /// <returns>User list</returns>
        [HttpGet("getAll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
        /// <summary>
        /// The method allows getting user object with the search of <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var user = _userService.GetById(id);
            return user == null ? NotFound() : Ok(user);

        }

        /// <summary>
        /// The methos allows user to delete its account.
        /// </summary>
        /// <returns>Success message</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Object))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Delete()
        {
            _userService.Delete();
            return Ok(new { message = "User deleted successfully" });
        }

        /// <summary>
        /// The method allows user to change its password.
        /// </summary>
        /// <param name="model">Email, old password and new password of the user.</param>
        /// <returns>Success message</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Object))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Update(UpdateRequest model)
        {
            _userService.Update(model);
            return Ok(new { message = "User updated successfully" });
        }
    }
}

