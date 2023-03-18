namespace PWAConverter.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string ProjectDetail { get; set; }
        public Manifest Manifest { get; set; }
        protected List<Source> _sources { get; } = new List<Source>();
        public IReadOnlyCollection<Source> Sources => _sources; 
    }
}
