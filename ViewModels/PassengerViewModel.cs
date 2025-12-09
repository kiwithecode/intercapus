using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusTrackerApp.Models;
using BusTrackerApp.Services;
using BusTrackerApp.Services.Interfaces;
using System.Collections.ObjectModel;

namespace BusTrackerApp.ViewModels;

public partial class PassengerViewModel : ObservableObject
{
    private readonly ISupabaseBusService _supabaseBusService;
    private readonly IAuthService _authService;
    private CancellationTokenSource? _updateCancellation;

    [ObservableProperty]
    private ObservableCollection<Bus> activeBuses = new();

    [ObservableProperty]
    private ObservableCollection<BusRoute> availableRoutes = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private GeoLocation? currentLocation;

    [ObservableProperty]
    private string? selectedRouteId;

    public PassengerViewModel(ISupabaseBusService supabaseBusService, IAuthService authService)
    {
        _supabaseBusService = supabaseBusService;
        _authService = authService;
    }

    [RelayCommand]
    async Task LoadBusesAsync()
    {
        IsLoading = true;
        try
        {
            // Obtener buses activos desde Supabase
            var buses = await _supabaseBusService.GetActiveBusesAsync();

            // Filtrar por ruta si hay una seleccionada
            if (!string.IsNullOrEmpty(SelectedRouteId))
            {
                buses = buses.Where(b => b.AssignedRoute?.Id == SelectedRouteId).ToList();
            }

            ActiveBuses.Clear();
            foreach (var bus in buses)
            {
                ActiveBuses.Add(bus);
            }

            System.Diagnostics.Debug.WriteLine($"✅ PassengerViewModel cargó {buses.Count} buses desde Supabase");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error cargando buses: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    async Task LoadRoutesAsync()
    {
        try
        {
            // Obtener todas las rutas desde Supabase
            var routes = await _supabaseBusService.GetAllRoutesAsync();
            AvailableRoutes.Clear();
            foreach (var route in routes)
            {
                AvailableRoutes.Add(route);
            }

            System.Diagnostics.Debug.WriteLine($"✅ PassengerViewModel cargó {routes.Count} rutas desde Supabase");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error cargando rutas: {ex.Message}");
        }
    }

    [RelayCommand]
    async Task SelectRouteAsync(string? routeId)
    {
        SelectedRouteId = routeId;
        await LoadBusesAsync();
    }

    [RelayCommand]
    async Task LogoutAsync()
    {
        StopRealTimeUpdates();
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    public async Task InitializeAsync()
    {
        await LoadRoutesAsync();
        await LoadBusesAsync();

        // Iniciar polling de actualizaciones en tiempo real desde Supabase
        StartRealTimeUpdates();

        System.Diagnostics.Debug.WriteLine("✅ PassengerViewModel inicializado con polling cada 2 segundos");
    }

    private void StartRealTimeUpdates()
    {
        _updateCancellation = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            while (!_updateCancellation.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(2000, _updateCancellation.Token); // Actualizar cada 2 segundos
                    await LoadBusesAsync();
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        });
    }

    private void StopRealTimeUpdates()
    {
        _updateCancellation?.Cancel();
    }
}
