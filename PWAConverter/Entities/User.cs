using System.Text.Json.Serialization;

namespace PWAConverter.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
        [JsonIgnore]
        public string PasswordSalt { get; set; }
        protected List<Project> _projectList { get; } = new List<Project>();
        public IReadOnlyCollection<Project> Projects => _projectList;

    }
}
