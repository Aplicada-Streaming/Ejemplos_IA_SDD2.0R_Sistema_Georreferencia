# Plan de Iteración — Sprint 69

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-69_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2029-02-05
**Fecha fin:** 2029-02-16
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 5,3 SP (S66–S68). **Sprint #2 de la épica "Hardening de producción": observabilidad.** Para operar en producción hay que poder **diagnosticar**: saber qué request causó qué log y leerlos de forma estructurada.

## 2. Objetivo del sprint

**Que cada petición sea rastreable de punta a punta y que los logs sean legibles por una máquina.** Tres piezas de observabilidad:

- **Id de correlación por request:** un identificador que acompaña a la petición; se reusa el que envíe el cliente/proxy (para correlacionar entre saltos) o se genera; va en el **scope de logging** (todos los logs del request lo llevan) y se **devuelve** en la respuesta (cabecera `X-Correlation-ID`).
- **Logs estructurados (JSON) fuera de Development:** para que un agregador (ELK/Loki/CloudWatch) los parsee; en Development se conserva la consola legible.
- **Log de cada request:** método, ruta, estado y duración.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| OBS-CORRELACION | Historia | Id de correlación por request (reuso/generación) + scope de logging + cabecera de respuesta | Alta | 3 | Backend (AG-08) | Cerrada |
| OBS-LOGS | Historia | Logs estructurados (JSON) fuera de Development + log de request (método/ruta/estado/duración) | Media | 2 | Backend (AG-08) | Cerrada |

Total: 5 SP.

## 4. Alcance técnico

- **Núcleo `Correlacion`** (`GeoVial.Application.Observabilidad`, gate): `Resolver(entrante) → string` (reusa el id entrante si es válido —no vacío, ≤ 128, sin caracteres de control— o genera un GUID compacto) + `EsValido`. Lógica pura y testeable.
- **`Program.cs`** (middleware temprano): resuelve el id de correlación de la cabecera `X-Correlation-ID`, lo escribe en la respuesta y abre un `BeginScope` con `CorrelationId` para que todos los logs del request lo lleven; va antes del log HTTP y de la autenticación.
- **`Program.cs`** (logging): fuera de Development, `ClearProviders()` + `AddJsonConsole()` (JSON estructurado); `AddHttpLogging` + `UseHttpLogging` con método/ruta/estado/duración.
- **Sin cambios de dominio ni migración.** Endpoints intactos; las pruebas siguen en InMemory.

## 5. Definition of Done aplicada

- Toda respuesta trae `X-Correlation-ID`; si el cliente lo envía y es válido, se reusa (eco); si no, se genera.
- Fuera de Development los logs salen en JSON; cada request se loguea con su método/ruta/estado/duración.
- Suite del gate verde con el núcleo nuevo cubierto; cobertura DoD sin regresión.
- Verificado en vivo (cabecera de correlación: generación y eco).

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| Un id entrante abusivo (largo / con control chars) se propaga a los logs | Media | Medio | `Correlacion.EsValido` lo descarta y genera uno nuevo; cubierto por tests |
| `AddJsonConsole` duplica logs si no se limpian los proveedores | Baja | Bajo | `ClearProviders()` antes de `AddJsonConsole` fuera de Development |
| El log HTTP es ruidoso | Baja | Bajo | Sólo campos esenciales (método/ruta/estado/duración) |

## 7. Criterios de hecho del sprint

Completo cuando: cada respuesta trae el id de correlación (reuso/generación); los logs salen estructurados fuera de Development y cada request se loguea; el gate queda verde con lo nuevo cubierto; verificado en vivo; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Épica "Hardening de producción", sprint #2 (observabilidad); roadmap de la retro S68 |
| Componentes | `GeoVial.Application` (`Correlacion`), `GeoVial.Api` (`Program`: middleware + logging) |
| Calidad | definition-of-done §1.4 |
| Tests | `CorrelacionTests` (+7) + `ObservabilidadTests` (+2: generación / eco de la cabecera) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Plan del Sprint 69 (hardening #2: observabilidad — id de correlación por request + logs estructurados + log de request). 5 SP. Generado por AG-07 |
