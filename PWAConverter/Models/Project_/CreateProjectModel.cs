using PWAConverter.Entities;
using PWAConverter.MongoModels;
using System.ComponentModel.DataAnnotations;

namespace PWAConverter.Models.Project_
{
    public class CreateProjectModel
    {
        public string Name { get; set; }
        public string File { get; set; }
        public string ProjectDetail { get; set; }

    }
}
