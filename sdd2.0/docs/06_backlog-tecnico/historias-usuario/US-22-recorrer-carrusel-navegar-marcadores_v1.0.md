# US-22 — Recorrer el carrusel y navegar entre marcadores

**Proyecto:** GeoVial
**Documento:** US-22-recorrer-carrusel-navegar-marcadores_v1.0.md
**Versión:** 1.0
**Estado:** Entregada (backend S04, frente cliente S17)
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-05 Revisión sobre mapa
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero recorrer las fotos de un marcador en un carrusel y pasar al marcador siguiente o anterior, para revisar todo el relevamiento de forma fluida sin volver al mapa en cada paso.

## 2. Contexto

NB-04 plantea la visualización de un marcador con carrusel de fotos, comentarios por foto, etiquetas y navegación entre marcadores. CU-09 (flujo 5.B) habilita recorrer el carrusel y navegar al marcador destino, también en modo solo lectura sobre un relevamiento cerrado. Sin esta historia, la revisión sería marcador por marcador con saltos al mapa.

## 3. Criterios de aceptación

- Given un marcador con cinco fotos, When el usuario recorre el carrusel y navega al marcador siguiente, Then el sistema avanza por las cinco fotos y carga el marcador siguiente con su carrusel.
- Given un relevamiento cerrado, When el usuario recorre el carrusel de un marcador, Then el sistema permite recorrer y navegar pero no agregar ni quitar contenido.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-09 |
| BT derivadas | BT-07, BT-09, BT-11, BT-20 |
| Tests previstos | acceptance/AT-09-carrusel-navegacion |

## 5. Prioridad y estimación

Must: la navegación fluida es parte de la revisión usable declarada en el alcance. 5 SP (Fibonacci): carrusel y navegación entre marcadores con modo solo lectura.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-09)
- [x] Reglas de negocio identificadas (RN-05, RN-01)
- [x] Dependencia con US-15/US-21 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El visor a pantalla completa con zoom es un Could aparte (US-24). El detalle visual del carrusel pertenece a 03.
