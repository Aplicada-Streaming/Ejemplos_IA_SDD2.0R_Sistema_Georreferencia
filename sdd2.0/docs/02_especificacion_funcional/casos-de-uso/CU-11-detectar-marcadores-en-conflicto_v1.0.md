# CU-11 — Detectar y listar marcadores en conflicto por radio configurable

**Proyecto:** GeoVial
**Documento:** CU-11-detectar-marcadores-en-conflicto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Detectar y listar los marcadores de un relevamiento que quedan dentro de un mismo radio configurable, y los recursos consolidados por última escritura, para que el jefe de área los tenga identificados como conflictos a resolver.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Consulta la lista de conflictos del relevamiento de su área |
| Sistema de detección de conflictos | Sistema | Evalúa la cercanía por radio, identifica los conflictos por última escritura y los presenta como lista |

## 3. Precondiciones

- El relevamiento pertenece al área del jefe (RN-01).
- El relevamiento tiene un radio de agrupación configurado (RN-02).
- Se realizó al menos una sincronización que pudo generar conflictos (CU-07).

## 4. Flujo principal

1. El jefe de área solicita ver los conflictos del relevamiento.
2. El sistema evalúa las distancias entre marcadores y detecta los que quedan dentro del radio configurado (RN-02).
3. El sistema incorpora los recursos marcados como conflicto por última escritura durante la sincronización (RN-04).
4. El sistema presenta la lista de conflictos, distinguiendo marcadores en un mismo radio de ediciones en conflicto.
5. El jefe de área revisa la lista para decidir la resolución de cada conflicto (CU-12).

## 5. Flujos alternativos

- 5.A Cambio del radio configurado. Disparador: el jefe de área ajusta el radio de agrupación del relevamiento. El sistema reevalúa la cercanía con el nuevo radio y actualiza la lista de marcadores en conflicto (RN-02). Punto de retorno: paso 4.
- 5.B Sin conflictos. Disparador: no hay marcadores dentro del radio ni recursos marcados por última escritura. El sistema informa que no hay conflictos pendientes. Punto de retorno: fin del flujo.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `ACCESO_NO_AUTORIZADO` | El relevamiento no pertenece al área del jefe (RN-01) | Niega el acceso a la lista y registra el intento |
| `RADIO_INVALIDO` | El radio configurado no es un valor admisible | Rechaza el ajuste de radio y conserva el radio anterior |
| `UNIFICACION_NO_AUTORIZADA` | Un proceso intenta unificar o descartar marcadores en la detección sin decisión humana (RN-02) | Bloquea la unificación y solo lista los conflictos |

## 7. Postcondiciones

- Éxito: el relevamiento tiene una lista de conflictos identificados (marcadores en un mismo radio y ediciones en conflicto) disponible para resolución.
- Fallo: no se entrega la lista por falta de acceso; ningún marcador se unifica ni descarta automáticamente.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento con radio 15 m y dos marcadores a 9 m de distancia | El jefe consulta los conflictos | El sistema lista esos dos marcadores como conflicto en un mismo radio, sin unificarlos |
| CA-02 | Dos marcadores a 30 m con radio 15 m | El jefe consulta los conflictos | El sistema no los lista como conflicto |
| CA-03 | Un relevamiento con radio 15 m y dos marcadores a 20 m | El jefe amplía el radio a 25 m | El sistema reevalúa y ahora los lista como conflicto en un mismo radio |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-01, RN-02, RN-04 |
| Historias de usuario a generar | US a generar en 06 (detección por radio, lista de conflictos, ajuste de radio) |
| Componentes esperados | Módulo de detección de conflictos (referencia tentativa a 05) |
| Tests previstos | Suite de detección de conflictos por radio (referencia tentativa a 08) |

## 10. Notas y supuestos

- La detección señala, no resuelve; la resolución es CU-12. Esta separación honra el criterio del negocio de no unificar ni descartar de forma automática.
- El radio es un parámetro del relevamiento; el valor por defecto se fija al crear el relevamiento (CU-01).

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-05 |

## 13. Interacción multiusuario y concurrencia

La detección opera sobre datos consolidados tras la sincronización de varios agentes. Si llegan nuevas sincronizaciones mientras el jefe revisa la lista, el sistema reevalúa la cercanía e incorpora los nuevos conflictos en una próxima consulta, sin unificar nada de forma automática (RN-02).
