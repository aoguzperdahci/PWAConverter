using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Helpers;
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
            var user = _dataContext.Users.Where(x=>x.Id == HttpContext.GetUserId()).Include(u => u.Projects.Where(p => p.Id == model.ProjectId)).ThenInclude(p => p.Manifest).First();
            if(user == null) { return NotFound(); }

            var project = user.Projects.DefaultIfEmpty(null).SingleOrDefault();
            if (project != null)
            {
                var manifest = project.Manifest;
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

        /// <summary>
        /// Get manifest of the project
        /// </summary>
        /// <param name="projectId">Project id</param>
        /// <returns>Manifest</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Manifest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetManifestFile(Guid projectId)
        {
            var user = _dataContext.Users.Where(x => x.Id == HttpContext.GetUserId()).Include(u => u.Projects.Where(p => p.Id == projectId)).ThenInclude(p => p.Manifest).First();
            if (user == null) { return NotFound(); }

            var project = user.Projects.DefaultIfEmpty(null).SingleOrDefault();
            if (project != null)
            {
                return Ok(project.Manifest);
            }

            return BadRequest();
        }

    }
}
