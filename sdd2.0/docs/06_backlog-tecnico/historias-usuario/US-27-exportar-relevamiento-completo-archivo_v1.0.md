# US-27 — Exportar el relevamiento completo en un único archivo

**Proyecto:** GeoVial
**Documento:** US-27-exportar-relevamiento-completo-archivo_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-07 Exportación e importación
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero exportar un relevamiento completo en un único archivo, para resguardarlo o compartirlo con el área central de evaluación.

## 2. Contexto

NB-04 y el alcance Must del BRIEF exigen exportar un relevamiento completo (datos, comentarios, etiquetas y fotos) en un único archivo. CU-08 (flujo 5.A) produce el archivo respetando el acotamiento por rol y área (RN-08) y registra la exportación (RN-07). Es lo que permite entregar el relevamiento al área central que confecciona informes.

## 3. Criterios de aceptación

- Given un relevamiento de la "Zona Norte", When el jefe solicita exportarlo, Then el sistema entrega un único archivo con datos, comentarios, etiquetas y fotos, y registra la exportación.
- Given una exportación que no puede registrarse en auditoría, When el jefe la solicita, Then el sistema responde `ACCION_NO_AUDITADA` y no entrega el archivo.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-08 |
| BT derivadas | BT-07, BT-09, BT-18, BT-21 |
| Tests previstos | acceptance/AT-08-exportacion |

## 5. Prioridad y estimación

Must: la exportación es parte del alcance Must y cierra el flujo de entrega al área central. 5 SP (Fibonacci): empaquetado de manifiesto y binarios de fotos en un único archivo, auditado.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-08)
- [x] Reglas de negocio identificadas (RN-07, RN-08, RN-01)
- [x] Dependencia con US-21 (revisión) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El formato físico del archivo (manifiesto y binarios) se define en las BT y el contrato REST. El cierre del relevamiento no impide exportarlo (opera sobre datos consolidados). La importación se cubre en US-28.
