# US-23 — Filtrar fotos y observaciones por etiquetas

**Proyecto:** GeoVial
**Documento:** US-23-filtrar-fotos-observaciones-etiquetas_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-05 Revisión sobre mapa
**Prioridad MoSCoW:** Could
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero filtrar las fotos y observaciones de un relevamiento por una o más etiquetas, para concentrarme en un tipo de hallazgo durante la revisión sin recorrer todo el relevamiento.

## 2. Contexto

El BRIEF (§4 Could Have) plantea el filtrado por etiquetas en la revisión web. CU-08 (flujo 5.C) muestra solo las observaciones que coinciden con las etiquetas elegidas. Es una ayuda de productividad sobre la revisión, no un bloqueante del MVP.

## 3. Criterios de aceptación

- Given un relevamiento con observaciones etiquetadas, When el jefe filtra por la etiqueta "fisura", Then el sistema muestra solo las observaciones con esa etiqueta.
- Given un filtro por una etiqueta sin coincidencias, When el jefe lo aplica, Then el sistema indica que no hay observaciones que coincidan.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-08 |
| BT derivadas | BT-07, BT-11 |
| Tests previstos | acceptance/AT-08-filtro-etiquetas |

## 5. Prioridad y estimación

Could: agrega valor a la revisión pero el MVP funciona sin filtros. 3 SP (Fibonacci): consulta filtrada sobre etiquetas ya modeladas (RC-04).

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-08)
- [x] Reglas conceptuales identificadas (RC-04 etiquetas)
- [x] Dependencia con US-15 (etiquetas) y US-21 (revisión) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

Las etiquetas se aplican a fotos o comentarios (RC-04); el filtro opera sobre ese etiquetado existente.
