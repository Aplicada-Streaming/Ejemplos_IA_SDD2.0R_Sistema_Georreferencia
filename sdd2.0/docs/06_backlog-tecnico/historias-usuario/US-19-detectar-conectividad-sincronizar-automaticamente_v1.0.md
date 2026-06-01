# US-19 — Detectar la conectividad y sincronizar automáticamente

**Proyecto:** GeoVial
**Documento:** US-19-detectar-conectividad-sincronizar-automaticamente_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-04 Sincronización offline
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero que la app detecte sola cuando recupero señal y dispare la sincronización, para no tener que acordarme de sincronizar manualmente al final de la jornada.

## 2. Contexto

NB-03 mitiga el riesgo de perder observaciones por no sincronizar (R-02) con sincronización automática al recuperar señal (NFR detección de conectividad automática). CU-07 (paso 1 y notas) detecta la conexión y dispara la sincronización, además de avisar que hay cambios pendientes. Sin esta historia, la sincronización dependería de la memoria del agente.

## 3. Criterios de aceptación

- Given un dispositivo con cambios pendientes en cola que recupera conexión, When la app detecta la señal, Then dispara automáticamente la sincronización y notifica que había cambios pendientes.
- Given un dispositivo sin cambios pendientes que recupera conexión, When la app detecta la señal, Then solo baja las actualizaciones de los relevamientos asignados sin subir nada.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-07 |
| BT derivadas | BT-15, BT-16, BT-13 |
| Tests previstos | acceptance/AT-07-deteccion-conectividad |

## 5. Prioridad y estimación

Must: la sincronización automática es lo que protege contra la pérdida de datos por olvido. 5 SP (Fibonacci): monitor de conectividad y disparo del pipeline de US-18.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-07)
- [x] Reglas de negocio identificadas (RN-04, RN-01)
- [x] Dependencia con US-18 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El agente puede además disparar la sincronización manualmente. El recordatorio de sincronizar por jornada acompaña esta detección.
