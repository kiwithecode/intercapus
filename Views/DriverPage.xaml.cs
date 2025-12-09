using BusTrackerApp.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace BusTrackerApp.Views;

public partial class DriverPage : ContentPage
{
    private readonly DriverViewModel _viewModel;

    public DriverPage(DriverViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        await _viewModel.InitializeAsync();
        
        // Centrar mapa en la ruta
        if (_viewModel.CurrentRoute?.RoutePoints.Any() == true)
        {
            var firstPoint = _viewModel.CurrentRoute.RoutePoints.OrderBy(p => p.Order).First();
            var location = new Location(firstPoint.Location.Latitude, firstPoint.Location.Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(3)));
            
            DrawRoute();
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.CurrentLocation))
        {
            UpdateCurrentLocationPin();
        }
    }

    private void DrawRoute()
    {
        map.Pins.Clear();
        
        if (_viewModel.CurrentRoute?.RoutePoints == null)
            return;

        // Agregar pines para cada punto de la ruta
        foreach (var point in _viewModel.CurrentRoute.RoutePoints.OrderBy(p => p.Order))
        {
            var pin = new Pin
            {
                Label = $"{point.Order + 1}. {point.Name}",
                Type = PinType.Place,
                Location = new Location(point.Location.Latitude, point.Location.Longitude)
            };
            map.Pins.Add(pin);
        }

        // Dibujar línea de ruta
        var polyline = new Polyline
        {
            StrokeColor = Colors.Blue,
            StrokeWidth = 5
        };

        foreach (var point in _viewModel.CurrentRoute.RoutePoints.OrderBy(p => p.Order))
        {
            polyline.Geopath.Add(new Location(point.Location.Latitude, point.Location.Longitude));
        }

        map.MapElements.Add(polyline);
    }

    private void UpdateCurrentLocationPin()
    {
        if (_viewModel.CurrentLocation == null)
            return;

        // Remover pin de ubicación actual anterior si existe
        var currentPin = map.Pins.FirstOrDefault(p => p.Label == "Mi Ubicación");
        if (currentPin != null)
            map.Pins.Remove(currentPin);

        // Agregar nuevo pin de ubicación actual
        var pin = new Pin
        {
            Label = "Mi Ubicación",
            Type = PinType.SavedPin,
            Location = new Location(_viewModel.CurrentLocation.Latitude, _viewModel.CurrentLocation.Longitude)
        };
        map.Pins.Add(pin);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
    }
}
