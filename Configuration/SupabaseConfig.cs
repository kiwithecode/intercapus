namespace BusTrackerApp.Configuration;

/// <summary>
/// Configuración de Supabase para el proyecto INTERCAPUS
/// </summary>
public static class SupabaseConfig
{
    /// <summary>
    /// URL del proyecto Supabase INTERCAPUS
    /// </summary>
    public const string SupabaseUrl = "https://eiqzcjgzfcderctxxubi.supabase.co";

    /// <summary>
    /// Clave pública/anon de Supabase (es segura para el cliente)
    /// IMPORTANTE: Necesitas obtener esta clave de tu proyecto Supabase
    /// Ve a: Settings → API → Project API keys → anon/public
    /// </summary>
    public const string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImVpcXpjamd6ZmNkZXJjdHh4dWJpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjUzMDAxNjksImV4cCI6MjA4MDg3NjE2OX0.M8uaauqflwuMmyTMa0M3Y3u65_MjvzPsXoWyWRhzTNk";

    /// <summary>
    /// Host de la base de datos PostgreSQL (solo para referencia, no se usa en el cliente)
    /// </summary>
    public const string DatabaseHost = "db.eiqzcjgzfcderctxxubi.supabase.co";

    // IMPORTANTE:
    // - NUNCA uses la "service_role" key en el cliente (solo en backend)
    // - La "anon" key es segura para usar en apps móviles
    // - Supabase usa Row Level Security (RLS) para proteger los datos
    // - NO pongas la contraseña de la base de datos en el cliente
}
