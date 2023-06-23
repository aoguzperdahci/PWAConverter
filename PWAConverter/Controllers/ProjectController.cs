using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Helpers;
using PWAConverter.Models.Manifest;
using PWAConverter.Models.Project_;
using PWAConverter.Models.Source;
using PWAConverter.MongoModels;
using PWAConverter.Services.Interfaces;
using System.Linq;
using System.Security.Claims;

namespace PWAConverter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private PWAConverterContext _dataContext;
        IIconService _iconService;
        protected IMongoCollection<BsonDocument> _iconCollection;
        protected IMongoCollection<BsonDocument> _projectDetailCollection;
        private readonly IPWAConverterMongoContext _mongoContext;

        public ProjectController(PWAConverterContext dataContext, IMapper mapper, IIconService iconService, IPWAConverterMongoContext mongoContext)
        {
            _dataContext = dataContext;
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
        [HttpGet("single")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var user = _dataContext.Users.Where(x => x.Id == HttpContext.GetUserId()).Include(u => u.Projects.Where(p => p.Id == id)).First();
            if (user == null) { return NotFound(); }

            var project = user.Projects.DefaultIfEmpty(null).SingleOrDefault();
            if (project != null)
            {
                var response = new GetProjectResponse
                {
                    Id = project.Id,
                    Name = project.Name,
                    Icon = _iconService.GetIcon(project.IconId)
                };
                return Ok(response);
            }

            return BadRequest();
        }

        /// <summary>
        /// Get project 
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns>Project</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetProjectResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllAsync()
        {
            var user = _dataContext.Users.Where(x => x.Id == HttpContext.GetUserId()).Include(u => u.Projects).First();
            if (user == null) { return BadRequest(); }
            List<GetProjectResponse> response = new List<GetProjectResponse>();
            foreach (var project in user.Projects)
            {
                response.Add(new GetProjectResponse
                {
                    Id = project.Id,
                    Name = project.Name,
                    Icon = _iconService.GetIcon(project.IconId)
                });
            }
            return Ok(response);
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

            var user = _dataContext.Users.Where(x => x.Id == HttpContext.GetUserId()).Include(u => u.Projects.Where(p => p.Id == id)).First();
            if (user == null) { return NotFound(); }

            var project = user.Projects.DefaultIfEmpty(null).SingleOrDefault();
            if (project != null)
            {
                _dataContext.Projects.Remove(project);
                var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(project.IconId));
                await _projectDetailCollection.DeleteOneAsync(deleteFilter);
                await _iconCollection.DeleteOneAsync(deleteFilter);

                if (project.Manifest != null)
                {
                    _dataContext.Manifests.Remove(project.Manifest);
                    await _dataContext.SaveChangesAsync();
                    return Ok();
                }

                return BadRequest();
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAsync(CreateProjectModel projectModel)
        {
            var user = _dataContext.Users.Where(x => x.Id == HttpContext.GetUserId()).Include(u => u.Projects).First();
            if (user == null) { return NotFound(); }

            var manifest = new Manifest
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
            await _dataContext.Manifests.AddAsync(manifest);

            if (projectModel.File.Length > 0)
            {
                var iconId = await _iconService.SaveIconAsync(projectModel.File);
                var bson = BsonSerializer.Deserialize<BsonDocument>(projectModel.ProjectDetail);
                var a = projectModel.ProjectDetail.ToJson();
                await _projectDetailCollection.InsertOneAsync(bson);
                var projectDetailId = bson["_id"].ToString();
                Project project = new Project
                {
                    IconId = iconId,
                    Name = projectModel.Name,
                    Manifest = manifest,
                    User = user,
                    ProjectDetailId = projectDetailId

                };
                await _dataContext.Projects.AddAsync(project);
                await _dataContext.SaveChangesAsync();
                return StatusCode(201,project.Id);
            }
            return BadRequest();

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
            var user = _dataContext.Users.Where(x => x.Id == HttpContext.GetUserId()).Include(u => u.Projects.Where(p => p.Id == model.Id)).First();
            if (user == null) { return NotFound(); }

            var project = user.Projects.DefaultIfEmpty(null).SingleOrDefault();
            if (project != null)
            {
                if (model.Icon.Length > 0)
                {
                    var bson = new BsonDocument("image", model.Icon);
                    bson.Set("_id", ObjectId.Parse(model.IconId));
                    var id = await _iconService.UpdateIconAsync(bson);

                    project.Name = model.Name != null ? model.Name : project.Name;
                    project.IconId = id;

                    _dataContext.Projects.Update(project);
                    await _dataContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    project.Name = model.Name != null ? model.Name : project.Name;

                    _dataContext.Projects.Update(project);
                    await _dataContext.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest();
        }
    }
}
