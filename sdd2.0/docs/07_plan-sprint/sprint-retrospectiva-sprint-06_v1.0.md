# Sprint Retrospectiva — Sprint 06

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-06_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Aislar el formato físico (ZIP + JSON) tras el puerto `IEmpaquetadorRelevamiento` dejó los handlers de aplicación trabajando con el manifiesto y un `byte[]`, sin acoplarse a `System.IO.Compression`; el empaquetador se prueba por separado con un round-trip directo.
- Reconstruir el grafo con claves locales del manifiesto y un mapa explícito old→new evitó relaciones rotas; la prueba de round-trip exportar → importar verifica conteos, relaciones y estado preservado de punta a punta.
- Validar la coherencia del manifiesto antes de tocar la base cumplió la regla de "no alterar datos ante un archivo inválido" sin necesidad de transacciones explícitas.
- Reutilizar los métodos de repositorio existentes (listados por relevamiento/marcador/observación) para armar el manifiesto evitó ampliar los puertos: el sprint no agregó deuda de infraestructura.

## 2. Qué no salió bien

- El backend persiste solo la referencia de la foto, no el binario (ADR-08, librería de alojamiento aún no integrada); la exportación es "completa" salvo por los binarios. Se documentó explícitamente en el plan y el review, pero conviene cerrarlo pronto para que el archivo sea realmente autocontenido.
- Dos sprints seguidos de 13 SP (S05 y S06) por épicas acotadas de 2 historias bajaron el promedio móvil; la planificación por épica chica deja capacidad sin comprometer que podría haberse llenado con ítems del backlog (US-23 filtrado por etiquetas).

## 3. Qué probar

- Al integrar la librería de alojamiento (ADR-08), incluir los binarios de las fotos en el ZIP (carpeta `fotos/`) y extender el round-trip para verificarlos.
- En la planificación, combinar una épica acotada con un ítem complementario del backlog (p. ej. US-23) para acercar el compromiso al rango estable de 24–28 SP cuando la capacidad lo permita.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Integrar la librería de alojamiento (ADR-08) e incluir los binarios de fotos en el ZIP | AG-08 | 2026-09-05 | Pendiente |
| Combinar épica acotada + ítem de backlog en la próxima planificación para aprovechar la capacidad | AG-06 / AG-07 | 2026-09-05 | Pendiente |
| Configurar la cobertura del pipeline con limpieza previa (arrastre de S04/S05) | AG-09 | 2026-09-05 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 05 | Estado actual |
| --- | --- |
| Documentar la convención tracked vs `AsNoTracking` en el README de persistencia | Pendiente |
| Configurar la cobertura del pipeline con limpieza previa | Pendiente (se reitera) |
| Refinar EP-07 (exportación/importación) y EP-04 (sincronización) | EP-07 completada (US-27/US-28 entregadas); EP-04 sigue en refinamiento |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 06 con 3 acciones nuevas y seguimiento de las del Sprint 05 (EP-07 completada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
