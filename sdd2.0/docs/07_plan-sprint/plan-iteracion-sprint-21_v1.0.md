# Plan de Iteración — Sprint 21

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-21_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-03-23
**Fecha fin:** 2027-04-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S18–S20); capacidad sugerida estricta 9 SP. Se compromete la preparación del primer release stable de la librería `GeoVial.Sync` (8 SP). El valor verificable —que el empaquetado produce un `.nupkg` correcto— entra al gate con una prueba sobre el contenido del paquete; la publicación efectiva de la versión la dispara el Release manager con el tag.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Dejar listo el primer release stable (`v1.0.0`) de la librería de sincronización `GeoVial.Sync`: el empaquetado produce el `.nupkg` durante el build (verificado por una prueba sobre su contenido), el CHANGELOG y las notas de release de `v1.0.0` están escritos, y el procedimiento de publicación queda documentado para que el Release manager dispare la publicación con el tag `v1.0.0` (estrategia-versionado §5). No se publica la versión en este sprint.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-REL | Tarea | Preparar el release stable v1.0.0 de GeoVial.Sync (empaquetado verificado + CHANGELOG/notas + checklist) | Alta | 8 | DevOps / QA | Pendiente |

Total de puntos comprometidos: 8 SP. Cierra el frente de release de EP-09: la librería pasa de "publicable" (Sprint 16, preview) a "lista para el primer stable", con la verificación del paquete bajo el gate. Resuelve la acción de la retro del Sprint 16 (test del contenido del `.nupkg`).

## 4. Alcance técnico

1. **Empaquetado en el build** (`GeoVial.Sync`): se habilita `GeneratePackageOnBuild` en `Release` para que el `.nupkg` se produzca como parte del build (en CI y local), no solo con `dotnet pack` manual.
2. **Prueba del contenido del paquete** (en el gate): una prueba localiza el `.nupkg` generado y verifica que contiene la DLL (`lib/net10.0/GeoVial.Sync.dll`), el `README.md` y un `.nuspec` con `id = GeoVial.Sync` y licencia `MIT`. Es la verificación automatizada que pedía la retro del Sprint 16.
3. **CHANGELOG y notas de release**: se cierra la sección `[No publicado]` del CHANGELOG como `[1.0.0]`, listando la superficie pública estable; se redactan las notas de release del primer stable.
4. **Checklist de release**: el procedimiento para que el Release manager publique `v1.0.0` (tag → workflow → verificación post-publish), apoyado en `guia-publicacion-paquete-github-packages`.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El build en `Release` produce el `.nupkg` de `GeoVial.Sync`.
- La prueba del contenido del paquete verifica DLL + README + `.nuspec` (id + licencia) y pasa en el gate.
- El CHANGELOG tiene la sección `[1.0.0]` y existen notas de release del primer stable.
- El checklist de release documenta el disparo por tag `v1.0.0` (publicación efectiva fuera de este sprint).
- Las 302 pruebas existentes siguen verdes; el gate de cobertura se mantiene.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| `GeneratePackageOnBuild` ralentiza cada build de Release | Media | Bajo | El costo de pack es de segundos; a cambio, CI produce y verifica el paquete en cada build |
| La prueba del `.nupkg` depende de la ruta del paquete generado | Media | Bajo | La prueba lo localiza por glob junto a la DLL de la librería; es agnóstica de la versión (MinVer) |
| Publicar v1.0.0 requiere aprobación y tag | Alta | Bajo | La publicación efectiva es por tag con aprobación del Release manager; este sprint deja todo listo y verificado |

## 7. Criterios de hecho del sprint

El Sprint 21 se considera completo cuando el build produce el `.nupkg`, la prueba del contenido del paquete está verde, el CHANGELOG `[1.0.0]` y las notas de release están escritos, y el checklist de release documenta el disparo por tag; las 302 pruebas siguen verdes; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| EP | EP-09 (Librería de sincronización publicada): frente de release |
| ADRs que gobiernan | ADR-07 (publicación/versionado, SemVer) |
| Docs DevOps | guia-publicacion-paquete-github-packages, estrategia-versionado (canal stable), pipeline-ci-cd (STAGE-11/13) |
| Calidad | definition-of-done §1.4 (release de la librería) |
| Tests previstos | unit/integration: contenido del `.nupkg` |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 21 (preparación del primer release stable v1.0.0 de GeoVial.Sync): empaquetado en build verificado por prueba del contenido del `.nupkg`, CHANGELOG/notas de release y checklist. Compromete 8 SP, cerrando el frente de release de EP-09. Generado por AG-07 |
