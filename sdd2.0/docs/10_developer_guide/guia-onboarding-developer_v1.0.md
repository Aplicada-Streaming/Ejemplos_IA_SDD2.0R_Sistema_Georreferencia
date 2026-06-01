# Guía de onboarding — Librería de sincronización GeoVial

**Proyecto:** GeoVial
**Documento:** guia-onboarding-developer_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Technical Writer / Developer Advocate Senior (AG-10), Equipo SDD 2.0
**Tipo Diátaxis:** Tutorial
**Audiencia:** Developer integrador que reutiliza la librería de sincronización en otros proyectos
**Nivel:** Básico
**Tiempo estimado de lectura:** 12 min

---

Este tutorial te lleva de cero a tener la librería de sincronización drenando una cola contra un backend simulado. El objetivo de tiempo es duro: Hello world en menos de 5 minutos, primer caso real en menos de 30 minutos, integración completa en menos de 1 hora. Si en una corrida real superás esos tiempos, reportalo (la guía se reescribe).

## 1. Prerequisites

| Requisito | Cómo obtenerlo |
| --- | --- |
| SDK de .NET (LTS vigente del proyecto, .NET 9 o superior) | Instalá el SDK desde el canal oficial de .NET. Verificá con `dotnet --info`. |
| Acceso al paquete `GeoVial.Sync` en GitHub Packages | Configurá el feed de GitHub Packages del repositorio GeoVial como fuente NuGet (ADR-07). Necesitás un token con permiso de lectura de paquetes. |
| Editor o IDE con soporte C# | Cualquiera que compile proyectos .NET. |

Verificá el SDK:

```bash
dotnet --info
```

Agregá el feed y el paquete (canal stable):

```bash
dotnet add package GeoVial.Sync
```

> La librería expone su superficie pública en la capa Abstractions (`ISyncEngine`, `IChangeQueue`, `ISyncBackendClient`, `IConflictReporter`, `IConnectivityMonitor`). Programás contra esas interfaces, nunca contra implementaciones concretas.

## 2. Hello world (< 5 min)

Objetivo: dar de alta un registro local y ver que entra en la cola de cambios. No hay backend todavía.

Creá un proyecto de consola y pegá el snippet:

```bash
dotnet new console -n SyncHelloWorld
cd SyncHelloWorld
dotnet add package GeoVial.Sync
```

```csharp
using GeoVial.Sync.Abstractions;

// 1. Obtené una cola de cambios (la implementación durable la provee la librería).
IChangeQueue queue = SyncFactory.CreateInMemoryQueue();

// 2. Encolá un cambio local.
var change = new ChangeRecord(
    changeId: Guid.NewGuid().ToString(),
    operationType: OperationType.Create,
    entity: "Nota",
    entityRef: "nota-001",
    timestamp: DateTimeOffset.UtcNow,
    payload: """{ "texto": "primera nota offline" }""");

await queue.EnqueueAsync(change);

// 3. Leé los pendientes ordenados por marca temporal.
var pendientes = await queue.GetPendingAsync();
Console.WriteLine($"Cambios en la cola: {pendientes.Count}");
foreach (var c in pendientes)
    Console.WriteLine($"  {c.ChangeId} | {c.OperationType} | {c.Entity}/{c.EntityRef}");
```

```bash
dotnet run
```

Output esperado:

```text
Cambios en la cola: 1
  3f2a... | Create | Nota/nota-001
```

Acabás de ver el primer concepto de la librería: la cola de cambios local. Encolaste un cambio y lo leíste pendiente, sin tocar la red.

## 3. Primer caso real (< 30 min)

Objetivo: sincronizar la cola contra un backend simulado (mock), ver el resultado y el estado de la cola después de drenarla.

El puerto hacia el backend es `ISyncBackendClient`. Para el tutorial, implementá un mock en memoria que acepte todo lo que se le sube y no devuelva actualizaciones.

```csharp
using GeoVial.Sync.Abstractions;

// Mock de backend: confirma todo lo subido, sin conflictos ni actualizaciones.
sealed class MockBackendClient : ISyncBackendClient
{
    public Task<SyncResult> PushAsync(IReadOnlyList<ChangeRecord> changes, CancellationToken ct = default)
        => Task.FromResult(new SyncResult(
            confirmed: changes.Select(c => c.ChangeId).ToList(),
            conflicts: Array.Empty<ConflictInfo>(),
            updates: Array.Empty<ChangeRecord>()));

    public Task<IReadOnlyList<ChangeRecord>> PullAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<ChangeRecord>>(Array.Empty<ChangeRecord>());
}
```

Armá el motor y sincronizá:

```csharp
IChangeQueue queue = SyncFactory.CreateInMemoryQueue();
await queue.EnqueueAsync(new ChangeRecord(
    Guid.NewGuid().ToString(), OperationType.Create, "Nota", "nota-001",
    DateTimeOffset.UtcNow, """{ "texto": "primera nota offline" }"""));

ISyncEngine engine = SyncFactory.CreateEngine(
    queue: queue,
    backend: new MockBackendClient());

SyncResult result = await engine.SynchronizeAsync(new SyncOptions(batchSize: 50));

Console.WriteLine($"Confirmados: {result.Confirmed.Count}");
Console.WriteLine($"Conflictos : {result.Conflicts.Count}");
Console.WriteLine($"Updates    : {result.Updates.Count}");
Console.WriteLine($"Cola restante: {(await queue.GetPendingAsync()).Count}");
```

Output esperado:

```text
Confirmados: 1
Conflictos : 0
Updates    : 0
Cola restante: 0
```

Lo que ocurrió, paso a paso:

1. El motor leyó los pendientes de la cola ordenados por `Timestamp`.
2. Los subió al mock vía `ISyncBackendClient.PushAsync`.
3. El mock confirmó el `ChangeId`; el motor marcó ese cambio como confirmado.
4. El motor pidió actualizaciones remotas (`PullAsync`); el mock no devolvió ninguna.
5. El motor vació de la cola lo confirmado: por eso `Cola restante: 0`.

Probá la idempotencia: volvé a sincronizar sin encolar nada nuevo. El resultado es `Confirmados: 0` y la cola sigue vacía; un replay del mismo `ChangeId` no aplica el cambio dos veces (ver CU-07 y RC-03 en 02).

## 4. Integración con un sistema (< 1 hora)

Objetivo: pasar del mock a una integración real en una aplicación móvil/offline, con cola durable y sincronización automática al recuperar conexión.

Los pasos completos copy-paste están en la guía de integración. Acá va el esqueleto para que entiendas el puente:

1. Reemplazá la cola en memoria por la cola durable (persistente local). En GeoVial es SQLite local.
2. Implementá `ISyncBackendClient` contra tu API real (en GeoVial, REST sobre `/api/v1/.../sync`), inyectando el token de sesión.
3. Suscribite a `IConnectivityMonitor.ConnectivityRestored` para disparar `SynchronizeAsync` automáticamente al volver la red.
4. Manejá `IConflictReporter` para listar los `ConflictInfo` que tu aplicación deberá resolver.

```csharp
connectivityMonitor.ConnectivityRestored += async (_, _) =>
{
    var result = await engine.SynchronizeAsync(new SyncOptions(batchSize: 50));
    if (result.Conflicts.Count > 0)
        await conflictReporter.PublishAsync(result.Conflicts);
};
```

Continuá con la guía completa: [guia-integracion-aplicacion-movil_v1.0.md](guia-integracion-aplicacion-movil_v1.0.md).

## 5. Siguientes pasos

Tres rutas, según lo que necesites ahora:

- Entender por qué la librería funciona así (last-write-wins, idempotencia, orden subir-bajar): [conceptos-fundamentales_v1.0.md](conceptos-fundamentales_v1.0.md).
- Buscar la firma exacta de un tipo, método, evento o excepción: [referencia-api_v1.0.md](referencia-api_v1.0.md).
- Ver código completo y ejecutable: samples `01-sync-basico` y `02-sync-maui-demo` del repositorio (referencia a 11), citados en PROJECT-README §14.

Si te trabás, consultá [troubleshooting_v1.0.md](troubleshooting_v1.0.md).

---

## Referencias cruzadas

- [contratos-abstractions-sync_v1.0.md](../05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md) — superficie pública usada en los snippets (05).
- [ADR-07-libreria-sincronizacion-github-packages_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-07-libreria-sincronizacion-github-packages_v1.0.md) — instalación desde GitHub Packages (05).
- [CU-07-sincronizar-cambios-locales_v1.0.md](../02_especificacion_funcional/casos-de-uso/CU-07-sincronizar-cambios-locales_v1.0.md) — pipeline subir-bajar-vaciar (02).
- [casos-prueba-referenciales_v1.0.md](../08_calidad_y_pruebas/casos-prueba-referenciales_v1.0.md) — TC-10/TC-11/TC-13 ilustran sync, idempotencia y reanudación (08).
- [guia-integracion-aplicacion-movil_v1.0.md](guia-integracion-aplicacion-movil_v1.0.md) y [conceptos-fundamentales_v1.0.md](conceptos-fundamentales_v1.0.md) (10).

## Control de cambios

| Versión | Fecha | Cambio | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Prerequisites, Hello world < 5 min, primer caso real < 30 min, puente de integración < 1 hora y siguientes pasos. | AG-10 |
