# US-29 — Registrar accesos y acciones administrativas en auditoría

**Proyecto:** GeoVial
**Documento:** US-29-registrar-accesos-acciones-auditoria_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-08 Auditoría y datos personales
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como usuario raíz, quiero que cada acceso autenticado y cada acción administrativa quede registrado de forma inalterable con su autor, momento y operación, para demostrar el correcto tratamiento de datos ante una auditoría.

## 2. Contexto

NB-06 exige trazabilidad de acciones bajo la Ley 25.326. CU-13 asienta cada evento de acceso y acción administrativa de forma inalterable (RN-07) y rechaza la acción si no puede registrarse. Es condición de operación legítima del sistema, no una capacidad visible al usuario final.

## 3. Criterios de aceptación

- Given un jefe general que da de alta un jefe de área, When se ejecuta el alta, Then el sistema asienta un registro con autor, momento y operación "alta de jefe de área".
- Given un registro de auditoría de hace tres meses, When un usuario intenta modificarlo, Then el sistema responde `AUDITORIA_INMUTABLE` y conserva el registro.
- Given una acción administrativa que no puede asentarse en auditoría, When un usuario la ejecuta, Then el sistema responde `ACCION_NO_AUDITADA` y rechaza la acción.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-06 |
| CU cubiertos | CU-13 |
| BT derivadas | BT-09, BT-12, BT-18 |
| Tests previstos | acceptance/AT-13-auditoria-inmutable |

## 5. Prioridad y estimación

Must: sin auditoría inalterable el sistema no cumple la Ley 25.326. 8 SP (Fibonacci): registro inmutable transversal con rechazo de acciones no auditables.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-13)
- [x] Reglas de negocio identificadas (RN-07, RN-08, RN-01)
- [x] Sin dependencias bloqueantes (infraestructura transversal temprana)
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La consulta del historial se cubre en US-30. La retención mínima es de doce meses; un período mayor es admisible. Esta US es infraestructura transversal que consumen las acciones administrativas de otras US.
