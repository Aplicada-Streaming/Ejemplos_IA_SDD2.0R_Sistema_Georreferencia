# Contrato REST — GeoVial

**Proyecto:** GeoVial
**Documento:** contratos-rest_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Trazabilidad upstream:** ADR-02, ADR-03, ADR-11; CU-01 a CU-14; RN-01 a RN-08

## 1. Alcance del contrato

Contrato externo de la API REST del backend de GeoVial, consumida por el front web Blazor y la app móvil MAUI (ADR-02). Materializa los CU de administración, captura, sincronización y revisión: CU-01 (crear/asignar), CU-02 (login/selección), CU-03 (jerarquía), CU-04/CU-05 (captura y ubicación), CU-07 (sincronización), CU-08 (revisión y exportación/importación), CU-09 (marcador), CU-10 (estados), CU-11/CU-12 (conflictos), CU-13/CU-14 (auditoría/autorización transversal).

## 2. Formato

- OpenAPI 3.x generado desde la API y versionado en el repositorio (PROJECT-README §6).
- Transporte HTTP/JSON. Errores en `application/problem+json` (RFC 7807, ADR-11).
- Versionado por URL: prefijo `/api/v1/` (ADR-02).
- Autenticación: bearer token JWT (ROPC), claims `sub` y `role` (ADR-03). Todos los endpoints salvo el de login exigen `Authorization: Bearer <token>`.

## 3. Operaciones

Listado representativo (la especificación completa vive en el OpenAPI generado). Versionadas bajo `/api/v1/`.

| Método | Ruta | Operación | CU |
| --- | --- | --- | --- |
| POST | `/api/v1/auth/token` | Login ROPC, emite access + refresh JWT | CU-02 |
| POST | `/api/v1/auth/refresh` | Refresh condicionado al método de seguridad (móvil) | CU-02 |
| GET | `/api/v1/usuarios` / POST / DELETE | Administrar jerarquía de usuarios y áreas | CU-03 |
| POST | `/api/v1/relevamientos` | Crear relevamiento | CU-01 |
| POST | `/api/v1/relevamientos/{id}/asignaciones` | Asignar/reasignar agentes | CU-01 |
| PATCH | `/api/v1/relevamientos/{id}/estado` | Transicionar estado / reabrir | CU-10 |
| POST | `/api/v1/relevamientos/{id}/observaciones` | Asentar observación con coordenada | CU-04, CU-05 |
| GET | `/api/v1/relevamientos/{id}/marcadores` | Marcadores sobre mapa | CU-08, CU-09 |
| POST | `/api/v1/relevamientos/{id}/sync` | Subir cambios locales y bajar actualizaciones | CU-07 |
| GET | `/api/v1/relevamientos/{id}/conflictos` | Listar conflictos pendientes | CU-11 |
| POST | `/api/v1/relevamientos/{id}/conflictos/{cid}/resolucion` | Resolver conflicto | CU-12 |
| GET | `/api/v1/relevamientos/{id}/export` | Exportar relevamiento completo (ZIP) | CU-08 |
| POST | `/api/v1/relevamientos/import` | Importar relevamiento completo (ZIP) | CU-08 |

## 4. Esquemas de datos

DTOs principales (definidos en `GeoVial.Shared`):

- `TokenRequest` { usuario, contraseña } → `TokenResponse` { accessToken, refreshToken, expiraEn }.
- `RelevamientoDto` { id, identificacionObra, estado, radioAgrupacionMetros, areaId }.
- `ObservacionDto` { id, marcadorId?, momentoCaptura, sinGeorreferenciar, fuenteCoordenada, fotos[], comentarios[], etiquetas[] }.
- `MarcadorDto` { id, latitud, longitud, enConflicto, observaciones[] }.
- `SyncRequest` { cambios: [ { cambioId, tipoOperacion, entidad, referenciaEntidad, marcaTemporal, payload } ] } → `SyncResponse` { confirmados[], conflictos[], actualizaciones[] }.
- `ConflictoDto` { id, tipo, recursosInvolucrados[], estadoResolucion }.

### 4.1 Contrato de exportación/importación en ZIP

La exportación (CU-08, 5.A) produce un único archivo ZIP con: un manifiesto JSON del relevamiento (datos, observaciones, marcadores, comentarios, etiquetas) y los binarios de las fotos referenciadas. La importación (CU-08, 5.B) consume ese ZIP y reconstruye el relevamiento, validando la pertenencia de área (RN-01) y la coherencia del manifiesto. Content-type `application/zip` en `export`; `multipart/form-data` con el ZIP en `import`. Toda exportación/importación se audita (RN-07) y respeta el acotamiento por rol y área (RN-08).

## 5. Manejo de errores

Formato `application/problem+json` (RFC 7807, ADR-11) con extensión `code`. Catálogo de códigos tomado de las RN y los CU:

| code | status HTTP | Origen |
| --- | --- | --- |
| `ACCESO_NO_AUTORIZADO` | 403 | RN-01, CU-04/CU-08 |
| `ACCESO_DATO_PERSONAL_NO_AUTORIZADO` | 403 | RN-08 |
| `FINALIDAD_NO_PERMITIDA` | 403 | RN-08 |
| `RELEVAMIENTO_SOLO_LECTURA` | 409 | RN-05, CU-04/CU-12 |
| `TRANSICION_INVALIDA` | 409 | RN-05, CU-10 |
| `REAPERTURA_NO_AUTORIZADA` | 403 | RN-05, CU-10 |
| `MARCADORES_EN_RADIO` | 409 | RN-02, CU-11 |
| `UNIFICACION_NO_AUTORIZADA` | 403 | RN-02 |
| `OBSERVACION_SIN_GEORREFERENCIA` | 422 | RN-03, CU-04 |
| `FUENTE_UBICACION_INCORRECTA` | 422 | RN-03 |
| `CONSOLIDACION_INVALIDA` | 409 | RN-04, CU-07 |
| `CONFLICTO_NO_MARCADO` | 409 | RN-04, CU-07 |
| `SINCRONIZACION_INTERRUMPIDA` | 503 | CU-07 |
| `CONFLICTO_INEXISTENTE` | 409 | CU-12 |
| `OFFLINE_NO_HABILITADO` | 403 | RN-06, CU-02 |
| `REINGRESO_SIN_METODO_SEGURIDAD` | 401 | RN-06, CU-02 |
| `ARCHIVO_EXPORTACION_INVALIDO` | 422 | CU-08 |
| `ACCION_NO_AUDITADA` | 409 | RN-07, CU-08/CU-13 |
| `AUDITORIA_INMUTABLE` | 403 | RN-07 |

Cada respuesta de error incluye `type`, `title`, `status`, `detail` y `code`.

## 6. Versionado del contrato

- Versionado por URL: `/api/v1/`. Un cambio incompatible introduce `/api/v2/` y deprecia la versión anterior con un período de coexistencia.
- Cambios compatibles hacia atrás (agregar campos opcionales, nuevos endpoints) no cambian la versión de URL.
- El catálogo de códigos de error es aditivo: no se reutiliza un `code` para un significado distinto.
- El contrato OpenAPI se valida en CI; un breaking change no declarado falla el build.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que materializa | CU-01, CU-02, CU-03, CU-04, CU-05, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12 |
| RN que cubre | RN-01, RN-02, RN-03, RN-04, RN-05, RN-06, RN-07, RN-08 |
| ADR que lo gobierna | ADR-02 (REST), ADR-03 (auth), ADR-11 (errores) |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Contrato REST inicial: operaciones, DTOs, contrato de exportación/importación ZIP, catálogo de errores Problem Details y versionado por URL. Generado por AG-05 |
