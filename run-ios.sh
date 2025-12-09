#!/bin/bash

echo "🔨 Compilando para iOS..."
dotnet build -f net9.0-ios

if [ $? -ne 0 ]; then
    echo "❌ Error en la compilación"
    exit 1
fi

echo "📱 Abriendo simulador de iOS..."
open -a Simulator

echo "⏳ Esperando a que el simulador inicie (5 segundos)..."
sleep 5

echo "📦 Instalando app en el simulador..."
xcrun simctl install booted bin/Debug/net9.0-ios/iossimulator-arm64/BusTrackerApp.app

if [ $? -ne 0 ]; then
    echo "❌ Error instalando la app. Asegúrate de que el simulador esté iniciado."
    exit 1
fi

echo "🚀 Lanzando BusTrackerApp..."
xcrun simctl launch booted com.companyname.bustrackerapp

if [ $? -ne 0 ]; then
    echo "❌ Error lanzando la app"
    exit 1
fi

echo "✅ App lanzada exitosamente!"
echo ""
echo "Para ver los logs en tiempo real, ejecuta:"
echo "xcrun simctl spawn booted log stream --predicate 'process == \"BusTrackerApp\"'"
