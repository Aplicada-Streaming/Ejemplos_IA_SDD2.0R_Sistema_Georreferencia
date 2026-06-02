# Plan de Iteración — Sprint 26

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-26_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-06-01
**Fecha fin:** 2027-06-12
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S23–S25); capacidad sugerida estricta 9 SP. Se compromete el pulido de la pantalla de revisión móvil (8 SP). El valor testeable —la caché de fotos y la navegación que conserva la posición— entra al gate; la pantalla MAUI queda fuera de CI.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Pulir la experiencia de revisión en la app móvil: cachear en memoria las fotos del carrusel para no re-descargarlas al navegar y conservar la posición del carrusel (marcador en foco) al recargar la revisión tras una edición. Vuelve al frente de producto tras los sprints de cierre y supply-chain (acciones de las retros S17/S19/S23).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-UX-MOVIL | Tarea | Caché de fotos del carrusel + conservar la posición al recargar (frente cliente) | Media | 8 | Dev móvil / Dev fullstack | Pendiente |

Total de puntos comprometidos: 8 SP. No agrega historias nuevas: pule la pantalla de revisión (US-21/US-22) ya entregada. El mapa interactivo sigue pendiente por requerir una clave de proveedor de mapas, fuera del alcance.

## 4. Alcance técnico

1. **`GeoVial.Revision`** (en la solución y el gate):
   - `CacheFotos`: una caché LRU en memoria (clave `FotoId` → bytes) con capacidad máxima configurable que evita re-descargar la foto en foco al navegar el carrusel; al exceder la capacidad descarta la entrada menos usada recientemente.
   - `NavegadorRevision.IrAlMarcador(Guid)`: posiciona el carrusel en un marcador por su identificador (reiniciando la foto), para restaurar la posición tras recargar la revisión.
2. **Pantalla de revisión en `GeoVial.Mobile`** (fuera de CI): usa `CacheFotos` para servir la foto en foco sin re-descargarla y, tras una edición, recarga la revisión conservando el marcador en foco con `IrAlMarcador`.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- La caché devuelve los bytes ya guardados sin re-descargar; al exceder la capacidad, descarta la entrada menos usada recientemente (LRU).
- `IrAlMarcador` posiciona el carrusel en el marcador indicado y reinicia la foto; si el marcador no existe, no rompe.
- El núcleo `GeoVial.Revision` respeta el gate de cobertura (líneas ≥ 80 %, branches ≥ 70 %); la pantalla MAUI queda fuera del gate.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La caché en memoria crece sin límite | Media | Medio | Capacidad máxima con descarte LRU; cubierto por pruebas |
| El mapa interactivo requiere clave de proveedor | Alta | Bajo | Fuera del alcance de este sprint; la revisión sigue por coordenada/carrusel |
| La pantalla MAUI no compila en CI | Alta | Bajo | `GeoVial.Mobile` queda fuera de la solución/CI; el núcleo testeable vive en `GeoVial.Revision` |

## 7. Criterios de hecho del sprint

El Sprint 26 se considera completo cuando la caché LRU de fotos y `IrAlMarcador` están terminados según la DoD con sus pruebas verdes; la pantalla de revisión usa la caché y conserva la posición tras editar; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que pulen | US-21 (revisión sobre mapa), US-22 (carrusel) |
| CU | CU-08 (revisión), CU-09 (gestión del marcador) |
| NB | NB-04 |
| Calidad | definition-of-done §1; retros S17/S19/S23 (caché y posición del carrusel) |
| Tests previstos | unit: caché LRU + navegación con `IrAlMarcador` |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 26 (pulido móvil de la revisión): caché LRU de fotos del carrusel + conservar la posición al recargar. Compromete 8 SP. El núcleo entra al gate; la pantalla MAUI queda fuera de CI. Generado por AG-07 |
