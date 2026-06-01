# Auditoría Fase G — 11 Examples — GeoVial

**Proyecto:** GeoVial
**Documento:** fase-g_v1.0.md
**Categoría auditada:** 11_examples (4 archivos: README + 3 ejemplos)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación de la Fase G)
**Fecha:** 2026-06-01
**Reglas de referencia:** `/sdd2.0/devs/rules/11_rules_examples.md`
**Método:** lectura exhaustiva de los 4 archivos de la categoría; conteo programático de secciones (`## N`) y de pasos numerados en §4; validación de encoding con iconv y scan de CRLF/BOM; regex de filenames (`_v` vs `.v`, kebab-case ASCII, token de framework); grep de `PushResponse` y de `maui` en nombres de archivo de docs; grep de vocabulario prohibido del bootstrap (Motor DSL, impresoras térmicas, ESC-POS, Bluetooth de impresión); resolución de enlaces relativos a 02/05/09/10; cotejo de cada ID upstream (CU-01..14, ADR-01..14, RN-01..08, RC-01..07) contra los archivos reales; verificación de paridad de API abriendo `contratos-abstractions-sync_v1.0.md` (05) y `referencia-api_v1.0.md` (10) y enumerando tipos/métodos/factories uno a uno; cotejo de carpetas `/samples` contra PROJECT-README §5/§5.X y de gating contra PROJECT-README §1. No se reauditó el upstream; sólo se verificó la existencia y coherencia de los IDs y firmas referenciados.

---

## 1. Resumen ejecutivo

La Fase G de GeoVial está completa, internamente consistente y trazable de punta a punta. Se verificaron empíricamente los 4 archivos de `docs/11_examples/`. Los artefactos obligatorios existen con sus secciones completas:

- **README.md**: tabla maestra con las cinco columnas exigidas (Sample / Nivel / Tiempo de setup / CU ilustrados / Ubicación en `/samples`), tres samples declarados, decisión de gating justificada (D8 dominante `web-monolith` —donde 11 sería sólo recomendada— promovida a generada por los sub-proyectos `library` y por `tiene_portal_developers: true`, exactamente lo que declara PROJECT-README §1), convenciones, procedimiento para agregar un sample, vínculo con 10 y 05, y validación de ejecutabilidad en CI apoyada en STAGE-11 de 09 (real). Cubre las cinco secciones de §4.3 de la regla.
- **ejemplo-01-sync-basico_v1.0.md** (Básico), **ejemplo-02-demo-movil-autonoma_v1.0.md** (Intermedio) y **ejemplo-03-filehosting-backends_v1.0.md** (Avanzado): los tres con las **nueve** secciones obligatorias de §4.2 en orden, cabecera completa de §4.1 (incluyendo Nivel y Ubicación del código), exactamente **cinco** pasos en §4, prerequisites con versiones en §3, output esperado literal en §6 y trazabilidad en §8 con más de una fila cada uno.

Conformidad D1–D8 satisfactoria: idioma rioplatense con tildes/eñes correctas (verificado: extensión, sincronización, inyección, configuración, señal); UTF-8 válido sin BOM y line endings LF en los 4 archivos; filenames kebab-case ASCII; versionado `_v1.0.md` uniforme (cero ocurrencias de `.v1.0`); `README.md` sin sufijo por convención de índice; trazabilidad D6 presente; tipo D8 cerrado (`web-monolith` con sub-proyectos `library`). No se detectó vocabulario prohibido del bootstrap (Motor DSL, impresoras térmicas, ESC-POS, Bluetooth de impresión): cero ocurrencias. El stack real (.NET 9, MAUI, SQLite, AWS S3, Docker, MinIO, GitHub Packages) aparece legítimamente en el CUERPO de los documentos.

Nomenclatura por progresión/capacidad confirmada: los slugs son `sync-basico`, `demo-movil-autonoma`, `filehosting-backends` —ninguno ata el sample a una entidad del dominio del producto (relevamiento, observación, marcador) ni reincide en el antipatrón del fuente (`multa`, `multaapp-nuget`)—. El token de framework `maui` **no** aparece en ningún nombre de archivo de docs (grep `find docs -iname "*maui*"`: cero resultados); vive sólo en la carpeta `/samples/02-sync-maui-demo/`, que es la materialización de código gobernada por PROJECT-README §5.X y donde el token de stack es legítimo. La correspondencia markdown↔carpeta es 1:1 y coincide exactamente con PROJECT-README §5 y §5.X (`01-sync-basico`, `02-sync-maui-demo`, `03-filehosting-backends`).

Consistencia de API con el contrato de 05: **plena**. El grep de `PushResponse` sobre `docs/11_examples/` arroja **cero** ocurrencias; los samples usan exclusivamente `SyncResult` (con `Confirmed[]`, `Conflicts[]`, `Updates[]`), tal como lo contractualiza `contratos-abstractions-sync_v1.0.md` §4 y lo refleja `referencia-api_v1.0.md`. Todas las interfaces (`IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, `IConflictReporter`, `IConnectivityMonitor`), tipos (`ChangeRecord`, `ConflictInfo` con `Kind ∈ {FieldConflict, MarkersWithinRadius}`, `SyncOptions` con `BatchSize`), métodos (`SynchronizeAsync`, `PushAsync`, `PullAsync`, `EnqueueAsync`, `GetPendingAsync`) y factories (`SyncFactory.CreateInMemoryQueue`, `CreateSqliteQueue`, `CreateEngine`, `CreateConnectivityMonitor`) usados en los snippets existen en el contrato (05) y/o en la referencia-api y las guías de 10 (`guia-onboarding-developer`, `guia-integracion-aplicacion-movil`, README de 10). No se invoca ningún método o tipo inexistente.

Todos los IDs upstream referenciados existen: CU-04/06/07/08/09/12 (de CU-01..14), ADR-05/06/07/08 (de ADR-01..14), RN-02/04/05 (de RN-01..08), RC-03 (de RC-01..07). El sample avanzado ilustra correctamente el punto de extensión de `extensibilidad_v1.0.md` (backend de almacenamiento configurable local/S3), que el propio `extensibilidad_v1.0.md` §5 referencia de vuelta a `samples/03-filehosting-backends`. Todos los enlaces relativos resuelven a archivos existentes.

No se hallaron violaciones D1–D8, ni vocabulario prohibido, ni nombre por dominio o con token de framework en filename, ni falta de sufijo `_v1.0.md`, ni sample sin trazabilidad a CU, ni API inconsistente con el contrato (sin `PushResponse`, sin método inexistente), ni markdown con menos de nueve secciones, ni output esperado ausente, ni §4 con más de cinco pasos, ni prerequisites sin versiones. No se identificó ningún P0 ni P1. Los hallazgos son P2/P3 menores que no rompen trazabilidad ni conformidad estructural.

### Conteo de hallazgos por nivel

| Nivel | Cantidad |
| --- | --- |
| P0 bloqueante | 0 |
| P1 alto | 0 |
| P2 medio | 2 |
| P3 bajo | 2 |
| **Total** | **4** |

### Veredicto

**APROBADO.** Los entregables de la categoría 11_examples son promovibles a estado Vigente sin condiciones bloqueantes. Las observaciones P2/P3 son de mejora opcional y no exigen reemisión.

---

## 2. Matriz de conformidad D1–D8 por documento

Convenciones: OK = cumple; n/a = no aplica al tipo de documento.

D1 idioma rioplatense con tildes/eñes · D2 UTF-8/LF (sin CRLF, sin BOM) · D3 filename kebab-case ASCII sin token de framework ni entidad de dominio del producto · D4 versionado `_v1.0.md` (no `.v1.0.md`; README exento por convención) · D5 IDs upstream uniformes y existentes (CU/ADR/RN/RC) · D6 trazabilidad en cabecera/§8 · D7 sin vocabulario prohibido del bootstrap · D8 tipo D8 cerrado (web-monolith con sub-proyectos library).

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `README.md` | OK | OK | OK | n/a | OK | OK | OK | OK |
| `ejemplo-01-sync-basico_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `ejemplo-02-demo-movil-autonoma_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `ejemplo-03-filehosting-backends_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |

Notas de verificación empírica:
- D2: `iconv -f UTF-8` válido para los 4; `file` reporta LF (sin CRLF); primeros 3 bytes sin firma BOM `ef bb bf`.
- D3: grep de `maui` en nombres de archivo de docs = 0; ningún slug contiene entidad de dominio (relevamiento, observacion, marcador, foto-como-entidad). `filehosting` y `sync` son capacidades, no entidades del dominio del producto.
- D4: cero ocurrencias del patrón heredado `.v1.0`; los tres ejemplos llevan `_v1.0.md`.
- D7: grep `ESC-POS|impresora|térmica|bluetooth|motor dsl|dsl` = 0.

---

## 3. Matriz de estructura obligatoria

### 3.1 README (§4.3 de la regla)

| Requisito | Estado | Evidencia |
| --- | --- | --- |
| Tabla maestra con columnas Sample/Nivel/Tiempo de setup/CU ilustrados/Ubicación | OK | §2, líneas 31-35; las cinco columnas presentes |
| 3 samples listados (mínimo `library` §2.2) | OK | tres filas: básico, intermedio, avanzado |
| Tiempo de setup por sample | OK | `< 5 min`, `10-15 min`, `15-20 min` |
| Propósito de la carpeta | OK | §1 |
| Convenciones | OK | §3 |
| Cómo agregar un sample (ref a §6 de la regla) | OK | §4 |
| Vínculo con 10 y 05 | OK | §5 (tabla guía↔contrato) |
| Correspondencia 1:1 markdown↔carpeta coherente con §5.X | OK | §2 nota + cotejo PROJECT-README §5.X |

### 3.2 Markdown explicativo de cada sample (§4.1 cabecera + §4.2 nueve secciones)

Conteo programático de secciones `## N.` y de pasos numerados en §4:

| Sample | Cabecera Nivel | Cabecera Ubicación | Secciones (9) | Pasos §4 (≤5) | Output §6 | Prereqs c/versión §3 | Trazab. §8 (≥1 fila) |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `ejemplo-01-sync-basico_v1.0.md` | Básico | `/samples/01-sync-basico/` | 9/9 | 5 | OK | OK | 6 filas |
| `ejemplo-02-demo-movil-autonoma_v1.0.md` | Intermedio | `/samples/02-sync-maui-demo/` | 9/9 | 5 | OK | OK | 9 filas |
| `ejemplo-03-filehosting-backends_v1.0.md` | Avanzado | `/samples/03-filehosting-backends/` | 9/9 | 5 | OK | OK | 6 filas |

Las nueve secciones, en los tres, aparecen en el orden exacto de §4.2: 1 Objetivo, 2 Nivel, 3 Prerequisites, 4 Cómo correrlo, 5 Estructura del código, 6 Qué esperar, 7 Variaciones sugeridas, 8 Trazabilidad, 9 Control de cambios. El output de §6 es texto literal de consola en los tres (incluye doble bloque local/S3 en el 03). Prerequisites declaran versión mínima: .NET 9 o superior, Android 8.0 (API 26), Docker con contenedores Linux, MinIO/emulador S3.

### 3.3 Cobertura del piso de samples `library` (§2.2)

`library` exige 3 samples (básico + intermedio + avanzado). Presentes los tres, con niveles distintos y progresión declarada (cada §2 justifica respecto al anterior). Cumple el piso.

---

## 4. Coherencia cross-doc y cross-categoría

### 4.1 Tabla sample → CU/ADR/RN/RC y verificación de existencia

| Sample | CU citados | ADR citados | RN/RC/NFR citados | ¿Todos existen upstream? |
| --- | --- | --- | --- | --- |
| 01-sync-basico | CU-06, CU-07 | ADR-05, ADR-07 | RC-03 | Sí — CU-06/07 en 02; ADR-05/07 en 05/adrs; RC-03 en reglas-conceptuales-de-modelo |
| 02-sync-maui-demo | CU-06, CU-07, CU-12 | ADR-05, ADR-06, ADR-07 | RN-04; RN-02 (en §7); NFR "Detección de conectividad" (PROJECT-README §13) | Sí — CU-06/07/12 en 02; ADR-05/06/07 en 05/adrs; RN-02/04 en reglas-de-negocio; NFR sin ID formal (ver hallazgo G-01) |
| 03-filehosting-backends | CU-04, CU-08, CU-09 | ADR-08 | RN-05 | Sí — CU-04/08/09 en 02; ADR-08 en 05/adrs; RN-05 en reglas-de-negocio; CU-04/08/09 coinciden con `extensibilidad_v1.0.md` §"CU que lo consumen" |

No hay IDs inexistentes. Los CU del sample 03 (CU-04 guardar foto, CU-08 exportar/importar con fotos, CU-09 carrusel) coinciden exactamente con la fila "CU que lo consumen" de `extensibilidad_v1.0.md`. RC-03 (unicidad de identificador de cola sync) sostiene la idempotencia por `ChangeId` que el sample 01 demuestra en su segunda corrida, congruente con la nota del contrato (`RC | RC-03`).

### 4.2 Verificación de consistencia de API con el contrato (05) y la referencia (10)

| Símbolo usado en snippets de 11 | ¿En contrato 05? | ¿En referencia-api/guías 10? | Veredicto |
| --- | --- | --- | --- |
| `SyncResult` (Confirmed/Conflicts/Updates) | Sí (§4) | Sí | Consistente |
| `PushResponse` | No existe (correcto) | No existe (correcto) | **Ausente — correcto**; grep en 11 = 0 |
| `IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, `IConflictReporter`, `IConnectivityMonitor` | Sí (§3) | Sí | Consistente |
| `ChangeRecord`, `ConflictInfo` (FieldConflict / MarkersWithinRadius), `SyncOptions` (BatchSize) | Sí (§4) | Sí | Consistente |
| `SynchronizeAsync`, `PushAsync`, `PullAsync` | Sí (semántica §3) | Sí (firmas referencia-api §) | Consistente |
| `EnqueueAsync`, `GetPendingAsync` | Sí (IChangeQueue) | Sí (referencia-api) | Consistente |
| `SyncFactory.CreateInMemoryQueue / CreateSqliteQueue / CreateEngine / CreateConnectivityMonitor` | No enumerado en contrato | Sí — `guia-onboarding-developer`, `guia-integracion-aplicacion-movil`, README de 10 | Consistente con 10 (ver nota) |
| `ConnectivityRestored` (evento) | Sí (IConnectivityMonitor) | Sí (referencia-api) | Consistente |

Nota sobre `SyncFactory`: el contrato de 05 enumera interfaces y tipos de datos, no la fábrica de construcción. Las factories `Create*` figuran de forma idéntica en tres documentos de 10 (la developer guide oficial), que es la superficie pública documentada que la regla §3.3 obliga a consumir. Por tanto los snippets de 11 son consistentes con la superficie pública declarada upstream y no inventan API. No constituye hallazgo.

Resultado del grep solicitado: `PushResponse` en `docs/11_examples/` = 0 ocurrencias; `maui` en nombres de archivo de docs = 0 ocurrencias.

### 4.3 Carpetas `/samples` vs PROJECT-README §5.X

| Carpeta documentada en 11 | ¿En PROJECT-README §5 y §5.X? | Nivel coincide |
| --- | --- | --- |
| `/samples/01-sync-basico/` | Sí (§5 línea 129, §5.X) | Básico = Básico |
| `/samples/02-sync-maui-demo/` | Sí (§5 línea 130 "requerida", §5.X) | Intermedio = Intermedio |
| `/samples/03-filehosting-backends/` | Sí (§5 línea 131, §5.X) | Avanzado = Avanzado |

Coincidencia exacta de nombres y niveles. El sample 02 corresponde a la demo MAUI autónoma "ajena al sistema" requerida explícitamente por el cliente (PROJECT-README §14), correctamente reflejada en el objetivo del markdown.

### 4.4 Punto de extensión en el sample avanzado

`ejemplo-03-filehosting-backends_v1.0.md` ilustra el punto de extensión de `extensibilidad_v1.0.md` (backend de almacenamiento): misma operación de subida contra backend local y AWS S3 sin tocar el dominio, resolución por DI y configuración (`appsettings.json`), idempotencia de Guardar y plantilla para un tercer backend. Coincide con §2/§3/§4/§5 de `extensibilidad_v1.0.md`, y este último referencia de vuelta a `samples/03-filehosting-backends`. Referencia a `Foto.ReferenciaArchivo` (modelo lógico §1.7) verificada como existente.

### 4.5 Resolución de enlaces relativos

Todos los documentos referenciados resuelven a archivos existentes: `contratos-abstractions-sync_v1.0.md` y `extensibilidad_v1.0.md` (05), `referencia-api_v1.0.md`, `guia-onboarding-developer_v1.0.md`, `conceptos-fundamentales_v1.0.md`, `guia-integracion-aplicacion-movil_v1.0.md` (10), `pipeline-ci-cd_v1.0.md` (09), los CU de `02_especificacion_funcional/casos-de-uso/`, y `PROJECT-README-geovial_v1.0.md` (intake). Ningún enlace roto.

---

## 5. Verificación de §6 de la regla (14 criterios)

| # | Criterio §6 | Estado | Evidencia |
| --- | --- | --- | --- |
| 1 | README con tabla maestra (nivel, tiempo de setup, CU, ubicación) | OK | README §2 |
| 2 | Samples mínimos para D8 (`library` = 3) | OK | tres samples presentes |
| 3 | Cada markdown con las nueve secciones | OK | 9/9 en los tres (conteo §3.2) |
| 4 | Ejecutable en ≤5 pasos copiables | OK | 5 pasos en §4 de los tres |
| 5 | Nivel declarado explícito en §2 | OK | Básico / Intermedio / Avanzado |
| 6 | Trazabilidad a CU/ADR/NFR en §8 (≥1 fila) | OK | 6 / 9 / 6 filas |
| 7 | Nombres por progresión/capacidad, no por dominio | OK | slugs `sync-basico`, `demo-movil-autonoma`, `filehosting-backends` |
| 8 | Sufijo `_v<X.Y>.md` en todos los markdown | OK | `_v1.0.md` en los tres |
| 9 | README lista samples con todas las columnas de §4.4 | OK | cinco columnas |
| 10 | Tiempo de setup en la tabla maestra | OK | tres valores |
| 11 | Output esperado en §6 con texto exacto | OK | bloques de consola literales |
| 12 | Prerequisites con versiones mínimas en §3 | OK | .NET 9+, Android 8.0, Docker, MinIO |
| 13 | Estructura `/samples` coincide con matriz §2.3 según D8 | Parcial-OK | carpetas coinciden con PROJECT-README §5.X (ver hallazgo G-02) |
| 14 | Pipeline CI valida compilación/ejecución (obligatorio `library`) | OK | README §6 + STAGE-11 real en 09 |

---

## 6. Hallazgos enumerados

### G-01 (P2) — NFR de conectividad referenciado por nombre, sin ID formal

- **Archivo / sección:** `ejemplo-02-demo-movil-autonoma_v1.0.md` §8 (fila de trazabilidad) y §2.
- **Evidencia:** la fila cita `NFR "Detección de conectividad" (PROJECT-README §13)` por nombre. En el upstream, los NFR de PROJECT-README §13 se expresan como tabla nombre→métrica sin ID formal tipo `NFR-XX` (a diferencia del Ejemplo 2 genérico de la regla, que usa `NFR-01`). La referencia por nombre es trazable y verificable (el NFR existe textualmente en §13: "Detección de conectividad: Automática…"), pero no aporta un ID estable.
- **Por qué P2 y no P1:** el criterio de la auditoría declara aceptable la trazabilidad de un NFR por nombre cuando los NFR no tienen ID formal en el upstream, que es exactamente este caso. No rompe trazabilidad.
- **Recomendación:** si en una revisión futura 05/00 asigna IDs formales a los NFR, actualizar la fila para citar el ID. Sin acción obligatoria en v1.0.

### G-02 (P2) — Slugs de `/samples` se alejan de los ejemplos canónicos de §2.3, aunque coinciden con PROJECT-README §5.X

- **Archivo / sección:** README §2 (Ubicación) y cabecera "Ubicación del código" de los tres markdown.
- **Evidencia:** §2.3 de la regla propone para `library` los slugs canónicos `01-basico-consola`, `02-intermedio-con-extensiones`, `03-avanzado-integracion-real`. Las carpetas reales son `01-sync-basico`, `02-sync-maui-demo`, `03-filehosting-backends`. La propia regla admite progresión por capacidad además de por nivel y prohíbe sólo el nombrado por dominio del producto; los slugs elegidos son por capacidad (sync, filehosting), no por entidad del dominio, y coinciden 1:1 con PROJECT-README §5.X, que es la fuente vinculante de la materialización. La discrepancia es respecto a los ejemplos ilustrativos de §2.3, no respecto a una prohibición.
- **Por qué P2 y no P1:** §2.3 es "vinculante" en cuanto a no renombrar por dominio, lo que aquí se respeta; los nombres provienen del intake. `02-sync-maui-demo` incorpora el token de stack `maui` sólo en la carpeta de código (permitido), no en el filename de docs.
- **Recomendación:** mantener los slugs alineados a PROJECT-README §5.X (prevalece el intake). Opcionalmente, anotar en la regla que los ejemplos de §2.3 son orientativos cuando el intake fija nombres por capacidad.

### G-03 (P3) — Variación del sample 01 introduce un símbolo (`SyncResult.Conflicts`) que recién se ejercita en el sample 02

- **Archivo / sección:** `ejemplo-01-sync-basico_v1.0.md` §7 (tabla de variaciones), fila "Simular un conflicto de campo".
- **Evidencia:** la variación menciona `SyncResult.Conflicts` y `ConflictInfo`/`FieldConflict` como "puente hacia el sample 02". Es un símbolo contractualizado y correcto, pero el sample básico declara en §2 que no maneja conflictos; el puente es deliberado y está rotulado como tal.
- **Por qué P3:** es estilístico/didáctico; no hay inconsistencia de API (los tipos existen en el contrato) ni violación estructural. La sección §7 admite explícitamente servir de puente al sample siguiente (§4.2.7 de la regla).
- **Recomendación:** ninguna obligatoria; la redacción ya aclara que es un puente.

### G-04 (P3) — README §6 ata la validación de CI a un comando concreto (`dotnet pack src/GeoVial.Sync`)

- **Archivo / sección:** `README.md` §6.
- **Evidencia:** el README transcribe `dotnet pack src/GeoVial.Sync` y nombra STAGE-11 y los gates de PROJECT-README §11. El dato es correcto y verificable hoy (STAGE-11 existe en `pipeline-ci-cd_v1.0.md` con ese comando), pero acopla la prosa del README de 11 a un comando que pertenece a 09; un futuro refactor del pipeline obligaría a editar también este README.
- **Por qué P3:** es un acoplamiento estilístico de mantenimiento, no un error de contenido. La trazabilidad es correcta.
- **Recomendación:** opcionalmente, referir a STAGE-11 por su nombre y dejar el comando exacto sólo en 09, para reducir duplicación.

---

## 7. Veredicto final

**APROBADO.**

La categoría 11_examples de GeoVial cumple la regla constructiva `11_rules_examples.md` y los criterios D1–D8: README con tabla maestra completa, tres samples `library` con las nueve secciones, cabecera con Nivel y Ubicación, ≤5 pasos, output esperado literal, prerequisites versionados y trazabilidad a CU/ADR/RN/RC reales. La consistencia de API con el contrato de 05 es plena (sin `PushResponse`, sin métodos inexistentes, `SyncResult` en todo caso). La nomenclatura es por capacidad/progresión sin token de framework en filenames y sin entidad de dominio del producto; las carpetas `/samples` coinciden con PROJECT-README §5.X; el sample avanzado ilustra el punto de extensión de `extensibilidad_v1.0.md`. No se identificaron hallazgos P0 ni P1. Los cuatro hallazgos son P2/P3 de mejora opcional y no condicionan la promoción a Vigente.

---

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Auditoría inicial de la Fase G (11_examples): matriz D1–D8, matriz de estructura obligatoria, verificación de §6 (14 criterios), coherencia cross-doc con tabla sample→CU/ADR/RN/RC y consistencia de API contra el contrato de 05, cuatro hallazgos (0 P0, 0 P1, 2 P2, 2 P3) y veredicto APROBADO. Auditor independiente. |
