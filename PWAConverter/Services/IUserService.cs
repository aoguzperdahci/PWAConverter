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
        void Delete(Guid id);
        void Update(Guid id, UpdateRequest model);
        string GetMyId();
    }
}
