# Ejemplo 02 — Demo móvil autónoma de evaluación de la librería

**Proyecto:** GeoVial
**Documento:** ejemplo-02-demo-movil-autonoma_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Developer Advocate / Sample Engineer Senior (AG-11), Equipo SDD 2.0
**Nivel:** Intermedio
**Ubicación del código:** `/samples/02-sync-maui-demo/`

## 1. Objetivo del sample

Demostrar la librería de sincronización `GeoVial.Sync` dentro de una aplicación móvil autónoma, ajena al sistema GeoVial, pensada para que un evaluador la corra y decida si reutiliza la librería en otros proyectos (requerimiento explícito del cliente, PROJECT-README §14). El sample ejercita el escenario offline-first completo: alta de registros locales, cola durable, sincronización contra un mock server, visualización del estado de la cola y resolución básica de conflictos. Al terminar, el evaluador sabe cómo integrar la librería en una app móvil con almacén local y backend HTTP.

## 2. Nivel

Intermedio. Asume que el lector ya corrió el sample 01 (pipeline básico contra mock en memoria). Agrega tres capacidades que el sample básico no demostraba: una cola durable que sobrevive al cierre de la app (no en memoria), la sincronización automática disparada al recuperar conexión vía `IConnectivityMonitor`, y la recolección y visualización de conflictos vía `IConflictReporter`. El runtime real es una app .NET MAUI sobre Android; la integración es la documentada en `guia-integracion-aplicacion-movil_v1.0.md` (10).

## 3. Prerequisites

| Requisito | Versión mínima | Cómo obtenerlo |
| --- | --- | --- |
| SDK de .NET | .NET 9 o superior (PROJECT-README §2) | Instalar el SDK desde el canal oficial de .NET. Verificar con `dotnet --info`. |
| Workload de .NET MAUI | Acorde al SDK instalado | `dotnet workload install maui` |
| Dispositivo o emulador Android | Android 8.0 (API 26) o superior (PROJECT-README §12) | Dispositivo físico por USB en modo desarrollador o emulador Android. |
| Paquete `GeoVial.Sync` | Canal stable de GitHub Packages | Feed de GitHub Packages configurado como fuente NuGet (ADR-07), con token de lectura de paquetes. |
| Mock server local | Incluido en el sample | Se levanta con el script del paso 2 (no requiere el backend real de GeoVial). |

La demo es ajena al sistema: no necesita la base SQL Server ni la API REST de GeoVial. El backend de sincronización es un mock server local incluido en el propio sample.

## 4. Cómo correrlo

1. Entrar a la carpeta del sample: `cd samples/02-sync-maui-demo`.
2. Levantar el mock server local de sincronización: `dotnet run --project mock-server`.
3. Restaurar y compilar la app móvil: `dotnet build -t:Run -f net9.0-android src/DemoApp` (con el dispositivo o emulador Android conectado).
4. En la app, capturar registros de ejemplo sin red y luego habilitar la conectividad para disparar la sincronización automática.
5. Observar en pantalla el estado de la cola y la lista de conflictos detectados; contrastar con §6.

## 5. Estructura del código

```text
02-sync-maui-demo/
├── README.md                       # Guion de la demo y orden de ejecución
├── mock-server/
│   ├── mock-server.csproj          # Mini backend HTTP que simula push/pull
│   └── Program.cs                  # Endpoints /sync/push y /sync/pull con conflictos sembrados
└── src/
    └── DemoApp/
        ├── DemoApp.csproj          # App .NET MAUI (net9.0-android), referencia a GeoVial.Sync
        ├── MainPage.xaml(.cs)      # UI: alta de registros, estado de la cola, lista de conflictos
        ├── RestBackendClient.cs    # ISyncBackendClient contra el mock server (push/pull HTTP)
        ├── SyncCoordinator.cs      # Cola durable, motor, monitor de conectividad y reporter
        └── tests/
            └── SyncDemoTests.cs    # Verifica encolado durable, sync y marcado de conflicto
```

`SyncCoordinator.cs` arma la integración de `guia-integracion-aplicacion-movil_v1.0.md` (10): cola durable con `SyncFactory.CreateSqliteQueue(dbPath)`, motor con `SyncFactory.CreateEngine(queue, backend)`, monitor con `SyncFactory.CreateConnectivityMonitor()` suscrito a `ConnectivityRestored`, y un `IConflictReporter` que publica los `ConflictInfo` devueltos en `SyncResult.Conflicts`. `RestBackendClient.cs` implementa `ISyncBackendClient.PushAsync`/`PullAsync` contra el mock server.

## 6. Qué esperar

La app muestra primero los registros encolados sin red, luego el resultado de la sincronización al recuperar conexión. Salida equivalente (panel de estado de la app y log de la demo):

```text
[Cola] Registros pendientes: 3
  Observacion/obs-101 | Create
  Comentario/com-101  | Update
  Etiqueta/eti-101    | Create
[Conectividad] Restaurada -> sincronizando (batchSize=50)
[Sync] Confirmados: 3
[Sync] Conflictos : 1
[Sync] Updates    : 2
[Cola] Registros pendientes: 0
[Conflictos] 1 pendiente de resolucion:
  conf-01 | FieldConflict | Comentario/com-101
```

El conflicto sembrado por el mock server (`FieldConflict` sobre `Comentario/com-101`) prevalece por última escritura (RN-04) y queda marcado, no descartado. La demo no resuelve el conflicto en el dispositivo: lo reporta para resolución posterior (en GeoVial, desde la web por el jefe de área, CU-12).

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado esperado |
| --- | --- | --- |
| Cortar la conexión a mitad de la subida | Detener el mock server durante la sincronización | Los cambios no confirmados quedan en la cola y se reanudan sin duplicar al volver la red (CU-07 flujo 5.A) |
| Sembrar un conflicto de marcadores en un mismo radio | Configurar el mock para devolver un `ConflictInfo` con `Kind = MarkersWithinRadius` | El panel lista el conflicto sin unificar los marcadores (RN-02) |
| Cerrar y reabrir la app antes de sincronizar | Encolar registros y reiniciar la app | Los registros siguen en la cola durable tras el reinicio (cola SQLite, ADR-05) |
| Variar el tamaño de lote | Ajustar `SyncOptions(batchSize: ...)` | La subida se hace en lotes del tamaño indicado |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-06 | Caso de uso | Recolecta registros locales sin conexión y los encola en la cola durable. |
| CU-07 | Caso de uso | Sube los cambios, baja actualizaciones, consolida por última escritura y marca conflictos al recuperar conexión. |
| CU-12 | Caso de uso | Recolecta los conflictos para resolución posterior (la demo los reporta; GeoVial los resuelve desde la web). |
| ADR-05 | Decisión arquitectónica | Materializa la cola durable SQLite que sobrevive al cierre de la app. |
| ADR-06 | Decisión arquitectónica | Demuestra last-write-wins con marca de conflicto y resolución diferida. |
| ADR-07 | Decisión arquitectónica | Evalúa la librería publicada como paquete para reuso en otros proyectos. |
| RN-04 | Regla de negocio | El conflicto de campo prevalece por última escritura y queda marcado, no descartado. |
| NFR "Detección de conectividad" (PROJECT-README §13) | Requisito no funcional | La sincronización se dispara sola al recuperar señal vía `IConnectivityMonitor.ConnectivityRestored`. |
| `contratos-abstractions-sync_v1.0.md` (05) | Contrato público | Usa `IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, `IConnectivityMonitor`, `IConflictReporter` y los tipos públicos. |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Demo móvil autónoma de evaluación: cola durable, mock server, estado de la cola y resolución básica de conflictos. Generado por AG-11. |
