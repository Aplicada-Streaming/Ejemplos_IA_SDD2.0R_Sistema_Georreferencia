# Plan de Iteración — Sprint 47

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-47_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-03-20
**Fecha fin:** 2028-03-31
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S44–S46); capacidad sugerida estricta 9 SP. Se compromete el **endpoint "relevamientos asignados a mí"** (8 SP), ítem del backlog de la revisión funcional. Núcleo (query + handler) en el gate; el móvil lo consume.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Hoy `GET /api/v1/relevamientos` devuelve **todos los relevamientos del área** del solicitante y el móvil filtra la asignación **del lado del cliente** (S43, `SelectorRelevamientos`). Para un agente de campo eso implica que su dispositivo recibe relevamientos de **otros** agentes de su área (fuga de datos + payload innecesario). El objetivo es un endpoint **"asignados a mí"** que filtre del lado del servidor por la asignación vigente del agente, y que el móvil lo consuma para traerse sólo su trabajo.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-CAMPO-MIS | Historia | Endpoint `GET /relevamientos/mios`: relevamientos con asignación vigente del agente autenticado | Alta | 5 | Dev backend (AG-06) | Cerrada |
| BT-MIS-CLIENTE | Tarea | El móvil (`ServicioSesion`) consume `/mios` en vez de traer todo el área y filtrar localmente | Media | 1 | Dev móvil (AG-08) | Cerrada |
| BT-MIS-TESTS | Tarea | Cubrir en el gate el filtrado por asignación (unit del handler + E2E por HTTP) | Alta | 2 | QA (AG-05) | Cerrada |

Total de puntos comprometidos: 8 SP. Núcleo en `GeoVial.Application` (en el gate); endpoint en `GeoVial.Api`; consumo en `GeoVial.Sync` (`ServicioSesion`).

## 4. Alcance técnico

1. **Aplicación (en el gate):** nuevo `ListarRelevamientosAsignadosQuery(Guid AgenteId)` + `ListarRelevamientosAsignadosHandler` que devuelve los relevamientos con una `AsignacionAgente` **vigente** del agente, respetando la vigencia del usuario (`Autorizacion.PuedeAccederArea` como defensa: un usuario dado de baja no recibe nada). Nuevo puerto `IRelevamientoRepository.ListarPorAgenteAsignadoAsync`. Registro del handler en `DependencyInjection`.
2. **Persistencia:** `ListarPorAgenteAsignadoAsync` filtra **en la base** (`Where(r => r.Asignaciones.Any(a => a.AgenteUsuarioId == id && a.Vigente))`), en vez de traer todo y filtrar en memoria como el listado de área.
3. **API:** `GET /api/v1/relevamientos/mios` → `ListarRelevamientosAsignadosQuery(callerId)` → `RelevamientoDto`. La ruta literal `/mios` no colisiona con `/{relevamientoId:guid}` (no es un GUID).
4. **Móvil (`GeoVial.Sync`):** `ServicioSesion.ListarRelevamientosAsync` consume `/mios`. `SelectorRelevamientos` se mantiene (marca/ordena/resuelve el activo); con `/mios` todos vienen asignados, pero la resolución del activo (primer asignado abierto) sigue aplicando.

## 5. Definition of Done aplicada

- `GET /relevamientos/mios` devuelve sólo los relevamientos con asignación vigente del agente; excluye los del área a los que no está asignado y los de otras áreas; 401 sin token.
- Núcleo (query + handler + filtro) cubierto por pruebas en el gate; el listado de área existente (`ListarRelevamientosQuery`) sigue intacto y verde.
- Cobertura DoD respetada (Domain/Application líneas ≥80 %, ramas ≥70 %).
- El MAUI compila para `net10.0-android`; el móvil consume `/mios`.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La ruta `/mios` colisiona con `/{relevamientoId:guid}` | Baja | Medio | El constraint `:guid` no matchea `mios`; la ruta literal tiene prioridad. Cubierto por el E2E |
| Cambiar el endpoint del móvil deja al agente sin lista si `/mios` falla | Baja | Medio | El cliente ya tolera respuesta vacía (lista vacía); el endpoint es aditivo, no se elimina `GET /relevamientos` |
| El filtro EF por colección anidada no traduce en algún proveedor | Baja | Bajo | `Any()` sobre navegación es estándar en EF Core; se valida con el E2E (InMemory) y la suite |

## 7. Criterios de hecho del sprint

El Sprint 47 se considera completo cuando: el endpoint `/mios` filtra por asignación vigente del agente, el móvil lo consume, el núcleo está cubierto en el gate sin romper el listado de área, la cobertura DoD se mantiene, el MAUI compila, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | `revision-funcional-app_v1.0.md` (backlog: endpoint "relevamientos asignados a mí") |
| CU | CU-01 (relevamientos), continúa F-M-04/05 (selección de relevamiento asignado, S43) |
| RN | RN-01 (acceso por área/asignación) |
| Componentes | `GeoVial.Application` (`Relevamientos`); `GeoVial.Api`; `GeoVial.Infrastructure`; `GeoVial.Sync` (`ServicioSesion`) |
| Calidad | definition-of-done §1.4 (cobertura), minimización de datos |
| Tests previstos | unit del handler (sólo asignados; excluye no-asignado del área; excluye baja) + E2E `/mios` (asignado sí, no-asignado no, 401 sin token) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 47 (endpoint "relevamientos asignados a mí"): `ListarRelevamientosAsignadosQuery`/handler + filtro EF por asignación vigente + `GET /relevamientos/mios` + consumo en el móvil. Compromete 8 SP. Generado por AG-07 |
