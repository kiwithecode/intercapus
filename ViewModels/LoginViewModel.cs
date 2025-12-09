using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusTrackerApp.Services;

namespace BusTrackerApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Por favor ingrese su email";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var user = await _authService.LoginAsync(Email, Password);
            
            if (user != null)
            {
                // Navegar según el rol del usuario
                if (user.Role == Models.UserRole.Driver)
                {
                    await Shell.Current.GoToAsync("//DriverPage");
                }
                else
                {
                    await Shell.Current.GoToAsync("//PassengerPage");
                }
            }
            else
            {
                ErrorMessage = "Credenciales inválidas";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
