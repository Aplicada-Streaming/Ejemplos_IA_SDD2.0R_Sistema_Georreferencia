# Sprint Review — Sprint 41

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-41_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-41_v1.0.md`:

> Arreglar el defecto bloqueante hallado en la revisión funcional (§2): un Jefe de Área no puede asignar agentes a un relevamiento contra la base real (`503 ACCION_NO_AUDITADA`), lo que impide que cualquier agente capture.

Veredicto: Cumplido.

Explicación corta: la causa raíz era que `AsignacionAgente` **pre-asignaba su clave primaria** (`AsignacionAgenteId = Guid.NewGuid()`) en el constructor. Como la PK es `ValueGeneratedOnAdd`, al agregar la asignación nueva por la colección del agregado a un relevamiento ya rastreado, EF aplicaba la heurística "clave seteada → entidad existente" y la persistía como **`UPDATE`** (no `INSERT`); el `UPDATE` de 0 filas lanzaba `DbUpdateConcurrencyException`, que el gate de auditoría tragaba devolviendo `ACCION_NO_AUDITADA`. **El fix quita el pre-seteo de la PK** (una línea): la entidad entra con clave vacía, EF la detecta `Added` y genera la Guid al insertar. Se agregó una prueba de integración contra **SQLite** (proveedor relacional) que reproduce el bug —falla sin el fix, pasa con él— cerrando el agujero del gate **InMemory** que lo ocultaba. Verificado además contra la base **SqlServer DEV** real.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| BT-FIX-ASIGNACION | Bug | Jefe de Área asigna un agente a un relevamiento: `204` (antes `503`) | El flujo de onboarding vuelve a funcionar |
| CU-04 | Funcionalidad | El agente asignado captura una observación: `201` (antes `403`) | El ciclo de campo quedó desbloqueado |
| BT-TEST-RELACIONAL | Calidad | Prueba SQLite que reproduce el bug y queda como regresión | El gate ya no es ciego a esta clase de bug |

## 3. Feedback recibido

- El bug era **bloqueante del producto**: sin asignación de agentes, ningún agente podía capturar; era el corazón del ciclo de campo y estaba roto contra la base real.
- La lección de fondo se repite: el gate **InMemory** no detecta defectos de persistencia que sí aparecen con un proveedor **relacional** (InMemory no aplica el chequeo de "se esperaba 1 fila afectada"). La prueba relacional cierra ese flanco para este caso.
- El fix fue mínimo y de causa raíz (no pre-asignar la PK), sin cambiar la API del dominio ni romper a los 13 llamadores de `AsignarAgente`.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos corregidos | 1 (crítico/bloqueante: asignación de agentes) |

Pruebas: **340** (300 unitarias + 40 de integración), +2 de integración (`AsignacionAgenteRelacionalTests`: alta + reactivación, sobre SQLite). La prueba reproduce el bug (falla sin el fix con `ACCION_NO_AUDITADA`) y pasa con él. Verificación adicional contra SqlServer DEV real (`204`/`201`). El gate de cobertura se mantiene.

> Nota de cuenta de pruebas: este sprint parte de `main` (que tiene hasta S39 = 338). El Sprint 40 (PR #50, app móvil, +6 unitarias) está pendiente de merge; sumados quedan 346. La cuenta de 340 corresponde a la base de S41 (338 + 2 de integración).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-FIX-ASIGNACION | Bug | Aceptado (fix de causa raíz; asignar `204` y capturar `201` contra base real; suite verde) |
| BT-TEST-RELACIONAL | Tarea | Aceptada (`AsignacionAgenteRelacionalTests` sobre SQLite; reproduce y cubre el bug como regresión) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 41 se traslada. |

Backlog restante (de la revisión funcional, `revision-funcional-app_v1.0.md` §5/§6): captura **offline real** (P0, S42), selección de relevamiento (S43), método de seguridad/reingreso + auto-sync (S44), y P2/P3.

## 7. Decisiones tomadas durante el review

- Adoptar el patrón de **prueba contra proveedor relacional (SQLite)** para defectos de persistencia, complementando el gate InMemory, sin imponer SqlServer en cada máquina.
- Fijar la causa raíz en el dominio (no pre-asignar PKs `ValueGeneratedOnAdd`) como guía para futuras entidades de agregados.
- Mergear este fix de forma **independiente** (P0) sin esperar al PR del Sprint 40; recomendación de orden de merge documentada en el PR.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 41 (fix del bug crítico de asignación de agentes). Veredicto Cumplido, velocity 8, 0 carry-over, 340 pruebas (+2 de integración relacional); verificado contra SqlServer DEV real. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
