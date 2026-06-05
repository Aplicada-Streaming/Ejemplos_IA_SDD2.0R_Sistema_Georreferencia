# Plan de Iteración — Sprint 56

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-56_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-07-24
**Fecha fin:** 2028-08-04
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S53–S55); se compromete una **mejora de UX acotada (5 SP)**: elegir el punto de la captura sobre el mapa. Es **glue que reusa el núcleo ya testeado** del map-pick (S51); se compromete por debajo de la capacidad a propósito (no hay núcleo nuevo). Verificación on-device.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cuando el agente sube una foto **del catálogo (galería) sin GPS**, la única forma de ubicarla antes de encolarla era **tipear lat/lon** — incómodo y propenso a errores. El objetivo es que pueda **elegir el punto sobre el mapa** (mover el marcador) y que la coordenada quede lista, reusando el map-pick de S51 (`MapaUbicacionHtml`). Se **conserva** la carga manual como alternativa. De paso se **consolida** el map-pick en una sola página reusable.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-CAPTURA-MAPA | Historia | Ubicar la captura (foto sin GPS) eligiendo el punto sobre el mapa, además de la carga manual | Alta | 3 | Dev móvil (AG-08) | Cerrada |
| BT-MAPA-CONSOLIDAR | Tarea | Página reusable `MapaSeleccionPuntoPage` (devuelve la coordenada); la bandeja la usa y postea; se elimina `MapaUbicacionPage` | Media | 2 | Dev móvil (AG-08) | Cerrada |

Total: 5 SP (mejora de UX acotada; glue que reusa núcleo del gate).

## 4. Alcance técnico

- **Reuso (sin cambios, en el gate):** `MapaUbicacionHtml.Construir(centroLat, centroLon)` (tocar/arrastrar el marcador → `geovial-ubicar://place?lat=..&lon=..`) + `ParseadorMensajeUbicacion.Intentar` (`GeoVial.Revision`) + la intercepción del esquema en `MapaWebViewClient`.
- **`MapaSeleccionPuntoPage` (nueva):** carga el HTML, intercepta el esquema y **devuelve `CoordenadaElegida?`** por `TaskCompletionSource` (sin backend).
- **`CapturaPage`:** botón "📍 Elegí el punto en el mapa" en `UbicacionPanel`; al volver con coordenada, valida (`ArmadorUbicacionManual`) y la fija en `_latManual`/`_lonManual` (mismo camino que la carga manual, que se conserva). `OnEnviar` sin cambios.
- **`BandejaPage`:** usa `MapaSeleccionPuntoPage` y **postea** con `ClienteUbicacionManual.UbicarAsync` (que ya inyectaba). Se **elimina** `MapaUbicacionPage` (su POST queda en `BandejaPage`).

## 5. Definition of Done aplicada

- Una foto sin GPS se puede ubicar eligiendo el punto en el mapa; la captura se encola con esa coordenada (no va a la bandeja). La carga manual sigue disponible.
- La bandeja sigue ubicando observaciones (regresión) con la página consolidada.
- Suite del gate verde (el núcleo del map-pick no cambia; cubierto por `UbicacionDesdeAppTests`). Cobertura DoD sin regresión.
- El MAUI compila y se verifica on-device.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Refactor de la bandeja (recién arreglada) | Media | Medio | La página compartida es el mismo WebView + intercepción que ya funciona; verificación on-device |
| El centro del mapa en la captura no está cerca de la obra | Media | Bajo | Centro por defecto + zoom del agente; centrar por GPS del dispositivo queda como mejora futura |
| Foto del catálogo de un lugar lejano | Baja | Bajo | Se mantienen ambas vías (mapa + manual) |

## 7. Criterios de hecho del sprint

Completo cuando: la captura permite elegir el punto en el mapa (y conserva la carga manual), la bandeja sigue ubicando con la página consolidada, la suite del gate sigue verde, el MAUI compila y se verifica on-device, y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Revisión on-device: la ubicación manual de la captura debería poder hacerse en el mapa |
| CU/RN | CU-05 (ubicación manual), RN-03 (fuente de ubicación), US-13 |
| Componentes | `GeoVial.Mobile` (`MapaSeleccionPuntoPage`, `CapturaPage`, `BandejaPage`); reuso de `GeoVial.Revision` (`MapaUbicacionHtml`, `ParseadorMensajeUbicacion`) |
| Calidad | definition-of-done §1.4 |
| Tests | Núcleo del map-pick ya cubierto por `UbicacionDesdeAppTests` (S51); este sprint es glue de UI |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 56 (UX: ubicar la captura en el mapa): `MapaSeleccionPuntoPage` reusable + botón en `CapturaPage` + consolidación de la bandeja (se elimina `MapaUbicacionPage`). Reusa el núcleo del map-pick (S51). 5 SP (acotado). Generado por AG-07 |
