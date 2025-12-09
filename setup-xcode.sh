#!/bin/bash

echo "🔧 Configuración para Ejecutar en iPhone 16"
echo ""
echo "============================================"
echo "PASO 1: Configurar Apple ID en Xcode"
echo "============================================"
echo ""
echo "Por favor, realiza estos pasos MANUALMENTE en Xcode:"
echo ""
echo "1. Abre Xcode:"
echo "   open -a Xcode"
echo ""
echo "2. En Xcode, ve a: Xcode → Settings (o Preferences)"
echo "3. Pestaña: Accounts"
echo "4. Haz clic en '+' (abajo a la izquierda)"
echo "5. Selecciona: Apple ID"
echo "6. Inicia sesión con tu Apple ID"
echo "7. Selecciona tu cuenta → Manage Certificates..."
echo "8. Haz clic en '+' → Apple Development"
echo "9. Cierra las ventanas"
echo ""
read -p "Presiona ENTER cuando hayas completado estos pasos..."

echo ""
echo "============================================"
echo "PASO 2: Conectar iPhone 16"
echo "============================================"
echo ""
echo "1. Conecta tu iPhone 16 al Mac con cable USB-C"
echo "2. Desbloquea el iPhone"
echo "3. Toca 'Confiar' cuando aparezca el mensaje"
echo ""
read -p "Presiona ENTER cuando hayas conectado el iPhone..."

echo ""
echo "🔍 Verificando dispositivos conectados..."
DEVICES=$(xcrun xctrace list devices | grep iPhone | head -1)

if [ -z "$DEVICES" ]; then
    echo "❌ No se detectó el iPhone"
    echo ""
    echo "Solución:"
    echo "  1. Desconecta y vuelve a conectar el cable"
    echo "  2. Desbloquea el iPhone"
    echo "  3. Toca 'Confiar en esta computadora'"
    echo "  4. Ejecuta este script de nuevo"
    exit 1
fi

echo "✅ iPhone detectado:"
echo "$DEVICES"
echo ""

echo "============================================"
echo "PASO 3: Cambiar Bundle ID"
echo "============================================"
echo ""
echo "Como usas cuenta gratuita, necesitas un Bundle ID único."
echo ""
read -p "Ingresa tu nombre (sin espacios, ej: kiwimac): " USERNAME

if [ -z "$USERNAME" ]; then
    USERNAME="kiwimac"
fi

NEW_BUNDLE_ID="com.$USERNAME.bustrackerapp"
echo ""
echo "Nuevo Bundle ID: $NEW_BUNDLE_ID"
echo ""

# Backup del archivo original
cp BusTrackerApp.csproj BusTrackerApp.csproj.backup

# Cambiar Bundle ID
sed -i '' "s|<ApplicationId>com.companyname.bustrackerapp</ApplicationId>|<ApplicationId>$NEW_BUNDLE_ID</ApplicationId>|g" BusTrackerApp.csproj

echo "✅ Bundle ID actualizado en BusTrackerApp.csproj"
echo ""

echo "============================================"
echo "PASO 4: Compilar para iPhone"
echo "============================================"
echo ""
echo "🔨 Compilando..."

dotnet build -f net9.0-ios -c Debug /p:RuntimeIdentifier=ios-arm64

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ Error en la compilación"
    echo ""
    echo "Esto es normal si es la primera vez."
    echo "Continúa con el siguiente paso para usar Xcode."
fi

echo ""
echo "============================================"
echo "PASO 5: Abrir en Xcode"
echo "============================================"
echo ""
echo "Ahora vamos a abrir el proyecto en Xcode para hacer el deploy."
echo ""

# Buscar el archivo .xcodeproj generado
XCODE_PROJECT=$(find obj/Debug/net9.0-ios -name "*.xcodeproj" | head -1)

if [ -n "$XCODE_PROJECT" ]; then
    echo "✅ Proyecto Xcode encontrado: $XCODE_PROJECT"
    echo ""
    read -p "¿Abrir en Xcode ahora? (s/n): " OPEN_XCODE
    
    if [ "$OPEN_XCODE" = "s" ] || [ "$OPEN_XCODE" = "S" ]; then
        echo "📱 Abriendo Xcode..."
        open "$XCODE_PROJECT"
        echo ""
        echo "============================================"
        echo "EN XCODE:"
        echo "============================================"
        echo ""
        echo "1. En la barra superior, selecciona tu iPhone 16 como target"
        echo "2. Presiona ▶ Run (o Cmd+R)"
        echo "3. Xcode compilará e instalará la app en tu iPhone"
        echo ""
        echo "PRIMERA VEZ:"
        echo "La app se instalará pero no se abrirá."
        echo "En tu iPhone:"
        echo "  - Configuración → General → Gestión de dispositivos"
        echo "  - Toca tu Apple ID o nombre"
        echo "  - Toca 'Confiar en \"Tu Nombre\"'"
        echo "  - Vuelve a Xcode y presiona ▶ Run de nuevo"
        echo ""
        echo "✅ ¡Listo! La app debería ejecutarse en tu iPhone 16"
    fi
else
    echo "⚠️ No se encontró proyecto Xcode generado"
    echo ""
    echo "Solución: Compila primero con .NET:"
    echo "  dotnet build -f net9.0-ios -c Debug"
    echo ""
    echo "Luego ejecuta este script de nuevo."
fi

echo ""
echo "============================================"
echo "🎉 Configuración Completada"
echo "============================================"
echo ""
echo "Para futuras ejecuciones, simplemente:"
echo "  1. Abre Xcode: open $XCODE_PROJECT"
echo "  2. Selecciona tu iPhone 16"
echo "  3. Presiona ▶ Run"
echo ""
