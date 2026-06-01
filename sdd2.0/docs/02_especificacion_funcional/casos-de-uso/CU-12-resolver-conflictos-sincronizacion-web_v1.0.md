# CU-12 — Resolver conflictos de sincronización desde la web

**Proyecto:** GeoVial
**Documento:** CU-12-resolver-conflictos-sincronizacion-web_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que un jefe de área resuelva desde la web los conflictos de un relevamiento —unificar o mantener separados marcadores en un mismo radio y dirimir ediciones en conflicto—, dejando la base consistente.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Decide la resolución de cada conflicto del relevamiento de su área |
| Sistema de resolución de conflictos | Sistema | Aplica la decisión, levanta la marca de conflicto y registra la resolución en auditoría |

## 3. Precondiciones

- Existe una lista de conflictos identificados para el relevamiento (CU-11).
- El relevamiento pertenece al área del jefe (RN-01) y no está cerrado (RN-05).

## 4. Flujo principal

1. El jefe de área abre un conflicto de la lista.
2. Para marcadores en un mismo radio, el jefe decide unificarlos en uno o mantenerlos separados (RN-02).
3. Para ediciones en conflicto, el jefe confirma la versión que prevalece o elige la alternativa registrada (RN-04).
4. El sistema aplica la decisión, levanta la marca de conflicto del recurso y deja la base consistente.
5. El sistema registra la resolución en el registro de auditoría (RN-07) y confirma.

## 5. Flujos alternativos

- 5.A Unificación de marcadores. Disparador: el jefe decide unificar dos marcadores en un mismo radio. El sistema fusiona sus observaciones en un único marcador conforme a la decisión humana (RN-02) y levanta el conflicto. Punto de retorno: paso 5.
- 5.B Intervención del usuario raíz. Disparador: el conflicto excede al jefe de área y el usuario raíz interviene para resolver la incoherencia. El sistema aplica la resolución dentro del alcance del rol raíz y la registra. Punto de retorno: paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `RELEVAMIENTO_SOLO_LECTURA` | Se intenta resolver un conflicto de un relevamiento cerrado (RN-05) | Rechaza la resolución e indica que el relevamiento debe reabrirse |
| `CONFLICTO_INEXISTENTE` | El conflicto ya fue resuelto o no existe | Informa que no hay conflicto pendiente para ese recurso |
| `ACCESO_NO_AUTORIZADO` | El relevamiento no pertenece al área del jefe (RN-01) | Niega la operación y registra el intento |

## 7. Postcondiciones

- Éxito: el conflicto queda resuelto según la decisión humana, la marca de conflicto se levanta y la resolución queda auditada.
- Fallo: el conflicto permanece pendiente; no se altera la base; el intento queda registrado cuando corresponde.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Dos marcadores en un mismo radio listados como conflicto | El jefe decide unificarlos en uno | El sistema fusiona sus observaciones en un único marcador y levanta el conflicto |
| CA-02 | Dos marcadores en un mismo radio listados como conflicto | El jefe decide mantenerlos separados | El sistema conserva ambos marcadores y levanta la marca de conflicto |
| CA-03 | Un conflicto pendiente en un relevamiento cerrado | El jefe intenta resolverlo | El sistema responde `RELEVAMIENTO_SOLO_LECTURA` |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-05 |
| Reglas de negocio aplicables | RN-01, RN-02, RN-04, RN-05, RN-07 |
| Historias de usuario a generar | US a generar en 06 (unificar o separar marcadores, dirimir ediciones, intervención raíz) |
| Componentes esperados | Módulo de resolución de conflictos (referencia tentativa a 05) |
| Tests previstos | Suite de resolución de conflictos desde la web (referencia tentativa a 08) |

## 10. Notas y supuestos

- La resolución es siempre una decisión humana; el sistema no unifica ni descarta marcadores de forma automática (RN-02).
- La unificación reasigna las observaciones de los marcadores fusionados al marcador resultante, conservando fotos, comentarios y etiquetas.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-05 |

## 13. Interacción multiusuario y concurrencia

La resolución se realiza desde la web sobre datos consolidados. Si dos usuarios autorizados intentan resolver el mismo conflicto de forma concurrente, prevalece la primera resolución aplicada y la segunda recibe `CONFLICTO_INEXISTENTE`; ambas quedan registradas en auditoría para reconstruir la secuencia de decisiones.
