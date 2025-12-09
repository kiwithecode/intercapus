using BusTrackerApp.Models;

namespace BusTrackerApp.Services;

public class AuthService : IAuthService
{
    private User? _currentUser;

    public User? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;

    public async Task<User?> LoginAsync(string email, string password)
    {
        // TODO: Implementar autenticación real con API
        // Por ahora, simulamos usuarios de prueba
        await Task.Delay(500); // Simular llamada a API

        if (email.Contains("conductor") || email.Contains("driver"))
        {
            _currentUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Conductor Demo",
                Email = email,
                Role = UserRole.Driver,
                AssignedBusId = "BUS-001"
            };
        }
        else
        {
            _currentUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Pasajero Demo",
                Email = email,
                Role = UserRole.Passenger
            };
        }

        return _currentUser;
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        return Task.CompletedTask;
    }
}
