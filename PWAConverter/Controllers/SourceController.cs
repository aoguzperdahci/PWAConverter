using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.Source;
using System.Security.Claims;

namespace PWAConverter.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SourceController:ControllerBase
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
            var claim = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            var userId = Guid.Parse(claim.Value);
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Id == userId).First();
            if (user == null) { return NotFound(); }
            Project project = user.Projects.Where(p => p.Id == model.ProjectId).First();
            if (project != null)
            {
                var sources = project.Sources.ToList();
                if (sources.Any(y => y.Url == model.Url && y.Method == model.Method))
                {
                    return BadRequest();
                }
                Source source = new Source { Project= project, Method= model.Method, Url=model.Url };
               
                await _dataContext.Sources.AddAsync(source);
                await _dataContext.SaveChangesAsync();
                return StatusCode(201);
            }
            return BadRequest();
        }

    }
}
