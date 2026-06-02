# Sprint Retrospectiva — Sprint 21

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-21_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Producir el paquete en el build (`GeneratePackageOnBuild` en Release) + una prueba que abre el `.nupkg` y verifica su contenido cerró la acción de la retro del Sprint 16 de forma automatizada y barata.
- La prueba localiza el paquete por glob junto a la librería y es agnóstica de la versión (MinVer), así que no se rompe al cambiar el número de versión.
- El checklist de release dejó explícito el límite: el equipo prepara y verifica; el tag `v1.0.0` y la aprobación son del Release manager. Sin ambigüedad sobre quién publica.
- La doble verificación (prueba del gate + paso en el workflow de publish) cubre tanto el build local/CI como el momento de publicación.

## 2. Qué no salió bien

- El `.nupkg` sigue sin firmar (supply-chain-seguridad §2); la firma es un pre-requisito para declararlo plenamente consumible en stable y no entró en este sprint.
- La prueba del contenido del paquete solo corre con sentido en Release (en Debug retorna temprano); no es un hueco real, pero la cobertura del empaquetado depende de que CI use Release.
- `GeneratePackageOnBuild` agrega segundos a cada build de Release; aceptable, pero conviene vigilar el tiempo de CI si se suman más paquetes.

## 3. Qué probar

- Firmar el paquete en el workflow de publicación (supply-chain) y verificar la firma post-publish.
- Tras taggear `v1.0.0`, validar la instalación por `dotnet add package` en un consumidor limpio (verificación post-publish de la guía).

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Firmar el paquete en el workflow de publicación y verificar la firma | AG-09 (DevOps) | 2027-04-17 | Pendiente |
| Taggear `v1.0.0` y validar la instalación post-publish en un consumidor limpio | AG-09 (Release manager) | 2027-04-17 | Pendiente |
| Pulido móvil pendiente (mapa interactivo, caché de fotos) y E2E del ciclo de conflictos | AG-08 / AG-05 | 2027-04-17 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 20 | Estado actual |
| --- | --- |
| E2E del ciclo de edición en conflicto (sync colisión → listar → confirmar) | Pendiente |
| Que el helper de escenario siembre su propia área (desacoplar del seed) | Pendiente |
| Preparar el primer release: tag v1.0.0 de la librería y endurecimiento de release | Completada en preparación (empaquetado verificado + CHANGELOG/notas + checklist); el tag lo dispara el Release manager |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 21 con 3 acciones nuevas y seguimiento de las del Sprint 20 (preparación del release completada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
