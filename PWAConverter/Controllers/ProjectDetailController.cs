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

namespace PWAConverter.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectDetailController:ControllerBase
    {
        private readonly PWAConverterContext _dataContext;
        private readonly IPWAConverterMongoContext _mongoContext;
        protected IMongoCollection<ProjectDetail> _projectDetailCollection;

        public ProjectDetailController(PWAConverterContext context, IPWAConverterMongoContext mongoContext)
        {
            _dataContext = context;
            _mongoContext = mongoContext;
            _projectDetailCollection = _mongoContext.GetCollection<ProjectDetail>(typeof(ProjectDetail).Name); 
           
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectDetail(CreateProjectDetailModel model)
        {
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Id == userId).First();
            if (user == null) { return NotFound(); }
            Project project = user.Projects.Where(p => p.Id == model.ProjectId).First();
            if (project != null)
            {
                ProjectDetail projectDetail = new ProjectDetail(
                    BsonDocument.Parse(model.CacheStorages),
                   BsonDocument.Parse(model.AdditionalFeatures),
                    project);
                _projectDetailCollection.InsertOne(projectDetail);
                return Ok();
            }
            return BadRequest();
        }
    }
}
