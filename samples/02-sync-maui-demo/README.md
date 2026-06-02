# Demo autónoma — Librería de sincronización `GeoVial.Sync`

Demo de evaluación de **US-32** (EP-09 *Librería de sincronización publicada*). Permite a un integrador
externo ejercitar la librería de sincronización **sin depender de GeoVial**: alta de registros locales,
sincronización contra un **backend simulado en memoria**, visualización del estado de la cola
(pendiente → sincronizado) y **resolución básica de conflictos**.

La demo ejercita **solo la superficie pública** (`Abstractions`) de `GeoVial.Sync`, versionada con SemVer
2.0.0 (ADR-07). No referencia el dominio ni la API de GeoVial.

## Estructura

| Proyecto | TFM | En CI | Rol |
| --- | --- | --- | --- |
| `SyncDemo.Nucleo` | `net10.0` | **Sí** (en `GeoVial.slnx` y en el gate de cobertura) | Lógica de la demo sobre los contratos públicos: `BackendSimulado`, `ResolutorConflictos`, `CoordinadorDemo` |
| `SyncDemo.Maui` | `net10.0-android` | No (requiere Android SDK provisionado) | Cáscara visual: capturar, sincronizar, resolver |

El núcleo testeable está cubierto por las pruebas de aceptación **AT-32**
(`tests/GeoVial.UnitTests/AceptacionDemoSyncTests.cs`), que usan la cola SQLite y el motor **reales** de la
librería contra el `BackendSimulado`. La cáscara MAUI queda fuera de la solución/CI por el mismo motivo que
`GeoVial.Mobile`: el empaquetado del APK requiere un Android SDK provisionado (hallazgo del Sprint 11).

## Qué muestra (criterios de aceptación US-32)

1. **Alta local + sincronización → la cola pasa de pendiente a sincronizado.** El `BackendSimulado` confirma
   los cambios subidos; la cola se vacía.
2. **Un cambio que choca con el mock se reporta como conflicto y admite resolución básica.** El backend marca
   un recurso en conflicto (RN-04, última escritura): el cambio se reporta y queda pendiente. La demo ofrece:
   - **Mantener lo local**: el backend despeja el conflicto y la próxima sincronización confirma el cambio.
   - **Aceptar lo remoto**: se descarta el cambio local de la cola.

> La resolución completa sobre el mapa (unificación de marcadores) es alcance de la web (**US-26**); esta es
> la versión básica que la demo necesita para cerrar el ciclo de evaluación.

## Cómo correr

### Núcleo + pruebas de aceptación (sin Android SDK)

```bash
dotnet test tests/GeoVial.UnitTests/GeoVial.UnitTests.csproj --filter FullyQualifiedName~AceptacionDemoSync
```

### Cáscara MAUI en un dispositivo/emulador Android

Requiere el Android SDK y la carga de trabajo `maui-android`:

```bash
export ANDROID_HOME="<ruta-al-android-sdk>"
cd samples/02-sync-maui-demo/SyncDemo.Maui
# Compilar e instalar en un dispositivo conectado por USB:
dotnet build -c Debug -f net10.0-android -r android-arm64 -t:Install -p:AndroidSdkDirectory="$ANDROID_HOME"
```

La app es **íntegramente local**: no abre conexiones de red ni necesita el backend de GeoVial corriendo.
