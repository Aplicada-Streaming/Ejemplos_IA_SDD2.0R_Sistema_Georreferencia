# US-25 — Detectar marcadores en conflicto por radio configurable

**Proyecto:** GeoVial
**Documento:** US-25-detectar-marcadores-conflicto-radio_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-06 Resolución de conflictos
**Prioridad MoSCoW:** Should
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero ver listados los marcadores que quedaron dentro de un mismo radio y las ediciones en conflicto, para saber qué necesita una decisión mía antes de cerrar el relevamiento.

## 2. Contexto

NB-05 busca consistencia de datos ante marcadores duplicados (R-01) y conflictos. CU-11 evalúa la cercanía por radio (RN-02) e incorpora los recursos marcados por última escritura (RN-04), sin unificar ni descartar nada de forma automática. Es el paso previo a la resolución manual (US-26).

## 3. Criterios de aceptación

- Given un relevamiento con radio 15 m y dos marcadores a 9 m de distancia, When el jefe consulta los conflictos, Then el sistema los lista como conflicto en un mismo radio sin unificarlos.
- Given dos marcadores a 30 m con radio 15 m, When el jefe consulta los conflictos, Then el sistema no los lista como conflicto.
- Given un relevamiento con radio 15 m y dos marcadores a 20 m, When el jefe amplía el radio a 25 m, Then el sistema reevalúa y los lista como conflicto en un mismo radio.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-05 |
| CU cubiertos | CU-11 |
| BT derivadas | BT-07, BT-09, BT-17 |
| Tests previstos | acceptance/AT-11-deteccion-conflictos |

## 5. Prioridad y estimación

Should: el MVP es defendible sin resolución de conflictos, pero la detección es la base de la consistencia. 5 SP (Fibonacci): cálculo de cercanía por radio reconfigurable y consolidación de la lista de conflictos.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-11)
- [x] Reglas de negocio identificadas (RN-02, RN-04, RN-01)
- [x] Dependencia con US-18 (sincronización que genera conflictos) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La detección señala, no resuelve: la resolución es US-26. Ningún marcador se unifica ni descarta de forma automática (RN-02).
