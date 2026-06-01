# US-08 — Reasignar agentes de un relevamiento

**Proyecto:** GeoVial
**Documento:** US-08-reasignar-agentes-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-02 Relevamientos y asignación
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero quitar y agregar agentes sobre un relevamiento ya existente, para reorganizar las cuadrillas sin perder lo que ya se recolectó.

## 2. Contexto

El alcance del BRIEF marca la reasignación de agentes como Should Have. CU-01 (flujo 5.A) la modela como actualización de asignaciones sobre un relevamiento en recolección o revisión, conservando las observaciones de los agentes removidos. Sin ella, un cambio de cuadrilla obligaría a recrear el relevamiento.

## 3. Criterios de aceptación

- Given un relevamiento en recolección con un agente asignado que ya recolectó observaciones, When el jefe lo remueve y agrega otro agente de su área, Then el sistema actualiza las asignaciones, conserva las observaciones existentes y audita el cambio.
- Given un relevamiento cerrado, When el jefe intenta reasignar agentes, Then el sistema responde `RELEVAMIENTO_SOLO_LECTURA` e indica que debe reabrirse.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-01 |
| BT derivadas | BT-03, BT-07, BT-09, BT-18 |
| Tests previstos | acceptance/AT-01-reasignar-agentes |

## 5. Prioridad y estimación

Should: el MVP funciona con la asignación inicial; la reasignación agrega flexibilidad operativa. 3 SP (Fibonacci): reutiliza la lógica de asignación de US-07 con conservación de datos.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-01)
- [x] Reglas de negocio identificadas (RN-01, RN-05, RN-07)
- [ ] Política de conservación de observaciones de agentes removidos a confirmar en refinamiento
- [x] Valor para el rol explícito

## 7. Notas y supuestos

Las observaciones recolectadas por un agente removido se conservan asociadas al relevamiento, no al agente.
