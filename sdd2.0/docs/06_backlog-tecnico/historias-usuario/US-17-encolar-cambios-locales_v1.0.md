# US-17 — Encolar los cambios locales para sincronizar

**Proyecto:** GeoVial
**Documento:** US-17-encolar-cambios-locales_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-04 Sincronización offline
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero que cada cambio que hago sin conexión quede registrado en una cola local con su orden y su momento, para que la sincronización posterior pueda subirlo sin perder ni duplicar nada.

## 2. Contexto

NB-03 mitiga el riesgo de pérdida de observaciones antes de sincronizar (R-02). CU-06 (paso 4) registra cada cambio en la cola con su tipo de operación, entidad y marca temporal; RC-03 exige un identificador único por cambio para idempotencia. Sin la cola, la sincronización no tendría qué subir de forma ordenada y segura.

## 3. Criterios de aceptación

- Given un agente recolectando sin señal, When captura o edita una observación, marcador, foto, comentario o etiqueta, Then el sistema registra un cambio en la cola con su tipo de operación, entidad y marca temporal.
- Given una edición de una observación ya capturada localmente, When el agente la modifica, Then el sistema actualiza el registro local y encola la edición con su propia marca temporal.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-06 |
| BT derivadas | BT-14, BT-15 |
| Tests previstos | acceptance/AT-06-cola-cambios |

## 5. Prioridad y estimación

Must: la cola es la base de una sincronización sin pérdida. 5 SP (Fibonacci): esquema de cola local SQLite con identificador único y orden temporal.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-06)
- [x] Reglas conceptuales identificadas (RC-03 idempotencia)
- [x] Dependencia con US-16 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El identificador único de cada cambio (ChangeId) es la clave de idempotencia que evita aplicar un cambio dos veces ante reintentos; se materializa en la librería de sincronización.
