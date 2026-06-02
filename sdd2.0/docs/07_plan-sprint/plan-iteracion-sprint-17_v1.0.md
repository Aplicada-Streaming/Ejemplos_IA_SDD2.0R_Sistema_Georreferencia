# Plan de Iteración — Sprint 17

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-17_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-01-26
**Fecha fin:** 2027-02-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 9,0 SP (S14–S16); capacidad sugerida estricta 10 SP. Se comprometen US-21 (revisión sobre mapa, frente cliente, 5 SP) y US-22 (recorrer carrusel / navegar marcadores, 5 SP), 10 SP. El valor testeable —la navegación del carrusel y el cliente de la API de revisión— entra al gate; la pantalla MAUI queda fuera de CI.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Permitir que el jefe de área revise un relevamiento sobre el mapa desde la app móvil: ver los marcadores con sus fotos y comentarios (US-21) y recorrerlos como un carrusel, navegando entre marcadores y entre las fotos de cada marcador (US-22), consumiendo la API de revisión ya existente (CU-08).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-21 | Historia | Revisar el relevamiento sobre el mapa (frente cliente móvil) | Media | 5 | Dev móvil / Dev fullstack | Pendiente |
| US-22 | Historia | Recorrer el carrusel y navegar entre marcadores (frente cliente) | Media | 5 | Dev móvil | Pendiente |

Total de puntos comprometidos: 10 SP. El backend de revisión (CU-08, `GET /relevamientos/{id}/revision`) ya está entregado (Sprint 04); este sprint construye el frente cliente: el cliente de la API y la navegación del carrusel, más la pantalla MAUI.

## 4. Alcance técnico

Se construye el núcleo testeable del cliente de revisión, separado de la cáscara MAUI:

1. **`GeoVial.Revision`** (biblioteca `net10.0`, dentro de la solución y del gate de CI):
   - `NavegadorRevision`: la lógica de navegación del carrusel (US-22) sobre `RevisionRelevamientoDto`: marcador actual, siguiente/anterior marcador (circular), y dentro del marcador, foto actual y siguiente/anterior foto. Maneja relevamientos sin marcadores y marcadores sin fotos sin romper.
   - `ClienteRevisionHttp`: cliente de la API de revisión (`GET /api/v1/relevamientos/{id}/revision`, con filtro opcional por etiquetas) que parsea el `RevisionRelevamientoDto`.
2. **Pantalla de revisión en `GeoVial.Mobile`** (fuera de la solución/CI): lista los marcadores del relevamiento, muestra el marcador actual (coordenada, indicador de conflicto), sus fotos (descargando el binario del endpoint de contenido) y comentarios, y permite recorrer el carrusel con los controles de navegación.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El cliente de revisión consume `GET /relevamientos/{id}/revision` (con y sin filtro de etiquetas) y parsea los marcadores con sus fotos y comentarios.
- La navegación del carrusel recorre los marcadores en ambos sentidos (circular) y, dentro de un marcador, sus fotos; un relevamiento sin marcadores o un marcador sin fotos no rompen la navegación.
- El núcleo `GeoVial.Revision` respeta el gate de cobertura (líneas ≥ 80 %, branches ≥ 70 %); la pantalla MAUI queda fuera del gate.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El control de mapas de MAUI requiere clave de proveedor | Media | Bajo | La revisión lista marcadores y los recorre como carrusel con su coordenada; el mapa interactivo es evolución de la pantalla, no bloquea la navegación testeable |
| La descarga de binarios de fotos satura la pantalla | Baja | Bajo | Se descarga la foto del marcador actual bajo demanda, no todas a la vez |
| La pantalla MAUI no compila en CI (Android SDK) | Alta | Bajo | `GeoVial.Mobile` queda fuera de la solución/CI; el núcleo testeable vive en `GeoVial.Revision` |

## 7. Criterios de hecho del sprint

El Sprint 17 se considera completo cuando US-21 y US-22 (frente cliente) están terminadas según la DoD con sus pruebas verdes: el cliente consume la API de revisión y la navegación del carrusel recorre marcadores y fotos; `GeoVial.Revision` está dentro del gate y la pantalla de revisión vive en `GeoVial.Mobile` fuera de CI; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que avanzan | US-21 (revisar sobre mapa), US-22 (carrusel y navegación) |
| CU que avanzan | CU-08 (revisión sobre mapa) |
| EP | EP-04 (revisión y consolidación) / CU-08 |
| NB que avanzan | NB-01 (jerarquía/autorización), NB-04 (revisión) |
| RN aplicadas | RN-01 (autorización por área), RN-05 (solo lectura) |
| BT derivadas | BT-07, BT-19 |
| Tests previstos | acceptance/AT-08-revision-mapa |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 17 (revisión sobre mapa y carrusel en el cliente móvil, US-21/US-22). Compromete 10 SP. El núcleo `GeoVial.Revision` entra al gate; la pantalla MAUI queda fuera de CI. Generado por AG-07 |
