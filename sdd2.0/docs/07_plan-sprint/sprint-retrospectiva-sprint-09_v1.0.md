# Sprint Retrospectiva — Sprint 09

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-09_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Descomponer US-18 (la historia más grande del backlog) en el alcance backend de consolidación permitió comprometerla y completarla en un sprint, en lugar de arrastrarla por su tamaño.
- Reutilizar la detección de marcadores en radio del Sprint 05 (vía el mediador) evitó duplicar lógica de conflictos en el módulo de sincronización.
- Modelar la idempotencia con un registro persistente de `CambioId` aplicados dio una reanudación sin duplicados simple y verificable; el conjunto de pruebas de idempotencia, last-write-wins y orden por marca temporal cubrió los tres criterios de aceptación de CU-07.
- Reaprovechar el tipo de conflicto `EdicionEnConflicto`, declarado en el Sprint 05 pero sin uso hasta ahora, conectó la consolidación (RN-04) con la resolución manual desde la web (CU-12) sin cambios de contrato.

## 2. Qué no salió bien

- EP-04 quedó dividida por plataforma: el backend de consolidación se entregó, pero el cliente móvil (captura offline, cola, conectividad) y la librería publicable `GeoVial.Sync` no pueden construirse sin el proyecto MAUI, que aún no existe en la solución. La épica no se cierra en un solo sprint y conviene reflejarlo en el roadmap.
- La consolidación last-write-wins se acotó a comentarios (el recurso del ejemplo de CA-02); las otras entidades editables (observaciones, etiquetas) quedan para una extensión posterior. Es una decisión consciente, pero deja la regla RN-04 parcialmente cubierta a nivel de entidades.

## 3. Qué probar

- Incorporar un proyecto móvil mínimo (MAUI) en un spike para validar el contrato `GeoVial.Sync` (`IChangeQueue`/`ISyncEngine`) contra el endpoint `/sync` ya construido, antes de comprometer US-16/17/19.
- Generalizar la consolidación a un mapa de entidades editables con su marca de última edición, para extender RN-04 más allá de comentarios sin reescribir el handler.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Spike de proyecto móvil MAUI para validar `GeoVial.Sync` contra `/sync` | AG-05 / AG-08 | 2026-10-24 | Pendiente |
| Diseñar la generalización de last-write-wins a otras entidades editables | AG-02 / AG-05 | 2026-10-24 | Pendiente |
| Refinar US-30 (consulta de auditoría con retención) para el próximo sprint backend | AG-06 | 2026-10-24 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 08 | Estado actual |
| --- | --- |
| Agregar `dotnet list package --vulnerable` al pipeline de PR | Pendiente |
| Validar licencia + advisories de toda dependencia nueva en un spike previo | Aplicada en la práctica este sprint (sin dependencias nuevas que auditar) |
| Refinar BT-19 y EP-04 | BT-19 completada (Sprint 08); EP-04 backend (US-18) entregado este sprint, resto diferido por plataforma móvil |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 09 con 3 acciones nuevas y seguimiento de las del Sprint 08. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
