# CU-01 — Crear relevamiento y asignar agentes de campo del área

**Proyecto:** GeoVial
**Documento:** CU-01-crear-relevamiento-asignar-agentes_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que un jefe de área cree un relevamiento de su área y le asigne o reasigne agentes de campo, para organizar el trabajo de recolección y delegarlo en personal no experto.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Crea el relevamiento, fija el radio de agrupación y asigna o reasigna agentes |
| Agente de campo | Secundario | Queda habilitado a recolectar en el relevamiento asignado |
| Sistema de relevamientos | Sistema | Valida el área, persiste el relevamiento, registra las asignaciones y la acción en auditoría |

## 3. Precondiciones

- El jefe de área está autenticado y su rol y área están vigentes.
- Existen agentes de campo dados de alta en el área del jefe.
- El relevamiento, si ya existe, no está cerrado.

## 4. Flujo principal

1. El jefe de área solicita crear un relevamiento indicando su identificación, la obra (puente o camino) y el radio de agrupación.
2. El sistema valida que el jefe pertenece a un área habilitada (RN-01).
3. El sistema crea el relevamiento en estado recolección (RN-05) con el radio indicado (RN-02).
4. El jefe de área selecciona uno o más agentes de campo de su área y los asigna al relevamiento.
5. El sistema valida que cada agente pertenece al área del jefe (RN-01) y registra las asignaciones.
6. El sistema registra la creación y las asignaciones en el registro de auditoría (RN-07) y confirma la operación.

## 5. Flujos alternativos

- 5.A Reasignación de agentes. Disparador: el jefe de área quita o agrega agentes sobre un relevamiento ya existente en estado recolección o revisión. El sistema actualiza las asignaciones, conserva las observaciones ya recolectadas por agentes removidos y registra el cambio en auditoría. Punto de retorno: paso 6 del flujo principal.
- 5.B Creación sin asignación inmediata. Disparador: el jefe crea el relevamiento sin asignar agentes todavía. El sistema persiste el relevamiento sin asignaciones y queda disponible para asignar después. Punto de retorno: paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `ACCESO_NO_AUTORIZADO` | El usuario no es jefe de área o el área del recurso no es la suya (RN-01) | Rechaza la operación, no crea nada y registra el intento |
| `AGENTE_FUERA_DE_AREA` | Un agente seleccionado no pertenece al área del jefe (RN-01) | Rechaza la asignación de ese agente e informa el motivo |
| `RELEVAMIENTO_SOLO_LECTURA` | Se intenta asignar agentes a un relevamiento cerrado (RN-05) | Rechaza la operación e indica que el relevamiento debe reabrirse |

## 7. Postcondiciones

- Éxito: existe un relevamiento en estado recolección con su radio de agrupación y, si se asignaron, sus agentes; la acción queda auditada.
- Fallo: no se crea ni modifica ningún relevamiento ni asignación; el intento queda registrado cuando corresponde.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe del área "Zona Norte" con dos agentes de su área | Crea un relevamiento "Puente Río 12" con radio de agrupación 15 metros y asigna ambos agentes | El sistema crea el relevamiento en estado recolección con radio 15 m y registra dos asignaciones |
| CA-02 | Un jefe del área "Zona Norte" | Intenta asignar un agente del área "Zona Sur" | El sistema rechaza con el código `AGENTE_FUERA_DE_AREA` y no registra la asignación |
| CA-03 | Un relevamiento "Camino 8" cerrado | El jefe intenta asignarle un agente | El sistema rechaza con el código `RELEVAMIENTO_SOLO_LECTURA` |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01, RN-02, RN-05, RN-07 |
| Historias de usuario a generar | US a generar en 06 (alta de relevamiento, asignación y reasignación de agentes) |
| Componentes esperados | Módulo de relevamientos y asignación de agentes (referencia tentativa a 05) |
| Tests previstos | Suite de creación y asignación de relevamientos (referencia tentativa a 08) |

## 10. Notas y supuestos

- La reasignación (Should Have del alcance) se modela como flujo alternativo 5.A de este CU, no como CU aparte, porque comparte actor primario, validaciones y postcondiciones con la asignación inicial. Reconciliación respecto del mapeo de NB-01, que preveía CU-01 para "crear relevamiento y asignar agentes"; la reasignación se absorbe sin alterar ese mapeo.
- El identificador de la obra y la geometría detallada del área son datos del dominio; su captura visual se define en 03.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-01 |

## 13. Interacción multiusuario y concurrencia

Varios jefes de área operan en paralelo sobre relevamientos de áreas distintas sin interferencia, por el acotamiento de RN-01. Si dos sesiones del mismo jefe modifican las asignaciones del mismo relevamiento, prevalece la última escritura confirmada (coherente con RN-04) y la asignación resultante queda registrada en auditoría.
