# Auditoría Fase C — 05 Arquitectura técnica — GeoVial

**Proyecto:** GeoVial
**Documento:** fase-c_v1.0.md
**Categoría auditada:** 05_arquitectura_tecnica (recursivo, incluye `adrs/`)
**Auditor:** Arquitecto de Soluciones + QA Senior (independiente, no participó de la generación de la Fase C)
**Fecha:** 2026-06-01
**Regla de referencia:** `/sdd2.0/devs/rules/05_rules_arquitectura_tecnica.md`
**Método:** lectura exhaustiva de los 8 documentos maestros + 14 ADR; verificación empírica con conteo de secciones, regex de IDs, resolución de enlaces, cotejo de tildes/eñes, line endings y BOM; cotejo cruzado contra 00, 01, 02 (CU-01..14, RN-01..08, RC-01..07, modelo conceptual de 12 entidades) y los intakes (README §13 NFR, §15 pre-ADR).

---

## 1. Resumen ejecutivo

La categoría 05 de GeoVial está completa, internamente consistente y trazable de punta a punta. Se verificaron los 22 archivos exigidos (8 documentos maestros + 14 ADR individuales). Todos los criterios bloqueantes se cumplen: no hay ADR consolidada (cada decisión vive en su archivo bajo `adrs/`), todas las ADR declaran Estado y Categoría, no se detectó vocabulario prohibido del bootstrap, no hay referencias a IDs inexistentes, cada CU-01..14 tiene componente en la vista lógica, cada RN-01..08 está reflejada, los NFR llevan métrica numérica y mecanismo, y la nomenclatura usa `_v1.0.md` (nunca `.v1.0.md`) con IDs de ADR de dos dígitos contiguos ADR-01..ADR-14.

### Conteo de hallazgos por nivel

| Nivel | Cantidad |
| --- | --- |
| P0 bloqueante | 0 |
| P1 alto | 0 |
| P2 medio | 2 |
| P3 bajo | 3 |
| **Total** | **5** |

### Veredicto

**APROBADO CON OBSERVACIONES.** Los entregables se pueden promover a 06. Las observaciones son menores (un NFR que arrastra un supuesto abierto declarado fielmente desde upstream, y detalles estilísticos); ninguna bloquea ni rompe trazabilidad. No requieren reproceso para avanzar; se recomiendan como pulido para la versión que cierre el supuesto.

---

## 2. Matriz de conformidad D1-D8 por documento

Convenciones: OK = cumple; n/a = no aplica al tipo de documento.

D1 idioma rioplatense con tildes/eñes · D2 UTF-8/LF · D3 filenames kebab-case ASCII · D4 versionado `_v1.0.md` · D5 IDs ADR dos dígitos contiguos · D6 trazabilidad en cabecera/secciones · D7 sin vocabulario prohibido del bootstrap · D8 tipo cerrado.

### 2.1 Documentos maestros

| Documento | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `arquitectura-solucion_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `decisiones-arquitectura_v1.0.md` | OK | OK | OK | OK | OK | OK | OK | OK |
| `modelo-datos-logico_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `contratos-rest_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `contratos-abstractions-sync_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `flujo-ejecucion_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `extensibilidad_v1.0.md` | OK | OK | OK | OK | n/a | OK | OK | OK |
| `README.md` (sección) | OK | OK | OK | OK | OK | OK | OK | OK |

### 2.2 ADR (14) — agrupados, con excepciones señaladas

Los 14 ADR comparten el mismo perfil de conformidad; no hay excepciones por documento. Verificación empírica:

| Criterio | Resultado sobre los 14 ADR |
| --- | --- |
| D1 tildes/eñes | OK en los 14 |
| D2 UTF-8/LF, sin BOM | OK en los 14 (scan CRLF: ninguno; scan BOM: ninguno) |
| D3 filename kebab-case ASCII | OK; slug lowercase estricto en los 14 (regex de mayúscula tras `ADR-XX-`: 0 coincidencias) |
| D4 `_v1.0.md` (no `.v1.0.md`) | OK en los 14 (patrón `.v[0-9]`: 0 coincidencias) |
| D5 ID dos dígitos contiguo ADR-01..14 | OK; numeración 01..14 sin huecos ni saltos |
| D6 trazabilidad en cabecera/cuerpo | OK; cada ADR tiene §1 Contexto con motivación y §9 Referencias con IDs reales |
| D7 vocabulario del bootstrap | OK; scan de "Motor DSL / ESC-POS / térmica / Bluetooth / impresora": 0 coincidencias |
| D8 tipo cerrado (Categoría del set permitido) | OK; categorías usadas: Estilo, Comunicación, Seguridad, Persistencia, Extensibilidad, Observabilidad, Despliegue (todas del enum §4.1) |

Nota sobre stack real: el uso de .NET, Blazor, MAUI, SQL Server, EF Core, SQLite/sqlite-net-pcl, OSM/Leaflet, ROPC/JWT, OpenAPI, Problem Details RFC 7807, AWS S3, Docker, GitHub Packages, MinVer y EXIF es legítimo y esperado en la capa técnica; no constituye hallazgo D7. El modelo lógico lleva tipos físicos (`uniqueidentifier`, `nvarchar`, `tinyint`, `datetime2(3)`, `decimal(9,6)`, `bit`, tipos SQLite) como corresponde a la capa 05.

---

## 3. Matriz de estructura obligatoria

| Artefacto | Exigencia | Resultado |
| --- | --- | --- |
| `arquitectura-solucion` | 10 secciones de §4.2 | OK: §1 Objetivo, §2 Estilo (con 2 alternativas descartadas), §3 Vista lógica, §4 Vista de procesos, §5 Vista de despliegue, §6 Vista de datos, §7 Cross-cutting, §8 NFR, §9 Riesgos, §10 Trazabilidad |
| `arquitectura-solucion` | 4 vistas mínimas | OK: lógica, procesos, despliegue, datos presentes |
| `arquitectura-solucion` | §7 cross-cutting centralizado | OK: logging, errores, config/secretos, autorización y auditoría en una sola sección |
| `arquitectura-solucion` | §8 NFR objetivo numérico + mecanismo + ADR | OK con 1 excepción parcial (ver H-01) |
| `decisiones-arquitectura` | índice 14 ADR con estado/fecha/categoría, sin cuerpo | OK: tabla §2 con los 14; estado y fecha; sin cuerpo de decisiones |
| Cada ADR (14) | 10 secciones de §4.3 | OK: los 14 tienen exactamente las 10 secciones (conteo H2 = 10 en cada uno) |
| Cada ADR (14) | Estado + Categoría en cabecera | OK: ambos campos presentes en los 14 |
| Cada ADR (14) | ≥2 alternativas con pros/contras | OK: 3 filas de datos (elegido + 2 descartadas) en cada uno |
| Cada ADR (14) | métricas de validación | OK: §8 con métricas en los 14 |
| Cada ADR (14) | referencias a NB/CU/RN/NFR | OK: §9 con IDs reales en los 14 |
| `modelo-datos-logico` | 7 secciones de §4.4 | OK: §1 Tablas, §2 Tipos físicos, §3 Índices, §4 Restricciones, §5 Migración inicial, §6 Multi-tenant, §7 Trazabilidad (más §0 Objetivo opcional) |
| `modelo-datos-logico` | tipos físicos, índices, restricciones | OK |
| `modelo-datos-logico` | migración inicial referenciada | OK: `20260601_InitialCreate` (EF Core) |
| `modelo-datos-logico` | trazabilidad tabla→entidad conceptual (12) | OK: las 12 entidades mapeadas una a una en §7 |
| `contratos-rest` | 7 secciones de §4.5 | OK: alcance, formato, operaciones, esquemas, errores, versionado, trazabilidad |
| `contratos-abstractions-sync` | 7 secciones de §4.5 | OK: idem |
| `flujo-ejecucion` | presente y coherente | OK: 2 pipelines (sync offline-first; georreferenciación EXIF→manual→bandeja) con diagramas y trazabilidad |
| `extensibilidad` | presente y coherente | OK: punto de extensión FileHosting, contrato de backend, registro por config, ejemplo (sample 03) |

---

## 4. Coherencia cross-doc y cross-categoría

### 4.1 Convención crítica §3.3 — ADR individuales

OK. Las 14 decisiones viven en archivos individuales bajo `adrs/`. `decisiones-arquitectura_v1.0.md` es solo índice (sin cuerpo de decisiones). No se detectó ninguna decisión embebida únicamente en el documento maestro sin su ADR: el estilo (ADR-01/ADR-10), persistencia (ADR-09/ADR-05), autenticación (ADR-03), errores (ADR-11), mapas (ADR-04), sync/conflictos (ADR-05/06/07), filehosting (ADR-08), gobernanza (ADR-12/13/14) tienen archivo propio.

### 4.2 Cobertura CU ↔ componente (vista lógica §3)

| CU | Componente(s) en vista lógica | Cubierto |
| --- | --- | --- |
| CU-01 | Módulo de relevamientos y asignación | OK |
| CU-02 | Módulo de acceso y selección | OK |
| CU-03 | Módulo de usuarios y jerarquía | OK |
| CU-04 | Observaciones y marcadores; Georreferenciación | OK |
| CU-05 | Georreferenciación (bandeja sin georreferenciar) | OK |
| CU-06 | Captura local (móvil); Sincronización | OK |
| CU-07 | Módulo de sincronización (GeoVial.Sync) | OK |
| CU-08 | Revisión sobre mapa; Exportación/importación | OK |
| CU-09 | Observaciones y marcadores; Revisión sobre mapa; FileHosting | OK |
| CU-10 | Módulo de estados del relevamiento | OK |
| CU-11 | Módulo de detección de conflictos | OK |
| CU-12 | Módulo de resolución de conflictos | OK |
| CU-13 | Módulo de auditoría y retención | OK |
| CU-14 | Módulo transversal de autorización | OK |

Los 14 CU tienen ≥1 componente. Verificación regex: CU-01..CU-14 presentes en §3 y en la tabla §10.1.

### 4.3 Cobertura RN ↔ componente/ADR

| RN | Reflejada en | ADR |
| --- | --- | --- |
| RN-01 | Autorización transversal | ADR-03, ADR-14 |
| RN-02 | Georreferenciación; detección de conflictos | ADR-04, ADR-06 |
| RN-03 | Georreferenciación | ADR-04 |
| RN-04 | Sincronización; resolución de conflictos | ADR-06 |
| RN-05 | Estados del relevamiento | ADR-09 |
| RN-06 | Acceso y selección | ADR-03, ADR-05 |
| RN-07 | Auditoría y retención | ADR-14 |
| RN-08 | Autorización + auditoría | ADR-14 |

Las 8 RN están reflejadas en componente o ADR sin redefinirse (se vinculan, no se reescriben). OK.

### 4.4 ADR ↔ IDs upstream reales (sin huérfanas, sin IDs inexistentes)

OK. Extracción de referencias por ADR: todos los IDs (ADR-01..14, ADR-001..008 como rótulos de la nota de renumeración, CU-01..14, RN-01..08, RC-01..07) existen en el corpus. Ninguna ADR queda huérfana de motivación: cada §1 Contexto enlaza CU/RN/RC/NFR o decisión del cliente, y cada §9 Referencias lista IDs reales. ADR-09 y ADR-10 motivan vía RC-0x + RN-05 + NFR + modelo conceptual (admisible por §3.4, que acepta NB/CU/RN/NFR).

### 4.5 Modelo lógico ↔ modelo conceptual (12 entidades)

OK. §7 del modelo lógico mapea entidad por entidad: Usuario, Área, Relevamiento, AsignaciónAgente, Observación, Marcador, Foto, Comentario, Etiqueta (+ asociativas), ConflictoSync, RegistroAuditoría y RegistroCambioSync (esquema SQLite local, §1.13). Las 12 entidades del conceptual de 02 quedan cubiertas una a una.

### 4.6 NFR del documento maestro ↔ README §13

| NFR | README §13 | arquitectura §8 | Coincide |
| --- | --- | --- | --- |
| Operación offline | ≥ 1 jornada (8 h) | ≥ 1 jornada (8 h) | OK |
| Tiempo de sincronización | ≤ 5 min / ≈100 obs | ≤ 5 min / ≈100 obs | OK |
| Latencia API lecturas | p95 ≤ 500 ms | p95 ≤ 500 ms | OK |
| Detección de conectividad | automática | automática | OK |
| Disponibilidad backend | SLO 99% horario laboral | SLO 99% horario laboral | OK |
| Tamaño de foto sincronizada | compresión/redimensión (límite a validar) | compresión/redimensión (límite a validar) | OK (supuesto arrastrado fielmente — H-01) |

La arquitectura §8 agrega además "Confiabilidad de georreferenciación ≥ 95%", coherente con NB-02 y el NFR del README de la sección. Los valores numéricos coinciden con §13.

### 4.7 Mapeo pre-ADR README §15 (ADR-001..008 → ADR-01..08)

| Pre-ADR §15 | ADR 05 | Tema | OK |
| --- | --- | --- | --- |
| ADR-001 | ADR-01 | Estilo monolito modular Clean Arch + CQRS ligero | OK |
| ADR-002 | ADR-02 | Backend monolítico expone API REST | OK |
| ADR-003 | ADR-03 | Auth ROPC + JWT refresh condicionado | OK |
| ADR-004 | ADR-04 | Mapas OSM + Leaflet | OK |
| ADR-005 | ADR-05 | SQLite + cola de cambios offline | OK |
| ADR-006 | ADR-06 | Last-write-wins + override manual | OK |
| ADR-007 | ADR-07 | Librería de sync en GitHub Packages | OK |
| ADR-008 | ADR-08 | Librería de alojamiento con backends configurables | OK |

Mapeo 1:1 documentado en `decisiones-arquitectura` §3 y en README de sección. ADR de gobernanza presentes: omisión de 04 (ADR-12), compatibilidad de plataformas (ADR-13), compliance Ley 25.326 (ADR-14). OK.

### 4.8 Categorías obligatorias web-monolith (§2.2)

| Categoría | ADR | OK |
| --- | --- | --- |
| Estilo | ADR-01 (+ ADR-10, ADR-12) | OK |
| Persistencia | ADR-09 (backend) + ADR-05 (móvil) | OK |
| Autenticación | ADR-03 | OK |
| Separación de capas | ADR-10 | OK |
| Manejo de errores | ADR-11 | OK |

Las 5 categorías obligatorias tienen ADR. OK.

### 4.9 Enlaces relativos

OK. Los 21 enlaces del README de sección resuelven a archivos existentes. Las referencias internas a `modelo-datos-logico`, `contratos-rest`, `contratos-abstractions-sync`, `extensibilidad`, `flujo-ejecucion` desde los documentos maestros y ADR apuntan a archivos presentes.

---

## 5. Anti-patrones de §4.7

| Anti-patrón | Estado |
| --- | --- |
| Arquitectura sin ADRs | Ausente (14 ADR aceptadas) |
| ADR sin estado | Ausente (Estado en los 14) |
| ADR consolidada | Ausente (archivos individuales) |
| Estilo implícito | Ausente (§2 declara estilo + justifica contra 2 alternativas) |
| NFR sin métrica | Ausente salvo el caso parcial H-01 (supuesto upstream) |
| Modelo sin migración | Ausente (`20260601_InitialCreate` referenciada) |
| Cross-cutting disperso | Ausente (centralizado en §7) |
| ADR aceptada editada | Ausente (control de cambios v1.0 inicial; ninguna reescritura) |
| Contrato sin versionado | Ausente (REST: URL `/api/v1/`; Abstractions: SemVer 2.0.0) |
| Casing inconsistente | Ausente (kebab-lowercase estricto) |

---

## 6. Hallazgos enumerados

### H-01 (P2 medio) — NFR "Tamaño de foto sincronizada" sin objetivo numérico cerrado

- **Archivo/sección:** `arquitectura-solucion_v1.0.md` §8 (fila "Tamaño de foto sincronizada"); README de sección no lo lista.
- **Evidencia:** la columna "Objetivo numérico" dice "Compresión/redimensión para acotar el payload (límites a validar)"; el mecanismo está fijado (medición del payload por foto antes/después de compresión) pero falta el valor numérico (KB/px). El propio documento lo declara como supuesto a validar (PROJECT-README §13 y Apéndice de supuestos punto 5).
- **Análisis:** no es una invención del generador ni una omisión arbitraria: arrastra fielmente un supuesto abierto desde upstream (README §13 tampoco fija el número). Por eso no se eleva a P0 (NFR sin métrica) — el mecanismo de medición sí está y la falta de número es trazable a un supuesto declarado.
- **Recomendación:** al cerrar el supuesto con el cliente, fijar el límite numérico (por ejemplo, lado mayor ≤ 1920 px y/o ≤ 500 KB por foto) y reflejarlo en §8 y en el README de sección. Hasta entonces, mantener la marca de supuesto.

### H-02 (P2 medio) — Cabecera del documento maestro sin campo "Estado" en transición coherente con ADR "Aceptado"

- **Archivo/sección:** `arquitectura-solucion_v1.0.md`, `modelo-datos-logico_v1.0.md`, `contratos-*`, `flujo-ejecucion`, `extensibilidad` (cabeceras).
- **Evidencia:** los documentos maestros declaran `Estado: Propuesto`, mientras las 14 ADR que los gobiernan están en `Aceptado` y `decisiones-arquitectura` y el README los listan como vigentes.
- **Análisis:** es coherente con el flujo SDD (los maestros de una categoría recién generada quedan "Propuesto" hasta su aprobación de fase; las ADR sí se aceptan individualmente). No rompe trazabilidad ni viola §4.1 (el campo Estado está presente y es un valor del enum). Se registra como observación de consistencia, no como defecto.
- **Recomendación:** al promover la fase, transicionar la cabecera de los maestros a `Aceptado` (o al estado de fase que defina el orquestador) para alinear con el estado de las ADR y del índice.

### H-03 (P3 bajo) — `decisiones-arquitectura` §1 y otros documentos referencian "§3.3" / "§2.2" de la regla sin citar el archivo de regla

- **Archivo/sección:** `decisiones-arquitectura_v1.0.md` §1 y §3; `modelo-datos-logico` y `flujo-ejecucion` mencionan "§2.2".
- **Evidencia:** se citan secciones de la regla por número (regla §3.3, §2.2) sin nombrar `05_rules_arquitectura_tecnica.md`.
- **Recomendación:** estilístico; nombrar el archivo de regla la primera vez para que el revisor externo resuelva la referencia sin ambigüedad.

### H-04 (P3 bajo) — Sample `02-sync-maui-demo` referenciado en contrato pero no en README §14 con ese identificador

- **Archivo/sección:** `contratos-abstractions-sync_v1.0.md` §7 ("samples/01-sync-basico y samples/02-sync-maui-demo").
- **Evidencia:** README §14 del intake nombra `01-sync-basico` y `03-filehosting-backends` y describe un "ejemplo de demostración MAUI" sin numerarlo `02`. El contrato introduce el identificador `02-sync-maui-demo` por inferencia.
- **Análisis:** la demo MAUI existe en §14 (requerida por el cliente); el identificador `02` es una numeración razonable pero no literal del intake. No afecta trazabilidad downstream a 11, que se materializa allí.
- **Recomendación:** confirmar el identificador definitivo del sample MAUI con la categoría 11 para que coincida exactamente.

### H-05 (P3 bajo) — Disponibilidad backend asociada a ADR-13 (Despliegue) en lugar de un ADR de observabilidad/SLO

- **Archivo/sección:** `arquitectura-solucion_v1.0.md` §8 (fila "Disponibilidad del backend → ADR-13").
- **Evidencia:** el NFR de disponibilidad (SLO 99%) se ata a ADR-13 (compatibilidad de plataformas), que efectivamente menciona el SLO en su §8, pero la disponibilidad encaja conceptualmente mejor en una decisión de despliegue/observabilidad dedicada.
- **Análisis:** no hay ruptura: ADR-13 incluye la métrica de disponibilidad y su mecanismo (health check). Es una elección de mapeo defendible para un v1 sin observabilidad crítica (PROJECT-README §1).
- **Recomendación:** estilístico; si en v2 se incorpora observabilidad crítica, mover el SLO a una ADR de observabilidad/despliegue específica.

---

## 7. Verificaciones empíricas realizadas (trazabilidad de la auditoría)

- Conteo de archivos: 8 maestros (incl. README de sección) + 14 ADR = 22. Confirmado.
- Conteo de secciones H2 por ADR: 10 en cada uno de los 14. Confirmado.
- Campos Estado y Categoría: presentes en los 14 ADR. Confirmado.
- Alternativas por ADR: 3 filas de datos (elegido + 2) en los 14. Confirmado.
- Regex de IDs referenciados por ADR: sin IDs inexistentes. Confirmado.
- CU-01..14 en vista lógica §3 y en tabla §10.1: completos. Confirmado.
- RN-01..08 en §10.2: completas. Confirmado.
- 12 entidades conceptuales mapeadas en modelo lógico §7: completas. Confirmado.
- NFR §8 vs README §13: valores numéricos coinciden. Confirmado.
- Scan vocabulario prohibido del bootstrap (Motor DSL, ESC-POS, térmica, Bluetooth, impresora): 0 coincidencias. Confirmado.
- Scan patrón `.v[0-9]` en filenames: 0 coincidencias (todos `_v1.0.md`). Confirmado.
- Scan mayúsculas en slug de ADR: 0 coincidencias (kebab-lowercase). Confirmado.
- Line endings: ningún archivo con CR (todo LF). BOM: ninguno. Tildes/eñes: 22/22 archivos. Confirmado.
- Resolución de enlaces relativos del README de sección: 21/21 OK. Confirmado.

---

## 8. Veredicto final

**APROBADO CON OBSERVACIONES.**

La categoría 05 de GeoVial cumple los criterios bloqueantes y de estructura de la regla `05_rules_arquitectura_tecnica.md`, mantiene trazabilidad íntegra con 00/01/02 y los intakes, y respeta la convención crítica de ADR individuales. No hay P0 ni P1.

### Condiciones para promover a 06 (no bloqueantes; recomendadas)

1. **H-01:** al cerrar el supuesto de tamaño de foto con el cliente, fijar el límite numérico en §8 y en el README de sección.
2. **H-02:** transicionar la cabecera de los documentos maestros de `Propuesto` a `Aceptado` (o al estado de fase definido) al aprobar la fase, para alinearlos con el estado de las ADR.
3. **H-03, H-04, H-05:** pulido estilístico (citar el archivo de regla, fijar el identificador del sample MAUI con la categoría 11, y reconsiderar el mapeo del SLO de disponibilidad en una eventual v2 con observabilidad crítica).

Ninguna condición exige reproceso de los entregables para avanzar; todas son mejoras incrementales compatibles con la promoción.

---

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Auditoría inicial de la Fase C (categoría 05 arquitectura técnica) de GeoVial. 0 P0, 0 P1, 2 P2, 3 P3. Veredicto: APROBADO CON OBSERVACIONES. |
