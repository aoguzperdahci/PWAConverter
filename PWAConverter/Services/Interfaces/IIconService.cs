using MongoDB.Bson;
using PWAConverter.MongoModels;

namespace PWAConverter.Services.Interfaces
{
    public interface IIconService
    {
        public Task<string> SaveIconAsync(string icon);
        public Task<string> UpdateIconAsync(BsonDocument icon);
        public string GetIcon(string iconId);
    }
}
