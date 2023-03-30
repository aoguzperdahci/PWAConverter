using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PWAConverter.Models.Auth;
using PWAConverter.Services.Interfaces;

namespace PWAConverter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Validates user credentials
        /// </summary>
        /// <param name="credentials">Email and password</param>
        /// <returns>JWT Token</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Authenticate(AuthenticateRequest credentials)
        {
            var response = await _authService.AuthenticateAsync(credentials);
            return response == null ? NotFound() : Ok(response);

        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Status code</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterRequest user)
        {
            bool isSuccessful = await _authService.RegisterAsync(user);
            if (isSuccessful)
            {
                return StatusCode(201);
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
