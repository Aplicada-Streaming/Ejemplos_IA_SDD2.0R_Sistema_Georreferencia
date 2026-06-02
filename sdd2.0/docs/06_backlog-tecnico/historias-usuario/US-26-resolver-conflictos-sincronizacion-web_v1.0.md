# US-26 — Resolver conflictos de sincronización desde la web

**Proyecto:** GeoVial
**Documento:** US-26-resolver-conflictos-sincronizacion-web_v1.0.md
**Versión:** 1.0
**Estado:** Entregada (radio S05, ediciones S18)
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-06 Resolución de conflictos
**Prioridad MoSCoW:** Should
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero unificar o mantener separados los marcadores en un mismo radio y dirimir las ediciones en conflicto desde la web, para dejar la base del relevamiento consistente antes de cerrarlo.

## 2. Contexto

NB-05 exige resolver desde la web los conflictos de sincronización (BRIEF §4 Should Have, casos 1 y 4). CU-12 aplica la decisión humana sobre la lista de conflictos (US-25), levanta la marca de conflicto (RN-04) y registra la resolución (RN-07). Es lo que compensa el last-write-wins evitando descartes silenciosos.

## 3. Criterios de aceptación

- Given dos marcadores en un mismo radio listados como conflicto, When el jefe decide unificarlos en uno, Then el sistema fusiona sus observaciones en un único marcador y levanta el conflicto.
- Given dos marcadores en un mismo radio listados como conflicto, When el jefe decide mantenerlos separados, Then el sistema conserva ambos marcadores y levanta la marca de conflicto.
- Given un conflicto pendiente en un relevamiento cerrado, When el jefe intenta resolverlo, Then el sistema responde `RELEVAMIENTO_SOLO_LECTURA`.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-05 |
| CU cubiertos | CU-12 |
| BT derivadas | BT-07, BT-09, BT-17, BT-18 |
| Tests previstos | acceptance/AT-12-resolucion-conflictos |

## 5. Prioridad y estimación

Should: cierra el ciclo de consistencia de NB-05; el MVP es defendible con detección y resolución posterior. 8 SP (Fibonacci): unificación con reasignación de observaciones, dirimir ediciones y levantar marcas de conflicto, todo auditado.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-12)
- [x] Reglas de negocio identificadas (RN-02, RN-04, RN-05, RN-07, RN-01)
- [x] Dependencia con US-25 (lista de conflictos) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La resolución es siempre una decisión humana; el sistema no unifica ni descarta automáticamente. La unificación reasigna las observaciones al marcador resultante conservando fotos, comentarios y etiquetas. El usuario raíz puede intervenir (relacionado con US-03).
