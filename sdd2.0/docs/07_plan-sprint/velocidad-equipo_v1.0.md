# Velocidad del equipo — GeoVial

**Proyecto:** GeoVial
**Documento:** velocidad-equipo_v1.0.md
**Versión:** 2.1
**Estado:** En curso
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

> Tracking actualizado al cierre del Sprint 11. La tabla §1 registra la velocity efectiva de los Sprint 00 a 11 ya ejecutados. El promedio móvil de 3 sprints se puebla desde S02 (con S00, S01, S02).

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
| S08 | 11 | 11 | 11 | 13,3 | Pipeline de imágenes (BT-19) + cierre de EP-05 (filtrado US-23 + visor US-24); 0 carry-over; 205 pruebas verdes |
| S09 | 13 | 13 | 13 | 13,3 | Sincronización backend: consolidación last-write-wins + idempotencia + conflictos (US-18, EP-04); 0 carry-over; 216 pruebas verdes |
| S10 | 13 | 13 | 13 | 12,3 | Cierre de EP-08: consulta de auditoría con retención (US-30) + acceso a datos personales con finalidad (US-31 §5.B); 0 carry-over; 234 pruebas verdes |
| S11 | 13 | 13 | 13 | 13,0 | Spike móvil: librería GeoVial.Sync + cola SQLite (BT-15, US-17) + cáscara MAUI; 0 carry-over; 242 pruebas verdes |

El promedio móvil de 3 sprints queda disponible en S02 (29,3 SP, sobre S00/S01/S02).

## 2. Tendencia

Doce sprints registrados (S00: 21, S01: 40, S02: 27, S03: 28, S04: 24, S05: 13, S06: 13, S07: 16, S08: 11, S09: 13, S10: 13, S11: 13). El promedio móvil de 3 sprints se estabilizó en 12–13 SP y en S11 marca 13,0 (ventana S09–S11), con tres sprints consecutivos de 13 SP: una cadencia sostenida de alcance acotado por decisión de planificación (cierres de épica y arranque de la plataforma móvil). Descontados esos efectos, los sprints de módulo completo del arranque (S02: 27, S03: 28, S04: 24) siguen marcando el techo estable de 24–28 SP. El S11 incorporó el perfil móvil (MAUI/SQLite); a partir de aquí la velocity se compara sobre un equipo con frente backend + móvil.

## 3. Capacidad ajustada

Con el promedio móvil de 3 sprints en 13,0 SP (S11), la capacidad sugerida estricta para S12 sería de hasta 14 SP (110 % del promedio móvil). Con la plataforma móvil ya arrancada (`GeoVial.Sync` + cola SQLite), S12 puede comprometer la captura offline (US-16) y la sincronización automática por conectividad (US-19) sobre esa base, además de avanzar la UI de campo. La planificación se hará sobre el equipo con frente backend + móvil, sin tratar el promedio móvil —deprimido por la racha de alcance acotado— como techo real de capacidad.

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
| 1.8 | 2026-06-01 | Registro de la velocity efectiva del Sprint 08: S08=11 (0 carry-over). Promedio móvil de 3 sprints (S06–S08) = 13,3; capacidad sugerida hacia 13–24 SP para S09 según cuántas historias de EP-04 (sincronización) entren refinadas. Por AG-07 |
| 1.9 | 2026-06-01 | Registro de la velocity efectiva del Sprint 09: S09=13 (0 carry-over). Promedio móvil de 3 sprints (S07–S09) = 13,3; capacidad sugerida estricta para S10 = 15 SP. US-18 entregada en su alcance backend; el resto de EP-04 (cliente móvil) espera la plataforma MAUI. Por AG-07 |
| 2.0 | 2026-06-01 | Registro de la velocity efectiva del Sprint 10: S10=13 (0 carry-over). Promedio móvil de 3 sprints (S08–S10) = 12,3; capacidad sugerida estricta para S11 = 14 SP. Cierra EP-08 y el backend del MVP (EP-01 a EP-08 salvo cliente móvil de EP-04); el trabajo restante requiere incorporar la plataforma MAUI. Por AG-07 |
| 2.1 | 2026-06-01 | Registro de la velocity efectiva del Sprint 11: S11=13 (0 carry-over). Promedio móvil de 3 sprints (S09–S11) = 13,0; capacidad sugerida estricta para S12 = 14 SP. Spike de plataforma móvil: librería GeoVial.Sync + cola SQLite (BT-15, US-17); habilita US-16/US-19 para S12. Por AG-07 |
