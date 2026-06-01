# US-11 — Capturar observación con georreferenciación automática

**Proyecto:** GeoVial
**Documento:** US-11-capturar-observacion-georreferenciacion-automatica_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-03 Captura y georreferenciación
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero tomar una foto y que el sistema le asigne automáticamente la coordenada geográfica, para registrar la observación sin cargar la ubicación a mano.

## 2. Contexto

NB-02 exige georreferenciación automática y confiable. CU-04 deriva la coordenada priorizando los metadatos de ubicación de la foto (RN-03) y crea o asocia el marcador. Es la propuesta de valor central del producto: eliminar la carga manual de coordenadas y la dispersión de fotos sueltas.

## 3. Criterios de aceptación

- Given un relevamiento en recolección con radio 15 m sin marcadores, When el agente toma una foto con metadatos de ubicación en un punto nuevo, Then el sistema crea un marcador en esa coordenada y asocia la observación.
- Given una foto sin metadatos de ubicación, When el agente intenta registrar la observación, Then el sistema responde `OBSERVACION_SIN_GEORREFERENCIA` y deriva a la ubicación manual del punto.
- Given un relevamiento cerrado, When el agente intenta capturar, Then el sistema responde `RELEVAMIENTO_SOLO_LECTURA`.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-04 |
| BT derivadas | BT-05, BT-06, BT-07, BT-09, BT-19 |
| Tests previstos | acceptance/AT-04-captura-georreferenciada |

## 5. Prioridad y estimación

Must: es la capacidad diferenciadora del sistema. 8 SP (Fibonacci): captura de foto, derivación de coordenada desde metadatos, creación/asociación de marcador y persistencia de la observación.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-04)
- [x] Reglas de negocio identificadas (RN-03, RN-02, RN-05, RN-01)
- [x] Dependencia con US-06 (relevamiento con radio) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La agrupación por radio se detalla en US-12. La captura sin conexión se cubre en US-16; aquí la captura es independiente del estado de conexión. El mecanismo de obtención de la coordenada vive en las BT (ADR-04).
