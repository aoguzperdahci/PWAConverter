using PWAConverter.Entities;

namespace PWAConverter.Models.Source
{
    public class CreateSourceModel
    {
        public string Url { get; set; }
        public Method Method { get; set; }
        public Guid ProjectId { get; set; }
    }
}
