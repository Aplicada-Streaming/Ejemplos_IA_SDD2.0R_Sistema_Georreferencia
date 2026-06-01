# Auditoría de Fase B — Especificación funcional (02) y UX/UI/DX (03)

| Campo | Valor |
| --- | --- |
| Fase | B |
| Categorías auditadas | 02_especificacion_funcional, 03_ux_ui_dx |
| Alcance | `/sdd2.0/docs/02_especificacion_funcional/` (recursivo) y `/sdd2.0/docs/03_ux_ui_dx/` (recursivo) |
| Gating | 04_prompts_ai OMITIDA por `usa_llm=false` (verificado: no existe carpeta con contenido) |
| Tipo D8 | web-monolith (con sub-proyectos mobile-app-maui y librería de sincronización) |
| Auditor | Arquitecto de Soluciones + QA Senior (independiente, sin participación en la generación) |
| Fecha | 2026-06-01 |
| Reglas aplicadas | `02_rules_especificacion_funcional.md`, `03_rules_ux_ui_dx.md`, §10 master-prompt |

---

## 1. Resumen ejecutivo

La Fase B está completa y bien construida. Los 14 CU superan el mínimo de 8 para web-monolith; cada uno trae las 11 secciones obligatorias del §4.2 más §13 de concurrencia, ≥3 criterios Given/When/Then con valores, ≥1 flujo alternativo y ≥3 excepciones con código. Las 8 RN, el modelo conceptual de 12 entidades (con diagrama Mermaid, glosario y trazabilidad, sin tipos físicos) y las 7 RC cumplen sus estructuras. En 03, los 6 artefactos (experiencia-de-uso, 6 wireframes con 9 secciones y estados mínimos, glosario-ux y dx-portal-developers) declaran Variante, toman WCAG 2.2 AA como piso, aplican Diátaxis y no incluyen CSS ni stack. No se detectó vocabulario prohibido del bootstrap, ni stack en 02, ni CSS/colores en wireframes. La cobertura bidireccional NB↔CU es total y sin huérfanos. Los hallazgos son inconsistencias de referencia cruzada RN↔CU (P1) y cabeceras de CU/RN/RC sin líneas D6 explícitas (P2), sin ningún P0.

Conteo por nivel: P0 = 0 · P1 = 1 (agrupa 5 RN) · P2 = 2 · P3 = 2.

Veredicto: APROBADO CON OBSERVACIONES.

---

## 2. Matriz D1-D8 por documento

Convenciones: OK = conforme; — = no aplica. D1 idioma rioplatense con tildes/eñes; D2 UTF-8/LF; D3 filename kebab + `_v1.0`; D4 una sola versión vigente; D5 sin vocabulario prohibido / sin stack indebido; D6 trazabilidad declarada; D8 tipo cerrado coherente.

### 2.1 Categoría 02 — índice, modelo y agrupados

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D8 | Notas |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| especificacion-funcional_v1.0.md | OK | OK | OK | OK | OK | OK (cabecera up/down) | OK | Matriz NB→CU→RN→US completa |
| modelo-conceptual_v1.0.md | OK | OK | OK | OK | OK | OK (§5, §8) | OK | 12 entidades, sin tipos físicos |
| CU-01..CU-14 (14 archivos) | OK | OK | OK | OK | OK | Parcial (§9, sin línea D6 en cabecera) | OK | Ver hallazgo H-3 (P2) |
| RN-01..RN-08 (8 archivos) | OK | OK | OK | OK | OK | Parcial (§5, sin línea D6 en cabecera) | OK | Ver H-1 (P1) y H-3 (P2) |
| RC-01..RC-07 (7 archivos) | OK | OK | OK | OK | OK | OK (§5 RN/CU) | OK | 6 secciones del §4.2.3 |
| README.md (02) | OK | OK | OK | OK | OK | OK | OK | Recomendado, presente |

### 2.2 Categoría 03

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D8 | Notas |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| experiencia-de-uso_v1.0.md | OK | OK | OK | OK | OK | OK (cabecera up/down) | OK | Variante UX/UI |
| wireframes-captura-movil_v1.0.md | OK | OK | OK | OK | OK | OK (§8) | OK | Variante UX/UI |
| wireframes-gestion-relevamientos_v1.0.md | OK | OK | OK | OK | OK | OK (§8) | OK | — |
| wireframes-login-relogueo_v1.0.md | OK | OK | OK | OK | OK | OK (§8) | OK | — |
| wireframes-mapa-revision-web_v1.0.md | OK | OK | OK | OK | OK | OK (§8) | OK | — |
| wireframes-marcador-carrusel_v1.0.md | OK | OK | OK | OK | OK | OK (§8) | OK | — |
| wireframes-resolucion-conflictos-web_v1.0.md | OK | OK | OK | OK | OK | OK (§8) | OK | — |
| glosario-ux_v1.0.md | OK | OK | OK | OK | OK | OK | OK | No duplica 02 |
| dx-portal-developers_v1.0.md | OK | OK | OK | OK | OK | OK (cabecera up/down) | OK | Variante DX, flag `tiene_portal_developers` |
| README.md (03) | OK | OK | OK | OK | OK | OK | OK | Recomendado, presente |

Excepciones D-matrix: ninguna viola D1-D8. La numeración de secciones de los CU salta de §11 a §13 (omite §12 Performance, que el §4.3 reserva a rest-api/worker/mobile): correcto para web-monolith, no es hueco.

---

## 3. Matriz de estructura obligatoria por documento

### 3.1 Categoría 02

| Documento | Estructura exigida | Resultado |
| --- | --- | --- |
| especificacion-funcional_v1.0.md | índice + matriz NB→CU→RN→US | OK: 14 CU, 8 RN, modelo, 7 RC, matriz §6 + cobertura bidireccional |
| Cada CU-XX | 11 secciones §4.2 + §13 concurrencia (web-monolith) | OK en los 14 |
| Cada CU-XX | ≥3 Given/When/Then con valores | OK: 3 por CU, con valores concretos (radios, metros, códigos) |
| Cada CU-XX | ≥1 flujo alternativo + ≥1 excepción con código | OK: 2 a 4 flujos alt y 3 códigos de excepción por CU |
| Cada RN-XX | 7 secciones §4.2.1 + CU afectados | OK estructura; CU afectados con inconsistencias (H-1) |
| modelo-conceptual | 9 secciones §4.2.2 (entidades sin tipos, diagrama, glosario, trazabilidad) | OK: 9 secciones, Mermaid erDiagram, 0 tipos físicos |
| Cada RC-XX | 6 secciones §4.2.3 | OK en las 7 |
| Mínimo de CU web-monolith = 8 | — | OK: hay 14 |

### 3.2 Categoría 03

| Documento | Estructura exigida | Resultado |
| --- | --- | --- |
| Cada artefacto | campo `Variante` en cabecera | OK en los 9 (8 UX/UI + 1 DX) |
| experiencia-de-uso | 11 secciones §4.2 | OK |
| Wireframes (≥4) | 9 secciones §4.2.1 + estados mínimos (vacío/cargando/con datos/error) | OK: 6 wireframes, todos con las 9 secciones y los 4 estados (más sin conexión/solo lectura/éxito donde aplica) |
| dx-portal-developers | 8 secciones §4.2.6 + Diátaxis | OK: 8 secciones, tabla Diátaxis de 4 modos, páginas landing/quick-start/reference/changelog/status |
| Accesibilidad | WCAG 2.2 AA como piso | OK en experiencia-de-uso §5 y dx-portal §6 |
| glosario-ux | sin duplicar 02 | OK: §2 referencia términos de 00/02, §3 agrega solo términos de presentación |
| quick-start verificable (DX) | — | OK: dx-portal §5 declara verificación manual previa a cada publicación |

---

## 4. Coherencia cross-doc

### 4.1 Cobertura bidireccional NB↔CU

| NB | CU asignados (matriz §6) | CU que declaran esta NB en §9 | Estado |
| --- | --- | --- | --- |
| NB-01 | CU-01, CU-02, CU-03 | CU-01, CU-02, CU-03 | OK |
| NB-02 | CU-04, CU-05 | CU-04, CU-05 | OK |
| NB-03 | CU-06, CU-07 | CU-06, CU-07 | OK |
| NB-04 | CU-08, CU-09, CU-10 | CU-08, CU-09, CU-10 | OK |
| NB-05 | CU-11, CU-12 | CU-11, CU-12 | OK |
| NB-06 | CU-13, CU-14 | CU-13, CU-14 | OK |

Cada NB-01..06 tiene ≥1 CU y cada CU-01..14 declara exactamente 1 NB. Sin huérfanos en ninguna dirección.

### 4.2 CU referenciados por los wireframes de 03

CU citados por artefactos de 03: CU-01, CU-02, CU-04, CU-05, CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12. Todos existen en 02. No hay CU inventados. CU-03, CU-13 y CU-14 (administración de jerarquía, auditoría y autorización transversal) no tienen wireframe propio; es coherente porque no aportan superficie de UI rica diferenciada (la autorización es transversal y la auditoría no tiene pantalla dedicada). No constituye hallazgo.

### 4.3 IDs no duplicados y enlaces

- IDs CU-01..14, RN-01..08, RC-01..07: contiguos y sin duplicados.
- Filenames: ninguno usa `.v`; todos `_v1.0.md` con slug kebab lowercase. No hay convivencia de versiones (no se requiere `_legacy/`).
- Enlaces relativos `.md` en 02 y 03: todos resuelven (verificado por recorrido de targets).
- Modelo conceptual: referencia CU/RN reales (§8) y las RC referencian entidades reales del modelo (verificado en RC-01..07).

### 4.4 Consistencia de referencias RN↔CU (defecto detectado)

Comparando las RN que cada CU declara en su §9 contra la lista "CU afectados" (§5) de cada RN, hay asimetrías: hay CU que citan una RN sin figurar en el §5 de esa RN, y RN que listan CU que no las citan.

| RN | CU que la citan pero no están en su §5 | CU en su §5 que no la citan |
| --- | --- | --- |
| RN-01 | — | CU-05, CU-06 |
| RN-02 | CU-01 | — |
| RN-03 | CU-06 | — |
| RN-05 | CU-01, CU-05, CU-09 | — |
| RN-07 | CU-02, CU-08 | — |
| RN-04, RN-06, RN-08 | (consistentes) | (consistentes) |

Detalle: la matriz del índice §6 y el §9 de CU-01 citan RN-02 para CU-01 (paso 4.3 "radio de agrupación (RN-02)"), pero el §5 y el §3 (ámbito) de RN-02 excluyen deliberadamente la creación de relevamiento. RN-01 (autorización) lista en su §5 los 14 CU, pero CU-05 y CU-06 no la citan en su §9. Ver hallazgo H-1.

### 4.5 Glosarios sin contradicción

glosario-ux §2 referencia los términos de dominio de 00/02 con su semántica autoritativa y §3 define solo términos de presentación (pin, carrusel, toast, skeleton, etc.). El glosario del modelo conceptual (02) y el de 03 no se contradicen.

---

## 5. Hallazgos enumerados

### H-1 (P1) — Listas "CU afectados" de RN inconsistentes con las RN que citan los CU

- Archivos: `reglas-de-negocio/RN-01..RN-08` (§5) y `casos-de-uso/CU-01..CU-14` (§9), índice §6.
- Sección: RN §5 "CU afectados" vs CU §9 "Reglas de negocio aplicables".
- Evidencia: RN-02 omite CU-01 que sí la cita; RN-03 omite CU-06; RN-05 omite CU-01, CU-05 y CU-09; RN-07 omite CU-02 y CU-08; RN-01 lista CU-05 y CU-06 que no la citan. La vinculación cross-doc (§3.3 de 02_rules: "cada CU enumera RN que lo restringen, sin inventarlas" y §6: "cada RN enumera CU afectados explícitos") exige simetría verificable.
- Impacto: no rompe la trazabilidad NB↔CU ni invalida documentos, pero deja una matriz RN↔CU ambigua que arrastrará ruido a 06/08.
- Recomendación: reconciliar en una sola pasada. Para cada par, decidir la fuente de verdad (¿la RN aplica al CU o no?) y alinear ambas direcciones; en particular resolver si RN-02 aplica a CU-01 (creación con radio) y si RN-01 debe figurar en el §9 de CU-05 y CU-06.

### H-2 (P2) — Discrepancia entre título lógico de CU-02 en el índice y el slug del archivo

- Archivos: `especificacion-funcional_v1.0.md` (fila CU-02) y `casos-de-uso/CU-02-seleccionar-relevamiento-asignado_v1.0.md` (H1).
- Sección: catálogo §2 / cabecera del CU.
- Evidencia: el índice y el H1 titulan "Iniciar sesión y seleccionar un relevamiento asignado", pero el slug del filename es `seleccionar-relevamiento-asignado` (no refleja el "iniciar sesión", que es parte central del CU y donde se absorbe el login/relogueo). El slug es kebab válido; la discrepancia es de cobertura semántica del nombre, no de convención.
- Recomendación: opcional renombrar a un slug que incluya el login, o dejar constancia en §10 (ya documentado parcialmente). No bloqueante.

### H-3 (P2) — Cabeceras de CU/RN/RC sin líneas de trazabilidad D6 explícitas

- Archivos: los 14 CU, 8 RN y 7 RC.
- Sección: cabecera §4.1.
- Evidencia: el índice (02) y los artefactos de 03 incluyen líneas `Trazabilidad upstream/downstream` en la cabecera; los CU/RN/RC no, y resuelven la trazabilidad en secciones (§9 del CU, §5 de la RN, §5 de la RC). El §4.1 de 02_rules no exige esas líneas en la cabecera, por lo que es conformidad parcial deseable, no incumplimiento.
- Recomendación: por consistencia con el índice y con 03, agregar líneas `Trazabilidad upstream`/`downstream` en la cabecera de CU/RN/RC. Mejora la lectura para revisores de 05/06/08.

### H-4 (P3) — experiencia-de-uso no enlaza CU-03 en su "CU origen"

- Archivo: `experiencia-de-uso_v1.0.md` §9.
- Evidencia: lista CU-01, CU-02, CU-04..CU-12 pero no CU-03 (administración de jerarquía de usuarios/áreas), que sí tiene formularios administrativos con interacción humana. No hay wireframe de administración de usuarios.
- Recomendación: confirmar si la administración de jerarquía queda fuera del alcance UX de v1; si no, sumar CU-03 al marco o un wireframe de altas/bajas. Estilístico/cobertura, no bloqueante.

### H-5 (P3) — Referencia a intake en la introducción del modelo conceptual

- Archivo: `modelo-conceptual_v1.0.md` (párrafo introductorio).
- Evidencia: cita `PROJECT-README §7` como modelo lógico-resumen. Es un insumo de intake legítimo y la referencia es conceptual (relaciones/cardinalidades), sin tipos físicos ni stack, por lo que no viola D5. Se anota solo por higiene de procedencia (lo canónico aguas arriba es 01/00).
- Recomendación: opcional, anclar la procedencia a la cadena 00/01 además del README de intake.

---

## 6. Verificaciones negativas (sin hallazgo)

- Vocabulario prohibido del bootstrap (Motor DSL, impresora térmica, ESC-POS, Bluetooth de impresión, ticket/comanda): ausente en 02 y 03.
- Stack legítimo de GeoVial (Blazor, .NET, SignalR, SQLite, MAUI, Android, CSS, etc.): ausente en 02 y en wireframes de 03 (verificado por regex).
- Tipos físicos en modelo/RC (varchar, int, uuid, timestamp, etc.): ausentes.
- 04_prompts_ai: carpeta inexistente; omisión correcta por `usa_llm=false`. No es hallazgo.
- Codificación UTF-8 y saltos LF en los 40 archivos de 02 y 03: conformes.
- Anti-patrones §4.5 (02) y §4.4 (03): no se detectó CU con UI fina, RN escrita como CU, modelo con tipos físicos, criterios narrativos sin valores, wireframe con CSS, ni DX sin quick-start verificable.

---

## 7. Veredicto final

**APROBADO CON OBSERVACIONES.**

No hay hallazgos P0: la fase no se rechaza y puede promover. Se registra 1 P1 (H-1), 2 P2 (H-2, H-3) y 2 P3 (H-4, H-5).

Condiciones recomendadas antes de consumir 02/03 en fases aguas abajo (06/08):

1. Resolver H-1 reconciliando las listas RN↔CU en ambas direcciones (bloquea la fidelidad de la matriz de cobertura que heredarán 06 y 08).
2. Atender H-2 y H-3 para consistencia de nomenclatura y cabeceras (no bloqueante).
3. H-4 y H-5 quedan a criterio del equipo al cierre de fase.

---

## 8. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Informe inicial de auditoría de Fase B (02 + 03) por auditor independiente |
