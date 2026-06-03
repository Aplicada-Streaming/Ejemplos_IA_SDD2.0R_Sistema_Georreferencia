# Plan de Iteración — Sprint 33

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-33_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-09-07
**Fecha fin:** 2027-09-18
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S30–S32); capacidad sugerida estricta 9 SP. Se compromete el mantenimiento post-release de supply-chain (8 SP): configurar Dependabot e institucionalizar la validación con tag preview `-rc`. Es DevOps; sin lógica de dominio nueva, el gate se mantiene.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Endurecer el mantenimiento post-release de la cadena de suministro: (1) configurar **Dependabot** para que actualice y alerte automáticamente las dependencias del repo —NuGet, GitHub Actions e imágenes base de Docker— cumpliendo `supply-chain-seguridad §4`; y (2) **institucionalizar** la validación con un tag preview `-rc` antes de cada release stable, convirtiendo en regla escrita la lección del Sprint 29 (cuatro bugs latentes que sólo aparecieron al disparar un tag). Atiende acciones reiteradas de las retros S29/S31/S32.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-SUPPLY-MANT | Tarea | Configurar Dependabot (NuGet/Actions/Docker) + checklist de release con validación `-rc` obligatoria | Media | 8 | DevOps (AG-09) | Pendiente |

Total de puntos comprometidos: 8 SP. No agrega historias nuevas: es mantenimiento de la cadena de suministro ya entregada (S22/S25/S28). Sin cambios de backend ni dominio.

## 4. Alcance técnico

1. **`.github/dependabot.yml`**: actualizaciones semanales para los ecosistemas
   - `nuget` (proyectos .NET, directorio raíz; agrupa minor/patch para reducir ruido),
   - `github-actions` (los workflows de `.github/workflows`),
   - `docker` (los tres Dockerfiles: `src/GeoVial.Api`, `src/GeoVial.Web`, `infra/db`).
   Con límites de PRs abiertos y etiquetas. Nota: las libs vendorizadas/CDN del mapa (Leaflet) no las cubre Dependabot (no hay ecosistema para JS hand-vendored); su versión se sigue a mano (documentado).
2. **`09_devops/checklist-release`**: checklist de release **genérico** que exige validar el pipeline con un tag preview `-rc.N` antes del stable (institucionaliza la lección de S29); aplica a cualquier workflow que sólo corra en tag.
3. **`supply-chain-seguridad` → v1.4**: §4 (dependency scanning) marcado como implementado con Dependabot; nota del seguimiento manual de Leaflet.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- `dependabot.yml` declara los tres ecosistemas (nuget, github-actions, docker×3) con cadencia semanal y estructura válida según el esquema de Dependabot.
- El checklist de release exige la validación con tag preview `-rc` antes del stable, con la justificación (incidente S29).
- `supply-chain-seguridad §4` queda como implementado (Dependabot) con la nota del seguimiento manual de Leaflet.
- La suite .NET (325 pruebas) permanece verde; el gate de cobertura no se ve afectado (cambio de DevOps, sin lógica nueva).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Ruido de PRs de Dependabot | Media | Bajo | Agrupar minor/patch de NuGet; límite de PRs abiertos; cadencia semanal |
| `dependabot.yml` con error de esquema (no verificable localmente) | Media | Bajo | Se escribe según el esquema documentado y se revisa; GitHub valida en push y reporta en la pestaña Dependabot |
| Falsa sensación de cobertura (Leaflet no lo ve Dependabot) | Media | Bajo | Documentado explícitamente: el seguimiento de Leaflet es manual |

## 7. Criterios de hecho del sprint

El Sprint 33 se considera completo cuando `dependabot.yml` está en el repo con los tres ecosistemas, el checklist de release exige la validación `-rc` antes del stable, `supply-chain-seguridad` queda en v1.4 con §4 implementado, la suite .NET sigue verde, y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Política | supply-chain-seguridad §4 (dependency scanning, Dependabot) |
| Proceso | checklist de release con validación `-rc` (lección S29) |
| Artefactos | `.github/dependabot.yml`, `09_devops/checklist-release` |
| Calidad | definition-of-done §1.4; retros S29/S31/S32 (Dependabot, tag preview) |
| Tests previstos | sin pruebas .NET nuevas; verificación = estructura del `dependabot.yml` + suite verde |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 33 (mantenimiento supply-chain): Dependabot (NuGet/Actions/Docker) + checklist de release con validación `-rc` obligatoria. Atiende acciones de S29/S31/S32. Compromete 8 SP; sin lógica de dominio nueva. Generado por AG-07 |
