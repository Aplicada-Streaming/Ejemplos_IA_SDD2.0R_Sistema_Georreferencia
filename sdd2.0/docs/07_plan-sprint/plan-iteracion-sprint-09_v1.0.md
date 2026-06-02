# Plan de Iteración — Sprint 09

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-09_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-09-29
**Fecha fin:** 2026-10-10
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 13,3 SP. Se compromete US-18 (13 SP), la historia más grande del backlog y el corazón de la continuidad operativa (NB-03). Es un compromiso de una sola historia por su tamaño y su efecto estructural sobre la consolidación de datos; el margen respecto del rango alto (24–28 SP de los sprints de módulo completo) se reserva por la complejidad de la consolidación last-write-wins y el marcado de conflictos.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 120 | 0,72 | 86 h |
| Dev frontend | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Permitir que la app de campo, al recuperar conexión, suba sus cambios locales encolados y el backend los consolide contra el estado central aplicando la última escritura por marca temporal (RN-04), marcando como conflicto todo recurso resuelto por esa vía y los marcadores que queden dentro de un mismo radio (RN-02), de forma idempotente por identificador de cambio, y luego baje las actualizaciones recientes del relevamiento; todo verificado por área y auditado.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-18 | Historia | Sincronizar subiendo y bajando cambios (backend de consolidación, CU-07) | Alta (Must) | 13 | Dev backend A + B | Pendiente |

Total de puntos comprometidos: 13 SP. US-18 (Must, `Ready`) materializa CU-07 y cierra la regla RN-04 (última escritura efectiva), que se venía difiriendo desde la resolución de conflictos (Sprint 05). Se descompone internamente en subir/consolidar y bajar, conforme a su nota de refinamiento.

## 4. Alcance técnico

El alcance de este sprint es el **backend de consolidación** de la sincronización (CU-07, lado servidor). El cliente móvil de captura offline (CU-06, US-16), el encolado local (US-17), la detección de conectividad (US-19) y la librería publicable `GeoVial.Sync` con `IChangeQueue`/`ISyncEngine` (BT-15, contratos-abstractions-sync) requieren la plataforma móvil (MAUI/SQLite) y la publicación del paquete, fuera de esta solución backend; quedan para sprints posteriores. El endpoint de sincronización implementa el contrato `POST /api/v1/relevamientos/{id}/sync` (contratos-rest §3, §4.1) que ese cliente consumirá.

Componentes que se construyen o modifican:

1. Recepción idempotente del lote de cambios (RC-03): cada cambio trae un identificador único (`CambioId`); el backend registra los cambios ya aplicados y descarta los reintentos sin volver a aplicarlos, permitiendo reanudar una sincronización interrumpida sin duplicar (CU-07 §5.A).
2. Consolidación last-write-wins (RN-04): para una edición concurrente del mismo recurso prevalece la marca temporal más reciente; el recurso editado de forma concurrente queda marcado como conflicto de edición (`ConflictoSync` tipo edición), resoluble desde la web (CU-12). Se aplica a los comentarios como recurso editable de texto (el ejemplo de CA-02), con la mecánica reutilizable (idempotencia, marca temporal, marca de conflicto) para extender a otras entidades.
3. Detección de marcadores en un mismo radio (RN-02): tras consolidar, se reutiliza la detección del Sprint 05 para señalar marcadores cercanos como conflicto, sin unificarlos.
4. Bajada de actualizaciones: el backend devuelve los recursos del relevamiento modificados desde la última marca temporal informada por el cliente.
5. Respuesta de sincronización (contratos-rest §4): confirmados (CambioId aplicados), conflictos (id, tipo, recursos) y actualizaciones; autorizada por área (RN-01) y auditada (RN-07).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- Un lote de cambios se aplica una sola vez: reenviar el mismo `CambioId` no duplica el efecto (idempotencia, CU-07 CA-03).
- Ante dos ediciones del mismo recurso, prevalece la de marca temporal más reciente y el recurso queda marcado como conflicto (RN-04, CU-07 CA-02).
- La sincronización sube los cambios, baja las actualizaciones del relevamiento y confirma el resultado sin pérdida (CU-07 CA-01).
- La sincronización respeta el acotamiento por área (RN-01) y queda auditada (RN-07).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La consolidación last-write-wins puede dejar recursos sin marca de conflicto (CONFLICTO_NO_MARCADO) | Media | Alto | Marcar el conflicto en la misma operación que aplica la última escritura; pruebas que verifican que todo recurso consolidado por LWW queda marcado |
| Un reintento podría aplicar dos veces un cambio | Media | Alto | Registro persistente de `CambioId` aplicados; el handler descarta los ya aplicados antes de consolidar |
| US-18 excede el sprint por su tamaño (13 SP) | Media | Medio | Descomposición interna subir/consolidar primero y bajar después; el cliente móvil y la librería quedan explícitamente fuera de alcance |

## 7. Criterios de hecho del sprint

El Sprint 09 se considera completo cuando el backend de consolidación de US-18 está terminado según la DoD con sus pruebas verdes: idempotencia, last-write-wins con marca de conflicto, detección de marcadores en radio y bajada de actualizaciones quedan demostradas en el sprint review; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que avanzan | US-18 (sincronizar subir/bajar, backend de consolidación) |
| CU que avanzan | CU-07 (sincronización) |
| NB que avanzan | NB-03 (continuidad operativa sin conexión), NB-05 (consistencia ante conflictos) |
| RN que cierran/avanzan | RN-04 (última escritura efectiva), RN-02 (marcadores en radio), RN-01, RN-07 |
| ADRs que gobiernan | ADR-06 (last-write-wins con override manual), ADR-05 (sincronización), ADR-07 (librería de sync — superficie diferida), ADR-01 (CQRS), ADR-09 (persistencia) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 09 (backend de consolidación de la sincronización, US-18). Compromete 13 SP. Cierra RN-04 (última escritura efectiva) e implementa el contrato `/sync`; el cliente móvil, la librería `GeoVial.Sync` (BT-15) y US-16/17/19 quedan fuera por requerir la plataforma móvil. Generado por AG-07 |
