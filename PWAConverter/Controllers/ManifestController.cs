using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.Manifest;
using System.Security.Claims;

namespace PWAConverter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ManifestController:ControllerBase
    {
        private PWAConverterContext _dataContext;
        private readonly IMapper _mapper;

        public ManifestController(PWAConverterContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        /// <summary>
        /// Update manifest
        /// </summary>
        /// <param name="model">Manifest id, short name, description, display mode, orientation
        /// background color, theme color, start url, scope, project id</param>
        /// <returns>Status code</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateManifest(UpdateManifestModel model)
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var user = _dataContext.Users.FindAsync(userId).Result;
            if(user == null) { return NotFound(); }
            bool isBelong = _dataContext.Projects.Any(p => p.Id == model.ProjectId && p.UserId == user.Id);
            if (isBelong)
            {
                var manifest = _dataContext.Manifests.FindAsync(model.Id).Result;
                if(manifest== null) { return NotFound(); }
                _mapper.Map<Manifest>(model);
                _dataContext.Manifests.Update(manifest);
                await _dataContext.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
    }
}
