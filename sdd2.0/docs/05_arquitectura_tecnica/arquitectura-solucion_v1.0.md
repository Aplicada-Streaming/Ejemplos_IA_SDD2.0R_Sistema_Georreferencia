# Arquitectura de solución — GeoVial

**Proyecto:** GeoVial
**Documento:** arquitectura-solucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Trazabilidad upstream:** 00 (visión, alcance, compatibilidad, restricciones); 01 (NB-01 a NB-06); 02 (CU-01 a CU-14, RN-01 a RN-08, modelo-conceptual, RC-01 a RC-07); PROJECT-BRIEF §1 a §12; PROJECT-README §1 a §16
**Trazabilidad downstream:** 06_backlog-tecnico, 07_plan-sprint, 08_calidad_y_pruebas, 09_devops, 10_developer_guide, 11_examples

## 1. Objetivo

Este documento es el artefacto maestro de arquitectura técnica de GeoVial. Describe el estilo arquitectónico adoptado, las cuatro vistas mínimas (lógica, procesos, despliegue y datos), las decisiones transversales (cross-cutting), los atributos de calidad con métricas numéricas, los riesgos y la trazabilidad de CU/RN/NFR a componentes y ADR. Está dirigido a los equipos de desarrollo (AG-06, AG-07), de calidad (AG-08) y de DevOps (AG-09) que materializan el diseño en sprints, pruebas e infraestructura.

GeoVial es un sistema del organismo de control de infraestructura vial que delega la recolección georreferenciada de observaciones (comentarios y fotos) sobre puentes y caminos en agentes de campo, con operación sin conexión y revisión centralizada sobre mapa. El tipo dominante D8 es `web-monolith`, con tres sub-proyectos declarados en PROJECT-README §1: la app móvil de captura (`mobile-app-maui`), la librería de sincronización (`library`) y la librería de alojamiento de archivos (`library`).

## 2. Estilo arquitectónico

**Estilo adoptado: monolito modular con Clean Architecture y CQRS ligero.**

El backend se estructura en capas Domain, Application, Infrastructure y Web/API, con módulos por contexto cohesivo: usuarios y jerarquía, relevamientos, observaciones y marcadores, sincronización y alojamiento de archivos. La dependencia entre capas es unidireccional hacia adentro: Web/API depende de Application; Application depende de Domain; Infrastructure implementa los puertos definidos por Application y Domain; Domain no depende de nada externo. Se adopta CQRS ligero —separación de commands y queries vía un mediador in-process— únicamente en el módulo de relevamientos/observaciones/marcadores, que es el de dominio más rico y con mayor asimetría entre escritura (captura, consolidación, transiciones de estado) y lectura (revisión sobre mapa, carrusel). El resto de los módulos usa servicios de aplicación directos.

La decisión se materializa en ADR-01 (estilo) y ADR-10 (separación de capas). La elección se evalúa contra dos alternativas descartadas:

| Criterio | Capas + Clean Arch (elegido) | Microservicios | Monolito en capas acoplado al ORM |
| --- | --- | --- | --- |
| Tamaño del equipo | 4 devs (encaja en 1-10) | Requiere 10+ | 4 devs |
| Dominios de negocio | 1 organismo, 1 bounded context | 5+ contextos autónomos | 1 |
| Deploy independiente | No requerido (tres contenedores fijos) | Requerido | No |
| Complejidad operativa | Baja | Alta (gateway, mesh, sagas) | Baja |
| Time to market (MVP fase 5) | Rápido | Lento | Rápido |
| Testeo del dominio sin infraestructura | Sí (Domain puro) | Sí pero con costo operativo | No (lógica acoplada al ORM) |

- **Microservicios:** descartado por sobredimensionamiento. El problema es un único bounded context con un equipo de ≈4 personas (PROJECT-README §3); el cliente pide explícitamente backend monolítico e infraestructura de tres contenedores (PROJECT-README §11, §16). La complejidad operativa de gateway, service mesh y sagas no se justifica.
- **Monolito en capas tradicional acoplado al ORM:** descartado porque impide testear el dominio sin infraestructura, requisito de la estrategia de testing desde la fase 2 (PROJECT-README §9), e incumple la cobertura mínima de dominio.

## 3. Vista lógica

Componentes (módulos con responsabilidad cohesiva, no clases). Las dependencias apuntan hacia adentro (Domain es el centro).

| Componente | Capa | Responsabilidad | Entradas | Salidas | Dependencias | CU cubiertos |
| --- | --- | --- | --- | --- | --- | --- |
| Módulo de usuarios y jerarquía | Application/Domain | Alta y baja jerárquica de usuarios, asociación a área, intervención raíz | Comandos de administración | Entidades Usuario/Área | Domain | CU-03 |
| Módulo de acceso y selección | Application/Web-API | Inicio de sesión ROPC, relogueo en terreno, selección de relevamiento asignado | Credenciales, token, método de seguridad | Token JWT, relevamientos asignados | Domain, Autorización | CU-02 |
| Módulo transversal de autorización | Application (cross-cutting) | Autoriza cada acceso por rol y área antes de ejecutar la operación | Identidad (claims `sub`, `role`), recurso | Permitir o `ACCESO_NO_AUTORIZADO` | Domain | CU-14 (transversal a CU-01..13) |
| Módulo de relevamientos y asignación | Application (CQRS) | Alta de relevamiento, asignación y reasignación de agentes, parámetro de radio | Commands de relevamiento | Relevamiento, AsignaciónAgente | Domain | CU-01 |
| Módulo de estados del relevamiento | Application/Domain | Transiciones de estado, cierre con solo lectura, reapertura explícita | Command de transición | Estado validado | Domain | CU-10 |
| Módulo de observaciones y marcadores | Application (CQRS)/Domain | Captura, asignación de coordenada, creación/asociación de marcador por radio | Foto, coordenada, comentarios, etiquetas | Observación, Marcador, Foto | Domain, Georreferenciación, FileHosting | CU-04, CU-09 |
| Módulo de georreferenciación | Application/Domain | Deriva coordenada priorizando metadatos de la foto, ubicación manual y bandeja sin georreferenciar | Foto con/sin metadatos, ubicación manual | Coordenada o marca sin georreferenciar | Domain | CU-04, CU-05 |
| Módulo de revisión sobre mapa | Application (Query)/Web | Vista de marcadores sobre mapa, agrupación de observaciones, carrusel, filtrado por etiquetas | Consulta de relevamiento | Vista consolidada | Domain, Autorización | CU-08, CU-09 |
| Módulo de exportación/importación | Application/Infrastructure | Produce y consume el archivo comprimido único de relevamiento completo | Relevamiento / archivo ZIP | Archivo ZIP / relevamiento reconstruido | Domain, FileHosting, Auditoría | CU-08 |
| Módulo de captura local (móvil) | Mobile/Application | Captura sin conexión, persistencia local, encolado de cambios, bandeja local sin georreferenciar | Foto, comentarios, etiquetas offline | RegistroCambioSync local | SQLite local, GeoVial.Sync | CU-06 |
| Módulo de sincronización | GeoVial.Sync (library) | Sube cambios locales, baja actualizaciones, consolida last-write-wins, marca conflictos | Cola local, estado central | Estado consolidado, marcas de conflicto | Web-API (REST), Domain | CU-06, CU-07 |
| Módulo de detección de conflictos | Application/Domain | Detecta marcadores en un mismo radio y ediciones en conflicto, lista pendientes | Marcadores, ediciones consolidadas | Lista de ConflictoSync | Domain | CU-11 |
| Módulo de resolución de conflictos | Application/Web | Unifica o separa marcadores, dirime ediciones, levanta la marca de conflicto | Decisión humana | Base consolidada, conflicto resuelto | Domain, Autorización, Auditoría | CU-12 |
| Módulo de auditoría y retención | Application/Infrastructure (cross-cutting) | Asienta accesos y acciones administrativas de forma inalterable, retención ≥ 12 meses | Evento auditable | RegistroAuditoría inmutable | Domain | CU-13 (transversal) |
| Librería de alojamiento de archivos | GeoVial.FileHosting (library) | Almacena y recupera fotos con backends configurables (local/S3/otro) | Stream de foto, configuración de backend | Referencia de archivo | Abstracción de backend | CU-04, CU-08, CU-09 |

Todos los componentes consumen el modelo de dominio del paquete `GeoVial.Domain` y los DTOs de `GeoVial.Shared`. La cobertura CU↔componente completa se detalla en §10.

## 4. Vista de procesos

Concurrencia, transacciones y manejo de estado en memoria.

- **Front web (Blazor Interactive Server).** Render del lado del servidor con circuito SignalR por sesión; el estado de interactividad del front administrativo vive en el servidor y requiere conexión persistente (trade-off PROJECT-README §16). Cada circuito atiende a un jefe de área o administrador; no comparte estado mutable entre usuarios.
- **API REST (commands/queries).** Cada request HTTP se atiende en su propio scope de DI. Los commands del módulo de relevamientos/observaciones se ejecutan dentro de una transacción de base de datos (unit of work por request) que garantiza atomicidad de la captura, la asociación de marcador y la consolidación. Las queries de revisión sobre mapa no abren transacción de escritura.
- **CQRS ligero.** El mediador in-process despacha cada command a su handler único; no hay cola ni bus externo. Queries y commands comparten el mismo proceso y la misma base; la separación es de modelo de lectura/escritura, no de despliegue.
- **Sincronización offline-first (motor de sync).** Es el punto de orquestación no trivial; el pipeline completo se detalla en `flujo-ejecucion_v1.0.md`. Resumen: al recuperar conexión, la app móvil (1) sube los RegistroCambioSync encolados en orden con su marca temporal, (2) el backend consolida cada cambio contra el estado central aplicando last-write-wins a nivel de campo y marcando conflicto, (3) detecta marcadores dentro de un mismo radio y los señala como conflicto sin unificarlos, (4) baja las actualizaciones de los relevamientos asignados, (5) vacía de la cola lo confirmado. La idempotencia se garantiza por el identificador único de cada cambio (RC-03): un reintento no aplica el mismo cambio dos veces.
- **Transacciones y consistencia.** Consistencia inmediata dentro del backend (SQL Server, transacción por request). Entre móvil y backend la consistencia es eventual: el estado local converge al central tras la sincronización. La resolución de conflictos (CU-12) es una acción humana posterior, transaccional sobre el backend.
- **Estado en memoria.** No hay cache distribuido en v1. El estado de sesión del circuito Blazor es server-side y efímero. El access token JWT es de vida corta (≈60 min, PROJECT-README §8) y el refresh en móvil se condiciona al método de seguridad del teléfono (RN-06, ADR-03).

## 5. Vista de despliegue

Tres unidades de despliegue contenerizadas (PROJECT-README §11, §16; compatibilidad-plataformas §2) más la app móvil instalada en dispositivo.

| Unidad de despliegue | Runtime objetivo | Contenido | Dependencias de infraestructura |
| --- | --- | --- | --- |
| Contenedor front-end | Contenedor Linux | `GeoVial.Web` (Blazor Interactive Server + MudBlazor) | Conexión persistente al backend; recursos de mapa (tiles OSM/Leaflet) servidos al navegador |
| Contenedor backend | Contenedor Linux | `GeoVial.Api` + `GeoVial.Application` + `GeoVial.Infrastructure` + `GeoVial.FileHosting`; almacenamiento local de archivos sobre este contenedor | Acceso a la base de datos; volumen para almacenamiento local de fotos (salvo backend externo S3) |
| Contenedor base de datos | Contenedor Linux | Motor SQL Server | Volumen persistente de datos |
| App móvil de captura | Android 8.0+ (API 26) en dispositivo | `GeoVial.Mobile` (.NET MAUI + Blazor + MudBlazor) + SQLite local + `GeoVial.Sync` | Acceso a la API REST del backend cuando hay conexión; almacenamiento local del dispositivo |

Notas de despliegue:

- El almacenamiento local de fotos reside sobre el contenedor del backend; queda acoplado al ciclo de vida del contenedor salvo que se configure un backend externo (AWS S3 u otro) mediante `GeoVial.FileHosting` (ADR-08, `extensibilidad_v1.0.md`).
- iOS, tablets y distribución por tiendas quedan fuera de v1 (ADR-13, compatibilidad-plataformas §4).
- El front web no soporta navegadores no evergreen; se exigen las últimas 2 versiones de Chrome, Edge, Firefox y Safari (ADR-13).
- El detalle de imágenes, pipeline y matriz de CI se delega a 09_devops; aquí solo se declara la topología objetivo.

## 6. Vista de datos

| Aspecto | Decisión |
| --- | --- |
| Persistencia backend | SQL Server accedido vía EF Core (ADR-09). Base central transaccional; integridad referencial declarativa y restricciones de check para los enums de dominio. |
| Persistencia local móvil | SQLite (sqlite-net-pcl) (ADR-05). Habilita la operación sin conexión y la cola de cambios pendientes (RegistroCambioSync). |
| Almacenamiento de fotos | Librería `GeoVial.FileHosting` con backends configurables: local sobre el contenedor del backend, AWS S3 u otro (ADR-08). La base guarda la referencia, no el binario. |
| Caches | Sin cache distribuido en v1 (observabilidad y carga no críticas, PROJECT-README §1 flags). |
| Particionamiento / sharding | No aplica. multi_tenant=false: un único organismo. Las "áreas" son ámbito de autorización (RN-01), no tenant; no hay columna discriminadora de tenant ni esquema por tenant. |
| Modelo lógico | Detallado en `modelo-datos-logico_v1.0.md`, derivado entidad por entidad del modelo-conceptual de 02 (12 entidades) con tipos físicos para SQL Server (backend) y esquema SQLite (cola local). |

## 7. Cross-cutting concerns

Decisiones transversales centralizadas (anti-patrón a evitar: dispersarlas por componente).

- **Logging.** Logging estructurado con identificador de correlación por request HTTP y por circuito Blazor. Nivel configurable por entorno. Observabilidad estándar, no crítica en v1 (PROJECT-README §1 `tiene_observabilidad_critica: false`); no se incorpora tracing distribuido ni métricas avanzadas en v1.
- **Manejo de errores.** Respuestas de error de la API REST con Problem Details RFC 7807 (`application/problem+json`) (ADR-11). Cada error de dominio expone un código estable de los catálogos de las RN y los CU (por ejemplo `ACCESO_NO_AUTORIZADO`, `RELEVAMIENTO_SOLO_LECTURA`, `MARCADORES_EN_RADIO`, `CONSOLIDACION_INVALIDA`, `OBSERVACION_SIN_GEORREFERENCIA`, `ACCION_NO_AUDITADA`). El contrato de errores se formaliza en `contratos-rest_v1.0.md`.
- **Configuración y secretos.** Configuración por entorno; `.env` fuera de git con `.env.example` versionado en desarrollo; secretos de CI en el gestor de secretos de la plataforma con rotación a 90 días; en producción, secret store gestionado del entorno de deploy (PROJECT-README §8). El usuario raíz configura el backend de alojamiento de archivos (ADR-08).
- **Autorización (transversal).** El módulo de autorización (CU-14) evalúa rol y área antes de ejecutar cualquier operación de escritura o lectura sobre recursos del dominio (RN-01, RN-08). Es un filtro previo a los handlers de command/query, no lógica repetida por componente.
- **Auditoría y compliance Ley 25.326.** El módulo de auditoría (CU-13) asienta accesos y acciones administrativas de forma inalterable con retención ≥ 12 meses (RN-07), y el acceso a datos personales se acota por rol y área y queda registrado (RN-08). Gobernado por ADR-14.

## 8. Quality attributes (NFR)

Valores numéricos de PROJECT-README §13 y vision/alcance de 00. Cada NFR declara objetivo, mecanismo de medición y ADR asociada.

| NFR | Objetivo numérico | Mecanismo de medición | ADR relacionada |
| --- | --- | --- | --- |
| Operación sin conexión | ≥ 1 jornada laboral completa (8 h) sin conexión | Prueba de campo simulada: captura continua durante 8 h sobre SQLite local sin red; verificación de que la cola conserva todos los cambios | ADR-05 |
| Tiempo de sincronización | ≤ 5 minutos para ≈100 observaciones con fotos | Telemetría de sincronización de la app: tiempo entre disparo y vaciado de cola para un dataset de 100 cambios (UI/integration test de 08) | ADR-05, ADR-06 |
| Latencia API (lecturas administrativas) | p95 ≤ 500 ms | Medición p95 sobre endpoints de revisión en prueba de integración de carga (WebApplicationFactory + dataset sintético) | ADR-02, ADR-09 |
| Detección de conectividad | Automática; sync se dispara sola al recuperar señal | Prueba de UI móvil que corta y restablece red y verifica disparo automático del pipeline de sync | ADR-05, ADR-06 |
| Disponibilidad del backend | SLO 99% en horario laboral | Conteo de disponibilidad sobre health check del contenedor backend en horario laboral; error budget mensual | ADR-13 |
| Confiabilidad de georreferenciación | ≥ 95% de observaciones con coordenada válida automática | Métrica sobre metadatos de las observaciones capturadas (fuente de coordenada = metadatos) / total | ADR-04 |
| Tamaño de foto sincronizada | Compresión/redimensión para acotar el payload (límites a validar) | Medición del payload por foto antes/después de compresión en el pipeline de captura | ADR-05 |

El límite numérico exacto de tamaño de foto queda como supuesto a validar en PROJECT-README §13; el mecanismo (compresión/redimensión) sí está fijado.

## 9. Riesgos arquitectónicos

| ID | Riesgo | Impacto | Probabilidad | Mitigación |
| --- | --- | --- | --- | --- |
| RA-01 | Marcadores duplicados dentro de un mismo radio que generan datos inconsistentes (R-01 de negocio) | Alto | Media | Detección por radio sin unificación automática (RN-02, CU-11), resolución manual desde la web (CU-12), radio configurable por relevamiento. ADR-06 |
| RA-02 | Pérdida de observaciones capturadas sin conexión antes de sincronizar (R-02) | Alto | Media | Persistencia local SQLite durable + cola idempotente (RC-03); sincronización automática al recuperar señal; recordatorio de sync por jornada. ADR-05 |
| RA-03 | Last-write-wins descarta cambios concurrentes válidos | Medio | Media | Marca de conflicto persistente y resolución humana desde la web (RN-04, CU-12); ningún descarte silencioso. ADR-06 |
| RA-04 | Acoplamiento del almacenamiento local de fotos al ciclo de vida del contenedor backend | Medio | Media | Abstracción de backend en `GeoVial.FileHosting`; el usuario raíz puede configurar S3 u otro backend externo. ADR-08, extensibilidad |
| RA-05 | Blazor Interactive Server requiere conexión persistente; caída de red degrada el front administrativo | Medio | Baja | Render server-side aceptado para uso interno; reconexión de circuito; la captura crítica de campo ocurre en la app móvil offline, no en el front web. ADR-01 |
| RA-06 | Georreferenciación de baja calidad cuando la foto no trae metadatos (R-03) | Medio | Media | Prioridad de metadatos, ubicación manual del punto y bandeja sin georreferenciar (RN-03, CU-05). ADR-04 |
| RA-07 | Breaking change no controlado en la API pública de la librería de sincronización publicada | Medio | Baja | SemVer estricto; cualquier breaking change bumpea MAJOR; superficie estable en Abstractions. ADR-07, contratos-abstractions-sync |
| RA-08 | Incumplimiento de la Ley 25.326 por acceso o retención indebidos de datos personales | Alto | Baja | Autorización por rol y área, auditoría inmutable ≥ 12 meses, minimización y finalidad acotada. ADR-14, RN-07, RN-08 |

## 10. Trazabilidad

### 10.1 CU → componente → RN → ADR → tests

| CU | Componente(s) | RN aplicables | ADR que lo gobiernan | Tests previstos (08) |
| --- | --- | --- | --- | --- |
| CU-01 | Relevamientos y asignación | RN-01, RN-02, RN-05, RN-07 | ADR-01, ADR-09, ADR-14 | Suite de creación y asignación |
| CU-02 | Acceso y selección | RN-01, RN-06, RN-07 | ADR-03 | Suite de acceso, relogueo y habilitación offline |
| CU-03 | Usuarios y jerarquía | RN-01, RN-07, RN-08 | ADR-09, ADR-14 | Suite de alta/baja jerárquica |
| CU-04 | Observaciones y marcadores; Georreferenciación | RN-01, RN-02, RN-03, RN-05 | ADR-04, ADR-09 | Suite de captura georreferenciada |
| CU-05 | Georreferenciación; bandeja sin georreferenciar | RN-02, RN-03, RN-05 | ADR-04 | Suite de ubicación manual |
| CU-06 | Captura local (móvil); cola de sync | RN-03, RN-05, RN-06 | ADR-05 | Suite de operación sin conexión |
| CU-07 | Sincronización (GeoVial.Sync) | RN-01, RN-02, RN-04 | ADR-05, ADR-06, ADR-07 | Suite de sincronización e idempotencia |
| CU-08 | Revisión sobre mapa; exportación/importación | RN-01, RN-05, RN-07, RN-08 | ADR-04, ADR-02, ADR-14 | Suite de revisión y exportación/importación |
| CU-09 | Marcadores y carrusel | RN-01, RN-05 | ADR-04, ADR-08 | Suite de gestión de marcador y carrusel |
| CU-10 | Estados del relevamiento | RN-01, RN-05, RN-07 | ADR-09 | Suite de transición de estados |
| CU-11 | Detección de conflictos | RN-01, RN-02, RN-04 | ADR-06 | Suite de detección por radio |
| CU-12 | Resolución de conflictos | RN-01, RN-02, RN-04, RN-05, RN-07 | ADR-06, ADR-14 | Suite de resolución de conflictos |
| CU-13 | Auditoría y retención | RN-01, RN-07, RN-08 | ADR-14 | Suite de auditoría e inmutabilidad |
| CU-14 | Autorización (transversal) | RN-01, RN-08 | ADR-03, ADR-14 | Suite de autorización por rol y área |

### 10.2 RN → componente/ADR

| RN | Reflejada en | ADR |
| --- | --- | --- |
| RN-01 | Módulo transversal de autorización | ADR-03, ADR-14 |
| RN-02 | Módulo de georreferenciación; detección de conflictos | ADR-04, ADR-06 |
| RN-03 | Módulo de georreferenciación | ADR-04 |
| RN-04 | Módulo de sincronización; resolución de conflictos | ADR-06 |
| RN-05 | Módulo de estados del relevamiento | ADR-09 |
| RN-06 | Módulo de acceso y selección | ADR-03, ADR-05 |
| RN-07 | Módulo de auditoría y retención | ADR-14 |
| RN-08 | Autorización + auditoría | ADR-14 |

### 10.3 NFR ↔ arquitectura ↔ ADR

Tabla cruzada en §8. Cada NFR liga a un mecanismo de medición y a una ADR; los componentes que lo realizan están en la vista lógica (§3) y de procesos (§4).

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Arquitectura de solución inicial de GeoVial: estilo monolito modular Clean Arch + CQRS ligero, cuatro vistas, cross-cutting, NFR con métricas numéricas, riesgos y trazabilidad a 14 CU, 8 RN y 14 ADR. Generada por AG-05 |
