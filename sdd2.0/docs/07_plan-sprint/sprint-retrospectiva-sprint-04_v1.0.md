# Sprint Retrospectiva — Sprint 04

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-04_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Resolver la provisión de credenciales destrabó las demos por UI que tres retros venían arrastrando; el ciclo completo ya es demostrable con un jefe de área real.
- Las uniones explícitas para el muchos a muchos de etiquetas (RC-04) se mapearon en EF y se consultaron sin sorpresas.
- Separar la provisión de credenciales en su propio servicio evitó tocar el constructor de la gestión de usuarios y mantuvo verdes los tests existentes.

## 2. Qué no salió bien

- La cobertura de la capa de aplicación volvió a caer por debajo del gate en la primera medición al sumar los handlers de revisión; hizo falta una segunda tanda de pruebas de borde.
- Una corrida de cobertura quedó inconsistente por un build incremental; se resolvió con un rebuild limpio, pero conviene fijarlo en el pipeline.

## 3. Qué probar

- Sumar al pipeline CI un paso de cobertura con `--no-incremental` (o limpieza previa) para evitar mediciones inconsistentes.
- Incorporar la práctica de escribir los tests de borde de cada handler junto con el handler, no al final del sprint.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Configurar la cobertura del pipeline con limpieza previa para evitar mediciones incrementales inconsistentes | AG-09 | 2026-08-01 | Pendiente |
| Adoptar la regla de escribir los tests de borde junto al handler en la DoR/DoD del equipo | AG-06 / AG-08 | 2026-08-01 | Pendiente |
| Refinar EP-06 (resolución de conflictos) y EP-07 (exportación/importación) para los próximos sprints | AG-06 | 2026-08-01 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 03 | Estado actual |
| --- | --- |
| Provisión de credenciales (alta de credencial / seed de prueba) | Completada (BT-23 entregada en este sprint) |
| Reporte de cobertura por módulo en el pipeline | Pendiente |
| Documentar la bandeja sin georreferenciar como vista del relevamiento | Completada (incluida en la revisión sobre mapa de US-21) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 04 con 3 acciones nuevas y seguimiento de las del Sprint 03 (provisión de credenciales y bandeja sin georreferenciar completadas). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
