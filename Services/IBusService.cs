using BusTrackerApp.Models;

namespace BusTrackerApp.Services;

public interface IBusService
{
    Task<List<Bus>> GetActiveBusesAsync();
    Task<Bus?> GetBusByIdAsync(string busId);
    Task<bool> UpdateBusLocationAsync(string busId, GeoLocation location);
    Task<BusRoute?> GetBusRouteAsync(string busId);
    Task<List<BusRoute>> GetAllRoutesAsync();
    void StartBusSimulation();
    void StopBusSimulation();
}
