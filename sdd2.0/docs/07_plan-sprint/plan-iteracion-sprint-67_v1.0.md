# Plan de Iteración — Sprint 67

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-67_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2029-01-08
**Fecha fin:** 2029-01-19
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 5,0 SP (S64–S66). **Pasada de accesibilidad** dedicada: cierra los dos hallazgos de la auditoría UX que se habían diferido por no ser "cambios de una línea" (H-14 anuncios por región en vivo, H-07 truncado de etiquetas de pestañas).

## 2. Objetivo del sprint

**Cerrar los dos últimos hallazgos de la auditoría UX móvil** (`evaluacion-ux-mobile_v1.0.md`), ambos de accesibilidad/UI:

- **H-07** — las etiquetas de las pestañas inferiores se truncaban ("Captu…") en Android porque el sistema **agranda la etiqueta seleccionada**. Se fuerza el modo "labeled" (todas al mismo tamaño) para que no trunquen.
- **H-14** — el estado de sincronización se veía (cinta) pero no se **anunciaba**: el agente en terreno, sin mirar la pantalla, no se enteraba de "sin conexión"/"sincronizando". Se anuncia por el lector de pantalla al **cambiar** el estado (región en vivo, WCAG 2.2 §4.1.3 status messages).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| H-14 | Historia | Anuncios por región en vivo del estado de sincronización (lector de pantalla) | Media | 3 | Dev móvil (AG-08) | Cerrada |
| H-07 | Historia | Truncado de etiquetas de pestañas (Android) | Media | 2 | Dev móvil (AG-08) | **Diferida** (limitación de MAUI Shell, ver §6) |

Total comprometido: 5 SP. Completado: 3 SP (H-14). H-07 se difiere tras investigarla (no es un fix de plataforma chico; necesita un renderer de Shell propio).

## 4. Alcance técnico

- **H-07 (`Platforms/Android/TabBarPersonalizacion.cs`, nuevo):** customización de plataforma que, vía el `ShellHandler.Mapper`, localiza la `BottomNavigationView` (cuando ya existe en el árbol de vistas) y le fija `LabelVisibilityMode = labeled` (valor 1). Así Android no agranda la etiqueta activa y "Captura"/"Revisión" dejan de truncarse. Se engancha **una sola vez** (al primer layout con la barra presente). Registrado desde `MauiProgram` bajo `#if ANDROID`.
- **H-14 (`CintaConexionView`):** al **cambiar** el estado (evento `Cambiado` del `MonitorSincronizacion`, no en el render inicial) se anuncia el texto del estado con `SemanticScreenReader.Default.Announce(...)` (región en vivo). Reusa el texto que ya produce `PresentacionCintaConexion` (gate); el `try/catch` evita ruido si no hay lector activo. La descripción semántica (`SemanticProperties`) ya estaba (S59).
- **Sin núcleo nuevo ni backend.** Glue de UI/plataforma; el gate (498) no se ve afectado (cambios sólo en `GeoVial.Mobile`).

## 5. Definition of Done aplicada

- **H-14 (hecho):** al cambiar el estado de conexión, el lector de pantalla anuncia el nuevo estado; el gate sigue verde (sin cambios; los archivos tocados son de `GeoVial.Mobile`, fuera del gate); el MAUI compila y arranca sin fallos.
- **H-07 (diferido):** el truncado **no** se resolvió por la vía de plataforma (el tab bar de MAUI Shell re-pisa los ajustes de la `BottomNavigationView`). Se revirtió el código frágil; se difiere con el hallazgo documentado. Backlog de la auditoría: P1 + P2 + H-14 hechos; H-07 pendiente como limitación de framework.

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| El binding de `BottomNavigationView`/`LabelVisibilityMode` varía entre versiones de Material | Media | Medio | Se usa el valor literal `1` (LABEL_VISIBILITY_LABELED) en vez del nombre del binding; verificación visual on-device |
| La barra aún no existe cuando corre el mapping | Media | Bajo | Se espera al `GlobalLayout` y se aplica una sola vez cuando la barra ya está |
| El anuncio es ruidoso (se repite) | Baja | Bajo | Sólo se anuncia al **cambiar** el estado, no en cada render |

## 7. Criterios de hecho del sprint

Completo cuando: las etiquetas de pestaña no truncan; el cambio de estado se anuncia por el lector de pantalla; el gate sigue verde; el MAUI compila y se verifica on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Auditoría UX móvil `evaluacion-ux-mobile_v1.0.md`, hallazgos **H-07** y **H-14** (diferidos en S63) |
| UX | experiencia-de-uso §5 (WCAG 2.2 AA: 1.4.1 no sólo color, 4.1.3 status messages) |
| Componentes | `GeoVial.Mobile` (`TabBarPersonalizacion`, `MauiProgram`, `CintaConexionView`) |
| Calidad | definition-of-done §1.4 |
| Tests | Sin núcleo nuevo (UI/plataforma); H-07 verificado on-device (visual), H-14 por construcción + on-device |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Plan del Sprint 67 (pasada de accesibilidad: H-14 anuncios por región en vivo + H-07 truncado de pestañas). Resultado: H-14 hecho (3 SP); H-07 diferido tras investigarla (limitación de MAUI Shell). Comprometido 5, completado 3. Sin núcleo ni backend. Generado por AG-07 |
