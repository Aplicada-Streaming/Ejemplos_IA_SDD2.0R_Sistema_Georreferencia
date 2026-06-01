# US-31 — Autorizar cada acceso por rol y área

**Proyecto:** GeoVial
**Documento:** US-31-autorizar-acceso-rol-area_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-08 Auditoría y datos personales
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como usuario raíz responsable del cumplimiento, quiero que cada acceso a un recurso se autorice según el rol y el área del usuario y que todo acceso fuera de alcance se bloquee y registre, para garantizar que cada persona solo ve y opera lo que le corresponde.

## 2. Contexto

NB-06 exige proteger los datos personales bajo la Ley 25.326. CU-14 es el control de acceso transversal: evalúa rol y área antes de cada operación (RN-01), bloquea el acceso fuera de alcance y a datos personales no habilitados (RN-08) y centraliza el manejo uniforme de los errores de autorización. Es la base sobre la que se apoyan la delegación acotada y el compliance.

## 3. Criterios de aceptación

- Given un jefe de la "Zona Norte", When intenta abrir un relevamiento de la "Zona Sur", Then el sistema responde `ACCESO_NO_AUTORIZADO` y registra el intento.
- Given un agente de campo, When intenta consultar datos personales de otro agente, Then el sistema responde `ACCESO_DATO_PERSONAL_NO_AUTORIZADO` y registra el intento.
- Given un jefe de la "Zona Norte", When accede a un relevamiento de la "Zona Norte", Then el sistema concede el acceso.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-06 |
| CU cubiertos | CU-14 |
| BT derivadas | BT-08, BT-09, BT-18 |
| Tests previstos | acceptance/AT-14-autorizacion-rol-area |

## 5. Prioridad y estimación

Must: la autorización por rol y área es transversal a todos los CU y condición del compliance. 8 SP (Fibonacci): filtro de autorización previo a los handlers, manejo uniforme de errores y registro de accesos fuera de alcance.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-14)
- [x] Reglas de negocio identificadas (RN-01, RN-08)
- [x] Sin dependencias bloqueantes (infraestructura transversal temprana)
- [x] Valor para el rol explícito

## 7. Notas y supuestos

Este control lo invocan todos los demás CU; es infraestructura transversal de seguridad. El manejo uniforme de errores evita filtrar información del recurso protegido. El mecanismo concreto vive en las BT (ADR-03, ADR-14).
