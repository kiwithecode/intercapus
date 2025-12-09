#!/bin/bash

# Configurar ruta del SDK de Android
export ANDROID_HOME="${ANDROID_HOME:-$HOME/Library/Android/sdk}"
export PATH="$ANDROID_HOME/emulator:$ANDROID_HOME/platform-tools:$PATH"

echo "🔨 Compilando para Android..."
dotnet build -f net9.0-android

if [ $? -ne 0 ]; then
    echo "❌ Error en la compilación"
    exit 1
fi

# Verificar si hay un emulador corriendo
RUNNING=$(adb devices | grep -v "List" | grep "emulator" | wc -l)

if [ $RUNNING -eq 0 ]; then
    echo "📱 Iniciando emulador Android (Pixel_8a)..."
    $ANDROID_HOME/emulator/emulator -avd Pixel_8a -no-snapshot-load > /dev/null 2>&1 &
    
    echo "⏳ Esperando a que el emulador inicie completamente..."
    adb wait-for-device
    
    # Esperar a que el sistema esté completamente arrancado
    echo "⏳ Esperando boot completo (esto puede tomar 30-60 segundos)..."
    while [ "$(adb shell getprop sys.boot_completed 2>/dev/null | tr -d '\r')" != "1" ]; do
        sleep 2
        echo -n "."
    done
    echo ""
    echo "✅ Emulador listo"
else
    echo "✅ Emulador ya está corriendo"
fi

echo "📦 Instalando BusTrackerApp en el emulador..."
dotnet build -f net9.0-android -t:Install

if [ $? -ne 0 ]; then
    echo "❌ Error instalando la app"
    exit 1
fi

echo "🚀 Lanzando BusTrackerApp..."
adb shell monkey -p com.companyname.bustrackerapp -c android.intent.category.LAUNCHER 1 > /dev/null 2>&1

echo "✅ App lanzada exitosamente en el emulador Android!"
echo ""
echo "⚠️ IMPORTANTE: Recuerda configurar tu Google Maps API Key en:"
echo "   Platforms/Android/AndroidManifest.xml línea 24"
echo ""
echo "Para ver los logs en tiempo real, ejecuta:"
echo "adb logcat | grep BusTracker"
