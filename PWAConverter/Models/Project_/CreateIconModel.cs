using PWAConverter.Entities;

namespace PWAConverter.Models.Project_
{
    public class CreateIconModel
    {
        public Guid ProjectId { get; set; }
        public IFormFile File { get; set; }
    }
}
