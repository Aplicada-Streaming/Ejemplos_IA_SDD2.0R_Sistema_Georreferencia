# Sprint Review — Sprint 66

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-66_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-66_v1.0.md`:

> Unificar el andamiaje HTML de los tres mapas Leaflet+OSM del móvil (revisión, ubicación, captura) en un único lugar (control de mapa compartido), preservando el comportamiento.

Veredicto: Cumplido.

Explicación corta: se introdujo el andamiaje `MapaLeaflet.Documento(estilos, cuerpo, script)` (gate), **único lugar** para la versión de Leaflet (1.9.4), las teselas de OSM (reusa `MapaTeselas`) y la estructura del documento (cabecera, `div#mapa`, creación del mapa y capa de teselas). Los tres builders —`MapaRevisionHtml`, `MapaUbicacionHtml`, `MapaCapturaHtml`— dejaron de duplicar ese boilerplate y ahora aportan sólo sus estilos, su cuerpo y su script propios. Es un **refactor que preserva comportamiento**: los tres suites de tests de mapas quedaron verdes sin cambios (prueba de preservación), y la API pública de cada builder (`Construir`) no cambió, así que los consumidores no se tocaron.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| DEUDA-MAPA | Refactor | `MapaLeaflet` centraliza Leaflet/OSM/estructura; los 3 mapas delegan | Una sola versión de Leaflet |
| DEUDA-MAPA | Calidad | Los 3 suites de mapas verdes sin cambios (preservación de comportamiento) | Sin regresión observable |
| DEUDA-MAPA | On-device | Mapa de revisión, map-pick y mapa de captura siguen funcionando | Verificado en el dispositivo |

## 3. Feedback recibido

- Si mañana hay que subir la versión de Leaflet o cambiar el origen de teselas, ahora es **un solo lugar** (antes, tres).
- La preservación quedó respaldada por los tests existentes de los tres mapas (no hicieron falta cambios en ellos).
- El refactor no tocó la API de los builders ni sus consumidores: riesgo acotado.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **498** (454 unitarias + 44 de integración), **+2 unitarias** del andamiaje `MapaLeaflet`; los tres suites de mapas (revisión, ubicación, captura) siguen verdes **sin cambios**, lo que evidencia la preservación de comportamiento. Cobertura DoD sin regresión. El MAUI compila (`net10.0-android`, arm64) y se redeployó; los tres mapas verificados on-device.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| DEUDA-MAPA | Tarea | Aceptada (`MapaLeaflet` + 3 builders refactorizados) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

## 7. Decisiones tomadas

- El andamiaje provee `var mapa = L.map('mapa')` + la capa de teselas (comunes); cada mapa concreto aporta su script (centro/marcadores/interacciones) que opera sobre `mapa`.
- Se mantiene el patrón de **tokens** (sin interpolación de C#) para no escapar las llaves del JS/CSS; el builder resuelve sus tokens antes de delegar y el andamiaje resuelve los suyos.
- Se preserva la API pública de los builders (`Construir`): los consumidores (CapturaPage, MapaSeleccionPuntoPage, MapaPage…) no se tocan.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Review del Sprint 66 (control de mapa compartido: andamiaje `MapaLeaflet` + refactor de los 3 builders). Cumplido, velocity 5, 0 carry-over, 498 pruebas (+2 del andamiaje; 3 suites de mapas verdes sin cambios). Generado por AG-07 |
