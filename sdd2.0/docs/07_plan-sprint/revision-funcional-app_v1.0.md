# Revisión Funcional — GeoVial (app móvil + backend)

**Proyecto:** GeoVial
**Documento:** revision-funcional-app_v1.0.md
**Versión:** 1.0
**Estado:** Informe
**Fecha:** 2026-06-03
**Autor:** Equipo SDD 2.0 (revisión asistida: 4 subagentes de análisis + pruebas en vivo contra el backend y el dispositivo)

## 1. Resumen ejecutivo

Se hizo una revisión funcional completa: análisis del código (API, app móvil, dominio/aplicación) contra la **línea base de requisitos** del SDD (14 casos de uso, 8 reglas de negocio), y **pruebas en vivo** creando usuarios de los 4 roles, login/logout, control de acceso y captura, contra el backend real (SqlServer DEV) y la app en el dispositivo.

**Veredicto: el backend cubre casi todos los casos de uso, pero la app móvil cumple solo de forma parcial el rol del agente de campo, y hay un BUG CRÍTICO que rompe el flujo central.**

- 🔴 **Crítico (bloqueante):** un Jefe de Área **no puede asignar agentes a un relevamiento** contra la base real (`503 ACCION_NO_AUDITADA`). Sin asignación, **ningún agente puede capturar** → el ciclo de campo (el corazón del producto) está roto en producción. La CI no lo detecta porque usa EF InMemory.
- 🟠 **Importante:** la app móvil no implementa varias capacidades "Must" del agente: selección de relevamiento (siempre toma "el primero"), captura **realmente offline** (la captura real postea directo, no encola), método de seguridad / reingreso offline (RN-06), y auto-sincronización.
- 🟢 **Bien:** autenticación real (S40), jerarquía de roles y autorización por área (verificado en vivo), captura con georreferenciación EXIF + ubicación manual, revisión con carrusel, y mapa OSM.

## 2. 🔴 Bug crítico — asignación de agentes rota contra la base real

**Síntoma (reproducido en vivo):** como Jefe de Área, `POST /api/v1/relevamientos/{id}/agentes` devuelve `503 ACCION_NO_AUDITADA`. En consecuencia, el agente asignado no existe y su captura da `403 ACCESO_NO_AUTORIZADO` (no está asignado). Ocurre con cualquier relevamiento (el del seed y uno creado por el propio jefe).

**Causa raíz (confirmada con el log SQL de EF):** al asignar, el agregado `Relevamiento` agrega una `AsignacionAgente` nueva **a través de su colección de dominio** (`_asignaciones.Add(...)`). En el `SaveChanges` (compartido con el gate de auditoría), EF genera un **`UPDATE [AsignacionAgente] ... WHERE AsignacionAgenteId=@p` con `OUTPUT 1`** en vez de un `INSERT`: trata la entidad nueva como **Modified** en lugar de **Added**. El UPDATE afecta 0 filas → `DbUpdateConcurrencyException` → `DbUpdateException` → el repositorio de auditoría lo traga (`catch (DbUpdateException) → return false`) → el handler devuelve `ACCION_NO_AUDITADA` (RN-07: si no se audita, se rechaza).

- El esquema de la tabla `AsignacionAgente` es correcto (PK, FK a `Relevamiento`, índice único); un `INSERT` directo por SQL **funciona**. El problema es el **estado de cambio** que EF asigna a la entidad agregada por navegación.
- `AsignacionAgente` es el **único** lugar donde un hijo se persiste a través de la colección del agregado en vez de un `repository.AddAsync(...)` explícito; por eso es la única escritura afectada (crear relevamiento, capturar, comentar, etc. usan `AddAsync` y andan).
- **Por qué la CI no lo ve:** los tests corren con `UseInMemoryDatabase`, que **no** aplica el chequeo de "se esperaba 1 fila afectada"; el `UPDATE` de 0 filas no lanza. Contra **SqlServer** sí. Mismo patrón que los bugs móviles de S40: invisible sin ejecución real.

**Direcciones de arreglo (a confirmar en el sprint de fix):**
1. Marcar explícitamente la asignación nueva como `Added` (p. ej. exponer un `IRelevamientoRepository.AgregarAsignacion(...)` que haga `_db.Add(asignacion)`), o
2. Configurar la clave `AsignacionAgenteId` como generada por la base / ajustar la convención para que EF detecte `Added` por clave no seteada, o
3. Revisar el acceso por campo de la navegación (`PropertyAccessMode.Field`) para que el change-tracking detecte el alta como inserción.
4. **Imprescindible:** agregar una prueba de integración contra SqlServer/LocalDB (o el provider real) que cubra "asignar agente → capturar", porque el gate InMemory no protege esta clase de bug.

## 3. Cumplimiento de la app móvil (rol Agente de Campo)

Línea base: capacidades **Must** del cliente móvil (F-M-01…19, CU-02/04/05/06/07/09). Estado verificado por código + pruebas en vivo/dispositivo.

| ID | Capacidad (agente) | Estado | Nota |
| --- | --- | --- | --- |
| F-M-01 | Login con credenciales | 🟢 Cumple | Login real (S40). Un AgenteCampo **sí** puede loguear por `/auth/login` (verificado). Usuario "raiz" viene precargado en el campo (cosmético). |
| F-M-02 | Configurar método de seguridad (precondición offline) | 🔴 No | Endpoint `/auth/metodo-seguridad` existe; la app no lo usa. |
| F-M-03 | Reingreso en terreno sin conexión (RN-06) | 🔴 No | Endpoint `/auth/reingreso` existe; verificado en vivo (sin método → 409). La app no implementa el flujo. |
| F-M-04 | Ver lista de relevamientos asignados | 🔴 No | La app toma **siempre el primero** (`PrimerRelevamientoAsync`); no lista ni filtra por asignados. |
| F-M-05 | Seleccionar/abrir un relevamiento | 🔴 No | Ídem: sin selección si hay varios. |
| F-M-06 | Tomar foto + georreferenciación EXIF | 🟢 Cumple | `CapturaPage`, verificado on-device (S40). |
| F-M-07 | Crear/asociar marcador por radio | 🟢 Cumple | En backend, vía captura. |
| F-M-08 | Comentarios | 🟠 Parcial | Se comenta desde `RevisionPage` (online); la captura no manda comentario inicial. |
| F-M-09 | Etiquetas | 🟠 Parcial | Etiquetado de fotos desde `RevisionPage` (online). |
| F-M-10 | Ubicar manualmente sin EXIF | 🟢 Cumple | `CapturaPage` (panel de ubicación). |
| F-M-11 | Bandeja sin georreferenciar | 🟠 Parcial | Se informa el estado, pero no hay una bandeja navegable. |
| F-M-12 | **Recolectar realmente sin conexión** | 🔴 No | La captura real (`CapturaPage`) **postea directo** al backend; la cola offline solo la usa un botón **stub** en `MainPage` (genera IDs aleatorios). No hay captura offline de punta a punta. |
| F-M-13 | Encolar cambios localmente | 🟠 Parcial | La cola SQLite existe, pero no la alimenta la captura real. |
| F-M-14 | Sincronizar al recuperar internet | 🟠 Parcial | Sync manual funciona (contra "el primero"); **auto-sync desactivada** (`CoordinadorAutoSync` sin relevamiento activo). |
| F-M-15 | Aviso de pendientes al recuperar señal | 🔴 No | — |
| F-M-16 | Carrusel de fotos | 🟢 Cumple | `RevisionPage`. |
| F-M-17 | Navegar marcadores | 🟢 Cumple | `RevisionPage`. |
| F-M-18 | Editar comentarios/etiquetas en campo | 🟠 Parcial | Online, desde `RevisionPage`. |
| F-M-19 | Ver mapa con marcadores | 🟢 Cumple | `MapaPage` (S40). |

**Resumen móvil:** 7 cumplen, 6 parciales, 6 no. Las brechas “No” más graves son la **captura offline real** (F-M-12/13, que es el sentido mismo de la app de campo, NB-03) y la **selección de relevamiento** (F-M-04/05).

## 4. Backend / API y RBAC (verificado en vivo)

- 🟢 **Cobertura de casos de uso casi completa.** La API expone auth, gestión jerárquica de usuarios, relevamientos, captura, revisión, conflictos, sincronización, export/import y auditoría (mapa de endpoints completo levantado por el análisis).
- 🟢 **Jerarquía y onboarding verificados en vivo:** Raíz→JefeGeneral→JefeArea→AgenteCampo; cada nivel crea y da credencial al inmediato inferior (cada nivel solo administra al de abajo — Raíz **no** puede crear directamente un Jefe de Área).
- 🟢 **RBAC correcto:** el agente recibe `403` al crear usuarios, crear relevamientos o consultar auditoría; sin token → `401`; clave/usuario inválidos → `401` uniforme (sin enumeración).
- 🟢 **RN-06 verificada:** `/auth/reingreso` y `/auth/offline` sin método de seguridad → `409`; tras `/auth/metodo-seguridad` → `204`.
- 🔴 **El bug de la §2** es la única falla funcional del backend hallada, pero es **bloqueante** del flujo de campo.
- 🟠 **Web no expuesta en este alcance** (la administración de usuarios, resolución de conflictos, transiciones y export/import son web; la app móvil no las cubre por diseño).

## 5. Brechas priorizadas (backlog propuesto)

| Prioridad | Ítem | Tipo | Origen |
| --- | --- | --- | --- |
| **P0** | Arreglar asignación de agentes (INSERT vs UPDATE) + prueba de integración con DB real | Bug | §2 |
| **P0** | Captura **offline real**: que `CapturaPage` encole en la cola SQLite y suba al sincronizar (cerrar F-M-12/13) | Historia | F-M-12 |
| **P1** | Selección de relevamiento asignado (listar + elegir; quitar “primer relevamiento”) | Historia | F-M-04/05 |
| **P1** | Método de seguridad + reingreso offline (RN-06) en la app | Historia | F-M-02/03 |
| **P1** | Activar auto-sincronización al recuperar conectividad (asignar relevamiento activo) | Historia | F-M-14 |
| **P2** | Bandeja navegable de observaciones sin georreferenciar | Historia | F-M-11 |
| **P2** | Aviso/badge de conflictos y de cambios pendientes | Historia | F-M-15/24 |
| **P2** | BaseAddress y usuario configurables (sacar hardcode de localhost/"raiz") | Tarea | gaps |
| **P3** | Refresh automático de token / re-login ante 401 | Tarea | gaps |

## 6. Plan propuesto (sprints)

Cadencia actual 8 SP/sprint. Propuesta:

- **Sprint 41 — P0 (bloqueante):** arreglar el bug de asignación de agentes con su prueba de integración contra base real (cierra el flujo onboarding→captura). Núcleo testeable + verificación contra SqlServer/LocalDB. *(Recomendado primero: sin esto el producto no funciona de punta a punta.)*
- **Sprint 42 — Captura offline real (F-M-12/13):** `CapturaPage` encola en la cola SQLite; el `MotorSincronizacion` sube las capturas; reemplazar el stub de `MainPage`.
- **Sprint 43 — Selección de relevamiento (F-M-04/05):** pantalla de selección del relevamiento asignado; quitar “primer relevamiento” de todas las páginas.
- **Sprint 44 — Método de seguridad + reingreso offline (RN-06, F-M-02/03)** y **auto-sync (F-M-14)**.
- **Sprint 45+ — P2/P3:** bandeja sin georreferenciar, avisos de conflictos/pendientes, configurabilidad, refresh de token.

## 7. Método de la revisión

- **Análisis de código (4 subagentes en paralelo):** superficie de API + auth/roles; pantallas y acciones de la app móvil + brechas; línea base de requisitos (CU/RN/F-M/F-W) del SDD; capacidades del dominio/aplicación.
- **Pruebas en vivo contra el backend real (SqlServer DEV):** creación de usuarios de los 4 roles, login/logout, control de acceso por rol/área, captura, RN-06, y reproducción + diagnóstico del bug crítico con el log SQL de EF.
- **Verificación en el dispositivo (moto g42):** login, mapa y captura (de S40).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Revisión funcional completa de la app móvil + backend contra la línea base del SDD, con pruebas en vivo multiusuario. Hallazgo crítico: asignación de agentes rota contra la base real (INSERT vs UPDATE; invisible para la CI InMemory). Backlog priorizado y plan de sprints 41–45. Generado por el equipo SDD 2.0 |
