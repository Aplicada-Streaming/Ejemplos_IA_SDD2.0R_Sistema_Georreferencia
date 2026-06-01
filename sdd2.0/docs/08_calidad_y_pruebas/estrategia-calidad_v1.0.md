# Estrategia de calidad — GeoVial

**Proyecto:** GeoVial
**Documento:** estrategia-calidad_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Ingeniero QA / SDET Senior (AG-08), Equipo SDD 2.0

## 1. Definición de calidad para GeoVial

GeoVial tiene calidad cuando la recolección georreferenciada de observaciones en terreno es confiable y no pierde datos pese a operar sin conexión, cuando la información sincronizada al backend es funcionalmente correcta y consistente con las reglas de jerarquía, estados y agrupación por radio, y cuando el acceso a los datos personales y a los registros del organismo respeta el rol, el área y la trazabilidad que exige la Ley 25.326. Dicho de otro modo: el sistema delega la captura en personal no experto sin degradar la integridad del dato, y concentra el criterio experto en una revisión central que opera sobre datos íntegros, autorizados y auditados.

El perfil de riesgo está dominado por dos fuentes: la captura sin conexión con sincronización diferida (riesgo de pérdida o duplicación de observaciones, R-02 del BRIEF / RA-02 de 05) y la georreferenciación de calidad variable (R-03 / RA-06). La calidad de GeoVial se mide, antes que por la riqueza de la interfaz, por la fiabilidad del ciclo capturar → encolar → sincronizar → consolidar → auditar.

## 2. Atributos de calidad priorizados (ISO/IEC 25010)

Cada atributo declara prioridad y, cuando corresponde, la métrica numérica con su NFR de origen (arquitectura-solución §8 de 05, derivada de PROJECT-README §13).

| Atributo ISO/IEC 25010 | Prioridad | Métrica / criterio | NFR de origen (05 §8) |
| --- | --- | --- | --- |
| Fiabilidad (madurez, tolerancia a fallos, recuperabilidad) | Alta | Operación sin conexión ≥ 1 jornada (8 h) sin pérdida; cola idempotente sin duplicar ante reintento; reanudación sin pérdida tras corte | NFR operación sin conexión; NFR detección de conectividad |
| Adecuación funcional (completitud, corrección, pertinencia) | Alta | Cada CU crítico cubierto y verde; cada RN verificada por test; georreferenciación automática ≥ 95% de observaciones con coordenada válida | NFR confiabilidad de georreferenciación; CU-01..14, RN-01..08 |
| Seguridad (confidencialidad, integridad, no repudio, responsabilidad) | Alta | Autorización por rol y área en cada acceso; auditoría inmutable con retención ≥ 12 meses; acceso a datos personales acotado y registrado | RN-01, RN-07, RN-08; ADR-14 (Ley 25.326) |
| Eficiencia de desempeño (comportamiento temporal) | Alta | Sincronización ≤ 5 min para ≈100 observaciones con fotos; latencia API de lecturas administrativas p95 ≤ 500 ms | NFR tiempo de sincronización; NFR latencia API |
| Mantenibilidad (modularidad, testabilidad, reusabilidad) | Media | Cobertura por capa según §2.2 de la regla; dominio testeable sin infraestructura (Clean Architecture, ADR-01/ADR-10); librería de sync con API pública estable (SemVer) | ADR-01, ADR-07, ADR-10 |
| Compatibilidad (coexistencia, interoperabilidad) | Media | API REST conforme a OpenAPI 3.x; errores en Problem Details RFC 7807; contrato estable consumido por web y móvil | ADR-02, ADR-11 |
| Usabilidad | Media | Flujos de captura y revisión operables; validados en componentes Blazor (bUnit) y UI móvil; el detalle de criterios UX vive en 03 | — |
| Portabilidad | Baja | Backend en tres contenedores Linux; app móvil Android 8.0+; backend de archivos configurable (local/S3/otro) | ADR-08, ADR-13 |

Justificación de las prioridades altas: fiabilidad y adecuación funcional son altas porque la captura offline y la georreferenciación son el núcleo del valor y el foco de los riesgos R-02 y R-03; seguridad es alta por la sujeción a la Ley 25.326 (datos personales de agentes y registros del organismo, RN-08, ADR-14); eficiencia es alta por los SLA numéricos de sincronización (≤ 5 min) y de latencia de API (p95 ≤ 500 ms).

## 3. Quality gates

Conjunto de criterios mecánicos que el pipeline de CI (PROJECT-README §11, materializado como stages en 09) aplica antes de declarar aceptable un build, una rama o un release. Cada gate especifica condición, herramienta y consecuencia.

| Gate | Condición | Herramienta | Consecuencia si falla |
| --- | --- | --- | --- |
| Build sin warnings | Compila con warnings tratados como error (`TreatWarningsAsErrors`) | Compilador .NET en stage Build de CI (README §11 stage 2) | Falla el build; el PR no avanza |
| Cobertura de líneas | Líneas ≥ 80% global del gate de CI; por capa según §2.2 (ver estrategia-testing §2) | Coverlet + reporte de cobertura en stage Tests (README §11 stage 3) | Falla el stage; el PR no se mergea |
| Cobertura de branches | Branches ≥ 70% | Coverlet + reporte de cobertura en stage Tests | Falla el stage; el PR no se mergea |
| Tests verdes | Suite unitaria + integración 100% verde, sin tests rojos ni omitidos sin justificación | xUnit en stage Tests; WebApplicationFactory + Testcontainers para integración | Falla el stage; el PR no se mergea |
| Contrato OpenAPI | El documento OpenAPI 3.x generado coincide con la API y está versionado en el repo | Generación y validación de OpenAPI en CI (README §6, §11) | Falla el stage; bloquea el merge |
| Conformidad de licencias | Dependencias MIT/Apache/BSD; sin GPL en componentes distribuidos | Verificación de licencias en CI (README §2) | Falla el build; bloquea el release |

Los tres primeros gates de cobertura y build son los gates bloqueantes nombrados explícitamente en PROJECT-README §9 y §11. No se admite bajar los umbrales sin ADR (regla §2.2).

## 4. Roles QA dentro del equipo

Equipo de 4 integrantes (PROJECT-README §1, `equipo_n: 4`): 2 dev backend, 1 dev frontend y 1 QA part-time (plan-iteracion-sprint-00 §1). La función QA/SDET la titulariza el AG-08, apoyado por todo el equipo bajo modelo de calidad compartida.

| Actividad | Diseña | Ejecuta | Aprueba |
| --- | --- | --- | --- |
| Casos de prueba referenciales (TC-XX) | QA (AG-08) | CI (automático) + QA | QA (AG-08) |
| Tests unitarios de dominio/aplicación | Dev autor de la US/BT | CI (automático) | QA revisa cobertura por capa |
| Tests de integración (API + persistencia) | Dev backend + QA | CI (automático) | QA (AG-08) |
| Tests de componente (bUnit) y UI móvil | Dev frontend + QA | CI / banco Android por USB | QA (AG-08) |
| Quality gates del pipeline | QA (AG-08) + DevOps (AG-09) | CI (automático) | AG-09 materializa en 09 |
| Validación de NFR contra SLA | QA (AG-08) + Arquitecto (AG-05) | Pruebas de integración/UI | Arquitecto (AG-05) |
| Aprobación de release | — | — | QA (AG-08) con visto de AG-05; criterios-validación |

QA part-time implica priorizar la automatización: todo TC referencial debe materializarse como test automatizado que corre en CI, evitando la ejecución manual recurrente.

## 5. Cadencia de revisión

- La estrategia de calidad y sus umbrales se revisan al cierre de cada sprint, en la retrospectiva, junto con la actualización de la matriz de cobertura (anti-patrón "matriz desactualizada", regla §4.10).
- Los umbrales de cobertura por capa se revalúan trimestralmente o cuando un ADR justifique modificarlos; cualquier baja de umbral exige ADR (regla §2.2).
- La Definition of Done es la fuente canónica (documento aparte); cualquier cambio en sus criterios se registra en su §3 de control de cambios y se comunica en el sprint review siguiente (regla §3.4).
- La matriz de cobertura se actualiza al cierre de cada sprint para reflejar el estado real de los tests (Pendiente → Verde/Rojo).

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Estrategia de calidad inicial: definición de calidad, atributos ISO/IEC 25010 priorizados con NFR de origen, quality gates del pipeline, roles QA del equipo de 4 y cadencia de revisión. Generada por AG-08 |
