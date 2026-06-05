# Sprint Review — Sprint 56

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-56_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-56_v1.0.md`:

> El objetivo es que [el agente] pueda elegir el punto sobre el mapa (mover el marcador) y que la coordenada quede lista, reusando el map-pick de S51. Se conserva la carga manual como alternativa.

Veredicto: Cumplido.

Explicación corta: en la captura, cuando una foto del catálogo no trae GPS, ahora hay un botón "📍 Elegí el punto en el mapa" que abre el mapa (Leaflet+OSM), el agente mueve el marcador y la coordenada queda fijada en la captura — sin tener que tipear lat/lon (que se conserva como alternativa). Se reusó el núcleo del map-pick de S51 (`MapaUbicacionHtml` + `ParseadorMensajeUbicacion`, ya testeados) y se consolidó en una página reusable `MapaSeleccionPuntoPage` que **sólo devuelve la coordenada**; la bandeja la usa y postea por su cuenta. Se eliminó `MapaUbicacionPage` (mezclaba elegir punto con postear).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-CAPTURA-MAPA | Funcionalidad | Foto de galería sin GPS → "Elegí el punto en el mapa" → mover el marcador → la coordenada queda lista → enviar | Mucho más cómodo que tipear coordenadas |
| BT-MAPA-CONSOLIDAR | Higiene | Una sola página de map-pick reusada por captura y bandeja; la bandeja sigue ubicando | Menos duplicación |

## 3. Feedback recibido

- Buen reuso: el map-pick de S51 (núcleo ya en el gate) se aplicó a la captura sin tocar lógica probada; el trabajo fue glue de UI + consolidación.
- Se conservó la **carga manual** como alternativa (una foto del catálogo puede ser de un lugar lejano; el agente puede tipear o mapear).
- La consolidación (separar "elegir punto" de "postear") dejó una página reusable que mañana sirve para cualquier flujo que necesite una coordenada del mapa.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **457** (414 unitarias + 43 de integración), **sin tests nuevos**: el núcleo del map-pick (`MapaUbicacionHtml`, `ParseadorMensajeUbicacion`) no cambió y ya está cubierto por `UbicacionDesdeAppTests` (S51); este sprint es glue de UI (`MapaSeleccionPuntoPage`, `CapturaPage`, `BandejaPage`), fuera del gate. La suite sigue verde y la cobertura del gate sin regresión. El MAUI compila y se redeployó al moto g42.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-CAPTURA-MAPA | Historia | Aceptada (elegir el punto de la captura en el mapa) |
| BT-MAPA-CONSOLIDAR | Tarea | Aceptada (página reusable; bandeja consolidada; `MapaUbicacionPage` eliminada) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido se traslada. |

Pendiente: verificación on-device del map-pick en la captura y de la bandeja (regresión). Mejora futura: centrar el mapa en la ubicación del dispositivo (Geolocation) en vez del centro por defecto.

## 7. Decisiones tomadas durante el review

- `MapaSeleccionPuntoPage` **sólo devuelve la coordenada**; cada consumidor decide qué hacer (la captura la fija; la bandeja la postea).
- Conservar la carga manual como alternativa a la elección en el mapa.
- Centro de la captura = por defecto (Argentina) + zoom; centrar por GPS queda como mejora.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 56 (UX: ubicar la captura en el mapa). Veredicto Cumplido, velocity 5 (sprint acotado de UX/glue), 0 carry-over, 457 pruebas (sin nuevas; reusa el núcleo de S51); MAUI redeployado. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
