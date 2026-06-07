# Sprint Review — Sprint 59

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-59_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-59_v1.0.md`:

> Mostrar una cinta fija de estado (al día / pendiente / sincronizando / sin conexión / error) en las solapas de trabajo (Captura, Revisión, Mapa, Bandeja), reusando el `MonitorSincronizacion` (S48).

Veredicto: Cumplido.

Explicación corta: se agregó una **cinta de estado de conexión persistente** fija arriba de las 4 solapas de trabajo. Reusa el `MonitorSincronizacion` (S48) y una nueva función pura `PresentacionCintaConexion` (gate) que asigna **ícono + color** por estado (✓ verde al día, ● ámbar pendiente, ↻ azul sincronizando, ⚠ rojo sin conexión/error) — **no sólo color** (WCAG 2.2 AA 1.4.1) — y describe el estado por accesibilidad. Cierra el hallazgo **H-05** (P1) de la auditoría UX: antes el estado offline sólo se veía en la solapa *Sync*; ahora está visible en *Captura* (la pantalla crítica en terreno) y en el resto. Verificado on-device: verde "✓ Todo sincronizado" online → rojo "⚠ Sin conexión" al cortar la señal.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| H-05-CINTA | Funcionalidad | Captura online: cinta verde "✓ Todo sincronizado" (`17-captura-cinta-online.png`) | El estado ahora se ve sin entrar a Sync |
| H-05-CINTA | Estado offline | Captura sin señal: cinta roja "⚠ Sin conexión" (`18-captura-cinta-offline.png`) | Cumple §4.1 (indicador persistente) |
| BT-CINTA-NUCLEO | Núcleo | `PresentacionCintaConexion`: ícono distinto por estado (no sólo color) | Accesible (WCAG 1.4.1) |

## 3. Feedback recibido

- La cinta va **fija** (fuera del scroll), así no desaparece al desplazar el contenido durante la captura.
- Reuso real: el `MonitorSincronizacion` + `ResumenSincronizacion.Calcular` de S48 (ya en el gate) se aplicaron a 4 pantallas sin tocar lógica de sincronización.
- Se mantuvo accesibilidad: ícono + texto + descripción semántica, no dependencia del color.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **480** (436 unitarias + 44 de integración), **+5** del núcleo `PresentacionCintaConexion`. Integración sin cambios (el sprint no toca backend/Application/Domain; sólo `GeoVial.Sync` puro + glue MAUI). Cobertura del gate sin regresión (Domain 88,5/80,5; Application 87,8/77,1; `GeoVial.Sync` reusado y ampliado, cubierto). El MAUI compila (`net10.0-android`, arm64) y se redeployó al moto g42 con verificación on-device del antes/después online↔offline.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| H-05-CINTA | Historia | Aceptada (cinta persistente en las 4 solapas) |
| BT-CINTA-NUCLEO | Tarea | Aceptada (`PresentacionCintaConexion` + tests) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

Pendiente del backlog de la auditoría (P1 estructurales, próximos sprints): **H-02** (permiso de ubicación + Centrar-GPS), **H-01/H-03** (captura sobre el mapa), **H-04** (carrusel deslizable).

## 7. Decisiones tomadas

- La cinta se muestra **siempre** (incluso "al día", en verde), no sólo ante problemas: visibilidad del estado del sistema (Nielsen).
- Ícono distinto por estado para no depender del color (WCAG 1.4.1).
- Se mantiene el indicador del hub *Sync* (S48); la cinta lo complementa en las solapas de trabajo.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Review del Sprint 59 (H-05: cinta de conexión persistente). Cumplido, velocity 5, 0 carry-over, 480 pruebas (+5 del núcleo); verificado on-device online↔offline. Generado por AG-07 |
