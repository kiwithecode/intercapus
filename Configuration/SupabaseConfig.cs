namespace BusTrackerApp.Configuration;

/// <summary>
/// Configuración de Supabase para el proyecto INTERCAPUS
/// </summary>
public static class SupabaseConfig
{
    // TODO: Reemplazar con tus credenciales reales de Supabase
    // Las puedes encontrar en: https://supabase.com/dashboard/project/[tu-proyecto]/settings/api

    /// <summary>
    /// URL del proyecto Supabase INTERCAPUS
    /// Ejemplo: "https://abcdefghijklmnop.supabase.co"
    /// </summary>
    public const string SupabaseUrl = "https://YOUR_PROJECT_ID.supabase.co";

    /// <summary>
    /// Clave pública/anon de Supabase (es segura para el cliente)
    /// </summary>
    public const string SupabaseAnonKey = "YOUR_ANON_KEY_HERE";

    // IMPORTANTE:
    // - NUNCA uses la "service_role" key en el cliente (solo en backend)
    // - La "anon" key es segura para usar en apps móviles
    // - Supabase usa Row Level Security (RLS) para proteger los datos
}
