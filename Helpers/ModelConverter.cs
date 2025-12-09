using BusTrackerApp.Models;
using BusTrackerApp.Models.Supabase;

namespace BusTrackerApp.Helpers;

/// <summary>
/// Convierte entre modelos de Supabase y modelos de la aplicación
/// </summary>
public static class ModelConverter
{
    // ============================================
    // CONVERSIONES: Supabase → App Models
    // ============================================

    public static Bus ToBus(SupabaseBus supabaseBus, SupabaseRoute? route = null, List<SupabaseRoutePoint>? routePoints = null)
    {
        var bus = new Bus
        {
            Id = supabaseBus.Id.ToString(),
            BusNumber = supabaseBus.BusNumber,
            PlateNumber = supabaseBus.PlateNumber,
            IsActive = supabaseBus.IsActive,
            LastUpdate = supabaseBus.LastUpdate,
            DriverId = supabaseBus.DriverId?.ToString()
        };

        // Ubicación actual
        if (supabaseBus.CurrentLatitude.HasValue && supabaseBus.CurrentLongitude.HasValue)
        {
            bus.CurrentLocation = new GeoLocation(
                supabaseBus.CurrentLatitude.Value,
                supabaseBus.CurrentLongitude.Value
            );
        }

        // Ruta asignada
        if (route != null)
        {
            bus.AssignedRoute = ToBusRoute(route, routePoints);
        }

        return bus;
    }

    public static BusRoute ToBusRoute(SupabaseRoute supabaseRoute, List<SupabaseRoutePoint>? routePoints = null)
    {
        var route = new BusRoute
        {
            Id = supabaseRoute.Id.ToString(),
            Name = supabaseRoute.Name,
            Color = supabaseRoute.Color,
            RoutePoints = new List<RoutePoint>()
        };

        // Convertir puntos de ruta
        if (routePoints != null)
        {
            route.RoutePoints = routePoints
                .OrderBy(rp => rp.PointOrder)
                .Select(ToRoutePoint)
                .ToList();
        }

        return route;
    }

    public static RoutePoint ToRoutePoint(SupabaseRoutePoint supabasePoint)
    {
        return new RoutePoint
        {
            Name = supabasePoint.Name,
            Location = new GeoLocation(supabasePoint.Latitude, supabasePoint.Longitude),
            Order = supabasePoint.PointOrder
        };
    }

    public static User ToUser(SupabaseUser supabaseUser)
    {
        return new User
        {
            Id = supabaseUser.Id.ToString(),
            Email = supabaseUser.Email,
            Name = supabaseUser.FullName,
            Role = supabaseUser.Role == "driver" ? UserRole.Driver : UserRole.Passenger,
            AssignedBusId = supabaseUser.AssignedBusId?.ToString()
        };
    }

    // ============================================
    // CONVERSIONES: App Models → Supabase
    // ============================================

    public static SupabaseBus ToSupabaseBus(Bus bus)
    {
        return new SupabaseBus
        {
            Id = Guid.Parse(bus.Id),
            BusNumber = bus.BusNumber,
            PlateNumber = bus.PlateNumber,
            IsActive = bus.IsActive,
            LastUpdate = bus.LastUpdate,
            CurrentLatitude = bus.CurrentLocation?.Latitude,
            CurrentLongitude = bus.CurrentLocation?.Longitude,
            RouteId = bus.AssignedRoute != null ? Guid.Parse(bus.AssignedRoute.Id) : null,
            DriverId = !string.IsNullOrEmpty(bus.DriverId) ? Guid.Parse(bus.DriverId) : null
        };
    }

    public static SupabaseRoute ToSupabaseRoute(BusRoute route)
    {
        return new SupabaseRoute
        {
            Id = Guid.Parse(route.Id),
            RouteCode = route.Id, // Usando el ID como código por ahora
            Name = route.Name,
            Color = route.Color,
            IsActive = true
        };
    }

    public static SupabaseRoutePoint ToSupabaseRoutePoint(RoutePoint point, Guid routeId)
    {
        return new SupabaseRoutePoint
        {
            Id = Guid.NewGuid(),
            RouteId = routeId,
            Name = point.Name,
            Latitude = point.Location.Latitude,
            Longitude = point.Location.Longitude,
            PointOrder = point.Order
        };
    }

    public static SupabaseLocationHistory ToLocationHistory(string busId, GeoLocation location, double? speed = null, double? accuracy = null)
    {
        return new SupabaseLocationHistory
        {
            Id = Guid.NewGuid(),
            BusId = Guid.Parse(busId),
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Speed = speed,
            Accuracy = accuracy,
            RecordedAt = DateTime.UtcNow
        };
    }
}
