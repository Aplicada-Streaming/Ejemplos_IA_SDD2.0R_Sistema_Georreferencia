# Plan de Iteración — Sprint 16

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-16_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-01-12
**Fecha fin:** 2027-01-23
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 9,0 SP (S13–S15); capacidad sugerida estricta 10 SP. Se compromete US-32-pub (publicación del paquete, 8 SP), cerrando EP-09. Es un sprint de empaquetado/DevOps: no agrega lógica de dominio, así que el gate de cobertura (dominio/aplicación) se mantiene sin nueva superficie; la verificación es el empaquetado correcto, el consumidor de prueba y el workflow de publicación.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Materializar la publicación de la librería de sincronización `GeoVial.Sync` como paquete NuGet en GitHub Packages, con versionado automático por MinVer (SemVer, ADR-07) y canales preview/stable, cerrando EP-09. Implementa la guía `09_devops/guia-publicacion-paquete-github-packages_v1.0.md` (estado Propuesto) y deja un consumidor de prueba (`samples/01-sync-basico`) que valida que la superficie pública es consumible.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-32-pub | Historia | Publicar `GeoVial.Sync` como paquete (preview) con versionado automático | Should | 8 | Dev fullstack / DevOps | Pendiente |

Total de puntos comprometidos: 8 SP. Cierra el frente de publicación de EP-09 (la demo de evaluación se entregó en el Sprint 13). Acción reiterada en las retrospectivas de los Sprints 12 a 15.

## 4. Alcance técnico

1. **Empaquetado de `GeoVial.Sync`**: metadatos NuGet (PackageId `GeoVial.Sync`, descripción, autores, tags, README de paquete, licencia, URL del repositorio) y versionado automático con **MinVer** (prefijo de tag `v`, prerelease automático para el canal preview; estrategia-versionado §3, §5). El `dotnet pack` produce un `.nupkg` con la DLL y el README.
2. **Consumidor de prueba `samples/01-sync-basico`**: una app de consola mínima que ejercita la superficie pública (`IChangeQueue`/`ISyncEngine`) contra un backend en memoria, demostrando alta local → sincronización → cola vacía. Entra a la solución/CI (net10.0 puro) como verificación de que la API es consumible (guía §3).
3. **Automatización de publicación**: scripts `scripts/publish-pack-sync.bat` (STAGE-11, pack a `./artifacts`) y `scripts/publish-sync.bat` (STAGE-13, push a GitHub Packages) y un workflow `.github/workflows/publish-sync.yml` que, en un tag `v*`, empaqueta y publica con el token de Actions (`packages: write`). La publicación real a GitHub Packages se dispara por tag con aprobación; este sprint deja el mecanismo listo y verificado localmente, sin publicar una versión.
4. **CHANGELOG** (Keep a Changelog) inicial de la librería y actualización de la guía/estrategia a estado Aceptado.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- `dotnet pack src/GeoVial.Sync -c Release` produce `GeoVial.Sync.<version>.nupkg` con la DLL y el README; la versión la calcula MinVer (no se edita a mano).
- `samples/01-sync-basico` compila y, ejecutado, muestra la cola pasando de pendiente a sincronizado contra el backend en memoria.
- El workflow de publicación está definido y dispara en tag `v*` con el token de Actions (`packages: write`); no se publica una versión en este sprint (se hace por tag con aprobación, estrategia-versionado §5).
- El gate de cobertura se mantiene (no hay lógica de dominio nueva); las 282 pruebas existentes siguen verdes.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| MinVer sin tags produce una versión 0.0.0-alpha (no 1.0.0) | Alta | Bajo | Es el comportamiento esperado del canal preview antes del primer tag `v1.0.0`; el stable se materializa al taggear (estrategia-versionado §3) |
| Publicar a GitHub Packages requiere token/credencial del repositorio | Alta | Medio | El workflow usa el token de Actions con `packages: write`; la publicación real es por tag con aprobación, fuera del alcance de este sprint |
| El consumidor de prueba acopla CI a un paquete no publicado | Media | Bajo | `samples/01-sync-basico` consume por ProjectReference en CI; la instalación por paquete (`dotnet add package`) es la verificación manual post-publish (guía §3) |

## 7. Criterios de hecho del sprint

El Sprint 16 se considera completo cuando `GeoVial.Sync` empaqueta como `.nupkg` con metadatos y versión MinVer, existe el consumidor de prueba `samples/01-sync-basico` que valida el consumo de la superficie pública, el workflow y los scripts de publicación están listos (sin publicar una versión), el CHANGELOG inicial está creado y la guía/estrategia quedan en estado Aceptado; las 282 pruebas siguen verdes; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US/EP que avanzan | EP-09 (Librería de sincronización publicada); cierre del frente de publicación |
| CU relacionados | CU-06, CU-07 (capacidad que materializa la librería) |
| ADRs que gobiernan | ADR-07 (publicación/versionado, SemVer) |
| Docs DevOps | guia-publicacion-paquete-github-packages, estrategia-versionado, pipeline-ci-cd (STAGE-11/STAGE-13) |
| Tests/Verificación | empaquetado (`dotnet pack`), consumidor `samples/01-sync-basico` |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 16 (publicación de `GeoVial.Sync` como paquete preview con MinVer). Compromete la publicación (8 SP), cerrando EP-09. Empaquetado + consumidor de prueba en CI; workflow/scripts de publish listos sin publicar versión. Generado por AG-07 |
