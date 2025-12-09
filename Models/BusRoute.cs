namespace BusTrackerApp.Models;

public class BusRoute
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public List<RoutePoint> RoutePoints { get; set; } = new();
    public string Color { get; set; } = "#007AFF"; // Color para mostrar en el mapa
}
