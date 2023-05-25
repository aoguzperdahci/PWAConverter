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

        public BsonDocument GetIcon(Guid projectId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("ProjectId", projectId);
            var document = _iconCollection.Find(filter).FirstOrDefault();
            return document;
        }

        public string SaveIcon(BsonDocument icon)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", icon["_id"]);
            var iconObj = _iconCollection.Find(filter).FirstOrDefault();
            if (iconObj == null)
            {
                _iconCollection.InsertOne(iconObj);
                
            }
            else
            {
                _iconCollection.ReplaceOne(iconObj, icon);
            }
            return icon["_id"].ToString();
        }
        public byte[] GetImage(string sBase64string)
        {
            byte[] bytes = null;
            if (!string.IsNullOrEmpty(sBase64string))
            {
                bytes = Convert.FromBase64String(sBase64string);
            }
            return bytes;
        }

       
    }
}
