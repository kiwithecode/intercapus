namespace BusTrackerApp.Models;

public class Bus
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string BusNumber { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public BusRoute? AssignedRoute { get; set; }
    public GeoLocation? CurrentLocation { get; set; }
    public string? DriverId { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
}
