# Velocidad del equipo — GeoVial

**Proyecto:** GeoVial
**Documento:** velocidad-equipo_v1.0.md
**Versión:** 1.7
**Estado:** En curso
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

> Tracking actualizado al cierre del Sprint 07. La tabla §1 registra la velocity efectiva de los Sprint 00 a 07 ya ejecutados. El promedio móvil de 3 sprints se puebla desde S02 (con S00, S01, S02).

## 1. Por sprint

| Sprint | Comprometidos | Completados | Velocity | Promedio móvil 3 sprints | Notas |
| --- | --- | --- | --- | --- | --- |
| S00 | 29 | 21 | 21 | — | Walking skeleton (1 semana). BT-09, BT-01 y BT-18 cerrados; BT-07 parcial (4 tablas del slice + migración inicial), el resto carry-over al sprint de persistencia |
| S01 | 40 | 40 | 40 | — | Primer slice end-to-end (jerarquía y usuarios); 0 carry-over de lo comprometido; 60 pruebas verdes |
| S02 | 27 | 27 | 27 | 29,3 | Slice de relevamientos (CQRS ligero); 0 carry-over; 91 pruebas verdes; BT-07 avanza con Relevamiento/AsignacionAgente |
| S03 | 28 | 28 | 28 | 31,7 | Slice de captura y georreferenciación; 0 carry-over; 107 pruebas verdes; BT-07 avanza con Marcador/Observacion/Foto |
| S04 | 24 | 24 | 24 | 26,3 | Revisión sobre mapa + comentarios/etiquetas + provisión de credenciales; 0 carry-over; 130 pruebas verdes; BT-07 completa las 12 entidades backend |
| S05 | 13 | 13 | 13 | 21,7 | Detección por radio + resolución de conflictos (EP-06); compromiso acotado (2 historias) con margen reservado por la fusión de marcadores; 0 carry-over; 159 pruebas verdes |
| S06 | 13 | 13 | 13 | 16,7 | Exportación/importación del relevamiento completo en ZIP (EP-07); compromiso acotado (2 historias); 0 carry-over; 176 pruebas verdes |
| S07 | 16 | 16 | 16 | 14,0 | Alojamiento de fotos con backends configurables (BT-20) + binarios en el ZIP (cierre BT-21); compromiso ampliado por la acción de retro; 0 carry-over; 192 pruebas verdes |

El promedio móvil de 3 sprints queda disponible en S02 (29,3 SP, sobre S00/S01/S02).

## 2. Tendencia

Ocho sprints registrados (S00: 21, S01: 40, S02: 27, S03: 28, S04: 24, S05: 13, S06: 13, S07: 16). El promedio móvil de 3 sprints baja de 16,7 (S06, ventana S04–S06) a 14,0 (S07, ventana S05–S07) al salir S04 (24) de la ventana, aunque S07 (16) ya marca el repunte buscado: es el mayor de los últimos tres y supera al promedio móvil previo. La serie reciente refleja la decisión de planificar por épica acotada (S05/S06) y la corrección de la retro (S07 amplía el compromiso). Descontados esos efectos de planificación, los sprints de módulo completo (S02: 27, S03: 28, S04: 24) siguen marcando el techo estable de 24–28 SP.

## 3. Capacidad ajustada

Con el promedio móvil de 3 sprints en 14,0 SP (S07), la capacidad sugerida estricta para S08 sería de hasta 15 SP (110 % del promedio móvil). Dado que la ventana está deprimida por los dos sprints acotados (S05/S06) y que S07 ya repuntó a 16, el equipo calibra S08 hacia el rango 16–24 SP según el backlog refinado, sin tratar el promedio móvil deprimido como techo real de capacidad.

## 4. Outliers explicados

| Sprint | Velocity | Promedio móvil | Desviación | Causa |
| --- | --- | --- | --- | --- |
| S00 | 21 | — | — | Sprint inaugural y corto (1 semana, walking skeleton); factor de focus conservador por arranque del equipo. Se trata como outlier hasta consolidar el promedio móvil |
| S05 | 13 | 26,3 (ventana previa) | −50 % | Compromiso deliberadamente acotado: EP-06 aportó solo 2 historias (US-25/US-26, 13 SP) y se reservó margen respecto del tope de 29 SP por la complejidad estructural de la fusión de marcadores. No refleja una caída de capacidad sino una decisión de alcance |
| S06 | 13 | 21,7 (ventana previa) | −40 % | Segundo compromiso acotado seguido: EP-07 aportó 2 historias (US-27/US-28, 13 SP). Decisión de alcance por tamaño de épica, no de capacidad; la acción de la retro es combinar épica acotada + ítem de backlog para llenar la capacidad |

A medida que se registren velocities, todo sprint cuyo valor se desvíe más del 30 % del promedio móvil se documenta en una fila aparte con su causa (vacaciones, incidente operativo, sprint inaugural, cambio de equipo, compromiso acotado por alcance).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Tracking de velocidad inicial con la estructura de §4.5 y los puntos comprometidos proyectados de S00 (29) y S01 (40). Velocity efectiva por registrar; promedio móvil de 3 sprints a poblar desde S03. Generado por AG-07 |
| 1.1 | 2026-06-01 | Registro de la velocity efectiva al cierre del Sprint 01: S00=21 (BT-07 parcial, resto carry-over) y S01=40 (0 carry-over). Tendencia y outlier de S00 actualizados. Por AG-07 |
| 1.2 | 2026-06-01 | Registro de la velocity efectiva del Sprint 02: S02=27 (0 carry-over). Promedio móvil de 3 sprints = 29,3; capacidad sugerida para S03 = 32 SP. Por AG-07 |
| 1.3 | 2026-06-01 | Registro de la velocity efectiva del Sprint 03: S03=28 (0 carry-over). Promedio móvil de 3 sprints (S01–S03) = 31,7; capacidad sugerida para S04 = 35 SP. Por AG-07 |
| 1.4 | 2026-06-01 | Registro de la velocity efectiva del Sprint 04: S04=24 (0 carry-over). Promedio móvil de 3 sprints (S02–S04) = 26,3; capacidad sugerida para S05 = 29 SP. Por AG-07 |
| 1.5 | 2026-06-01 | Registro de la velocity efectiva del Sprint 05: S05=13 (0 carry-over). Promedio móvil de 3 sprints (S03–S05) = 21,7; capacidad sugerida para S06 = 24 SP. S05 documentado como outlier por compromiso acotado de alcance (EP-06, 2 historias). Por AG-07 |
| 1.6 | 2026-06-01 | Registro de la velocity efectiva del Sprint 06: S06=13 (0 carry-over). Promedio móvil de 3 sprints (S04–S06) = 16,7; capacidad sugerida estricta para S07 = 18 SP, con recomendación de combinar épica acotada + ítem de backlog hacia el rango estable 24–28 SP. S06 documentado como outlier por compromiso acotado (EP-07, 2 historias). Por AG-07 |
| 1.7 | 2026-06-01 | Registro de la velocity efectiva del Sprint 07: S07=16 (0 carry-over). Promedio móvil de 3 sprints (S05–S07) = 14,0; capacidad sugerida hacia 16–24 SP para S08 (la ventana está deprimida por S05/S06; S07 ya repuntó a 16). Por AG-07 |
