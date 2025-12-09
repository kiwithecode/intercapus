using Postgrest.Attributes;
using Postgrest.Models;

namespace BusTrackerApp.Models.Supabase;

// ============================================
// MODELO: Usuario (users table)
// ============================================
[Table("users")]
public class SupabaseUser : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("role")]
    public string Role { get; set; } = "passenger"; // "driver" o "passenger"

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("assigned_bus_id")]
    public Guid? AssignedBusId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

// ============================================
// MODELO: Ruta (routes table)
// ============================================
[Table("routes")]
public class SupabaseRoute : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("route_code")]
    public string RouteCode { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("color")]
    public string Color { get; set; } = "#007AFF";

    [Column("description")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

// ============================================
// MODELO: Punto de Ruta (route_points table)
// ============================================
[Table("route_points")]
public class SupabaseRoutePoint : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("route_id")]
    public Guid RouteId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }

    [Column("point_order")]
    public int PointOrder { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

// ============================================
// MODELO: Bus (buses table)
// ============================================
[Table("buses")]
public class SupabaseBus : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("bus_number")]
    public string BusNumber { get; set; } = string.Empty;

    [Column("plate_number")]
    public string PlateNumber { get; set; } = string.Empty;

    [Column("route_id")]
    public Guid? RouteId { get; set; }

    [Column("driver_id")]
    public Guid? DriverId { get; set; }

    [Column("current_latitude")]
    public double? CurrentLatitude { get; set; }

    [Column("current_longitude")]
    public double? CurrentLongitude { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("last_update")]
    public DateTime LastUpdate { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

// ============================================
// MODELO: Historial de Ubicaciones (location_history table)
// ============================================
[Table("location_history")]
public class SupabaseLocationHistory : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("bus_id")]
    public Guid BusId { get; set; }

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }

    [Column("speed")]
    public double? Speed { get; set; }

    [Column("accuracy")]
    public double? Accuracy { get; set; }

    [Column("recorded_at")]
    public DateTime RecordedAt { get; set; }
}

// ============================================
// MODELOS COMPUESTOS (para queries con JOINs)
// ============================================

/// <summary>
/// Representa un bus con toda su información relacionada (ruta y conductor)
/// </summary>
public class BusFullInfo
{
    public Guid Id { get; set; }
    public string BusNumber { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public double? CurrentLatitude { get; set; }
    public double? CurrentLongitude { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastUpdate { get; set; }

    // Información de la ruta
    public Guid? RouteId { get; set; }
    public string? RouteCode { get; set; }
    public string? RouteName { get; set; }
    public string? RouteColor { get; set; }

    // Información del conductor
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public string? DriverEmail { get; set; }

    // Puntos de la ruta (se llenan separadamente)
    public List<SupabaseRoutePoint>? RoutePoints { get; set; }
}
