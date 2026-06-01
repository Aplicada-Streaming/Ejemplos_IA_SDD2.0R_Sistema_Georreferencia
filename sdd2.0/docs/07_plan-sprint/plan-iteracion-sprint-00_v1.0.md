# Plan de Iteración — Sprint 00

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-00_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha inicio:** 2026-06-02
**Fecha fin:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 1 semana (5 días hábiles). Sprint corto justificado por tratarse del sprint de arranque dedicado al walking skeleton end-to-end (PROJECT-README §4 fase 1; §3.2 de las reglas habilita el sprint corto cuando el equipo arranca y valida un esqueleto navegable para reducir riesgo en una fase exploratoria).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), conforme `equipo_n: 4` de PROJECT-README §1.
- Unidad de estimación: story points (Fibonacci), heredada de 06; no se re-estima en el sprint.
- Capacidad declarada: 30 story points. Factor de focus conservador por ser sprint inaugural (curva de aprendizaje, alta del entorno, acuerdo de DoD); se ajustará al cierre con la velocity efectiva registrada.

Tabla de capacidad del equipo (semana de 5 días hábiles, jornada de 6 h efectivas):

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 60 | 0,65 | 39 h |
| Dev frontend | 1 | 30 | 0,60 | 18 h |
| QA | 1 (part-time) | 15 | 0,60 | 9 h |

La capacidad en horas (≈66 h efectivas) se traduce a la capacidad declarada en puntos (30 SP) con factor de focus inaugural conservador; el ratio horas/punto se recalibra al cierre del sprint con la velocity efectiva.

## 2. Objetivo del sprint

Disponer de un walking skeleton end-to-end navegable —monorepo con capas Clean Architecture, persistencia real y API REST versionada— que verifique en CI el camino completo desde un command hasta la base de datos sobre el cual se construirán los slices funcionales.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-09 | Backlog técnico | Scaffolding del monorepo y capas Clean Architecture con mediador CQRS | Alta | 8 | Dev backend A | Pendiente |
| BT-01 | Backlog técnico | Modelo de dominio de usuarios, jerarquía y áreas | Alta | 5 | Dev backend B | Pendiente |
| BT-07 | Backlog técnico | Persistencia backend con EF Core y SQL Server | Alta | 8 | Dev backend A | Pendiente |
| BT-18 | Backlog técnico | API REST versionada con OpenAPI y Problem Details | Alta | 8 | Dev backend B | Pendiente |

Total de puntos comprometidos: 29 SP (dentro de la capacidad declarada de 30 SP).

## 4. Alcance técnico

Componentes que se construyen, en orden de dependencias (referidos a la arquitectura de 05, sin redefinirla):

1. BT-09 — Levanta los proyectos del monorepo de PROJECT-README §5 (`GeoVial.Domain`, `GeoVial.Application`, `GeoVial.Infrastructure`, `GeoVial.Api`, `GeoVial.Web`, `GeoVial.Shared`, más los esqueletos de sub-proyectos) y la separación de capas con dependencias apuntando hacia Domain; configura el mediador CQRS ligero que despacha un command de prueba. Habilita el pipeline de CI inicial (restore, build sin warnings tratados como error, tests). No tiene dependencias previas.
2. BT-07 — Materializa la persistencia con EF Core sobre SQL Server local, con pruebas de integración mediante Testcontainers. Depende de BT-09.
3. BT-18 — Expone la API REST bajo `/api/v1/` con OpenAPI 3.x generado y validado en CI y errores en `application/problem+json`. Depende de BT-09.
4. BT-01 — Define las entidades Usuario y Área en Domain con el enum de rol, testeable sin infraestructura. Depende de BT-09.

El recorrido del walking skeleton encadena BT-09 → BT-07 → BT-18 con BT-01 aportando el primer fragmento de dominio navegable. La arquitectura de capas, persistencia y API se gobierna por 05 (`arquitectura-solucion_v1.0.md`, `contratos-rest_v1.0.md`) y las ADR de §8.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica del proyecto, que reside en `08_calidad_y_pruebas/definition-of-done_v1.0.md` (categoría 08, aún por generar). Este plan no la redefine: la referencia por nombre y ubicación prevista.

Criterios específicos del Sprint 00, adicionales a la DoD canónica:

- El walking skeleton compila y queda verde en el pipeline de CI inicial.
- El mediador CQRS despacha correctamente un command de prueba de extremo a extremo.
- La DoD canónica del proyecto queda acordada y formalizada por el equipo durante este sprint (entregable de arranque del Sprint 0 según §2.2 de las reglas).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El entorno de desarrollo íntegramente local (SQL Server local, contenedores no validados hasta fases finales, PROJECT-README §16) genera fricción de alta en el equipo durante el sprint inaugural | Alta | Medio | Día 1 dedicado a la puesta a punto del entorno y a un `.env.example` versionado; checklist de arranque compartido; capacidad declarada al factor de focus conservador para absorber la curva |
| El acoplamiento entre BT-09, BT-07 y BT-18 puede bloquear el camino del walking skeleton si el scaffolding de capas se retrasa | Media | Alto | Priorizar BT-09 como primer entregable y mergearlo apenas verde en CI; arrancar BT-01 (sin infraestructura) en paralelo para no detener al equipo; spike de integración EF Core + Testcontainers el día 2 |
| Las pruebas de integración con Testcontainers (SQL Server) pueden ser inestables en los entornos locales del equipo | Media | Medio | Validar la imagen de Testcontainers en cada máquina el día 1; fallback documentado a una instancia SQL Server local compartida si el contenedor no levanta |

## 7. Criterios de hecho del sprint

El Sprint 00 se considera completo cuando: todas las BT comprometidas están en estado terminado según la DoD canónica; el walking skeleton end-to-end queda verde en el pipeline de CI; existe un esqueleto navegable (command → handler → persistencia → API) demostrable en el sprint review; la DoD canónica queda acordada por el equipo; y se facilitan el sprint review y la retrospectiva con sus artefactos completados a partir de las plantillas de la sección.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que empiezan a habilitarse | CU-03 (administración de jerarquía, dominio de usuarios y áreas habilitado por BT-01); base de infraestructura transversal a CU-01..CU-14 a través de BT-09 y BT-18 |
| NB que avanzan | NB-01 (delegación de la recolección: se habilita el dominio de jerarquía y usuarios); NB-06 (trazabilidad: la API versionada con Problem Details y la persistencia auditable sientan la base) |
| ADRs que gobiernan | ADR-01 (estilo monolito modular Clean Architecture + CQRS), ADR-10 (separación de capas), ADR-12 (omisión de la categoría 04 sin LLM), ADR-09 (persistencia SQL Server + EF Core), ADR-02 (backend expone API REST), ADR-11 (manejo de errores Problem Details) |

Nota: el Sprint 00 no entrega valor de negocio pleno; entrega un esqueleto navegable que habilita el primer slice funcional del Sprint 01.

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 00 (walking skeleton). Compromete BT-09, BT-01, BT-07 y BT-18 de EP-T1 y la infraestructura de soporte. Sprint corto de 1 semana justificado por arranque. Generado por AG-07 |
