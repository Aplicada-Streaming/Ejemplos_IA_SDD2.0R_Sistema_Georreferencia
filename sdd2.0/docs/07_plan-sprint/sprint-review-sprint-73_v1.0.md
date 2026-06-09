# Sprint Review — Sprint 73

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-73_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-09
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-73_v1.0.md`:

> Que un jefe vea el resumen de actividad de un relevamiento en el front web: totales y productividad por agente.

Veredicto: Cumplido.

Explicación corta: segundo sprint de la épica **Reporting / analytics**: la **UI web del tablero**. La nueva página `Tablero.razor` (`/tablero`, Blazor Server) tiene login gate, lista los relevamientos visibles en un selector y, al elegir uno, llama a `GET /relevamientos/{id}/resumen` (vía `GeoVialApiCliente.ObtenerResumenAsync`, S72) y muestra los **totales** (marcadores, conflictos, observaciones, bandeja, fotos, comentarios) y la **productividad por agente**. Es UI sobre el endpoint ya cubierto por el gate; sin tocar backend ni dominio.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| RPT-TABLERO | Reporting/UI | `/tablero`: login → elegir relevamiento → tabla de totales + productividad por agente | Tablero de jefes funcional |
| RPT-TABLERO | Navegación | Link al tablero desde la home | Descubrible |

## 3. Feedback recibido

- El tablero cierra la historia del resumen: el dato de S72 ahora se ve sin curl, en el front administrativo.
- Reusar el `GeoVialApiCliente` (login + token + llamadas) dejó la página chica y consistente con el resto del front.
- El selector de relevamiento + tablas es suficiente para el MVP; gráficos y mapa de calor vienen después.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **543** (485 unitarias + 58 de integración), **sin nuevas**: es UI web sin núcleo nuevo; el endpoint que consume ya está cubierto por el gate (S72). El gate no se ve afectado (cambios sólo en `GeoVial.Web`). La web **compila** (`net10.0`) y **sirve** `/tablero` (verificado: HTTP 200 con la página renderizada). La interacción completa (login → elegir → resumen) usa el circuito Blazor (no automatizable por HTTP simple); se verifica de forma manual.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| RPT-TABLERO | Historia | Aceptada (página del tablero + cliente + link) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

**Épica Reporting / analytics — sprint 2/N.** Hecho: backend del resumen (S72) + UI del tablero (S73). Próximos: resumen **por área** (todos los relevamientos del área, no uno solo) + **exportes** (CSV/PDF); **mapa de calor** de observaciones; gráficos.

## 7. Decisiones tomadas

- La página reusa el `GeoVialApiCliente` (sesión/token en el circuito Blazor) y el patrón de las páginas existentes (login gate + render), en vez de un layout nuevo.
- El tablero muestra **un relevamiento a la vez** (elegido en un selector); el resumen por área es el siguiente sprint.
- Verificación: build + que la ruta sirva; la interacción completa queda como prueba manual (limitación conocida de la UI Blazor en el gate).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-09 | Review del Sprint 73 (reporting #2: UI web del tablero). Cumplido, velocity 5, 0 carry-over, 543 pruebas (sin nuevas; UI web). La web compila y sirve `/tablero`. Generado por AG-07 |
