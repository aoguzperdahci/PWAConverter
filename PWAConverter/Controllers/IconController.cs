using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.Models.Project_;
using PWAConverter.MongoModels;
using PWAConverter.Services.Interfaces;

namespace PWAConverter.Controllers
{
    public class IconController : ControllerBase
    {
        IIconService _iconService;
       
        private readonly PWAConverterContext _dataContext;

        public IconController(IIconService iconService, PWAConverterContext dataContext)
        {
            _iconService = iconService;
            _dataContext = dataContext;
        }

        [HttpPost("saveIcon")]
        public async Task<IActionResult> SaveIcon(CreateIconModel model)
        {
            var project = _dataContext.Projects.ToList().Where(x => x.Id == model.ProjectId).FirstOrDefault();
            var user = _dataContext.Users.Include("Projects").ToList().Where(x => x.Projects.Contains(project));
            if (user == null) { return BadRequest(); }
            
            
           /* if (model.File.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.File.CopyTo(ms);
                    var fileBytes = ms.ToArray();

                    icon.Image = fileBytes;
                    icon.Id = _iconService.SaveIcon(icon);

                    if (icon.Id.Trim() != null)
                    {
                        return Ok(icon.Id);
                    }
                    return BadRequest();
                }
            }
           */
            return BadRequest();
        }
        
        
       
    }
}
