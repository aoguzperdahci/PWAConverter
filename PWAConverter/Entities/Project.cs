using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Runtime.Serialization;
using PWAConverter.MongoModels;

namespace PWAConverter.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IconId{ get; set; }
        public string ProjectDetailId { get; set; }
        public User User { get; set; }
        public Manifest Manifest { get; set; }
        protected List<Source> _sources { get; } = new List<Source>();
        public IReadOnlyCollection<Source> Sources => _sources; 
    }
}
