using MongoDB.Bson;
using PWAConverter.MongoModels;

namespace PWAConverter.Services.Interfaces
{
    public interface IIconService
    {
        public string SaveIcon(BsonDocument icon);
        public BsonDocument GetIcon(Guid projectId);
        public byte[] GetImage(string sBase64string);
    }
}
