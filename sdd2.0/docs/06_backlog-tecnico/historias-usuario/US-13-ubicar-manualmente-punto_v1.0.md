# US-13 — Ubicar manualmente el punto de la observación

**Proyecto:** GeoVial
**Documento:** US-13-ubicar-manualmente-punto_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-03 Captura y georreferenciación
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo o jefe de área, quiero ubicar manualmente sobre el mapa el punto de una observación cuya foto no trae ubicación, para no perder esa observación cuando la georreferenciación automática falla.

## 2. Contexto

NB-02 admite que algunas fotos no traigan metadatos de ubicación (BRIEF §7 caso 2). CU-05 permite colocar el punto sobre el mapa o, si no se ubica, dejar la observación en la bandeja sin georreferenciar del relevamiento. Sin esta historia, una foto sin metadatos se perdería o quedaría inutilizable.

## 3. Criterios de aceptación

- Given una observación con foto sin metadatos en un relevamiento con radio 15 m, When el usuario coloca el punto a 5 m de un marcador existente, Then el sistema asocia la observación a ese marcador con la coordenada manual.
- Given una observación con foto sin metadatos, When el usuario no ubica el punto, Then el sistema la deriva a la bandeja sin georreferenciar con `OBSERVACION_SIN_GEORREFERENCIA`.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-05 |
| BT derivadas | BT-05, BT-06, BT-07 |
| Tests previstos | acceptance/AT-05-ubicacion-manual |

## 5. Prioridad y estimación

Must: garantiza que ninguna observación se pierda por falta de metadatos. 5 SP (Fibonacci): colocación sobre mapa, reagrupación por radio y bandeja sin georreferenciar.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-05)
- [x] Reglas de negocio identificadas (RN-03, RN-02, RN-05)
- [x] Dependencia con US-11 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La bandeja sin georreferenciar es una agrupación lógica dentro del relevamiento, no un relevamiento aparte. La carga manual desde la web priorizando metadatos se cubre en US-14.
