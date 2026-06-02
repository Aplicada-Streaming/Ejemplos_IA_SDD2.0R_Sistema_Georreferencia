# Plan de Iteración — Sprint 25

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-25_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-05-18
**Fecha fin:** 2027-05-29
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S22–S24); capacidad sugerida estricta 9 SP. Se compromete el SBOM firmado del paquete (8 SP). Es un sprint de supply-chain/DevOps: no agrega lógica de dominio, así que el gate de cobertura se mantiene; la verificación es la generación y firma del SBOM en el workflow de publicación.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Generar el SBOM (CycloneDX, JSON) del paquete `GeoVial.Sync` y firmarlo con cosign junto al `.nupkg` en el pipeline de publicación (supply-chain-seguridad §1, STAGE-09 → STAGE-10), adjuntándolo al release para que el consumidor verifique el inventario de dependencias y su integridad. Completa el endurecimiento de supply-chain de la librería (cierra el anti-patrón "falta de SBOM").

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-SBOM | Tarea | Generar y firmar el SBOM CycloneDX del paquete en el workflow de publicación | Alta | 8 | DevOps | Pendiente |

Total de puntos comprometidos: 8 SP. Cierra la acción reiterada de las retros S22/S24 (SBOM firmado del paquete). No agrega funcionalidad; completa el endurecimiento de supply-chain de la librería.

## 4. Alcance técnico

1. **Generación del SBOM** (`publish-sync.yml`, STAGE-09): se instala la herramienta CycloneDX para .NET y se genera el SBOM (CycloneDX JSON) del proyecto `GeoVial.Sync`, con su árbol de dependencias.
2. **Firma del SBOM** (STAGE-10): el SBOM se firma con cosign en modo keyless (igual que el `.nupkg`), produciendo su bundle de verificación, que el workflow verifica antes de publicar.
3. **Artefactos del release**: el `.nupkg`, su firma, el SBOM y la firma del SBOM se adjuntan como artefactos del workflow.
4. **Documentación**: se marca `supply-chain-seguridad §1` (SBOM) como implementado para la librería; las notas de release describen el SBOM y su verificación; el CHANGELOG lo registra.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El workflow genera el SBOM CycloneDX (JSON) del paquete `GeoVial.Sync`.
- El SBOM se firma con cosign y el workflow verifica su firma antes de publicar.
- El SBOM y su firma se adjuntan como artefactos del release.
- `supply-chain-seguridad §1` refleja el SBOM del paquete como implementado; el CHANGELOG lo registra.
- Las 307 pruebas existentes siguen verdes; el gate de cobertura se mantiene.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La CLI de CycloneDX cambia de flags entre versiones | Media | Bajo | Se fija la invocación con los flags estables (`-o`, `-fn`, `-j`); la generación corre en el workflow, aislada del build |
| El SBOM solo se ejercita al disparar el workflow por tag | Media | Bajo | Es una etapa de release; la firma del SBOM se verifica en el propio workflow (autovalidación) |
| Tiempo extra en el pipeline de publicación | Baja | Bajo | La generación y firma del SBOM son de segundos; corren solo en el publish por tag |

## 7. Criterios de hecho del sprint

El Sprint 25 se considera completo cuando el workflow de publicación genera el SBOM CycloneDX del paquete, lo firma con cosign y verifica su firma antes de publicar, y lo adjunta como artefacto; `supply-chain-seguridad §1` y el CHANGELOG lo reflejan; las 307 pruebas siguen verdes; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| EP | EP-09 (Librería de sincronización publicada): endurecimiento de supply-chain |
| ADRs que gobiernan | ADR-07 (publicación/versionado); ADR-14 (compliance) |
| Docs DevOps | supply-chain-seguridad §1 (SBOM CycloneDX) y §2 (firma cosign), pipeline-ci-cd (STAGE-09/STAGE-10), release-libreria-sync-v1.0.0 |
| Calidad | definition-of-done §1.4; anti-patrón "falta de SBOM" |
| Verificación | generación + firma del SBOM en el workflow |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 25 (SBOM CycloneDX firmado del paquete): generación (STAGE-09) + firma cosign (STAGE-10) + adjunto al release + documentación. Compromete 8 SP, cerrando el endurecimiento de supply-chain de la librería. Generado por AG-07 |
