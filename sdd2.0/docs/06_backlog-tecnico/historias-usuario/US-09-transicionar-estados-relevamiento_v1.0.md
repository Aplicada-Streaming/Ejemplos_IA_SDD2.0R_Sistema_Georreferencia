# US-09 — Transicionar estados del relevamiento

**Proyecto:** GeoVial
**Documento:** US-09-transicionar-estados-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-02 Relevamientos y asignación
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero hacer avanzar un relevamiento por sus estados recolección → revisión → cierre, para ordenar el trabajo entre la recolección de campo y la evaluación experta.

## 2. Contexto

NB-04 organiza el flujo recolección → revisión → cierre. CU-10 valida las transiciones permitidas (RN-05) y deja el relevamiento de solo lectura al cerrarlo. Sin esta historia, el relevamiento no tiene ciclo de vida y la revisión no puede separarse de la recolección.

## 3. Criterios de aceptación

- Given un relevamiento en recolección de la "Zona Norte", When el jefe lo pasa a revisión, Then el sistema deja el relevamiento en revisión y registra la transición.
- Given un relevamiento en recolección, When el jefe intenta pasarlo directamente a cierre, Then el sistema responde `TRANSICION_INVALIDA` y conserva el estado recolección.
- Given un relevamiento que pasa a cerrado, When un usuario intenta modificar su contenido, Then el sistema lo trata como solo lectura.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-10 |
| BT derivadas | BT-04, BT-07, BT-09, BT-18 |
| Tests previstos | acceptance/AT-10-transicion-estados |

## 5. Prioridad y estimación

Must: sin ciclo de vida no hay cierre ni revisión consolidada. 5 SP (Fibonacci): máquina de estados con validación de transiciones y bloqueo de solo lectura.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-10)
- [x] Reglas de negocio identificadas (RN-05, RN-01, RN-07)
- [x] Dependencia con US-06 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La reapertura explícita de un relevamiento cerrado se cubre en US-10. El aviso al pasar a revisión se cubre como Could en US-20.
