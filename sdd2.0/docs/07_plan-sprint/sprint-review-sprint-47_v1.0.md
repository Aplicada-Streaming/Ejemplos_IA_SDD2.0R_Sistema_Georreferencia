# Sprint Review — Sprint 47

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-47_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-47_v1.0.md`:

> El objetivo es un endpoint "asignados a mí" que filtre del lado del servidor por la asignación vigente del agente, y que el móvil lo consuma para traerse sólo su trabajo.

Veredicto: Cumplido.

Explicación corta: hasta S46, `GET /api/v1/relevamientos` devolvía **todos los relevamientos del área** del solicitante y el móvil marcaba/ordenaba la asignación del lado del cliente (S43). Para un agente de campo eso significaba recibir en su dispositivo relevamientos de otros agentes de su área. Se agregó `GET /api/v1/relevamientos/mios`, que mediante un nuevo `ListarRelevamientosAsignadosQuery`/handler filtra **en la base** por la asignación vigente del agente, y el móvil (`ServicioSesion`) pasó a consumirlo. El `SelectorRelevamientos` (S43) se conserva intacto: sigue marcando/ordenando y resolviendo el relevamiento activo.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-CAMPO-MIS | Funcionalidad | `GET /mios` (por HTTP): un agente recibe sólo su relevamiento asignado, no otro de la misma área sin asignar | El agente ve sólo su trabajo |
| Seguridad | Minimización de datos | El dispositivo ya no recibe relevamientos ajenos del área | Menos exposición y menos payload |
| BT-MIS-CLIENTE | Integración | El móvil consume `/mios`; la selección y el relevamiento activo (S43) siguen funcionando | Coherente con la selección previa |

## 3. Feedback recibido

- Cierra un ítem del backlog de la revisión funcional con un cambio limpio y enteramente verificable en CI (backend + núcleo).
- El filtro se hace **en la base** (`Any` sobre la asignación), no trayendo todo y filtrando en memoria como el listado de área; mejor en payload y en exposición de datos.
- Cambio **aditivo**: el listado de área (`GET /relevamientos`, usado por jefes/web) queda intacto; sólo el móvil cambia de endpoint.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **386** (343 unitarias + 43 de integración), +4 respecto de S46: 2 unitarias (`RelevamientoAplicacionTests`: el handler devuelve sólo los asignados; excluye al agente dado de baja) y 2 de integración (`RelevamientosMiosE2ETests`: `/mios` trae el asignado y no el ajeno de la misma área; 401 sin token). El listado de área existente sigue verde. Cobertura del gate: Domain 88,5 % / 79,8 %; Application 87,3 % / 76,2 % (umbral 80 % / 70 %). El MAUI compila para `net10.0-android`.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-CAMPO-MIS | Historia | Aceptada (`/mios` filtra por asignación vigente del agente) |
| BT-MIS-CLIENTE | Tarea | Aceptada (el móvil consume `/mios`) |
| BT-MIS-TESTS | Tarea | Aceptada (unit + E2E en el gate) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 47 se traslada. |

Pendiente menor (no compromiso): prueba on-device de que el móvil lista sólo los asignados (no se verificó en dispositivo este sprint; cubierto por el E2E). Backlog restante (revisión funcional §5/§6): bandeja sin georreferenciar navegable, indicador de estado de sincronización en la UI, avisos de conflictos/pendientes, biométrico nativo.

## 7. Decisiones tomadas durante el review

- Endpoint **aditivo** `/mios` en vez de cambiar `GET /relevamientos` (que sigue sirviendo el listado de área a jefes/web).
- Filtrar **en la base** por la asignación, no en memoria.
- Defensa: un usuario dado de baja (`EstadoVigencia == false`) no recibe nada, aunque tenga asignaciones.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 47 (endpoint "relevamientos asignados a mí"). Veredicto Cumplido, velocity 8, 0 carry-over, 386 pruebas (+4); cobertura del gate mantenida. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
