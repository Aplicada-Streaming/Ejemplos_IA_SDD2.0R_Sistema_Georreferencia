# Sprint Retrospectiva — Sprint 10

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-10_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Reconocer que US-29 (registro) y la autorización transversal de US-31 ya estaban entregadas evitó re-trabajo: el sprint se concentró en lo genuinamente pendiente (consulta de auditoría y acceso a datos personales con finalidad), cerrando EP-08 con un compromiso ajustado.
- La inmutabilidad del registro de auditoría ya era una propiedad del diseño (entidad sin mutadores); formalizarla con una prueba por reflexión la dejó verificada sin agregar mecanismo.
- Reutilizar `Autorizacion.PuedeAccederDatoPersonal` (Sprint 01) para el flujo §5.B mantuvo la decisión de autorización en un único lugar del dominio.
- El backend del MVP (EP-01 a EP-08 salvo el cliente móvil) queda funcionalmente completo y demostrable end-to-end.

## 2. Qué no salió bien

- El conjunto permitido de finalidades (relevamiento/evaluación) quedó como un enum cerrado en el dominio; si el organismo necesitara declarar nuevas finalidades, hoy requiere recompilar. Es suficiente para el alcance regulatorio actual, pero conviene anotarlo como punto de extensión futuro.
- La consulta de auditoría no pagina: para volúmenes grandes a doce meses podría devolver muchos registros. Se acotó por defecto a la retención y por filtros, pero falta paginación para un uso intensivo.

## 3. Qué probar

- Agregar paginación (tamaño de página + cursor o desplazamiento) a la consulta de auditoría antes de un uso intensivo en producción.
- Externalizar el catálogo de finalidades permitidas a configuración si el negocio requiere declararlas sin recompilar.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Agregar paginación a la consulta de auditoría | AG-08 | 2026-11-07 | Pendiente |
| Incorporar un proyecto móvil (MAUI/SQLite) para habilitar EP-04 y la librería de sync | AG-05 | 2026-11-07 | Pendiente |
| Externalizar el catálogo de finalidades a configuración (punto de extensión) | AG-02 | 2026-11-07 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 09 | Estado actual |
| --- | --- |
| Spike de proyecto móvil MAUI para validar `GeoVial.Sync` contra `/sync` | Pendiente (se reitera como prioridad para EP-04) |
| Diseñar la generalización de last-write-wins a otras entidades editables | Pendiente |
| Refinar US-30 (consulta de auditoría) | Completada (entregada en este sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 10 con 3 acciones nuevas y seguimiento de las del Sprint 09 (US-30 completada). Cierra EP-08. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
