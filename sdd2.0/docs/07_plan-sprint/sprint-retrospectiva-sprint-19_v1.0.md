# Sprint Retrospectiva — Sprint 19

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-19_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- El cliente de edición (`ClienteEdicionMarcador`) quedó pequeño y testeable al 100 % de branches: validación local + POST, con un resultado uniforme; reusa los contratos compartidos.
- La validación local de texto/etiqueta vacíos evita viajes innecesarios al backend y dio pruebas claras (la llamada no se hace).
- El patrón núcleo testeable (`GeoVial.Revision`) + pantalla MAUI se aplicó por quinta vez (S14–S19) sin fricción; agregar la edición a la pantalla de revisión existente fue directo.
- Reusar los endpoints del Sprint 04 mantuvo el sprint enteramente en el frente cliente.

## 2. Qué no salió bien

- Tras una edición, la pantalla recarga toda la revisión y vuelve al primer marcador: se pierde la posición en el carrusel; falta una actualización incremental o conservar el índice.
- No hay una prueba de integración HTTP del ciclo completo (revisar → comentar → ver el comentario): la verificación es a nivel del cliente.
- El incidente de git de este sprint (un PR se cerró por accidente al borrar su rama antes de mergear, por un comando de housekeeping encadenado con un merge en paralelo) costó una recuperación; conviene no encadenar checkout/branch-delete con merges en background paralelos.

## 3. Qué probar

- Conservar el índice del marcador (o actualizar incrementalmente) al recargar la revisión tras una edición.
- Una prueba de integración que recorra revisar → agregar comentario → verlo en la revisión por HTTP.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Conservar la posición del carrusel al recargar tras editar | AG-08 (móvil) | 2027-03-20 | Pendiente |
| Provisionar clave de mapas y mapa interactivo (deuda de US-13/US-21) | AG-08 (móvil) | 2027-03-20 | Pendiente |
| Convención de housekeeping de ramas: nunca encadenar branch-delete con un merge en paralelo | AG-07 / AG-09 | 2027-03-20 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 18 | Estado actual |
| --- | --- |
| Edición sobre el marcador desde el móvil (comentario/etiqueta, US-15 cliente) | Completada (entregada en este sprint) |
| Prueba de integración del ciclo de edición en conflicto (sync → listar → confirmar) | Pendiente |
| Selector de relevamientos del área en la pantalla web de conflictos | Pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 19 con 3 acciones nuevas y seguimiento de las del Sprint 18 (US-15 cliente completada). Se registra el incidente de git (PR cerrado por accidente, recuperado) y su acción preventiva. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
