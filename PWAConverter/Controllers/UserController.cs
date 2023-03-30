using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PWAConverter.Entities;
using PWAConverter.Models.User;
using PWAConverter.Services.Interfaces;
using System.Security.Claims;

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
        /// Get current user
        /// </summary>
        /// <returns>User</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUser()
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var userDTO = new UserDTO { Name = user.Name, Email = user.Email };
                return Ok(userDTO);
            }
        }

        /// <summary>
        /// The methos allows user to delete its account.
        /// </summary>
        /// <returns>Status code</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUser()
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var isSuccessful = await _userService.DeleteAsync(userId);

            if (isSuccessful)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Update user password
        /// </summary>
        /// <param name="model">User credentials</param>
        /// <returns>Status code</returns>
        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest model)
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var isSuccessful = await _userService.UpdatePasswordAsync(userId, model);

            if (isSuccessful)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Update user email
        /// </summary>
        /// <param name="model">User credentials and new email</param>
        /// <returns>Status code</returns>
        [HttpPut("email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEmail(UpdateEmailRequest model)
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var isSuccessful = await _userService.UpdateEmailAsync(userId, model);

            if (isSuccessful)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

    }
}

