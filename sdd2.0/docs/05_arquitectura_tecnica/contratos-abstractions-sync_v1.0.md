# Contrato de Abstractions — Librería de sincronización — GeoVial

**Proyecto:** GeoVial
**Documento:** contratos-abstractions-sync_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Trazabilidad upstream:** ADR-07, ADR-05, ADR-06; CU-06, CU-07; RN-04

## 1. Alcance del contrato

Superficie pública (capa Abstractions) de la librería de sincronización `GeoVial.Sync`, publicada como paquete en GitHub Packages para reuso en otros proyectos (ADR-07, PROJECT-README §1, §10, §14). Materializa CU-06 (captura local y encolado) y CU-07 (subir cambios locales, bajar actualizaciones, consolidar last-write-wins, marcar conflictos). Este contrato define qué expone la librería a sus consumidores, no su implementación interna.

## 2. Formato

Contrato de API pública en .NET expresado como interfaces y tipos de la capa Abstractions. Estable y versionado con SemVer 2.0.0; cualquier cambio incompatible bumpea MAJOR (ADR-07). La superficie interna (implementaciones concretas) no forma parte del contrato y puede cambiar sin bump MAJOR.

## 3. Operaciones (superficie pública)

| Elemento Abstractions | Responsabilidad |
| --- | --- |
| `IChangeQueue` | Encolar un cambio local, leer pendientes ordenados por marca temporal, marcar confirmado, vaciar lo sincronizado |
| `ISyncEngine` | Orquestar el pipeline: subir cambios locales, bajar actualizaciones, reportar conflictos; idempotencia por identificador de cambio |
| `ISyncBackendClient` | Puerto hacia el backend (la implementación REST contra `/api/v1/.../sync` vive fuera de Abstractions) |
| `IConflictReporter` | Exponer los conflictos detectados (last-write-wins y marcadores en un mismo radio) para resolución posterior |
| `IConnectivityMonitor` | Notificar la recuperación de conexión para disparar la sincronización automática |

## 4. Esquemas de datos (tipos públicos)

- `ChangeRecord` { ChangeId (identificador único de idempotencia), OperationType (Create/Update), Entity, EntityRef, Timestamp (UTC), Payload }.
- `SyncResult` { Confirmed[], Conflicts[], Updates[] }.
- `ConflictInfo` { ConflictId, Kind (FieldConflict | MarkersWithinRadius), InvolvedResources[] }.
- `SyncOptions` { BatchSize, RetryPolicy }.

`ChangeId` es la clave de idempotencia (RC-03): un reintento con el mismo `ChangeId` no aplica el cambio dos veces. La consolidación usa `Timestamp` para last-write-wins (RN-04).

## 5. Manejo de errores

La librería expone errores tipados en su superficie pública, alineados con el catálogo de dominio:

| Error público | Causa | Equivalente de dominio |
| --- | --- | --- |
| `ConsolidationException` | No se pudo aplicar last-write-wins | `CONSOLIDACION_INVALIDA` (RN-04) |
| `ConflictNotMarkedException` | Recurso consolidado sin marca de conflicto | `CONFLICTO_NO_MARCADO` (RN-04) |
| `SyncInterruptedException` | Corte de conexión durante la sincronización | `SINCRONIZACION_INTERRUMPIDA` (CU-07) |

Los consumidores que usen el cliente REST contra GeoVial reciben además las respuestas Problem Details del contrato REST (`contratos-rest_v1.0.md` §5).

## 6. Versionado del contrato

- SemVer 2.0.0 + Conventional Commits (PROJECT-README §10), versión calculada con MinVer/Nerdbank.GitVersioning.
- Cualquier breaking change de la superficie Abstractions (firma, tipo público removido o cambiado) bumpea MAJOR.
- Cambios aditivos compatibles (nuevo método con default, nuevo tipo) bumpean MINOR.
- Correcciones internas sin cambio de superficie bumpean PATCH.
- Canales preview (prerelease) y stable en GitHub Packages.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que materializa | CU-06, CU-07 |
| RN que cubre | RN-04 (last-write-wins), RN-02 (marcadores en un mismo radio reportados) |
| RC | RC-03 (idempotencia por ChangeId) |
| ADR que lo gobierna | ADR-07 (publicación/versionado), ADR-05 (cola), ADR-06 (conflictos) |
| Ejemplo de consumo | samples/01-sync-basico y samples/02-sync-maui-demo (11) |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Contrato de Abstractions inicial de la librería de sincronización: superficie pública, tipos, errores y política SemVer. Generado por AG-05 |
