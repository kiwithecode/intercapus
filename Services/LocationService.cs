using BusTrackerApp.Models;

namespace BusTrackerApp.Services;

public class LocationService : ILocationService
{
    private CancellationTokenSource? _cancelTokenSource;
    private bool _isTracking = false;

    public event EventHandler<GeoLocation>? LocationChanged;

    public async Task<GeoLocation?> GetCurrentLocationAsync()
    {
        try
        {
            var location = await Geolocation.Default.GetLastKnownLocationAsync();
            
            if (location == null)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                location = await Geolocation.Default.GetLocationAsync(request);
            }

            if (location != null)
            {
                return new GeoLocation(location.Latitude, location.Longitude);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error obteniendo ubicación: {ex.Message}");
        }

        return null;
    }

    public async Task<bool> StartTrackingAsync()
    {
        if (_isTracking)
            return true;

        try
        {
            // Primero verificar/solicitar "WhenInUse"
            var whenInUseStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (whenInUseStatus != PermissionStatus.Granted)
            {
                whenInUseStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (whenInUseStatus != PermissionStatus.Granted)
            {
                System.Diagnostics.Debug.WriteLine("Permiso de ubicación 'WhenInUse' denegado");
                return false;
            }

            // Para conductores: Intentar solicitar "Always" para tracking en background (iOS)
            // Esto permite que la app rastree ubicación incluso cuando está en segundo plano
            #if IOS
            try
            {
                var alwaysStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                if (alwaysStatus != PermissionStatus.Granted)
                {
                    alwaysStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();

                    if (alwaysStatus == PermissionStatus.Granted)
                    {
                        System.Diagnostics.Debug.WriteLine("Permiso de ubicación 'Always' concedido - tracking en background habilitado");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Permiso de ubicación 'Always' denegado - tracking limitado a foreground");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"No se pudo solicitar permiso 'Always': {ex.Message}");
            }
            #endif

            _cancelTokenSource = new CancellationTokenSource();
            _isTracking = true;

            _ = Task.Run(async () =>
            {
                while (!_cancelTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var location = await GetCurrentLocationAsync();
                        if (location != null)
                        {
                            LocationChanged?.Invoke(this, location);
                        }
                        await Task.Delay(5000, _cancelTokenSource.Token); // Actualizar cada 5 segundos
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error en tracking loop: {ex.Message}");
                    }
                }
            }, _cancelTokenSource.Token);

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error iniciando tracking: {ex.Message}");
            return false;
        }
    }

    public Task StopTrackingAsync()
    {
        if (_cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
        {
            _cancelTokenSource.Cancel();
        }
        _isTracking = false;
        return Task.CompletedTask;
    }
}
