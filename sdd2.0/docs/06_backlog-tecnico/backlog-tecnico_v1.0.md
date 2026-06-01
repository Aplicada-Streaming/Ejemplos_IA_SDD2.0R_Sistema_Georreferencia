# Backlog técnico — GeoVial

**Proyecto:** GeoVial
**Documento:** backlog-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Scrum Master / Agile Coach (AG-06), Equipo SDD 2.0
**Tipo de proyecto:** web-monolith
**Estimación adoptada:** Fibonacci (1, 2, 3, 5, 8, 13, 21)
**Trazabilidad upstream:** 05 (arquitectura-solución, ADR-01 a ADR-14, modelo-datos-lógico, contratos-rest, contratos-abstractions-sync, extensibilidad); 02 (CU-01 a CU-14); 01 (NB-01 a NB-06)
**Trazabilidad downstream:** product-backlog_v1.0.md, 07_plan-sprint, 08_calidad_y_pruebas, 09_devops

Las BT viven inline en este documento: el backlog tiene 22 BT (sección 2), por debajo del umbral de 30 que exige archivos individuales en `tareas-tecnicas/` (§3.3 de las reglas). Cada BT mantiene su fuente upstream, sus dependencias, sus criterios técnicos y su trazabilidad a US.

## 1. Épicas técnicas

### EP-T1 — Fundaciones y arquitectura

- Objetivo: levantar el monorepo, las capas Clean Architecture, el modelo de dominio base y el walking skeleton end-to-end.
- Alcance: scaffolding de proyectos, separación de capas Domain/Application/Infrastructure/Web-API, mediador CQRS ligero, modelo de dominio de usuarios y jerarquía.
- Fuente upstream: ADR-01 (estilo monolito modular Clean Arch + CQRS), ADR-10 (separación de capas), ADR-12 (omisión de 04); arquitectura-solución §2, §3; PROJECT-README §4 fase 1.
- BT contenidas: BT-09, BT-01, BT-02, BT-10.

### EP-T2 — Persistencia y modelo de datos

- Objetivo: materializar la persistencia transaccional del backend y los módulos de dominio ricos.
- Alcance: EF Core + SQL Server, mapeo de las 12 entidades del modelo lógico, módulos de relevamientos/asignación, estados, observaciones y marcadores, georreferenciación.
- Fuente upstream: ADR-09 (SQL Server + EF Core), modelo-datos-lógico; componentes de la vista lógica de arquitectura-solución §3; RN-02, RN-03, RN-05.
- BT contenidas: BT-07, BT-03, BT-04, BT-05, BT-06.

### EP-T3 — Seguridad, autorización y auditoría

- Objetivo: proveer autenticación, autorización transversal por rol y área y auditoría inmutable.
- Alcance: ROPC/JWT con refresh condicionado, módulo transversal de autorización, registro de auditoría inalterable con retención.
- Fuente upstream: ADR-03 (auth), ADR-14 (compliance Ley 25.326); cross-cutting de arquitectura-solución §7; RN-01, RN-06, RN-07, RN-08.
- BT contenidas: BT-08, BT-18 (parcial, errores), BT-12.

### EP-T4 — API REST y contrato

- Objetivo: exponer la API REST versionada consumida por web y móvil, con contrato OpenAPI y errores normalizados.
- Alcance: endpoints REST, OpenAPI 3.x versionado, Problem Details RFC 7807, catálogo de códigos de error.
- Fuente upstream: ADR-02 (backend expone REST), ADR-11 (Problem Details); contratos-rest; PROJECT-README §6.
- BT contenidas: BT-18.

### EP-T5 — Sincronización offline y librería publicada

- Objetivo: construir la app de captura local, la cola y el motor de sincronización publicable.
- Alcance: scaffolding MAUI, SQLite local y cola, motor de sync (subir/bajar/consolidar), monitor de conectividad, consolidación last-write-wins y marcado de conflictos, publicación en GitHub Packages y demo autónoma.
- Fuente upstream: ADR-05 (SQLite + cola), ADR-06 (last-write-wins + override manual), ADR-07 (librería en GitHub Packages); contratos-abstractions-sync; RC-03; RN-04; PROJECT-README §14.
- BT contenidas: BT-13, BT-14, BT-15, BT-16, BT-17, BT-22.

### EP-T6 — Revisión web, archivos e imágenes

- Objetivo: proveer la vista de revisión sobre mapa, el alojamiento configurable de fotos, el pipeline de imágenes y la exportación/importación.
- Alcance: mapa Leaflet + carrusel en web/móvil, librería de alojamiento con backends configurables, compresión/redimensión de fotos, empaquetado ZIP de relevamiento completo.
- Fuente upstream: ADR-04 (OpenStreetMap + Leaflet), ADR-08 (alojamiento con backends configurables); extensibilidad; contratos-rest §4.1; arquitectura-solución §3, §8 (NFR tamaño de foto).
- BT contenidas: BT-11, BT-20, BT-19, BT-21.

## 2. BT por épica

Tipo: feature | spike | refactor | devops | docs. Prioridad alineada al MoSCoW de las US consumidoras y a la secuencia de delivery (PROJECT-README §4). Estimación en SP (Fibonacci).

### EP-T1 — Fundaciones y arquitectura

| BT | Título | Tipo | Prioridad | SP | Fuente upstream | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-09 | Scaffolding del monorepo y capas Clean Architecture con mediador CQRS | devops | Must | 8 | ADR-01, ADR-10, ADR-12; PROJECT-README §4 fase 1, §5 | — | Compila el monorepo con los proyectos de §5; las dependencias entre capas apuntan hacia Domain; el mediador despacha un command de prueba; walking skeleton end-to-end verde en CI |
| BT-01 | Modelo de dominio de usuarios, jerarquía y áreas | feature | Must | 5 | ADR-10; modelo-datos-lógico; CU-03; RC-05 | BT-09 | Las entidades Usuario y Área existen en Domain con el enum de rol (RC-05); el dominio se testea sin infraestructura |
| BT-02 | Validación del nivel jerárquico administrable | feature | Must | 3 | CU-03; RN-01 | BT-01 | Cada nivel solo administra el inmediato inferior; un nivel no autorizado produce `ACCESO_NO_AUTORIZADO` |
| BT-10 | Entidad Relevamiento con radio de agrupación y seed inicial | feature | Must | 3 | modelo-datos-lógico; RN-02; CU-01 | BT-01, BT-07 | El relevamiento persiste su radio; existe seed de usuario raíz y un área para arrancar |

### EP-T2 — Persistencia y modelo de datos

| BT | Título | Tipo | Prioridad | SP | Fuente upstream | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-07 | Persistencia backend con EF Core y SQL Server | feature | Must | 8 | ADR-09; modelo-datos-lógico | BT-09 | Las 12 entidades del modelo lógico mapean a SQL Server; integridad referencial y check de enums declarados; pruebas de integración con Testcontainers verdes |
| BT-03 | Módulo de relevamientos y asignación (CQRS) | feature | Must | 5 | arquitectura-solución §3; CU-01; RN-01 | BT-07, BT-10 | Commands de alta, asignación y reasignación; valida pertenencia de área; audita cada acción |
| BT-04 | Máquina de estados del relevamiento | feature | Must | 5 | CU-10; RN-05 | BT-07 | Solo se permiten transiciones recolección→revisión→cierre y reapertura explícita; cierre deja solo lectura; `TRANSICION_INVALIDA` ante transición no permitida |
| BT-05 | Módulo de observaciones y marcadores (CQRS) | feature | Must | 8 | arquitectura-solución §3; CU-04, CU-09; RC-02, RC-04 | BT-07 | Crea/asocia marcador por radio; gestiona fotos, comentarios y etiquetas con sus cardinalidades; bloqueo de solo lectura tras cierre |
| BT-06 | Módulo de georreferenciación y prioridad de metadatos | feature | Must | 5 | ADR-04; CU-04, CU-05; RN-03 | BT-05 | Deriva coordenada de metadatos como fuente primaria; ubicación manual y bandeja sin georreferenciar; `FUENTE_UBICACION_INCORRECTA` si se ubica manual una foto con metadatos |

### EP-T3 — Seguridad, autorización y auditoría

| BT | Título | Tipo | Prioridad | SP | Fuente upstream | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-08 | Autenticación ROPC/JWT con refresh condicionado y autorización por rol y área | feature | Must | 8 | ADR-03, ADR-14; CU-02, CU-14; RN-01, RN-06, RN-08 | BT-09, BT-18 | Login ROPC emite access + refresh; refresh móvil condicionado al método de seguridad; filtro transversal autoriza por rol y área antes de cada handler; accesos fuera de alcance bloqueados y registrados |
| BT-12 | Auditoría inmutable con retención ≥ 12 meses | feature | Must | 5 | ADR-14; CU-13; RN-07, RN-08 | BT-07 | Cada acceso y acción administrativa se asienta con autor, momento y operación; el registro es inalterable (`AUDITORIA_INMUTABLE`); una acción no auditable se rechaza (`ACCION_NO_AUDITADA`) |

### EP-T4 — API REST y contrato

| BT | Título | Tipo | Prioridad | SP | Fuente upstream | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-18 | API REST versionada con OpenAPI y Problem Details | feature | Must | 8 | ADR-02, ADR-11; contratos-rest; PROJECT-README §6 | BT-09 | Endpoints bajo `/api/v1/`; OpenAPI 3.x generado y validado en CI; errores en `application/problem+json` con `code` del catálogo; breaking change no declarado falla el build |

### EP-T5 — Sincronización offline y librería publicada

| BT | Título | Tipo | Prioridad | SP | Fuente upstream | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-13 | Scaffolding de la app móvil MAUI con método de seguridad | feature | Must | 8 | PROJECT-README §1, §4 fase 3; ADR-03; CU-02; RN-06 | BT-09 | La app MAUI compila e integra Blazor + MudBlazor; el método de seguridad del teléfono habilita el modo offline; relogueo en terreno sin red |
| BT-14 | Persistencia local SQLite y esquema de cola de cambios | feature | Must | 5 | ADR-05; CU-06; RC-03 | BT-13 | SQLite local durable; la cola registra cada cambio con tipo, entidad, marca temporal e identificador único; conserva una jornada completa sin red |
| BT-15 | Motor de sincronización (librería) — superficie pública | feature | Must | 13 | ADR-05, ADR-07; contratos-abstractions-sync; CU-06, CU-07 | BT-14, BT-18 | `IChangeQueue` e `ISyncEngine` expuestos en Abstractions; sube cambios en orden, baja actualizaciones; idempotencia por ChangeId; reanuda sin duplicar |
| BT-16 | Cliente REST de sync y monitor de conectividad | feature | Must | 5 | contratos-abstractions-sync; ADR-02; CU-07 | BT-15 | `ISyncBackendClient` contra `/api/v1/.../sync`; `IConnectivityMonitor` dispara la sincronización automática al recuperar señal |
| BT-17 | Consolidación last-write-wins y marcado de conflictos | feature | Should | 8 | ADR-06; CU-07, CU-11, CU-12; RN-02, RN-04 | BT-15, BT-07 | Ante choque de campo prevalece la última escritura y marca conflicto; detecta marcadores en un mismo radio sin unificar; alimenta la lista de conflictos |
| BT-22 | Publicación de la librería en GitHub Packages y demo autónoma | devops | Should | 5 | ADR-07; PROJECT-README §10, §14; samples §5.X | BT-15 | Paquete publicado con canales preview/stable; cualquier breaking change bumpea MAJOR; demo MAUI autónoma en `samples/02-sync-maui-demo` |

### EP-T6 — Revisión web, archivos e imágenes

| BT | Título | Tipo | Prioridad | SP | Fuente upstream | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-11 | Vista de revisión sobre mapa con Leaflet y carrusel | feature | Must | 8 | ADR-04; CU-08, CU-09 | BT-05, BT-18 | Marcadores ubicados sobre mapa OSM/Leaflet en web; carrusel y navegación entre marcadores; filtrado por etiquetas; mismo recurso de mapa en web y móvil |
| BT-20 | Librería de alojamiento de archivos con backends configurables | feature | Must | 8 | ADR-08; extensibilidad; CU-04, CU-08, CU-09; PROJECT-README §7 | BT-09 | Abstracción de backend con Guardar/Recuperar/Eliminar/Existe; implementaciones local y S3; la base persiste solo la referencia; el usuario raíz configura el backend |
| BT-19 | Pipeline de imágenes (compresión y redimensión) | feature | Must | 5 | arquitectura-solución §8 (NFR tamaño de foto); ADR-05; CU-04 | BT-20 | Cada foto se comprime/redimensiona para acotar el payload de sincronización; la fuente de coordenada (metadatos) se preserva |
| BT-21 | Exportación e importación de relevamiento completo en ZIP | feature | Must | 8 | contratos-rest §4.1; CU-08; RN-01, RN-07, RN-08 | BT-07, BT-20, BT-18 | `export` produce un ZIP con manifiesto y binarios; `import` reconstruye validando área y coherencia; `ARCHIVO_EXPORTACION_INVALIDO` ante archivo incoherente; toda operación auditada |

## 3. Trazabilidad BT ↔ US ↔ CU

| BT | US consumidoras | CU upstream | Fuente upstream (ADR / componente / contrato) |
| --- | --- | --- | --- |
| BT-01 | US-01, US-02, US-03 | CU-03 | ADR-10; modelo-datos-lógico; componente Usuarios y jerarquía |
| BT-02 | US-01 | CU-03 | CU-03; RN-01 |
| BT-03 | US-06, US-07, US-08 | CU-01 | Componente Relevamientos y asignación (CQRS); ADR-01 |
| BT-04 | US-09, US-10, US-20 | CU-10 | Componente Estados del relevamiento; RN-05 |
| BT-05 | US-11, US-12, US-13, US-14, US-15 | CU-04, CU-09 | Componente Observaciones y marcadores (CQRS); RC-02, RC-04 |
| BT-06 | US-11, US-13, US-14 | CU-04, CU-05 | Componente Georreferenciación; ADR-04; RN-03 |
| BT-07 | US-06, US-07, US-08, US-09, US-10, US-11, US-12, US-13, US-14, US-15, US-18, US-21, US-22, US-25, US-26, US-27, US-28 | CU-01, CU-04, CU-08, CU-09, CU-10 | ADR-09; modelo-datos-lógico (persistencia compartida) |
| BT-08 | US-04, US-05, US-31 | CU-02, CU-14 | ADR-03, ADR-14; componente Acceso y Autorización |
| BT-09 | US-01..US-31 (infraestructura compartida) | transversal a CU-01..CU-14 | ADR-01, ADR-10, ADR-12 (scaffolding y capas, infraestructura compartida) |
| BT-10 | US-06 | CU-01 | modelo-datos-lógico; RN-02 |
| BT-11 | US-21, US-22, US-23, US-24 | CU-08, CU-09 | ADR-04; componente Revisión sobre mapa |
| BT-12 | US-01, US-29, US-30 | CU-13 | ADR-14; componente Auditoría y retención |
| BT-13 | US-04, US-05, US-16, US-19 | CU-02, CU-06, CU-07 | PROJECT-README §1; ADR-03; componente Captura local |
| BT-14 | US-16, US-17 | CU-06 | ADR-05; RC-03; componente Cola de sync |
| BT-15 | US-17, US-18, US-19, US-32 | CU-06, CU-07 | ADR-05, ADR-07; contratos-abstractions-sync |
| BT-16 | US-18, US-19, US-32 | CU-07 | contratos-abstractions-sync; ADR-02 |
| BT-17 | US-18, US-25, US-26, US-32 | CU-07, CU-11, CU-12 | ADR-06; RN-02, RN-04 |
| BT-18 | US-01, US-04, US-06..US-10, US-18, US-26, US-27..US-31 (infraestructura compartida) | transversal a CU-01..CU-14 | ADR-02, ADR-11; contratos-rest (API compartida) |
| BT-19 | US-11, US-14, US-15, US-16 | CU-04 | arquitectura-solución §8 (NFR); ADR-05 |
| BT-20 | US-15, US-22, US-24 | CU-04, CU-08, CU-09 | ADR-08; extensibilidad |
| BT-21 | US-27, US-28 | CU-08 | contratos-rest §4.1; componente Exportación/importación |
| BT-22 | US-32 | CU-06, CU-07 | ADR-07; PROJECT-README §14 |

Notas de cobertura:

- Toda BT declara fuente upstream (ADR, componente o contrato de 05) y al menos una US consumidora.
- BT-09 (scaffolding y capas) y BT-18 (API REST compartida) se justifican como infraestructura compartida con ADR explícita (ADR-01/ADR-10 y ADR-02/ADR-11 respectivamente), conforme a §4.8: soportan transversalmente al backlog completo. Para no quedar como BT sin US consumidora, además se listan US representativas que las consumen de forma directa.
- BT-17 y BT-22 son Should porque sus US consumidoras principales (resolución de conflictos NB-05, demo de la librería) son Should; el resto de las BT son Must por sostener US Must del MVP.

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Backlog técnico inicial: 6 épicas técnicas, 22 BT inline (umbral < 30) derivadas de los componentes, ADR y contratos de 05, con matriz BT↔US↔CU completa. Generado por AG-06 |
