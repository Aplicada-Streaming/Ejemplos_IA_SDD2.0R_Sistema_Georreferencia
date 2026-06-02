# Sprint Review — Sprint 16

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-16_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-16_v1.0.md`:

> Materializar la publicación de la librería de sincronización `GeoVial.Sync` como paquete NuGet en GitHub Packages, con versionado automático por MinVer (SemVer, ADR-07) y canales preview/stable, cerrando EP-09.

Veredicto: Cumplido.

Explicación corta: `GeoVial.Sync` quedó empaquetable con metadatos NuGet (PackageId, descripción, autores, tags, README de paquete, licencia MIT, URL del repositorio) y versionado automático por **MinVer** (prefijo de tag `v`). `dotnet pack` produce `GeoVial.Sync.<version>.nupkg` con la DLL y el README; sin tag `v*`, MinVer entrega la versión preview `0.0.0-alpha.0.N` (canal preview), y el primer stable se materializará al crear el tag `v1.0.0`. El consumidor de prueba `samples/01-sync-basico` valida que la superficie pública es consumible (alta local → sincronización → cola vacía). Los scripts (`publish-pack-sync.bat`, `publish-sync.bat`, `publish-sync-deprecate.bat`) y el workflow `publish-sync.yml` (dispara en tag `v*` con el token de Actions) dejan la publicación lista, sin publicar una versión en este sprint.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Empaquetado | `dotnet pack` produce el `.nupkg` con la DLL, el README y la versión MinVer; nuspec con id/licencia/readme | Paquete consumible y trazable |
| EP-09 | Verificación | `samples/01-sync-basico` consume la librería y muestra la cola pasar de pendiente a sincronizado | API pública consumible |
| EP-09 | DevOps | Workflow `publish-sync.yml` definido (STAGE-11/13) que en tag `v*` empaqueta y publica a GitHub Packages | Mecanismo de publicación listo |

## 3. Feedback recibido

- EP-09 queda cerrada: la librería ya es publicable y consumible como paquete, no solo como proyecto interno.
- Se materializaron las guías DevOps que estaban en estado Propuesto (publicación + versionado), cerrando una acción reiterada en las retrospectivas de los Sprints 12 a 15.
- La publicación real de una versión queda a un tag `v1.0.0` con aprobación del Release manager; el mecanismo está verificado localmente.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 282 verdes (252 unitarias + 30 de integración), sin cambio respecto del Sprint 15: es un sprint de empaquetado/DevOps sin lógica de dominio nueva, por lo que no se agregaron pruebas; el gate de cobertura (dominio/aplicación) se mantiene. La verificación fue el `dotnet pack` (contenido del `.nupkg`) y la ejecución de `samples/01-sync-basico`. Build Release sin warnings tratados como error, con MinVer activo. Se ajustó el checkout de CI a `fetch-depth: 0` para que MinVer disponga del historial y los tags.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-32-pub | Historia | Aceptada (empaquetado + MinVer + consumidor de prueba + scripts/workflow de publicación; sin publicar versión, por tag con aprobación) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 16 se traslada. |

La publicación efectiva de `v1.0.0` (tag + aprobación del Release manager) y la firma del paquete (supply-chain) quedan como pasos de release, fuera del alcance de este sprint.

## 7. Decisiones tomadas durante el review

- Adoptar MIT como licencia del paquete (y del repositorio de ejemplos) — `LICENSE` en la raíz.
- Mantener `GeoVial.Sync` con `MinVerTagPrefix=v`; el primer release stable se hará por tag `v1.0.0` con aprobación.
- Conservar el consumo interno (demo/app) por ProjectReference en CI; la instalación por paquete es la verificación manual post-publish.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 16 (publicación de `GeoVial.Sync` como paquete preview con MinVer; cierre del frente de publicación de EP-09). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
