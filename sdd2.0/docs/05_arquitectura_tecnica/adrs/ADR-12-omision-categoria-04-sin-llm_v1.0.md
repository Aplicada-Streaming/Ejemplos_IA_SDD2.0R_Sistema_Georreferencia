# ADR-12 — Omisión de la categoría 04 (no se incorpora LLM en v1)

**Proyecto:** GeoVial
**Documento:** ADR-12-omision-categoria-04-sin-llm_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Estilo

## 1. Contexto

La cadena SDD 2.0 contempla la categoría 04 (contratos de prompts) cuando el proyecto delega lógica en un modelo de lenguaje grande (LLM). PROJECT-README §1 declara el flag de gating `usa_llm: false`. Ningún CU (CU-01 a CU-14) ni ninguna RN (RN-01 a RN-08) de GeoVial requiere generación, clasificación o razonamiento por LLM: la georreferenciación deriva de metadatos o ubicación manual (RN-03), la consolidación es last-write-wins determinista (RN-04) y la resolución de conflictos es una decisión humana (CU-12). Se necesita registrar formalmente por qué la categoría 04 se omite y bajo qué condiciones se reevaluaría.

## 2. Decisión

GeoVial no incorpora ningún componente basado en LLM en la v1. La categoría 04 (contratos de prompts) se omite por `usa_llm=false`. Ninguna decisión de arquitectura de la v1 depende de un LLM. La incorporación futura de fuentes automatizadas (drones, estaciones) o de asistencia por IA se trataría como un cambio de alcance que reactivaría la evaluación de la categoría 04.

## 3. Estado

Aceptado el 2026-06-01. ADR de gobernanza que documenta la omisión de la categoría 04.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| No incorporar LLM en v1 (elegido) | Coherente con el alcance; sin costo ni dependencia de un proveedor de IA; determinismo en consolidación y georreferenciación | Ninguna asistencia por IA en v1 |
| Incorporar un LLM para asistencia (p. ej. sugerir etiquetas) | Posible mejora de UX | Fuera de alcance; agrega dependencia, costo y contratos de prompt sin requisito de negocio |
| Reservar puntos de integración para un LLM futuro | Prepara la extensión | Especulativo; no hay requisito que lo motive en v1 |

## 5. Consecuencias positivas

1. Se evita una dependencia y un costo de un proveedor de IA sin requisito que lo justifique.
2. La consolidación y la georreferenciación permanecen deterministas y auditables.
3. La cadena de documentación queda consistente: no se generan artefactos de la categoría 04 vacíos.

## 6. Consecuencias negativas y trade-offs

1. No hay asistencia por IA en la v1; aceptado por alcance.
2. Una futura incorporación de IA requerirá reabrir la categoría 04 y, eventualmente, una ADR nueva que supere consideraciones de esta.

## 7. Implementación

No se generan artefactos de la categoría 04. Los flags de gating (`usa_llm: false`) permanecen registrados en PROJECT-README §1. Si en una versión futura se incorpora una fuente automatizada o asistencia por IA, se eleva un cambio de alcance (alcance-proyecto §9) y se evalúa la categoría 04 con su propia ADR.

## 8. Métricas de validación

- No existen artefactos de la categoría 04 en `docs/` para la v1.
- Ningún componente de la vista lógica depende de un servicio de LLM.

## 9. Referencias

- PROJECT-README §1 (`usa_llm: false`).
- alcance-proyecto §5 (exclusiones), §9 (gestión de cambios de alcance).
- vision-producto §4 (fuentes automatizadas como paso futuro).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. ADR de gobernanza que registra la omisión de la categoría 04 |
