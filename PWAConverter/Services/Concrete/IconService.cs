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
        protected IMongoCollection<Icon> _iconCollection;

        public IconService(IPWAConverterMongoContext mongoContext)
        {
            _mongoContext = mongoContext;
            _iconCollection = _mongoContext.GetCollection<Icon>(typeof(Icon).Name);
        }

        public Icon GetIcon(Guid projectId)
        {
            return _iconCollection.Find(x=>x.ProjectId == projectId).ToList().FirstOrDefault();
        }

        public string SaveIcon(Icon icon)
        {
            var iconObj = _iconCollection.Find(x=>x.Id== icon.Id).FirstOrDefault();
            if (iconObj != null)
            {
                _iconCollection.InsertOne(icon);
                
            }
            else
            {
                _iconCollection.ReplaceOne(x => x.Id == icon.Id, icon);
            }
            return icon.Id;
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
