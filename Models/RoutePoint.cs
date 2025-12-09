namespace BusTrackerApp.Models;

public class RoutePoint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public GeoLocation Location { get; set; } = new GeoLocation();
    public int Order { get; set; } // Orden en la ruta
}
