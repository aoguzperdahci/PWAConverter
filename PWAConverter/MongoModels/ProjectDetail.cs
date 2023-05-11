using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PWAConverter.Entities;

namespace PWAConverter.MongoModels
{
    public class ProjectDetail
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public BsonDocument CacheStorages { get; set; }
        public BsonDocument AdditionalFeatures { get;set; }
        public Project Project { get; set; }

        public ProjectDetail(BsonDocument cacheStorages, BsonDocument additionalFeatures, Project project)
        {
            CacheStorages = cacheStorages;
            AdditionalFeatures = additionalFeatures;
            Project = project;
        }
    }
}
