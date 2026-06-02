# Plan de Iteración — Sprint 13

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-13_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-11-24
**Fecha fin:** 2026-12-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 13,0 SP (S10–S12); capacidad sugerida estricta 14 SP. Se compromete US-32 (8 SP). El valor testeable —el núcleo de la demo sobre la superficie pública de `GeoVial.Sync`— entra al gate de cobertura; la cáscara MAUI de la demo queda fuera de CI, igual que la app `GeoVial.Mobile` (hallazgo de S11/S12: el empaquetado del APK requiere un Android SDK provisionado).

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Publicar una demo autónoma que ejercite la librería de sincronización `GeoVial.Sync` contra un backend simulado (US-32, EP-09), de modo que un integrador externo pueda evaluar el reuso sin depender de GeoVial: alta de registros locales, sincronización contra un mock con visualización del estado de la cola (pendiente → sincronizado) y resolución básica de conflictos.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-32 | Historia | Evaluar la librería de sincronización con una demo autónoma | Should | 8 | Dev móvil / Dev fullstack | Pendiente |

Total de puntos comprometidos: 8 SP. US-32 (Should de EP-09) se apoya en la cola (US-17), el motor (US-18) y el encolado offline (US-16) ya entregados. El sprint queda por debajo de la capacidad sugerida (8 de 14 SP): US-32 es la única historia de EP-09 y se mantiene el patrón de alcance acotado deliberado para no abrir un segundo frente (captura de campo EP-03) a medio refinar.

## 4. Alcance técnico

La demo vive en `samples/02-sync-maui-demo` y ejercita solo la superficie pública (Abstractions) de la librería. Se separa el núcleo testeable de la cáscara visual:

1. **`SyncDemo.Nucleo`** (biblioteca `net10.0`, dentro de la solución y del gate de CI): la lógica de la demo sobre los contratos públicos.
   - `BackendSimulado` (`ISyncBackendClient`): backend en memoria que consolida los cambios subidos y, para un conjunto configurable de recursos en conflicto, devuelve un `ConflictInfo` sin confirmar el cambio (queda pendiente hasta resolver). En la segunda sincronización (`since != null`) ofrece las actualizaciones acumuladas, demostrando la bajada de cambios (US-19 CA-02).
   - `ResolutorConflictos`: resolución básica (US-32 CA-02). «Mantener lo local» (RN-04, prevalece la última escritura): despeja el conflicto del recurso en el backend y la próxima sincronización lo confirma. «Aceptar lo remoto»: descarta el cambio local de la cola.
   - `CoordinadorDemo`: fachada que orquesta capturar (encolar vía `ChangeRecordFactory`), sincronizar (motor) y consultar el estado de la cola; consumida por la cáscara MAUI y por las pruebas de aceptación.
2. **`SyncDemo.Maui`** (cáscara `net10.0-android`, fuera de la solución/CI): una página que da de alta observaciones, muestra el estado de la cola, sincroniza contra `BackendSimulado` y ofrece la resolución básica del conflicto. Reusa `SyncDemo.Nucleo`; no referencia ningún proyecto de GeoVial salvo `GeoVial.Sync`.
3. **`README` del sample**: cómo correr la demo y qué superficie pública ejercita (SemVer de la librería, ADR-07).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- Con la demo autónoma y el backend simulado, dar de alta registros locales y sincronizar lleva el estado de la cola de pendiente a sincronizado (AT-32, sobre la superficie pública).
- Un cambio que choca con el estado del mock se reporta como conflicto y admite una resolución básica (mantener local → se confirma; aceptar remoto → se descarta).
- El núcleo de la demo (`SyncDemo.Nucleo`) respeta el gate de cobertura (líneas ≥ 80 %, branches ≥ 70 %); la cáscara MAUI queda fuera del gate.
- La demo ejercita exclusivamente las Abstractions de `GeoVial.Sync` (no referencia el dominio ni la API de GeoVial).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La demo MAUI no compila en CI (Android SDK) | Alta | Bajo | Mantener `SyncDemo.Maui` fuera de la solución/CI; el núcleo testeable vive en `SyncDemo.Nucleo` (net10.0 puro) |
| El backend simulado diverge del contrato real `/sync` | Media | Medio | `BackendSimulado` implementa el mismo `ISyncBackendClient`; las pruebas reutilizan la cola SQLite y el motor reales, no dobles |
| La resolución de conflictos invade alcance de la web (US-26) | Media | Bajo | La resolución de la demo es básica (mantener/aceptar) sobre la cola y el mock; la resolución completa sobre mapa sigue siendo US-26 (web) |

## 7. Criterios de hecho del sprint

El Sprint 13 se considera completo cuando US-32 está terminada según la DoD con AT-32 en verde: la demo autónoma da de alta registros locales, sincroniza contra el backend simulado mostrando la cola pasar de pendiente a sincronizado, y reporta un conflicto admitiendo su resolución básica; `SyncDemo.Nucleo` está dentro del gate de cobertura y la cáscara `SyncDemo.Maui` fuera de CI; el `README` del sample documenta cómo correrla; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que avanzan | US-32 (demo autónoma de evaluación de la librería) |
| CU que avanzan | CU-06 (alta/encolado local), CU-07 (sincronización), CU-12 (resolución de conflictos, alcance básico) |
| EP | EP-09 (Librería de sincronización publicada) |
| NB que avanzan | NB-03 (continuidad operativa sin conexión) |
| BT derivadas | BT-15, BT-16, BT-17, BT-22 |
| ADRs que gobiernan | ADR-05 (SQLite/cola), ADR-07 (librería de sync, SemVer), contratos-abstractions-sync |
| Tests previstos | acceptance/AT-32-demo-sync |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 13 (demo autónoma de evaluación de la librería de sincronización). Compromete US-32 (8 SP) sobre la superficie pública de `GeoVial.Sync`. El núcleo `SyncDemo.Nucleo` entra al gate; la cáscara `SyncDemo.Maui` queda fuera de CI. Generado por AG-07 |
