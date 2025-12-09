using BusTrackerApp.Configuration;

namespace BusTrackerApp.Services;

public class SupabaseService
{
    private static SupabaseService? _instance;
    private Supabase.Client? _client;
    private readonly string _supabaseUrl;
    private readonly string _supabaseKey;

    // Singleton pattern
    public static SupabaseService Instance => _instance ??= new SupabaseService();

    private SupabaseService()
    {
        _supabaseUrl = SupabaseConfig.SupabaseUrl;
        _supabaseKey = SupabaseConfig.SupabaseAnonKey;
    }

    public async Task<Supabase.Client> GetClientAsync()
    {
        if (_client != null)
            return _client;

        try
        {
            var options = new Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            _client = new Supabase.Client(_supabaseUrl, _supabaseKey, options);
            await _client.InitializeAsync();

            System.Diagnostics.Debug.WriteLine("✅ Supabase conectado exitosamente");
            return _client;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error conectando a Supabase: {ex.Message}");
            throw;
        }
    }

    public Supabase.Client? Client => _client;

    public bool IsConnected => _client != null;

    // Método para reconectar si hay problemas
    public async Task ReconnectAsync()
    {
        _client = null;
        await GetClientAsync();
    }
}
