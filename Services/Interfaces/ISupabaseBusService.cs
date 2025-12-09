using BusTrackerApp.Models;

namespace BusTrackerApp.Services.Interfaces;

/// <summary>
/// Interfaz para operaciones de buses con Supabase
/// </summary>
public interface ISupabaseBusService
{
    /// <summary>
    /// Obtiene todos los buses activos desde Supabase
    /// </summary>
    Task<List<Bus>> GetActiveBusesAsync();

    /// <summary>
    /// Obtiene un bus específico por su ID
    /// </summary>
    Task<Bus?> GetBusByIdAsync(string busId);

    /// <summary>
    /// Actualiza la ubicación de un bus en Supabase
    /// </summary>
    Task<bool> UpdateBusLocationAsync(string busId, GeoLocation location);

    /// <summary>
    /// Obtiene la ruta asignada a un bus
    /// </summary>
    Task<BusRoute?> GetBusRouteAsync(string busId);

    /// <summary>
    /// Obtiene todas las rutas disponibles
    /// </summary>
    Task<List<BusRoute>> GetAllRoutesAsync();

    /// <summary>
    /// Guarda la ubicación en el historial
    /// </summary>
    Task<bool> SaveLocationHistoryAsync(string busId, GeoLocation location, double? speed = null, double? accuracy = null);

    /// <summary>
    /// Evento que se dispara cuando la ubicación de un bus cambia en tiempo real
    /// </summary>
    event EventHandler<Bus>? BusLocationChanged;

    /// <summary>
    /// Suscribirse a cambios en tiempo real de la tabla buses
    /// </summary>
    Task SubscribeToBusUpdatesAsync();

    /// <summary>
    /// Cancelar suscripción a cambios en tiempo real
    /// </summary>
    Task UnsubscribeFromBusUpdatesAsync();
}
