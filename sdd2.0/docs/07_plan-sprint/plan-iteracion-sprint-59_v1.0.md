# Plan de Iteración — Sprint 59

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-59_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-09-04
**Fecha fin:** 2028-09-15
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points (Fibonacci).
- Capacidad: promedio móvil 7,0 SP (S56–S58). Se compromete una **mejora de UX acotada (5 SP)** que reusa núcleo ya testeado (S48); sin backend.

## 2. Objetivo del sprint

Cerrar el hallazgo **H-05** de la auditoría UX móvil (`evaluacion-ux-mobile_v1.0.md`): la **cinta de estado de conexión/sincronización no era persistente** (sólo aparecía en la solapa *Sync*; la pantalla crítica de *Captura* no mostraba el estado offline). La experiencia-de-uso §4.1 exige un **indicador de conexión persistente** en móvil. El objetivo es mostrar una cinta fija de estado (al día / pendiente / sincronizando / sin conexión / error) en las solapas de trabajo (**Captura, Revisión, Mapa, Bandeja**), reusando el `MonitorSincronizacion` (S48).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| H-05-CINTA | Historia | Cinta de conexión persistente en las 4 solapas de trabajo, reusando el monitor de sync | Alta | 3 | Dev móvil (AG-08) | Cerrada |
| BT-CINTA-NUCLEO | Tarea | Núcleo puro `PresentacionCintaConexion` (ícono+color por estado, WCAG 1.4.1) + tests | Media | 2 | Dev móvil (AG-08) | Cerrada |

Total: 5 SP (UX acotado; reusa el núcleo de sync de S48, sin backend).

## 4. Alcance técnico

- **Núcleo (gate, `GeoVial.Sync`):** `PresentacionCintaConexion.Para(ResumenSincronizacion) → CintaVisual(Icono, Texto, ColorFondo, ColorTexto)`. Cada `EstadoSync` lleva un **glifo distinto** (✓/●/↻/⚠), así no se distingue sólo por color (WCAG 2.2 AA, 1.4.1). Reusa `ResumenSincronizacion.Calcular` (S48, ya en el gate).
- **`CintaConexionView` (MAUI, `ContentView`):** se vincula al `MonitorSincronizacion` (singleton), se suscribe a `Cambiado` mientras está visible (Loaded/Unloaded), refresca al aparecer y pinta lo que decide el núcleo. Expone `SemanticProperties.Description` para el lector de pantalla.
- **Cableado:** la cinta se ubica **fija arriba** (fuera del `ScrollView`) en `CapturaPage` y `RevisionPage` (XAML, `Grid` Auto/*), y en `MapaPage`/`BandejaPage` (code-only, fila Auto del `Grid`). Cada página inyecta `MonitorSincronizacion` (ya registrado) y llama `Cinta.Vincular(monitor)`.
- **Sin cambios de backend/Application/Domain.** No hay registro nuevo en DI (la vista se instancia directo; el monitor ya es singleton).

## 5. Definition of Done aplicada

- Las 4 solapas de trabajo muestran la cinta de estado **fija** (no se va con el scroll); refleja en vivo al día / pendiente / sincronizando / **sin conexión** / error.
- La cinta usa ícono + texto (no sólo color) y anuncia el estado por accesibilidad.
- Suite del gate verde con tests del núcleo nuevo; cobertura DoD sin regresión.
- El MAUI compila y se verifica on-device (cinta verde online → roja sin conexión en Captura).

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| Fuga de suscripciones al cambiar de solapa | Media | Bajo | La vista se suscribe/desuscribe en Loaded/Unloaded |
| La cinta tapa contenido | Baja | Bajo | Va en fila `Auto` de un `Grid`; el contenido va en `*` |
| Color sin alternativa (accesibilidad) | Media | Medio | Glifo distinto por estado (WCAG 1.4.1) + descripción semántica |

## 7. Criterios de hecho del sprint

Completo cuando: la cinta de conexión es persistente y en vivo en Captura/Revisión/Mapa/Bandeja, con ícono+texto y accesible; la suite del gate verde con el núcleo cubierto; el MAUI compila y se verifica on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Auditoría UX móvil `evaluacion-ux-mobile_v1.0.md`, hallazgo **H-05** (P1) |
| CU/RN/UX | experiencia-de-uso §4.1 (indicador de conexión persistente), §5 (accesibilidad, no sólo color); wireframes-captura-movil §2 (cinta de estado) |
| Componentes | `GeoVial.Sync` (`PresentacionCintaConexion`); `GeoVial.Mobile` (`CintaConexionView`, `CapturaPage`, `RevisionPage`, `MapaPage`, `BandejaPage`) |
| Calidad | definition-of-done §1.4; gate Domain/Application líneas ≥80 % / ramas ≥70 % |
| Tests | `PresentacionCintaConexionTests` (ícono/color por estado, íconos distintos) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Plan del Sprint 59 (H-05: cinta de conexión persistente). Núcleo `PresentacionCintaConexion` (gate, WCAG 1.4.1) + `CintaConexionView` reusable en las 4 solapas, reusando el `MonitorSincronizacion` de S48. Sin backend. 5 SP (UX acotado). Generado por AG-07 |
