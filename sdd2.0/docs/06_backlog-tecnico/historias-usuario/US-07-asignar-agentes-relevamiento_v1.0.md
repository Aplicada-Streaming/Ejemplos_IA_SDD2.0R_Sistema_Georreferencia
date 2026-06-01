# US-07 — Asignar agentes al relevamiento

**Proyecto:** GeoVial
**Documento:** US-07-asignar-agentes-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-02 Relevamientos y asignación
**Prioridad MoSCoW:** Must
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero asignar agentes de campo de mi área a un relevamiento, para delegar su recolección en las cuadrillas correctas.

## 2. Contexto

NB-01 pone la delegación en el centro. CU-01 (pasos 4-5) habilita asignar agentes del área del jefe al relevamiento, validando la pertenencia de área (RN-01). Sin asignación, los agentes no ven el relevamiento (US-04) y la delegación no se concreta.

## 3. Criterios de aceptación

- Given un jefe del área "Zona Norte" con dos agentes de su área, When asigna ambos a un relevamiento de "Zona Norte", Then el sistema registra las dos asignaciones y queda auditado.
- Given un jefe del área "Zona Norte", When intenta asignar un agente del área "Zona Sur", Then el sistema rechaza con `AGENTE_FUERA_DE_AREA` y no registra esa asignación.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-01 |
| BT derivadas | BT-03, BT-07, BT-09, BT-18 |
| Tests previstos | acceptance/AT-01-asignar-agentes |

## 5. Prioridad y estimación

Must: la asignación es el acto que materializa la delegación. 3 SP (Fibonacci): validación de pertenencia de área sobre el relevamiento ya creado.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-01)
- [x] Reglas de negocio identificadas (RN-01, RN-07)
- [x] Dependencia con US-06 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La creación sin asignación inmediata (CU-01 5.B) es válida: un relevamiento puede crearse y asignarse después. La reasignación se cubre en US-08.
