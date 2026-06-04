# Plan de Iteración — Sprint 42

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-42_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-01-11
**Fecha fin:** 2028-01-23
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S39–S41); capacidad sugerida estricta 9 SP. Se compromete la **captura offline real** del cliente móvil (8 SP), brecha P0 de la revisión funcional. Núcleo de cola+motor en el gate; cliente HTTP y UI fuera de CI, verificados on-device.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cerrar la brecha **F-M-12/13** de la revisión funcional (`revision-funcional-app_v1.0.md` §3/§5): la captura de campo **no era realmente offline** — `CapturaPage` posteaba directo al backend y la cola SQLite sólo la usaba un botón *stub* de `MainPage`. Sin conexión no se podía capturar, que es el sentido mismo de la app de campo (NB-03).

Meta: que una captura (foto + coordenada) se **encole localmente** y funcione sin conexión, y se **suba al reconectar** (manual o al sincronizar), conservándose sin pérdida durante la jornada.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-16-OFFLINE | Historia | Cola local de capturas (foto + coordenada) + motor que la drena al backend; `CapturaPage` encola en vez de postear directo | Alta | 6 | Dev móvil (AG-08) | Cerrada |
| BT-CAP-TESTS | Tarea | Núcleo testeable en el gate: `MotorCapturas` (drain) + `ColaCapturasSqlite` (persistencia, BLOB) con pruebas | Alta | 2 | QA (AG-05) | Cerrada |

Total de puntos comprometidos: 8 SP. Cliente móvil; el núcleo con lógica (cola + motor) vive en `GeoVial.Sync` (en el gate).

## 4. Alcance técnico

1. **Núcleo (`GeoVial.Sync`, en el gate):**
   - `CapturaPendiente`: una captura encolada (foto en bytes + coordenada EXIF/manual + relevamiento + marca temporal); `CapturaId` como clave de idempotencia local.
   - `IColaCapturas` + `ColaCapturasSqlite`: cola sobre SQLite que persiste la foto (BLOB) y la coordenada hasta subirla; `INSERT OR IGNORE` por `CapturaId` (no duplica); error tipado si el disco está lleno (US-16 CA-03).
   - `ICapturaBackendClient` (puerto) + `MotorCapturas`: el motor drena la cola por lotes (orden por marca temporal), sube cada captura y marca como subidas las aceptadas; ante un corte de conexión conserva lo pendiente y señala la interrupción (`SyncInterruptedException`); ante un rechazo del backend (4xx) deja la captura y no cicla. Paralelo a `MotorSincronizacion` (que sincroniza comentarios por /sync).
2. **Cliente REST (`GeoVial.Mobile`, fuera de CI):** `ClienteCapturaHttp` implementa `ICapturaBackendClient`: POST observación + POST binario de la foto (reusa `ConstructorContenidoMultipart`), sobre el `HttpClient` compartido con el token de sesión.
3. **UI (`CapturaPage`, fuera de CI):** "Enviar captura" **encola** (funciona sin conexión) e intenta drenar en el acto; "Sincronizar capturas" (toolbar) drena lo pendiente; muestra las pendientes. La ubicación manual se fija **antes** de encolar (la captura viaja con su coordenada). Recuerda el relevamiento destino conocido para poder capturar sin conexión (la selección con varios relevamientos es S43).
4. **DI (`MauiProgram`):** registra `IColaCapturas` (SQLite `cola-capturas.db`), `ICapturaBackendClient` y `MotorCapturas`.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica. Criterios específicos:

- Una captura se encola localmente y se sube al backend al sincronizar; sin conexión queda en la cola sin pérdida y se sube al reconectar.
- `MotorCapturas` y `ColaCapturasSqlite` cubiertos por pruebas en el gate (drena todo, orden por marca temporal, corte de conexión conserva lo pendiente, rechazo 4xx no cicla; persistencia de la foto BLOB + idempotencia). La suite .NET pasa a **354** y sigue verde.
- El MAUI compila para `net10.0-android`.
- **Verificación on-device (moto g42):** login como agente asignado → tomar foto → "Enviar captura" → la captura se encola y se sube; verificado contra el backend que la observación quedó creada.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Duplicar observaciones al reintentar (el endpoint de captura no es idempotente) | Media | Medio | El motor marca subida apenas el backend responde 2xx; el binario es best-effort (igual que el flujo online). Duplicado sólo ante caída entre subida y marca; documentado |
| Sin conexión no se conoce el relevamiento destino | Media | Medio | Se recuerda el último relevamiento conocido (online); la selección con varios es S43 |
| La cola crece con fotos grandes (BLOB) | Baja | Medio | El pipeline de imagen comprime; error tipado si el disco se llena (US-16 CA-03) |

## 7. Criterios de hecho del sprint

El Sprint 42 se considera completo cuando: la captura se encola localmente y se sube al sincronizar (verificado on-device), el núcleo (cola + motor) está cubierto por pruebas en el gate, la suite .NET sigue verde (354), el MAUI compila para android, y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | `revision-funcional-app_v1.0.md` §3/§5 (F-M-12/13: captura offline real) |
| Necesidad / CU | NB-03 (continuidad sin conexión); CU-06 (recolección offline), CU-07 (sincronización) |
| Componentes | `GeoVial.Sync` (`CapturaPendiente`, `IColaCapturas`, `ColaCapturasSqlite`, `ICapturaBackendClient`, `MotorCapturas`); `GeoVial.Mobile` (`ClienteCapturaHttp`, `CapturaPage`, `MauiProgram`) |
| Calidad | definition-of-done §1.4 |
| Tests previstos | `MotorCapturasTests` + `ColaCapturasSqliteTests` (8 casos); verificación on-device |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan del Sprint 42 (captura offline real, F-M-12/13): cola SQLite de capturas + `MotorCapturas` (núcleo en el gate) + `ClienteCapturaHttp` y refactor de `CapturaPage` a encolar/drenar. Suite a 354. Verificado on-device (captura encolada y subida; observación creada en el backend). Compromete 8 SP. Generado por AG-07 |
