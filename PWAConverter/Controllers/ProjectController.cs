using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.Manifest;
using PWAConverter.Models.Project_;
using System.Security.Claims;

namespace PWAConverter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private PWAConverterContext _dataContext;
        private readonly IMapper _mapper;
        public ProjectController(PWAConverterContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        /// <summary>
        /// Get project 
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns>Project</returns>
        [HttpGet("getProject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Project?> GetByIdAsync(Guid id)
        {
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Id == GetUserId()).First();
            if (user == null) { return null; }
            bool isBelong = user.Projects.Any(x => x.Id == id);
            if (isBelong) {

                return user.Projects.Where(x=>x.Id == id).First(); 
            }
            return null;
           
        }

        /// <summary>
        /// The method allows user to delete one of its projects.
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns>Status code</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var project = GetByIdAsync(id).Result;
            if(project == null) { return NotFound(); }
                _dataContext.Projects.Remove(project);
                
                if (project.Manifest != null) {
                    _dataContext.Manifests.Remove(project.Manifest);
                    await _dataContext.SaveChangesAsync();
                    return Ok();
                }
            return BadRequest();
        }
        /// <summary>
        /// Create a project
        /// </summary>
        /// <param name="projectModel">Project name and icon</param>
        /// <returns>Status code</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAsync(CreateProjectModel projectModel)
        {
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Id == GetUserId()).First();
            if (user == null) { return NotFound(); }
            
            var manifestCommand = new CreateManifestModel
            {
                ShortName = projectModel.Name,
                Description = "",
                DisplayMode = DisplayMode.StandAlone,
                Orientation = Orientation.Any,
                BackGroundColor = "#000000",
                ThemeColor = "#000000",
                StartUrl = "/",
                Scope = "/"
            };
            var manifest = _mapper.Map<Manifest>(manifestCommand);
            await _dataContext.Manifests.AddAsync(manifest);
            await _dataContext.SaveChangesAsync();

            Project project = new Project{ 
                IconId = projectModel.IconId,
                Name = projectModel.Name,
                ProjectDetailId= projectModel.ProjectDetailId,
                Manifest = manifest,
                User = user,
            };

            await _dataContext.Projects.AddAsync(project);
            await _dataContext.SaveChangesAsync();
            return StatusCode(201);

        }
        /// <summary>
        /// Update project
        /// </summary>
        /// <param name="model">Project id, name, icon</param>
        /// <returns>Status code</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]    
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateAsync(UpdateProjectModel model)
        {
                var project = GetByIdAsync(model.Id).Result;
                if (project == null) { return NotFound(); }
                project.Name = model.Name != null ? model.Name : project.Name;
                project.IconId = model.IconId != null ? model.IconId : project.IconId ;
                project.ProjectDetailId = model.ProjectDetailId != null ? model.ProjectDetailId : project.ProjectDetailId;
                 _dataContext.Projects.Update(project);
                await _dataContext.SaveChangesAsync();
                return Ok();
        }
        /// <summary>
        /// Get manifest of the project
        /// </summary>
        /// <param name="projectId">Project id</param>
        /// <returns>Manifest</returns>
        [HttpGet("getManifest")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetManifestFile(Guid projectId)
        {
                var project = GetByIdAsync(projectId).Result;
                if (project == null) { return NotFound(); }
                var manifest = project.Manifest;
                if(manifest == null) { return NotFound(); }
                return Ok(manifest);
        }
        /// <summary>
        /// Get the source list of project
        /// </summary>
        /// <param name="projectId">Project id</param>
        /// <returns>Source List</returns>
        [HttpGet("getSources")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSources(Guid projectId)
        {
                var project = GetByIdAsync(projectId).Result;
                if (project == null) { return NotFound(); }
                var sourceList =  project.Sources.ToList();
                return Ok(sourceList);
            
        }

        private Guid GetUserId()
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            return Guid.Parse(claim.Value);
        }

    }
}
