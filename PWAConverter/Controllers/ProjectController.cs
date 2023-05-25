﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.Manifest;
using PWAConverter.Models.Project_;
using PWAConverter.MongoModels;
using PWAConverter.Services.Interfaces;
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
        IIconService _iconService;
        protected IMongoCollection<BsonDocument> _iconCollection;
        protected IMongoCollection<BsonDocument> _projectDetailCollection;
        private readonly IPWAConverterMongoContext _mongoContext;

        public ProjectController(PWAConverterContext dataContext, IMapper mapper, IIconService iconService, IPWAConverterMongoContext mongoContext)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _iconService = iconService;
            _mongoContext = mongoContext;
            _projectDetailCollection = _mongoContext.GetCollection<BsonDocument>("ProjectDetail"); 
            _iconCollection = _mongoContext.GetCollection<BsonDocument>("Icon"); 
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

                return _dataContext.Projects.Include("Sources").Include("Manifest").ToList().Where(x=>x.Id == id).First(); 
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
                var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(project.IconId));
                await _projectDetailCollection.DeleteOneAsync(deleteFilter);
                await _iconCollection.DeleteOneAsync(deleteFilter);
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

            if (projectModel.File.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    projectModel.File.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    var document = new BsonDocument
                    {
                        {"Image",fileBytes },
                        {"ProjectId","" }
                    };
                    var id = _iconService.SaveIcon(document);
                    
                    Project project = new Project
                    {
                        IconId = id,
                        Name = projectModel.Name,
                        Manifest = manifest,
                        User = user,
                    };
                    document.Set("ProjectId", project.Id);
                    await _dataContext.Projects.AddAsync(project);
                    await _dataContext.SaveChangesAsync();
                    return StatusCode(201);
                }

            }
            else
            {
                Project project = new Project
                {
                    Name = projectModel.Name,
                    Manifest = manifest,
                    User = user,
                };
                await _dataContext.Projects.AddAsync(project);
                await _dataContext.SaveChangesAsync();
                return StatusCode(201);
            }
           
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(UpdateProjectModel model)
        {
                var project = GetByIdAsync(model.Id).Result;
                if (project == null) { return NotFound(); }
                
            if (model.File.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.File.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    var document = new BsonDocument
                    {
                        {"Image",fileBytes },
                        {"ProjectId","" }
                    };
                    var id = _iconService.SaveIcon(document);

                    project.Name = model.Name != null ? model.Name : project.Name;
                    project.IconId =  id;
                    project.ProjectDetailId = model.ProjectDetailId != null ? model.ProjectDetailId : project.ProjectDetailId;

                    _dataContext.Projects.Update(project);
                    await _dataContext.SaveChangesAsync();
                    return Ok();
                }

            }
            else
            {
                project.Name = model.Name != null ? model.Name : project.Name;
                project.IconId = model.IconId != null ? model.IconId : project.IconId;
                project.ProjectDetailId = model.ProjectDetailId != null ? model.ProjectDetailId : project.ProjectDetailId;

                _dataContext.Projects.Update(project);
                await _dataContext.SaveChangesAsync();
                return Ok();
            }
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
                var sourceList =  _dataContext.Sources.ToList().Where(x=>x.Project.Id == projectId);
                return Ok(sourceList);
            
        }

        private Guid GetUserId()
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            return Guid.Parse(claim.Value);
        }

        /// <summary>
        /// Gets the icon of project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>Status codes</returns>
        [HttpGet("getIcon")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetIcon(Guid projectId)
        {
            var project = GetByIdAsync(projectId).Result;
            if (project == null) { return NotFound(); }
            var icon = _iconService.GetIcon(projectId);
            if(icon == null) { return BadRequest(); }
            var image = _iconService.GetImage(icon["Image"].ToString());
            return Ok(image);
        }

    }
}