# Troubleshooting — Librería de sincronización GeoVial

**Proyecto:** GeoVial
**Documento:** troubleshooting_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Technical Writer / Developer Advocate Senior (AG-10), Equipo SDD 2.0
**Tipo Diátaxis:** How-to (orientado a diagnóstico)
**Audiencia:** Developer integrador que reutiliza la librería de sincronización en otros proyectos
**Nivel:** Medio
**Tiempo estimado de lectura:** 14 min

---

Objetivo de tiempo: resolver un error frecuente en menos de 10 minutos. Cada entrada `ISSUE-XX` trae síntoma, causa probable, diagnóstico paso a paso y solución. Los códigos de error citados son los del catálogo de dominio declarado en 05 y 02 (CU-07, CU-12); no se inventan códigos.

## 1. Errores comunes

| ID | Síntoma | Causa probable | Solución |
| --- | --- | --- | --- |
| ISSUE-01 | Tras sincronizar, dos ediciones del mismo campo conviven o el recurso no aparece como conflicto | `CONSOLIDACION_INVALIDA` o `CONFLICTO_NO_MARCADO`: la consolidación no aplicó last-write-wins o no marcó el recurso (RN-04) | Verificar `Timestamp` en UTC y que el backend retorne el conflicto; reintentar el ciclo |
| ISSUE-02 | La cola no se vacía después de recuperar señal | El ciclo no se disparó, el backend no confirmó los `ChangeId`, o no se llamó a vaciar lo confirmado | Confirmar la suscripción a `ConnectivityRestored` y el mapeo de `Confirmed` |
| ISSUE-03 | Un registro queda sin sincronizar como esperás o cae en una bandeja aparte | El cambio no trae coordenada (foto sin EXIF y sin ubicación manual): deriva a la bandeja sin georreferenciar (RN-03) | Es comportamiento esperado; el cambio se encola igual y sincroniza, pero queda sin georreferenciar hasta ubicarlo |
| ISSUE-04 | La subida falla con 401 tras el relogueo en campo | Token de acceso expirado (vida corta ≈60 min) o relogueo sin método de seguridad del teléfono | Renovar el bearer token antes de `PushAsync`; reloguear con el método de seguridad |
| ISSUE-05 | Dos marcadores cercanos no se unifican automáticamente | Conflicto `MarkersWithinRadius`: la librería nunca fusiona marcadores en un mismo radio (RN-02, ADR-06) | Es comportamiento esperado; la unificación es decisión humana posterior (CU-12) |
| ISSUE-06 | La sincronización se corta a la mitad y temés duplicar cambios | `SINCRONIZACION_INTERRUMPIDA`: corte de conexión durante el ciclo | Reanudar el ciclo; la idempotencia por `ChangeId` evita duplicar lo ya confirmado (RC-03) |

## 2. Diagnóstico paso a paso

### ISSUE-01 — Conflicto de sync no consolidado o no marcado

1. Confirmá que cada `ChangeRecord` lleva `Timestamp` en UTC: `change.Timestamp.Offset == TimeSpan.Zero`. Un offset local rompe el orden de last-write-wins.
2. Capturá el `SyncResult`: si `result.Conflicts` está vacío cuando esperabas un choque, el backend no devolvió el conflicto en `SyncResult.Conflicts`.
3. Si la operación lanzó `ConsolidationException` (`CONSOLIDACION_INVALIDA`), el cambio queda en la cola: revisá que el recurso central exista y sea consolidable.
4. Si lanzó `ConflictNotMarkedException` (`CONFLICTO_NO_MARCADO`), el recurso se consolidó sin marca: no se considera sincronizado hasta crear la marca. Reintentá el ciclo.
5. Solución: corregir el `Timestamp`, asegurar que el cliente de backend propague los conflictos, y reintentar `SynchronizeAsync`.

### ISSUE-02 — Cola no drenada

1. Verificá que el evento se disparó: agregá un log en el handler de `ConnectivityRestored`.
2. Inspeccioná los pendientes: `(await queue.GetPendingAsync()).Count`. Si es > 0 tras sincronizar, los cambios no se confirmaron.
3. Revisá `result.Confirmed`: si está vacío, el backend rechazó la subida (revisá ISSUE-04 por 401).
4. Confirmá que marcás y vaciás lo confirmado: por cada `id` en `Confirmed`, `MarkConfirmedAsync(id)` y luego `DrainConfirmedAsync()`.
5. Solución: completar el mapeo `Confirmed → MarkConfirmedAsync → DrainConfirmedAsync`.

### ISSUE-03 — Registro sin georreferenciar

1. Inspeccioná el `Payload` del `ChangeRecord`: si no contiene coordenada, el dato cae a la bandeja sin georreferenciar (RN-03, CU-06 flujo 5.A).
2. Confirmá que el cambio igual se encoló: aparece en `GetPendingAsync` y se sincroniza.
3. Solución: si se esperaba georreferenciación, ubicar el punto manualmente antes de encolar; si no, es el comportamiento correcto y el dato queda pendiente de georreferenciar.

### ISSUE-04 — Token expirado en relogueo

1. Capturá el código HTTP de `PushAsync`/`PullAsync`: un 401 indica token inválido o expirado.
2. Verificá la antigüedad del access token (vida corta ≈60 min).
3. Confirmá que el relogueo en campo se hizo con el método de seguridad del teléfono (precondición para habilitar offline, RN-06).
4. Solución: renovar el bearer token y reasignar el header `Authorization` del `HttpClient` antes de reintentar el ciclo. Los cambios no confirmados siguen en la cola.

### ISSUE-05 — Conflicto de marcadores en un mismo radio

1. Inspeccioná `result.Conflicts`: buscá los de `Kind == MarkersWithinRadius`.
2. Confirmá que `InvolvedResources` lista los marcadores cercanos.
3. La librería nunca los fusiona: la decisión es humana (CU-12).
4. Solución: publicá el conflicto vía `IConflictReporter.PublishAsync` y resolvelo desde la web (unificar o mantener separados). No hay acción automática en el dispositivo.

### ISSUE-06 — Sincronización interrumpida

1. Si `SynchronizeAsync` lanzó `SyncInterruptedException` (`SINCRONIZACION_INTERRUMPIDA`), los cambios no confirmados quedaron en la cola.
2. Verificá pendientes con `GetPendingAsync`: deben coincidir con lo no confirmado.
3. Reanudá el ciclo al recuperar señal: la idempotencia por `ChangeId` (RC-03) evita aplicar dos veces lo ya confirmado.
4. Solución: reintentar `SynchronizeAsync`; no purgar la cola manualmente.

## 3. Logs útiles

| Qué revisar | Nivel | Patrón a buscar | Cómo |
| --- | --- | --- | --- |
| Disparo del ciclo de sync | Information | `ConnectivityRestored` / `SynchronizeAsync start` | Log en el handler del evento |
| Confirmaciones de subida | Information | `Confirmed=` y conteo | Loguear `result.Confirmed.Count` tras cada ciclo |
| Conflictos detectados | Warning | `ConflictKind=FieldConflict` / `MarkersWithinRadius` | Loguear cada `ConflictInfo` de `result.Conflicts` |
| Errores tipados | Error | `CONSOLIDACION_INVALIDA`, `CONFLICTO_NO_MARCADO`, `SINCRONIZACION_INTERRUMPIDA` | Capturar la excepción y loguear su código de dominio |
| Estado de la cola | Debug | `Pending=` y conteo | `GetPendingAsync().Count` antes y después del ciclo |
| Rechazo HTTP del backend | Error | `401`, `409` | Loguear el status code en el cliente `ISyncBackendClient` |

## 4. Cómo reportar un bug

Reportá los bugs como issue en el repositorio de GeoVial (GitHub, donde se publica el paquete, ADR-07). Antes de reportar, revisá si el síntoma corresponde a una entrada `ISSUE-XX` esperada (ISSUE-03 y ISSUE-05 son comportamiento por diseño, no bugs).

Plantilla de issue:

```markdown
### Resumen
<una línea>

### Versión de GeoVial.Sync
<versión del paquete; canal preview o stable>

### Entorno
- Runtime .NET: <versión>
- Plataforma del consumidor: <aplicación móvil / consola / otra>
- Almacén local: <SQLite local / en memoria / otro>

### Pasos para reproducir
1.
2.
3.

### Resultado esperado

### Resultado obtenido
<incluir el código de error de dominio si aplica: CONSOLIDACION_INVALIDA, etc.>

### Issue relacionado de troubleshooting
<ISSUE-XX si corresponde, o "ninguno">

### Logs adjuntos
<extracto con los patrones de la tabla §3; sin datos personales (Ley 25.326)>
```

Datos mínimos a adjuntar: versión del paquete, runtime, plataforma, pasos reproducibles, código de error de dominio y extracto de logs. No adjuntes datos personales (alineado a la Ley 25.326).

Severidad y respuesta esperada:

| Severidad | Criterio | Respuesta esperada |
| --- | --- | --- |
| Crítica | Pérdida o duplicación de datos al sincronizar | Triage prioritario |
| Alta | El pipeline no consolida o no marca conflictos | Triage en el siguiente ciclo de trabajo |
| Media | Comportamiento confuso pero sin pérdida de datos | Backlog priorizado |
| Baja | Documentación o mensaje de error poco claro | Backlog |

---

## Referencias cruzadas

- [contratos-abstractions-sync_v1.0.md](../05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md) — errores tipados y su mapeo a dominio (05).
- [ADR-06-conflictos-last-write-wins-override-manual_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-06-conflictos-last-write-wins-override-manual_v1.0.md) — por qué no hay fusión automática (05).
- [CU-07-sincronizar-cambios-locales_v1.0.md](../02_especificacion_funcional/casos-de-uso/CU-07-sincronizar-cambios-locales_v1.0.md) y [CU-12-resolver-conflictos-sincronizacion-web_v1.0.md](../02_especificacion_funcional/casos-de-uso/CU-12-resolver-conflictos-sincronizacion-web_v1.0.md) — flujos y códigos (02).
- [casos-prueba-referenciales_v1.0.md](../08_calidad_y_pruebas/casos-prueba-referenciales_v1.0.md) — TC-11/TC-12/TC-13 reproducen idempotencia, last-write-wins y reanudación (08).
- [referencia-api_v1.0.md](referencia-api_v1.0.md) y [guia-integracion-aplicacion-movil_v1.0.md](guia-integracion-aplicacion-movil_v1.0.md) (10).

## Control de cambios

| Versión | Fecha | Cambio | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Seis entradas ISSUE-XX con diagnóstico paso a paso, tabla de logs y plantilla de reporte de bug. | AG-10 |
