using MongoDB.Bson;
using PWAConverter.Entities;

namespace PWAConverter.Models.Project_
{
    public class CreateProjectDetailModel
    {
        public string CacheStorages { get; set; }
        public string AdditionalFeatures { get; set; }
        public Guid ProjectId { get; set; }
    }
}
