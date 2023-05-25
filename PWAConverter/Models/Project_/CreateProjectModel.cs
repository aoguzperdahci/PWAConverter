using PWAConverter.Entities;
using PWAConverter.MongoModels;
using System.ComponentModel.DataAnnotations;

namespace PWAConverter.Models.Project_
{
    public class CreateProjectModel
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }

    }
}
