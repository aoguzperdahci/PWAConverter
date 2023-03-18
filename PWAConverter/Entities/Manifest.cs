namespace PWAConverter.Entities
{
    public class Manifest
    {
        public Guid Id { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public enum DisplayMode { FullScreen, StandAlone, MinimalUI, Browser};
        public enum Orientation { Any, Landscape, Portrait };
        public string BackGroundColor { get; set; }
        public string ThemeColor { get; set;}
        public string StartUrl { get; set; }
        public string Scope { get; set; }
    }
}
