namespace PWAConverter.Entities
{
    public class Source
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public Method Method { get; set; }
        public Project Project { get; set; }
    }
}
