# Sprint Review — Sprint 11

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-11_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-11_v1.0.md`:

> Arrancar la plataforma móvil construyendo la librería de sincronización `GeoVial.Sync` (superficie pública de Abstractions, ADR-07) y la cola local de cambios sobre SQLite (US-17), con un motor que sube los cambios encolados al endpoint de sincronización ya construido (`/sync`, Sprint 09) y procesa su respuesta, dejando demostrable de punta a punta el ciclo encolar → sincronizar contra el backend, e incorporar una cáscara MAUI mínima que cablea la librería.

Veredicto: Cumplido.

Explicación corta: `GeoVial.Sync` expone la superficie pública del contrato (`IChangeQueue`, `ISyncEngine`, `ISyncBackendClient`, `IConflictReporter`, `IConnectivityMonitor` y los tipos `ChangeRecord`/`SyncResult`/`ConflictInfo`/`SyncOptions`); la cola `ColaCambiosSqlite` (US-17) encola, lee ordenado por marca temporal, confirma y vacía de forma idempotente por `ChangeId`; el motor `MotorSincronizacion` sube los pendientes por el cliente REST `ClienteSyncHttp` contra `/api/v1/relevamientos/{id}/sync`, marca los confirmados, reporta conflictos y reanuda sin duplicar ante una interrupción. La cáscara MAUI `GeoVial.Mobile` cablea la librería por inyección de dependencias.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-17 | Historia | Encolar un cambio en SQLite, leerlo como pendiente ordenado y vaciarlo al confirmar; re-encolar el mismo `ChangeId` no duplica | Cola robusta e idempotente |
| BT-15 | Backlog técnico | El motor sube los pendientes al `/sync` real (cliente REST), confirma y reporta conflictos | Contrato de sync funcional |
| BT-15 | Backlog técnico | Ante un corte, el motor lanza `SyncInterrupted` y conserva los pendientes para reanudar | Reanudación sin pérdida |
| BT-15 | Backlog técnico | La cáscara MAUI `GeoVial.Mobile` cablea cola + motor + cliente por DI | Plataforma móvil arrancada |

## 3. Feedback recibido

- La librería de sincronización y la cola local quedan probadas contra el endpoint `/sync` del backend, cerrando el lazo cliente↔servidor del módulo de sincronización.
- Sobre esta base, el sprint siguiente puede abordar la captura offline (US-16) y la sincronización automática por conectividad (US-19), y la UI de campo de la app.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 13 |
| Puntos completados | 13 |
| Velocity efectiva | 13 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 242 verdes (213 unitarias + 29 de integración), +8 respecto del Sprint 10. Cobertura: dominio 89,6 % líneas / 79,8 % branches; aplicación 89,9 % / 81,8 %; `GeoVial.Sync` 95,4 % / 92,8 %; `GeoVial.FileHosting` 100 % / 100 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-15 | Backlog técnico | Aceptada (superficie pública + motor + cliente REST; testeada contra `/sync`) |
| US-17 | Historia | Aceptada (cola SQLite idempotente) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 11 se traslada. |

Hallazgo del spike: la cáscara MAUI `GeoVial.Mobile` compila su código y cablea `GeoVial.Sync`, pero el empaquetado completo del APK de Android requiere un Android SDK provisionado (plataforma, build-tools, RID `android-arm64`) que no está disponible en el entorno de CI actual. Por eso el proyecto `GeoVial.Mobile` se mantiene fuera de la solución y del gate de pruebas; su construcción completa se hará en un entorno de desarrollo móvil. El valor verificado del sprint (librería + cola + motor) es independiente del workload MAUI. La captura offline (US-16) y la conectividad (US-19) llegan en el sprint siguiente.

## 7. Decisiones tomadas durante el review

- Mantener `GeoVial.Mobile` como cáscara fuera de la solución/CI hasta disponer de un entorno con el Android SDK provisionado; concentrar las pruebas en la librería.
- Planificar US-16 (captura offline) y US-19 (conectividad y sincronización automática) para el próximo sprint, sobre la base de `GeoVial.Sync`.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 11 (spike de plataforma móvil: librería `GeoVial.Sync` y cola SQLite). Veredicto Cumplido, velocity 13, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
