# Plan de Iteración — Sprint 65

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-65_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-11-27
**Fecha fin:** 2028-12-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 5,0 SP (S62–S64). Sprint de **evolución de UX** elegido por el usuario: cerrar la visión "capturar sobre el mapa" (S62) dejando que el agente **fije la coordenada de la captura tocando el mapa embebido**.

## 2. Objetivo del sprint

**Que el agente fije la coordenada de una captura tocando (o arrastrando un pin sobre) el mapa embebido de la pantalla de Captura** (evolución de H-01), sin pasar por una página aparte ni tipear lat/lon.

- El mapa de Captura (que desde S64 renderiza) muestra los marcadores del relevamiento como **contexto** y suma un **pin de captura** tap-to-place.
- Tocar el mapa fija la coordenada de la captura; arrastrar el pin la afina. Si la foto trae EXIF, la coordenada del EXIF sigue teniendo precedencia (RN-03).
- Se conservan las vías previas (foto con EXIF, página de map-pick S56, carga manual) como alternativas.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| H-01-EVOL | Historia | Fijar la coordenada de la captura tocando el mapa embebido | Media | 5 | Dev móvil (AG-08) | Cerrada |

Total: 5 SP (UX que reusa núcleo del gate: parser del esquema S51, `VistaMapa`, intercepción del WebView, `FijarCoordenada`).

## 4. Alcance técnico

- **Núcleo nuevo (`MapaCapturaHtml`, GeoVial.Revision, gate):** arma el HTML del mapa de captura — Leaflet + OSM + los marcadores del relevamiento como **contexto** (círculos, no arrastrables) + un **pin de captura** que, al tocar el mapa o arrastrarlo, avisa la coordenada por el esquema centinela `geovial-ubicar://place?lat=..&lon=..` (el mismo que ya entiende `ParseadorMensajeUbicacion` de S51). Lógica pura y testeable; combina el contexto de `MapaRevisionHtml` con el "tocar para elegir" de `MapaUbicacionHtml`.
- **Glue (`CapturaPage.xaml.cs`):** el mapa pasa a `MapaCapturaHtml.Construir(...)`; el `MapaWebViewClient` (Android) recibe el callback de intercepción del esquema → `OnPuntoMapa(url)` → `ParseadorMensajeUbicacion.Intentar` → `FijarCoordenada` (la misma que usa la carga manual y el map-pick). El mensaje de "foto sin EXIF" guía a tocar el mapa.
- **Sin cambios de backend/Application/Domain.** Reusa: `ParseadorMensajeUbicacion` y el esquema (S51), `VistaMapa` (S30/S32), `MapaWebViewClient` con su callback (S55), `FijarCoordenada` (S56), la cola de capturas (S42).

## 5. Definition of Done aplicada

- En la pantalla de Captura, tocar el mapa coloca el pin y fija la coordenada de la captura; arrastrar el pin la afina; "Enviar captura" la usa.
- La foto con EXIF mantiene la precedencia de su coordenada (RN-03); la página de map-pick y la carga manual siguen disponibles.
- Suite del gate verde con el núcleo nuevo cubierto; cobertura DoD sin regresión.
- El MAUI compila y se verifica on-device.

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| Tocar el mapa genera navegaciones del esquema que el WebView intente abrir | Media | Medio | `MapaWebViewClient.ShouldOverrideUrlLoading` consume `geovial-*` (return true); patrón ya probado en S51/S55 |
| Confusión entre EXIF y punto del mapa | Baja | Bajo | EXIF mantiene precedencia (RN-03); el mensaje guía cuándo usar el mapa |
| Pin de captura confundido con marcadores existentes | Baja | Bajo | Contexto en círculos; pin de captura como marcador arrastrable con popup "Punto de la captura" |

## 7. Criterios de hecho del sprint

Completo cuando: tocar el mapa de Captura fija la coordenada de la captura (y arrastrar la afina); la suite del gate queda verde con el núcleo cubierto; el MAUI compila y se verifica on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Auditoría UX móvil `evaluacion-ux-mobile_v1.0.md` (evolución de H-01); elección del usuario para S65 |
| CU/UX | CU-04/CU-05 (captura y ubicación); experiencia-de-uso (capturar sobre el mapa) |
| Componentes | `GeoVial.Revision` (`MapaCapturaHtml`), `GeoVial.Mobile` (`CapturaPage`) |
| Calidad | definition-of-done §1.4 |
| Tests | `MapaCapturaHtmlTests` (núcleo); verificación on-device del glue |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Plan del Sprint 65 (evolución de H-01: fijar la coordenada de la captura tocando el mapa embebido). Núcleo `MapaCapturaHtml` (contexto + pin tap-to-place) + glue en `CapturaPage`, reusando el esquema/parser de S51. 5 SP. Generado por AG-07 |
