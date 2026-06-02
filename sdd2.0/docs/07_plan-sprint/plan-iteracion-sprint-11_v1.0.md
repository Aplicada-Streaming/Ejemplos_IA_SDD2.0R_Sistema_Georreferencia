# Plan de Iteración — Sprint 11

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-11_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-10-27
**Fecha fin:** 2026-11-07
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (2 dev backend, 1 dev móvil, 1 QA part-time), `equipo_n: 4`. Este sprint incorpora el perfil móvil (MAUI/SQLite) para arrancar EP-04.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 12,3 SP. Se comprometen 13 SP. Es un sprint de arranque de la plataforma móvil (spike): el grueso del valor es la librería de sincronización y la cola local, testeables sin la UI; la shell MAUI se incorpora como cáscara que cablea la librería.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Arrancar la plataforma móvil construyendo la librería de sincronización `GeoVial.Sync` (superficie pública de Abstractions, ADR-07) y la cola local de cambios sobre SQLite (US-17), con un motor que sube los cambios encolados al endpoint de sincronización ya construido (`/sync`, Sprint 09) y procesa su respuesta, dejando demostrable de punta a punta el ciclo encolar → sincronizar contra el backend, e incorporar una cáscara MAUI mínima que cablea la librería.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-15 | Backlog técnico | Librería `GeoVial.Sync`: superficie pública (`IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, tipos) y motor | Alta (Must) | 8 | Dev móvil + Dev fullstack | Pendiente |
| US-17 | Historia | Encolar los cambios locales para sincronizar (cola SQLite) | Alta (Must) | 5 | Dev móvil | Pendiente |

Total de puntos comprometidos: 13 SP. BT-15 materializa el contrato `contratos-abstractions-sync` (ADR-07); US-17 implementa la cola local (CU-06). Ambos se apoyan en el endpoint `/sync` (US-18, Sprint 09). La captura offline completa (US-16) y la sincronización automática por conectividad (US-19) llegan en el sprint siguiente, sobre esta base.

## 4. Alcance técnico

Componentes que se construyen o modifican:

1. BT-15 — Librería `GeoVial.Sync` (ADR-07, contratos-abstractions-sync): un proyecto de librería .NET con la superficie pública de Abstractions —`IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, `IConflictReporter`, `IConnectivityMonitor`— y los tipos `ChangeRecord`, `SyncResult`, `ConflictInfo`, `SyncOptions`, con `ChangeId` como clave de idempotencia (RC-03). Incluye un motor `ISyncEngine` que lee los pendientes de la cola, los sube en orden por el `ISyncBackendClient`, marca los confirmados, reporta los conflictos y vacía lo sincronizado, reanudando sin duplicar; y un cliente HTTP que consume el contrato REST `/api/v1/relevamientos/{id}/sync` (Sprint 09).
2. US-17 — Cola local de cambios (CU-06): una implementación de `IChangeQueue` sobre SQLite (la persistencia local del cliente móvil, ADR-05) que encola un cambio local, lee los pendientes ordenados por marca temporal, marca confirmado y vacía lo sincronizado, de forma idempotente por `ChangeId`.
3. Cáscara móvil — un proyecto MAUI `GeoVial.Mobile` mínimo que referencia `GeoVial.Sync` y cablea la cola y el motor por inyección de dependencias, como punto de partida de la app de campo. La UI completa de captura llega en sprints posteriores.

El alcance es el arranque de la plataforma: la librería y la cola son el núcleo testeable; la app MAUI se incorpora como cáscara. La captura offline (US-16), la detección de conectividad y la sincronización automática (US-19) quedan para el sprint siguiente.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- La cola encola un cambio, lo lee como pendiente ordenado por marca temporal, lo marca confirmado y lo vacía; un `ChangeId` repetido no se encola dos veces.
- El motor sube los pendientes por el cliente de backend, marca los confirmados según la respuesta y reanuda sin duplicar tras una interrupción.
- La superficie pública de `GeoVial.Sync` está versionada (SemVer) y cubierta por pruebas; la librería compila como artefacto independiente.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La compilación de la UI MAUI depende de SDKs de plataforma (Android) y puede fallar en CI | Alta | Medio | Concentrar el valor y las pruebas en la librería `GeoVial.Sync` y la cola SQLite, independientes del workload MAUI; la cáscara MAUI se mantiene mínima y fuera del gate de pruebas |
| Acoplar la librería publicable al backend de GeoVial | Media | Medio | La superficie de Abstractions no depende del backend; el cliente REST concreto es una implementación separada de `ISyncBackendClient` |
| La cola SQLite podría perder el orden o duplicar ante reintentos | Media | Alto | Clave primaria por `ChangeId` (idempotencia) y orden por marca temporal; pruebas que verifican encolado idempotente, orden y vaciado |

## 7. Criterios de hecho del sprint

El Sprint 11 se considera completo cuando la librería `GeoVial.Sync` y la cola SQLite (US-17) están terminadas según la DoD con sus pruebas verdes; el ciclo encolar → subir → confirmar contra el endpoint `/sync` queda demostrado de punta a punta; la cáscara MAUI cablea la librería; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| BT que avanzan | BT-15 (librería de sincronización) |
| US que avanzan | US-17 (encolar cambios locales) |
| CU que avanzan | CU-06 (recolección y encolado sin conexión), CU-07 (sincronización, lado cliente) |
| NB que avanzan | NB-03 (continuidad operativa sin conexión) |
| ADRs que gobiernan | ADR-07 (publicación/versionado de la librería), ADR-05 (SQLite/cola), ADR-06 (conflictos), contratos-abstractions-sync |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 11 (spike de plataforma móvil: librería `GeoVial.Sync` y cola SQLite). Compromete BT-15 y US-17 (13 SP). Arranca EP-04 sobre el endpoint `/sync` del Sprint 09; la captura offline (US-16) y la conectividad (US-19) llegan después. Generado por AG-07 |
