# US-20 — Avisar al jefe de área cuando el relevamiento pasa a revisión

**Proyecto:** GeoVial
**Documento:** US-20-avisar-jefe-area-paso-revision_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-04 Sincronización offline
**Prioridad MoSCoW:** Could
**Estimación:** 2 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero que el sistema me avise cuando un relevamiento pasa de recolección a revisión, para empezar a evaluarlo apenas la cuadrilla termina la recolección sin tener que estar consultando el estado.

## 2. Contexto

El BRIEF (§4 Could Have) plantea el aviso al jefe de área cuando un relevamiento pasa a revisión. CU-10 (flujo 5.B) deja constancia del cambio para avisar, sin fijar el canal. Es una mejora de oportunidad sobre la revisión, no un bloqueante del flujo.

## 3. Criterios de aceptación

- Given un relevamiento en recolección de la "Zona Norte", When pasa a revisión, Then el sistema deja constancia del cambio disponible para el jefe de área de "Zona Norte".
- Given un relevamiento que pasa a revisión, When el jefe consulta sus relevamientos, Then el cambio de estado a revisión queda visible como aviso.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-10 |
| BT derivadas | BT-04, BT-09 |
| Tests previstos | acceptance/AT-10-aviso-revision |

## 5. Prioridad y estimación

Could: el MVP funciona sin el aviso; el jefe puede consultar el estado a mano. 2 SP (Fibonacci): registro y exposición del evento de cambio de estado.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-10)
- [ ] Canal de aviso (en app, web, otro) pendiente de definir con 03
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El canal de aviso se define en 03; esta US deja constancia del evento. Comparte CU con US-09 (transición de estados) pero aporta el valor de notificación.
