# Plan de Iteración — Sprint 64

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-64_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-11-13
**Fecha fin:** 2028-11-24
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 5,0 SP (S61–S63). Sprint **operativo + fix** salido de un pedido del usuario: cablear el backend de desarrollo a **SQL Server real** con datos precargados, y corregir el **mapa en blanco** de la pantalla de Captura (regresión de S62).

## 2. Objetivo del sprint

**Dejar el entorno de desarrollo sobre una base de datos real persistente con ejemplos precargados, y corregir el espacio en blanco de la pantalla de Captura.**

- **Base real (ADR-09):** el backend de desarrollo deja de usar InMemory y se conecta a **SQL Server local** (host `DEV`, autenticación integrada de Windows). Aplica migraciones al arrancar (ya lo hacía `SeedInicial` si el proveedor es relacional) y siembra el set de prueba.
- **Datos precargados idempotentes:** seed de desarrollo (jerarquía de DATOS-DE-PRUEBA.md: jefe general, jefa de área y tres agentes + tres relevamientos con asignaciones), **seguro de re-ejecutar** en cada arranque sobre la base persistente.
- **Gate aislado de la base real:** las pruebas de integración deben seguir corriendo en memoria, sin SQL Server, pese a la nueva cadena de conexión de `appsettings.Development.json`.
- **Fix del mapa de Captura (H-01/H-03, regresión S62):** el WebView del mapa quedaba **en blanco** ocupando ~⅓ de la pantalla porque el HTML de Leaflet (`height:100%`) necesita un contenedor con altura definida y estaba en una fila `Auto`.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| INFRA-DB-REAL | Tarea | Cablear SQL Server real (host DEV, Windows auth) + seed de desarrollo idempotente + aislamiento del gate (InMemory) | Alta | 3 | Backend (AG-08) | Cerrada |
| FIX-MAPA-CAPTURA | Bug | Mapa en blanco en Captura: altura fija de la fila del WebView | Media | 2 | Dev móvil (AG-08) | Cerrada |

Total: 5 SP.

## 4. Alcance técnico

- **Conexión (`appsettings.Development.json`):** `ConnectionStrings:GeoVial = Server=DEV;Database=GeoVial;Trusted_Connection=True;TrustServerCertificate=True`. `AddInfrastructure` (sin cambios) ya elige SQL Server cuando la cadena está presente (ADR-09); el `GeoVialDbContextFactory` ya estandarizaba el host `DEV` (README §16).
- **Seed (`SeedDesarrollo.cs`, nuevo, Infrastructure):** crea jerarquía + relevamientos + asignaciones de DATOS-DE-PRUEBA.md. **Idempotente**: usuarios por nombre de credencial, relevamientos por identificación de obra. `Program.cs` lo invoca en Development **sobre proveedor relacional**; sobre InMemory (tests) mantiene el `SeedDemo` mínimo previo para no alterar las aserciones del gate.
- **Aislamiento del gate (`FabricaPruebas.cs`, nuevo, tests):** `WebApplicationFactory<Program>` que corre en `Development` pero **fuerza InMemory** (quita el registro de SQL Server y vuelve a registrar el DbContext en memoria, aislado por fábrica). Las 5 clases E2E pasan a `IClassFixture<FabricaPruebas>`.
- **Fix del mapa (`CapturaPage.xaml`):** la fila del WebView pasa de `Auto` a **altura fija** (`RowDefinitions="Auto,180,*"`), dándole al contenedor una altura definida para que Leaflet renderice; de paso queda más compacto y deja "Tomar foto" (H-11) a la vista.
- **Sin cambios de dominio/Application.** Sin migración nueva (el esquema ya existía; sólo cambia el proveedor de InMemory a SQL Server).

## 5. Definition of Done aplicada

- El backend de desarrollo levanta contra SQL Server `DEV` (Windows auth), crea la base y el esquema y siembra el set de prueba; los datos **persisten** entre reinicios.
- Re-arrancar el backend no duplica datos (seed idempotente).
- La suite del gate corre **en memoria** (sin SQL Server), determinista; **489 pruebas verdes** (445 unitarias + 44 integración), cobertura DoD sin regresión (los archivos nuevos son Infrastructure/test, fuera del gate Domain/Application).
- La pantalla de Captura muestra el mapa (no en blanco) y la acción primaria a la vista; verificado on-device.

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| La cadena de SQL en Development rompe el gate (los E2E hacían `UseEnvironment("Development")`) | Alta | Alto | `FabricaPruebas` fuerza InMemory en pruebas; gate re-verificado verde |
| El seed duplica datos al reiniciar sobre base persistente | Media | Medio | Idempotente por nombre de credencial / identificación de obra |
| Datos basura previos en la base local | Media | Bajo | Recreación limpia de la base (con confirmación del usuario); data demo no productiva |
| Permisos de Windows para crear la base | Baja | Medio | El login `DEV\fernando` tiene permiso; `MigrateAsync` crea base + esquema al arrancar |

## 7. Criterios de hecho del sprint

Completo cuando: el backend de desarrollo corre sobre SQL Server `DEV` con datos precargados persistentes e idempotentes; el gate corre en memoria y queda verde; el mapa de Captura renderiza sin espacio en blanco; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Pedido del usuario (base real + ejemplos precargados; ajustar el blanco de Captura) |
| ADR | ADR-09 (SQL Server vía EF Core); README §16 (desarrollo local en host DEV) |
| CU/UX | DATOS-DE-PRUEBA.md (set de prueba); auditoría UX H-01/H-03 (mapa en Captura, S62) |
| Componentes | `GeoVial.Api` (config/seed), `GeoVial.Infrastructure` (`SeedDesarrollo`), `GeoVial.IntegrationTests` (`FabricaPruebas`), `GeoVial.Mobile` (`CapturaPage`) |
| Calidad | definition-of-done §1.4; gate en memoria |
| Tests | Sin núcleo de dominio nuevo; aislamiento del gate verificado (489 verdes); fix de UI verificado on-device |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Plan del Sprint 64 (base de datos real SQL Server en DEV + seed de desarrollo idempotente + aislamiento del gate en memoria; fix del mapa en blanco de Captura). 5 SP. Generado por AG-07 |
