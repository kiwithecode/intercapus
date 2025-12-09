using BusTrackerApp.Models;

namespace BusTrackerApp.Services;

public interface ILocationService
{
    Task<GeoLocation?> GetCurrentLocationAsync();
    Task<bool> StartTrackingAsync();
    Task StopTrackingAsync();
    event EventHandler<GeoLocation>? LocationChanged;
}
