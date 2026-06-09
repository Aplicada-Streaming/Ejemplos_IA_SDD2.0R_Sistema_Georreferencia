# Plan de Iteración — Sprint 72

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-72_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2029-03-19
**Fecha fin:** 2029-03-30
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 6,0 SP (S69–S71). **Primer sprint de la épica "Reporting / analytics"** (elegida por el usuario): dar a los jefes una vista de gestión sobre los datos ya capturados. Este sprint construye el **backend del resumen** (la base del tablero); la UI web va en el siguiente sprint.

## 2. Objetivo del sprint

**Que un jefe pueda obtener el resumen de actividad de un relevamiento:** totales (marcadores, conflictos, observaciones, bandeja, fotos, comentarios) y **productividad por agente** (observaciones por agente), autorizado por área.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| RPT-RESUMEN | Historia | Resumen de actividad del relevamiento (endpoint + agregado + productividad por agente) | Alta | 5 | Backend (AG-08) | Cerrada |

Total: 5 SP (backend del tablero; la UI web es el próximo sprint de la épica).

## 4. Alcance técnico

- **Núcleo `CalculadoraResumenRelevamiento`** (`GeoVial.Application.Reportes`, gate): función pura que, a partir de los marcadores, observaciones y conteos de fotos/comentarios, arma el `ResumenRelevamientoDto` (totales + productividad por agente, ordenada de mayor a menor). Testeable sin base.
- **DTO `ResumenRelevamientoDto` / `ProductividadAgenteDto`** (`GeoVial.Shared`): contrato del reporte (conteos; no trae fotos ni comentarios).
- **Query + handler** `ResumenRelevamientoQuery` / `ResumenRelevamientoHandler` (CQRS ligero): autoriza por área (RN-01, reusa `Autorizacion.PuedeAccederArea`), carga los datos con los repositorios existentes y delega en la calculadora. `null` → 404 (no autorizado / inexistente, sin filtrar).
- **Endpoint** `GET /api/v1/relevamientos/{id}/resumen` (autenticado).
- **Sin cambios de dominio ni migración.** Reusa los repos de marcadores/observaciones/fotos/comentarios.

## 5. Definition of Done aplicada

- Un jefe (o usuario con acceso al área) obtiene el resumen del relevamiento; un usuario sin acceso recibe 404; sin token, 401.
- El resumen cuenta marcadores/conflictos/observaciones/bandeja/fotos/comentarios y la productividad por agente.
- Suite del gate verde con el núcleo nuevo cubierto; cobertura DoD sin regresión.
- Verificado en vivo contra SQL Server real.

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| Cargar fotos/comentarios por marcador es costoso en relevamientos grandes | Media | Bajo | Aceptable para el MVP del reporte; si escala, agregar conteos en la base (repo) en un sprint posterior |
| El resumen expone datos de otra área | Baja | Alto | Autoriza por área (`PuedeAccederArea`); cubierto por test (404 para inaccesible) |

## 7. Criterios de hecho del sprint

Completo cuando: el endpoint devuelve el resumen con totales + productividad; respeta la autorización por área (404/401); el gate queda verde con lo nuevo cubierto; verificado en vivo; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Épica "Reporting / analytics" (elección del usuario), sprint #1 |
| CU/UX | Gestión sobre los datos capturados (CU-08 revisión, extendido a agregado) |
| Componentes | `GeoVial.Application.Reportes` (`CalculadoraResumenRelevamiento`, query/handler), `GeoVial.Shared` (DTO), `GeoVial.Api` (endpoint) |
| Calidad | definition-of-done §1.4; autorización por área (RN-01) |
| Tests | `CalculadoraResumenRelevamientoTests` (+4) + `ResumenRelevamientoE2ETests` (+3: agrega / 401 / 404) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Plan del Sprint 72 (reporting #1: resumen de actividad del relevamiento — backend del tablero). 5 SP. Generado por AG-07 |
