using BusTrackerApp.Models;

namespace BusTrackerApp.Services;

public class RouteSimulationService : IRouteSimulationService
{
    private CancellationTokenSource? _cancellationTokenSource;
    private List<RoutePoint> _routePoints = new();
    private int _currentPointIndex = 0;
    private double _progressToNextPoint = 0.0;
    private bool _isSimulating = false;

    public event EventHandler<GeoLocation>? SimulatedLocationChanged;

    public bool IsSimulating => _isSimulating;
    public double CurrentProgress => _progressToNextPoint;
    public int CurrentPointIndex => _currentPointIndex;

    public Task<bool> StartSimulationAsync(List<RoutePoint> routePoints)
    {
        if (_isSimulating)
            return Task.FromResult(false);

        if (routePoints == null || routePoints.Count < 2)
            return Task.FromResult(false);

        _routePoints = routePoints.OrderBy(p => p.Order).ToList();
        _currentPointIndex = 0;
        _progressToNextPoint = 0.0;
        _isSimulating = true;

        _cancellationTokenSource = new CancellationTokenSource();

        // Iniciar simulación en background
        _ = Task.Run(async () => await SimulateRouteAsync(_cancellationTokenSource.Token));

        return Task.FromResult(true);
    }

    public Task StopSimulationAsync()
    {
        _cancellationTokenSource?.Cancel();
        _isSimulating = false;
        return Task.CompletedTask;
    }

    private async Task SimulateRouteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _isSimulating)
        {
            try
            {
                // Incrementar progreso (simular movimiento)
                _progressToNextPoint += 0.02; // 2% por actualización (más lento y realista)

                if (_progressToNextPoint >= 1.0)
                {
                    // Avanzar al siguiente punto
                    _currentPointIndex = (_currentPointIndex + 1) % _routePoints.Count;
                    _progressToNextPoint = 0.0;
                }

                // Calcular ubicación interpolada
                var currentPoint = _routePoints[_currentPointIndex];
                var nextIndex = (_currentPointIndex + 1) % _routePoints.Count;
                var nextPoint = _routePoints[nextIndex];

                var interpolatedLat = Lerp(
                    currentPoint.Location.Latitude,
                    nextPoint.Location.Latitude,
                    _progressToNextPoint
                );

                var interpolatedLng = Lerp(
                    currentPoint.Location.Longitude,
                    nextPoint.Location.Longitude,
                    _progressToNextPoint
                );

                var simulatedLocation = new GeoLocation(interpolatedLat, interpolatedLng);

                // Emitir evento de ubicación simulada
                SimulatedLocationChanged?.Invoke(this, simulatedLocation);

                // Actualizar cada 3 segundos para simular velocidad realista de un bus
                await Task.Delay(3000, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en simulación de ruta: {ex.Message}");
            }
        }

        _isSimulating = false;
    }

    private double Lerp(double start, double end, double t)
    {
        return start + (end - start) * t;
    }
}
