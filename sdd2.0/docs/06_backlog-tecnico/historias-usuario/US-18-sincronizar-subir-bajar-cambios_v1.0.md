# US-18 — Sincronizar subiendo y bajando cambios

**Proyecto:** GeoVial
**Documento:** US-18-sincronizar-subir-bajar-cambios_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-04 Sincronización offline
**Prioridad MoSCoW:** Must
**Estimación:** 13 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero que al recuperar conexión la app suba primero mis cambios locales y después baje las últimas actualizaciones de mis relevamientos, para que mi trabajo de campo quede consolidado y yo tenga la información más reciente.

## 2. Contexto

NB-03 exige sincronización confiable. CU-07 sube los cambios encolados en orden, consolida cada uno contra el estado central aplicando última escritura (RN-04) y marcando conflictos, detecta marcadores en un mismo radio (RN-02) y luego baja las actualizaciones. El NFR fija ≤ 5 minutos para ≈100 observaciones. Es el corazón de la continuidad operativa.

## 3. Criterios de aceptación

- Given una cola con 100 cambios locales y conexión recuperada, When el sistema sincroniza, Then sube los 100 cambios, baja las actualizaciones de los relevamientos asignados y vacía la cola sin pérdida.
- Given dos ediciones del mismo comentario, una local más reciente que la central, When el sistema consolida, Then prevalece la edición de marca temporal más reciente y el recurso queda marcado como conflicto.
- Given una subida interrumpida por corte de señal a la mitad, When el sistema reanuda al recuperar conexión, Then completa la subida sin duplicar los cambios ya confirmados.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-07 |
| BT derivadas | BT-15, BT-16, BT-17, BT-09 |
| Tests previstos | acceptance/AT-07-sincronizacion |

## 5. Prioridad y estimación

Must: sin sincronización, la recolección offline no llega al backend. 13 SP (Fibonacci): pipeline subir/consolidar/marcar conflicto/bajar con idempotencia y reanudación; es la US más grande del backlog y candidata a descomponerse si excede un sprint.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-07)
- [x] Reglas de negocio identificadas (RN-04, RN-02, RN-01)
- [x] Dependencia con US-17 (cola) y la API REST de sync declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

Si en refinamiento supera un sprint, se descompone (por ejemplo, subir vs. bajar). El marcado de conflictos alimenta US-25 y US-26. La idempotencia se garantiza por el ChangeId (RC-03).
