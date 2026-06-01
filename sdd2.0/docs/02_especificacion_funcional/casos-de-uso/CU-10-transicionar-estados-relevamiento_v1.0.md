# CU-10 — Transicionar el relevamiento entre recolección, revisión y cierre

**Proyecto:** GeoVial
**Documento:** CU-10-transicionar-estados-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que un jefe de área haga avanzar un relevamiento por sus estados —recolección, revisión, cierre— y lo reabra de forma explícita, para ordenar el trabajo entre la recolección de campo y la evaluación experta.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Cambia el estado del relevamiento y decide su reapertura |
| Sistema de relevamientos | Sistema | Valida las transiciones permitidas, aplica el bloqueo de solo lectura y registra el cambio en auditoría |

## 3. Precondiciones

- El relevamiento pertenece al área del jefe (RN-01).
- El relevamiento se encuentra en un estado desde el cual la transición solicitada es válida (RN-05).

## 4. Flujo principal

1. El jefe de área solicita transicionar el relevamiento al estado siguiente.
2. El sistema valida que la transición solicitada es una de las permitidas: recolección → revisión, revisión → cierre (RN-05).
3. El sistema aplica el nuevo estado.
4. Si el nuevo estado es cerrado, el sistema deja el relevamiento de solo lectura (RN-05).
5. El sistema registra la transición en el registro de auditoría (RN-07) y confirma el cambio.

## 5. Flujos alternativos

- 5.A Reapertura explícita. Disparador: el jefe de área decide reabrir un relevamiento cerrado. El sistema realiza la transición cerrado → recolección solo ante la acción explícita del jefe (RN-05), levanta el bloqueo de solo lectura y registra la reapertura en auditoría. Punto de retorno: paso 5.
- 5.B Aviso al pasar a revisión. Disparador: el relevamiento pasa de recolección a revisión. El sistema deja constancia del cambio para avisar al jefe de área. Punto de retorno: paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `TRANSICION_INVALIDA` | La transición solicitada no es una de las permitidas (RN-05) | Rechaza la transición y conserva el estado actual |
| `REAPERTURA_NO_AUTORIZADA` | Se intenta reabrir un relevamiento sin la acción explícita del jefe de área (RN-05) | Rechaza la reapertura y mantiene el relevamiento cerrado |
| `ACCESO_NO_AUTORIZADO` | El relevamiento no pertenece al área del jefe (RN-01) | Niega la operación y registra el intento |

## 7. Postcondiciones

- Éxito: el relevamiento queda en el estado destino; si es cerrado, queda de solo lectura; la transición queda auditada.
- Fallo: el relevamiento conserva su estado; el intento queda registrado cuando corresponde.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento en estado recolección de la "Zona Norte" | El jefe de "Zona Norte" lo pasa a revisión | El sistema deja el relevamiento en revisión y registra la transición |
| CA-02 | Un relevamiento en estado recolección | El jefe intenta pasarlo directamente a cierre | El sistema responde `TRANSICION_INVALIDA` y conserva el estado recolección |
| CA-03 | Un relevamiento cerrado | El jefe ejecuta la acción explícita de reapertura | El sistema lo pasa a recolección, levanta el solo lectura y registra la reapertura |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-01, RN-05, RN-07 |
| Historias de usuario a generar | US a generar en 06 (transición de estados, cierre con solo lectura, reapertura explícita) |
| Componentes esperados | Módulo de estados del relevamiento (referencia tentativa a 05) |
| Tests previstos | Suite de transición de estados y reapertura (referencia tentativa a 08) |

## 10. Notas y supuestos

- El aviso al jefe de área cuando un relevamiento pasa de recolección a revisión es un Could Have del alcance; se deja constancia del evento sin definir el canal de aviso, que se trata en 03.
- El cierre no impide exportar el relevamiento (CU-08), que opera sobre datos consolidados de solo lectura.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-04 |

## 13. Interacción multiusuario y concurrencia

La transición de estados es una acción del jefe de área. Si el jefe cierra un relevamiento mientras un agente intenta sincronizar cambios sobre él, el bloqueo de solo lectura (RN-05) rige sobre lo ya cerrado; los cambios del agente que no se consolidaron antes del cierre quedan pendientes y se tratan al reabrir el relevamiento. La concurrencia de dos transiciones sobre el mismo relevamiento se resuelve por última escritura (RN-04), con ambas registradas en auditoría.
