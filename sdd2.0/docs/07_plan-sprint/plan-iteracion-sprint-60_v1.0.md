# Plan de Iteración — Sprint 60

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-60_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-09-18
**Fecha fin:** 2028-09-29
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 7,0 SP (S57–S59). Se compromete una **mejora de UX (5 SP)** que reusa el HTML de mapa ya en el gate; sin backend.

## 2. Objetivo del sprint

Cerrar el hallazgo **H-02** de la auditoría UX móvil (`evaluacion-ux-mobile_v1.0.md`), el más cercano a un P0: **la app no usaba la ubicación del dispositivo** (el manifiesto sólo declaraba `CAMERA`), así que no había "Centrar por GPS" ni posición del agente en el mapa, contra experiencia-de-uso §6 y wireframes-captura-movil ("Centrar por GPS", "mapa con tu posición"). El objetivo es **declarar y pedir el permiso de ubicación** y agregar **"📍 Mi ubicación"** que recentre el mapa Leaflet en la posición del agente con una marca distinta.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| H-02-GPS | Historia | Permiso de ubicación + "Centrar por GPS" en el mapa y en el map-pick, con marca "tu posición" | Alta | 3 | Dev móvil (AG-08) | Cerrada |
| BT-GPS-NUCLEO | Tarea | Núcleo puro `ScriptUbicacionDispositivo` (JS de centrado + marca) + tests | Media | 2 | Dev móvil (AG-08) | Cerrada |

Total: 5 SP (UX; reusa el HTML de mapa del gate, sin backend).

## 4. Alcance técnico

- **Manifest:** `ACCESS_FINE_LOCATION` + `ACCESS_COARSE_LOCATION` + `uses-feature` GPS (no requerido).
- **Núcleo (gate, `GeoVial.Revision`):** `ScriptUbicacionDispositivo.Centrar(lat, lon)` → JS que hace `mapa.setView` y coloca/actualiza una marca azul distinta "Tu posición" (`circleMarker`, no se confunde con los pines). Punto decimal (InvariantCulture), defensivo si no hay mapa.
- **Glue de plataforma (`GeoVial.Mobile`):** `UbicacionDispositivo.ObtenerAsync()` estático: pide el permiso (`Permissions.LocationWhenInUse`) y consulta `Geolocation` (con fallback a la última conocida); devuelve `CoordenadaElegida?` (null si se deniega o no hay lectura), no lanza.
- **UI:** acción de toolbar **"📍 Mi ubicación"** en `MapaPage` y en `MapaSeleccionPuntoPage` (map-pick de la captura/bandeja); al tocar, obtiene el GPS y ejecuta el script con `EvaluateJavaScriptAsync`. Mensaje llano si no se pudo.
- **Sin cambios de backend/Application/Domain.** Sin registro nuevo en DI (helpers estáticos).

## 5. Definition of Done aplicada

- El mapa (y el map-pick) tienen "📍 Mi ubicación"; al tocarlo, la app pide el permiso (si falta) y recentra el mapa en la posición del agente con una marca distinta "Tu posición".
- Si el permiso se deniega o no hay lectura, se informa en lenguaje llano sin romperse.
- Suite del gate verde con el núcleo cubierto; cobertura DoD sin regresión.
- El MAUI compila y se verifica on-device (mapa pasa de la vista por defecto a nivel calle en la posición real).

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| Permiso denegado por el usuario | Media | Bajo | Mensaje llano; el resto del mapa sigue usable |
| Coma decimal por cultura rompe el JS | Baja | Alto | Núcleo formatea con InvariantCulture; cubierto por test |
| El mapa aún no cargó al pedir centrar | Media | Bajo | Script defensivo (`typeof mapa==='undefined'`); `MapaPage` carga antes de centrar |

## 7. Criterios de hecho del sprint

Completo cuando: el permiso de ubicación está declarado y se pide en runtime; "📍 Mi ubicación" recentra el mapa en la posición del agente con marca distinta en el mapa y en el map-pick; la suite del gate verde con el núcleo cubierto; el MAUI compila y se verifica on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Auditoría UX móvil `evaluacion-ux-mobile_v1.0.md`, hallazgo **H-02** (P1, roza P0) |
| CU/UX | experiencia-de-uso §6 (mapa con posición), wireframes-captura-movil §3-§4 ("Centrar por GPS", "mapa con tu posición") |
| Componentes | `GeoVial.Revision` (`ScriptUbicacionDispositivo`); `GeoVial.Mobile` (`UbicacionDispositivo`, `MapaPage`, `MapaSeleccionPuntoPage`, `AndroidManifest`) |
| Calidad | definition-of-done §1.4; gate Domain/Application líneas ≥80 % / ramas ≥70 % |
| Tests | `ScriptUbicacionDispositivoTests` (centra, marca distinta, punto decimal, defensivo) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Plan del Sprint 60 (H-02: permiso de ubicación + "Centrar por GPS"). Núcleo `ScriptUbicacionDispositivo` (gate) + `UbicacionDispositivo` (Permissions+Geolocation) + acción "📍 Mi ubicación" en mapa y map-pick. Sin backend. 5 SP. Generado por AG-07 |
