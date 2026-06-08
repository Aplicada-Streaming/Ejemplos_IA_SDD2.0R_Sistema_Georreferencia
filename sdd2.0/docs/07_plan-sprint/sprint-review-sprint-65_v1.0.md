# Sprint Review — Sprint 65

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-65_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-65_v1.0.md`:

> Que el agente fije la coordenada de una captura tocando (o arrastrando un pin sobre) el mapa embebido de la pantalla de Captura (evolución de H-01).

Veredicto: Cumplido.

Explicación corta: la pantalla de Captura completa la visión "capturar sobre el mapa" (S62): el mapa embebido —que renderiza desde S64— ahora es **interactivo**. Muestra los marcadores del relevamiento como **contexto** (círculos) y un **pin de captura** que el agente coloca tocando el mapa y afina arrastrándolo; la app recibe la coordenada por el esquema centinela `geovial-ubicar://` (el mismo de S51) y la fija como coordenada de la captura. La foto con EXIF mantiene la precedencia de su coordenada (RN-03); la página de map-pick (S56) y la carga manual siguen disponibles. El núcleo `MapaCapturaHtml` (gate) combina el contexto de `MapaRevisionHtml` con el "tocar para elegir" de `MapaUbicacionHtml`.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| H-01-EVOL | UX | Tocar el mapa de Captura coloca el pin y fija la coordenada; arrastrar lo afina | "Capturar sobre el mapa" sin página aparte |
| H-01-EVOL | Contexto | Los marcadores del relevamiento se ven como círculos para ubicarse | Captura en contexto |
| H-01-EVOL | Compatibilidad | EXIF mantiene precedencia; map-pick y carga manual siguen | Sin romper las vías previas |

## 3. Feedback recibido

- Cierra la fricción de tener que abrir una página aparte para elegir el punto cuando la foto no trae GPS.
- El mapa de contexto ayuda a no duplicar marcadores cercanos (el agente ve lo ya capturado mientras coloca el nuevo punto).
- Reusar el esquema/parser de S51 mantuvo el cambio chico y testeable.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **496** (452 unitarias + 44 de integración), **+7 unitarias** del núcleo nuevo `MapaCapturaHtml`. Gate verde; cobertura DoD sin regresión (`GeoVial.Revision` suma el núcleo, cubierto). El MAUI compila (`net10.0-android`, arm64) y se redeployó al moto g42; verificación on-device del toque sobre el mapa.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| H-01-EVOL | Historia | Aceptada (tap-to-place + arrastrar + contexto) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

## 7. Decisiones tomadas

- Núcleo nuevo `MapaCapturaHtml` (en vez de extender `MapaRevisionHtml`): el mapa de captura no abre la revisión al tocar un marcador, sino que coloca el pin de captura; separar evita ramas condicionales en el HTML de revisión. El "control de mapa compartido" (unificar los 3 HTML) queda como deuda técnica.
- Tocar el mapa avisa la coordenada **en vivo** (sin botón "Confirmar"): la pantalla de Captura ya tiene su "Enviar captura", que la usa.
- EXIF mantiene precedencia sobre el punto del mapa (RN-03): el mapa es la vía cómoda para fotos sin GPS.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Review del Sprint 65 (evolución de H-01: capturar tocando el mapa embebido). Cumplido, velocity 5, 0 carry-over, 496 pruebas (+7 del núcleo `MapaCapturaHtml`). Generado por AG-07 |
