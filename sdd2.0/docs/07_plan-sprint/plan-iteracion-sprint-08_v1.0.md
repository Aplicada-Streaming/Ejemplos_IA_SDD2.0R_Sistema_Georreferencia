# Plan de Iteración — Sprint 08

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-08_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-09-15
**Fecha fin:** 2026-09-26
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 14,0 SP (ventana deprimida por los dos sprints acotados S05/S06; S07 repuntó a 16). Se comprometen 11 SP. El compromiso cierra la épica de revisión (EP-05) sumando el pipeline de imágenes (Must), aprovechando el trabajo de alojamiento del Sprint 07; el margen se reserva por la incorporación de una dependencia externa de procesamiento de imágenes.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 120 | 0,72 | 86 h |
| Dev frontend | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Acotar el tamaño de las fotos comprimiéndolas y redimensionándolas al subirlas (sin perder la georreferenciación, que ya vive en el dominio), y cerrar la revisión sobre el mapa permitiendo al jefe de área filtrar las observaciones por etiquetas y abrir una foto a pantalla completa con zoom, todo sobre el alojamiento de fotos ya integrado.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-19 | Backlog técnico | Pipeline de imágenes (compresión y redimensión) al subir el binario | Alta (Must) | 5 | Dev backend A | Pendiente |
| US-23 | Historia | Filtrar fotos y observaciones por etiquetas en la revisión | Media (Could) | 3 | Dev backend B / Dev frontend | Pendiente |
| US-24 | Historia | Visor de fotos a pantalla completa con zoom | Media (Could) | 3 | Dev frontend | Pendiente |

Total de puntos comprometidos: 11 SP. BT-19 es Must y depende del alojamiento (BT-20, Sprint 07). US-23 y US-24 (Could de EP-05) se refinan a `Ready` en este plan: cierran la épica de revisión sobre mapa, son de bajo costo y se apoyan en el etiquetado (RC-04, Sprint 04) y el carrusel (US-22). El grueso del trabajo Must restante (EP-04 sincronización, EP-08 consulta de auditoría) es un esfuerzo mayor que se aborda en sprints dedicados.

## 4. Alcance técnico

Componentes que se construyen o modifican (sobre la arquitectura de 05, sin redefinirla):

1. BT-19 — Pipeline de imágenes (arquitectura-solución §8 NFR tamaño de foto; ADR-05): se define la abstracción `IPipelineImagen` en `GeoVial.FileHosting` (sin dependencias de proveedor) y una implementación sobre una librería de procesamiento de imágenes de licencia permisiva (SixLabors.ImageSharp 2.x, Apache 2.0) en `GeoVial.Infrastructure`. La subida de contenido de foto (CU-04) procesa el binario —redimensión a un máximo configurable y compresión JPEG— antes de persistirlo en el backend de alojamiento, acotando el payload. La fuente de coordenada (RN-03) se preserva: ya está asentada en el dominio (`Foto.Fuente`) en la captura, independiente del binario reprocesado.
2. US-23 — Filtrado por etiquetas (CU-08 §5.C): la consulta de revisión acepta un conjunto de etiquetas; cuando se especifica, la proyección incluye solo las fotos y comentarios que tienen alguna de esas etiquetas y descarta los marcadores sin contenido coincidente (RC-04). Sin coincidencias, la revisión vuelve vacía y la web lo indica.
3. US-24 — Visor a pantalla completa (CU-09): se expone la descarga del binario de una foto (`GET /api/v1/fotos/{id}/contenido`), autorizada por área (RN-01), que recupera el binario del backend activo. La web abre la foto a pantalla completa sobre el carrusel, con zoom, y al cerrar vuelve al carrusel.

El detalle fino de interacción del visor (gestos, niveles de zoom) pertenece a 03; aquí se entrega la capacidad funcional (ampliar y acercar). El pipeline de imágenes del cliente móvil y la sincronización (EP-04) quedan fuera de este sprint.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- Al subir una foto, el binario persistido queda comprimido/redimensionado por debajo del máximo configurado; la georreferenciación del dominio no se altera.
- El filtro por etiquetas muestra solo las observaciones coincidentes; sin coincidencias, lo indica sin error.
- La foto se abre a pantalla completa con zoom y al cerrar vuelve al carrusel; la descarga del binario respeta el acotamiento por área (RN-01).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La librería de imágenes podría introducir una licencia no permisiva | Media | Alto | Fijar SixLabors.ImageSharp 2.x (Apache 2.0); la abstracción `IPipelineImagen` vive en `GeoVial.FileHosting` sin la dependencia, que se confina a infraestructura |
| Redimensionar puede agrandar imágenes pequeñas o romper la orientación | Media | Medio | Redimensión solo hacia abajo (no upscaling) y autoorientación antes de codificar; pruebas que verifican que la dimensión y el tamaño no aumentan |
| Mostrar el binario en la web requiere autorización por área | Baja | Medio | La descarga pasa por un endpoint autenticado y autorizado (RN-01); la web obtiene el binario y lo muestra como recurso embebido, sin exponer el backend |

## 7. Criterios de hecho del sprint

El Sprint 08 se considera completo cuando BT-19, US-23 y US-24 están terminadas según la DoD con sus pruebas verdes; la subida con compresión, el filtrado por etiquetas y el visor a pantalla completa quedan demostrados en el sprint review; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| BT que avanzan | BT-19 (pipeline de imágenes) |
| US que avanzan | US-23 (filtrado por etiquetas), US-24 (visor a pantalla completa) |
| CU que avanzan | CU-04 (guardar foto con compresión), CU-08 (filtrado en revisión), CU-09 (visor del carrusel) |
| NB que avanzan | NB-04 (revisión y resguardo del relevamiento) |
| ADRs que gobiernan | ADR-05 (NFR de tamaño/sincronización), ADR-08 (alojamiento), ADR-04 (mapa/carrusel), ADR-10 (capas) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 08 (pipeline de imágenes y cierre de EP-05). Compromete BT-19, US-23 y US-24 (11 SP). Cierra la épica de revisión sobre mapa y acota el tamaño de foto; la dependencia de imágenes se fija en licencia permisiva. Generado por AG-07 |
