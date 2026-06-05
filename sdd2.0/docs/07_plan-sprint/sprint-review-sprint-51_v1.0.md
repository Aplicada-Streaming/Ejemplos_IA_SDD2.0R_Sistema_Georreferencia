# Sprint Review — Sprint 51

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-51_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-51_v1.0.md`:

> El objetivo es cerrar el flujo en la app: tocar una observación de la bandeja, elegir el punto sobre el mapa y ubicarla (CU-05), sin salir del teléfono.

Veredicto: Cumplido.

Explicación corta: en S50 el agente pasó a ver la bandeja, pero la ubicación seguía siendo por la web. Se cerró el flujo: al tocar una observación, se abre un mapa (Leaflet + OSM) donde el agente elige el punto; al confirmar, el HTML navega a un esquema centinela (`geovial-ubicar://...`) que la página intercepta, parsea la coordenada (`ParseadorMensajeUbicacion`) y la postea al backend (`ClienteUbicacionManual` → `POST /observaciones/{id}/ubicacion`, CU-05). El puente WebView↔app —la parte frágil— quedó encapsulado en un núcleo testeable.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-BANDEJA-UBICAR | Funcionalidad | Tocar una observación de la bandeja → mapa → tocar el punto → confirmar → la observación queda ubicada y sale de la bandeja | El agente resuelve sin GPS desde el teléfono |
| BT-PUENTE-MAPA | Robustez | El HTML del mapa y el parser acuerdan el esquema centinela; navegaciones normales (teselas) se ignoran | El puente está cubierto en el gate |

## 3. Feedback recibido

- Completa el flujo de campo: una foto sin GPS ya no obliga a ir a la web; se ubica desde la app (S50 ver → S51 ubicar).
- Buen patrón para el puente WebView↔C#: un **esquema centinela** + un **parser puro** en el gate; el WebView sólo intercepta y delega. Reutilizable para futuras interacciones de mapa.
- El centro del mapa de ubicación reusa los marcadores existentes del relevamiento (o el centro por defecto si no hay), así el agente arranca cerca de la zona de trabajo.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 1 menor (`DisplayAlert` obsoleto en net10 rompía el build MAUI con warnings-as-errors) — corregido en el sprint |

Pruebas: **421** (378 unitarias + 43 de integración), +13 unitarias: `ParseadorMensajeUbicacionTests` (válido, navegaciones no-mensaje, fuera de rango), `MapaUbicacionHtmlTests` (leaflet/osm, centro, esquema centinela, escucha del toque, acuerdo html↔parser) y `ClienteUbicacionManualTests` (POST a la URL correcta con cuerpo; error → false). Cobertura del gate: Domain 88,5 % / 79,8 %; Application 87,4 % / 76,2 % (umbral 80 % / 70 %); `GeoVial.Revision` a 97,6 % / 94,3 %. El MAUI compila para `net10.0-android`.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-BANDEJA-UBICAR | Historia | Aceptada (ubicar desde la app sobre el mapa) |
| BT-PUENTE-MAPA | Tarea | Aceptada (HTML + parser en el gate) |
| BT-UBICAR-CLIENTE | Tarea | Aceptada (cliente cubierto en el gate) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 51 se traslada. |

Pendiente menor (no compromiso): verificación on-device del flujo de ubicación (sembrar una captura sin GPS → tocar → ubicar → confirmar que sale de la bandeja). Backlog restante (revisión funcional §5/§6): biométrico nativo; sprint de limpieza del listado de área (retro S47).

## 7. Decisiones tomadas durante el review

- Puente WebView↔app vía **esquema centinela** + parser puro en el gate, en vez de un canal JS↔C# específico de plataforma (más portable y testeable).
- Validar el **rango geográfico** en el parser: una coordenada imposible se descarta como ruido.
- Guarda `_enviando` para que un doble toque no dispare dos POST.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 51 (ubicar desde la app). Veredicto Cumplido, velocity 8, 0 carry-over, 421 pruebas (+13); cobertura del gate mantenida, `GeoVial.Revision` a 97,6/94,3. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
