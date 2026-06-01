# CU-07 — Sincronizar cambios locales al recuperar conexión

**Proyecto:** GeoVial
**Documento:** CU-07-sincronizar-cambios-locales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que la app de campo, al recuperar conexión, suba primero los cambios locales recolectados sin señal y luego baje las últimas actualizaciones de los relevamientos asignados al agente, sin pérdida y resolviendo los choques por última escritura.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Dispara o confirma la sincronización al recuperar señal |
| Sistema de sincronización | Sistema | Sube los cambios encolados, baja las actualizaciones remotas, consolida choques y marca conflictos |

## 3. Precondiciones

- Existe una cola local con cambios pendientes de sincronizar (CU-06).
- El dispositivo recuperó conexión.
- El agente tiene una sesión válida y relevamientos asignados (RN-01).

## 4. Flujo principal

1. El sistema detecta la conexión recuperada y notifica que hay cambios pendientes de sincronizar.
2. El sistema sube primero los cambios locales encolados, en orden, con su marca temporal.
3. Por cada cambio, el sistema consolida contra el estado central: ante choque de campo prevalece la última escritura (RN-04) y el recurso queda marcado como conflicto.
4. El sistema detecta marcadores que quedan dentro de un mismo radio y los señala como conflicto (RN-02), sin unificarlos.
5. El sistema baja las últimas actualizaciones de los relevamientos asignados al agente.
6. El sistema vacía de la cola los cambios sincronizados con éxito y confirma el resultado de la sincronización.

## 5. Flujos alternativos

- 5.A Sincronización parcial por interrupción. Disparador: la conexión se corta a mitad de la subida. El sistema conserva en la cola los cambios no confirmados y reanuda la sincronización al recuperar señal, sin duplicar los ya subidos. Punto de retorno: paso 2.
- 5.B Sin cambios pendientes. Disparador: la cola está vacía al recuperar conexión. El sistema solo baja las actualizaciones remotas de los relevamientos asignados. Punto de retorno: paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `CONSOLIDACION_INVALIDA` | La consolidación no pudo aplicar la última escritura (RN-04) | Aborta el cambio en conflicto, lo conserva en la cola e informa el fallo |
| `CONFLICTO_NO_MARCADO` | Un recurso consolidado por última escritura no quedó marcado como conflicto (RN-04) | No considera el recurso sincronizado hasta crear la marca |
| `SINCRONIZACION_INTERRUMPIDA` | Se pierde la conexión durante la sincronización | Conserva los cambios no confirmados y reanuda sin duplicar |

## 7. Postcondiciones

- Éxito: los cambios locales quedan reflejados en el estado central, las actualizaciones remotas quedan bajadas, la cola queda vacía de lo sincronizado y los choques quedan marcados como conflicto.
- Fallo: los cambios no confirmados permanecen en la cola para reintentar; no hay pérdida ni duplicación.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una cola con 100 cambios locales y conexión recuperada | El sistema sincroniza | Sube los 100 cambios, baja las actualizaciones de los relevamientos asignados y vacía la cola sin pérdida |
| CA-02 | Dos ediciones del mismo comentario, una local más reciente que la central | El sistema consolida | Prevalece la edición de marca temporal más reciente y el recurso queda marcado como conflicto |
| CA-03 | Una subida interrumpida por corte de señal a la mitad | El sistema reanuda al recuperar conexión | Completa la subida sin duplicar los cambios ya confirmados |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-01, RN-02, RN-04 |
| Historias de usuario a generar | US a generar en 06 (subir cambios locales, bajar actualizaciones, marcado de conflictos) |
| Componentes esperados | Módulo de sincronización (referencia tentativa a 05) |
| Tests previstos | Suite de sincronización, idempotencia y marcado de conflictos (referencia tentativa a 08) |

## 10. Notas y supuestos

- La detección de conexión es automática y la sincronización se dispara sola al recuperar señal; el agente puede además confirmarla.
- El orden subir local antes de bajar remoto es una decisión del dominio declarada por el negocio.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-03 |

## 13. Interacción multiusuario y concurrencia

La sincronización es el punto donde convergen los cambios de varios agentes sobre el mismo relevamiento. Los choques de campo se resuelven por última escritura (RN-04) y los marcadores dentro de un mismo radio se señalan como conflicto (RN-02), quedando ambos casos a resolución del jefe de área desde la web (CU-11, CU-12). Cada cambio encolado tiene un identificador único que evita aplicarlo dos veces ante reintentos.
