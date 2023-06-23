using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Helpers;
using PWAConverter.Models.Source;
using System.Security.Claims;

namespace PWAConverter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SourceController : ControllerBase
    {
        private PWAConverterContext _dataContext;
        private readonly IMapper _mapper;

        public SourceController(PWAConverterContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Create source
        /// </summary>
        /// <param name="model">Url, method, project id of the source</param>
        /// <returns>Status code</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAsync(CreateSourceModel model)
        {
            var project = _dataContext.Projects.Include(p => p.Sources).Where(p => p.Id == model.ProjectId).First();
            if (project == null) { return NotFound(); }
            if (project.Sources.Any(y => y.Url == model.Url && y.Method == model.Method))
            {
                return BadRequest();
            }
            Source source = new Source { Project = project, Method = model.Method, Url = model.Url };

            await _dataContext.Sources.AddAsync(source);
            await _dataContext.SaveChangesAsync();
            return StatusCode(201);
        }

        /// <summary>
        /// Get the source list of project
        /// </summary>
        /// <param name="projectId">Project id</param>
        /// <returns>Source List</returns>
        [HttpGet("getSources")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSourceResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetSources(Guid projectId)
        {
            var user = _dataContext.Users.Where(u => u.Id == HttpContext.GetUserId()).Include(u => u.Projects.Where(p => p.Id == projectId)).ThenInclude(p => p.Sources).First();
            if (user == null) { return NotFound(); }
            var r = user.Projects.SelectMany(p => p.Sources.Select(s => new GetSourceResponse { Id = s.Id, Method = s.Method, Url = s.Url })).ToList();
            return Ok(r);
        }
    }
}
