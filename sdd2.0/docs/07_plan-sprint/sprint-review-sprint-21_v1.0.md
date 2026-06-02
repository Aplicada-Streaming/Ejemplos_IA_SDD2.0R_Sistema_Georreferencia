# Sprint Review — Sprint 21

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-21_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-21_v1.0.md`:

> Dejar listo el primer release stable (`v1.0.0`) de la librería de sincronización `GeoVial.Sync`: el empaquetado produce el `.nupkg` durante el build (verificado por una prueba sobre su contenido), el CHANGELOG y las notas de release de `v1.0.0` están escritos, y el procedimiento de publicación queda documentado para que el Release manager dispare la publicación con el tag `v1.0.0`. No se publica la versión en este sprint.

Veredicto: Cumplido.

Explicación corta: `GeoVial.Sync` produce ahora el `.nupkg` como parte del build en Release (`GeneratePackageOnBuild`), y una prueba del gate verifica su contenido (DLL `lib/net10.0/GeoVial.Sync.dll`, `README.md` y un `.nuspec` con `id=GeoVial.Sync` y licencia `MIT`), cerrando la acción de la retro del Sprint 16. El CHANGELOG incorpora la sección `[1.0.0]` con la superficie pública congelada, y un documento de release (`09_devops/release-libreria-sync-v1.0.0`) reúne las notas y el checklist para que el Release manager publique con el tag `v1.0.0`. El workflow de publicación suma un paso que verifica el `.nupkg` antes de empujarlo al feed.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Empaquetado | El build en Release genera el `.nupkg`; la prueba del gate valida su contenido | Paquete listo y verificado |
| EP-09 | DevOps | El workflow verifica el `.nupkg` (DLL/README/nuspec) antes de publicar | Publicación con red de seguridad |
| EP-09 | Release | CHANGELOG `[1.0.0]` + notas + checklist de release por tag | Procedimiento de release claro |

## 3. Feedback recibido

- La librería queda lista para su primer stable: el paquete se produce y verifica en cada build, no solo a mano.
- La superficie pública queda congelada bajo SemVer a partir de `v1.0.0` (ADR-07).
- La publicación efectiva de la versión es una decisión del Release manager (tag `v1.0.0` + aprobación), correctamente fuera del alcance automatizado de un sprint.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 303 verdes (270 unitarias + 33 de integración), +1 respecto del Sprint 20 (la prueba del contenido del `.nupkg`). El gate de cobertura se mantiene. Build Release sin warnings tratados como error, generando el paquete.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-REL | Tarea | Aceptada (empaquetado en build verificado + CHANGELOG/notas + checklist; publicación por tag con aprobación, fuera del sprint) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 21 se traslada. |

La publicación efectiva de `v1.0.0` (tag + aprobación) y la firma del paquete (supply-chain) quedan como pasos de release del Release manager. El pulido móvil (mapa interactivo, caché) y el E2E del ciclo de conflictos siguen en el backlog.

## 7. Decisiones tomadas durante el review

- Producir el paquete en el build (`GeneratePackageOnBuild` en Release) para que CI lo genere y verifique siempre.
- Congelar la superficie pública con `v1.0.0`: en adelante, breaking change ⇒ MAJOR (ADR-07).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 21 (preparación del primer release stable v1.0.0 de GeoVial.Sync). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
