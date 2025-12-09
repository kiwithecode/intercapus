using BusTrackerApp.Models;

namespace BusTrackerApp.Services;

public interface IRouteSimulationService
{
    event EventHandler<GeoLocation>? SimulatedLocationChanged;
    Task<bool> StartSimulationAsync(List<RoutePoint> routePoints);
    Task StopSimulationAsync();
    bool IsSimulating { get; }
    double CurrentProgress { get; }
    int CurrentPointIndex { get; }
}
