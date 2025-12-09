# 🚌 Bus Tracker App

Aplicación móvil desarrollada en .NET MAUI 9 para rastreo de buses universitarios en tiempo real.

## 📋 Características

### Para Pasajeros
- 🗺️ Ver mapa con todos los buses activos en tiempo real
- 📍 Ver rutas de los buses universitarios
- 🔄 Actualización automática de ubicaciones
- 📱 Interfaz intuitiva y moderna

### Para Conductores
- 🚌 Ver ruta asignada con puntos de parada
- 📍 Rastreo GPS en tiempo real
- 🗺️ Visualización de la ruta completa
- ▶️ Iniciar/pausar rastreo de ubicación
- 📊 Información de ubicación actual

## 🛠️ Tecnologías Utilizadas

- **.NET MAUI 9** - Framework multiplataforma
- **Microsoft.Maui.Controls.Maps** - Integración de mapas
- **CommunityToolkit.Mvvm** - Patrón MVVM
- **Geolocalización** - Servicios de ubicación nativos

## 📦 Requisitos

- .NET 9 SDK
- Visual Studio 2022 o Visual Studio Code con extensión .NET MAUI
- Para Android: Google Maps API Key
- Para iOS: Xcode y configuración de desarrollo

## 🚀 Instalación

1. **Clonar el repositorio**
   ```bash
   cd /Users/kiwimac/CascadeProjects/BusTrackerApp
   ```

2. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```

3. **Configurar Google Maps API Key (Android)**
   - Obtener una API Key de [Google Cloud Console](https://console.cloud.google.com/)
   - Editar `Platforms/Android/AndroidManifest.xml`
   - Reemplazar `YOUR_GOOGLE_MAPS_API_KEY_HERE` con tu API Key

4. **Compilar el proyecto**
   ```bash
   dotnet build
   ```

5. **Ejecutar en Android**
   ```bash
   dotnet build -t:Run -f net9.0-android
   ```

6. **Ejecutar en iOS**
   ```bash
   dotnet build -t:Run -f net9.0-ios
   ```

## 👤 Modo Demo

La aplicación incluye datos de demostración para facilitar las pruebas:

### Login como Pasajero
- Email: `pasajero@universidad.edu` (o cualquier email sin "conductor")
- Contraseña: cualquiera

### Login como Conductor
- Email: `conductor@universidad.edu` (o cualquier email con "conductor" o "driver")
- Contraseña: cualquiera

## 📱 Estructura del Proyecto

```
BusTrackerApp/
├── Models/              # Modelos de datos
│   ├── User.cs
│   ├── Bus.cs
│   ├── BusRoute.cs
│   ├── Location.cs
│   └── RoutePoint.cs
├── Services/            # Servicios de negocio
│   ├── AuthService.cs
│   ├── BusService.cs
│   └── LocationService.cs
├── ViewModels/          # ViewModels MVVM
│   ├── LoginViewModel.cs
│   ├── PassengerViewModel.cs
│   └── DriverViewModel.cs
├── Views/               # Vistas XAML
│   ├── LoginPage.xaml
│   ├── PassengerPage.xaml
│   └── DriverPage.xaml
└── Converters/          # Convertidores de valores
    ├── InvertedBoolConverter.cs
    ├── StringNotEmptyConverter.cs
    └── IsNotNullConverter.cs
```

## 🔧 Configuración Adicional

### Permisos de Ubicación

#### Android
Los permisos ya están configurados en `AndroidManifest.xml`:
- `ACCESS_FINE_LOCATION`
- `ACCESS_COARSE_LOCATION`

#### iOS
Los permisos ya están configurados en `Info.plist`:
- `NSLocationWhenInUseUsageDescription`
- `NSLocationAlwaysAndWhenInUseUsageDescription`

## 🌐 Integración con Backend (Próximos Pasos)

Actualmente la aplicación usa datos simulados. Para integrar con un backend real:

1. Crear API REST con endpoints:
   - `POST /api/auth/login` - Autenticación
   - `GET /api/buses/active` - Obtener buses activos
   - `PUT /api/buses/{id}/location` - Actualizar ubicación
   - `GET /api/routes` - Obtener rutas

2. Implementar SignalR para actualizaciones en tiempo real

3. Actualizar los servicios en `Services/` para consumir la API

## 📝 Notas de Desarrollo

- Los datos de rutas están configurados para Bogotá (coordenadas de ejemplo)
- El rastreo de ubicación se actualiza cada 5 segundos
- La lista de buses se actualiza cada 10 segundos
- Se requiere conexión a internet para los mapas

## 🤝 Contribuir

1. Fork el proyecto
2. Crear una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

## 👨‍💻 Autor

Desarrollado con ❤️ usando .NET MAUI 9

## 🐛 Problemas Conocidos

- En iOS, el rastreo en segundo plano requiere configuración adicional
- Los mapas requieren conexión a internet activa
- La API Key de Google Maps debe configurarse para producción

## 📞 Soporte

Para reportar problemas o sugerencias, por favor crear un issue en el repositorio.
