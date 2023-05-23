using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.Project_;
using PWAConverter.MongoModels;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static MongoDB.Driver.WriteConcern;

namespace PWAConverter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectDetailController:ControllerBase
    {
        private readonly PWAConverterContext _dataContext;
        private readonly IPWAConverterMongoContext _mongoContext;
        protected IMongoCollection<BsonDocument> _projectDetailCollection;

        public ProjectDetailController(PWAConverterContext context, IPWAConverterMongoContext mongoContext)
        {
            _dataContext = context;
            _mongoContext = mongoContext;
            _projectDetailCollection = _mongoContext.GetCollection<BsonDocument>("ProjectDetail"); 
           
        }
        /// <summary>
        /// Get project details
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>Status code</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("getProjectDetail")]
        public async Task<IActionResult> GetProjectDetail(Guid projectId)
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Id == userId).First();
            if (user == null) { return null; }
            bool isBelong = user.Projects.Any(x => x.Id == projectId);
            if (isBelong)
            {
                var project = _dataContext.Projects.ToList().Where(x => x.Id == projectId).First();
                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(project.ProjectDetailId));
                var document = _projectDetailCollection.Find(filter).FirstOrDefault();
                return Ok(document);
            }
            return BadRequest();
        }
        /// <summary>
        /// Creates project details.
        /// </summary>
        /// <param name="model">Project id, cache storages, additional features</param>
        /// <returns>Status codes</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateProjectDetail(CreateProjectDetailModel model)
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Id == userId).First();
            if (user == null) { return NotFound(); }
            Project project = user.Projects.Where(p => p.Id == model.ProjectId).First();
            if (project != null)
            {
                var document = new BsonDocument
            {
                { "CacheStorages", model.CacheStorages },
                { "AdditionalFeatures", model.AdditionalFeatures},
                { "ProjectId", model.ProjectId.ToString()}
            };
                _projectDetailCollection.InsertOne(document);
                project.ProjectDetailId = document["_id"].ToString();
                _dataContext.Projects.Update(project);
                await _dataContext.SaveChangesAsync();
                return StatusCode(201);
            }
            return BadRequest();
        }
        /// <summary>
        /// Updates project details
        /// </summary>
        /// <param name="model">Id, project id, cache storages, additional features</param>
        /// <returns>Status codes</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> UpdateProjectDetail(UpdateProjectDetailModel model)
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Id == userId).First();
            if (user == null) { return NotFound(); }
            Project project = user.Projects.Where(p => p.Id == model.ProjectId).First();
            if (project != null)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(model.Id));
                var updateCache = Builders<BsonDocument>.Update.Set("CacheStorages", model.CacheStorages);
                var updateFeatures = Builders<BsonDocument>.Update.Set("AdditionalFeatues", model.AdditionalFeatures);
                await _projectDetailCollection.UpdateOneAsync(filter, updateCache);
                await _projectDetailCollection.UpdateOneAsync(filter, updateFeatures);
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// Deletes project details
        /// </summary>
        /// <param name="model">Id, project id</param>
        /// <returns>Status codes</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProjectDetail(DeleteProjectDetailModel model)
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Id == userId).First();
            if (user == null) { return NotFound(); }
            Project project = user.Projects.Where(p => p.Id == model.ProjectId).First();
            if (project != null)
            {
                
                var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(model.Id));
                var result =  await _projectDetailCollection.DeleteOneAsync(deleteFilter);
                return Ok(result);
            }
            return BadRequest();

        }
    }
}
