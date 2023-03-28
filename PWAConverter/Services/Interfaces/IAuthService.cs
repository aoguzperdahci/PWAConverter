using PWAConverter.Models.Auth;

namespace PWAConverter.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(AuthenticateRequest model);
        Task<bool> RegisterAsync(RegisterRequest model);

    }
}
