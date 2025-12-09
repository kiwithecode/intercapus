using BusTrackerApp.Models;

namespace BusTrackerApp.Services;

public class BusService : IBusService
{
    private readonly List<Bus> _buses = new();
    private readonly List<BusRoute> _routes = new();
    private readonly Dictionary<string, int> _busCurrentPointIndex = new();
    private readonly Dictionary<string, double> _busProgressToNextPoint = new();
    private CancellationTokenSource? _simulationCancellation;
    private Task? _simulationTask;

    public BusService()
    {
        InitializeDemoData();
        InitializeBusPositions();
    }

    private void InitializeDemoData()
    {
        // RUTA 1: Línea Azul - Campus Principal -> Centro Ciudad
        var route1 = new BusRoute
        {
            Id = "ROUTE-001",
            Name = "Línea 1: Campus - Centro",
            Color = "#007AFF",
            RoutePoints = new List<RoutePoint>
            {
                new RoutePoint { Name = "Campus Principal", Location = new GeoLocation(4.6097, -74.0817), Order = 0 },
                new RoutePoint { Name = "Estación Universidad", Location = new GeoLocation(4.6120, -74.0700), Order = 1 },
                new RoutePoint { Name = "Plaza Central", Location = new GeoLocation(4.6150, -74.0650), Order = 2 },
                new RoutePoint { Name = "Terminal Centro", Location = new GeoLocation(4.6180, -74.0600), Order = 3 }
            }
        };

        // RUTA 2: Línea Verde - Campus Norte -> Campus Sur
        var route2 = new BusRoute
        {
            Id = "ROUTE-002",
            Name = "Línea 2: Norte - Sur",
            Color = "#34C759",
            RoutePoints = new List<RoutePoint>
            {
                new RoutePoint { Name = "Campus Norte", Location = new GeoLocation(4.6200, -74.0850), Order = 0 },
                new RoutePoint { Name = "Biblioteca Central", Location = new GeoLocation(4.6150, -74.0820), Order = 1 },
                new RoutePoint { Name = "Cafetería Principal", Location = new GeoLocation(4.6100, -74.0800), Order = 2 },
                new RoutePoint { Name = "Campus Sur", Location = new GeoLocation(4.6050, -74.0780), Order = 3 }
            }
        };

        // RUTA 3: Línea Roja - Centro Histórico -> Zona Industrial
        var route3 = new BusRoute
        {
            Id = "ROUTE-003",
            Name = "Línea 3: Centro Histórico - Industrial",
            Color = "#FF3B30",
            RoutePoints = new List<RoutePoint>
            {
                new RoutePoint { Name = "Centro Histórico", Location = new GeoLocation(4.5980, -74.0760), Order = 0 },
                new RoutePoint { Name = "Parque Santander", Location = new GeoLocation(4.6010, -74.0720), Order = 1 },
                new RoutePoint { Name = "Estación de Buses", Location = new GeoLocation(4.6040, -74.0680), Order = 2 },
                new RoutePoint { Name = "Zona Industrial", Location = new GeoLocation(4.6080, -74.0640), Order = 3 },
                new RoutePoint { Name = "Terminal Industrial", Location = new GeoLocation(4.6110, -74.0600), Order = 4 }
            }
        };

        // RUTA 4: Línea Amarilla - Universidades -> Aeropuerto
        var route4 = new BusRoute
        {
            Id = "ROUTE-004",
            Name = "Línea 4: Universidades - Aeropuerto",
            Color = "#FFCC00",
            RoutePoints = new List<RoutePoint>
            {
                new RoutePoint { Name = "Ciudad Universitaria", Location = new GeoLocation(4.6280, -74.0900), Order = 0 },
                new RoutePoint { Name = "Universidad Nacional", Location = new GeoLocation(4.6350, -74.0830), Order = 1 },
                new RoutePoint { Name = "Portal Norte", Location = new GeoLocation(4.6420, -74.0750), Order = 2 },
                new RoutePoint { Name = "Terminal Aeropuerto", Location = new GeoLocation(4.7020, -74.1467), Order = 3 }
            }
        };

        // RUTA 5: Línea Morada - Residencial Este -> Comercial Oeste
        var route5 = new BusRoute
        {
            Id = "ROUTE-005",
            Name = "Línea 5: Residencial - Comercial",
            Color = "#AF52DE",
            RoutePoints = new List<RoutePoint>
            {
                new RoutePoint { Name = "Barrio Residencial", Location = new GeoLocation(4.6300, -74.0550), Order = 0 },
                new RoutePoint { Name = "Centro Comercial Andino", Location = new GeoLocation(4.6200, -74.0600), Order = 1 },
                new RoutePoint { Name = "Parque 93", Location = new GeoLocation(4.6180, -74.0650), Order = 2 },
                new RoutePoint { Name = "Zona T", Location = new GeoLocation(4.6150, -74.0700), Order = 3 },
                new RoutePoint { Name = "Terminal Occidental", Location = new GeoLocation(4.6100, -74.0800), Order = 4 }
            }
        };

        // RUTA 6: Línea Naranja - Campus Medicina -> Hospitales
        var route6 = new BusRoute
        {
            Id = "ROUTE-006",
            Name = "Línea 6: Medicina - Hospitales",
            Color = "#FF9500",
            RoutePoints = new List<RoutePoint>
            {
                new RoutePoint { Name = "Facultad de Medicina", Location = new GeoLocation(4.6050, -74.0900), Order = 0 },
                new RoutePoint { Name = "Hospital Universitario", Location = new GeoLocation(4.6080, -74.0850), Order = 1 },
                new RoutePoint { Name = "Clínica Central", Location = new GeoLocation(4.6120, -74.0800), Order = 2 },
                new RoutePoint { Name = "Hospital San Ignacio", Location = new GeoLocation(4.6160, -74.0750), Order = 3 }
            }
        };

        // RUTA 7: Línea Cyan - Deportivo -> Recreación
        var route7 = new BusRoute
        {
            Id = "ROUTE-007",
            Name = "Línea 7: Deportivo - Recreación",
            Color = "#5AC8FA",
            RoutePoints = new List<RoutePoint>
            {
                new RoutePoint { Name = "Complejo Deportivo", Location = new GeoLocation(4.5950, -74.0950), Order = 0 },
                new RoutePoint { Name = "Coliseo Cubierto", Location = new GeoLocation(4.6000, -74.0900), Order = 1 },
                new RoutePoint { Name = "Parque Simón Bolívar", Location = new GeoLocation(4.6550, -74.0900), Order = 2 },
                new RoutePoint { Name = "Centro de Convenciones", Location = new GeoLocation(4.6600, -74.0850), Order = 3 },
                new RoutePoint { Name = "Parque de la Independencia", Location = new GeoLocation(4.6050, -74.0820), Order = 4 }
            }
        };

        _routes.Add(route1);
        _routes.Add(route2);
        _routes.Add(route3);
        _routes.Add(route4);
        _routes.Add(route5);
        _routes.Add(route6);
        _routes.Add(route7);

        // BUSES: Un bus por cada línea con su conductor asignado
        _buses.Add(new Bus
        {
            Id = "BUS-001",
            BusNumber = "L1-101",
            PlateNumber = "ABC-123",
            AssignedRoute = route1,
            CurrentLocation = new GeoLocation(4.6097, -74.0817),
            IsActive = true,
            DriverId = "DRIVER-001"
        });

        _buses.Add(new Bus
        {
            Id = "BUS-002",
            BusNumber = "L2-201",
            PlateNumber = "DEF-456",
            AssignedRoute = route2,
            CurrentLocation = new GeoLocation(4.6200, -74.0850),
            IsActive = true,
            DriverId = "DRIVER-002"
        });

        _buses.Add(new Bus
        {
            Id = "BUS-003",
            BusNumber = "L3-301",
            PlateNumber = "GHI-789",
            AssignedRoute = route3,
            CurrentLocation = new GeoLocation(4.5980, -74.0760),
            IsActive = true,
            DriverId = "DRIVER-003"
        });

        _buses.Add(new Bus
        {
            Id = "BUS-004",
            BusNumber = "L4-401",
            PlateNumber = "JKL-012",
            AssignedRoute = route4,
            CurrentLocation = new GeoLocation(4.6280, -74.0900),
            IsActive = true,
            DriverId = "DRIVER-004"
        });

        _buses.Add(new Bus
        {
            Id = "BUS-005",
            BusNumber = "L5-501",
            PlateNumber = "MNO-345",
            AssignedRoute = route5,
            CurrentLocation = new GeoLocation(4.6300, -74.0550),
            IsActive = true,
            DriverId = "DRIVER-005"
        });

        _buses.Add(new Bus
        {
            Id = "BUS-006",
            BusNumber = "L6-601",
            PlateNumber = "PQR-678",
            AssignedRoute = route6,
            CurrentLocation = new GeoLocation(4.6050, -74.0900),
            IsActive = true,
            DriverId = "DRIVER-006"
        });

        _buses.Add(new Bus
        {
            Id = "BUS-007",
            BusNumber = "L7-701",
            PlateNumber = "STU-901",
            AssignedRoute = route7,
            CurrentLocation = new GeoLocation(4.5950, -74.0950),
            IsActive = true,
            DriverId = "DRIVER-007"
        });
    }

    public Task<List<Bus>> GetActiveBusesAsync()
    {
        return Task.FromResult(_buses.Where(b => b.IsActive).ToList());
    }

    public Task<Bus?> GetBusByIdAsync(string busId)
    {
        var bus = _buses.FirstOrDefault(b => b.Id == busId);
        return Task.FromResult(bus);
    }

    public Task<bool> UpdateBusLocationAsync(string busId, GeoLocation location)
    {
        var bus = _buses.FirstOrDefault(b => b.Id == busId);
        if (bus != null)
        {
            bus.CurrentLocation = location;
            bus.LastUpdate = DateTime.UtcNow;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<BusRoute?> GetBusRouteAsync(string busId)
    {
        var bus = _buses.FirstOrDefault(b => b.Id == busId);
        return Task.FromResult(bus?.AssignedRoute);
    }

    public Task<List<BusRoute>> GetAllRoutesAsync()
    {
        return Task.FromResult(_routes.ToList());
    }

    private void InitializeBusPositions()
    {
        // Inicializar posición de cada bus en su ruta
        foreach (var bus in _buses)
        {
            _busCurrentPointIndex[bus.Id] = 0;
            _busProgressToNextPoint[bus.Id] = 0.0;
        }
    }

    public void StartBusSimulation()
    {
        if (_simulationTask != null && !_simulationTask.IsCompleted)
            return;

        _simulationCancellation = new CancellationTokenSource();
        _simulationTask = Task.Run(async () => await SimulateBusMovementAsync(_simulationCancellation.Token));
    }

    public void StopBusSimulation()
    {
        _simulationCancellation?.Cancel();
    }

    private async Task SimulateBusMovementAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                foreach (var bus in _buses.Where(b => b.IsActive))
                {
                    MoveBusAlongRoute(bus);
                }

                await Task.Delay(2000, cancellationToken); // Actualizar cada 2 segundos
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private void MoveBusAlongRoute(Bus bus)
    {
        if (bus.AssignedRoute == null || bus.AssignedRoute.RoutePoints.Count == 0)
            return;

        var currentIndex = _busCurrentPointIndex[bus.Id];
        var progress = _busProgressToNextPoint[bus.Id];

        // Incrementar progreso (velocidad de movimiento)
        progress += 0.05; // 5% por actualización

        if (progress >= 1.0)
        {
            // Avanzar al siguiente punto
            currentIndex = (currentIndex + 1) % bus.AssignedRoute.RoutePoints.Count;
            progress = 0.0;

            _busCurrentPointIndex[bus.Id] = currentIndex;
            _busProgressToNextPoint[bus.Id] = progress;
        }
        else
        {
            _busProgressToNextPoint[bus.Id] = progress;
        }

        // Calcular posición interpolada entre dos puntos
        var currentPoint = bus.AssignedRoute.RoutePoints[currentIndex];
        var nextIndex = (currentIndex + 1) % bus.AssignedRoute.RoutePoints.Count;
        var nextPoint = bus.AssignedRoute.RoutePoints[nextIndex];

        var interpolatedLat = Lerp(currentPoint.Location.Latitude, nextPoint.Location.Latitude, progress);
        var interpolatedLng = Lerp(currentPoint.Location.Longitude, nextPoint.Location.Longitude, progress);

        bus.CurrentLocation = new GeoLocation(interpolatedLat, interpolatedLng);
        bus.LastUpdate = DateTime.UtcNow;
    }

    private double Lerp(double start, double end, double t)
    {
        return start + (end - start) * t;
    }
}
