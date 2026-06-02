# Sprint Retrospectiva — Sprint 16

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-16_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Existían guías DevOps detalladas (publicación + versionado) en estado Propuesto: implementarlas fue traducir un diseño ya pensado a artefactos concretos (csproj, scripts, workflow), sin reabrir decisiones.
- MinVer se integró sin fricción: el build siguió verde con `TreatWarningsAsErrors`, y el ajuste de `fetch-depth: 0` en CI previene el warning de repo shallow antes de que aparezca.
- El consumidor de prueba `samples/01-sync-basico` da una verificación concreta y barata de que la superficie pública es consumible, y entra a CI por ProjectReference sin acoplar el build a un paquete no publicado.
- El `dotnet pack` produjo un `.nupkg` correcto al primer intento (DLL + README + licencia MIT + nuspec), confirmando los metadatos.

## 2. Qué no salió bien

- Es un sprint sin lógica de dominio nueva, así que no aportó pruebas automatizadas nuevas: la verificación (pack + ejecución del sample) es manual/observacional, no un test del gate. Un test que valide el contenido del `.nupkg` cerraría ese hueco.
- No se publicó una versión real a GitHub Packages: requiere el token del repositorio y la aprobación del Release manager (tag `v1.0.0`), que exceden un sprint de preparación.
- El paquete aún no se firma (supply-chain-seguridad §2); la firma es un pre-requisito para declararlo consumible en stable.

## 3. Qué probar

- Crear el tag `v1.0.0` sobre `main` (con aprobación) para disparar el workflow y publicar el primer stable, y verificar la instalación por `dotnet add package` en un consumidor limpio.
- Agregar un test (o un stage de CI) que abra el `.nupkg` generado y verifique que contiene la DLL, el README y los metadatos esperados, para automatizar la verificación del empaquetado.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Taggear `v1.0.0` y publicar el primer stable de `GeoVial.Sync` (con aprobación del Release manager) | AG-09 (DevOps) | 2027-02-06 | Pendiente |
| Firmar el paquete (supply-chain) antes de declararlo consumible en stable | AG-09 (DevOps) | 2027-02-06 | Pendiente |
| Stage/test que verifique el contenido del `.nupkg` (DLL + README + metadatos) | AG-05 / QA | 2027-02-06 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 15 | Estado actual |
| --- | --- |
| Provisionar clave de mapas y mapa interactivo para la ubicación manual (US-13 UI completa) | Pendiente (se reitera) |
| Helper de integración del escenario completo de captura (área→agente asignado) | Pendiente (se reitera) |
| Publicar `GeoVial.Sync` como paquete preview en GitHub Packages | Completada en mecanismo (empaquetado + MinVer + workflow + sample); la publicación efectiva de una versión es por tag |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 16 con 3 acciones nuevas y seguimiento de las del Sprint 15 (mecanismo de publicación de la librería completado). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
