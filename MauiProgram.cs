using Microsoft.Extensions.Logging;
using BusTrackerApp.Services;
using BusTrackerApp.ViewModels;
using BusTrackerApp.Views;

namespace BusTrackerApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiMaps()  // iOS usa Apple Maps (no requiere API Key), Android usa Google Maps (requiere API Key en AndroidManifest.xml)
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Registrar servicios
		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<IBusService, BusService>();
		builder.Services.AddSingleton<ILocationService, LocationService>();
		builder.Services.AddSingleton<IRouteSimulationService, RouteSimulationService>();

		// Registrar ViewModels
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<PassengerViewModel>();
		builder.Services.AddTransient<DriverViewModel>();

		// Registrar Views
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<PassengerPage>();
		builder.Services.AddTransient<DriverPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
