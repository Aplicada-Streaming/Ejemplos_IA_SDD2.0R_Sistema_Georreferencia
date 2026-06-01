# ADR-04 — Mapas con OpenStreetMap + Leaflet en web y móvil

**Proyecto:** GeoVial
**Documento:** ADR-04-mapas-openstreetmap-leaflet_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Comunicación

## 1. Contexto

GeoVial es un sistema centrado en mapa: el agente ubica puntos y centra por posicionamiento en móvil; el jefe de área revisa marcadores sobre el mapa con carrusel (CU-04, CU-05, CU-08, CU-09). La georreferenciación prioriza los metadatos de la foto y, en su ausencia, la ubicación manual del punto sobre el mapa (RN-03, CU-05). Se busca un mismo recurso de mapa en web y móvil, sin costos de licencia para un organismo público. El cliente fijó OpenStreetMap + Leaflet (PROJECT-README §2). NFR asociado: confiabilidad de georreferenciación ≥ 95%.

## 2. Decisión

Se adopta OpenStreetMap como fuente de tiles y Leaflet como biblioteca de mapa interactivo, integrada tanto en el front web Blazor como en la app móvil MAUI (vía componente web embebido). El mismo recurso de mapa se usa para crear y ubicar marcadores, mover el pin, centrar por posicionamiento en móvil y revisar sobre mapa.

## 3. Estado

Aceptado el 2026-06-01. Renumeración de `ADR-004` (PROJECT-README §15, decidido por el cliente) a `ADR-04`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| OpenStreetMap + Leaflet (elegido) | Sin costo de licencia; mismo recurso web y móvil; biblioteca madura y liviana | Calidad de tiles dependiente de la cobertura OSM en la región |
| Proveedor de mapas comercial con SDK | Tiles y geocoding de alta calidad | Costo de licencia recurrente; no apto para uso interno de organismo público sin presupuesto fijado |
| Renderizado de mapa nativo por plataforma | Integración nativa | Divergencia entre web y móvil; duplica el esfuerzo de UI de mapa |

## 5. Consecuencias positivas

1. Mismo recurso de mapa en web y móvil reduce el esfuerzo de UI y mantiene coherencia visual.
2. Sin costos de licencia, alineado a un organismo público.
3. Leaflet permite mover el pin, agrupar marcadores y centrar por posicionamiento.

## 6. Consecuencias negativas y trade-offs

1. La calidad de tiles depende de la cobertura OSM; aceptado para el alcance de puentes y caminos del organismo.
2. La integración en MAUI se hace vía componente web embebido, lo que ata el mapa a la capa Blazor del móvil.

## 7. Implementación

`GeoVial.Web` y `GeoVial.Mobile` integran Leaflet sobre tiles OSM. El módulo de georreferenciación deriva la coordenada del marcador priorizando metadatos de la foto (RN-03); ante ausencia, la ubicación manual usa el pin de Leaflet (CU-05); si tampoco se ubica, la observación cae en la bandeja sin georreferenciar (`OBSERVACION_SIN_GEORREFERENCIA`). El radio de agrupación visualiza los marcadores candidatos (RN-02).

## 8. Métricas de validación

- ≥ 95% de observaciones con coordenada válida automática (NFR de georreferenciación).
- Una foto sin metadatos solicita ubicación manual del pin (suite de georreferenciación, 08).
- El mismo componente de mapa se carga en web y móvil.

## 9. Referencias

- PROJECT-README §2 (stack, mapas).
- RN-02, RN-03; CU-04, CU-05, CU-08, CU-09.
- ADR-06 (conflictos por radio).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Decisión aceptada. Renumeración de ADR-004 a ADR-04 |
