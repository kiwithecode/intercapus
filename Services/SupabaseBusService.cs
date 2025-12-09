using BusTrackerApp.Models;
using BusTrackerApp.Models.Supabase;
using BusTrackerApp.Helpers;
using BusTrackerApp.Services.Interfaces;
using PostgrestConstants = Postgrest.Constants;

namespace BusTrackerApp.Services;

/// <summary>
/// Servicio para gestionar buses y rutas usando Supabase
/// </summary>
public class SupabaseBusService : ISupabaseBusService
{
    private readonly SupabaseService _supabaseService;

    public event EventHandler<Bus>? BusLocationChanged;

    public SupabaseBusService(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    /// <summary>
    /// Obtiene todos los buses activos desde Supabase
    /// </summary>
    public async Task<List<Bus>> GetActiveBusesAsync()
    {
        try
        {
            var client = await _supabaseService.GetClientAsync();

            // Obtener buses activos
            var busesResponse = await client
                .From<SupabaseBus>()
                .Where(b => b.IsActive == true)
                .Get();

            var buses = new List<Bus>();

            foreach (var supabaseBus in busesResponse.Models)
            {
                // Obtener la ruta del bus
                BusRoute? route = null;
                List<SupabaseRoutePoint>? routePoints = null;

                if (supabaseBus.RouteId.HasValue)
                {
                    var routeResponse = await client
                        .From<SupabaseRoute>()
                        .Where(r => r.Id == supabaseBus.RouteId.Value)
                        .Single();

                    if (routeResponse != null)
                    {
                        var pointsResponse = await client
                            .From<SupabaseRoutePoint>()
                            .Where(rp => rp.RouteId == supabaseBus.RouteId.Value)
                            .Order("point_order", PostgrestConstants.Ordering.Ascending)
                            .Get();

                        routePoints = pointsResponse.Models.ToList();
                        route = ModelConverter.ToBusRoute(routeResponse, routePoints);
                    }
                }

                var bus = ModelConverter.ToBus(supabaseBus, route: null, routePoints: null);
                bus.AssignedRoute = route;
                buses.Add(bus);
            }

            System.Diagnostics.Debug.WriteLine($"✅ Obtenidos {buses.Count} buses activos desde Supabase");
            return buses;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error obteniendo buses activos: {ex.Message}");
            return new List<Bus>();
        }
    }

    /// <summary>
    /// Obtiene un bus específico por su ID
    /// </summary>
    public async Task<Bus?> GetBusByIdAsync(string busId)
    {
        try
        {
            var client = await _supabaseService.GetClientAsync();
            var busGuid = Guid.Parse(busId);

            var busResponse = await client
                .From<SupabaseBus>()
                .Where(b => b.Id == busGuid)
                .Single();

            if (busResponse == null)
                return null;

            // Obtener la ruta si existe
            BusRoute? route = null;
            if (busResponse.RouteId.HasValue)
            {
                route = await GetRouteByIdAsync(busResponse.RouteId.Value);
            }

            return ModelConverter.ToBus(busResponse, route: route?.Id != null ? await GetSupabaseRouteAsync(Guid.Parse(route.Id)) : null);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error obteniendo bus {busId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Actualiza la ubicación de un bus en Supabase
    /// </summary>
    public async Task<bool> UpdateBusLocationAsync(string busId, GeoLocation location)
    {
        try
        {
            var client = await _supabaseService.GetClientAsync();
            var busGuid = Guid.Parse(busId);

            var updateData = new SupabaseBus
            {
                Id = busGuid,
                CurrentLatitude = location.Latitude,
                CurrentLongitude = location.Longitude,
                LastUpdate = DateTime.UtcNow
            };

            await client
                .From<SupabaseBus>()
                .Where(b => b.Id == busGuid)
                .Update(updateData);

            System.Diagnostics.Debug.WriteLine($"✅ Ubicación del bus {busId} actualizada: {location.Latitude}, {location.Longitude}");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error actualizando ubicación del bus {busId}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Obtiene la ruta asignada a un bus
    /// </summary>
    public async Task<BusRoute?> GetBusRouteAsync(string busId)
    {
        try
        {
            var client = await _supabaseService.GetClientAsync();
            var busGuid = Guid.Parse(busId);

            // Obtener el bus para saber su route_id
            var bus = await client
                .From<SupabaseBus>()
                .Where(b => b.Id == busGuid)
                .Single();

            if (bus?.RouteId == null)
                return null;

            return await GetRouteByIdAsync(bus.RouteId.Value);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error obteniendo ruta del bus {busId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Obtiene todas las rutas disponibles
    /// </summary>
    public async Task<List<BusRoute>> GetAllRoutesAsync()
    {
        try
        {
            var client = await _supabaseService.GetClientAsync();

            var routesResponse = await client
                .From<SupabaseRoute>()
                .Where(r => r.IsActive == true)
                .Get();

            var routes = new List<BusRoute>();

            foreach (var supabaseRoute in routesResponse.Models)
            {
                var pointsResponse = await client
                    .From<SupabaseRoutePoint>()
                    .Where(rp => rp.RouteId == supabaseRoute.Id)
                    .Order("point_order", PostgrestConstants.Ordering.Ascending)
                    .Get();

                var route = ModelConverter.ToBusRoute(supabaseRoute, pointsResponse.Models.ToList());
                routes.Add(route);
            }

            System.Diagnostics.Debug.WriteLine($"✅ Obtenidas {routes.Count} rutas desde Supabase");
            return routes;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error obteniendo rutas: {ex.Message}");
            return new List<BusRoute>();
        }
    }

    /// <summary>
    /// Guarda la ubicación en el historial
    /// </summary>
    public async Task<bool> SaveLocationHistoryAsync(string busId, GeoLocation location, double? speed = null, double? accuracy = null)
    {
        try
        {
            var client = await _supabaseService.GetClientAsync();

            var historyEntry = ModelConverter.ToLocationHistory(busId, location, speed, accuracy);

            await client
                .From<SupabaseLocationHistory>()
                .Insert(historyEntry);

            System.Diagnostics.Debug.WriteLine($"✅ Ubicación guardada en historial para bus {busId}");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error guardando historial de ubicación: {ex.Message}");
            return false;
        }
    }

    // Métodos auxiliares privados

    private async Task<BusRoute?> GetRouteByIdAsync(Guid routeId)
    {
        try
        {
            var client = await _supabaseService.GetClientAsync();

            var routeResponse = await client
                .From<SupabaseRoute>()
                .Where(r => r.Id == routeId)
                .Single();

            if (routeResponse == null)
                return null;

            var pointsResponse = await client
                .From<SupabaseRoutePoint>()
                .Where(rp => rp.RouteId == routeId)
                .Order("point_order", PostgrestConstants.Ordering.Ascending)
                .Get();

            return ModelConverter.ToBusRoute(routeResponse, pointsResponse.Models.ToList());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error obteniendo ruta {routeId}: {ex.Message}");
            return null;
        }
    }

    private async Task<SupabaseRoute?> GetSupabaseRouteAsync(Guid routeId)
    {
        try
        {
            var client = await _supabaseService.GetClientAsync();
            return await client
                .From<SupabaseRoute>()
                .Where(r => r.Id == routeId)
                .Single();
        }
        catch
        {
            return null;
        }
    }

    // ============================================
    // MÉTODOS DE REALTIME (Polling simplificado)
    // ============================================
    // NOTA: La implementación actual usa polling. Realtime de Supabase requiere
    // configuración adicional en el servidor y ajustes en el SDK.
    // Para la prueba de concepto, polling cada 2 segundos es suficiente.

    /// <summary>
    /// Suscribirse a cambios en tiempo real de la tabla buses (usando polling)
    /// </summary>
    public async Task SubscribeToBusUpdatesAsync()
    {
        // Por ahora, retornamos success. El polling real se hará en el ViewModel
        // usando un timer que llama a GetActiveBusesAsync() cada 2 segundos
        System.Diagnostics.Debug.WriteLine("✅ Modo polling activado para actualizaciones de buses");
        await Task.CompletedTask;
    }

    /// <summary>
    /// Cancelar suscripción a cambios en tiempo real
    /// </summary>
    public async Task UnsubscribeFromBusUpdatesAsync()
    {
        System.Diagnostics.Debug.WriteLine("✅ Modo polling desactivado");
        await Task.CompletedTask;
    }
}
