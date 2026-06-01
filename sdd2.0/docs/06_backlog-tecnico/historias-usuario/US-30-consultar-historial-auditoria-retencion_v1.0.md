# US-30 — Consultar el historial de auditoría con retención

**Proyecto:** GeoVial
**Documento:** US-30-consultar-historial-auditoria-retencion_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-08 Auditoría y datos personales
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como usuario raíz, quiero consultar el registro de accesos y acciones filtrando por usuario, recurso o rango de fechas, para reconstruir el historial que pide una auditoría dentro del período de retención.

## 2. Contexto

NB-06 exige conservar el registro por el período legal (≥ 12 meses, RN-07) y poder reconstruir el historial de accesos a un dato personal (RN-08). CU-13 (pasos 4-5 y flujo 5.A) devuelve los eventos que cumplen el filtro. Sin la consulta, el registro inmutable no serviría para responder a una auditoría.

## 3. Criterios de aceptación

- Given una auditoría que pide los accesos a un dato del último año, When el usuario raíz consulta el registro por ese dato, Then el sistema devuelve todos los accesos registrados dentro de los doce meses.
- Given un usuario sin rol raíz, When intenta consultar el registro completo, Then el sistema responde `ACCESO_NO_AUTORIZADO` y registra el intento.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-06 |
| CU cubiertos | CU-13 |
| BT derivadas | BT-09, BT-12, BT-18 |
| Tests previstos | acceptance/AT-13-consulta-historial |

## 5. Prioridad y estimación

Must: la consulta es lo que hace operativa la trazabilidad legal. 5 SP (Fibonacci): consulta filtrada sobre el registro inmutable con autorización restringida al rol raíz.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-13)
- [x] Reglas de negocio identificadas (RN-07, RN-08, RN-01)
- [x] Dependencia con US-29 (registro) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La consulta del registro completo queda restringida al rol raíz. El período de retención mínimo es de doce meses.
