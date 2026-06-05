# Plan de Iteración — Sprint 46

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-46_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-03-06
**Fecha fin:** 2028-03-17
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S43–S45); capacidad sugerida estricta 9 SP. Se compromete la **idempotencia de la captura en el backend** (8 SP), brecha de robustez señalada por la revisión funcional y abierta de raíz por S42 (captura offline) + S45 (auto-sync que drena la cola). Todo el núcleo (dominio + aplicación) vive en el gate.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cerrar la grieta de **doble captura** que abrió la combinación S42 + S45: la cola de capturas se drena automáticamente al recuperar señal y, ante un corte **después** de que el server recibió la observación pero **antes** de marcarla subida en la cola local, el reintento vuelve a postear la misma captura → **observación + foto duplicadas** (y, eventualmente, marcador duplicado). El backend debe ser **idempotente** respecto de la clave de captura que el cliente ya genera (`CapturaId`): un reenvío de la misma `CapturaId` devuelve la observación original en vez de crear otra.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-CAPT-IDEMP | Historia | Idempotencia de la captura en el backend: aceptar `CapturaId` del cliente y deduplicar (reenvío devuelve la observación original, sin duplicar) | Alta | 5 | Dev backend (AG-06) | Cerrada |
| BT-CAPT-CLIENTE | Tarea | El cliente móvil envía la `CapturaId` (que ya genera localmente) en el alta de observación | Media | 1 | Dev móvil (AG-08) | Cerrada |
| BT-CAPT-TESTS | Tarea | Cubrir en el gate la deduplicación (unit del handler + E2E doble-POST por HTTP) | Alta | 2 | QA (AG-05) | Cerrada |

Total de puntos comprometidos: 8 SP. Núcleo en `GeoVial.Domain` + `GeoVial.Application` (en el gate); ajuste menor en el cliente móvil.

## 4. Alcance técnico

1. **Dominio (`GeoVial.Domain`, en el gate):** `Observacion` recibe una clave de idempotencia opcional `CapturaId` (`Guid?`), seteada por las fábricas `Georreferenciada`/`EnBandejaSinGeorreferenciar`. Nullable para no romper observaciones existentes ni el comportamiento previo cuando el cliente no la envía.
2. **Aplicación (`GeoVial.Application`, en el gate):** `CapturarObservacionCommand` agrega `Guid? CapturaId` (opcional). El handler, si llega una `CapturaId` ya registrada, **devuelve la `ResultadoCaptura` original** (misma `ObservacionId`/`FotoId`/`MarcadorId`) sin crear nada ni reauditar — misma semántica que la idempotencia del sync (`CambioAplicado`: "ya aplicado, se confirma sin reaplicar"). Nuevo puerto `IObservacionRepository.ObtenerPorCapturaIdAsync`.
3. **Contrato (`GeoVial.Shared`):** `CapturarObservacionRequest` agrega `Guid? CapturaId = null` (al final, compatible con los llamadores de 3 args).
4. **API (`GeoVial.Api`):** el endpoint `POST /relevamientos/{id}/observaciones` propaga `req.CapturaId` al comando.
5. **Persistencia (`GeoVial.Infrastructure`):** configuración EF de `Observacion.CapturaId` + **índice único filtrado** (`WHERE CapturaId IS NOT NULL`) como red de seguridad ante concurrencia en SqlServer; implementación de `ObtenerPorCapturaIdAsync`; migración `CapturaIdempotente`.
6. **Cliente móvil (`GeoVial.Mobile`):** `ClienteCapturaHttp` envía `captura.CapturaId` (ya existente en `CapturaPendiente`) en el alta.

> Nota de honestidad técnica: el proveedor InMemory (que usan los tests E2E y el dev sin SQL Server) **no aplica índices únicos**, así que la red real contra el reintento secuencial —el escenario realista del auto-sync— es la **comprobación en el handler**. El índice único filtrado protege la carrera concurrente sólo en SqlServer (DEV/prod).

## 5. Definition of Done aplicada

- Reenviar una captura con la misma `CapturaId` no crea una segunda observación/foto: el backend devuelve la original (verificado por HTTP, doble-POST).
- Núcleo (dominio + handler) cubierto por pruebas en el gate; los tests de captura existentes siguen verdes (compatibilidad hacia atrás: sin `CapturaId` el comportamiento no cambia).
- Cobertura DoD respetada (Domain/Application líneas ≥80 %, ramas ≥70 %).
- El MAUI compila para `net10.0-android`; el cliente envía la `CapturaId`.
- Migración relacional generada y aplicable (`MigrateAsync` al arrancar sobre SqlServer).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Cambiar el DTO/command rompe los llamadores de 3 args | Media | Medio | `CapturaId` opcional al final (`= null`); los call sites existentes compilan sin cambios |
| El índice único con un solo NULL permitido (SqlServer) choca con observaciones sin clave | Media | Alto | Índice **filtrado** `WHERE CapturaId IS NOT NULL`: sólo indexa capturas con clave |
| InMemory no captura el duplicado por índice → falsa sensación de cobertura | Alta | Medio | La dedup se valida por el **handler** (no por el índice); el E2E corre sobre InMemory y ejercita justamente esa rama |
| La migración filtrada no aplica en SQLite (tests relacionales) | Baja | Bajo | SQLite acepta el filtro y trata los NULL como distintos; se valida corriendo la suite de integración |

## 7. Criterios de hecho del sprint

El Sprint 46 se considera completo cuando: el backend deduplica por `CapturaId` (reenvío devuelve la observación original sin duplicar), el cliente móvil envía la clave, el núcleo está cubierto en el gate sin romper los tests previos, la cobertura DoD se mantiene, el MAUI compila, la migración relacional queda generada, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | `revision-funcional-app_v1.0.md` (backlog: idempotencia de la captura en el backend); grieta abierta por S42 + S45 |
| CU | CU-06 (captura de observación), CU-07 (sincronización) |
| RN | Patrón de idempotencia análogo a RC-03 (sync) sobre la clave de cliente |
| Componentes | `GeoVial.Domain` (`Observacion`); `GeoVial.Application` (`Captura`); `GeoVial.Shared`; `GeoVial.Api`; `GeoVial.Infrastructure`; `GeoVial.Mobile` (`ClienteCapturaHttp`) |
| Calidad | definition-of-done §1.4 (cobertura), §1.5 (idempotencia) |
| Tests previstos | unit del handler (reenvío devuelve original, no duplica, no reaudita) + E2E doble-POST por HTTP |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 46 (idempotencia de la captura en el backend): `Observacion.CapturaId` + handler que deduplica por la clave de cliente (devuelve la original), DTO/endpoint/cliente móvil que la propagan, índice único filtrado + migración. Compromete 8 SP. Generado por AG-07 |
