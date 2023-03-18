namespace PWAConverter.Entities
{
    public class Source
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public enum Method { HttpGet, HttpPost, HttpPut };
        public Project Project { get; set; }
    }
}
