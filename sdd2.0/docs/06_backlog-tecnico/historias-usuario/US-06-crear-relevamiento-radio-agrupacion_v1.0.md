# US-06 — Crear relevamiento con radio de agrupación

**Proyecto:** GeoVial
**Documento:** US-06-crear-relevamiento-radio-agrupacion_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-02 Relevamientos y asignación
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero crear un relevamiento de mi área indicando la obra y el radio de agrupación, para organizar el trabajo de recolección sobre un punto de partida claro y reproducible.

## 2. Contexto

NB-01 (delegación) y NB-04 (revisión) necesitan una unidad de trabajo: el relevamiento. CU-01 describe su creación en estado recolección con el radio de agrupación (RN-02) que después gobierna la asociación de observaciones a marcadores. Sin esta historia no hay contenedor de trabajo que asignar ni recolectar.

## 3. Criterios de aceptación

- Given un jefe del área "Zona Norte", When crea un relevamiento "Puente Río 12" con radio de agrupación 15 metros, Then el sistema lo crea en estado recolección con radio 15 m y lo registra en auditoría.
- Given un usuario que no es jefe de área, When intenta crear un relevamiento, Then el sistema responde `ACCESO_NO_AUTORIZADO` y no crea nada.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-01 |
| BT derivadas | BT-03, BT-07, BT-09, BT-10, BT-18 |
| Tests previstos | acceptance/AT-01-crear-relevamiento |

## 5. Prioridad y estimación

Must: el relevamiento es la entidad central del proceso. 5 SP (Fibonacci): alta de entidad con validación de área y parámetro de radio, dentro del módulo CQRS de relevamientos.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-01)
- [x] Reglas de negocio identificadas (RN-01, RN-02, RN-05)
- [x] Sin dependencias bloqueantes más allá de la jerarquía (US-01, US-02)
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El radio por defecto se fija al crear el relevamiento y puede ajustarse después (relacionado con US-25). El identificador de obra es un dato del dominio.
