# Sprint Retrospectiva — Sprint 47

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-47_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Slice limpio y enteramente verificable en CI: query + handler + endpoint + consumo móvil, con unit + E2E en el gate. Sin necesidad de dispositivo para validar el comportamiento central.
- Cambio **aditivo**: se agregó `/mios` sin tocar `GET /relevamientos` (listado de área para jefes/web), así que nada existente se rompió y el `SelectorRelevamientos` (S43) siguió intacto.
- Mejora de **minimización de datos** concreta: el dispositivo del agente ya no recibe relevamientos de otros agentes de su área (antes el listado de área los traía todos y el filtro era sólo cosmético, del lado del cliente).
- El filtro se hace **en la base** (`Any` sobre la asignación), consistente con cómo debería escalar; el listado de área en memoria queda como está (no era el foco).

## 2. Qué no salió bien

- Queda una **inconsistencia de estilo**: `ListarRelevamientosHandler` (área) trae todo y filtra en memoria, mientras el nuevo `ListarRelevamientosAsignadosHandler` filtra en la base. No es un bug, pero conviene unificar el listado de área hacia el filtro en base en un sprint de limpieza.
- No se verificó **on-device** que el móvil ahora liste sólo los asignados; se confió en el E2E. Es bajo riesgo (el cliente sólo cambió la URL), pero la verificación visual quedó pendiente.
- El `SelectorRelevamientos` ahora recibe una lista ya filtrada a "asignados", por lo que su marcado "asignado/no asignado" es trivialmente todo-verdadero en el móvil; su valor real pasó a ser sólo el orden y la resolución del activo. No molesta, pero hay lógica que dejó de aportar en el camino móvil (sí sigue siendo útil en tests y si algún día se listara mezclado).

## 3. Qué probar

- Verificar on-device que la lista del Picker muestra sólo los relevamientos asignados al agente logueado.
- Considerar un sprint de limpieza que unifique `ListarRelevamientosHandler` (área) al filtro en base, y revise si `SelectorRelevamientos` puede simplificarse dado que el móvil ya recibe sólo asignados.
- Confirmar el comportamiento para un **jefe de área** que use la app (si aplicara): `/mios` le devolvería sólo lo que él tenga asignado, que puede ser vacío; decidir si el rol jefe usa otro endpoint.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Verificar on-device que el móvil lista sólo los asignados | AG-05 (QA) | 2028-04-14 | Pendiente |
| Sprint de limpieza: unificar el listado de área al filtro en base | AG-06 (backend) | 2028-04-14 | Pendiente |
| Indicador de estado de sincronización en la UI | AG-08 (móvil) | 2028-04-14 | Pendiente (se reitera de S45/S46) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 46 | Estado actual |
| --- | --- |
| Migrar la integración de captura/sync a SQLite relacional para ejercitar índices únicos en CI | Pendiente (se reitera) |
| Probar el ciclo offline completo en modo avión on-device (sin duplicar) | Pendiente (se reitera de S42/S45) |
| Indicador de estado de sincronización en la UI | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 47 (endpoint "asignados a mí"): slice limpio verificable en CI, aditivo, con minimización de datos. Se anota la inconsistencia de estilo entre el listado de área (filtro en memoria) y el nuevo (filtro en base) y se propone un sprint de limpieza. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
