# Sprint Retrospectiva — Sprint 08

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-08_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Mantener la abstracción `IPipelineImagen` en `GeoVial.FileHosting` (igual que `IAlmacenFotos`) dejó el dominio y la aplicación libres de la librería de imágenes; cambiar de proveedor fue un cambio de una línea en infraestructura.
- El gate de auditoría de NuGet (con warnings tratados como error) atrapó la vulnerabilidad de ImageSharp 2.x en build, antes de que llegara a un PR; el cambio a SkiaSharp (MIT, sin vulnerabilidad) fue limpio y reforzó la postura de licencias permisivas del proyecto.
- Reusar la proyección de revisión existente para el filtrado por etiquetas (filtrar y descartar marcadores vacíos sobre el resultado ya armado) evitó duplicar la consulta y mantuvo el handler simple.
- Cerrar EP-05 entera en un sprint dejó la revisión sobre mapa como una unidad demostrable de punta a punta.

## 2. Qué no salió bien

- La librería de imágenes obligó a una iteración: la primera elección (ImageSharp) falló el gate de seguridad y hubo que rehacer la implementación con SkiaSharp. Conviene chequear advisories y licencia de una dependencia antes de codificar contra su API.
- `SKBitmap.Decode` no devuelve null ante un binario no-imagen sino un bitmap vacío; el primer pipeline lo trataba como imagen válida. Se endureció con una guarda de dimensiones, pero el comportamiento del decoder debía verificarse con una prueba desde el inicio.

## 3. Qué probar

- Antes de fijar una dependencia nueva, validar licencia y advisories (`dotnet list package --vulnerable`) en un spike corto, no durante la implementación.
- Para el cliente móvil (BT-19 móvil, sprints futuros), reusar el mismo `IPipelineImagen` antes de encolar la foto, de modo que la compresión ocurra en el origen y acote el payload de sincronización.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Agregar `dotnet list package --vulnerable` como paso del pipeline de PR | AG-09 | 2026-10-03 | Pendiente |
| Validar licencia + advisories de toda dependencia nueva en un spike previo a codificar | AG-05 / AG-08 | 2026-10-03 | Pendiente |
| Refinar EP-04 (sincronización) y US-30 (consulta de auditoría) para los próximos sprints | AG-06 | 2026-10-03 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 07 | Estado actual |
| --- | --- |
| Parametrizar la ruta del backend local por entorno | Pendiente (la configuración existe; falta documentarla por entorno) |
| Agregar la verificación contra S3 real (sample 03) al pipeline de entrega | Pendiente |
| Refinar BT-19 (pipeline de imágenes) y EP-04 (sincronización) | BT-19 completada en este sprint; EP-04 sigue en refinamiento |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 08 con 3 acciones nuevas y seguimiento de las del Sprint 07 (BT-19 completada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
