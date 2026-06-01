# Auditoría Fase D — 06 Backlog técnico y 07 Plan de sprint — GeoVial

**Proyecto:** GeoVial
**Documento:** fase-d_v1.0.md
**Categorías auditadas:** 06_backlog-tecnico (recursivo, incluye `historias-usuario/`) y 07_plan-sprint
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación de la Fase D)
**Fecha:** 2026-06-01
**Reglas de referencia:** `/sdd2.0/devs/rules/06_rules_backlog_tecnico.md` y `/sdd2.0/devs/rules/07_rules_plan_sprint.md`
**Método:** lectura exhaustiva de los 38 archivos de ambas categorías (3 maestros de 06 + 32 US individuales + README de 06; 2 planes de sprint + 2 plantillas + velocidad + README de 07); verificación empírica con conteo de secciones, regex de IDs (dos vs tres dígitos), detección de doble separador y apertura con `--`, búsqueda de vocabulario prohibido del bootstrap, scan de CRLF y BOM, resolución de enlaces relativos; cotejo cruzado contra 01 (NB-01..06), 02 (CU-01..14, RN, RC), 05 (ADR-01..14, componentes, contratos) y los intakes (README §4 delivery, `equipo_n: 4`). No se reauditó el upstream; solo se verificó la existencia y coherencia de los IDs referenciados.

---

## 1. Resumen ejecutivo

La Fase D de GeoVial está completa, internamente consistente y trazable de punta a punta. Se verificaron empíricamente los 38 archivos exigidos. Todos los criterios bloqueantes se cumplen: no hay US huérfana de CU (las 32 US referencian al menos un CU real de CU-01..14), ninguna BT carece de upstream (las 22 BT declaran ADR/componente/contrato de 05 más una US consumidora o justificación de infraestructura compartida), todos los IDs de los sprints existen en el backlog de 06 y sus estimaciones coinciden, no hay IDs de tres dígitos, no hay doble separador en nombres de sprint, ningún archivo abre con `--`, no se detectó vocabulario prohibido del bootstrap en US ni en el product-backlog, la distribución MoSCoW no es 100 % Must (23/6/3), los sprint goals son una sola frase, no hay mini-plan (el equipo es de 4) y existen todos los documentos y secciones obligatorias.

Los únicos hallazgos son de severidad P2/P3 y se concentran en texto narrativo desactualizado del `product-backlog_v1.0.md` (dos cifras heredadas que contradicen las tablas correctas del propio documento) y detalles estilísticos. Ninguno rompe trazabilidad ni conformidad estructural.

### Conteo de hallazgos por nivel

| Nivel | Cantidad |
| --- | --- |
| P0 bloqueante | 0 |
| P1 alto | 0 |
| P2 medio | 2 |
| P3 bajo | 2 |
| **Total** | **4** |

### Veredicto

**APROBADO CON OBSERVACIONES.** Los entregables se pueden promover a 07/08. Las observaciones son menores (dos cifras narrativas desactualizadas en el product-backlog y dos detalles estilísticos); ninguna bloquea ni rompe trazabilidad. Se recomiendan como pulido para la siguiente versión del product-backlog.

---

## 2. Matriz de conformidad D1-D8 por documento

Convenciones: OK = cumple; n/a = no aplica al tipo de documento.

D1 idioma rioplatense con tildes/eñes · D2 UTF-8/LF (sin CRLF, sin BOM) · D3 filenames kebab-case ASCII · D4 versionado `_v1.0.md` (no `.v1.0.md`) · D5 IDs de dos dígitos uniformes (US-XX/BT-XX/EP-XX; sprint sin doble separador) · D6 trazabilidad en cabecera/secciones · D7 sin vocabulario prohibido del bootstrap · D8 tipo cerrado (web-monolith, equipo n=4).

### 2.1 Documentos maestros de 06 y 07

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `06/product-backlog_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `06/backlog-tecnico_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `06/definition-of-ready_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `06/README.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `07/plan-iteracion-sprint-00_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `07/plan-iteracion-sprint-01_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `07/template-sprint-review_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `07/template-sprint-retrospectiva_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `07/velocidad-equipo_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `07/README.md` | OK | OK | OK | OK | OK | OK | OK | OK |

Nota D5 para 07: ambos planes usan el patrón único `plan-iteracion-sprint-XX_v1.0.md` (regex de doble separador `plan-iteracion_sprint`: 0 coincidencias). Ningún archivo abre con `--` (scan de primera línea: 0 coincidencias). Nota D7: regex `motor dsl|impresora|térmica|esc-pos|bluetooth` sobre ambas carpetas: 0 coincidencias. El stack real (EF Core, SQL Server, MAUI, Leaflet, SQLite, ROPC/JWT) aparece legítimamente en las BT y en el alcance técnico §4 de los sprints (referenciando ADR/componentes de 05); en las US y en el product-backlog el lenguaje es de valor de negocio, conforme a la aclaración del alcance.

### 2.2 Las 32 US individuales — agrupadas, con excepciones señaladas

Las 32 US comparten el mismo perfil de conformidad; no hay excepciones por documento. Verificación empírica:

| Criterio | Resultado sobre las 32 US |
| --- | --- |
| D1 tildes/eñes | OK en las 32 |
| D2 UTF-8/LF, sin BOM | OK en las 32 (scan CRLF: ninguno; scan BOM: ninguno) |
| D3 filename kebab-case ASCII | OK; slug lowercase estricto en las 32 (regex de mayúscula tras `US-XX-`: 0 coincidencias) |
| D4 `_v1.0.md` (no `.v1.0.md`) | OK en las 32 |
| D5 ID de dos dígitos US-01..US-32 | OK; secuencia continua y uniforme; regex `US-[0-9]{3}`: 0 coincidencias |
| D6 trazabilidad (NB/CU/BT en §4) | OK en las 32; cada una declara ≥1 CU |
| D7 sin vocabulario prohibido | OK en las 32 |
| D8 cabecera con Épica/MoSCoW/Estimación | OK en las 32 |
| Estructura: 7 secciones §4.4 | OK; conteo de H2 = 7 en las 32 (sin desvíos) |
| ≥2 escenarios Given/When/Then | OK; mínimo 2 en todas (3 en US-01/04/05/09/11/16/18/19/25/26/29/31) |

No se detectaron excepciones: las 32 US presentan exactamente las 7 secciones obligatorias y al menos dos escenarios Given/When/Then, incluidas las Should y Could (que igualmente respetan el mínimo de dos).

---

## 3. Matriz de estructura obligatoria

| Requisito | Regla | Verificación empírica | Resultado |
| --- | --- | --- | --- |
| product-backlog: 5 secciones §4.2 + épicas EP-XX | 06 §4.2 | Objetivos/MVP, Épicas (EP-01..EP-09), Historias por épica, Métricas de avance, Refinamiento (+ Control de cambios) | OK |
| backlog-tecnico: 3 secciones §4.3 + matriz BT↔US↔CU | 06 §4.3 | Épicas técnicas (EP-T1..EP-T6), BT por épica, Trazabilidad BT↔US↔CU completa | OK |
| DoR: 5-8 criterios US, 4-6 BT, excepciones, aprobador, sin solapar DoD de 08 | 06 §4.6 | 7 criterios US (en rango), 5 criterios BT (en rango), excepciones (spike, Could dependiente de 03), aprobador AG-06; declara explícitamente que la DoD vive en 08 y no la duplica | OK |
| US individuales >20 → en `historias-usuario/` | 06 §3.3 | 32 US > 20 → 32 archivos individuales presentes | OK |
| Cada US: 7 secciones §4.4 | 06 §4.4 | Historia/Contexto/Criterios/Trazabilidad/Prioridad-estimación/DoR check/Notas en las 32 | OK |
| BT inline o individual según umbral (>30) | 06 §3.3 | 22 BT < 30 → inline en backlog-tecnico, con fuente, dependencias, criterios y trazabilidad | OK |
| 07: equipo de 4 → sprint plan completo (no mini-plan) | 07 §2.2 | `equipo_n: 4`; existen los 4 artefactos completos; no existe `mini-plan_v1.0.md` | OK |
| Sprint 00 y Sprint 01 con 9 secciones §4.2 | 07 §4.2 | Ambos planes con las 9 secciones (Info general, Objetivo, Comprometidas, Alcance técnico, DoD aplicada, Riesgos, Criterios de hecho, Trazabilidad, Control de cambios) | OK |
| Sprint goal de UNA frase (no lista) | 07 §4.2 §4.7 | §2 de ambos planes: 0 bullets, frase declarativa única | OK |
| Template review: 7 secciones §4.3 | 07 §4.3 | Objetivo/resultado, Demos, Feedback, Métricas, Items vs comprometidos, Carry-over, Decisiones | OK |
| Template retro: 5 secciones §4.4 | 07 §4.4 | Qué salió bien, Qué no, Qué probar, Acciones (obligatoria), Seguimiento previo | OK |
| Velocidad: 4 secciones §4.5 con promedio móvil 3 sprints | 07 §4.5 | Por sprint, Tendencia, Capacidad ajustada, Outliers; columna de promedio móvil 3 sprints presente (poblada desde S03 por diseño de la métrica) | OK |
| Riesgos: mínimo 2 por sprint | 07 §4.2 | Sprint 00: 3 riesgos; Sprint 01: 3 riesgos | OK |
| Retro produce ≥1 acción con responsable y fecha | 07 §4.4 | La plantilla obliga la tabla §4 con responsable y fecha; nota de prohibición de retro sin acciones | OK |

---

## 4. Coherencia cross-doc y cross-categoría

### 4.1 Cobertura US ↔ CU (sin huérfanas)

Las 32 US declaran al menos un CU en §4. Todos los CU referenciados existen en `02/casos-de-uso/` (CU-01..CU-14). Además, la unión de los CU referenciados cubre los 14 CU de la especificación funcional (completitud funcional ascendente).

| CU | US que lo cubren |
| --- | --- |
| CU-01 | US-06, US-07, US-08 |
| CU-02 | US-04, US-05 |
| CU-03 | US-01, US-02, US-03 |
| CU-04 | US-11, US-12 |
| CU-05 | US-13, US-14 |
| CU-06 | US-16, US-17, US-32 |
| CU-07 | US-18, US-19, US-32 |
| CU-08 | US-21, US-23, US-27, US-28 |
| CU-09 | US-15, US-22, US-24 |
| CU-10 | US-09, US-10, US-20 |
| CU-11 | US-25 |
| CU-12 | US-26 |
| CU-13 | US-29, US-30 |
| CU-14 | US-31 |

Resultado: sin US huérfana de CU; sin CU sin US consumidora. **OK.**

### 4.2 BT ↔ (US / upstream)

Las 22 BT (BT-01..BT-22, secuencia continua de dos dígitos) declaran fuente upstream real de 05 (ADR-01..14, componentes de la vista lógica, contratos-rest, contratos-abstractions-sync, modelo-datos-lógico) y al menos una US consumidora. Las dos BT transversales se justifican como infraestructura compartida con ADR explícita:

- BT-09 (scaffolding y capas): infraestructura compartida, ADR-01/ADR-10/ADR-12; además lista US representativas.
- BT-18 (API REST + OpenAPI + Problem Details): infraestructura compartida, ADR-02/ADR-11; además lista US representativas.

La matriz BT↔US↔CU del backlog-tecnico (§3) es completa: cada fila declara US consumidoras, CU upstream y la fuente de 05. El máximo ID de US citado en la matriz es US-32 (dentro de rango). Las prioridades MoSCoW de las BT son coherentes con sus US consumidoras (BT-17 y BT-22 Should por sostener US Should; el resto Must). **OK.**

### 4.3 Distribución MoSCoW

Conteo empírico sobre los 32 archivos US (campo `Prioridad MoSCoW`): Must 23, Should 6, Could 3 → 71,9 % / 18,8 % / 9,4 %. Coincide con la tabla §4 del product-backlog y con la tabla del README de 06. No es 100 % Must; hay reparto real. **OK** (con salvedad narrativa, ver Hallazgo H-01).

### 4.4 IDs de los sprints existen en 06 y estimaciones coinciden

| Sprint | Item | ¿Existe en 06? | SP en 06 | SP en el sprint | Coincide |
| --- | --- | --- | --- | --- | --- |
| 00 | BT-09 | sí | 8 | 8 | OK |
| 00 | BT-01 | sí | 5 | 5 | OK |
| 00 | BT-07 | sí | 8 | 8 | OK |
| 00 | BT-18 | sí | 8 | 8 | OK |
| 01 | US-01 | sí | 8 | 8 | OK |
| 01 | US-02 | sí | 3 | 3 | OK |
| 01 | US-04 | sí | 5 | 5 | OK |
| 01 | US-05 | sí | 5 | 5 | OK |
| 01 | US-31 | sí | 8 | 8 | OK |
| 01 | BT-02 | sí | 3 | 3 | OK |
| 01 | BT-08 | sí | 8 | 8 | OK |

Ningún ID inventado. Totales declarados consistentes: Sprint 00 = 29 SP, Sprint 01 = 40 SP, ambos coinciden con `velocidad-equipo_v1.0.md`. Las dependencias upstream citadas en los sprints (BT-01, BT-09, BT-18 para el Sprint 01) existen y están entregadas en el Sprint 00. **OK.**

### 4.5 Sprint 01 end-to-end y Sprint 00 walking skeleton

- Sprint 00: walking skeleton (monorepo, capas Clean Arch, persistencia, API REST versionada), 1 semana justificada como sprint corto de arranque (§3.2). Coherente con la fase 1 de README §4. No entrega valor de negocio pleno (declarado en §8). **OK.**
- Sprint 01: primer slice end-to-end de jerarquía y usuarios; traza a CU-03, CU-02, CU-14 y a NB-01, NB-06 reales; gobernado por ADR-03, ADR-14, ADR-10, ADR-01 existentes. Entrega valor end-to-end (backend + front web + base). **OK.**

### 4.6 Enlaces relativos

Todos los enlaces de los README de 06 y 07 resuelven (product-backlog, backlog-tecnico, DoR, `historias-usuario/`, los 5 artefactos de 07, y las referencias cruzadas a `../06`, `../02`, `../05/adrs`). **OK.**

---

## 5. Hallazgos enumerados

### H-01 (P2) — Cifras narrativas desactualizadas en el product-backlog: MoSCoW "65,6/21,9/12,5"

- **Archivo:** `06_backlog-tecnico/product-backlog_v1.0.md`
- **Sección:** §6 Control de cambios (línea 144).
- **Evidencia:** la fila de control de cambios declara "MoSCoW 65,6/21,9/12,5", pero la distribución real verificada sobre los 32 archivos US y la tabla §4 del propio documento es 71,9/18,8/9,4 (23 Must / 6 Should / 3 Could). El README de 06 también registra 71,9/18,8/9,4. La cifra del control de cambios es un residuo de una versión previa del backlog (probablemente 21 Must / 7 Should / 4 Could sobre otro total) que no se actualizó.
- **Recomendación:** corregir la fila de control de cambios a "MoSCoW 71,9/18,8/9,4" para que el resumen histórico coincida con la tabla vigente. No afecta trazabilidad ni la priorización real.

### H-02 (P2) — Recuento de US incorrecto en el cuerpo del product-backlog: "31 US"

- **Archivo:** `06_backlog-tecnico/product-backlog_v1.0.md`
- **Sección:** §3 Historias por épica (línea 36).
- **Evidencia:** el párrafo introductorio de §3 dice "por superar el umbral de 20 US (§3.3 de las reglas; 31 US)", pero el total real es 32 US (US-01..US-32). El resto del documento es coherente con 32: la métrica §4 ("Total de US: 32"), el control de cambios ("32 US derivadas de los 14 CU"), la nota de numeración (línea 115: "US-01 a US-32") y el README de 06 ("US: 32"). El "31" es un dato aislado y contradictorio dentro del propio archivo.
- **Recomendación:** corregir "31 US" a "32 US" en la línea 36. El umbral de archivos individuales se cumple igual (32 > 20), de modo que la decisión estructural no cambia.

### H-03 (P3) — Conteo de épicas técnicas vs. de producto sin etiqueta explícita en algunas referencias

- **Archivo:** `06_backlog-tecnico/backlog-tecnico_v1.0.md`
- **Sección:** cabecera/§4 Control de cambios.
- **Evidencia:** el documento maneja correctamente 9 épicas de producto (EP-01..EP-09) y 6 técnicas (EP-T1..EP-T6); el README de 06 las distingue de forma clara. Es coherente, pero la convivencia de `EP-XX` (producto) y `EP-T<n>` (técnica) podría confundir a un lector externo que busque "EP-XX de dos dígitos" según §3.2 de las reglas. Las épicas técnicas usan el sufijo `-T<n>` (un dígito), no `EP-0X`; esto es una convención propia coherente internamente, pero no idéntica al patrón `EP-XX` de dos dígitos de la regla.
- **Recomendación:** opcional. Documentar en una nota de la sección que el prefijo `EP-T<n>` es deliberado para distinguir épicas técnicas de épicas de producto y que la regla de dos dígitos aplica a las épicas de producto `EP-XX`. Sin impacto en trazabilidad.

### H-04 (P3) — Estimación en cabecera del Sprint 01 (42 SP) vs. capacidad en horas (≈145,5 h) sin ratio explícito

- **Archivo:** `07_plan-sprint/plan-iteracion-sprint-01_v1.0.md`
- **Sección:** §1 Información general.
- **Evidencia:** la traducción de ≈145,5 h efectivas a 42 SP declarados se hace "con factor de focus conservador" sin exponer el ratio horas/punto, igual que en el Sprint 00 (≈66 h → 30 SP). Es una práctica aceptable en sprints inaugurales sin velocity histórica (y el propio plan dice que se recalibra desde S03), pero el lector no puede reconstruir la conversión. No es un incumplimiento: §4.2 exige declarar la unidad (lo hace: story points Fibonacci) y la capacidad (la declara).
- **Recomendación:** opcional. Anotar el ratio horas/punto asumido (por ejemplo "≈3,5 h/punto inaugural") para hacer auditable la conversión hasta que la velocity efectiva lo reemplace.

---

## 6. Verificación de los §6 (12 + 12 ítems)

### 6.1 — 06_rules §6

| # | Criterio | Resultado |
| --- | --- | --- |
| 1 | product-backlog con 5 secciones §4.2 y épicas EP-XX | OK |
| 2 | backlog-tecnico con 3 secciones §4.3 y matriz BT↔US↔CU completa | OK |
| 3 | DoR con 5-8 criterios US (7) y 4-6 BT (5), excepciones y aprobador | OK |
| 4 | IDs US/BT/EP de dos dígitos uniformes; sin BT-001 heredado | OK |
| 5 | Cada US con ≥1 CU; sin huérfanas | OK |
| 6 | Cada BT con fuente upstream y ≥1 US consumidora o infraestructura compartida | OK |
| 7 | MoSCoW no 100 % Must (23/6/3) | OK |
| 8 | US Must y Should con Given/When/Then, ≥2 escenarios | OK |
| 9 | >20 US → archivos individuales; >30 BT → individuales (22 BT inline, correcto) | OK |
| 10 | Ningún archivo `.v<X.Y>.md`; todos `_v<X.Y>.md` | OK |
| 11 | DoR no solapa la DoD de 08 (declarado explícitamente) | OK |
| 12 | Sin stacks/productos/protocolos del dominio fuente en US y product-backlog | OK |

### 6.2 — 07_rules §6

| # | Criterio | Resultado |
| --- | --- | --- |
| 1 | plan-iteracion-sprint-XX para Sprint 0 y Sprint 1 con 9 secciones | OK |
| 2 | Sprint goal una sola frase, sin bullets | OK |
| 3 | Existen template review y template retro reusables | OK |
| 4 | velocidad-equipo con tabla por sprint y promedio móvil 3 sprints | OK |
| 5 | Cada plan referencia la DoD canónica de 08 y agrega solo lo específico | OK |
| 6 | Cada plan declara trazabilidad a CU y NB en §8 | OK |
| 7 | Cada plan con ≥2 riesgos y mitigación concreta (3 y 3) | OK |
| 8 | Retro produce ≥1 acción con responsable y fecha (plantilla lo obliga) | OK |
| 9 | Ningún archivo con doble separador `plan-iteracion_sprint-XX` | OK |
| 10 | Ningún archivo abre con `--` previo al H1 | OK |
| 11 | Sin stacks/productos/protocolos del dominio fuente fuera de §4 y BT | OK |
| 12 | Equipo de 4 → no hay mini-plan; existen los 4 artefactos | OK |

---

## 7. Anti-patrones (§4.8 de 06 y §4.7 de 07)

| Anti-patrón | Detectado |
| --- | --- |
| US sin valor explícito (cláusula `para` vacía) | No |
| BT que en realidad es una US | No |
| DoR sin criterios verificables | No (los 7+5 son sí/no) |
| Estimación sin técnica declarada | No (Fibonacci declarada y mantenida) |
| Backlog sin refinement con cadencia | No (§5 declara cadencia) |
| IDs heterogéneos (US-01 con BT-001) | No |
| Todo Must Have | No (23/6/3) |
| US huérfanas de CU | No |
| BT sin US consumidora | No (las 2 transversales justificadas) |
| Sprint goal vago o como lista | No |
| Sprint sin DoD aplicada | No (referencia a 08) |
| Retrospectiva sin acciones | No (plantilla lo prohíbe) |
| Doble separador / apertura con `--` | No |

---

## 8. Veredicto final

**APROBADO CON OBSERVACIONES.**

La Fase D (06 backlog técnico y 07 plan de sprint) de GeoVial cumple la totalidad de los criterios bloqueantes y de los 24 ítems de §6 de ambas reglas. La trazabilidad es completa y bidireccional: US↔CU sin huérfanas, BT↔upstream(05)+US, matriz cruzada completa, IDs de los sprints existentes en 06 con estimaciones coincidentes, sprint goals de una frase, y cero rastros del vocabulario o las convenciones heredadas del bootstrap. Se puede promover a 07/08 sin reproceso.

Condiciones (no bloqueantes, recomendadas para la próxima revisión del product-backlog):

1. Corregir la cifra MoSCoW del control de cambios del product-backlog a 71,9/18,8/9,4 (H-01).
2. Corregir "31 US" a "32 US" en §3 del product-backlog (H-02).
3. Opcional: documentar la convención `EP-T<n>` (H-03) y el ratio horas/punto de los sprints inaugurales (H-04).
