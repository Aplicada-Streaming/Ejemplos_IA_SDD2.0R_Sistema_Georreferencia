# Plan de Iteración — Sprint 66

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-66_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-12-11
**Fecha fin:** 2028-12-22
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 5,0 SP (S63–S65). Sprint de **deuda técnica** reiterada en varias retros y que subió de prioridad al introducir S65 el **tercer** HTML de mapa casi idéntico: unificar el andamiaje de los mapas Leaflet+OSM.

## 2. Objetivo del sprint

**Unificar el andamiaje HTML de los tres mapas Leaflet+OSM del móvil** (revisión, ubicación, captura) en un único lugar (control de mapa compartido), eliminando la triplicación del boilerplate. **Refactor que preserva el comportamiento** (mismo HTML observable; los tests de los tres mapas siguen verdes).

- Antes: `MapaRevisionHtml`, `MapaUbicacionHtml` y `MapaCapturaHtml` repetían la cabecera de Leaflet (versión/CDN), la capa de teselas de OSM y la estructura del documento.
- Después: un andamiaje `MapaLeaflet` aporta esa parte común; cada mapa concreto sólo aporta sus **estilos**, su **cuerpo** y su **script** (marcadores e interacciones).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| DEUDA-MAPA | Tarea | Control de mapa compartido: andamiaje `MapaLeaflet` + refactor de los 3 builders | Media | 5 | Dev móvil (AG-08) | Cerrada |

Total: 5 SP (refactor de deuda técnica, sin feature nuevo; preserva comportamiento).

## 4. Alcance técnico

- **Núcleo nuevo (`MapaLeaflet`, GeoVial.Revision, gate):** `Documento(estilos, cuerpo, script)` arma el HTML — cabecera Leaflet 1.9.4 (un único lugar para la versión/CDN) + `<style>` inyectado + cuerpo + `var mapa = L.map('mapa')` + capa de teselas de OSM (reusa `MapaTeselas`) + script inyectado. Tokens (sin interpolación) para no escapar llaves del JS/CSS.
- **`MapaRevisionHtml`, `MapaUbicacionHtml`, `MapaCapturaHtml`:** dejan de traer el documento completo; cada uno arma sólo su `script` (con sus tokens ya resueltos: pines, centro, esquema) y sus estilos/cuerpo, y delega en `MapaLeaflet.Documento(...)`. La **API pública** de cada builder (`Construir`) no cambia: los consumidores (CapturaPage, MapaSeleccionPuntoPage, MapaPage…) no se tocan.
- **Sin cambios de comportamiento.** El HTML resultante conserva lo que los tests verifican (Leaflet, teselas OSM, centro/bounds, marcadores, esquema centinela, `mapa.on('click')`).
- **Sin backend.**

## 5. Definition of Done aplicada

- Un único lugar para la versión de Leaflet y el origen de teselas; los tres mapas se arman desde el andamiaje.
- Los tres mapas siguen funcionando igual (revisión con pines clicables, ubicación con barra de confirmar, captura con contexto + pin tap-to-place).
- Suite del gate verde con el núcleo nuevo cubierto y **los tres suites de mapas sin cambios** (prueba de preservación de comportamiento); cobertura DoD sin regresión.
- El MAUI compila y los mapas se verifican on-device.

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| El refactor cambia sutilmente el HTML y rompe un mapa | Media | Medio | Los tests de los 3 mapas (preservación) deben quedar verdes; verificación on-device de los 3 |
| Orden de resolución de tokens (andamiaje vs script del mapa) | Baja | Medio | El builder resuelve sus tokens antes de delegar; el andamiaje sólo resuelve los suyos |

## 7. Criterios de hecho del sprint

Completo cuando: los tres mapas se arman desde `MapaLeaflet`; los tres suites de mapas quedan verdes (preservación de comportamiento) y el núcleo nuevo cubierto; el MAUI compila y los mapas se verifican on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Deuda técnica "control de mapa compartido" (retros S62/S65) |
| ADR/UX | US-21 (mapa interactivo); reuso de `MapaTeselas` |
| Componentes | `GeoVial.Revision` (`MapaLeaflet`, `MapaRevisionHtml`, `MapaUbicacionHtml`, `MapaCapturaHtml`) |
| Calidad | definition-of-done §1.4; preservación verificada por los tests existentes |
| Tests | `MapaLeafletTests` (núcleo) + 3 suites de mapas sin cambios (verdes) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Plan del Sprint 66 (control de mapa compartido: andamiaje `MapaLeaflet` + refactor de los 3 builders, preservando comportamiento). 5 SP. Generado por AG-07 |
