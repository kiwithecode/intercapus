using BusTrackerApp.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.Specialized;

namespace BusTrackerApp.Views;

public partial class PassengerPage : ContentPage
{
    private readonly PassengerViewModel _viewModel;
    private CancellationTokenSource? _updateCancellation;

    public PassengerPage(PassengerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Suscribirse a cambios en la colección de buses
        _viewModel.ActiveBuses.CollectionChanged += OnActiveBusesChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.InitializeAsync();

        // Centrar mapa en ubicación inicial (Bogotá - ejemplo)
        var location = new Location(4.6097, -74.0817);
        map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(5)));

        // Actualizar pines del mapa
        UpdateMapPins();

        // Dibujar líneas de rutas
        DrawRouteLines();

        // Iniciar actualización continua de pines
        StartContinuousMapUpdate();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _updateCancellation?.Cancel();
    }

    private void OnActiveBusesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateMapPins();
    }

    private void StartContinuousMapUpdate()
    {
        _updateCancellation = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            while (!_updateCancellation.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(2000, _updateCancellation.Token);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdateMapPins();
                    });
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        });
    }

    private void UpdateMapPins()
    {
        map.Pins.Clear();

        foreach (var bus in _viewModel.ActiveBuses)
        {
            if (bus.CurrentLocation != null)
            {
                var pin = new Pin
                {
                    Label = $"Bus {bus.BusNumber}",
                    Address = bus.AssignedRoute?.Name ?? "Sin ruta",
                    Type = PinType.Place,
                    Location = new Location(bus.CurrentLocation.Latitude, bus.CurrentLocation.Longitude)
                };
                map.Pins.Add(pin);
            }
        }
    }

    private void DrawRouteLines()
    {
        map.MapElements.Clear();

        foreach (var route in _viewModel.AvailableRoutes)
        {
            if (route.RoutePoints.Count < 2)
                continue;

            var polyline = new Polyline
            {
                StrokeColor = Color.FromArgb(route.Color),
                StrokeWidth = 4
            };

            foreach (var point in route.RoutePoints.OrderBy(p => p.Order))
            {
                polyline.Geopath.Add(new Location(point.Location.Latitude, point.Location.Longitude));
            }

            // Cerrar el ciclo de la ruta (volver al punto inicial)
            var firstPoint = route.RoutePoints.OrderBy(p => p.Order).First();
            polyline.Geopath.Add(new Location(firstPoint.Location.Latitude, firstPoint.Location.Longitude));

            map.MapElements.Add(polyline);
        }
    }
}
