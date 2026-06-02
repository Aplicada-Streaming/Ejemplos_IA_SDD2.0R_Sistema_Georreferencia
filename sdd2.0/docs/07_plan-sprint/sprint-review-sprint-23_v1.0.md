# Sprint Review — Sprint 23

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-23_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-23_v1.0.md`:

> Verificar de punta a punta por HTTP el ciclo de conflictos por radio (CU-11/CU-12): detectar marcadores en un mismo radio y resolverlos —unificar o mantener separados— sobre la API real, cerrando la cobertura E2E del flujo de conflictos que faltaba. Se extrae además un helper de escenario compartido que siembra su propia área, desacoplándose del seed.

Veredicto: Cumplido.

Explicación corta: dos pruebas E2E recorren por HTTP el ciclo completo de conflictos por radio: el agente captura dos observaciones a ~33 m (con radio 15 m quedan en marcadores distintos), se amplía el radio a 50 m, se detecta el conflicto, se lista (tipo radio, con sus dos marcadores) y se resuelve —unificando en uno (la lista queda vacía y el relevamiento con un único marcador) o manteniéndolos separados (ambos marcadores se conservan)—. Se extrajo un helper de escenario compartido (`EscenarioE2E`) que siembra su **propia** área, desacoplando las pruebas del seed de Development (acción de la retro del Sprint 20).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| E2E | Prueba | Capturar 2 marcadores → ampliar radio → detectar → unificar → 1 marcador, sin pendientes | Resolución por unificación verificada |
| E2E | Prueba | Detectar → mantener separados → ambos marcadores conservados, sin pendientes | Resolución por separación verificada |
| E2E | Infra | Helper de escenario que siembra su propia área (desacoplado del seed) | Pruebas más robustas |

## 3. Feedback recibido

- El ciclo de conflictos por radio (detección + resolución), entregado en backend (S05) y web (S18), queda verificado de punta a punta por HTTP.
- El helper de escenario compartido y desacoplado del seed habilita más E2E a bajo costo y elimina una dependencia frágil.
- Con esto, los flujos centrales del MVP (captura, ubicación manual, revisión, edición, autorización y conflictos) están cubiertos por E2E.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 305 verdes (270 unitarias + 35 de integración), +2 respecto del Sprint 22 (los dos E2E del ciclo de conflictos). El gate de cobertura de dominio/aplicación se mantiene (sin lógica nueva). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-E2E-CONF | Tarea | Aceptada (helper de escenario compartido + 2 pruebas E2E del ciclo de conflictos por radio) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 23 se traslada. |

El E2E del ciclo de edición en conflicto (sync con colisión → listar → confirmar) requiere orquestar la sincronización con colisión y sigue en el backlog, junto con el pulido móvil (mapa interactivo, caché) y el SBOM firmado.

## 7. Decisiones tomadas durante el review

- Provocar el conflicto de radio ampliando el radio tras dos capturas distintas (en vez de sincronizar desde dos dispositivos), que es el camino más simple por HTTP.
- Ejecutar el ciclo con el agente del escenario (autorizado en su área, RN-01), sin necesidad de un jefe aparte.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 23 (endurecimiento E2E del ciclo de conflictos por radio: helper compartido + 2 pruebas E2E). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
