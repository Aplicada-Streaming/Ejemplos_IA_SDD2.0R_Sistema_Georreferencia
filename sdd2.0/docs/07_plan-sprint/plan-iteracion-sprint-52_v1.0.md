# Plan de Iteración — Sprint 52

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-52_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-05-29
**Fecha fin:** 2028-06-09
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S49–S51); capacidad sugerida estricta 9 SP. Se compromete un **sprint de limpieza acotado** (5 SP): unificar el listado de relevamientos por área para que filtre **en la base** en vez de traer todo y filtrar en memoria. Es una acción de retro arrastrada desde S47 (reiterada en S47-S51). Se compromete **menos de la capacidad sugerida a propósito**: es deuda técnica de bajo riesgo, no una historia de producto; estimarla en 8 sería inflarla.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

`ListarRelevamientosHandler` traía **todas** las filas (`ListarTodosAsync`) y filtraba en memoria con `Autorizacion.PuedeAccederArea`. Para un jefe de área o agente eso es traer toda la tabla para descartar casi todo: no escala. El objetivo es **filtrar en la base** (área propia para esos roles; todo para raíz/jefe general), centralizando la decisión de rol en `Autorizacion` y **preservando exactamente** el comportamiento observable. Cierra la acción de retro arrastrada desde S47.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-LISTADO-AREA | Tarea | `ListarRelevamientosHandler` filtra en la base: `IRelevamientoRepository.ListarPorAreaAsync` + decisión de rol en `Autorizacion.AccedeATodasLasAreas` | Media | 3 | Dev backend (AG-06) | Cerrada |
| BT-LISTADO-TESTS | Tarea | Cubrir en el gate los caminos (raíz ve todo, área filtra, no vigente vacío) sin cambiar el comportamiento previo | Media | 2 | QA (AG-05) | Cerrada |

Total de puntos comprometidos: 5 SP (sprint de limpieza; por debajo de la capacidad sugerida, ver §1).

## 4. Alcance técnico

1. **Dominio:** `Autorizacion.AccedeATodasLasAreas(usuario)` → raíz/jefe general vigentes; centraliza la decisión de rol para no duplicarla en el handler.
2. **Aplicación:** `ListarRelevamientosHandler` — si el usuario no existe o no está vigente, vacío; si accede a todas las áreas, `ListarTodosAsync`; si no, `ListarPorAreaAsync(usuario.AreaId)` (o vacío si no tiene área). Mismo resultado que el filtro previo por `PuedeAccederArea`, sin el barrido en memoria.
3. **Persistencia:** `IRelevamientoRepository.ListarPorAreaAsync(areaId)` filtra en la base (`Where(r => r.AreaId == areaId)`), análogo a `ListarPorAgenteAsignadoAsync` (S47).
4. **Sin cambios de API, contrato ni móvil:** el endpoint `GET /relevamientos` y todos los consumidores quedan igual; sólo cambia cómo el handler obtiene el conjunto.

## 5. Definition of Done aplicada

- El listado por área se obtiene filtrando en la base; el comportamiento observable no cambia (mismos resultados por rol/vigencia).
- Cubierto en el gate (raíz ve todas, área filtra, no vigente vacío, `AccedeATodasLasAreas` por rol); los tests previos siguen verdes.
- Cobertura DoD respetada (Domain/Application líneas ≥80 %, ramas ≥70 %).
- El MAUI sigue compilando (build de control; el sprint no toca el móvil).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El refactor cambia sutilmente el comportamiento (p. ej. no vigente) | Media | Medio | Se preserva el guard de vigencia; tests de raíz/área/no-vigente cubren los caminos; el test de área previo sigue verde |
| Duplicar la lógica de rol fuera de `Autorizacion` | Media | Bajo | Se agrega `AccedeATodasLasAreas` en `Autorizacion`; el handler no decide por rol directamente |
| Sprint por debajo de la capacidad afecta la métrica de velocity | Alta | Bajo | Se documenta como sprint de limpieza acotado (outlier con motivo) en `velocidad-equipo` |

## 7. Criterios de hecho del sprint

El Sprint 52 se considera completo cuando: el listado por área filtra en la base preservando el comportamiento, está cubierto en el gate sin romper lo previo, la cobertura DoD se mantiene, el MAUI sigue compilando, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Acción de retro arrastrada desde S47 (unificar el listado de área al filtro en base) |
| CU | CU-01 (relevamientos); RN-01 (acceso por área) |
| Componentes | `GeoVial.Domain` (`Autorizacion`); `GeoVial.Application` (`Relevamientos`); `GeoVial.Infrastructure` |
| Calidad | definition-of-done §1.4 (cobertura); higiene de acceso a datos |
| Tests previstos | handler (raíz todo / área filtra / no vigente vacío) + `Autorizacion.AccedeATodasLasAreas` por rol y vigencia |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 52 (limpieza: listado de área en base): `ListarPorAreaAsync` + `Autorizacion.AccedeATodasLasAreas`; handler filtra en base preservando el comportamiento. Sprint acotado de 5 SP (deuda de retro de S47). Generado por AG-07 |
