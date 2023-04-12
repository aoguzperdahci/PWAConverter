using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var user = _dataContext.Users.FindAsync(userId).Result;
            if (user == null) { return NotFound(); }
            bool isBelong = _dataContext.Projects.Any(p => p.Id == model.ProjectId && p.UserId == user.Id);
            if (isBelong)
            {
                var sources = _dataContext.Sources.ToList();
                if (sources.Any(y => y.Url == model.Url && y.Method == model.Method))
                {
                    return BadRequest();
                }
                var source = _mapper.Map<Source>(model);
                if (source == null) { return BadRequest(); }
                await _dataContext.Sources.AddAsync(source);
                await _dataContext.SaveChangesAsync();
                return StatusCode(201);
            }
            return BadRequest();
        }

    }
}
