# Guía de integración — Aplicación móvil offline — Librería de sincronización GeoVial

**Proyecto:** GeoVial
**Documento:** guia-integracion-aplicacion-movil_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Technical Writer / Developer Advocate Senior (AG-10), Equipo SDD 2.0
**Tipo Diátaxis:** How-to
**Audiencia:** Developer integrador que reutiliza la librería de sincronización en una aplicación móvil/offline
**Nivel:** Medio
**Tiempo estimado de lectura:** 18 min

---

## 1. Objetivo

Integrar la librería `GeoVial.Sync` en una aplicación móvil que captura datos sin conexión y debe sincronizarlos con un backend al recuperar la red. Al terminar, tu aplicación encolará cambios de forma durable, los subirá y bajará automáticamente cuando vuelva la conexión, y te entregará los conflictos para que los resuelvas. El runtime de referencia es .NET (la app de captura de GeoVial corre sobre .NET MAUI con persistencia local SQLite); los pasos aplican a cualquier aplicación móvil .NET que tenga un almacén local y un backend HTTP.

El porqué de cada decisión (orden subir-bajar, last-write-wins, idempotencia) vive en [conceptos-fundamentales_v1.0.md](conceptos-fundamentales_v1.0.md); esta guía es solo el cómo.

## 2. Prerequisites

- Aplicación móvil .NET ya creada (por ejemplo .NET MAUI) con un almacén local disponible (SQLite local en GeoVial).
- Paquete `GeoVial.Sync` agregado desde GitHub Packages (canal stable), según [guia-onboarding-developer_v1.0.md](guia-onboarding-developer_v1.0.md) §1.
- Un backend HTTP accesible que exponga endpoints de sincronización (en GeoVial, REST sobre `/api/v1/.../sync`).
- Un mecanismo de obtención de token de sesión válido (en GeoVial, flujo ROPC → JWT bearer; el relogueo en campo se habilita con el método de seguridad del teléfono).

## 3. Pasos

### Paso 1. Registrar la cola durable local

Reemplazá la cola en memoria del onboarding por la cola durable respaldada en el almacén local. La librería persiste los `ChangeRecord`; tu aplicación sigue siendo dueña de su modelo de dominio.

```csharp
using GeoVial.Sync.Abstractions;

// Cola durable sobre SQLite local (path del almacén de la app).
IChangeQueue queue = SyncFactory.CreateSqliteQueue(dbPath: localDbPath);
```

Efecto esperado: los cambios encolados sobreviven al cierre de la app antes de sincronizar.

### Paso 2. Implementar el cliente de backend

Implementá `ISyncBackendClient` contra tu API real e inyectá el token de sesión en cada llamada.

```csharp
sealed class RestBackendClient(HttpClient http) : ISyncBackendClient
{
    public async Task<SyncResult> PushAsync(
        IReadOnlyList<ChangeRecord> changes, CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync("api/v1/sync/push", changes, ct);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<SyncResult>(ct))!;
    }

    public async Task<IReadOnlyList<ChangeRecord>> PullAsync(CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<ChangeRecord>>("api/v1/sync/pull", ct)
           ?? new List<ChangeRecord>();
}
```

Configurá el `HttpClient` con el bearer token antes de sincronizar:

```csharp
http.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
```

Efecto esperado: la subida y bajada quedan ligadas a tu backend con la sesión del usuario.

### Paso 3. Construir el motor de sincronización

```csharp
ISyncEngine engine = SyncFactory.CreateEngine(
    queue: queue,
    backend: new RestBackendClient(http));
```

### Paso 4. Encolar cambios al capturar datos offline

Cada vez que tu aplicación produce o edita un dato sin red, encolá un `ChangeRecord` con un `ChangeId` único (clave de idempotencia, RC-03). Acotá el payload (en GeoVial las fotos se comprimen/redimensionan antes de encolar).

```csharp
await queue.EnqueueAsync(new ChangeRecord(
    changeId: Guid.NewGuid().ToString(),
    operationType: OperationType.Create,
    entity: "Observacion",
    entityRef: observacionId,
    timestamp: DateTimeOffset.UtcNow,
    payload: payloadJson));
```

### Paso 5. Disparar la sincronización al recuperar conexión

Suscribite a `IConnectivityMonitor.ConnectivityRestored` para drenar la cola automáticamente (detección de conectividad automática, NFR de GeoVial).

```csharp
IConnectivityMonitor monitor = SyncFactory.CreateConnectivityMonitor();

monitor.ConnectivityRestored += async (_, _) =>
{
    SyncResult result = await engine.SynchronizeAsync(new SyncOptions(batchSize: 50));
    if (result.Conflicts.Count > 0)
        await conflictReporter.PublishAsync(result.Conflicts);
};
```

Efecto esperado: al volver la red, la app sube lo local, baja lo remoto y vacía la cola, sin intervención manual.

### Paso 6. Recolectar los conflictos para resolución posterior

Los conflictos no se resuelven en el dispositivo: se reportan para resolverse después (en GeoVial, desde la web por el jefe de área, CU-12). Implementá o consumí `IConflictReporter`.

```csharp
foreach (ConflictInfo c in result.Conflicts)
{
    // c.Kind: FieldConflict | MarkersWithinRadius
    // c.InvolvedResources: recursos en conflicto a marcar para revisión.
    await conflictReporter.PublishAsync(new[] { c });
}
```

## 4. Verificación

Confirmá que la integración funciona con estos checks:

1. Captura offline. Con la red deshabilitada, capturá un dato. Verificá que `await queue.GetPendingAsync()` lo lista. (Equivale a TC-09 de 08.)
2. Sincronización al recuperar señal. Restablecé la red. Verificá que el evento `ConnectivityRestored` disparó la sincronización y que `SyncResult.Confirmed` contiene los `ChangeId` subidos y la cola quedó vacía. (Equivale a TC-10 y TC-23 de 08.)
3. Idempotencia. Forzá un reintento de subida del mismo `ChangeId` (replay). Verificá que el estado central no cambia: el cambio se aplica una sola vez. (Equivale a TC-11 de 08.)
4. Conflicto marcado. Provocá dos ediciones del mismo campo con marcas temporales distintas. Verificá que prevalece la más reciente y que el recurso aparece en `SyncResult.Conflicts`. (Equivale a TC-12 de 08.)

La estrategia y los casos de prueba que validan estos checks están definidos en 08; esta guía los cita, no los redefine.

## 5. Troubleshooting específico

Subset de problemas frecuentes en esta integración. El detalle de diagnóstico está en [troubleshooting_v1.0.md](troubleshooting_v1.0.md).

| Síntoma en la app móvil | Issue global |
| --- | --- |
| La cola no se vacía tras recuperar señal | [ISSUE-02](troubleshooting_v1.0.md) (cola no drenada) |
| El conflicto de campo no aparece marcado tras sincronizar | [ISSUE-01](troubleshooting_v1.0.md) (conflicto de sync) |
| Un cambio sin coordenada queda sin sincronizar como esperás | [ISSUE-03](troubleshooting_v1.0.md) (registro sin georreferenciar) |
| La subida falla con 401 tras el relogueo en campo | [ISSUE-04](troubleshooting_v1.0.md) (token expirado en relogueo) |
| Dos marcadores cercanos no se unifican solos | [ISSUE-05](troubleshooting_v1.0.md) (marcadores en mismo radio): es el comportamiento esperado (RN-02) |

---

## Referencias cruzadas

- [contratos-abstractions-sync_v1.0.md](../05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md) — interfaces usadas en cada paso (05).
- [ADR-05-sqlite-cola-cambios-offline_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-05-sqlite-cola-cambios-offline_v1.0.md) — cola durable local (05).
- [CU-06-recolectar-observaciones-sin-conexion_v1.0.md](../02_especificacion_funcional/casos-de-uso/CU-06-recolectar-observaciones-sin-conexion_v1.0.md) y [CU-07-sincronizar-cambios-locales_v1.0.md](../02_especificacion_funcional/casos-de-uso/CU-07-sincronizar-cambios-locales_v1.0.md) — captura y sincronización (02).
- [casos-prueba-referenciales_v1.0.md](../08_calidad_y_pruebas/casos-prueba-referenciales_v1.0.md) — TC-09/10/11/12/23 para la verificación (08).
- [guia-onboarding-developer_v1.0.md](guia-onboarding-developer_v1.0.md) y [troubleshooting_v1.0.md](troubleshooting_v1.0.md) (10).

## Control de cambios

| Versión | Fecha | Cambio | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. How-to de integración en aplicación móvil offline: cola durable, cliente de backend, motor, encolado, sync automática y reporte de conflictos. Slug genérico `aplicacion-movil`. | AG-10 |
