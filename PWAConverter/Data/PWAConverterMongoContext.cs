using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using PWAConverter.Models;

namespace PWAConverter.Data
{
    public class PWAConverterMongoContext : IPWAConverterMongoContext
    {
        private IMongoDatabase _db { get; set; }
        private readonly IConfiguration _configuration;
        const string connectionUri = "mongodb://localhost:27017";

        public PWAConverterMongoContext(IConfiguration configuration)
        {
            _configuration = configuration;
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            
            var client = new MongoClient(settings);
            
            _db = client.GetDatabase(_configuration.GetSection("MongoSettings:DatabaseName").Value); 
            
        }
        public IMongoCollection<BsonDocument> GetCollection<BsonDocument>(string name)
        {
            return _db.GetCollection<BsonDocument>(name);
        }
    }
}
