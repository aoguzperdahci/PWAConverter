using PWAConverter.MongoModels;

namespace PWAConverter.Services.Interfaces
{
    public interface IIconService
    {
        public string SaveIcon(Icon icon);
        public Icon GetIcon(Guid projectId);
        public byte[] GetImage(string sBase64string);
    }
}
