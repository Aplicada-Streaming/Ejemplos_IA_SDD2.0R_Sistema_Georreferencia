# Criterios de validación — GeoVial

**Proyecto:** GeoVial
**Documento:** criterios-validacion_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Ingeniero QA / SDET Senior (AG-08), Equipo SDD 2.0

## 1. Propósito

Define qué significa "GeoVial está validado para release". Un release solo se declara validado cuando se cumplen, de forma medible y en un ambiente equivalente al productivo, los criterios funcionales, no funcionales, de regresión y de calidad de código que siguen. Cualquier criterio no cumplido se acepta únicamente con la excepción documentada de §6. Estos criterios complementan la Definition of Done (capa release) sin redefinirla: la DoD vive en `definition-of-done_v1.0.md`.

## 2. Criterios funcionales

- Cada CU crítico está cubierto por al menos un TC verde que valida sus criterios Given-When-Then (matriz-cobertura-pruebas §2). CU críticos para release v1: CU-01, CU-02, CU-03, CU-04, CU-05, CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12, CU-13, CU-14 (los catorce CU son parte del alcance Must/Should de v1; ver plan-pruebas §1).
- Cada RN-01..08 tiene su TC verde (matriz §4): jerarquía y autorización, agrupación por radio, prioridad de metadatos, last-write-wins con marca de conflicto, estados y solo lectura, precondición de método de seguridad offline, retención de auditoría y tratamiento de datos personales.
- Los flujos de extremo a extremo críticos quedan demostrados: captura georreferenciada offline → encolado → sincronización → consolidación → revisión sobre mapa → auditoría.
- Verificación: suite de tests verde en CI (xUnit, integración, componente, UI móvil) con los TC de la matriz en estado Verde.

## 3. Criterios no funcionales

Cada NFR de 05 §8 cumple su SLA medido en el ambiente de pruebas equivalente al productivo.

| NFR | SLA | Criterio de validación | Test/medición |
| --- | --- | --- | --- |
| Operación sin conexión | ≥ 1 jornada (8 h) sin pérdida | Captura continua 8 h conserva todos los cambios encolados | TC-09 (UI móvil) |
| Tiempo de sincronización | ≤ 5 min para ≈100 observaciones con fotos | Tiempo disparo → vaciado de cola ≤ 5 min | TC-10 (integración) |
| Latencia API (lecturas administrativas) | p95 ≤ 500 ms | p95 medido sobre endpoints de revisión ≤ 500 ms | TC-24 (integración de carga) |
| Detección de conectividad | Automática | La sync se dispara sola al recuperar señal | TC-23 (UI móvil) |
| Disponibilidad del backend | SLO 99% en horario laboral | Error budget mensual respetado | SLO observado en 09 |
| Confiabilidad de georreferenciación | ≥ 95% con coordenada automática | Lógica de fuente de coordenada correcta + métrica de campo ≥ 95% | TC-07/TC-08 + métrica en 09 |
| Tamaño de foto sincronizada | Compresión/redimensión activa | Payload reducido por compresión/redimensión en el pipeline | TC-10 (payload antes/después) |

La validación de NFR la firma el Arquitecto (AG-05) junto con QA (AG-08), sobre ambiente equivalente al productivo (estrategia-calidad §4).

## 4. Criterios de regresión

- La suite de regresión completa se ejecuta y queda verde antes del release.
- Ningún test que estaba verde en la versión anterior pasó a rojo sin justificación documentada (anti-patrón "falta de prueba de regresión", regla §4.10).
- Todo bug cerrado generó al menos un TC nuevo o extendió uno existente que previene su reaparición; ese TC forma parte de la suite de regresión.
- Verificación: comparación del reporte de CI de la versión candidata contra el de la versión anterior; lista de TC de regresión añadidos por bugs cerrados.

## 5. Criterios de calidad de código

- Cobertura por capa cumplida según estrategia-testing §2 (dominio 85/75, aplicación 80/70, infraestructura 70/60, presentación 60/50) y gate global de CI: líneas ≥ 80%, branches ≥ 70% (PROJECT-README §9, §11).
- Build sin warnings tratados como error (gate de build, README §11 stage 2).
- Análisis estático sin warnings nuevos respecto de la línea base; las dependencias cumplen la política de licencias (MIT/Apache/BSD; sin GPL en componentes distribuidos, README §2).
- Contrato OpenAPI 3.x generado coincide con la API y está versionado (TC-25).
- Mutation score no exigido en v1 para `web-monolith` (regla §2.2); no bloquea el release.
- Verificación: reporte de cobertura por capa de Coverlet; salida del stage Build; reporte de análisis estático; validación de OpenAPI en CI.

## 6. Excepciones documentadas

- Cualquier criterio funcional, no funcional, de regresión o de calidad de código no cumplido se acepta para release únicamente con una ADR explícita que registre la decisión, su justificación y su plan de remediación con BT asociado en el backlog.
- Casos previstos en v1 que se gestionan por esta vía o por observación en 09 (no bloquean el release si están documentados):
  - Límite numérico de tamaño de foto sin fijar (supuesto a validar, README §13): se valida el mecanismo de compresión/redimensión; el umbral exacto se incorpora cuando el cliente lo confirme.
  - Disponibilidad (SLO 99%) y confiabilidad de georreferenciación (≥ 95%): métricas estadísticas observadas en 09; su validación plena es continua post-release, no un test bloqueante de la suite.
- Toda excepción la aprueba QA (AG-08) con visto del Arquitecto (AG-05) y queda registrada en la ADR correspondiente de 05.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Criterios de validación iniciales: funcionales (14 CU + 8 RN verdes), no funcionales (cada NFR con SLA y test/medición), regresión, calidad de código (cobertura por capa + gates + análisis estático) y excepciones con ADR. Generados por AG-08 |
