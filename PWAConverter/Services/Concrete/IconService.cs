using MongoDB.Bson;
using MongoDB.Driver;
using PWAConverter.Data;
using PWAConverter.Entities;
using PWAConverter.MongoModels;
using PWAConverter.Services.Interfaces;

namespace PWAConverter.Services.Concrete
{
    public class IconService : IIconService
    {
        private readonly IPWAConverterMongoContext _mongoContext;
        protected IMongoCollection<BsonDocument> _iconCollection;

        public IconService(IPWAConverterMongoContext mongoContext)
        {
            _mongoContext = mongoContext;
            _iconCollection = _mongoContext.GetCollection<BsonDocument>("Icon");
        }

        public string GetIcon(string iconId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(iconId));
            var iconObj = _iconCollection.Find(filter).FirstOrDefault();
            if (iconObj == null) { return null; }
            return iconObj["Image"].ToString();
        }

        public async Task<string> SaveIconAsync(string icon)
        {
            var document = new BsonDocument("Image", icon);
            await _iconCollection.InsertOneAsync(document);
            return document["_id"].ToString();
        }

        public async Task<string> UpdateIconAsync(BsonDocument icon)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", icon["_id"]);
            var iconObj = _iconCollection.Find(filter).FirstOrDefault();
            _iconCollection.ReplaceOne(iconObj, icon);
            return icon["_id"].ToString();
        }

    }
}
