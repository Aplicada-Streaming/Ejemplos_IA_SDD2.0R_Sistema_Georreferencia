# Sprint Review — Sprint 48

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-48_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-48_v1.0.md`:

> El objetivo es un indicador de estado de sincronización que muestre, de un vistazo: al día / pendientes / sincronizando / sin conexión / error, combinando las dos colas (comentarios S11 + capturas S42) con la conectividad.

Veredicto: Cumplido.

Explicación corta: el agente no tenía forma de saber si su trabajo estaba a salvo o seguía en la cola local. Se agregó un núcleo `ResumenSincronizacion.Calcular` (función pura que deriva el estado y el texto a partir de conectividad + pendientes + sync en curso + último error) y un `MonitorSincronizacion` que lo alimenta desde las dos colas reales y la conectividad, notificando cambios. El `MainPage` muestra un label que se actualiza al aparecer, al capturar y alrededor de la sincronización. Cierra la acción de retro reiterada en S45, S46 y S47.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-SYNC-IND | Funcionalidad | El label muestra "Todo sincronizado" / "N pendientes por sincronizar" / "Sincronizando…" / "Sin conexión · N…" / "No se pudo sincronizar · N (se reintentará)" | El agente sabe de un vistazo si su trabajo está a salvo |
| BT-SYNC-NUCLEO | Robustez | El estado combina comentarios + capturas + conectividad; función pura testeada en el gate | Coherente con captura offline (S42) y auto-sync (S45) |

## 3. Feedback recibido

- Cierra una preocupación real del trabajo de campo: la ambigüedad entre "subí" y "quedó en cola" era riesgosa con offline + auto-sync.
- El núcleo quedó como **función pura** + un monitor delgado: máxima testabilidad, mínima superficie de glue móvil.
- Honesto: el indicador se refresca al aparecer la pantalla y alrededor de la sync manual; un auto-sync silencioso en background se reflejará al volver a la pantalla (mejora futura anotada en retro).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **400** (357 unitarias + 43 de integración), +14 unitarias: 9 de `ResumenSincronizacionTests` (cada estado, prioridad de "sincronizando", singular/plural, error sin pendientes, normalización de negativos) y 5 de `MonitorSincronizacionTests` (refresco suma las dos colas y notifica, sin conexión, error, marcar sincronizando conserva el conteo). Cobertura del gate: Domain 88,5 % / 79,8 %; Application 87,3 % / 76,0 % (umbral 80 % / 70 %); el núcleo nuevo (`GeoVial.Sync`) a 94,6 % / 89,0 %. El MAUI compila para `net10.0-android`.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-SYNC-IND | Historia | Aceptada (indicador con los cinco estados, cableado en `MainPage`) |
| BT-SYNC-NUCLEO | Tarea | Aceptada (cálculo puro + monitor en el gate) |
| BT-SYNC-TESTS | Tarea | Aceptada (14 casos en el gate) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 48 se traslada. |

Pendiente menor (no compromiso): reflejar en el indicador el **fin de un auto-sync en background** (hoy se refleja al volver a la pantalla); verificación on-device del indicador. Backlog restante (revisión funcional §5/§6): bandeja sin georreferenciar navegable, avisos de conflictos/pendientes en la UI, biométrico nativo, y el sprint de limpieza del listado de área (retro S47).

## 7. Decisiones tomadas durante el review

- Núcleo como **función pura** + monitor delgado, para maximizar la cobertura en el gate y minimizar la glue.
- Total de pendientes = comentarios + capturas (un solo número para el agente; no se discrimina por tipo en el texto).
- El refresco se dispara en eventos de UI (aparecer, capturar, sincronizar); el auto-sync silencioso se reflejará al volver a la pantalla (mejora futura).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 48 (indicador de estado de sincronización). Veredicto Cumplido, velocity 8, 0 carry-over, 400 pruebas (+14); cobertura del gate mantenida; núcleo nuevo a 94,6/89,0. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
