# Sprint Review — Sprint 64

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-64_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-64_v1.0.md`:

> Dejar el entorno de desarrollo sobre una base de datos real persistente con ejemplos precargados, y corregir el espacio en blanco de la pantalla de Captura.

Veredicto: Cumplido.

Explicación corta: el backend de desarrollo pasó de **InMemory** a **SQL Server real** (host `DEV`, autenticación de Windows; ADR-09): aplica migraciones al arrancar, crea la base `GeoVial` y siembra el set de DATOS-DE-PRUEBA.md con un seed **idempotente** (`SeedDesarrollo`). Los datos **persisten** entre reinicios y re-arrancar no duplica. Para no romper el gate —que corría en `Development` y habría tomado la nueva cadena—, se introdujo `FabricaPruebas`, que fuerza **InMemory** en las pruebas de integración. Además se corrigió el **mapa en blanco** de la pantalla de Captura (regresión de S62): el WebView de Leaflet necesitaba una altura de contenedor definida; la fila pasó de `Auto` a fija (180), con lo que el mapa renderiza y la captura queda más compacta.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| INFRA-DB-REAL | Infra | Backend sobre SQL Server `DEV`; `campo1` → 2 relevamientos; conteos exactos (6 usuarios, 6 credenciales, 3 relevamientos, 4 asignaciones) | Datos persistentes y limpios |
| INFRA-DB-REAL | Calidad | Gate corre en memoria pese a la cadena de SQL en Development (489 verdes) | Sin dependencia de SQL en CI |
| FIX-MAPA-CAPTURA | Bug | El mapa de Captura renderiza (Argentina con zoom/atribución), compacto; "Tomar foto" a la vista | Desaparece el blanco; verificado on-device |

## 3. Feedback recibido

- La base persistente elimina la fricción de re-sembrar tras cada reinicio (antes, InMemory perdía todo y había que correr `seed-datos-prueba.py`).
- El seed idempotente en código reemplaza al script externo y es seguro de re-ejecutar.
- El blanco de Captura era una regresión visible de S62; el fix lo resuelve sin tocar el flujo de captura.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **489** (445 unitarias + 44 de integración), **sin nuevas** de dominio (el cambio es de infraestructura/configuración + un fix de XAML). El gate se re-verificó verde **corriendo en memoria** vía `FabricaPruebas`; cobertura DoD sin regresión (los archivos nuevos —`SeedDesarrollo`, `FabricaPruebas`— son Infrastructure/test, fuera del gate Domain/Application). El MAUI compila (`net10.0-android`, arm64) y se redeployó al moto g42.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| INFRA-DB-REAL | Tarea | Aceptada (SQL Server real + seed idempotente + gate aislado) |
| FIX-MAPA-CAPTURA | Bug | Aceptada (mapa renderiza, sin blanco) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

## 7. Decisiones tomadas

- La cadena de conexión va en `appsettings.Development.json` (host `DEV`, Windows auth), consistente con `GeoVialDbContextFactory` y README §16; las pruebas se aíslan con `FabricaPruebas` (InMemory) para no depender de SQL Server.
- El seed de desarrollo se hace en código (`SeedDesarrollo`), idempotente, en lugar del script externo `seed-datos-prueba.py` (que duplicaba sobre base persistente).
- Sobre InMemory (tests) se conserva el `SeedDemo` mínimo previo para no alterar las aserciones del gate.
- La base local tenía datos basura de sesiones previas; con confirmación del usuario se **recreó limpia** (data demo no productiva).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Review del Sprint 64 (base de datos real SQL Server en DEV + seed idempotente + gate aislado en memoria; fix del mapa en blanco de Captura). Cumplido, velocity 5, 0 carry-over, 489 pruebas (sin nuevas). Generado por AG-07 |
