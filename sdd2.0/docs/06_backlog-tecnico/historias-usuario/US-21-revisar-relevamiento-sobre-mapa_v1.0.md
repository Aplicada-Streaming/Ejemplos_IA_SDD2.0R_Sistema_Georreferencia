# US-21 — Revisar el relevamiento sobre el mapa por marcadores

**Proyecto:** GeoVial
**Documento:** US-21-revisar-relevamiento-sobre-mapa_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-05 Revisión sobre mapa
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero revisar un relevamiento sobre el mapa recorriendo sus marcadores con las observaciones agrupadas, para evaluar el estado de la obra y confeccionar mis informes sin cruzar planillas contra carpetas de fotos.

## 2. Contexto

NB-04 centraliza la revisión sobre un mapa donde cada punto agrupa sus fotos y comentarios. CU-08 muestra los marcadores ubicados según sus coordenadas y la bandeja sin georreferenciar, acotado por área (RN-01). Es la propuesta de valor para el jefe de área frente al cruce manual actual.

## 3. Criterios de aceptación

- Given un relevamiento "Puente Río 12" en revisión con cinco marcadores de la "Zona Norte", When el jefe de "Zona Norte" lo abre sobre el mapa, Then el sistema muestra los cinco marcadores ubicados y la bandeja sin georreferenciar.
- Given un relevamiento de la "Zona Sur", When un jefe de la "Zona Norte" intenta abrirlo, Then el sistema responde `ACCESO_NO_AUTORIZADO`.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-08 |
| BT derivadas | BT-07, BT-09, BT-11 |
| Tests previstos | acceptance/AT-08-revision-mapa |

## 5. Prioridad y estimación

Must: la revisión sobre mapa es el objetivo del jefe de área. 8 SP (Fibonacci): vista de mapa con marcadores, agrupación de observaciones y autorización por área; lado de lectura del módulo CQRS.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-08)
- [x] Reglas de negocio identificadas (RN-01, RN-05)
- [x] Dependencia con US-11/US-12 (marcadores) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El carrusel y la navegación entre marcadores se cubren en US-22. La exportación e importación se cubren en US-27 y US-28. El recurso de mapa es el mismo en web y móvil (ADR-04).
