using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Helpers;
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
        private PWAConverterContext _dataContext;

        public UserController(IUserService userService, PWAConverterContext dataContext)
        {
            _userService = userService;
            _dataContext = dataContext;
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
            var user = await _userService.GetByIdAsync(HttpContext.GetUserId());

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
        /// The method allows user to delete its account.
        /// </summary>
        /// <returns>Status code</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUser()
        {
            var isSuccessful = await _userService.DeleteAsync(HttpContext.GetUserId());

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
            var isSuccessful = await _userService.UpdatePasswordAsync(HttpContext.GetUserId(), model);

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
            var isSuccessful = await _userService.UpdateEmailAsync(HttpContext.GetUserId(), model);

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
        /// Get the project list of user.
        /// </summary>
        /// <returns>Project list</returns>
        [HttpGet("projectList")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Project>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProjectList()
        {
            var user = _userService.GetByIdAsync(HttpContext.GetUserId());
            return Ok(user.Result.Projects);
        }

    }
}

