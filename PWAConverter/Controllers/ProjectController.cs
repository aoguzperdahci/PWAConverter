using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Migrations;
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
            var user = _dataContext.Users.FindAsync(GetUserId()).Result;
            if(user == null) { return null; }
            var projects = _dataContext.Projects;
            bool isBelong = projects.Any(p => p.Id == id && p.UserId == user.Id);
            if (isBelong) {
                 
                return await projects.FindAsync(id); 
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
            var user = _dataContext.Users.FindAsync(GetUserId());
            if (user.Result == null) { return NotFound(); }
            bool isBelong = _dataContext.Projects.Any(p=>p.Id== id && p.UserId == user.Result.Id);
            if (isBelong)
            {
                var project = GetByIdAsync(id).Result;
                if(project == null) { return NotFound(); }
                _dataContext.Projects.Remove(project);
                
                if (_dataContext.Manifests.Any(p=>p.ProjectId == id)) {
                    var manifest = _dataContext.Manifests.FindAsync(id).Result;
                    if(manifest!= null) {
                        _dataContext.Manifests.Remove(manifest);
                    }
                }
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
            var user = _dataContext.Users.FindAsync(GetUserId()).Result;
            if (user == null) { return NotFound(); }
            projectModel.UserId = user.Id;
            Project project = _mapper.Map<Project>(projectModel);
            
            if (project == null) { return BadRequest(); }

            var projects = _dataContext.Projects;
            await projects.AddAsync(project);
            await _dataContext.SaveChangesAsync();
                        
            var manifestCommand = new CreateManifestModel
            {
                ProjectId = project.Id,
                ShortName = project.Name,
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
            var user = _dataContext.Users.FindAsync(GetUserId()).Result;
            if (user == null) { return NotFound(); }
            bool isBelong = _dataContext.Projects.Any(p => p.Id == model.Id && p.UserId == user.Id);
            if (isBelong)
            {
                var project = GetByIdAsync(model.Id).Result;
                if (project == null) { return NotFound(); }
                _mapper.Map<Project>(model);
                 _dataContext.Projects.Update(project);
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
        [HttpGet("getManifest")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetManifestFile(Guid projectId)
        {
            var user = _dataContext.Users.FindAsync(GetUserId()).Result;
            if (user == null) { return NotFound(); }
            bool isBelong = _dataContext.Projects.Any(p => p.Id == projectId && p.UserId == user.Id);
            if (isBelong)
            {
                var project = GetByIdAsync(projectId).Result;
                if (project == null) { return NotFound(); }
                var manifestList = await _dataContext.Manifests.ToListAsync();
                var manifest = manifestList.Find(x => x.ProjectId == projectId);
                if(manifest == null) { return NotFound(); }
                return Ok(manifest);
            }
            return BadRequest();
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
            var user =_dataContext.Users.FindAsync(GetUserId()).Result;
            if (user == null) { return NotFound(); }
            bool isBelong = _dataContext.Projects.Any(p => p.Id == projectId && p.UserId == user.Id);
            if (isBelong)
            {
                var project = GetByIdAsync(projectId).Result;
                if (project == null) { return NotFound(); }
                //var sourceList = project.Sources.ToList();
                var sourceList =  _dataContext.Sources.ToList().Where(x=>x.ProjectId == projectId);
                return Ok(sourceList);
            }
            return BadRequest();
        }

        private Guid GetUserId()
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            return Guid.Parse(claim.Value);
        }

    }
}
