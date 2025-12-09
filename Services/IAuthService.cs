using BusTrackerApp.Models;

namespace BusTrackerApp.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(string email, string password);
    Task LogoutAsync();
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }
}
