# Instrucciones para Prueba con 2 Dispositivos Físicos

## Resumen de la Implementación Supabase

✅ **Completado:**
1. Cuenta Supabase creada - Proyecto: INTERCAPUS
2. Base de datos PostgreSQL configurada con 5 tablas
3. Datos de prueba insertados:
   - 1 conductor: `conductor@udla.edu.ec`
   - 1 pasajero: `pasajero@udla.edu.ec`
   - 1 bus: `UDLA-001` (Placa: PIC-1234)
   - 1 ruta UDLA con 2 puntos:
     - Punto A: UDLAPark (-0.1865, -78.4808)
     - Punto B: Av. de los Granados y Colimes (-0.1950, -78.4850)
4. SDK supabase-csharp v0.16.2 instalado
5. SupabaseBusService implementado con operaciones CRUD
6. DriverViewModel modificado para usar Supabase
7. PassengerViewModel modificado para usar Supabase
8. Polling cada 2 segundos para actualizaciones en tiempo real

## Arquitectura de Sincronización

### Driver (Conductor)
- **GPS Real**: Cuando el conductor inicia tracking, su ubicación GPS se envía a Supabase cada vez que cambia
- **Modo Simulación**: Simula movimiento a lo largo de la ruta UDLA
- **Historial**: Cada ubicación se guarda en `location_history` con timestamp, speed y accuracy
- **Actualización**: Llama a `UpdateBusLocationAsync()` que actualiza `current_latitude` y `current_longitude` en la tabla `buses`

### Passenger (Pasajero)
- **Polling**: Cada 2 segundos llama a `GetActiveBusesAsync()` para obtener buses activos desde Supabase
- **Visualización**: Muestra buses en el mapa con sus ubicaciones actuales
- **Filtrado**: Puede filtrar buses por ruta (actualmente solo hay ruta UDLA)

## Preparación para Prueba

### Requisitos
- 2 dispositivos físicos (iOS o Android)
- Conexión a internet en ambos dispositivos
- Permisos de ubicación otorgados

### Credenciales de Prueba

**Dispositivo 1 - Conductor:**
- Email: `conductor@udla.edu.ec`
- Contraseña: (cualquiera - autenticación local)
- Rol: Driver

**Dispositivo 2 - Pasajero:**
- Email: `pasajero@udla.edu.ec`
- Contraseña: (cualquiera - autenticación local)
- Rol: Passenger

## Pasos para Probar

### 1. Compilar y Desplegar

**Para iOS (ambos dispositivos):**
```bash
# Device 1 (Conductor)
dotnet build -t:Run -f net9.0-ios -p:RuntimeIdentifier=ios-arm64 -p:ArchiveOnBuild=false

# Device 2 (Pasajero)
dotnet build -t:Run -f net9.0-ios -p:RuntimeIdentifier=ios-arm64 -p:ArchiveOnBuild=false
```

**Para Android (ambos dispositivos):**
```bash
# Device 1 (Conductor)
dotnet build -t:Run -f net9.0-android

# Device 2 (Pasajero)
dotnet build -t:Run -f net9.0-android
```

### 2. Iniciar Sesión

**Dispositivo 1:**
1. Abrir la app
2. Email: `conductor@udla.edu.ec`
3. Contraseña: cualquiera
4. Role: Driver
5. Tap "Iniciar Sesión"

**Dispositivo 2:**
1. Abrir la app
2. Email: `pasajero@udla.edu.ec`
3. Contraseña: cualquiera
4. Role: Passenger
5. Tap "Iniciar Sesión"

### 3. Prueba con GPS Real

**Dispositivo 1 (Conductor):**
1. En la pantalla del conductor, verás el bus UDLA-001 asignado
2. Verás la ruta UDLA con 2 puntos
3. Tap "Iniciar Tracking GPS"
4. Camina con el dispositivo (o muévete en vehículo)
5. Observa que "GPS Real" aparece en el modo

**Dispositivo 2 (Pasajero):**
1. En la pantalla del pasajero, deberías ver el mapa
2. Deberías ver el bus UDLA-001 en el mapa
3. El bus debería moverse en el mapa cada 2 segundos cuando el conductor se mueva
4. Observa la actualización automática del marcador del bus

### 4. Prueba con Simulación

**Dispositivo 1 (Conductor):**
1. Tap "Detener Tracking" (si está activo)
2. Tap "Iniciar Simulación"
3. El bus comenzará a moverse automáticamente a lo largo de la ruta UDLA
4. Observa que "Modo Demo (Simulación)" aparece

**Dispositivo 2 (Pasajero):**
1. Deberías ver el bus moviéndose suavemente a lo largo de la ruta
2. El movimiento debería actualizarse cada 2 segundos
3. El bus seguirá la ruta desde UDLAPark hasta Av. de los Granados y Colimes

### 5. Verificar en Supabase

Puedes verificar los datos en tiempo real en Supabase:

1. Ve a: https://eiqzcjgzfcderctxxubi.supabase.co
2. Table Editor → `buses`
3. Observa que `current_latitude` y `current_longitude` cambian cuando el conductor se mueve
4. Table Editor → `location_history`
5. Verás un registro de todas las ubicaciones por donde pasó el bus

## Qué Deberías Observar

### ✅ Comportamiento Esperado

1. **Sincronización en Tiempo Real:**
   - Cuando el conductor se mueve (GPS real o simulación), el pasajero ve el movimiento
   - Delay máximo: 2 segundos (debido al polling)

2. **Ubicación Precisa:**
   - En GPS real: ubicación del conductor con accuracy reportada
   - En simulación: movimiento suave a lo largo de la ruta UDLA

3. **Debug Output:**
   - En Xcode/Android Studio deberías ver logs como:
     - `✅ Ubicación del bus UDLA-001 actualizada: -0.1865, -78.4808`
     - `✅ PassengerViewModel cargó 1 buses desde Supabase`
     - `✅ Supabase conectado exitosamente`

### ❌ Problemas Potenciales

1. **Bus no aparece en el mapa:**
   - Verificar conexión a internet
   - Revisar logs en debug console
   - Verificar que el bus está activo en Supabase

2. **Ubicación no se actualiza:**
   - Verificar permisos de ubicación en Settings
   - En iOS: Settings → Privacy → Location Services → BusTrackerApp → "While Using"
   - En Android: Settings → Apps → BusTrackerApp → Permissions → Location → Allow

3. **Error de conexión a Supabase:**
   - Verificar que la URL y anon key en `SupabaseConfig.cs` son correctas
   - Verificar conexión a internet en ambos dispositivos

## Verificación de Datos

### Query SQL para verificar datos de prueba:

```sql
-- Ver el bus y su ubicación actual
SELECT
    b.bus_number,
    b.plate_number,
    b.current_latitude,
    b.current_longitude,
    b.last_update,
    r.name as route_name
FROM buses b
LEFT JOIN routes r ON b.route_id = r.id
WHERE b.bus_number = 'UDLA-001';

-- Ver historial de ubicaciones
SELECT
    bus_id,
    latitude,
    longitude,
    speed,
    accuracy,
    recorded_at
FROM location_history
ORDER BY recorded_at DESC
LIMIT 10;

-- Ver usuarios
SELECT email, role, phone_number FROM users;
```

## Notas Importantes

1. **Autenticación Local**: Por ahora, la autenticación es local (no usa Supabase Auth). Solo verifica que el email exista en memoria.

2. **Asignación de Bus**: El conductor debe tener un `assigned_bus_id` en la base de datos. Ya está configurado en los datos de prueba.

3. **Ruta UDLA**: La ruta tiene solo 2 puntos. En producción, necesitarás agregar más puntos intermedios para una ruta más detallada.

4. **Polling vs Realtime**: Actualmente usa polling cada 2 segundos. Para producción, considera implementar WebSocket Realtime de Supabase.

5. **Historial de Ubicaciones**: Cada ubicación se guarda en `location_history`. Considera implementar limpieza automática de datos antiguos (ej: > 24 horas).

## Próximos Pasos (Post-Prueba)

Si la prueba es exitosa:

1. ✅ Implementar Supabase Auth (en lugar de autenticación local)
2. ✅ Agregar más puntos a la ruta UDLA para mayor precisión
3. ✅ Implementar WebSocket Realtime (en lugar de polling)
4. ✅ Agregar más rutas y buses
5. ✅ Implementar notificaciones push cuando el bus se acerca
6. ✅ Agregar estimación de tiempo de llegada (ETA)
7. ✅ Implementar panel de administración

## Soporte

Si encuentras problemas durante la prueba, revisa:
1. Logs en Debug Console (Xcode/Android Studio)
2. Datos en Supabase Table Editor
3. Conexión a internet en ambos dispositivos
4. Permisos de ubicación otorgados

---

**Proyecto:** BusTrackerApp - INTERCAPUS
**Supabase URL:** https://eiqzcjgzfcderctxxubi.supabase.co
**Última actualización:** 2025-12-09
