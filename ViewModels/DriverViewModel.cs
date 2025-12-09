using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusTrackerApp.Models;
using BusTrackerApp.Services;
using BusTrackerApp.Services.Interfaces;
using System.Collections.ObjectModel;

namespace BusTrackerApp.ViewModels;

public partial class DriverViewModel : ObservableObject
{
    private readonly ISupabaseBusService _supabaseBusService;
    private readonly ILocationService _locationService;
    private readonly IRouteSimulationService _simulationService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private Bus? assignedBus;

    [ObservableProperty]
    private BusRoute? currentRoute;

    [ObservableProperty]
    private GeoLocation? currentLocation;

    [ObservableProperty]
    private bool isTracking;

    [ObservableProperty]
    private bool isSimulating;

    [ObservableProperty]
    private string currentMode = "GPS Real";

    [ObservableProperty]
    private ObservableCollection<RoutePoint> routePoints = new();

    public DriverViewModel(
        ISupabaseBusService supabaseBusService,
        ILocationService locationService,
        IRouteSimulationService simulationService,
        IAuthService authService)
    {
        _supabaseBusService = supabaseBusService;
        _locationService = locationService;
        _simulationService = simulationService;
        _authService = authService;

        _locationService.LocationChanged += OnLocationChanged;
        _simulationService.SimulatedLocationChanged += OnSimulatedLocationChanged;
    }

    private async void OnLocationChanged(object? sender, GeoLocation location)
    {
        CurrentLocation = location;

        // Actualizar ubicación del bus en Supabase
        if (AssignedBus != null)
        {
            await _supabaseBusService.UpdateBusLocationAsync(AssignedBus.Id, location);
            // Guardar en historial con datos de GPS
            await _supabaseBusService.SaveLocationHistoryAsync(
                AssignedBus.Id,
                location,
                speed: location.Speed,
                accuracy: location.Accuracy);
        }
    }

    private async void OnSimulatedLocationChanged(object? sender, GeoLocation location)
    {
        CurrentLocation = location;

        // Actualizar ubicación del bus en Supabase (simulación)
        if (AssignedBus != null)
        {
            await _supabaseBusService.UpdateBusLocationAsync(AssignedBus.Id, location);
            // Guardar en historial (simulación no tiene speed/accuracy reales)
            await _supabaseBusService.SaveLocationHistoryAsync(
                AssignedBus.Id,
                location,
                speed: null,
                accuracy: null);
        }
    }

    [RelayCommand]
    async Task StartTrackingAsync()
    {
        var started = await _locationService.StartTrackingAsync();
        if (started)
        {
            IsTracking = true;
            CurrentMode = "GPS Real";
        }
    }

    [RelayCommand]
    async Task StopTrackingAsync()
    {
        await _locationService.StopTrackingAsync();
        IsTracking = false;
    }

    [RelayCommand]
    async Task StartSimulationAsync()
    {
        if (CurrentRoute == null || RoutePoints.Count < 2)
            return;

        // Detener GPS real si está activo
        if (IsTracking)
        {
            await StopTrackingAsync();
        }

        var started = await _simulationService.StartSimulationAsync(RoutePoints.ToList());
        if (started)
        {
            IsSimulating = true;
            CurrentMode = "Modo Demo (Simulación)";
        }
    }

    [RelayCommand]
    async Task StopSimulationAsync()
    {
        await _simulationService.StopSimulationAsync();
        IsSimulating = false;
        CurrentMode = "GPS Real";
    }

    [RelayCommand]
    async Task LogoutAsync()
    {
        await StopTrackingAsync();
        await StopSimulationAsync();
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    public async Task InitializeAsync()
    {
        var user = _authService.CurrentUser;
        if (user?.AssignedBusId != null)
        {
            // Obtener el bus asignado desde Supabase
            AssignedBus = await _supabaseBusService.GetBusByIdAsync(user.AssignedBusId);
            if (AssignedBus != null)
            {
                // Obtener la ruta asignada desde Supabase
                CurrentRoute = await _supabaseBusService.GetBusRouteAsync(AssignedBus.Id);
                if (CurrentRoute != null)
                {
                    RoutePoints.Clear();
                    foreach (var point in CurrentRoute.RoutePoints.OrderBy(p => p.Order))
                    {
                        RoutePoints.Add(point);
                    }
                }
            }
        }

        System.Diagnostics.Debug.WriteLine($"✅ DriverViewModel inicializado - Bus: {AssignedBus?.BusNumber}, Ruta: {CurrentRoute?.Name}");
    }
}
