# Velocidad del equipo — GeoVial

**Proyecto:** GeoVial
**Documento:** velocidad-equipo_v1.0.md
**Versión:** 1.1
**Estado:** En curso
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

> Tracking actualizado al cierre del Sprint 01. La tabla §1 registra la velocity efectiva de los Sprint 00 y 01 ya ejecutados; S02 en adelante quedan por planificar. El promedio móvil de 3 sprints se puebla recién en S02 (con S00, S01, S02).

## 1. Por sprint

| Sprint | Comprometidos | Completados | Velocity | Promedio móvil 3 sprints | Notas |
| --- | --- | --- | --- | --- | --- |
| S00 | 29 | 21 | 21 | — | Walking skeleton (1 semana). BT-09, BT-01 y BT-18 cerrados; BT-07 parcial (4 tablas del slice + migración inicial), el resto carry-over al sprint de persistencia |
| S01 | 40 | 40 | 40 | — | Primer slice end-to-end (jerarquía y usuarios); 0 carry-over de lo comprometido; 60 pruebas verdes |
| S02 | por planificar | por registrar | por registrar | por registrar | Primer sprint con promedio móvil de 3 sprints disponible |

El promedio móvil de 3 sprints se puebla recién en S02, cuando existan tres velocities efectivas registradas (S00, S01, S02). Hasta entonces la columna queda en "—" por diseño de la métrica.

## 2. Tendencia

Dos puntos registrados (S00: 21, S01: 40). El salto se explica porque S00 fue un sprint inaugural y corto (1 semana, walking skeleton) con factor de focus deliberadamente conservador, mientras que S01 fue un sprint estándar de 2 semanas con el equipo ya rodado. No se infiere tendencia consolidada hasta tener el promedio móvil de 3 sprints; S00 se trata como outlier (ver §4).

## 3. Capacidad ajustada

No hay promedio móvil de 3 sprints para derivar la capacidad sugerida del próximo sprint mientras el proyecto no ejecute al menos tres sprints. Hasta S03 la capacidad de cada sprint se declara por estimación directa con factor de focus conservador (Sprint 00: 30 SP; Sprint 01: 42 SP). Regla por defecto a aplicar desde S03: no comprometer más del 110 % del promedio móvil de 3 sprints.

## 4. Outliers explicados

| Sprint | Velocity | Promedio móvil | Desviación | Causa |
| --- | --- | --- | --- | --- |
| S00 | 21 | — | — | Sprint inaugural y corto (1 semana, walking skeleton); factor de focus conservador por arranque del equipo. Se trata como outlier hasta consolidar el promedio móvil |

A medida que se registren velocities, todo sprint cuyo valor se desvíe más del 30 % del promedio móvil se documentará en una fila aparte con su causa (vacaciones, incidente operativo, sprint inaugural, cambio de equipo).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Tracking de velocidad inicial con la estructura de §4.5 y los puntos comprometidos proyectados de S00 (29) y S01 (40). Velocity efectiva por registrar; promedio móvil de 3 sprints a poblar desde S03. Generado por AG-07 |
| 1.1 | 2026-06-01 | Registro de la velocity efectiva al cierre del Sprint 01: S00=21 (BT-07 parcial, resto carry-over) y S01=40 (0 carry-over). Tendencia y outlier de S00 actualizados. Por AG-07 |
