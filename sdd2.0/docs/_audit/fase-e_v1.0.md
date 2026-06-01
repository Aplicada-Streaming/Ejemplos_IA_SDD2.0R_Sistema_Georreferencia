# Auditoría Fase E — 08 Calidad y pruebas — GeoVial

**Proyecto:** GeoVial
**Documento:** fase-e_v1.0.md
**Categoría auditada:** 08_calidad_y_pruebas (9 archivos)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación de la Fase E)
**Fecha:** 2026-06-01
**Regla de referencia:** `/sdd2.0/devs/rules/08_rules_calidad_y_pruebas.md`
**Método:** lectura exhaustiva de los 9 archivos de la categoría; verificación empírica con conteo de TC y de secciones, regex de IDs (TC de dos dígitos, contigüidad TC-01..26), scan de CRLF/BOM y encoding, búsqueda de vocabulario prohibido del bootstrap, resolución de enlaces relativos del README, y cotejo cruzado contra 02 (CU-01..14 con Given-When-Then, RN-01..08, RC-03), 05 §8 (NFR numéricos, ADR), 06 (Definition of Ready) y 07 (referencias a la DoD en Sprint 00 y Sprint 01). No se reauditó el upstream; solo se verificó la existencia y coherencia de los IDs referenciados.

---

## 1. Resumen ejecutivo

La Fase E de GeoVial está completa, internamente consistente y trazable de punta a punta. Se verificaron empíricamente los 9 archivos de la categoría 08. Todos los criterios bloqueantes se cumplen: los 7 artefactos obligatorios existen con sus secciones completas, más la `guia-testing-extensibilidad` (justificada porque `tiene_extensibilidad: true`, punto de extensión `GeoVial.FileHosting`) y el README. El catálogo de casos de prueba tiene 26 TC contiguos de dos dígitos (TC-01..26); cada TC referencia al menos un CU, RN o NFR real; no hay TC huérfano ni referencia a ID inexistente. La matriz de cobertura tiene las tres tablas obligatorias (CU↔Tests, NFR↔Tests, RN↔Tests) más la tabla de cobertura por capa. Los 14 CU (CU-01..14) quedan cubiertos por al menos un TC que mapea sus criterios Given-When-Then; las 8 RN (RN-01..08) tienen TC; los 7 NFR de 05 §8 tienen test y tooling (disponibilidad observada como SLO en 09, conforme a §4.9). La DoD es canónica, no solapa la DoR de 06 y los sprints de 07 la referencian sin redefinirla. No se detectó vocabulario prohibido del bootstrap (Motor DSL, impresoras térmicas, ESC-POS, Bluetooth de impresión); el tooling real declarado (xUnit, FluentAssertions, WebApplicationFactory, Testcontainers, bUnit, MAUI UI/Appium, Coverlet) es legítimo. Ningún archivo lleva sufijo de dominio; todos siguen `_v1.0.md`. Encoding UTF-8 y line endings LF en los 9 archivos.

La cobertura se reporta por capa (no como número global), con umbrales diferenciados y el gate global de CI (líneas ≥ 80%, branches ≥ 70%) coherente con README §9/§11. La pirámide 70/20/10 está justificada numéricamente contra sus dos degeneraciones. No se hallaron P0 ni P1. Los únicos hallazgos son P2/P3 de naturaleza opcional/estilística y no rompen trazabilidad ni conformidad estructural.

### Conteo de hallazgos por nivel

| Nivel | Cantidad |
| --- | --- |
| P0 bloqueante | 0 |
| P1 alto | 0 |
| P2 medio | 2 |
| P3 bajo | 2 |
| **Total** | **4** |

### Veredicto

**APROBADO CON OBSERVACIONES.** Los entregables son promovibles a 09 (materialización de los quality gates en el pipeline) y a 10 (developer guide de testing). Las observaciones son menores (cobertura por capa con valores `Pendiente` por diseño en v1 y dos detalles estilísticos); ninguna bloquea ni rompe trazabilidad.

---

## 2. Matriz de conformidad D1-D8 por documento

Convenciones: OK = cumple; n/a = no aplica al tipo de documento.

D1 idioma rioplatense con tildes/eñes · D2 UTF-8/LF (sin CRLF, sin BOM) · D3 filenames kebab-case ASCII sin sufijo de dominio · D4 versionado `_v1.0.md` (no `.v1.0.md`) · D5 IDs de dos dígitos uniformes (TC-XX contiguos) · D6 trazabilidad en cabecera/secciones · D7 sin vocabulario prohibido del bootstrap · D8 tipo cerrado (web-monolith).

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `estrategia-calidad_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `estrategia-testing_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `plan-pruebas_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `matriz-cobertura-pruebas_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `casos-prueba-referenciales_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `criterios-validacion_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `definition-of-done_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `guia-testing-extensibilidad_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `README.md` | OK | OK | OK | OK | OK | OK | OK | OK |

Notas de verificación empírica:

- **D2:** los 9 archivos son UTF-8 sin BOM, con line endings LF (scan de `\r`: 0 coincidencias por archivo). `core.autocrlf=true` en el repo no afectó el contenido versionado.
- **D3/D4:** los 9 nombres siguen `<rol>_v1.0.md` o `README.md`; ninguno lleva sufijo de dominio (`-motor`, `-geovial`) ni el patrón heredado `.v1.0`. Corrige explícitamente el déficit del fuente SDD 1.0 (§0 de la regla).
- **D5:** regex `^### TC-\d{2}` sobre el catálogo: TC-01 a TC-26, contiguos, dos dígitos, sin saltos ni IDs de tres dígitos. Slugs kebab en los títulos (p. ej. `TC-01 crear-relevamiento-asignar-agentes-area`).
- **D7:** regex `motor dsl|impresora|térmica|esc-pos|escpos|bluetooth` sobre los 9 archivos: 0 coincidencias. El tooling de testing (xUnit, FluentAssertions, WebApplicationFactory, Testcontainers, bUnit, MAUI UI/Appium, Coverlet) y el stack del proyecto (EF Core, SQL Server, SQLite, Blazor) aparecen legítimamente, conforme a la aclaración del alcance y a README §9.
- **D8:** todos los documentos declaran/asumen `web-monolith` y adoptan la pirámide 70/20/10 de §2.2 para ese tipo.

---

## 3. Matriz de estructura obligatoria

| Artefacto | Obligatorio | Existe | Secciones requeridas | Estado |
| --- | --- | --- | --- | --- |
| `estrategia-calidad_v1.0.md` | Sí (§4.2, 5 secciones) | Sí | Definición de calidad; Atributos ISO/IEC 25010 priorizados con NFR de origen; Quality gates; Roles QA (RACI); Cadencia | OK (5/5 + control de cambios) |
| `estrategia-testing_v1.0.md` | Sí (§4.3, 7 secciones) | Sí | Pirámide 70/20/10 justificada; Cobertura por capa; Tooling; BDD; Mocks/fixtures; Datos de prueba; Ambiente | OK (7/7 + control de cambios) |
| `plan-pruebas_v1.0.md` | Sí (§4.4, 6 secciones) | Sí | Alcance; Entrada; Salida; Riesgos; Plan por sprint; Recursos | OK (6/6 + control de cambios) |
| `matriz-cobertura-pruebas_v1.0.md` | Sí (§4.5) | Sí | Propósito; CU↔Tests; NFR↔Tests; RN↔Tests; Cobertura por capa; Gaps | OK (3 tablas obligatorias + cobertura por capa + gaps) |
| `casos-prueba-referenciales_v1.0.md` | Sí (§4.6) | Sí | TC con id/nombre, tipo, cobertura, setup, pasos GWT, expected, actual, status | OK (26 TC completos) |
| `criterios-validacion_v1.0.md` | Sí (§4.7, 6 secciones) | Sí | Propósito; Funcionales; No funcionales; Regresión; Calidad de código; Excepciones | OK (6/6 + control de cambios) |
| `definition-of-done_v1.0.md` | Sí (§4.8, 3 secciones) | Sí | DoD por capa (US/BT/sprint/release); Excepciones; Vigencia | OK (4 subcapas + 3 secciones + control de cambios) |
| `guia-testing-extensibilidad_v1.0.md` | Sí (tiene_extensibilidad=true) | Sí | Propósito; qué se prueba; contract tests; fixtures; ambiente; conformidad; trazabilidad | OK |
| `README.md` | Recomendado | Sí | Índice; quality gates; enlace a DoD canónica; conteo/cobertura; tooling | OK |

Verificaciones puntuales:

- **estrategia-testing §1:** pirámide con porcentajes 70/20/10 y justificación numérica explícita contra pirámide invertida (e2e pesado) y contra pirámide aplanada (cobertura sin capas). Cumple el requisito de justificación.
- **estrategia-testing §2 / matriz §5:** cobertura por capa con umbrales dominio 85/75, aplicación 80/70, infraestructura 70/60, presentación 60/50. Mutation testing declarado no exigido en v1 para web-monolith (coherente con §2.2 de la regla, que solo lo fija como piso para `library`).
- **casos-prueba-referenciales:** cada TC declara Tipo (unit/integration/e2e/component/contract), Cubre (CU/RN/NFR/ADR), Setup, Pasos Given-When-Then, Expected output, Actual output (`pendiente`) y Status (`pendiente`). Los 8 campos de §4.6 presentes en los 26 TC.
- **definition-of-done:** las cuatro capas US/BT/sprint/release con casillas `- [ ]` y mecanismo de validación entre paréntesis en cada criterio (no hay criterio sin métrica verificable).

---

## 4. Coherencia cross-doc y cross-categoría

### 4.1 Cobertura CU↔TC (14 CU de 02)

Se verificó que cada CU-01..14 aparece en la tabla CU↔Tests de la matriz con al menos un TC, y que el TC existe en el catálogo y referencia ese CU.

| CU | TC en matriz | TC existe en catálogo | TC referencia el CU | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | TC-01, TC-02, TC-03 | Sí | Sí | Cubierto |
| CU-02 | TC-04, TC-05 | Sí | Sí | Cubierto |
| CU-03 | TC-06 | Sí | Sí | Cubierto |
| CU-04 | TC-07, TC-08 | Sí | Sí | Cubierto |
| CU-05 | TC-08, TC-22 | Sí | Sí | Cubierto |
| CU-06 | TC-09 | Sí | Sí | Cubierto |
| CU-07 | TC-10, TC-11, TC-12, TC-13 | Sí | Sí | Cubierto |
| CU-08 | TC-14, TC-15 | Sí | Sí | Cubierto |
| CU-09 | TC-16 | Sí | Sí | Cubierto |
| CU-10 | TC-17 | Sí | Sí | Cubierto |
| CU-11 | TC-18 | Sí | Sí | Cubierto |
| CU-12 | TC-19 | Sí | Sí | Cubierto |
| CU-13 | TC-20 | Sí | Sí | Cubierto |
| CU-14 | TC-21 | Sí | Sí | Cubierto |

14/14 CU cubiertos. Sin CU huérfano. Cada CU del upstream tiene su sección `## 8. Criterios de aceptación` con tabla Given/When/Then (verificado por muestreo en CU-01, CU-07, CU-14); la matriz resume esos criterios y los mapea a TC con cobertura de escenarios CA-XX explícita en el catálogo.

### 4.2 Cobertura NFR↔TC (7 NFR de 05 §8)

| NFR (05 §8) | SLA / objetivo | Test | Tooling | Estado |
| --- | --- | --- | --- | --- |
| Operación sin conexión | ≥ 8 h sin pérdida | TC-09 | MAUI UI/Appium (captura 8 h sobre SQLite) | OK |
| Tiempo de sincronización | ≤ 5 min para ≈100 obs. | TC-10 | Telemetría de sync en integración | OK |
| Latencia API (lecturas adm.) | p95 ≤ 500 ms | TC-24 | WebApplicationFactory + dataset, p95 | OK |
| Detección de conectividad | Automática | TC-23 | UI móvil corte/restablecimiento de red | OK |
| Disponibilidad del backend | SLO 99% | Métrica observada en 09 | Health check + error budget (09) | OK (SLO, conforme §4.9) |
| Confiabilidad de georreferenciación | ≥ 95% coord. automática | TC-07, TC-08 + métrica de campo | Verificación de fuente de coordenada + métrica en 09 | OK |
| Tamaño de foto sincronizada | Compresión/redimensión (límite a validar) | TC-10 (payload antes/después) | Medición de payload en pipeline de sync | OK |

Los 7 NFR de 05 §8 (verificados directamente en la tabla §8 de `arquitectura-solucion_v1.0.md`) tienen test y tooling. La disponibilidad se trata como SLO observado en 09 y la confiabilidad de georreferenciación se complementa con métrica de campo, ambos por su naturaleza estadística en producción, lo que es admisible (regla §4.9). El TC-24 y TC-23 existen en el catálogo y referencian los NFR correspondientes.

### 4.3 Cobertura RN↔TC (8 RN de 02)

| RN | TC en matriz | TC existe | Estado |
| --- | --- | --- | --- |
| RN-01 | TC-21, TC-02, TC-06 | Sí | Cubierta |
| RN-02 | TC-18, TC-07 | Sí | Cubierta |
| RN-03 | TC-08, TC-22 | Sí | Cubierta |
| RN-04 | TC-12, TC-13 | Sí | Cubierta |
| RN-05 | TC-17, TC-03 | Sí | Cubierta |
| RN-06 | TC-05, TC-09 | Sí | Cubierta |
| RN-07 | TC-20 | Sí | Cubierta |
| RN-08 | TC-21, TC-20 | Sí | Cubierta |

8/8 RN cubiertas. Sin RN huérfana.

### 4.4 Integridad de referencias de los TC (sin huérfanos ni IDs inexistentes)

- Los 26 TC declaran línea `Cubre:` con al menos un CU, RN o NFR. Ninguno carece de trazabilidad upstream.
- IDs referenciados verificados contra el upstream: CU-01..14 (existen), RN-01..08 (existen), NFR de 05 §8 (existen), RC-03 (existe como regla conceptual de modelo en `02/.../RC-03-unicidad-identificador-cola-sync_v1.0.md` y en ADR-05/ADR-09), ADR-02/08/11 (existen en `05/adrs`). No se halló ninguna referencia a un ID inexistente.
- La matriz referencia TC-01..TC-24; TC-25 (contrato OpenAPI) y TC-26 (conformidad FileHosting) se trazan desde `criterios-validacion` y `guia-testing-extensibilidad`. Todos los TC referenciados existen en el catálogo. Ningún TC fuera del rango TC-01..26.

### 4.5 Verificación DoD↔DoR↔sprints

- **No solapamiento DoD/DoR:** la DoR de 06 (`definition-of-ready_v1.0.md` §) se declara filtro de entrada ("cuándo un ítem está listo para entrar a un sprint") y exige criterios de aceptación Given/When/Then presentes y estimación. La DoD de 08 se declara filtro de salida ("cuándo un ítem está terminado") y exige que esos criterios estén verificados por tests verdes, cobertura, build, contrato y auditoría. La DoR (06 línea 12) afirma explícitamente que no duplica ni solapa la DoD; la DoD (08 §, cabecera y §1) afirma lo recíproco. No hay solapamiento.
- **DoD canónica única:** `definition-of-done_v1.0.md` se declara fuente canónica única (§3 Vigencia) y el README la nombra como tal.
- **Sprints referencian sin redefinir:** `plan-iteracion-sprint-00 §5` y `plan-iteracion-sprint-01 §5` aplican la DoD canónica por nombre y ubicación (`08_calidad_y_pruebas/definition-of-done_v1.0.md`) y declaran explícitamente "este plan no la redefine". Los criterios "específicos del sprint" que agregan son adicionales, no redefiniciones de los criterios canónicos; la propia DoD §3 admite esa adición. No se configura el anti-patrón "DoD redefinida en cada sprint".

### 4.6 Cobertura por capa y gates

- La cobertura se reporta por capa en estrategia-testing §2 y en matriz §5 (no como número global único). El gate global de CI (líneas ≥ 80%, branches ≥ 70%) se presenta como piso transversal adicional a los pisos por capa, coherente con README §9 y README §11 de los intakes (citados consistentemente en estrategia-calidad §3, estrategia-testing §2, plan-pruebas §3, criterios-validacion §5, definition-of-done §1.3/§1.4). No es el anti-patrón "cobertura como número global": los umbrales por capa prevalecen y la trazabilidad a CU/RN/NFR se declara prioritaria sobre el número.

### 4.7 Enlaces relativos

- Los 8 enlaces relativos del README resuelven a archivos existentes de la categoría (verificado uno a uno). Las referencias cruzadas a `05/extensibilidad_v1.0.md`, ADR-08, US-31, BT-01/BT-02 y `samples/03-filehosting-backends` (11) resuelven contra el upstream existente.

### 4.8 Anti-patrones de §4.10

| Anti-patrón | ¿Presente? | Evidencia |
| --- | --- | --- |
| Cobertura global sin capas | No | Cobertura por capa en estrategia-testing §2 y matriz §5 |
| TC sin trazabilidad | No | Los 26 TC declaran `Cubre:` con CU/RN/NFR |
| Snapshot sin política | No | Dataset/fixtures con regeneración por PR justificado (estrategia-testing §6) |
| E2E pesado | No | E2E acotado al 10% y a flujos críticos (estrategia-testing §1; plan §4 RC-Q7) |
| DoD sin métricas | No | Cada criterio DoD tiene mecanismo de validación entre paréntesis |
| Test sin assert | No | Cada TC declara Expected output verificable |
| Coverage como meta única | No | Trazabilidad declarada por encima del número (estrategia-testing §1) |
| DoD redefinida por sprint | No | Sprints 00/01 referencian sin redefinir (07 §5) |
| Matriz desactualizada | No | Estado `Pendiente` coherente: la suite aún no se ejecutó (v1 antes de los sprints); cadencia de actualización declarada |

---

## 5. Hallazgos enumerados

### P0 bloqueante

Ninguno.

### P1 alto

Ninguno.

### P2 medio

**H-01 (P2) — Cobertura por capa sin valores observados (por diseño en v1).**
- Archivo/sección: `matriz-cobertura-pruebas_v1.0.md` §5; `estrategia-testing_v1.0.md` §2.
- Evidencia: las columnas Líneas/Branches por capa figuran como `Pendiente` porque la suite aún no se ejecutó (los tests se implementan en los sprints de 07). Está declarado como gap esperado (§6) y la matriz documenta su plan de remediación (primera lectura al cierre de Sprint 00). No es el anti-patrón "matriz desactualizada" (no hay tests implementados que la matriz ignore), pero deja la matriz sin números reales en v1.
- Recomendación: completar los valores observados por capa al cierre del Sprint 00 (gates operativos) y consolidar en Sprint 01, como ya prevé el plan de remediación. Mantener el estado `Pendiente` solo mientras no exista suite ejecutada.

**H-02 (P2) — Trazabilidad NFR parcial para dos NFR de naturaleza estadística.**
- Archivo/sección: `matriz-cobertura-pruebas_v1.0.md` §3; `criterios-validacion_v1.0.md` §3.
- Evidencia: "Disponibilidad del backend" (SLO 99%) y "Confiabilidad de georreferenciación" (≥ 95%) no se validan con un test bloqueante de la suite, sino con SLO/métrica observada en 09. Es admisible por §4.9 y está documentado como excepción (criterios-validacion §6, DoD §2), pero la verificación plena del objetivo numérico queda fuera de 08.
- Recomendación: mantener el tratamiento actual (correcto), y asegurar en la Fase 09 que el SLO de disponibilidad y la métrica de ≥ 95% queden materializados como observabilidad con revisión mensual, cerrando el lazo declarado en los gaps.

### P3 bajo

**H-03 (P3) — Líneas muy largas en varios documentos.**
- Archivo/sección: todos los documentos de 08 (longitud de línea de 326 a 723 caracteres por archivo).
- Evidencia: párrafos y celdas de tabla en una sola línea física larga. No afecta el render Markdown ni la trazabilidad; es un detalle de legibilidad del fuente en diff.
- Recomendación: opcional; no requiere acción. Si se desea, aplicar wrapping semántico en una pasada de pulido.

**H-04 (P3) — RC-03 referenciado como ID de cobertura junto a CU/RN/NFR.**
- Archivo/sección: `casos-prueba-referenciales_v1.0.md` (TC-11, TC-13), `plan-pruebas_v1.0.md` §4 (RC-Q2/RC-Q3).
- Evidencia: RC-03 (regla conceptual de modelo de 02) se usa como referencia de cobertura en la línea `Cubre:`. Es un ID upstream real y aporta precisión, pero la regla §3.2 enumera CU/RN/NFR como los IDs de trazabilidad canónicos de 08; RC-XX es un identificador de modelo conceptual, no de los tres tipos nombrados.
- Recomendación: estilístico; mantener RC-03 como referencia complementaria está bien, pero todo TC que lo cita también referencia un CU y/o RN (TC-11→CU-07/RN-04, TC-13→CU-07/RN-04), de modo que la trazabilidad canónica queda igualmente satisfecha. Sin acción obligatoria.

---

## 6. Cumplimiento de §6 de la regla (13 ítems)

| # | Criterio de aceptación §6 | Estado | Evidencia |
| --- | --- | --- | --- |
| 1 | estrategia-calidad con ISO 25010 + quality gates | OK | §2 atributos priorizados con NFR de origen; §3 quality gates |
| 2 | estrategia-testing con pirámide numérica + cobertura por capa + tooling | OK | §1 pirámide 70/20/10; §2 cobertura por capa; §3 tooling |
| 3 | plan-pruebas con entrada/salida + riesgos por sprint | OK | §2/§3 criterios; §4 riesgos; §5 plan por sprint |
| 4 | matriz con las 3 tablas + cobertura por capa | OK | §2 CU↔Tests; §3 NFR↔Tests; §4 RN↔Tests; §5 cobertura por capa |
| 5 | casos-prueba con ≥1 TC por CU crítico, setup/pasos/expected/status | OK | 26 TC; los 14 CU cubiertos; 8 campos por TC |
| 6 | criterios-validacion con criterios numéricos | OK | §2-§5 con SLA y mediciones; §6 excepciones |
| 7 | definition-of-done por capa, criterios verificables, excepciones | OK | §1 US/BT/sprint/release; §2 excepciones; §3 vigencia |
| 8 | guia-testing-extensibilidad si admite plugins | OK | Existe; punto de extensión GeoVial.FileHosting (ADR-08) |
| 9 | Ningún archivo con sufijo de dominio; patrón `_v<X.Y>.md` | OK | 9 nombres conformes |
| 10 | Cada NFR numérico con test en la matriz | OK | §3 NFR↔Tests; 7 NFR con test/tooling |
| 11 | Cada TC referencia ≥1 CU/RN/NFR | OK | Líneas `Cubre:` en los 26 TC |
| 12 | DoD no redefinida en sprints; los sprints la referencian | OK | 07 sprint-00/01 §5 |
| 13 | Cobertura por capa, no número global | OK | estrategia-testing §2; matriz §5 |

13/13 ítems cumplidos.

---

## 7. Veredicto final

**APROBADO CON OBSERVACIONES.**

La categoría 08_calidad_y_pruebas de GeoVial cumple las reglas constructivas, la estructura obligatoria, la trazabilidad cross-doc (CU/RN/NFR↔TC) y la coherencia cross-categoría (DoD canónica no solapada con la DoR de 06 y referenciada sin redefinir por los sprints de 07). No hay hallazgos P0 ni P1. Se promueve a 09 y 10.

Condiciones (no bloqueantes, para la evolución de la versión):

1. Completar los valores observados de cobertura por capa al cierre del Sprint 00 y consolidar en Sprint 01 (H-01), pasando los TC de `Pendiente` a Verde/Rojo en la matriz según la cadencia ya declarada.
2. Asegurar en 09 la materialización del SLO de disponibilidad y de la métrica de ≥ 95% de georreferenciación con revisión mensual (H-02).
3. Hallazgos H-03 y H-04 son opcionales y no requieren acción.

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Auditoría independiente de la Fase E (08_calidad_y_pruebas): conformidad D1-D8, estructura obligatoria de los 7 artefactos + guia-extensibilidad + README, cumplimiento de §6 (13 ítems), coherencia cross-doc (CU/RN/NFR↔TC) y verificación DoD↔DoR/sprints. Veredicto APROBADO CON OBSERVACIONES con 0 P0, 0 P1, 2 P2, 2 P3. |
