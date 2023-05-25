using MongoDB.Driver;

namespace PWAConverter.Data
{
    public interface IPWAConverterMongoContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
