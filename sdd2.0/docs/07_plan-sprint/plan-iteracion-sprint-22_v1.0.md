# Plan de Iteración — Sprint 22

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-22_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-04-06
**Fecha fin:** 2027-04-17
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S19–S21); capacidad sugerida estricta 9 SP. Se compromete la firma del paquete de la librería (8 SP). Es un sprint de supply-chain/DevOps: no agrega lógica de dominio, por lo que el gate de cobertura se mantiene; la verificación es la firma y su validación en el workflow de publicación.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Firmar el paquete `GeoVial.Sync` en el pipeline de publicación con **cosign (sigstore) en modo keyless** (OIDC de GitHub Actions, sin llaves privadas de larga vida), verificando la firma en el propio workflow antes de publicar y documentando la verificación para los consumidores. Es la última pieza de endurecimiento de release para declarar `v1.0.0` plenamente consumible (supply-chain-seguridad §2; anti-patrón "falta de firma del artefacto").

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-SIGN | Tarea | Firmar el paquete con cosign keyless + verificación en el workflow + documentación de verificación | Alta | 8 | DevOps | Pendiente |

Total de puntos comprometidos: 8 SP. Cierra la acción de la retro del Sprint 21 (firmar el paquete antes de declararlo consumible en stable). No agrega funcionalidad; endurece el release.

## 4. Alcance técnico

1. **Firma en el workflow** (`publish-sync.yml`, STAGE-10): tras empaquetar y verificar el contenido, se instala cosign y se firma cada `.nupkg` en modo **keyless** con el OIDC de Actions (`id-token: write`), produciendo un bundle detached (`<paquete>.cosign.bundle`) con certificado efímero y registro en Rekor.
2. **Verificación en el pipeline**: el mismo workflow ejecuta `cosign verify-blob` contra la identidad esperada (emisor OIDC de Actions + identidad del repositorio) antes de publicar; si la firma no valida, el publish no ocurre.
3. **Artefactos del release**: el `.nupkg` y su `.cosign.bundle` se adjuntan como artefactos del workflow.
4. **Documentación**: el procedimiento de verificación para los consumidores (`cosign verify-blob ...`) en las notas de release y la guía de publicación; se marca la firma del paquete de la librería como implementada en `supply-chain-seguridad` y se anota en el CHANGELOG.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El workflow de publicación firma el `.nupkg` con cosign keyless y produce el bundle de verificación.
- El workflow verifica la firma (`cosign verify-blob`) antes de publicar; una firma inválida aborta el publish.
- La verificación para consumidores está documentada en las notas de release y la guía de publicación.
- `supply-chain-seguridad` refleja la firma del paquete de la librería como implementada; el CHANGELOG lo registra.
- Las 303 pruebas existentes siguen verdes; el gate de cobertura se mantiene.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La firma keyless requiere permisos OIDC del workflow | Alta | Bajo | Se añade `id-token: write` al workflow; cosign usa el OIDC de Actions, sin llaves privadas |
| La firma corre solo en el workflow (tag), no en el gate de pruebas | Media | Bajo | Es una etapa de release; la verificación de la firma vive en el propio workflow (autovalidación) |
| El consumidor no sabe verificar la firma | Media | Bajo | Se documenta `cosign verify-blob` con la identidad e issuer esperados en notas de release y guía |

## 7. Criterios de hecho del sprint

El Sprint 22 se considera completo cuando el workflow de publicación firma el `.nupkg` con cosign keyless, verifica la firma antes de publicar, adjunta el bundle como artefacto, y la verificación queda documentada para los consumidores; `supply-chain-seguridad` y el CHANGELOG lo reflejan; las 303 pruebas siguen verdes; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| EP | EP-09 (Librería de sincronización publicada): endurecimiento de release |
| ADRs que gobiernan | ADR-07 (publicación/versionado); ADR-14 (compliance) |
| Docs DevOps | supply-chain-seguridad §2 (firma cosign keyless, Rekor, SLSA L2), pipeline-ci-cd (STAGE-10), guia-publicacion-paquete-github-packages, release-libreria-sync-v1.0.0 |
| Calidad | definition-of-done §1.4; anti-patrón "falta de firma del artefacto" |
| Verificación | firma + `cosign verify-blob` en el workflow |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 22 (firma del paquete de la librería con cosign keyless): firma + verificación en el workflow de publicación + documentación de verificación. Compromete 8 SP, cerrando el endurecimiento de release de EP-09. Generado por AG-07 |
