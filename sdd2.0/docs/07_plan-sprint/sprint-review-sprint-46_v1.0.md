# Sprint Review — Sprint 46

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-46_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-46_v1.0.md`:

> El backend debe ser idempotente respecto de la clave de captura que el cliente ya genera (`CapturaId`): un reenvío de la misma `CapturaId` devuelve la observación original en vez de crear otra.

Veredicto: Cumplido.

Explicación corta: la combinación S42 (captura offline) + S45 (auto-sync que drena la cola) dejaba abierta una grieta de **doble captura**: si la señal se cortaba **después** de que el server creó la observación pero **antes** de que la cola local la marcara subida, el reintento volvía a postear la misma captura → observación + foto (y eventualmente marcador) duplicados. Ahora el cliente envía la `CapturaId` que ya generaba localmente, y el handler de captura, ante una clave ya registrada, **devuelve la observación original** (mismo `ObservacionId`/`FotoId`/`MarcadorId`) sin crear nada ni reauditar y sin reevaluar el estado del relevamiento (la operación ya tuvo éxito) — misma semántica que la idempotencia del sync (`CambioAplicado`).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-CAPT-IDEMP | Robustez | Doble POST de la misma captura (por HTTP): el segundo devuelve la observación original y la revisión muestra un solo marcador con una sola foto | Ya no hay riesgo de duplicar al reconectar |
| BT-CAPT-CLIENTE | Funcionalidad | El cliente móvil envía la `CapturaId` en el alta de observación | El reintento del auto-sync es seguro |
| BT-CAPT-TESTS | Calidad | Unit del handler (reenvío devuelve original, no duplica, no reaudita) + E2E doble-POST | La dedup queda cubierta en el gate |

## 3. Feedback recibido

- Cierra la grieta que abrieron S42 + S45: el ciclo offline ahora es **seguro ante reintentos**, no sólo automático.
- Buena reutilización del patrón de idempotencia que ya existía en el sync (`CambioAplicado`); el equipo lo aplicó a la captura con la clave de cliente.
- Honestidad técnica registrada: InMemory (tests E2E / dev) no aplica índices únicos, así que la red real contra el reintento **secuencial** —el escenario realista— es la comprobación en el handler; el índice único filtrado protege la **carrera concurrente** sólo en SqlServer. El E2E corre sobre InMemory y ejercita justamente la rama del handler.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **382** (341 unitarias + 41 de integración), +4 respecto de S45: 3 unitarias (`CapturaTests`: el reenvío devuelve la original; no duplica ni reaudita; sin `CapturaId` no hay dedup —compatibilidad—) y 1 de integración (`CapturaE2ETests`: doble-POST idempotente por HTTP). Los tests de captura previos siguen verdes (compatibilidad hacia atrás). Cobertura del gate: Domain líneas 88,5 % / ramas 79,8 %; Application líneas 87,3 % / ramas 76,2 % (umbral 80 % / 70 %). El MAUI compila para `net10.0-android`. Migración relacional `CapturaIdempotente` generada (columna + índice único filtrado).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-CAPT-IDEMP | Historia | Aceptada (dedup por `CapturaId`: el reenvío devuelve la observación original sin duplicar) |
| BT-CAPT-CLIENTE | Tarea | Aceptada (el cliente móvil envía la clave) |
| BT-CAPT-TESTS | Tarea | Aceptada (unit + E2E en el gate) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 46 se traslada. |

Pendiente menor (no compromiso): la **prueba del ciclo offline completo en modo avión** on-device sigue como acción de retro (reiterada de S42/S45). Backlog restante (revisión funcional §5/§6): bandeja sin georreferenciar navegable, avisos de conflictos/pendientes en la UI, biométrico nativo, endpoint "relevamientos asignados a mí", indicador de estado de sincronización en la UI.

## 7. Decisiones tomadas durante el review

- Guardar la `CapturaId` en la propia `Observacion` (no en una tabla aparte como `CambioAplicado`) porque el reenvío necesita recuperar el `FotoId` original para encadenar la subida del binario.
- Cortocircuitar la idempotencia **al inicio** del handler, antes de validar estado del relevamiento: una captura ya registrada debe devolverse aunque el relevamiento se haya cerrado entretanto (la operación ya había tenido éxito).
- Índice único **filtrado** (`WHERE CapturaId IS NOT NULL`) para permitir múltiples observaciones sin clave en SqlServer.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 46 (idempotencia de la captura en el backend). Veredicto Cumplido, velocity 8, 0 carry-over, 382 pruebas (+4); cobertura del gate mantenida; migración relacional generada. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
