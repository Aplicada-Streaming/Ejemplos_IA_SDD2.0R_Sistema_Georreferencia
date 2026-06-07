# Sprint Review — Sprint 62

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-62_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-07
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-62_v1.0.md`:

> Embeber el mapa en la pantalla de captura (pines del relevamiento + posición del agente + "Centrar por GPS"), encima de los controles de captura, reusando lo ya construido.

Veredicto: Cumplido.

Explicación corta: la captura **ahora ocurre sobre el mapa**. Se embebió un mapa Leaflet+OSM en `CapturaPage`, encima de los controles, que muestra los **pines del relevamiento** (centrado en ellos vía `VistaMapa`) y permite **"📍 Mi ubicación"** para recentrar en la posición del agente (reusa S60). Cierra el núcleo de **H-01/H-03**, el último P1 estructural de la auditoría UX. Reusa `MapaRevisionHtml` (gate), el GPS de S60 y el caché de teselas de S38; el flujo de captura (EXIF/RN-03, map-pick S56, manual, encolado offline) queda intacto. Verificado on-device con Puente Río 12 (2 marcadores sembrados): la captura muestra el mapa con los 2 pines y el formulario debajo.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| H-01-MAPA-CAPTURA | Funcionalidad | Solapa Captura: mapa con los pines del relevamiento + cinta + formulario debajo (`08-captura.png` antes → `23-captura-con-mapa.png` después) | Por fin la captura tiene contexto geográfico |
| H-01-MAPA-CAPTURA | GPS | "📍 Mi ubicación" recentra el mapa de la captura en la posición del agente | Reusa lo de S60, consistente |

## 3. Feedback recibido

- La captura dejó de ser un formulario "a ciegas": el agente ve dónde está y los puntos ya tomados, como pide el wireframe §2.
- Reuso máximo: el mapa, el GPS y el caché de teselas ya estaban en el gate; el sprint fue glue de UI.
- El mapa es **contexto** (no rompe la captura si no carga sin conexión): decisión deliberada para no arriesgar la pantalla más crítica.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **489** (445 unitarias + 44 de integración), **sin nuevas**: el sprint reusa núcleos ya cubiertos (`MapaRevisionHtml`, `ScriptUbicacionDispositivo`) y es glue de UI (como S56). La suite del gate sigue verde y la cobertura sin regresión. El MAUI compila (`net10.0-android`, arm64) y se redeployó al moto g42; verificación on-device con datos sembrados (2 pines en el mapa de la captura).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| H-01-MAPA-CAPTURA | Historia | Aceptada (mapa con pines + posición + centrar-GPS en la captura) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

**Cierre del backlog de P1 de la auditoría UX:** con S59 (H-05), S60 (H-02), S61 (H-04) y S62 (H-01/H-03) quedan cerrados **los 4 P1 estructurales**. Evolución posible (no P1): fijar la coordenada de la captura arrastrando el pin sobre el mapa embebido (hoy se usa EXIF / map-pick S56 / manual). Restan los **P2** de pulido de la auditoría (carga al entrar sin "Recargar", disparador central, logout en overflow, contraste de placeholders, anuncios en vivo).

## 7. Decisiones tomadas

- El mapa de la captura es **contexto de sólo lectura** (pines + posición); la coordenada de la captura sigue por EXIF / map-pick (S56) / manual. La integración "arrastrar el pin aquí para fijar la captura" queda como evolución.
- El mapa se carga **una vez** por aparición de la solapa; alto fijo acotado para no comerse el viewport.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-07 | Review del Sprint 62 (H-01/H-03: mapa en la pantalla de captura). Cumplido, velocity 5, 0 carry-over, 489 pruebas (sin nuevas; reusa núcleos). Cierra los 4 P1 estructurales de la auditoría UX. Verificado on-device. Generado por AG-07 |
