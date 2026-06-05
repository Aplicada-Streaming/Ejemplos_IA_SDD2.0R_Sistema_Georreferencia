# Sprint Retrospectiva — Sprint 50

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-50_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se atacó un gap visible de la revisión funcional (la bandeja sin georreferenciar era invisible en la app) entregando primero la mitad de **lectura**, con un alcance bien acotado y honesto.
- El enriquecimiento del DTO fue **aditivo** (campo `Bandeja` opcional + conservar `ObservacionesSinGeorreferenciar`): cero rupturas de consumidores o tests previos.
- Buen reparto gate/glue: el handler enriquecido y el `PresentadorBandeja` viven en el gate (Revision a 97,2 % líneas), y la solapa es glue delgada.
- Separar "ver" de "ubicar en el mapa" evitó meter en un solo sprint la interacción WebView bidireccional (tap → coordenada), que es la parte realmente compleja.

## 2. Qué no salió bien

- El `ViewCell` (obsoleto) rompió el build MAUI por warnings-as-errors; se detectó recién al compilar el móvil, no antes. Recordatorio: los controles de lista en MAUI evolucionan; usar `CollectionView` con plantillas que devuelven la vista directamente.
- La feature es **media**: el agente ve la bandeja pero todavía no puede ubicar desde la app. Es deliberado, pero hasta que llegue S51 el valor está incompleto.
- No se verificó **on-device**: la solapa se confió al build + al núcleo cubierto. La verificación visual quedó pendiente.

## 3. Qué probar

- S51 (planificado): ubicar una observación de la bandeja desde la app — tap en el mapa Leaflet → coordenada → `POST /observaciones/{id}/ubicacion` (CU-05). Requiere un puente WebView→C# (JS `postMessage`/`invokeCSharpAction`), que conviene encapsular en un núcleo testeable (parseo del mensaje del mapa).
- Verificar on-device la solapa Bandeja (con una captura sin GPS sembrada) y que el conteo y las etiquetas se ven bien.
- Evaluar mostrar una miniatura de la foto en cada fila (hoy sólo el nombre del archivo).

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| S51: ubicar desde la app (tap en mapa → CU-05) con núcleo testeable del puente WebView | AG-08 (móvil) + AG-09 (fullstack) | 2028-05-26 | Pendiente |
| Verificar on-device la solapa Bandeja | AG-05 (QA) | 2028-05-26 | Pendiente |
| Sprint de limpieza: unificar el listado de área al filtro en base (se arrastra de S47-S49) | AG-06 (backend) | 2028-05-26 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 49 | Estado actual |
| --- | --- |
| Verificar on-device el refresco en vivo + el aviso de conflictos | Pendiente (se reitera) |
| Sprint de limpieza: unificar el listado de área al filtro en base | Pendiente (se reitera de S47) |
| Evaluar bandeja sin georreferenciar navegable | Completada en su parte de lectura (entregada en este sprint); la ubicación desde la app pasa a S51 |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 50 (bandeja sin georreferenciar — lectura): enriquecimiento aditivo + presenter en el gate + solapa; se separó "ver" de "ubicar" (S51). Defecto del build MAUI por `ViewCell` obsoleto, corregido. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
