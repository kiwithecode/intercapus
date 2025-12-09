#!/bin/bash

echo "📱 Preparando deploy a iPhone de kevin..."
echo ""

# Verificar que el iPhone está conectado
IPHONE_CONNECTED=$(xcrun xctrace list devices 2>/dev/null | grep "iPhone de kevin")

if [ -z "$IPHONE_CONNECTED" ]; then
    echo "❌ iPhone de kevin no detectado"
    echo ""
    echo "Solución:"
    echo "  1. Conecta tu iPhone al Mac con el cable"
    echo "  2. Desbloquea el iPhone"
    echo "  3. Toca 'Confiar' cuando aparezca el mensaje"
    echo "  4. Ejecuta este script de nuevo"
    exit 1
fi

echo "✅ iPhone detectado:"
echo "$IPHONE_CONNECTED"
echo ""

# Limpiar compilaciones anteriores
echo "🧹 Limpiando builds anteriores..."
dotnet clean BusTrackerApp.csproj -f net9.0-ios > /dev/null 2>&1

# Compilar
echo "🔨 Compilando para iOS..."
dotnet build BusTrackerApp.csproj -f net9.0-ios -c Debug

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ Error en compilación"
    exit 1
fi

echo ""
echo "✅ Compilación exitosa"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📲 CÓMO INSTALAR EN TU IPHONE"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "🎯 OPCIÓN 1 - VS Code + C# Dev Kit (RECOMENDADO):"
echo ""
echo "   1. Instalar VS Code (si no lo tienes):"
echo "      brew install --cask visual-studio-code"
echo ""
echo "   2. Abrir proyecto:"
echo "      code ."
echo ""
echo "   3. En VS Code:"
echo "      • Extensions (Cmd+Shift+X)"
echo "      • Buscar: C# Dev Kit"
echo "      • Instalar (de Microsoft)"
echo "      • Presionar F5"
echo "      • Seleccionar: iPhone de kevin"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "🎯 OPCIÓN 2 - Visual Studio para Mac:"
echo ""
echo "   1. Instalar Visual Studio:"
echo "      https://visualstudio.microsoft.com/vs/mac/"
echo ""
echo "   2. Abrir proyecto:"
echo "      open BusTrackerApp.sln"
echo ""
echo "   3. En Visual Studio:"
echo "      • Seleccionar 'iPhone de kevin' en el menú superior"
echo "      • Presionar ▶ Run (Cmd+Enter)"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "⚠️  IMPORTANTE - Primera Ejecución:"
echo ""
echo "    La primera vez, verás un mensaje de error en el iPhone."
echo "    En tu iPhone, ve a:"
echo ""
echo "    Configuración → General → Gestión de dispositivos"
echo "    → Confiar en tu certificado"
echo ""
echo "    Luego vuelve a ejecutar desde VS Code o Visual Studio."
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📋 Información del Deploy:"
echo ""
echo "   • Dispositivo: iPhone de kevin (iOS 26.2)"
echo "   • Bundle ID: com.kiwimac.bustrackerapp"
echo "   • Build: Debug"
echo "   • Framework: net9.0-ios"
echo ""
