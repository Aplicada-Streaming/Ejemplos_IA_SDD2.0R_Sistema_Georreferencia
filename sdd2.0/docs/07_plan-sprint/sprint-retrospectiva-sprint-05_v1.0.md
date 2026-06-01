# Sprint Retrospectiva — Sprint 05

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-05_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Aplicar la regla acordada en la retro anterior —escribir los tests de borde junto a cada handler— mantuvo la cobertura por encima del gate desde la primera medición, sin segunda tanda correctiva.
- Modelar la fusión como una reasignación en una sola transacción (observaciones, fotos y comentarios del absorbido) evitó marcadores y contenido huérfanos; las pruebas que cuentan el contenido tras unificar lo confirman.
- La idempotencia de la detección (orden canónico del par y verificación de pendiente previo) evitó duplicar conflictos al re-detectar, tal como anticipaba el riesgo del plan.
- Mantener la decisión siempre humana (el sistema nunca unifica ni descarta solo, RN-02) simplificó el razonamiento sobre concurrencia: la primera resolución gana, la segunda recibe `CONFLICTO_INEXISTENTE`.

## 2. Qué no salió bien

- Para reasignar contenido hubo que ampliar varios puertos con variantes "para edición" (con seguimiento de cambios) frente a las de solo lectura existentes; el repositorio quedó con dos métodos por entidad y conviene vigilar que no se confundan.
- La marca de conflicto se levanta sobre los marcadores del par al resolver, aun cuando un marcador podría participar de otro conflicto pendiente; en este sprint no se da el caso, pero es una arista a cubrir cuando exista la sincronización real (EP-04).

## 3. Qué probar

- Documentar en el README de persistencia la convención de los métodos "para edición" (tracked) frente a los de solo lectura (`AsNoTracking`) para evitar usos equivocados.
- Al incorporar la sincronización (EP-04), revisar la regla de levantado de marca para que solo se baje cuando el marcador no tenga otros conflictos pendientes.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Documentar la convención tracked vs `AsNoTracking` en el README de persistencia | AG-08 | 2026-08-22 | Pendiente |
| Configurar la cobertura del pipeline con limpieza previa (arrastre del Sprint 04) | AG-09 | 2026-08-22 | Pendiente |
| Refinar EP-07 (exportación/importación) y EP-04 (sincronización, last-write-wins efectivo) para los próximos sprints | AG-06 | 2026-08-22 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 04 | Estado actual |
| --- | --- |
| Configurar la cobertura del pipeline con limpieza previa | Pendiente (se reitera) |
| Adoptar la regla de escribir los tests de borde junto al handler | Completada (aplicada en este sprint con buen resultado) |
| Refinar EP-06 (resolución de conflictos) y EP-07 (exportación/importación) | EP-06 completada (US-25/US-26 entregadas); EP-07 sigue en refinamiento |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 05 con 3 acciones nuevas y seguimiento de las del Sprint 04 (regla de tests de borde completada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
