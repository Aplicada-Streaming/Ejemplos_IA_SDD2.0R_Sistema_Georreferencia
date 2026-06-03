# Sprint Review — Sprint 34

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-34_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-34_v1.0.md`:

> Dar **offline parcial** al mapa web de la revisión: un **Service Worker** que cachea las teselas de OpenStreetMap ya visitadas, de modo que las zonas navegadas se vean sin red.

Veredicto: Cumplido.

Explicación corta: el front web suma `wwwroot/sw-teselas.js`, un Service Worker **cache-first** que cachea sólo las teselas de OSM (host `tile.openstreetmap.org`, `.png`), maneja las respuestas opacas (no-cors) y acota la caché a 500 entradas con recorte FIFO; se registra en `App.razor`. Así, las zonas ya navegadas del mapa se ven **sin red**. El contrato de "qué URL es una tesela cacheable" se centralizó y testeó en `GeoVial.Revision.MapaTeselas.EsUrlDeTesela` (mismo criterio host+`.png` que el Service Worker), junto con la URL de teselas y la atribución; `MapaRevisionHtml` se refactorizó para reusar esa única fuente. El núcleo entra al gate (100 % cubierto); el Service Worker queda fuera del gate y se verifica por build + prueba manual. Alcance web (el WebView móvil no usa el Service Worker; su offline queda como mejora futura).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-21 | Producto | Cargar el mapa con red, cortar la red y volver a la zona vista: las teselas se siguen mostrando | Offline parcial del mapa web |
| US-21 | Calidad | `MapaTeselas` centraliza la config OSM y `EsUrlDeTesela` define qué se cachea (100 % cubierto) | Contrato del caché verificado |
| US-21 | Robustez | La caché se acota a 500 teselas (FIFO) para no crecer sin control | Almacenamiento bajo control |

## 3. Feedback recibido

- El offline parcial cubre el caso real (volver a una zona ya navegada sin red); las zonas nuevas siguen requiriendo conexión, lo cual es esperable.
- Centralizar la config de teselas en `MapaTeselas` quitó la duplicación de la URL/atribución entre el HTML móvil y el contrato del caché.
- Fue claro el límite de alcance: el Service Worker es de la web; el offline del WebView móvil es otro esfuerzo.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 332 verdes (295 unitarias + 37 de integración), +7 unitarias (`MapaTeselas`). Cobertura del gate: Domain líneas 89,7 % / branches 79,8 %; Application 90,0 % / 82,0 %; Revision 99,2 % / 100 % (`MapaTeselas` 100 % / 100 %). `GeoVial.Web` compila en Release sin warnings y el `dotnet publish` bundlea `sw-teselas.js`; el Service Worker queda fuera del gate.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-21-MAPA-OFFLINE | Tarea | Aceptada (Service Worker de caché de teselas + `MapaTeselas` en el gate; offline parcial web) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 34 se traslada. |

Backlog restante: offline de teselas en el móvil (caché nativa/WebView), bundle de Leaflet en el móvil, y mantenimiento continuo (PRs/alertas de Dependabot; evaluar automatizar la regla `-rc` como check de CI).

## 7. Decisiones tomadas durante el review

- Cachear sólo las teselas de OSM (no toda petición) con cache-first y FIFO acotado; centralizar el contrato en `MapaTeselas`.
- Cachear explícitamente las respuestas opacas (no-cors) de las teselas, ya que las imágenes de Leaflet llegan así.
- Declarar el alcance web; el offline del WebView móvil queda como mejora futura.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 34 (caché de teselas por Service Worker, offline parcial del mapa web). Veredicto Cumplido, velocity 8, 0 carry-over, 332 pruebas verdes. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
