# Plan de Iteración — Sprint 73

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-73_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2029-04-02
**Fecha fin:** 2029-04-13
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 6,0 SP (S70–S72). **Segundo sprint de la épica "Reporting / analytics": la UI web del tablero.** S72 entregó el backend del resumen; este sprint lo muestra en el front web.

## 2. Objetivo del sprint

**Que un jefe vea el resumen de actividad de un relevamiento en el front web:** elegir un relevamiento y ver sus totales (marcadores, conflictos, observaciones, bandeja, fotos, comentarios) y la productividad por agente.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| RPT-TABLERO | Historia | Página web del tablero: elegir relevamiento → ver el resumen | Alta | 5 | Front web (AG-08) | Cerrada |

Total: 5 SP (UI web sobre el endpoint de S72; sin núcleo nuevo).

## 4. Alcance técnico

- **`GeoVialApiCliente.ObtenerResumenAsync(relevamientoId)`** (`GeoVial.Web`): consume `GET /api/v1/relevamientos/{id}/resumen` con el token de la sesión; `null` si no hay acceso.
- **Página `Tablero.razor`** (`/tablero`, Blazor Server interactivo): login gate (reusa `GeoVialApiCliente`), lista los relevamientos visibles (`ListarRelevamientosAsync`) en un selector, y al elegir uno carga y muestra el resumen (tabla de totales + tabla de productividad por agente). Usa el patrón de las páginas existentes (Relevamientos/Revisión).
- **Link** desde `Home.razor` al tablero.
- **Sin cambios de backend/dominio.** Reusa el endpoint y los DTO de S72.

## 5. Definition of Done aplicada

- En `/tablero`, un jefe inicia sesión, elige un relevamiento y ve sus totales + productividad por agente.
- Un usuario sin acceso al área recibe un mensaje (el endpoint devuelve 404, sin filtrar).
- La web compila (`net10.0`) y sirve la página.
- El gate no se ve afectado (cambios sólo en `GeoVial.Web`, fuera del gate); se mantiene en 543.

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| La UI Blazor no es verificable de forma automatizada | Alta | Bajo | El endpoint está cubierto por el gate (S72); la página se verifica por build + que sirva la ruta; la interacción completa, manual |
| El selector no refresca el resumen al cambiar | Baja | Bajo | `@onchange` recarga el resumen; estado limpiado al cambiar de relevamiento |

## 7. Criterios de hecho del sprint

Completo cuando: la página del tablero muestra el resumen de un relevamiento elegido (totales + productividad); la web compila y sirve `/tablero`; el gate se mantiene; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Épica "Reporting / analytics", sprint #2 (UI del tablero); roadmap retro S72 |
| Componentes | `GeoVial.Web` (`Tablero.razor`, `GeoVialApiCliente`, `Home.razor`) |
| Calidad | definition-of-done §1.4; reusa el endpoint gate-testeado de S72 |
| Tests | Sin núcleo nuevo (UI web); verificado por build + que la ruta sirva; el endpoint subyacente está en el gate |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-09 | Plan del Sprint 73 (reporting #2: UI web del tablero — resumen del relevamiento en el front). 5 SP, sin núcleo nuevo. Generado por AG-07 |
