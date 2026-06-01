# Sprint Retrospectiva — Sprint 03

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-03_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Encapsular el cálculo de distancia en un objeto de valor Coordenada con pruebas de casos límite (dentro/fuera del radio) dejó la regla de agrupación (RN-02) clara y verificable, sin tocar la base.
- La prueba de integración del flujo de captura contra EF Core dio confianza en el mapeo de las tres entidades nuevas (marcador, observación, foto) sin necesidad de SQL Server real.
- El patrón de mediador CQRS del Sprint 02 se reusó sin fricción para el módulo de captura.

## 2. Qué no salió bien

- La cobertura de la capa de aplicación quedó justo sobre el gate (80,5 %): el crecimiento del módulo de captura agregó caminos que conviene seguir cubriendo para no rozar el umbral.
- La demo por UI sigue limitada por la falta de credenciales para agentes; es la tercera retro que arrastra esta deuda.

## 3. Qué probar

- Cerrar la deuda de provisión de credenciales en el próximo sprint para destrabar las demos por UI de una vez.
- Agregar una verificación de cobertura por módulo en el pipeline para detectar antes el acercamiento al gate.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Refinar e implementar la provisión de credenciales (alta de credencial / seed de prueba) en el próximo sprint | AG-06 / AG-05 | 2026-07-18 | Pendiente |
| Sumar un reporte de cobertura por módulo al pipeline CI | AG-09 | 2026-07-18 | Pendiente |
| Documentar la bandeja sin georreferenciar como vista del relevamiento para EP-05 | AG-03 / AG-02 | 2026-07-18 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 02 | Estado actual |
| --- | --- |
| Refinar una US de provisión de credenciales / seed de prueba | Pendiente (se eleva a acción prioritaria en S04) |
| Validar la imagen de Testcontainers (SQL Server) | Pendiente |
| Mantener el patrón de mediador CQRS documentado | Completada (reusado en el módulo de captura) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 03 con 3 acciones nuevas y seguimiento de las del Sprint 02. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
