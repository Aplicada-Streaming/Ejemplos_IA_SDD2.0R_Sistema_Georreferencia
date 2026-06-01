# Modelo de datos lógico — GeoVial

**Proyecto:** GeoVial
**Documento:** modelo-datos-logico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Trazabilidad upstream:** 02 modelo-conceptual_v1.0.md (12 entidades), RC-01 a RC-07, RN-01 a RN-08; PROJECT-README §7; ADR-09, ADR-05, ADR-14

## 0. Objetivo y convenciones

Modelo lógico derivado entidad por entidad del modelo conceptual de 02, con tipos físicos. El esquema principal es de EF Core sobre SQL Server en el backend (ADR-09); §1.13 documenta aparte el esquema SQLite de la cola local del móvil (ADR-05). Convenciones: claves primarias `uniqueidentifier` (GUID) salvo indicación; marcas temporales `datetime2(3)` en UTC; identificadores de texto `nvarchar`. multi_tenant=false: ninguna tabla lleva columna discriminadora de tenant (ver §6).

## 1. Tablas (esquema backend SQL Server)

### 1.1 Usuario — (conceptual: Usuario)

Persona que opera el sistema en un nivel de la jerarquía.

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| UsuarioId | uniqueidentifier | NOT NULL | newsequentialid() |
| Nombre | nvarchar(200) | NOT NULL | — |
| RolJerarquico | tinyint | NOT NULL | — (1=raíz, 2=jefe general, 3=jefe de área, 4=agente) |
| AreaId | uniqueidentifier | NULL | — (requerido para jefe de área y agente, RC-05) |
| EstadoVigencia | bit | NOT NULL | 1 |
| MetodoSeguridadConfigurado | bit | NOT NULL | 0 |

### 1.2 Area — (conceptual: Área)

Ámbito administrativo de autorización. No es tenant.

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| AreaId | uniqueidentifier | NOT NULL | newsequentialid() |
| Nombre | nvarchar(200) | NOT NULL | — |
| JefeAreaUsuarioId | uniqueidentifier | NULL | — (FK a Usuario con rol jefe de área) |

### 1.3 Relevamiento — (conceptual: Relevamiento)

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| RelevamientoId | uniqueidentifier | NOT NULL | newsequentialid() |
| IdentificacionObra | nvarchar(300) | NOT NULL | — |
| Estado | tinyint | NOT NULL | 1 (1=recolección, 2=revisión, 3=cerrado, RC-06) |
| RadioAgrupacionMetros | decimal(9,2) | NOT NULL | — (positivo, RN-02) |
| AreaId | uniqueidentifier | NOT NULL | — |

### 1.4 AsignacionAgente — (conceptual: AsignaciónAgente)

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| AsignacionAgenteId | uniqueidentifier | NOT NULL | newsequentialid() |
| RelevamientoId | uniqueidentifier | NOT NULL | — |
| AgenteUsuarioId | uniqueidentifier | NOT NULL | — |
| Vigente | bit | NOT NULL | 1 |

### 1.5 Observacion — (conceptual: Observación)

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| ObservacionId | uniqueidentifier | NOT NULL | newsequentialid() |
| RelevamientoId | uniqueidentifier | NOT NULL | — |
| MarcadorId | uniqueidentifier | NULL | — (NULL = en bandeja sin georreferenciar, RC-02) |
| AgenteUsuarioId | uniqueidentifier | NOT NULL | — |
| MomentoCaptura | datetime2(3) | NOT NULL | — |
| SinGeorreferenciar | bit | NOT NULL | 0 |

### 1.6 Marcador — (conceptual: Marcador)

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| MarcadorId | uniqueidentifier | NOT NULL | newsequentialid() |
| RelevamientoId | uniqueidentifier | NOT NULL | — |
| Latitud | decimal(9,6) | NOT NULL | — |
| Longitud | decimal(9,6) | NOT NULL | — |
| EnConflicto | bit | NOT NULL | 0 (levantado solo por resolución humana, RN-02/RN-04) |

### 1.7 Foto — (conceptual: Foto)

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| FotoId | uniqueidentifier | NOT NULL | newsequentialid() |
| MarcadorId | uniqueidentifier | NOT NULL | — |
| ObservacionId | uniqueidentifier | NOT NULL | — |
| TieneMetadatosUbicacion | bit | NOT NULL | 0 |
| FuenteCoordenada | tinyint | NOT NULL | — (1=metadatos, 2=manual, RN-03) |
| ReferenciaArchivo | nvarchar(1024) | NOT NULL | — (referencia en FileHosting, no el binario, ADR-08) |

### 1.8 Comentario — (conceptual: Comentario)

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| ComentarioId | uniqueidentifier | NOT NULL | newsequentialid() |
| MarcadorId | uniqueidentifier | NOT NULL | — |
| FotoId | uniqueidentifier | NULL | — (0..1 foto, RC-04) |
| AutorUsuarioId | uniqueidentifier | NOT NULL | — |
| Texto | nvarchar(2000) | NOT NULL | — |
| Momento | datetime2(3) | NOT NULL | — |

### 1.9 Etiqueta — (conceptual: Etiqueta)

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| EtiquetaId | uniqueidentifier | NOT NULL | newsequentialid() |
| Nombre | nvarchar(100) | NOT NULL | — |

### 1.10 FotoEtiqueta / ComentarioEtiqueta — (asociativas N:N de Etiqueta, RC-04)

Tablas de unión muchos a muchos.

| Tabla | Columnas | Tipo |
| --- | --- | --- |
| FotoEtiqueta | FotoId, EtiquetaId | uniqueidentifier, uniqueidentifier (PK compuesta) |
| ComentarioEtiqueta | ComentarioId, EtiquetaId | uniqueidentifier, uniqueidentifier (PK compuesta) |

### 1.11 ConflictoSync — (conceptual: ConflictoSync)

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| ConflictoSyncId | uniqueidentifier | NOT NULL | newsequentialid() |
| Tipo | tinyint | NOT NULL | — (1=marcadores en un mismo radio, 2=edición en conflicto) |
| RelevamientoId | uniqueidentifier | NOT NULL | — |
| RecursosInvolucrados | nvarchar(max) | NOT NULL | — (lista serializada de marcadores/observaciones) |
| EstadoResolucion | tinyint | NOT NULL | 1 (1=pendiente, 2=resuelto) |
| DecisorUsuarioId | uniqueidentifier | NULL | — (jefe de área o raíz) |

### 1.12 RegistroAuditoria — (conceptual: RegistroAuditoría)

Asiento inalterable; retención ≥ 12 meses (RN-07, ADR-14).

| Columna | Tipo físico | Nulabilidad | Default |
| --- | --- | --- | --- |
| RegistroAuditoriaId | uniqueidentifier | NOT NULL | newsequentialid() |
| AutorUsuarioId | uniqueidentifier | NOT NULL | — |
| Momento | datetime2(3) | NOT NULL | sysutcdatetime() |
| Operacion | nvarchar(200) | NOT NULL | — |
| RecursoAfectado | nvarchar(300) | NOT NULL | — |

### 1.13 RegistroCambioSync — (conceptual: RegistroCambioSync) — esquema SQLite local del móvil (ADR-05)

Tabla de la cola local del dispositivo, no del backend. Tipos físicos de SQLite (sqlite-net-pcl).

| Columna | Tipo físico SQLite | Nulabilidad | Notas |
| --- | --- | --- | --- |
| CambioId | TEXT (GUID) | NOT NULL | PK; identificador único de idempotencia (RC-03) |
| TipoOperacion | INTEGER | NOT NULL | 1=alta, 2=edición |
| Entidad | TEXT | NOT NULL | nombre de la entidad afectada |
| ReferenciaEntidad | TEXT | NOT NULL | identificador local de la entidad |
| MarcaTemporal | TEXT (ISO-8601 UTC) | NOT NULL | base del last-write-wins (RN-04) |
| EstadoSincronizacion | INTEGER | NOT NULL | 0=pendiente, 1=enviado, 2=confirmado |

El backend no persiste RegistroCambioSync; consume los cambios subidos por el motor de sincronización (CU-07) y los consolida. La idempotencia se valida contra el CambioId (RC-03).

## 2. Atributos con tipo de dato físico

Los tipos físicos están en las tablas de §1. Notas:

- Coordenadas: `decimal(9,6)` para latitud/longitud (≈0,1 m de precisión). El cálculo de distancia para el radio (RN-02) se realiza en la capa Application; no se usa el tipo espacial nativo en v1 para mantener portabilidad.
- Enums de dominio (`RolJerarquico`, `Estado`, `FuenteCoordenada`, `Tipo` de conflicto, `EstadoResolucion`) se almacenan como `tinyint` con restricción check (§4).
- Marcas temporales en UTC (`datetime2(3)` en backend; ISO-8601 en SQLite).

## 3. Índices

| Índice | Tabla | Columnas | Tipo | Motivación |
| --- | --- | --- | --- | --- |
| IX_Usuario_Area | Usuario | AreaId | No único | Autorización y listados por área (RN-01) |
| IX_Relevamiento_Area | Relevamiento | AreaId | No único | Filtrado de relevamientos por área (CU-01, CU-08) |
| IX_Observacion_Relevamiento | Observacion | RelevamientoId | No único | Carga de observaciones de un relevamiento (CU-08) |
| IX_Observacion_Marcador | Observacion | MarcadorId | No único | Agrupación de observaciones por marcador (CU-04, CU-09) |
| IX_Marcador_Relevamiento | Marcador | RelevamientoId | No único | Vista de marcadores sobre mapa (CU-08) |
| IX_Foto_Marcador | Foto | MarcadorId | No único | Carrusel de fotos del marcador (CU-09) |
| UX_AsignacionAgente | AsignacionAgente | RelevamientoId, AgenteUsuarioId | Único | Evita asignación duplicada del mismo agente (CU-01) |
| UX_FotoEtiqueta | FotoEtiqueta | FotoId, EtiquetaId | Único (PK) | Evita etiqueta duplicada en la foto (RC-04) |
| IX_Conflicto_Relevamiento_Estado | ConflictoSync | RelevamientoId, EstadoResolucion | No único | Lista de conflictos pendientes (CU-11, CU-12) |
| IX_Auditoria_Momento | RegistroAuditoria | Momento | No único | Reconstrucción del historial por período (RN-07) |
| UX_SQLite_Cambio | RegistroCambioSync (SQLite) | CambioId | Único | Idempotencia de la cola local (RC-03) |

## 4. Restricciones

| Tipo | Definición | Regla de origen |
| --- | --- | --- |
| PK | Cada tabla por su `*Id`; PK compuesta en FotoEtiqueta/ComentarioEtiqueta | — |
| FK | Usuario.AreaId → Area; Relevamiento.AreaId → Area; AsignacionAgente.RelevamientoId → Relevamiento; AsignacionAgente.AgenteUsuarioId → Usuario; Observacion.RelevamientoId → Relevamiento; Observacion.MarcadorId → Marcador; Marcador.RelevamientoId → Relevamiento; Foto.MarcadorId → Marcador; Foto.ObservacionId → Observacion; Comentario.MarcadorId → Marcador; Comentario.FotoId → Foto; FotoEtiqueta/ComentarioEtiqueta → Foto/Comentario/Etiqueta | RC-02, RC-04, RC-07 |
| Check CK_Usuario_Rol | RolJerarquico IN (1,2,3,4) | RC-05 |
| Check CK_Usuario_AreaRequerida | RolJerarquico IN (3,4) ⇒ AreaId NOT NULL | RC-05 |
| Check CK_Relevamiento_Estado | Estado IN (1,2,3) | RC-06 |
| Check CK_Relevamiento_Radio | RadioAgrupacionMetros > 0 | RN-02 |
| Check CK_Foto_Fuente | FuenteCoordenada IN (1,2) | RN-03 |
| Check CK_Observacion_Marcador | SinGeorreferenciar = 1 ⇒ MarcadorId IS NULL; SinGeorreferenciar = 0 ⇒ MarcadorId IS NOT NULL | RC-02, RN-03 |
| Check CK_Conflicto_Estado | EstadoResolucion IN (1,2) | RN-04 |
| Única UX_AsignacionAgente | (RelevamientoId, AgenteUsuarioId) | RC-07 |
| Integridad de área (aplicación) | AsignacionAgente: el agente pertenece al área del relevamiento; se valida en Application (no expresable como check entre tablas) | RC-07 |
| Inmutabilidad (aplicación) | RegistroAuditoria sin UPDATE/DELETE dentro de la retención; sin permisos de modificación | RN-07 |

## 5. Migración inicial

Tooling de migración del proyecto: migraciones de EF Core (ADR-09). Migración inicial identificada `20260601_InitialCreate`:

- Crea las 12 tablas backend (§1.1 a §1.12, incluidas las asociativas de §1.10).
- Declara PK, FK, índices (§3) y restricciones check y únicas (§4).
- No crea RegistroCambioSync en el backend (vive solo en SQLite del móvil, §1.13).

El esquema SQLite local se crea por la app móvil al primer arranque (creación de la tabla `RegistroCambioSync` y de las tablas locales espejo necesarias para la captura offline), versionado con la app (ADR-05).

## 6. Estrategia multi-tenant

multi_tenant=false. No hay partición por tenant: no existe columna discriminadora de tenant, esquema por tenant ni base por tenant. GeoVial atiende a un único organismo. Las "áreas" son ámbito de autorización (RN-01): acotan qué recursos ve y modifica cada usuario, pero no segmentan físicamente la base. El aislamiento entre áreas es lógico, aplicado por el módulo transversal de autorización (CU-14), no por particionamiento de datos.

## 7. Trazabilidad

| Tabla lógica | Entidad conceptual (02) | CU que la consumen | RC/RN aplicables |
| --- | --- | --- | --- |
| Usuario | Usuario | CU-02, CU-03, CU-13, CU-14 | RC-05; RN-01, RN-06, RN-08 |
| Area | Área | CU-01, CU-03, CU-14 | RN-01 |
| Relevamiento | Relevamiento | CU-01, CU-02, CU-08, CU-10, CU-11 | RC-06; RN-02, RN-05 |
| AsignacionAgente | AsignaciónAgente | CU-01, CU-02 | RC-07; RN-01 |
| Observacion | Observación | CU-04, CU-05, CU-06, CU-08 | RC-02; RN-02, RN-03, RN-05 |
| Marcador | Marcador | CU-04, CU-05, CU-09, CU-11, CU-12 | RC-01; RN-02, RN-04 |
| Foto | Foto | CU-04, CU-05, CU-09 | RC-04; RN-03 |
| Comentario | Comentario | CU-09 | RC-04; RN-05 |
| Etiqueta + asociativas | Etiqueta | CU-08, CU-09 | RC-04; RN-05 |
| ConflictoSync | ConflictoSync | CU-07, CU-11, CU-12 | RC-01; RN-02, RN-04 |
| RegistroAuditoria | RegistroAuditoría | CU-13, CU-14 | RN-07, RN-08 |
| RegistroCambioSync (SQLite) | RegistroCambioSync | CU-06, CU-07 | RC-03; RN-04 |

Las 12 entidades conceptuales de 02 quedan cubiertas una a una.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Modelo lógico inicial: 12 entidades con tipos físicos (SQL Server backend + SQLite cola local), índices, restricciones, migración inicial 20260601_InitialCreate y trazabilidad al conceptual. Generado por AG-05 |
