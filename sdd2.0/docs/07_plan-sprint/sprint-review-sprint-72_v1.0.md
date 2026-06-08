# Sprint Review — Sprint 72

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-72_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-72_v1.0.md`:

> Que un jefe pueda obtener el resumen de actividad de un relevamiento: totales y productividad por agente, autorizado por área.

Veredicto: Cumplido.

Explicación corta: primer sprint de la épica **Reporting / analytics**. Se agregó `GET /api/v1/relevamientos/{id}/resumen`, que devuelve el `ResumenRelevamientoDto`: estado, marcadores (y cuántos en conflicto), observaciones (y cuántas en la bandeja sin georreferenciar), fotos, comentarios, y la **productividad por agente** (observaciones por agente, de mayor a menor). El agregado es un núcleo puro (`CalculadoraResumenRelevamiento`, gate); el handler autoriza por área (RN-01) y carga los datos con los repositorios existentes. Es el **backend del tablero**; la UI web es el próximo sprint de la épica.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| RPT-RESUMEN | Reporting | `/resumen` de "Puente Río 12": 2 marcadores, 2 observaciones, 2 fotos; productividad "Carlos → 2" | Vista de gestión |
| RPT-RESUMEN | Seguridad | Sin token → 401; relevamiento inaccesible → 404 (sin filtrar) | Autorización por área |

## 3. Feedback recibido

- El resumen da, de un vistazo, el estado del relevamiento y quién capturó cuánto: base del tablero de jefes.
- Reusar los repos y la autorización por área dejó el cambio chico y consistente con el resto.
- La productividad por agente es el primer indicador de gestión sobre los datos ya capturados.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **543** (485 unitarias + 58 de integración), **+7**: 4 unitarias de `CalculadoraResumenRelevamiento` + 3 de integración (`ResumenRelevamientoE2ETests`: agrega marcadores/observaciones/productividad, 401, 404). Cobertura DoD sin regresión (núcleo nuevo cubierto en `GeoVial.Application`). Verificado en vivo contra SQL Server real.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| RPT-RESUMEN | Historia | Aceptada (endpoint + agregado + productividad) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

**Épica Reporting / analytics — sprint 1/N.** Hecho: backend del resumen de relevamiento. Próximos: **UI web del tablero** (mostrar el resumen en el front), resumen **por área** (todos los relevamientos), mapa de calor de observaciones, exportes (CSV/PDF).

## 7. Decisiones tomadas

- El resumen es un reporte de **conteos** (no trae fotos ni comentarios), pensado para un panel; eficiente y suficiente para el primer indicador.
- El agregado se modeló como **núcleo puro** (`CalculadoraResumenRelevamiento`) para cubrirlo en el gate sin base; el handler sólo carga y delega.
- Se reusan los repos existentes (fotos/comentarios por marcador) en vez de agregar conteos en la base; si escala, se optimiza en un sprint posterior.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Review del Sprint 72 (reporting #1: resumen de actividad del relevamiento). Cumplido, velocity 5, 0 carry-over, 543 pruebas (+7). Arranca la épica de reporting. Generado por AG-07 |
