# Sprint Retrospectiva — Sprint 68

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-68_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Se rompió la racha de acotados con valor real.** Tras nueve sprints de ≤5 SP, S68 vuelve al alcance pleno (8) con dos historias de hardening que cierran agujeros de despliegue concretos (config insegura, falta de health checks).
- **Núcleo testeable.** La validación de configuración quedó como función pura (`GuardrailsProduccion`), cubierta por el gate (+9 unitarias); el fail-fast en producción y los health checks se verifican por integración (+3).
- **Sin romper desarrollo.** El guardrail es estricto sólo en `Production`; Development y el gate siguen permisivos —verificado que el backend arranca y loguea bien—.

## 2. Qué no salió bien

- **El fail-fast por excepción es tosco.** Lanzar `InvalidOperationException` al arrancar funciona, pero el mensaje va al log sin formato amigable; una capa de validación de opciones (`IValidateOptions`) sería más idiomática. Aceptable para el alcance.
- **Readiness mínimo.** El chequeo de base es un `CanConnect`; no cubre dependencias externas (almacenamiento de fotos S3, etc.) ni profundidad (una query real). Suficiente para empezar; se amplía en próximos sprints de la épica.

## 3. Qué probar

- Despliegue de prueba en `Production` **sin** `ConnectionStrings__GeoVial` ni `Jwt__ClaveSecreta`: el backend no debe levantar (falla rápido con el detalle).
- Con ambas variables provistas: arranca y `/health/ready` responde Healthy.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Épica hardening #2: observabilidad (logging estructurado + correlación de request) | AG-08 | 2029-02-16 | Planificado |
| Épica hardening #3: handler global de errores + headers de seguridad + rate limiting | AG-08 | 2029-03-02 | Pendiente |
| Épica hardening #4: contenedor productivo + readiness ampliado (S3, migraciones) | AG-09 | 2029-03-16 | Pendiente |
| H-07 con renderer de Shell propio (deuda de S67) | AG-08 | — | Diferido |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 67 | Estado |
| --- | --- |
| Definir la próxima épica de alcance pleno | **Hecho**: épica "Hardening de producción", arrancada en S68 |
| H-07 con renderer de Shell propio | Diferido (no es el foco de la épica actual) |
| Prueba manual de accesibilidad con TalkBack (H-14) | Pendiente |
| Script de reset/arranque del entorno de desarrollo | Pendiente (reiterado) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Retro del Sprint 68 (hardening #1). Bien: vuelta al alcance pleno con valor, núcleo testeable, sin romper desarrollo. Mejoras: fail-fast más idiomático (`IValidateOptions`), readiness más profundo. Roadmap de la épica en §4. Generada por AG-07 |
