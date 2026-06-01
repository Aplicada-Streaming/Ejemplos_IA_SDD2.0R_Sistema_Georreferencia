# Velocidad del equipo — GeoVial

**Proyecto:** GeoVial
**Documento:** velocidad-equipo_v1.0.md
**Versión:** 1.3
**Estado:** En curso
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

> Tracking actualizado al cierre del Sprint 03. La tabla §1 registra la velocity efectiva de los Sprint 00 y 01 ya ejecutados; S02 en adelante quedan por planificar. El promedio móvil de 3 sprints se puebla recién en S02 (con S00, S01, S02).

## 1. Por sprint

| Sprint | Comprometidos | Completados | Velocity | Promedio móvil 3 sprints | Notas |
| --- | --- | --- | --- | --- | --- |
| S00 | 29 | 21 | 21 | — | Walking skeleton (1 semana). BT-09, BT-01 y BT-18 cerrados; BT-07 parcial (4 tablas del slice + migración inicial), el resto carry-over al sprint de persistencia |
| S01 | 40 | 40 | 40 | — | Primer slice end-to-end (jerarquía y usuarios); 0 carry-over de lo comprometido; 60 pruebas verdes |
| S02 | 27 | 27 | 27 | 29,3 | Slice de relevamientos (CQRS ligero); 0 carry-over; 91 pruebas verdes; BT-07 avanza con Relevamiento/AsignacionAgente |
| S03 | 28 | 28 | 28 | 31,7 | Slice de captura y georreferenciación; 0 carry-over; 107 pruebas verdes; BT-07 avanza con Marcador/Observacion/Foto |
| S04 | por planificar | por registrar | por registrar | por registrar | — |

El promedio móvil de 3 sprints queda disponible en S02 (29,3 SP, sobre S00/S01/S02).

## 2. Tendencia

Cuatro sprints registrados (S00: 21, S01: 40, S02: 27, S03: 28). El promedio móvil de 3 sprints sube de 29,3 (S02) a 31,7 (S03, ventana S01–S03), ya sin el outlier inaugural S00. Los sprints estándar de 2 semanas se estabilizan en torno a 27–28 SP, con S01 (40) como pico inicial por incluir historias grandes (login + jerarquía). La tendencia se lee estable en el rango 27–28 para sprints de módulo acotado.

## 3. Capacidad ajustada

Con el promedio móvil de 3 sprints en 31,7 SP (S03), la capacidad sugerida para S04 es de hasta 35 SP (110 % del promedio móvil). El equipo ya calibra la capacidad declarada contra el promedio móvil en lugar de la estimación directa.

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
| 1.2 | 2026-06-01 | Registro de la velocity efectiva del Sprint 02: S02=27 (0 carry-over). Promedio móvil de 3 sprints = 29,3; capacidad sugerida para S03 = 32 SP. Por AG-07 |
| 1.3 | 2026-06-01 | Registro de la velocity efectiva del Sprint 03: S03=28 (0 carry-over). Promedio móvil de 3 sprints (S01–S03) = 31,7; capacidad sugerida para S04 = 35 SP. Por AG-07 |
