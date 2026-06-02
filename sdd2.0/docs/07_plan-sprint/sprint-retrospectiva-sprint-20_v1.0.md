# Sprint Retrospectiva — Sprint 20

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-20_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Las tres pruebas E2E pasaron al primer intento: el diseño en capas (handlers ya probados) hizo que el camino completo por HTTP funcionara sin sorpresas.
- El helper de escenario (sembrar área → agente con credencial → relevamiento asignado vía el `DbContext` de la factory) resultó simple y reutilizable; era la pieza que faltaba para todo E2E de captura.
- Sembrar por `DbContext` y autenticar por HTTP fue más determinista que armar el escenario con la cascada de altas administrativas, y dejó las pruebas legibles.
- Aprovechar que el proveedor en memoria comparte base (nombre fijo) con identificadores únicos por escenario evitó interferencias entre pruebas sin montar una factory por test.

## 2. Qué no salió bien

- No entró el E2E del ciclo de edición en conflicto (sincronizar con colisión → listar → confirmar): requiere orquestar dos sincronizaciones del mismo recurso y se difirió por alcance.
- Las pruebas E2E dependen del seed de Development (área "Zona Norte"); si el seed cambia, el helper que toma "la primera área" debería sembrar también su propia área para no acoplarse.
- El proveedor en memoria comparte estado entre todas las pruebas de integración; con muchas más E2E convendría aislar la base por colección de pruebas.

## 3. Qué probar

- Un E2E del ciclo de edición en conflicto cuando se estabilice un helper de sincronización con colisión.
- Aislar la base en memoria por colección de pruebas (o factory por clase) si el set de E2E crece.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| E2E del ciclo de edición en conflicto (sync colisión → listar → confirmar) | AG-05 / QA | 2027-04-03 | Pendiente |
| Que el helper de escenario siembre su propia área (desacoplar del seed) | AG-05 | 2027-04-03 | Pendiente |
| Preparar el primer release: tag v1.0.0 de la librería y endurecimiento de release | AG-09 (DevOps) | 2027-04-03 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 19 | Estado actual |
| --- | --- |
| Conservar la posición del carrusel al recargar tras editar | Pendiente |
| Provisionar clave de mapas y mapa interactivo (deuda de US-13/US-21) | Pendiente |
| Convención de housekeeping de ramas (no encadenar branch-delete con merges en paralelo) | Aplicada en este sprint (housekeeping de git paso a paso) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 20 con 3 acciones nuevas y seguimiento de las del Sprint 19 (convención de housekeeping aplicada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
