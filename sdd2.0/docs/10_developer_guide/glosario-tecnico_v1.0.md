# Glosario técnico — Librería de sincronización GeoVial

**Proyecto:** GeoVial
**Documento:** glosario-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Technical Writer / Developer Advocate Senior (AG-10), Equipo SDD 2.0
**Tipo Diátaxis:** Reference
**Audiencia:** Developer integrador que reutiliza la librería de sincronización en otros proyectos
**Nivel:** Básico
**Tiempo estimado de lectura:** 6 min

---

Vocabulario canónico del consumidor de la librería de sincronización. Es la fuente única: el resto de los documentos de 10 enlaza acá en lugar de redefinir. Los términos del dominio de negocio (relevamiento, marcador, observación, etiqueta, agente de campo) están definidos en el glosario del cliente (PROJECT-BRIEF §12) y en el modelo conceptual de 02; este glosario los referencia sin duplicar su semántica.

| Término | Definición operativa | Referencia cross-doc |
| --- | --- | --- |
| `abstractions` | Capa de superficie pública de la librería: las interfaces y tipos contra los que programa el consumidor; estable y versionada con SemVer. | [contratos-abstractions-sync_v1.0.md](../05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md) (05) |
| `change-record` | Unidad de cambio encolada: identificador, tipo de operación, entidad, referencia, marca temporal y payload. | [referencia-api_v1.0.md](referencia-api_v1.0.md) §1 (`ChangeRecord`) |
| `change-id` | Identificador único de un `change-record`; clave de idempotencia que evita aplicar el mismo cambio dos veces. | [RC-03-unicidad-identificador-cola-sync_v1.0.md](../02_especificacion_funcional/modelo-datos/reglas-conceptuales-de-modelo/RC-03-unicidad-identificador-cola-sync_v1.0.md) (02) |
| `change-queue` | Cola local durable y ordenada de `change-record` pendientes de sincronizar. | [conceptos-fundamentales_v1.0.md](conceptos-fundamentales_v1.0.md) §2; [referencia-api_v1.0.md](referencia-api_v1.0.md) §2 (`IChangeQueue`) |
| `sync-engine` | Orquestador del pipeline subir-consolidar-bajar; idempotente por `change-id`. | [referencia-api_v1.0.md](referencia-api_v1.0.md) §2 (`ISyncEngine`) |
| `backend-client` | Puerto hacia el backend que el consumidor implementa para subir y bajar cambios. | [referencia-api_v1.0.md](referencia-api_v1.0.md) §2 (`ISyncBackendClient`) |
| `connectivity-monitor` | Componente que notifica la recuperación de conexión para disparar la sincronización automática. | [referencia-api_v1.0.md](referencia-api_v1.0.md) §2 (`IConnectivityMonitor`) |
| `last-write-wins` | Criterio de consolidación: ante un choque de campo prevalece el cambio de marca temporal más reciente; el recurso queda marcado como conflicto. | [ADR-06-conflictos-last-write-wins-override-manual_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-06-conflictos-last-write-wins-override-manual_v1.0.md) (05) |
| `conflict-info` | Conflicto detectado y reportado para resolución posterior: de campo (`FieldConflict`) o de marcadores en un mismo radio (`MarkersWithinRadius`). | [referencia-api_v1.0.md](referencia-api_v1.0.md) §1 (`ConflictInfo`) |
| `idempotencia` | Propiedad por la cual reintentar la subida de un `change-record` ya aplicado no lo aplica de nuevo. | [RC-03-unicidad-identificador-cola-sync_v1.0.md](../02_especificacion_funcional/modelo-datos/reglas-conceptuales-de-modelo/RC-03-unicidad-identificador-cola-sync_v1.0.md) (02) |
| `sync-result` | Resultado de un ciclo de sincronización: confirmados, conflictos y actualizaciones bajadas. | [referencia-api_v1.0.md](referencia-api_v1.0.md) §1 (`SyncResult`) |
| `drenar-cola` | Operación de vaciar de la cola los `change-record` confirmados tras una subida exitosa. | [referencia-api_v1.0.md](referencia-api_v1.0.md) §2 (`IChangeQueue.DrainConfirmedAsync`) |
| `marcadores-en-radio` | Marcadores que quedan dentro de un mismo radio; la librería los reporta como conflicto y nunca los fusiona automáticamente. | [RN-02 / RC-01](../02_especificacion_funcional/reglas-de-negocio/RN-02-identidad-marcador-radio-agrupacion_v1.0.md) (02); [conceptos-fundamentales_v1.0.md](conceptos-fundamentales_v1.0.md) §5 |
| `bandeja-sin-georreferenciar` | Destino de un dato sin coordenada; el `change-record` se encola y sincroniza igual, pero queda pendiente de georreferenciar. | [CU-06](../02_especificacion_funcional/casos-de-uso/CU-06-recolectar-observaciones-sin-conexion_v1.0.md) flujo 5.A (02) |

---

## Referencias cruzadas

- [contratos-abstractions-sync_v1.0.md](../05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md) — fuente de los términos de la superficie pública (05).
- [modelo-conceptual_v1.0.md](../02_especificacion_funcional/modelo-datos/modelo-conceptual_v1.0.md) — vocabulario de dominio que este glosario no duplica (02).
- [referencia-api_v1.0.md](referencia-api_v1.0.md) y [conceptos-fundamentales_v1.0.md](conceptos-fundamentales_v1.0.md) (10).

## Control de cambios

| Versión | Fecha | Cambio | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Vocabulario canónico del consumidor con definición operativa y referencia cross-doc; sin duplicar el glosario de dominio de 02. | AG-10 |
