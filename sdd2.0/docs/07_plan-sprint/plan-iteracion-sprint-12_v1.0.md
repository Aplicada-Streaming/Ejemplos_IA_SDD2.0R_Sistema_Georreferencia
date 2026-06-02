# Plan de Iteración — Sprint 12

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-12_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-11-10
**Fecha fin:** 2026-11-21
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 13,0 SP. Se comprometen 13 SP. Continúa la plataforma móvil sobre `GeoVial.Sync` (Sprint 11): el valor testeable es el encolado de la captura offline y el coordinador de sincronización automática; la UI de captura y el sensor de conectividad de plataforma se incorporan en la cáscara MAUI, fuera del gate de pruebas.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Permitir que el agente recolecte observaciones sin conexión guardándolas localmente en la cola para sincronizar (US-16) y que la app, al recuperar señal, dispare la sincronización automáticamente avisando si había cambios pendientes (US-19), todo sobre la librería `GeoVial.Sync` y la cola SQLite del Sprint 11.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-16 | Historia | Recolectar observaciones sin conexión (encolado local) | Alta (Must) | 8 | Dev móvil | Pendiente |
| US-19 | Historia | Detectar la conectividad y sincronizar automáticamente | Alta (Must) | 5 | Dev móvil / Dev fullstack | Pendiente |

Total de puntos comprometidos: 13 SP. US-16 y US-19 (Must de EP-04) se apoyan en la cola y el motor del Sprint 11. La captura completa de campo (foto/GPS) y su pantalla pertenecen a la UI MAUI; este sprint entrega el encolado de la observación capturada y el coordinador de sincronización, que son el núcleo testeable.

## 4. Alcance técnico

Componentes que se construyen o modifican en `GeoVial.Sync` y la cáscara MAUI:

1. US-16 — Encolado de la captura offline (CU-06): un colector que toma los datos de una observación capturada y los encola como un cambio (`ChangeRecord`) en la cola local, de forma que se conserven sin pérdida hasta sincronizar (NFR de jornada completa). Se centraliza el formato del payload de comentario en un factory compartido con el cliente REST, garantizando que lo encolado en el cliente coincida con lo que consume `/sync`. Sin espacio de almacenamiento local, el guardado responde `ALMACENAMIENTO_LOCAL_INSUFICIENTE` y conserva lo ya guardado (US-16 CA-03): la cola SQLite traduce el error de disco lleno (SQLITE_FULL) a una excepción tipada de la librería.
2. US-19 — Sincronización automática por conectividad (CU-07 §1): un coordinador que, al notificarse la recuperación de conexión (`IConnectivityMonitor`), dispara la sincronización por el motor del Sprint 11; si había cambios pendientes en la cola lo notifica, y si no, solo baja las actualizaciones sin subir nada (US-19 CA-02). La detección de conectividad de plataforma (Android/MAUI) es una implementación de `IConnectivityMonitor` en la cáscara móvil.
3. Cáscara MAUI — `GeoVial.Mobile` incorpora una implementación de `IConnectivityMonitor` sobre la API de red de MAUI y cablea el colector y el coordinador por inyección de dependencias. Se mantiene fuera de la solución/CI (el empaquetado del APK requiere un Android SDK provisionado, hallazgo del Sprint 11).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- Una observación capturada sin conexión se encola localmente; 100 capturas se conservan sin pérdida; sin espacio responde `ALMACENAMIENTO_LOCAL_INSUFICIENTE` sin perder lo ya guardado.
- Al recuperar conexión con cambios pendientes, la sincronización se dispara automáticamente y se notifica que había pendientes; sin pendientes, solo baja actualizaciones.
- El payload encolado en el cliente coincide con el contrato `/sync` (round-trip verificado).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| La detección de disco lleno depende de la plataforma | Media | Medio | Traducir SQLITE_FULL a una excepción tipada en la cola; el colector la propaga; prueba que verifica la propagación del contrato |
| La sincronización automática en el evento de conectividad puede tragarse errores (async void) | Media | Medio | Exponer el núcleo como un método awaitable testeable; el manejador del evento lo invoca; pruebas sobre el método y sobre el disparo por evento |
| El empaquetado de la app MAUI no compila en CI (Android SDK) | Alta | Bajo | Mantener `GeoVial.Mobile` fuera de la solución/CI; concentrar las pruebas en `GeoVial.Sync` |

## 7. Criterios de hecho del sprint

El Sprint 12 se considera completo cuando US-16 (encolado offline) y US-19 (auto-sync por conectividad) están terminadas según la DoD con sus pruebas verdes; el ciclo capturar sin señal → recuperar conexión → sincronizar automáticamente queda demostrado de punta a punta (con cola y motor); la cáscara MAUI cablea el sensor de conectividad y el colector; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que avanzan | US-16 (recolección offline), US-19 (conectividad y sync automática) |
| CU que avanzan | CU-06 (recolección/encolado sin conexión), CU-07 (sincronización automática) |
| NB que avanzan | NB-03 (continuidad operativa sin conexión) |
| ADRs que gobiernan | ADR-05 (SQLite/cola), ADR-07 (librería de sync), contratos-abstractions-sync |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 12 (captura offline y sincronización automática por conectividad). Compromete US-16 y US-19 (13 SP) sobre `GeoVial.Sync`. La UI de captura y el empaquetado de la app MAUI quedan fuera del gate de pruebas. Generado por AG-07 |
