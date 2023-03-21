using PWAConverter.Entities;
using PWAConverter.Models.User;

namespace PWAConverter.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
        void Register(RegisterRequest model);
        void Delete();
        void Update(UpdateRequest model);
        Guid GetMyId();
    }
}
