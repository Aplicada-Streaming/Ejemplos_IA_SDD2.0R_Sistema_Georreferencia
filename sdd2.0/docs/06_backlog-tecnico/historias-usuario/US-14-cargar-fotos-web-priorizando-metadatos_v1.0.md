# US-14 — Cargar fotos desde la web priorizando metadatos

**Proyecto:** GeoVial
**Documento:** US-14-cargar-fotos-web-priorizando-metadatos_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-03 Captura y georreferenciación
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero cargar manualmente fotos desde la web y que el sistema use primero los metadatos de ubicación de cada foto, para incorporar observaciones tomadas fuera de la app sin recargar coordenadas a mano.

## 2. Contexto

El alcance Must del BRIEF incluye la carga manual de observaciones priorizando los metadatos de la foto, con el radio para agrupar. CU-05 (flujo 5.A) usa los metadatos como fuente primaria (RN-03) y solo pide ubicación manual si faltan. Es el relevamiento manual a través del sistema web declarado en el alcance.

## 3. Criterios de aceptación

- Given una foto cargada desde la web que trae metadatos de ubicación, When el jefe la carga, Then el sistema usa los metadatos como fuente de coordenada y la agrupa por radio.
- Given una foto cargada desde la web con metadatos de ubicación válidos, When el jefe intenta ubicarla manualmente, Then el sistema responde `FUENTE_UBICACION_INCORRECTA` y usa los metadatos.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-05 |
| BT derivadas | BT-05, BT-06, BT-07, BT-19 |
| Tests previstos | acceptance/AT-05-carga-web-metadatos |

## 5. Prioridad y estimación

Must: el relevamiento manual web es parte del alcance Must. 5 SP (Fibonacci): carga de foto en web, lectura de metadatos y prioridad de fuente de coordenada.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-05)
- [x] Reglas de negocio identificadas (RN-03, RN-02)
- [x] Dependencia con US-06 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La prioridad de fuente de coordenada (metadatos antes que ubicación manual) es una regla de dominio (RN-03); su lectura técnica vive en las BT.
