# Sprint Retrospectiva — Sprint 41

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-41_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se desbloqueó el **ciclo de campo** (el corazón del producto): el Jefe de Área vuelve a asignar agentes y el agente vuelve a capturar, verificado contra la base **SqlServer DEV real**.
- Se reprodujo el bug con una prueba **antes** de arreglarlo (test rojo → fix → verde), y se confirmó que la prueba es válida revirtiendo el fix (vuelve a fallar con `ACCION_NO_AUDITADA`).
- El fix fue de **causa raíz** y mínimo (no pre-asignar la PK `ValueGeneratedOnAdd`), sin tocar la API del dominio ni a sus 13 llamadores.
- La nueva prueba relacional (SQLite) cierra el agujero del gate **InMemory** para esta clase de defecto, sin imponer SqlServer en cada máquina (mismo espíritu que el no-op de LocalStack de S39).

## 2. Qué no salió bien

- El bug vivía **desde el origen** del módulo de relevamientos y nunca se detectó: toda la suite corre con EF **InMemory**, que no aplica el chequeo de "se esperaba 1 fila afectada"; el defecto sólo aparece con un proveedor relacional. Es el mismo patrón que los bugs móviles de S40 (invisibles sin ejecución real).
- El primer intento de prueba quedó mal armado (dos proveedores EF registrados a la vez); recién se vio el bug real tras corregir el override del `DbContext` (quitar también la `IDbContextOptionsConfiguration` de EF Core 10). Reproducir con fidelidad llevó más que arreglar.
- El defecto se encontró **en producción de hecho** (la app en el dispositivo no podía completar el flujo), no en CI; lo atrapó la revisión funcional con pruebas en vivo, no el gate.

## 3. Qué probar

- Barrer el resto de los handlers que persisten **hijos de agregados** por colección (no por `AddAsync` explícito) y cubrirlos con pruebas relacionales si los hubiera; en esta revisión `AsignacionAgente` era el único, pero conviene confirmarlo ante nuevas entidades.
- Evaluar correr **una pasada de la suite de integración contra un proveedor relacional** en CI (SQLite, o SqlServer/LocalDB como service container) para no depender sólo de InMemory.
- Revisar que ninguna otra entidad de agregado pre-asigne una PK `ValueGeneratedOnAdd` (misma trampa).

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Evaluar una pasada de integración contra proveedor relacional (SQLite/LocalDB) en `ci.yml` | AG-09 (DevOps) | 2028-01-22 | Pendiente |
| Auditar entidades de agregados que pre-asignen PK `ValueGeneratedOnAdd` (misma trampa que `AsignacionAgente`) | AG-06 (backend) | 2028-01-22 | Pendiente |
| Continuar el backlog de la revisión funcional: captura offline real (S42) | AG-08 (móvil) | 2028-01-22 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 40 | Estado actual |
| --- | --- |
| Verificación on-device obligatoria en el DoD de la app móvil | Pendiente (se reitera) |
| Nunca encadenar branch-delete con un merge no confirmado | Vigente (se respetó en S41) |
| Endurecer el interceptor de teselas (no cachear respuestas de bloqueo) | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 41 (fix del bug crítico de asignación de agentes): el gate InMemory no veía un defecto de persistencia que sí aparece con un proveedor relacional; se reprodujo con prueba (rojo→verde) y se cerró el flanco con una prueba SQLite. 3 acciones nuevas (integración relacional en CI, auditoría de PKs pre-asignadas, captura offline). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
