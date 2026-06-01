# Sprint Retrospectiva — Sprint 02

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-02_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- El CQRS ligero con un mediador mínimo (petición → manejador resuelto por inyección de dependencias) dio una frontera command/query clara sin agregar dependencias externas.
- Modelar el relevamiento como agregado con su colección de asignaciones simplificó las invariantes (estados, solo lectura, asignación por área) y se mapeó en EF sin fricción.
- La acción de la retro del Sprint 01 (definir casos de borde en la DoR) ayudó: la cobertura quedó sobre el gate desde la primera medición, sin tests de relleno tardíos.

## 2. Qué no salió bien

- La demo de alta desde el front quedó limitada porque no hay forma de proveer credenciales a un jefe de área (solo el raíz tiene credencial sembrada); se trabajó alrededor con tests, pero la demo end-to-end por UI quedó incompleta para ese rol.
- La verificación de integración del camino feliz de relevamientos por la API también depende de tener un jefe de área autenticable, hoy no disponible sin provisión de credenciales.

## 3. Qué probar

- Sumar una historia/tarea de provisión de credenciales (alta de credencial al dar de alta un usuario, o seed de prueba) para destrabar las demos por UI de los roles no raíz.
- Evaluar Testcontainers en una máquina del equipo para empezar a cubrir la capa de persistencia con integración real en el próximo sprint de captura.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Refinar una US de provisión de credenciales / seed de prueba y priorizarla en el backlog | AG-06 | 2026-07-04 | Pendiente |
| Validar la imagen de Testcontainers (SQL Server) en el entorno del equipo | AG-09 | 2026-07-04 | Pendiente |
| Mantener el patrón de mediador CQRS documentado para los módulos de dominio rico siguientes | AG-05 | 2026-07-04 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 01 | Estado actual |
| --- | --- |
| Corregir CU-03 CA-03 y US-02 CA1 | Completada |
| Agregar a la DoR los casos de borde con cobertura objetivo | En progreso (aplicado en S02; pendiente formalizar en la plantilla de DoR) |
| Planificar BT-07 completo con Testcontainers | En progreso (BT-07 avanzó con Relevamiento/AsignacionAgente; Testcontainers pendiente) |
| Agendar rotación de la clave inicial del usuario raíz | Pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 02 con 3 acciones nuevas y seguimiento de las 4 del Sprint 01. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
