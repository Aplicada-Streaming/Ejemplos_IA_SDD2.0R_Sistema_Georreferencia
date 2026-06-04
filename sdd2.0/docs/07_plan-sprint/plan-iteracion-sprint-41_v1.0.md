# Plan de Iteración — Sprint 41

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-41_v1.0.md
**Versión:** 1.1
**Estado:** Cerrado
**Fecha inicio:** 2027-12-28
**Fecha fin:** 2028-01-09
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S38–S40); capacidad sugerida estricta 9 SP. Se compromete el **arreglo del bug crítico de asignación de agentes** detectado en la revisión funcional (8 SP). Bug de backend/persistencia con prueba de regresión contra un proveedor relacional; entra al gate.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Arreglar el **defecto bloqueante** hallado en la revisión funcional (`revision-funcional-app_v1.0.md` §2): un **Jefe de Área no puede asignar agentes a un relevamiento** contra la base real (`503 ACCION_NO_AUDITADA`), lo que impide que cualquier agente capture (sin asignación → `403`). Es el corazón del producto (ciclo de campo) y hoy está roto en producción.

Causa raíz (confirmada con el log SQL de EF): al asignar, el agregado `Relevamiento` agrega una `AsignacionAgente` **nueva por su colección de dominio**; EF la persiste como **`UPDATE … OUTPUT 1`** (estado *Modified*) en vez de `INSERT` (*Added*). El `UPDATE` afecta 0 filas → `DbUpdateConcurrencyException` → el gate de auditoría lo traga (`catch DbUpdateException → false`) → `ACCION_NO_AUDITADA`. **La CI no lo detecta** porque corre con EF **InMemory** (no aplica el chequeo de "se esperaba 1 fila"); contra un proveedor **relacional** (SqlServer real, o SQLite en la prueba) sí falla.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-FIX-ASIGNACION | Bug | La asignación de agentes persiste la `AsignacionAgente` como `INSERT` (no `UPDATE`); el Jefe de Área puede asignar y el agente capturar | Alta | 5 | Dev backend (AG-06) | En curso |
| BT-TEST-RELACIONAL | Tarea | Prueba de integración contra un proveedor **relacional** (SQLite) que reproduce el bug y lo cubre como regresión (cierra el agujero del gate InMemory) | Alta | 3 | QA (AG-05) | En curso |

Total de puntos comprometidos: 8 SP. Backend/persistencia; la lógica de dominio no cambia, sólo la persistencia/alta de la entidad.

## 4. Alcance técnico

1. **Reproducción (BT-TEST-RELACIONAL):** prueba de integración nueva (`AsignacionAgenteRelacionalTests`) que arma el contenedor DI real (`AddApplication` + `AddInfrastructure`) pero con `GeoVialDbContext` sobre **SQLite** (`UseSqlite`, conexión `:memory:` abierta, `EnsureCreated`). Siembra un área, un Jefe de Área, un Agente de Campo y un relevamiento; ejecuta `AsignarAgentesCommand` por el mediador y verifica que **resulta éxito** y que la `AsignacionAgente` **queda persistida**, y que luego el agente **puede capturar**. Con el código actual la prueba **falla** (reproduce el `ACCION_NO_AUDITADA`).
2. **Fix (BT-FIX-ASIGNACION) — implementado:** la causa raíz era que `AsignacionAgente` **pre-asignaba su PK** (`AsignacionAgenteId = Guid.NewGuid()`) en el constructor. La clave es `ValueGeneratedOnAdd`; con la clave ya seteada, EF aplicaba la heurística "clave seteada → entidad existente → `Modified` → `UPDATE`" al detectar la asignación nueva por la colección del agregado, y el `UPDATE` de 0 filas lanzaba. **El fix quita el pre-seteo de la PK** (una línea en el constructor del dominio): la entidad entra con clave vacía → `IsKeySet=false` → EF la detecta `Added` y genera la Guid al insertar. Sin cambios de API del dominio (no rompe los 13 llamadores de `AsignarAgente`). Las asignaciones reactivadas (baja lógica → `Vigente=true`) siguen siendo `UPDATE`, cubierto por test.
3. **Gate reforzado:** la prueba relacional queda en la suite para que esta clase de bug (INSERT vs UPDATE, sólo visible en proveedor relacional) no vuelva a pasar el gate InMemory.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica. Criterios específicos:

- El Jefe de Área asigna agentes con éxito contra un proveedor relacional; el agente asignado puede capturar.
- La prueba de integración relacional reproduce el bug (falla antes del fix) y pasa después; queda como regresión en la suite.
- La suite .NET completa sigue verde; el gate de cobertura no se ve afectado (no hay lógica de dominio nueva).
- Verificación adicional contra la base SqlServer DEV real (manual) de que la asignación ahora persiste.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El fix arregla el INSERT pero rompe la reactivación (baja→alta) | Media | Medio | La prueba cubre ambos caminos (alta nueva y reactivación); ambos verdes |
| SQLite no reproduzca igual que SqlServer | Baja | Medio | El chequeo "se esperaba 1 fila" lo hace EF Core para todo proveedor relacional; se valida además contra SqlServer DEV |
| Regresión en otros handlers que persisten hijos por agregado | Baja | Medio | Es el único caso de alta por colección de agregado; los demás usan `AddAsync` explícito (revisado en la revisión funcional) |

## 7. Criterios de hecho del sprint

El Sprint 41 se considera completo cuando: la asignación de agentes persiste como inserción y funciona contra un proveedor relacional y contra SqlServer DEV, existe una prueba de integración relacional que cubre el bug como regresión, la suite .NET completa sigue verde, y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | `revision-funcional-app_v1.0.md` §2 (bug crítico de asignación de agentes) |
| Caso de uso | CU-01 (asignar agentes a relevamiento); RN-07 (gate de auditoría) |
| Componentes | `GeoVial.Application/Relevamientos/Manejadores.cs` (`AsignarAgentesHandler`), `GeoVial.Infrastructure/Persistencia/Repositorios.cs` (`RelevamientoRepository`), `GeoVialDbContext` |
| Calidad | definition-of-done §1.4; cierra el agujero del gate InMemory |
| Tests previstos | `AsignacionAgenteRelacionalTests` (alta + reactivación + captura, sobre SQLite) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 41 (fix del bug crítico de asignación de agentes hallado en la revisión funcional: INSERT vs UPDATE de `AsignacionAgente`, invisible para el gate InMemory). Compromete 8 SP con prueba de regresión contra proveedor relacional. Generado por AG-07 |
| 1.1 | 2026-06-03 | Cierre del Sprint 41. Causa raíz confirmada (PK pre-asignada + `ValueGeneratedOnAdd` → `Modified`); fix de una línea en `AsignacionAgente` (no pre-asignar la PK). Prueba de regresión `AsignacionAgenteRelacionalTests` (alta + reactivación) sobre SQLite: falla sin el fix (`ACCION_NO_AUDITADA`), pasa con él. Suite .NET 340 verde. Verificado además contra SqlServer DEV real: asignar agente `204` (antes `503`) y el agente captura `201` (antes `403`). Estado: Cerrado |
