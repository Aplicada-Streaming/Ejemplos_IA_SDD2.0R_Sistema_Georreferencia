# Auditoría de Fase A — GeoVial

| Campo | Valor |
| --- | --- |
| Fase | A (categorías 00_contexto y 01_necesidades_negocio) |
| Alcance | `/sdd2.0/docs/00_contexto/` y `/sdd2.0/docs/01_necesidades_negocio/` (recursivo, incluye `necesidades-de-negocio/`) |
| Insumos de regla | `00_rules_contexto.md`, `01_rules_necesidades_negocio.md` |
| Insumos de trazabilidad | `PROJECT-BRIEF-geovial_v1.0.md`, `PROJECT-README-geovial_v1.0.md` |
| Tipo de proyecto (D8) | `web-monolith` con sub-proyectos `mobile-app-maui`, `library` (declarados en README §1) |
| Auditor | Auditor independiente — Arquitecto de Soluciones + QA Senior (no participó de la generación) |
| Fecha | 2026-05-31 |
| Versión del informe | 1.0 |

---

## 1. Resumen ejecutivo

La Fase A está completa y conforme en lo estructural y normativo. Los 14 documentos auditados (6 en categoría 00, 8 en categoría 01) cumplen D1–D8: idioma rioplatense con tildes y eñes, UTF-8 sin BOM con EOL LF (verificado por conteo de bytes CR=0), filenames kebab-case ASCII con separador de versión `_v1.0.md`, sin vocabulario prohibido del bootstrap (Motor DSL, ESC-POS, impresoras térmicas, Bluetooth) y sin emojis. El stack legítimo de GeoVial no aparece en visión ni alcance (anti-patrón de visión técnica evitado). Todas las secciones obligatorias de §4.2 están presentes en cada documento, los enlaces relativos resuelven y los filenames NB pasan el regex.

Se detectaron incoherencias menores de coherencia cross-doc en la declaración de aristas inversas de dependencias ("es prerequisito de") entre el índice maestro, el README y los §8 de las NB, que no rompen la trazabilidad (las aristas directas "depende de" son consistentes y acíclicas) ni la cadena D6.

Hallazgos: P0 = 0, P1 = 1, P2 = 4, P3 = 3. Veredicto: APROBADO CON OBSERVACIONES.

---

## 2. Matriz D1–D8 por documento

Convención: C = conforme, N/A = no aplica. D1 idioma rioplatense; D2 UTF-8/LF; D3 kebab-case ASCII; D4 versionado `_v`; D5 una sola versión vigente; D6 trazabilidad en cabecera; D7 sin vocabulario bootstrap; D8 tipo en conjunto cerrado.

### Categoría 00_contexto

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| vision-producto_v1.0.md | C | C | C | C | C | C | C | C |
| alcance-proyecto_v1.0.md | C | C | C | C | C | C | C | C |
| roadmap-producto_v1.0.md | C | C | C | C | C | C | C | C |
| compatibilidad-plataformas_v1.0.md | C | C | C | C | C | C | C | C |
| acuerdo-equipo_v1.0.md | C | C | C | C | C | C | C | C |
| README.md | C | C | C | N/A | C | C | C | C |

### Categoría 01_necesidades_negocio

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| necesidades-negocio_v1.0.md (índice) | C | C | C | C | C | C | C | C |
| README.md | C | C | C | N/A | C | C | C | C |
| NB-01-delegacion-recoleccion-personal-no-experto_v1.0.md | C | C | C | C | C | C | C | C |
| NB-02-georreferenciacion-automatica-confiable_v1.0.md | C | C | C | C | C | C | C | C |
| NB-03-continuidad-operativa-sin-conexion_v1.0.md | C | C | C | C | C | C | C | C |
| NB-04-revision-centralizada-sobre-mapa_v1.0.md | C | C | C | C | C | C | C | C |
| NB-05-consistencia-datos-ante-conflictos_v1.0.md | C | C | C | C | C | C | C | C |
| NB-06-trazabilidad-proteccion-datos-personales_v1.0.md | C | C | C | C | C | C | C | C |

Evidencia D2: conteo de bytes crudos arroja CR = 0 y BOM ausente (primeros 3 bytes = `# `) en los 14 archivos; `file` reporta "UTF-8 text" sin terminadores CRLF. Evidencia D3/D4: los 6 NB pasan el regex `^NB-[0-9]{2}-[a-z0-9-]+_v[0-9]+\.[0-9]+\.md$`; ningún archivo usa `.v` antes de la versión. Evidencia D7: scan negativo de `motor dsl|esc-pos|impresora|térmica|bluetooth|nuget|.net 10` en ambas carpetas.

---

## 3. Matriz de estructura obligatoria por documento

### Categoría 00 — cabecera §4.1 y secciones §4.2

| Documento | Cabecera §4.1 | Secciones obligatorias | Estado |
| --- | --- | --- | --- |
| vision-producto | Completa | §1–§10 presentes (problema, stakeholders, propuesta, visión 3 años, SMART, métricas, restricciones, riesgos, glosario, trazabilidad) | Conforme |
| alcance-proyecto | Completa | §1–§10 presentes (propósito, descripción, objetivos, incluido, excluido, supuestos, restricciones, criterios aceptación, gestión cambios, trazabilidad) | Conforme |
| roadmap-producto | Completa | §1–§6 presentes (propósito, fases, matriz fase→épica→sprint→release, dependencias, criterios transición, trazabilidad) | Conforme |
| compatibilidad-plataformas | Completa | §1–§6 presentes (resumen, matriz, restricciones justificadas, alternativas, estado por plataforma, trazabilidad) | Conforme |
| acuerdo-equipo | Completa | §1–§7 presentes (propósito, equipo/roles, ceremonias, acuerdos, DoD, DoR, herramientas) + §8 trazabilidad extra | Conforme |
| README.md | Cabecera adaptada (sin versión) | Documentos, orden de lectura, stakeholders, nota de omisión, pendiente | Conforme |

### Categoría 01 — cabecera §4.1 y secciones §4.2

| Documento | Cabecera §4.1 | Secciones obligatorias | Estado |
| --- | --- | --- | --- |
| necesidades-negocio (índice) | Completa + `Cantidad de NB` y `Versión del catálogo` | Propósito, resumen (tabla D), mapa dependencias, trazabilidad agregada, scope, control de cambios | Conforme |
| README.md | Cabecera adaptada | Tabla NB, mapa dependencias, orden de lectura, RACI, notas | Conforme (ver hallazgo F-02) |
| NB-01 | Completa | §1–§10 presentes | Conforme |
| NB-02 | Completa | §1–§10 presentes | Conforme |
| NB-03 | Completa | §1–§10 presentes | Conforme |
| NB-04 | Completa | §1–§10 presentes | Conforme |
| NB-05 | Completa | §1–§10 presentes | Conforme |
| NB-06 | Completa | §1–§10 presentes | Conforme |

Cada NB tiene §5 con 4 criterios de éxito SMART (número + unidad + plazo), §6 con 4 stakeholders cubriendo propietario/implementador/beneficiario, §7 con CU previstas en estado `a generar`, §9 con MoSCoW justificado y §10 con control de cambios. El glosario de la visión (§9) tiene 13 términos del dominio (≥ 10 exigidos para equipo > 2).

---

## 4. Coherencia cross-doc dentro de la fase

| Verificación | Resultado | Detalle |
| --- | --- | --- |
| Trazabilidad upstream declarada en cada cabecera | Conforme | Las 6 NB y los 5 documentos de 00 citan BRIEF/README con secciones específicas; cumple §3.3 de ambas reglas. |
| Trazabilidad downstream declarada | Conforme | 00 declara 01/02/03/05/06/07; cada NB declara CU previstas (CU-01 a CU-14) con estado `a generar`. |
| IDs NB no duplicados | Conforme | NB-01 a NB-06, sin repeticiones; IDs de CU previstas (CU-01..CU-14) sin colisión entre NB. |
| Enlaces relativos resuelven | Conforme | 6 enlaces del README de 00, 6 del índice 01 y 8 del README de 01 apuntan a archivos/carpeta existentes. |
| Aristas de dependencia directas ("depende de") acíclicas y ≤ 3 | Conforme | Índice §3 y §8 de cada NB coinciden: NB-01 raíz; NB-02→01; NB-03→01,02; NB-04→02,03; NB-05→03,02; NB-06→01. Orden topológico válido, sin ciclos, máximo 2 dependencias. |
| Aristas inversas ("es prerequisito de") consistentes | No conforme (menor) | Contradicción entre fuentes; ver F-01 y F-02. |
| Glosario sin contradicciones | Conforme | El glosario de la visión es coherente con el del BRIEF §12; las NB usan los mismos términos (relevamiento, marcador, observación, sincronización) sin redefinirlos en conflicto. |
| MoSCoW de NB coherente con alcance de 00 | Conforme | 5 NB Must (delegación, georreferenciación, offline/sync, revisión, trazabilidad/datos personales) + 1 Should (consistencia ante conflictos) mapean al MoSCoW del alcance §4; NB-05 Should se alinea con el "Should Have" de resolución de conflictos del BRIEF §4 y alcance §4.2. |
| Inclusión de compatibilidad-plataformas | Conforme | Documentada como excepción justificada: el sub-proyecto `mobile-app-maui` (README §1) activa el documento pese a que el tipo dominante `web-monolith` lo omitiría; coherente con §2.2 y §3.4 de las reglas 00. |

Detalle de la incoherencia de aristas inversas (no rompe trazabilidad porque las aristas directas, que son las que ordenan el backlog, son consistentes):

- Índice §3 + §8 de cada NB (aristas directas, autoritativas): NB-06 depende solo de NB-01; NB-04 depende de NB-02 y NB-03.
- NB-01 §8 declara ser prerequisito de "NB-02, NB-03 y NB-04". Según las aristas directas, NB-01 es prerequisito de NB-02, NB-03 y NB-06 (no de NB-04).
- README §2 (columna "es prerequisito de") declara que NB-04 es prerequisito de NB-06; y NB-04 §8 declara "es prerequisito de NB-06". Sin embargo, ni el índice §3 ni el §8 de NB-06 registran esa dependencia (NB-06 depende únicamente de NB-01). La arista NB-04 → NB-06 está afirmada en dos lugares y negada en otros dos.

---

## 5. Hallazgos enumerados

### F-01 (P1) — Arista de dependencia inversa contradictoria NB-04 ↔ NB-06

- Nivel: P1 (alto; incoherencia cross-doc en aristas de dependencia, sin romper la cadena D6).
- Archivos: `necesidades-de-negocio/NB-04-revision-centralizada-sobre-mapa_v1.0.md` (§8), `README.md` (§2), `necesidades-negocio_v1.0.md` (§3), `necesidades-de-negocio/NB-06-trazabilidad-proteccion-datos-personales_v1.0.md` (§8).
- Sección: §8 de las NB, §2 del README, §3 del índice.
- Evidencia: NB-04 §8 afirma "Es prerequisito de NB-06 en cuanto a la trazabilidad de las acciones de revisión y cierre" y el README §2 lo refleja ("NB-04 … es prerequisito de NB-06"). En cambio, el índice §3 declara `NB-06 → NB-01` (única dependencia) y NB-06 §8 afirma "Depende de NB-01 … No es prerequisito de otras NB". La relación NB-04 → NB-06 queda afirmada y negada simultáneamente.
- Recomendación: definir una única fuente de verdad para el grafo (recomendado: las aristas directas "depende de" del índice §3). Si la dependencia NB-06 ← NB-04 es real, agregarla a NB-06 §8 y al índice §3; si no lo es, eliminar la afirmación de NB-04 §8 y la columna correspondiente del README §2. Regenerar las columnas inversas a partir de las directas para garantizar simetría.

### F-02 (P2) — "Es prerequisito de" de NB-01 inconsistente entre fuentes

- Nivel: P2 (medio).
- Archivos: `necesidades-de-negocio/NB-01-delegacion-recoleccion-personal-no-experto_v1.0.md` (§8), `README.md` (§2).
- Sección: §8 NB-01, §2 README.
- Evidencia: NB-01 §8 declara ser prerequisito de "NB-02, NB-03 y NB-04". El README §2 declara que NB-01 es prerequisito de "NB-02, NB-03, NB-06". Según las aristas directas autoritativas (índice §3: NB-02→01, NB-03→01, NB-06→01; NB-04 no depende de 01), la lista correcta es NB-02, NB-03 y NB-06. NB-01 §8 contiene a NB-04 por error y omite NB-06.
- Recomendación: corregir NB-01 §8 a "prerequisito de NB-02, NB-03 y NB-06", alineando con el índice y el README.

### F-03 (P2) — Cabecera del índice maestro lista trazabilidad upstream a §7 del BRIEF en la cabecera, pero los §8 de las NB la usan de forma desigual

- Nivel: P2 (medio; campo de cabecera parcialmente desalineado).
- Archivos: `necesidades-negocio_v1.0.md` (cabecera), NB individuales.
- Sección: cabecera del índice y §8/§Trazabilidad agregada.
- Evidencia: la cabecera del índice declara upstream "PROJECT-BRIEF §1, §3, §4, §7, §8, §10, §11", pero solo NB-05 (§7 casos 1 y 4) traza efectivamente a §7; la trazabilidad agregada §4.1 mapea §7 únicamente a NB-05. No es un error de contenido, pero la cabecera agregada sugiere una cobertura de §7 más amplia que la real.
- Recomendación: mantener la cabecera del índice como unión de los upstream de las NB (correcto) y, opcionalmente, anotar entre paréntesis qué NB origina cada sección poco frecuente para evitar lectura ambigua.

### F-04 (P2) — README de categoría 00: el cuerpo declara alimentar a "08 y 09" no listadas como downstream en algunos documentos fuente

- Nivel: P2 (medio; coherencia de alcance de trazabilidad).
- Archivo: `00_contexto/README.md` (párrafo introductorio).
- Sección: introducción.
- Evidencia: el README afirma que la sección "alimenta a las categorías 01, 02, 03, 05, 06, 07, 08, 09 y 11". El acuerdo-equipo sí declara downstream a 08 y 09, y compatibilidad a 09, por lo que la afirmación es defendible a nivel de sección; sin embargo, la cabecera de §3.3 de las reglas 00 enuncia el downstream canónico como 01, 02, 03, 05, 07, 11. La ampliación a 06/08/09 es correcta por el contenido real pero conviene dejarla explícita por documento para trazabilidad fina.
- Recomendación: mantener la lista ampliada (es más precisa que la canónica) y, si se busca conformidad literal con §3.3, anotar que 06/08/09 provienen de roadmap/acuerdo-equipo/compatibilidad.

### F-05 (P2) — Métrica no SMART en visión §6 ("A definir con el cliente")

- Nivel: P2 (medio; campo parcial, no afecta criterios de aceptación porque §5 SMART está completo).
- Archivo: `00_contexto/vision-producto_v1.0.md` (§6, fila "Adopción del personal de campo").
- Sección: §6 Métricas de éxito.
- Evidencia: la fila "Adopción del personal de campo" tiene target "A definir con el cliente", sin valor numérico. Las otras 4 métricas son SMART. El criterio §6 de las reglas exige los objetivos SMART en §5 (cumplido con 4 objetivos numéricos), por lo que esto no rompe el criterio de aceptación, pero deja una métrica de éxito sin target.
- Recomendación: fijar un target provisorio marcado como supuesto a validar (coherente con el tratamiento de supuestos del intake), por ejemplo "≥ 70 % de agentes activos", para que la métrica sea medible desde 08. Nota: NB-01 §5 ya define ese target en ≥ 70 %, por lo que basta con replicarlo.

### F-06 (P3) — acuerdo-equipo agrega §8 Trazabilidad fuera del set §4.2 (§1–§7)

- Nivel: P3 (bajo; mejora de claridad, no defecto).
- Archivo: `00_contexto/acuerdo-equipo_v1.0.md` (§8).
- Evidencia: §4.2 de las reglas define para acuerdo-equipo las secciones §1–§7. El documento agrega §8 Trazabilidad. La trazabilidad ya está en la cabecera §4.1; la sección adicional es redundante pero útil. No es un anti-patrón.
- Recomendación: aceptar como mejora; opcionalmente renombrar a "Trazabilidad (complementaria a cabecera)" para dejar claro que excede el esqueleto obligatorio.

### F-07 (P3) — Estado "Propuesto" uniforme en toda la fase

- Nivel: P3 (bajo).
- Archivos: todos los documentos de 00 y 01.
- Evidencia: todos declaran Estado "Propuesto", valor válido del enum cerrado. Es coherente con que el BRIEF/README están "En revisión". No es defecto.
- Recomendación: ninguna acción obligatoria; al aprobarse la fase, promover el estado a "Aprobado"/"Vigente" de forma sincronizada.

### F-08 (P3) — Índice maestro: campo "Estado" del enum NB usa "Propuesto" donde la tabla D ejemplifica "Aprobado"

- Nivel: P3 (bajo; estilístico).
- Archivo: `necesidades-negocio_v1.0.md` (§2) y READMEs.
- Evidencia: la columna Estado de la tabla resumen usa "Propuesto" para las 6 NB; el ejemplo de la regla (§4.4 tabla D) muestra "Aprobado". "Propuesto" pertenece al enum cerrado, por lo que es conforme; se señala solo por contraste con el ejemplo.
- Recomendación: ninguna; el valor es válido.

---

## 6. Veredicto final

APROBADO CON OBSERVACIONES.

No se detectaron hallazgos P0: no hay ruptura de trazabilidad de la cadena D6, no falta ningún documento obligatorio, no hay violación de D1–D8, no aparece vocabulario prohibido del bootstrap y todas las cabeceras y secciones obligatorias están presentes. La cadena PROJECT-BRIEF → 00_contexto → NB → CU previstas está declarada y es consistente en sus aristas directas.

Se admiten 1 hallazgo P1, 4 P2 y 3 P3, todos de coherencia cross-doc o de completitud de campos, sin efecto bloqueante sobre la promoción a la Fase B.

### Condiciones para promover sin observaciones

1. Resolver F-01: unificar la fuente de verdad del grafo de dependencias y eliminar la contradicción sobre la arista NB-04 → NB-06 (corregir índice §3, README §2, NB-04 §8 y NB-06 §8 hasta que coincidan).
2. Resolver F-02: corregir NB-01 §8 a "prerequisito de NB-02, NB-03 y NB-06".
3. Recomendado (no bloqueante): atender F-03 a F-05 para dejar la trazabilidad de cabeceras y la métrica de adopción (visión §6) plenamente alineadas.

La Fase A puede avanzar a la Fase B (categoría 02) en paralelo a la corrección de F-01 y F-02, dado que la generación de CU se apoya en las aristas directas "depende de", que ya son consistentes.

---

## Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-31 | Auditoría inicial de Fase A (categorías 00 y 01) por auditor independiente |
