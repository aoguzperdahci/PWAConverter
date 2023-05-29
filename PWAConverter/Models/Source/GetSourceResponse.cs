using PWAConverter.Entities;

namespace PWAConverter.Models.Source
{
    public class GetSourceResponse
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public Method Method { get; set; }
    }
}
