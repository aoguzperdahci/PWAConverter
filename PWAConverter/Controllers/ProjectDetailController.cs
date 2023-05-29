using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Helpers;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public IActionResult GetProjectDetail(Guid projectId)
        {
            var user = _dataContext.Users.Where(x => x.Id == HttpContext.GetUserId()).Include(u => u.Projects.Where(p => p.Id == projectId)).First();
            if (user == null) { return NotFound(); }
            var project = user.Projects.Single();
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(project.ProjectDetailId));
            var document = _projectDetailCollection.Find(filter).FirstOrDefault();
            document["_id"] = "";
            return Ok(document.ToJson());
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
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Id == HttpContext.GetUserId()).First();
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
            var user = _dataContext.Users.Where(x => x.Id == HttpContext.GetUserId()).Include(u => u.Projects.Where(p => p.Id == model.ProjectId)).First();
            if (user == null) { return NotFound(); }
            var project = user.Projects.Single();
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(project.ProjectDetailId));
            var newBson = BsonSerializer.Deserialize<BsonDocument>(model.ProjectDetail);
            await _projectDetailCollection.ReplaceOneAsync(filter, newBson);
            return Ok();
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
            var user = _dataContext.Users.Where(x => x.Id == HttpContext.GetUserId()).Include(u => u.Projects.Where(p => p.Id == model.ProjectId)).First();
            if (user == null) { return NotFound(); }
            var project = user.Projects.Single();
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(project.ProjectDetailId));
            var result =  await _projectDetailCollection.DeleteOneAsync(deleteFilter);
            return Ok(result);
        }
    }
}
