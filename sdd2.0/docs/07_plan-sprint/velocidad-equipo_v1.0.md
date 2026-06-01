# Velocidad del equipo — GeoVial

**Proyecto:** GeoVial
**Documento:** velocidad-equipo_v1.0.md
**Versión:** 1.0
**Estado:** En curso
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

> El proyecto aún no ejecutó ningún sprint a la fecha de este documento. La tabla §1 registra los puntos comprometidos proyectados de los Sprint 00 y 01 según sus planes de iteración; los puntos completados, la velocity efectiva y el promedio móvil quedan declarados como "por registrar" y se completan al cierre de cada sprint con su sprint review. No se inventa velocity efectiva de sprints no ejecutados.

## 1. Por sprint

| Sprint | Comprometidos | Completados | Velocity | Promedio móvil 3 sprints | Notas |
| --- | --- | --- | --- | --- | --- |
| S00 | 29 | por registrar | por registrar | — | Sprint inaugural (walking skeleton, 1 semana); factor de focus conservador |
| S01 | 40 | por registrar | por registrar | — | Primer slice end-to-end (jerarquía y usuarios); aún sin histórico consolidado |
| S02 | por planificar | por registrar | por registrar | — | — |
| S03 | por planificar | por registrar | por registrar | por registrar | Primer sprint con promedio móvil de 3 sprints disponible |

El promedio móvil de 3 sprints se puebla recién a partir de S03, cuando existen tres velocities efectivas registradas (S00, S01, S02). Hasta entonces la columna queda en "—" por diseño de la métrica.

## 2. Tendencia

Sin tendencia evaluable todavía: no hay velocity efectiva registrada de ningún sprint. La lectura cualitativa (estable, ascendente, descendente, errática) se completará a partir de S03, cuando el promedio móvil de 3 sprints permita comparar.

## 3. Capacidad ajustada

No hay promedio móvil de 3 sprints para derivar la capacidad sugerida del próximo sprint mientras el proyecto no ejecute al menos tres sprints. Hasta S03 la capacidad de cada sprint se declara por estimación directa con factor de focus conservador (Sprint 00: 30 SP; Sprint 01: 42 SP). Regla por defecto a aplicar desde S03: no comprometer más del 110 % del promedio móvil de 3 sprints.

## 4. Outliers explicados

| Sprint | Velocity | Promedio móvil | Desviación | Causa |
| --- | --- | --- | --- | --- |
| S00 | por registrar | — | — | Sprint inaugural y corto (1 semana, walking skeleton); factor de focus subestimado por arranque del equipo. Se prevé como outlier y se confirmará al cierre |

A medida que se registren velocities, todo sprint cuyo valor se desvíe más del 30 % del promedio móvil se documentará en una fila aparte con su causa (vacaciones, incidente operativo, sprint inaugural, cambio de equipo).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Tracking de velocidad inicial con la estructura de §4.5 y los puntos comprometidos proyectados de S00 (29) y S01 (40). Velocity efectiva por registrar; promedio móvil de 3 sprints a poblar desde S03. Generado por AG-07 |
