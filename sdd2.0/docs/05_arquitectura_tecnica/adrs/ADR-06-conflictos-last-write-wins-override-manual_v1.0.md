# ADR-06 — Resolución de conflictos last-write-wins con override manual desde la web

**Proyecto:** GeoVial
**Documento:** ADR-06-conflictos-last-write-wins-override-manual_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Persistencia

## 1. Contexto

Varias cuadrillas trabajan en paralelo y sin conexión sobre el mismo relevamiento, generando ediciones incompatibles del mismo recurso y marcadores casi superpuestos (R-01, R-02 de negocio). Al sincronizar, los choques de campo necesitan un criterio determinista de consolidación, y los marcadores dentro de un mismo radio no deben unificarse ni descartarse de forma automática (RN-02, RN-04). La decisión final debe quedar en manos humanas (CU-11, CU-12). Motivan: CU-07, CU-11, CU-12; RN-02, RN-04.

## 2. Decisión

Se adopta last-write-wins a nivel de campo como criterio de consolidación: ante un choque, prevalece el cambio con la marca temporal más reciente. Todo recurso consolidado por esta vía queda marcado como conflicto resoluble manualmente desde la web hasta que un usuario autorizado lo dé por resuelto. Los marcadores dentro de un mismo radio se señalan como conflicto y nunca se fusionan automáticamente; el jefe de área decide unificarlos o mantenerlos separados (override manual).

## 3. Estado

Aceptado el 2026-06-01. Renumeración de `ADR-006` (PROJECT-README §15, estado original Propuesto) a `ADR-06`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Last-write-wins + marca de conflicto + override manual (elegido) | Determinista, simple, no pierde de vista cambios descartados; decisión humana donde importa | Puede descartar cambios concurrentes válidos hasta la revisión manual |
| Merge automático por campo (CRDT u operacional) | Sin descartes; convergencia automática | Complejidad alta; no resuelve la identidad de marcadores en un radio, que es semántica y requiere criterio humano |
| Bloqueo pesimista / primera escritura gana | Evita choques | Inviable offline: no hay coordinación de bloqueos sin conexión |

## 5. Consecuencias positivas

1. Criterio determinista de consolidación que no requiere coordinación online (apto para offline-first).
2. La marca de conflicto preserva la revisión humana: ningún cambio se descarta de forma silenciosa.
3. La identidad de marcadores en un radio queda a decisión del jefe de área (RN-02), respetando la semántica del dominio.

## 6. Consecuencias negativas y trade-offs

1. Last-write-wins puede descartar un cambio concurrente válido; trade-off explícito aceptado, compensado por la marca de conflicto y la resolución manual (PROJECT-README §16).
2. Acumular conflictos sin resolver degrada la consistencia percibida; mitigado por listar pendientes (CU-11).

## 7. Implementación

El módulo de sincronización (CU-07) aplica last-write-wins por campo y marca el recurso (`CONFLICTO_NO_MARCADO` si no se marca). El módulo de detección (CU-11) lista marcadores en un mismo radio (`MARCADORES_EN_RADIO`) y ediciones en conflicto. El módulo de resolución (CU-12) aplica la decisión humana, levanta la marca y audita (RN-07). Ningún proceso unifica o descarta sin decisión (`UNIFICACION_NO_AUTORIZADA`). El pipeline se detalla en `flujo-ejecucion_v1.0.md`.

## 8. Métricas de validación

- Dos ediciones del mismo campo: prevalece la de marca temporal más reciente (CA-02 de CU-07).
- El recurso consolidado queda marcado como conflicto y la marca persiste hasta la resolución (suite de consolidación, 08).
- Ningún proceso automático fusiona marcadores en un radio (CA-01/CA-02 de CU-12).

## 9. Referencias

- PROJECT-README §6 (resolución de conflictos), §16 (trade-off).
- RN-02, RN-04; CU-07, CU-11, CU-12.
- ADR-05 (offline), ADR-04 (mapas/radio).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Decisión aceptada. Renumeración de ADR-006 a ADR-06 |
