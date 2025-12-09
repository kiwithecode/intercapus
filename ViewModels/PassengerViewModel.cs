using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusTrackerApp.Models;
using BusTrackerApp.Services;
using System.Collections.ObjectModel;

namespace BusTrackerApp.ViewModels;

public partial class PassengerViewModel : ObservableObject
{
    private readonly IBusService _busService;
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

    public PassengerViewModel(IBusService busService, IAuthService authService)
    {
        _busService = busService;
        _authService = authService;
    }

    [RelayCommand]
    async Task LoadBusesAsync()
    {
        IsLoading = true;
        try
        {
            var buses = await _busService.GetActiveBusesAsync();

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
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando buses: {ex.Message}");
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
            var routes = await _busService.GetAllRoutesAsync();
            AvailableRoutes.Clear();
            foreach (var route in routes)
            {
                AvailableRoutes.Add(route);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando rutas: {ex.Message}");
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
        _busService.StopBusSimulation();
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    public async Task InitializeAsync()
    {
        await LoadRoutesAsync();
        await LoadBusesAsync();

        // Iniciar simulación de movimiento de buses
        _busService.StartBusSimulation();

        // Actualización periódica más frecuente para ver el movimiento suave
        StartRealTimeUpdates();
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
