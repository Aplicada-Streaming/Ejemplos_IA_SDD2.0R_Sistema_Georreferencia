# Ejemplo 01 — Consumidor mínimo de la librería de sincronización

**Proyecto:** GeoVial
**Documento:** ejemplo-01-sync-basico_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Developer Advocate / Sample Engineer Senior (AG-11), Equipo SDD 2.0
**Nivel:** Básico
**Ubicación del código:** `/samples/01-sync-basico/`

## 1. Objetivo del sample

Demostrar el camino feliz mínimo de la librería de sincronización `GeoVial.Sync`: dar de alta registros locales en la cola de cambios y sincronizarlos contra un backend mock. Al ejecutarlo, el desarrollador entiende el pipeline básico de la librería (encolar, subir, bajar, vaciar la cola) sin necesidad de un backend real ni de un dispositivo móvil. Es el primer contacto ejecutable con la superficie pública del paquete.

## 2. Nivel

Básico. Es el punto de entrada absoluto a la librería: una aplicación de consola sin red real, sin persistencia durable y sin conflictos. Usa una cola en memoria y un backend simulado que confirma todo. No asume ningún sample previo. Los samples siguientes agregan persistencia durable, conectividad automática y resolución de conflictos (sample 02) y el punto de extensión de backends de archivos (sample 03).

## 3. Prerequisites

| Requisito | Versión mínima | Cómo obtenerlo |
| --- | --- | --- |
| SDK de .NET | .NET 9 o superior (LTS vigente, PROJECT-README §2) | Instalar el SDK desde el canal oficial de .NET. Verificar con `dotnet --info`. |
| Paquete `GeoVial.Sync` | Canal stable de GitHub Packages | Configurar el feed de GitHub Packages del repositorio GeoVial como fuente NuGet (ADR-07). Requiere token con permiso de lectura de paquetes (ver `guia-onboarding-developer_v1.0.md` §1 de 10). |
| Editor o IDE con soporte C# | Cualquiera que compile proyectos .NET | — |

No requiere base de datos, ni red, ni dispositivo Android. La sincronización se hace contra un mock en memoria provisto por el propio sample.

## 4. Cómo correrlo

1. Clonar el repositorio y entrar a la carpeta del sample: `cd samples/01-sync-basico`.
2. Restaurar dependencias: `dotnet restore`.
3. Ejecutar el sample: `dotnet run`.
4. Observar el alta de los registros locales y el resultado de la sincronización en consola.
5. Comparar la salida con el output esperado de §6.

## 5. Estructura del código

```text
01-sync-basico/
├── README.md                  # Instrucciones del sample y resumen del flujo
├── 01-sync-basico.csproj       # Proyecto de consola .NET, referencia a GeoVial.Sync
├── Program.cs                  # Punto de entrada: encola registros y sincroniza
├── MockBackendClient.cs        # ISyncBackendClient en memoria: confirma todo, sin conflictos
└── tests/
    └── SyncBasicoTests.cs      # Verifica el output esperado (cola vaciada, confirmados)
```

`Program.cs` usa la superficie pública documentada en `referencia-api_v1.0.md` (10): obtiene una `IChangeQueue` con `SyncFactory.CreateInMemoryQueue()`, encola dos `ChangeRecord` de ejemplo, construye un `ISyncEngine` con `SyncFactory.CreateEngine(queue, backend)` y llama a `SynchronizeAsync(new SyncOptions(batchSize: 50))`. `MockBackendClient.cs` implementa `ISyncBackendClient.PushAsync` confirmando todos los `ChangeId` recibidos y `PullAsync` devolviendo lista vacía.

## 6. Qué esperar

Salida esperada en consola tras `dotnet run`:

```text
Registros encolados: 2
  Observacion/obs-001 | Create
  Comentario/com-001  | Create
Sincronizando contra el backend mock...
Confirmados: 2
Conflictos : 0
Updates    : 0
Cola restante: 0
```

Una segunda corrida sin encolar nada nuevo confirma la idempotencia: `Confirmados: 0` y `Cola restante: 0`, porque un replay del mismo `ChangeId` no aplica el cambio dos veces (RC-03).

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado esperado |
| --- | --- | --- |
| Encolar más registros | Agregar más llamadas a `queue.EnqueueAsync` en `Program.cs` | `Confirmados` y `Registros encolados` reflejan la nueva cantidad; `Cola restante: 0` |
| Simular un conflicto de campo | Hacer que `MockBackendClient.PushAsync` devuelva un `ConflictInfo` con `Kind = FieldConflict` | `Conflictos : 1`; el recurso aparece en `SyncResult.Conflicts` (puente hacia el sample 02) |
| Simular actualizaciones remotas | Hacer que `PullAsync` devuelva un `ChangeRecord` | `Updates : 1`; la app recibe la actualización bajada del backend |
| Variar el `batchSize` | Cambiar `SyncOptions(batchSize: ...)` | El motor sube los cambios en lotes del tamaño indicado |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-06 | Caso de uso | Materializa el alta de registros locales y su encolado en la cola de cambios. |
| CU-07 | Caso de uso | Ejecuta el pipeline de sincronización (subir locales, bajar remotos, vaciar la cola) contra el backend mock. |
| ADR-05 | Decisión arquitectónica | Demuestra la cola de cambios para soporte offline (aquí en memoria; durable en el sample 02). |
| ADR-07 | Decisión arquitectónica | Consume la librería como paquete de GitHub Packages, programando contra su superficie pública versionada. |
| RC-03 | Regla conceptual de modelo | La idempotencia por `ChangeId` se observa en la segunda corrida (replay sin doble aplicación). |
| `contratos-abstractions-sync_v1.0.md` (05) | Contrato público | Usa `IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, `ChangeRecord`, `SyncResult`, `SyncOptions` tal como se contractualizan. |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Sample básico de consola: alta de registros locales y sincronización contra backend mock en memoria. Generado por AG-11. |
