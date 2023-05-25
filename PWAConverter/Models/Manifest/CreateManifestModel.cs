using PWAConverter.Entities;

namespace PWAConverter.Models.Manifest
{
    public class CreateManifestModel
    {
        public string ShortName { get; set; }
        public string Description { get; set; }
        public DisplayMode DisplayMode { get; set; }
        public Orientation Orientation { get; set; }
        public string BackGroundColor { get; set; }
        public string ThemeColor { get; set; }
        public string StartUrl { get; set; }
        public string Scope { get; set; }
    }
}
