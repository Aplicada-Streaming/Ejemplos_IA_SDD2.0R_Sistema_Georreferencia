# US-03 — Intervención del usuario raíz por incoherencia

**Proyecto:** GeoVial
**Documento:** US-03-intervencion-usuario-raiz-incoherencia_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-01 Jerarquía, usuarios y acceso
**Prioridad MoSCoW:** Should
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como usuario raíz, quiero intervenir para corregir una incoherencia de jerarquía o de área dentro del alcance de mi rol, para destrabar situaciones que ni el jefe general ni el jefe de área pueden resolver por sí mismos.

## 2. Contexto

El BRIEF describe al usuario raíz como quien configura el sistema y puede intervenir para resolver incoherencias. CU-03 (flujo 5.B) habilita esa corrección, y CU-12 (flujo 5.B) la extiende a conflictos que exceden al jefe de área. Sin esta historia, una incoherencia jerárquica deja al sistema bloqueado sin vía de resolución.

## 3. Criterios de aceptación

- Given un usuario raíz autenticado y una incoherencia de pertenencia de área, When corrige la asociación dentro del alcance del rol raíz, Then el sistema aplica la corrección y la registra en auditoría.
- Given un jefe de área, When intenta ejecutar una intervención reservada al rol raíz, Then el sistema rechaza con `ACCESO_NO_AUTORIZADO`.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-03 |
| BT derivadas | BT-01, BT-07, BT-09, BT-18 |
| Tests previstos | acceptance/AT-03-intervencion-raiz |

## 5. Prioridad y estimación

Should: el MVP opera con la jerarquía normal; la intervención raíz cubre el caso excepcional de incoherencia, valioso pero no bloqueante. 5 SP (Fibonacci): requiere un alcance de autorización ampliado y trazabilidad reforzada.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-03)
- [x] Reglas de negocio identificadas (RN-01, RN-07)
- [ ] Alcance exacto de "incoherencia" pendiente de acotar en refinamiento
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El alcance de la intervención raíz se acotará en refinamiento para no convertirse en un comodín de edición sin límites; toda intervención queda auditada (RN-07).
