# Sprint Review — Sprint 60

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-60_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-60_v1.0.md`:

> Declarar y pedir el permiso de ubicación y agregar "📍 Mi ubicación" que recentre el mapa Leaflet en la posición del agente con una marca distinta.

Veredicto: Cumplido.

Explicación corta: se cerró **H-02** (P1, el más cercano a P0 de la auditoría). Se declararon `ACCESS_FINE/COARSE_LOCATION` en el manifiesto (antes sólo `CAMERA`), se agregó `UbicacionDispositivo` (pide el permiso en runtime + `Geolocation`, con fallback a la última conocida) y un núcleo puro `ScriptUbicacionDispositivo` (gate) que genera el JS para recentrar el mapa y dejar una marca azul distinta **"Tu posición"**. La acción **"📍 Mi ubicación"** está en `MapaPage` y en el map-pick de la captura/bandeja (`MapaSeleccionPuntoPage`). Verificado on-device: el mapa pasó de la vista por defecto (toda Argentina) a **nivel calle en la posición real del dispositivo**, con el punto azul.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| H-02-GPS | Funcionalidad | Mapa por defecto (`19-mapa-antes.png`) → "📍 Mi ubicación" → recentrado a nivel calle con marca "Tu posición" (`20-mapa-centrado-gps.png`) | Es la base GIS de campo que faltaba |
| H-02-GPS | Permiso | Primer uso pide el permiso de ubicación (antes ni se solicitaba) | Correcto |
| BT-GPS-NUCLEO | Núcleo | `ScriptUbicacionDispositivo`: centra + marca distinta, punto decimal, defensivo | Bien aislado y probado |

## 3. Feedback recibido

- La marca de posición es un `circleMarker` azul **distinto** de los pines de marcadores: el agente no la confunde con una observación.
- El núcleo formatea con InvariantCulture (punto decimal): evita el clásico bug de coma que rompe el JS en es-AR.
- La acción está donde sirve: en el mapa de revisión y en el map-pick (para colocar la captura cerca de la posición del agente).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **484** (440 unitarias + 44 de integración), **+4** del núcleo `ScriptUbicacionDispositivo`. Integración sin cambios (sin backend). Cobertura del gate sin regresión. El MAUI compila (`net10.0-android`, arm64) y se redeployó al moto g42 con verificación on-device del antes/después (centrado real por GPS + permiso solicitado).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| H-02-GPS | Historia | Aceptada (permiso + "Centrar por GPS" con marca de posición) |
| BT-GPS-NUCLEO | Tarea | Aceptada (`ScriptUbicacionDispositivo` + tests) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

Pendiente del backlog de la auditoría (P1 estructurales): **H-01/H-03** (captura sobre el mapa con posición y pines), **H-04** (carrusel deslizable). Mejora futura: una marca de posición "viva" que siga al agente (no sólo al tocar el botón).

## 7. Decisiones tomadas

- La marca de posición se **reemplaza** al recentrar (no se acumula): `window.__posDisp`.
- "Centrar por GPS" es una acción explícita (toque), no un seguimiento continuo (ahorra batería y permiso de fondo).
- El permiso es `LocationWhenInUse` (en uso), no en segundo plano.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Review del Sprint 60 (H-02: permiso de ubicación + "Centrar por GPS"). Cumplido, velocity 5, 0 carry-over, 484 pruebas (+4 del núcleo); verificado on-device (centrado real por GPS). Generado por AG-07 |
