using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PWAConverter.Entities;

namespace PWAConverter.MongoModels
{
    public class Icon
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public byte[] Image { get; set; }
        public Guid ProjectId { get; set; }
    }
        
}
