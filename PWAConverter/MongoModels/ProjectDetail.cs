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
        public string ProjectId { get; set; }

        public ProjectDetail(BsonDocument cacheStorages, BsonDocument additionalFeatures, string projectId)
        {
            CacheStorages = cacheStorages;
            AdditionalFeatures = additionalFeatures;
            ProjectId = projectId;
        }
    }
}
