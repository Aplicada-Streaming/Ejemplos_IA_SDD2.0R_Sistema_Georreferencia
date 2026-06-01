# Definition of Done — GeoVial

**Proyecto:** GeoVial
**Documento:** definition-of-done_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Ingeniero QA / SDET Senior (AG-08), Equipo SDD 2.0
**Trazabilidad upstream:** 02 (CU-01..14, RN-01..08), 05 (NFR §8, ADR), 06 (Definition of Ready), 07 (sprint goals)
**Trazabilidad downstream:** 07_plan-sprint (referencia esta DoD; no la redefine), 09_devops (materializa los gates), 10_developer_guide (cita las convenciones de tests)

Este documento es la fuente canónica de la Definition of Done (DoD) de GeoVial. Define cuándo un ítem está terminado, a diferencia de la Definition of Ready de 06, que define cuándo un ítem está listo para empezar. La DoR es el filtro de entrada al Sprint Planning; esta DoD es el filtro de salida. No se solapan: la DoR exige criterios de aceptación Given/When/Then presentes y estimación; la DoD exige que esos criterios estén verificados por tests verdes, con cobertura, build, contrato y auditoría cumplidos. Los sprint plans de 07 referencian esta DoD por nombre y ubicación; no la redefinen (regla §4.10, anti-patrón "DoD redefinida en cada sprint").

Cada criterio se expresa como casilla `- [ ]` verificable mecánicamente: un comando, un check del pipeline o una métrica del reporte. La respuesta a "¿cómo se valida?" está entre paréntesis al final de cada criterio.

## 1. DoD por capa

### 1.1 DoD de US (historia de usuario)

- [ ] El código compila sin warnings tratados como error (stage Build de CI; README §11 stage 2).
- [ ] Cada criterio de aceptación Given/When/Then de la US tiene un TC asociado en la matriz y está verde (reporte de tests de CI; matriz-cobertura-pruebas §2).
- [ ] Los tests unitarios de dominio y aplicación que cubren la US pasan al 100% (stage Tests de CI; xUnit).
- [ ] La cobertura por capa de los archivos tocados respeta los umbrales de estrategia-testing §2 y no baja el gate global (líneas ≥ 80%, branches ≥ 70%) (reporte de Coverlet en CI).
- [ ] Si la US toca la API REST, el documento OpenAPI 3.x regenerado coincide con la implementación y está versionado (validación de OpenAPI en CI; TC-25).
- [ ] Si la US ejerce una RN, el TC de esa RN está verde (matriz §4).
- [ ] Si la US implica acción administrativa o acceso a datos personales, queda registrada en auditoría inmutable (TC-20 / TC-21 verdes; RN-07, RN-08).
- [ ] El cambio se integró por PR a la rama protegida con revisión aprobada (GitHub Flow; README §10).

### 1.2 DoD de BT (tarea técnica)

- [ ] El código compila sin warnings tratados como error (stage Build de CI).
- [ ] Los criterios de aceptación técnicos de la BT se cumplen y son verificables (tests pasan / contrato respetado / deuda saldada) (reporte de CI).
- [ ] Existen tests automatizados para la lógica que introduce la BT (unit y/o integration según corresponda) y están verdes (stage Tests de CI).
- [ ] La cobertura por capa de la BT respeta los umbrales y no baja el gate global de CI (reporte de Coverlet).
- [ ] Si la BT es de infraestructura compartida, su ADR de origen está enlazada y vigente (referencia a 05; DoR de BT §2.1 de 06).
- [ ] Si la BT modifica el contrato REST o el contrato de una librería publicada, el contrato versionado se actualiza y los contract tests pasan (TC-25 / TC-26; SemVer, README §10).
- [ ] Si es spike, su caja temporal se respetó y el resultado quedó documentado para alimentar el refinamiento (nota en la BT).

### 1.3 DoD de sprint

- [ ] Todas las US y BT comprometidas cumplen su DoD de US/BT respectiva (tablero del sprint; reporte de CI por ítem).
- [ ] El pipeline de CI completo está verde en la rama del sprint: build, tests, cobertura y contrato (CI verde sobre el merge final).
- [ ] El gate de cobertura del sprint se cumple: líneas ≥ 80%, branches ≥ 70%, con los pisos por capa (reporte de Coverlet; README §9/§11).
- [ ] El incremento es demostrable end-to-end en el sprint review (demo registrada en el template de sprint review de 07).
- [ ] La matriz de cobertura está actualizada con el estado real de los TC del sprint (Pendiente → Verde/Rojo) (matriz-cobertura-pruebas §2-§4).
- [ ] Toda deuda técnica aceptada está registrada como BT explícito en el backlog (product-backlog / backlog-tecnico de 06).
- [ ] Los TC de regresión por bugs cerrados en el sprint están añadidos y verdes (suite de regresión en CI; regla §4.10).

### 1.4 DoD de release

- [ ] Todos los criterios de validación de `criterios-validacion_v1.0.md` se cumplen (funcionales, no funcionales, regresión y calidad de código) (reporte de validación de release).
- [ ] Cada CU crítico (CU-01..14) está cubierto por al menos un TC verde (matriz §2).
- [ ] Cada RN-01..08 tiene su TC verde (matriz §4).
- [ ] Cada NFR numérico de 05 §8 cumple su SLA en ambiente equivalente al productivo (TC-09, TC-10, TC-23, TC-24; SLO de disponibilidad y métrica de georreferenciación observados en 09).
- [ ] La suite de regresión completa está verde y ningún test verde previo pasó a rojo sin justificación (comparación de reportes de CI).
- [ ] El build de imágenes Docker (front, backend, base de datos) y el empaquetado de la librería de sincronización están verdes (stages 4-6 de CI; README §11).
- [ ] La versión se calculó con SemVer + Conventional Commits y todo breaking change de la API pública de la librería bumpeó MAJOR (MinVer/Nerdbank; README §10).
- [ ] Las excepciones a cualquier criterio están documentadas con ADR y plan de remediación (criterios-validacion §6).

## 2. Excepciones admitidas

- Se puede declarar Done un ítem sin cumplir un criterio solo si la desviación queda registrada como deuda técnica con un BT explícito en el backlog de 06 y, cuando afecta un criterio de release, con una ADR en 05 que incluya plan de remediación.
- Casos previstos en v1 que no bloquean el cierre si están documentados: límite numérico de tamaño de foto sin fijar (supuesto a validar, README §13); disponibilidad (SLO 99%) y confiabilidad de georreferenciación (≥ 95%) como métricas estadísticas observadas en 09, no como tests bloqueantes de la suite.
- Toda excepción la aprueba QA (AG-08); las de capa release requieren visto del Arquitecto (AG-05).

## 3. Vigencia

- Este documento es la fuente canónica única de la DoD de GeoVial. Los planes de sprint de 07 (Sprint 00 y Sprint 01) referencian esta DoD por nombre y ubicación (`08_calidad_y_pruebas/definition-of-done_v1.0.md`) y solo agregan criterios específicos del sprint, sin redefinir los criterios canónicos.
- Cualquier cambio en los criterios versionables de esta DoD se registra en el control de cambios de abajo y se comunica al equipo en el sprint review siguiente (regla §3.4).
- La DoD canónica se acuerda y formaliza con el equipo durante el Sprint 00 (entregable de arranque, plan-iteracion-sprint-00 §5).

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | DoD canónica inicial: criterios verificables mecánicamente por capa (US, BT, sprint, release), excepciones con BT/ADR y vigencia como fuente única referenciada por 07. No solapa la DoR de 06. Generada por AG-08 |
