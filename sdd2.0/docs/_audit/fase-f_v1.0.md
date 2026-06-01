# Auditoría Fase F — 09 DevOps y 10 Developer guide — GeoVial

**Proyecto:** GeoVial
**Documento:** fase-f_v1.0.md
**Categorías auditadas:** 09_devops (7 archivos) y 10_developer_guide (7 archivos)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación de la Fase F)
**Fecha:** 2026-06-01
**Reglas de referencia:** `/sdd2.0/devs/rules/09_rules_devops.md`, `/sdd2.0/devs/rules/10_rules_developer_guide.md`
**Método:** lectura exhaustiva de los 14 archivos de ambas categorías; verificación empírica con conteo de stages, secciones y entradas; regex de IDs (STAGE-XX, ENV, ISSUE-XX); scan de CRLF/BOM y validación de encoding (iconv); regex de vocabulario prohibido del bootstrap; verificación del patrón de filenames (`_v` vs `.v`, kebab-case ASCII, slug genérico); resolución de los enlaces relativos; cotejo cruzado contra 05 (NFR numéricos de arquitectura-solucion §8, ADR-05/06/07, `contratos-abstractions-sync_v1.0.md`), 08 (definition-of-done §1.1–§1.4, estrategia-testing, criterios-validacion, casos-prueba-referenciales TC-01..26) y los intakes (PROJECT-README §9/§10/§11/§12/§14). Verificación de paridad referencia-api↔contrato abriendo ambos documentos y enumerando tipos, métodos y excepciones uno a uno. No se reauditó el upstream; solo se verificó la existencia y coherencia de los IDs referenciados.

---

## 1. Resumen ejecutivo

La Fase F de GeoVial está sustancialmente completa, internamente consistente y trazable de punta a punta en sus dos categorías. Se verificaron empíricamente los 14 archivos. Todos los artefactos obligatorios existen con sus secciones completas:

- **09_devops** (7 archivos): `pipeline-ci-cd` (14 stages técnicos mapeados a los 6 de README §11, con las 6 secciones de §4.2), `estrategia-versionado` (6 secciones de §4.3), `entornos-deploy` (5 secciones de §4.4 + dos planos), dos `guia-publicacion-<tipo-artefacto>` (`image-docker` y `paquete-github-packages`, ambas con las 5 secciones de §4.5), `supply-chain-seguridad` (6 secciones de §4.6) y README. La separación publicación-de-paquete (canales preview/stable de `GeoVial.Sync`) vs despliegue-de-servicio (DEV/QA/STAGING/PROD del monolito) es explícita y se sostiene en todos los documentos. Cada quality gate referencia un criterio DoD de 08 o un NFR de 05 reales; el pipeline ejecuta la DoD como gates sin redefinirla; cada NFR numérico de 05 §8 tiene gate o tratamiento documentado.
- **10_developer_guide** (7 archivos): `conceptos-fundamentales` (Explanation, §4.2), `guia-onboarding-developer` (Tutorial con TTFS, §4.3), `guia-integracion-aplicacion-movil` (How-to, §4.4), `referencia-api` (Reference, §4.5), `troubleshooting` (6 ISSUE-XX, §4.7), `glosario-tecnico` (§4.8) y README. Cada documento declara su cabecera Diátaxis (Tipo/Audiencia/Nivel/Tiempo) y una sección "Referencias cruzadas" con al menos un enlace a 05. El gating de la categoría (D8 dominante `web-monolith`, opcional) está justificado por el sub-proyecto `library` publicable, lo que la vuelve obligatoria para esa superficie.

Conformidad D1–D8 satisfactoria: idioma rioplatense con tildes/eñes; UTF-8 sin BOM y line endings LF en los 14 archivos; filenames kebab-case ASCII; versionado `_v1.0.md` uniforme (cero ocurrencias del patrón heredado `.v1.0`); sufijo `_v1.0.md` presente en todos los artefactos de 10 (README exento por convención); slug genérico `aplicacion-movil` en la guía de integración (no hardcodea framework comercial; el cuerpo nombra .NET MAUI como runtime real, permitido); ningún gestor hardcodeado en un nombre genérico de 09 (los dos `guia-publicacion-<tipo>` usan valores admitidos de §3.1: `image-docker` y `paquete-github-packages`). No se detectó vocabulario prohibido del bootstrap (Motor DSL, impresoras térmicas, ESC-POS, Bluetooth de impresión). El stack real (GitHub Actions, GitHub Packages, MinVer, Docker, .NET 9, MAUI, SQLite, SQL Server, cosign) aparece legítimamente.

La paridad `referencia-api` ↔ `contratos-abstractions-sync_v1.0.md` es muy alta: las 5 interfaces, los 4 tipos de datos y las 3 excepciones del contrato §3/§4/§5 están todos documentados sin omisiones y con la semántica correcta. El único desvío es el tipo `PushResponse`, retorno de `ISyncBackendClient.PushAsync` en la referencia-api, que no figura en la enumeración de tipos públicos del contrato (§4). Es un DTO delgado cuyos campos (`Confirmed`, `Conflicts`) ya están contractualizados vía `SyncResult`/`ConflictInfo`, por lo que no fabrica semántica nueva, pero rompe la regla de "paridad uno a uno sin tipos inventados". Se clasifica P1 (ver hallazgo F-01) con lectura estricta P0 anotada.

No se hallaron violaciones D1–D8, ni vocabulario prohibido, ni gates que no referencien DoD/NFR real, ni DoD redefinida, ni NFR numérico sin tratamiento, ni filename hardcodeado, ni confusión publicación-vs-despliegue, ni mezcla de cuadrantes Diátaxis, ni troubleshooting con menos de 5 ISSUE. El único hallazgo alto es la paridad de `PushResponse`. El resto son P2/P3.

### Conteo de hallazgos por nivel

| Nivel | Cantidad |
| --- | --- |
| P0 bloqueante | 0 |
| P1 alto | 1 |
| P2 medio | 3 |
| P3 bajo | 2 |
| **Total** | **6** |

### Veredicto

**APROBADO CON OBSERVACIONES.** Los entregables de 09 y 10 son promovibles. La condición única para cerrar el P1 es alinear el tipo `PushResponse` con el contrato de 05 (incorporarlo al contrato §4 o reexpresar la firma de `PushAsync` sobre tipos ya contractualizados). Las observaciones P2/P3 son menores y no rompen trazabilidad ni conformidad estructural.

---

## 2. Matriz de conformidad D1–D8 por documento

Convenciones: OK = cumple; n/a = no aplica al tipo de documento.

D1 idioma rioplatense con tildes/eñes · D2 UTF-8/LF (sin CRLF, sin BOM) · D3 filenames kebab-case ASCII con slug genérico (sin gestor/framework hardcodeado) · D4 versionado `_v1.0.md` (no `.v1.0.md`) · D5 IDs uniformes (STAGE-XX, ISSUE-XX de dos dígitos) · D6 trazabilidad en cabecera/secciones · D7 sin vocabulario prohibido del bootstrap · D8 tipo cerrado (web-monolith con sub-proyecto library).

### 09_devops

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `pipeline-ci-cd_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `estrategia-versionado_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `entornos-deploy_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `guia-publicacion-image-docker_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `guia-publicacion-paquete-github-packages_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `supply-chain-seguridad_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `README.md` | OK | OK | OK | OK | n/a | OK | OK | OK |

### 10_developer_guide

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `conceptos-fundamentales_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `guia-onboarding-developer_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `guia-integracion-aplicacion-movil_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `referencia-api_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `troubleshooting_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `glosario-tecnico_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `README.md` | OK | OK | OK | OK | n/a | OK | OK | OK |

Notas de verificación empírica:

- **D2:** los 14 archivos validan como UTF-8 (iconv) sin BOM; scan de `\r`: 0 coincidencias por archivo (todos LF).
- **D3 (09):** los dos documentos de publicación usan `<tipo-artefacto>` admitido por §3.1 de la regla 09 (`image-docker`, `paquete-github-packages`). No hay gestor hardcodeado en un nombre genérico: la regla obliga un documento por tipo de artefacto, y GeoVial tiene dos tipos (imágenes del monolito y paquete de la librería), por lo que dos guías nominadas por su tipo es la materialización correcta, no el anti-patrón `guia-publicacion-nuget` del fuente.
- **D3 (10):** `guia-integracion-aplicacion-movil_v1.0.md` usa slug genérico (`aplicacion-movil`, descriptor de sistema objetivo análogo a los ejemplos `cli`/`servicio-web`/`aplicacion-de-escritorio` de §3.1), no un nombre comercial. Corrige el `guia-integracion-maui` del fuente. El cuerpo nombra .NET MAUI como runtime real, lo cual la regla permite explícitamente.
- **D4:** cero ocurrencias de `.v<X.Y>`; todos los artefactos versionables llevan `_v1.0.md`. Los dos README quedan sin sufijo por convención.
- **D5:** STAGE-01..STAGE-14 contiguos de dos dígitos en el pipeline; ISSUE-01..ISSUE-06 contiguos en troubleshooting.
- **D7:** regex `motor dsl|impresora|térmica|esc-?pos|bluetooth`: 0 coincidencias en las dos carpetas.
- **D8:** todos asumen `web-monolith` con sub-proyecto `library` (`GeoVial.Sync`); el modelo de ambientes/canales y la guía 10 se derivan de ese tipo.

---

## 3. Matriz de estructura obligatoria

### 3.1 — 09_devops

| Artefacto | Requisito de §6 / §4 | Verificación | Estado |
| --- | --- | --- | --- |
| `pipeline-ci-cd` | 6 secciones §4.2; stages lint/build/test/SCA/SBOM/firma/publish con gates | §1 stages (STAGE-01..14 cubren los 7 obligatorios), §2 matriz SO/runtime, §3 caché/artefactos, §4 promotion, §5 rollback, §6 notificaciones | Completo |
| `estrategia-versionado` | 6 secciones §4.3 | §1 SemVer 2.0.0, §2 Conventional Commits, §3 herramienta (MinVer), §4 branching (GitHub Flow), §5 canales, §6 deprecation | Completo |
| `entornos-deploy` | 5 secciones §4.4 + modelo DEV/QA/STAGING/PROD + canales preview/stable | §1 ambientes y canales (dos planos), §2 IaC (Docker Compose), §3 12-factor, §4 secretos, §5 promoción | Completo |
| `guia-publicacion-image-docker` | 5 secciones §4.5 | §1 pre-requisitos, §2 comando/stage, §3 verificación post-publish, §4 rollback, §5 métricas | Completo |
| `guia-publicacion-paquete-github-packages` | 5 secciones §4.5 | §1..§5 presentes | Completo |
| `supply-chain-seguridad` | 6 secciones §4.6 SBOM/firma/SLSA/SCA/SAST-DAST/CVE | §1 SBOM, §2 firma, §3 SLSA L2, §4 dependency scanning, §5 SAST/DAST, §6 CVE | Completo |
| `README` | índice navegable con orden de lectura | Orden de lectura, estado de artefactos, plataforma/workflows | Completo |

### 3.2 — 10_developer_guide

| Artefacto | Requisito de §6 / §4 | Verificación | Estado |
| --- | --- | --- | --- |
| `conceptos-fundamentales` | 5 secciones §4.2 (Explanation) | §1 concepto central, §2 modelo mental, §3 decisiones (ADR-05/06/07), §4 vocabulario, §5 qué NO hace | Completo |
| `guia-onboarding-developer` | 5 secciones §4.3 (Tutorial con TTFS) | §1 prerequisites, §2 Hello world <5 min, §3 caso real <30 min, §4 integración <1 h, §5 siguientes pasos | Completo |
| `guia-integracion-aplicacion-movil` | 5 secciones §4.4 (How-to) | §1 objetivo, §2 prerequisites, §3 pasos numerados (6), §4 verificación, §5 troubleshooting específico | Completo |
| `referencia-api` | 5 secciones §4.5 (Reference) | §1 tipos, §2 métodos, §3 eventos, §4 excepciones, §5 ejemplos por método | Completo |
| `troubleshooting` | ≥5 ISSUE-XX, §4.7 | 6 ISSUE-XX; §1 tabla, §2 diagnóstico paso a paso por ISSUE, §3 logs, §4 reporte de bug | Completo |
| `glosario-tecnico` | §4.8 tabla término/definición/cross-ref | 14 términos kebab con definición operativa y referencia cross-doc | Completo |
| `README` | índice, orden de lectura, prerequisitos, quick-start 5–10 líneas | Presentes; registra el gating de la categoría | Completo |

Cabeceras Diátaxis: los 6 documentos de cuadrante declaran `Tipo Diátaxis`/Audiencia/Nivel/Tiempo. El README usa "Tipo Diátaxis" como columna del índice (no como campo de cuadrante propio), lo cual es correcto: un README no es un cuadrante Diátaxis.

---

## 4. Coherencia cross-doc y cross-categoría

### 4.1 Gates ↔ DoD/NFR (09)

Verificación de que cada quality gate referencia un criterio DoD de 08 o un NFR de 05 real, y de que el pipeline no redefine la DoD:

| STAGE | Gate | Referencia DoD/NFR | ¿Real? |
| --- | --- | --- | --- |
| STAGE-01 Lint | 0 warnings nuevos | criterios-validacion §5 (análisis estático) | Sí |
| STAGE-02 Restore | política de licencias MIT/Apache/BSD | criterios-validacion §5 | Sí |
| STAGE-03 Build | sin warnings como error | definition-of-done §1.1/§1.2 | Sí |
| STAGE-04 Test unit | 100% verde; pisos 85/75, 80/70 | estrategia-testing §2; DoD/pirámide | Sí (pisos coinciden con 08 §2) |
| STAGE-05 Test integración | TC-24 p95 ≤500 ms, TC-10 sync ≤5 min, TC-25 OpenAPI | NFR 05 §8 (latencia, sync); DoD OpenAPI | Sí (TC-10/24/25 existen en 08) |
| STAGE-06 Test componente | presentación ≥60/50 | DoD US; pirámide 10% | Sí |
| STAGE-07 Cobertura | líneas ≥80%, branches ≥70% | definition-of-done §1.3/§1.4; README §9/§11 | Sí (coincide con gate global de 08) |
| STAGE-08 SCA | 0 CVE críticas; 0 altas sin excepción | DoD release; supply-chain §4 | Sí |
| STAGE-09 SBOM | SBOM adjunto | supply-chain §1 | Sí |
| STAGE-10 Firma | firma válida en transparency log | supply-chain §2 | Sí |
| STAGE-11 Package | .nupkg con SemVer | definition-of-done §1.4 | Sí |
| STAGE-12 Build imágenes | 3 imágenes construyen | definition-of-done §1.4 | Sí |

El pipeline cita la DoD por sección y la ejecuta como gates; no la reescribe. Las secciones §1.1–§1.4 de `definition-of-done_v1.0.md` existen y corresponden a lo citado.

Cobertura de NFR numéricos de 05 §8 (5 métricas):

| NFR (05 §8) | Valor | Gate o tratamiento | Estado |
| --- | --- | --- | --- |
| Tiempo de sincronización | ≤5 min | Gate bloqueante TC-10 en STAGE-05 | Cubierto |
| Latencia API | p95 ≤500 ms | Gate bloqueante TC-24 en STAGE-05 | Cubierto |
| Disponibilidad backend | SLO 99% | Métrica estadística en operación (criterios-validacion §3/§6), excepción documentada de la DoD §2; gate humano de promoción a PROD | Cubierto con excepción documentada |
| Confiabilidad georreferenciación | ≥95% | Métrica estadística en operación (criterios-validacion §6) | Cubierto con excepción documentada |
| Tamaño de foto sincronizada | acotar payload | Responsabilidad del consumidor (conceptos §5) + medición en pipeline de captura | Cubierto |

Las dos métricas estadísticas (disponibilidad, confiabilidad) no se materializan como test bloqueante sino como observación operativa, y la excepción está explícitamente prevista por la DoD §2 y criterios-validacion §6. No se trata de un NFR sin tratamiento, sino de un tratamiento alternativo justificado y trazado. No constituye P0.

### 4.2 Separación publicación-de-paquete vs despliegue-de-servicio

Verificada en `pipeline-ci-cd §0/§4`, `entornos-deploy §0/§1.1/§1.2`, ambas guías de publicación y los dos README. El monolito de tres contenedores se promueve por ambientes DEV/QA/STAGING/PROD con rollback por reversión de deploy; el paquete `GeoVial.Sync` se publica por canales preview/stable con rollback por delist/deprecate + PATCH. Los dos planos se declaran "independientes" y no se cruzan en ningún documento. Rollback diferenciado por tipo de artefacto presente (pipeline §5.1 imágenes, §5.2 paquete, §5.3 tag). No se observa el anti-patrón "confundir publicación con despliegue".

### 4.3 Paridad referencia-api ↔ contrato (10 ↔ 05)

Cotejo uno a uno de `referencia-api_v1.0.md` contra `contratos-abstractions-sync_v1.0.md`:

| Elemento del contrato | En contrato | En referencia-api | Estado |
| --- | --- | --- | --- |
| `IChangeQueue` (§3) | Sí | Sí (§2) | OK |
| `ISyncEngine` (§3) | Sí | Sí (§2) | OK |
| `ISyncBackendClient` (§3) | Sí | Sí (§2) | OK |
| `IConflictReporter` (§3) | Sí | Sí (§2) | OK |
| `IConnectivityMonitor` (§3) | Sí | Sí (§2 + §3 evento) | OK |
| `ChangeRecord` (§4) | Sí | Sí (§1) | OK |
| `SyncResult` (§4) | Sí | Sí (§1) | OK |
| `ConflictInfo` (§4) | Sí | Sí (§1) | OK |
| `SyncOptions` (§4) | Sí | Sí (§1) | OK |
| `ConsolidationException` (§5) | Sí | Sí (§4) | OK |
| `ConflictNotMarkedException` (§5) | Sí | Sí (§4) | OK |
| `SyncInterruptedException` (§5) | Sí | Sí (§4) | OK |
| `PushResponse` | **No** (ausente de §4) | Sí (§2, retorno de `PushAsync`) | **Desvío F-01** |

Los métodos documentados (`EnqueueAsync`, `GetPendingAsync`, `MarkConfirmedAsync`, `DrainConfirmedAsync`, `SynchronizeAsync`, `PushAsync`, `PullAsync`, `PublishAsync`, `GetUnresolvedAsync`, `IsConnected`, `ConnectivityRestored`) se derivan fielmente de las responsabilidades en prosa del contrato §3, que no enumera firmas; la materialización de firmas es competencia de la Reference (§4.5) y no introduce semántica ajena al contrato. El único tipo no contractualizado es `PushResponse` (hallazgo F-01). Los códigos de error de dominio del troubleshooting (`CONSOLIDACION_INVALIDA`, `CONFLICTO_NO_MARCADO`, `SINCRONIZACION_INTERRUMPIDA`) coinciden con el catálogo del contrato §5; no se inventan códigos.

### 4.4 Glosario sin duplicación semántica

El `glosario-tecnico` declara explícitamente que los términos de dominio (relevamiento, marcador, observación, etiqueta) viven en el glosario del cliente y en el modelo conceptual de 02, y los referencia sin redefinir su semántica. Sus 14 términos son de la superficie del consumidor de la librería. No se observa duplicación con semántica distinta respecto de 02/03.

### 4.5 Resolución de enlaces relativos

Se verificó la existencia de los destinos de los enlaces cross-doc: `contratos-abstractions-sync_v1.0.md`, `contratos-rest_v1.0.md`, ADR-05/06/07, README de 05, CU-06/07/12, modelo-conceptual de 02, RC-03, RN-02, definition-of-done, estrategia-testing, casos-prueba-referenciales y criterios-validacion: todos existen. Los enlaces internos de 10 entre sí y los de los README resuelven. Cada documento de 10 tiene al menos un enlace a 05 (conceptos 4, onboarding 2, integración 2, referencia 4, troubleshooting 2, glosario 3, README 3).

---

## 5. Hallazgos enumerados

### F-01 — P1 alto — `PushResponse` ausente del contrato de 05

- **Archivo:** `10_developer_guide/referencia-api_v1.0.md` §2 (`ISyncBackendClient`, nota al pie del bloque); también usado en `guia-onboarding-developer` §3 y `guia-integracion-aplicacion-movil` §3.
- **Sección:** §2 Métodos / firma de `PushAsync`.
- **Evidencia:** `referencia-api` documenta `Task<PushResponse> PushAsync(...)` y define `PushResponse { Confirmed: IReadOnlyList<string>, Conflicts: IReadOnlyList<ConflictInfo> }` como tipo de retorno público del puerto. El contrato `contratos-abstractions-sync_v1.0.md` §4 enumera exactamente cuatro tipos públicos (`ChangeRecord`, `SyncResult`, `ConflictInfo`, `SyncOptions`); `PushResponse` no figura (regex sobre 05: 0 coincidencias). La propia referencia-api afirma "no se documenta ningún tipo ni método ausente del contrato", afirmación que `PushResponse` contradice.
- **Lectura de severidad:** los criterios de Fase F listan "referencia-api con tipos inexistentes en el contrato" como gatillo P0. Se atenúa a **P1** porque `PushResponse` es un DTO de transporte cuyos campos ya están contractualizados (`Confirmed` y `Conflicts` son la misma forma que alimenta `SyncResult`), por lo que no fabrica semántica nueva ni superficie de dominio; el desvío es de completitud del contrato, no de invención conceptual. Se deja anotada la lectura estricta P0 para decisión del orquestador.
- **Recomendación:** una de dos. (a) Incorporar `PushResponse` al contrato `contratos-abstractions-sync_v1.0.md` §4 como tipo público de la superficie de `ISyncBackendClient` (con bump del contrato), recuperando la paridad uno a uno; o (b) reexpresar la firma de `PushAsync` en la referencia-api sobre tipos ya contractualizados (por ejemplo retornar la porción de `SyncResult` correspondiente) y eliminar `PushResponse` de la superficie documentada. Opción (a) es la natural dado que el puerto necesita un tipo de retorno propio.

### F-02 — P2 medio — Enlaces a ISSUE sin ancla de fragmento

- **Archivo:** `10_developer_guide/guia-integracion-aplicacion-movil_v1.0.md` §5; análogamente las referencias `[ISSUE-0X](troubleshooting_v1.0.md)`.
- **Sección:** §5 Troubleshooting específico.
- **Evidencia:** los enlaces apuntan al archivo `troubleshooting_v1.0.md` sin fragmento (`#issue-0x`), de modo que resuelven al documento pero no al ítem concreto. El troubleshooting no expone anclas explícitas por ISSUE.
- **Recomendación:** agregar anclas de encabezado por ISSUE en `troubleshooting` (los H3 ya existen en §2) y enlazar con fragmento, para que el salto caiga en el diagnóstico puntual. No rompe trazabilidad; es mejora de navegación.

### F-03 — P2 medio — Estado de cabecera dispar entre 09 y 10

- **Archivos:** los 7 de 09 declaran `Estado: Propuesto`; los 7 de 10 declaran `Estado: Vigente`.
- **Sección:** cabecera de metadatos.
- **Evidencia:** ambos conjuntos comparten fecha 2026-06-01 y son de la misma fase, pero 09 está "Propuesto" y 10 "Vigente". Dado que 10 (developer guide de la librería) consume comandos y canales definidos en 09 (pipeline, estrategia-versionado, guías de publicación), declarar 10 "Vigente" mientras su upstream de 09 sigue "Propuesto" es una incoherencia de madurez.
- **Recomendación:** unificar el estado de la fase (ambos "Propuesto" o ambos "Vigente") o documentar por qué 10 se considera vigente sobre un 09 aún propuesto. Es de gobernanza documental, no de contenido.

### F-04 — P2 medio — `SyncFactory` y métodos `*Async` sin entrada en referencia-api

- **Archivos:** `guia-onboarding-developer`, `guia-integracion-aplicacion-movil`, README de 10 (quick-start) usan `SyncFactory.CreateInMemoryQueue/CreateSqliteQueue/CreateEngine/CreateConnectivityMonitor`.
- **Sección:** snippets de tutorial/how-to/quick-start.
- **Evidencia:** `SyncFactory` es el punto de entrada de fábrica usado en todos los ejemplos copy-paste, pero no aparece en `referencia-api` ni en el contrato. Si es parte de la superficie pública consumible, la Reference debería listarlo; si es conveniencia de los ejemplos, conviene aclararlo. No es violación de paridad con el contrato (el contrato no lo menciona y la referencia-api tampoco), pero deja un punto de entrada usado sin documentar en la Reference.
- **Recomendación:** decidir si `SyncFactory` es superficie pública. Si lo es, incorporarlo al contrato de 05 y a la referencia-api; si es helper de ejemplo, anotarlo como tal en el onboarding para no inducir a tratarlo como API estable.

### F-05 — P3 bajo — `RetryPolicy` como tipo de `SyncOptions` sin definición propia

- **Archivo:** `referencia-api_v1.0.md` §1 (`SyncOptions`).
- **Sección:** §1 Tipos públicos.
- **Evidencia:** `SyncOptions.RetryPolicy` es de tipo `RetryPolicy`; el contrato §4 lista `SyncOptions { BatchSize, RetryPolicy }` pero no define el tipo `RetryPolicy` como esquema. La referencia-api lo deja como tipo sin tabla de propiedades. Es coherente con el contrato (que tampoco lo detalla), por eso es P3 y no paridad.
- **Recomendación:** detallar mínimamente la forma de `RetryPolicy` (o aclarar "definido por la implementación") tanto en el contrato como en la referencia, para cerrar la cadena de tipos.

### F-06 — P3 bajo — Workflows reales aún no materializados

- **Archivo:** `09_devops/README.md` (Plataforma y workflows) y `pipeline-ci-cd`.
- **Sección:** Plataforma y workflows.
- **Evidencia:** el README declara que los workflows de `.github/workflows/` están "pendientes de materialización"; el pipeline define stages y comandos reproducibles por scripts BAT. Es coherente con el estado "Propuesto" y con la práctica SDD (el documento precede al YAML), pero deja la verificación de ejecución real fuera de alcance documental.
- **Recomendación:** al pasar 09 a "Vigente", adjuntar o enlazar los workflows reales y los scripts BAT referenciados (`build-*.bat`, `publish-*.bat`) para cerrar la reproducibilidad declarada.

---

## 6. Veredicto final

**APROBADO CON OBSERVACIONES.**

Condiciones para el cierre:

1. **Resolver F-01 (P1)** antes de declarar la referencia-api "paridad uno a uno" cerrada: incorporar `PushResponse` al contrato de 05 o reexpresar la firma de `PushAsync` sobre tipos ya contractualizados. Es la única condición de bloqueo blando; con lectura estricta del criterio sería P0, por lo que se recomienda atenderla en el próximo PR de la fase.
2. **Atender F-02, F-03 y F-04 (P2)** como mejoras de navegación, coherencia de estado y completitud de la superficie de fábrica; no bloquean la promoción.
3. **F-05 y F-06 (P3)** quedan como deuda menor para el pase a "Vigente".

Ambas categorías cumplen la estructura obligatoria, la conformidad D1–D8, la separación publicación-vs-despliegue, la cobertura de gates↔DoD/NFR y la trazabilidad a 05/08/02 e intakes. No se hallaron P0. La Fase F es promovible con la observación P1 registrada.

---

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Auditoría inicial de la Fase F (09 DevOps y 10 Developer guide) de GeoVial. Verificación empírica de los 14 archivos, conformidad D1–D8, estructura obligatoria, coherencia gates↔DoD/NFR, separación publicación-vs-despliegue, paridad referencia-api↔contrato y resolución de enlaces. 0 P0, 1 P1 (PushResponse fuera del contrato), 3 P2, 2 P3. Veredicto: APROBADO CON OBSERVACIONES. |
