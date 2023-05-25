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
            var user = _dataContext.Users.Include("Projects").ToList().Where(x=>x.Id == userId).First();
            if(user == null) { return NotFound(); }
            Project project = user.Projects.Where(p => p.Id == model.ProjectId).First();
            if (project != null)
            {
                var manifest = _dataContext.Manifests.ToList().Where(x=>x.Id == model.Id).First();
                if(manifest== null) { return NotFound(); }
                manifest.ShortName = model.ShortName;
                manifest.Description = model.Description;
                manifest.DisplayMode = model.DisplayMode;
                manifest.Orientation = model.Orientation;
                manifest.BackGroundColor= model.BackGroundColor;
                manifest.ThemeColor = model.ThemeColor;
                manifest.StartUrl = model.StartUrl;
                manifest.Scope = model.Scope;

                _dataContext.Manifests.Update(manifest);
                await _dataContext.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
    }
}
