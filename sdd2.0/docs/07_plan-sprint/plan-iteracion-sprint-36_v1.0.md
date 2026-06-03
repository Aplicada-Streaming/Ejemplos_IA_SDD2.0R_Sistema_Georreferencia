# Plan de Iteración — Sprint 36

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-36_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-10-19
**Fecha fin:** 2027-10-30
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S33–S35); capacidad sugerida estricta 9 SP. Se compromete la migración del backend de fotos S3 a AWSSDK.S3 v4 (8 SP), el major diferido en S35. Mantenimiento; en el gate.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Migrar el backend de alojamiento de fotos sobre S3 (`AlmacenS3`) de **AWSSDK.S3 v3 a v4** (el PR de Dependabot #40 diferido en S35), ajustando el código a la API v4 si hace falta y verificándolo con el gate. Cierra la deuda de major del SDK de AWS.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-AWS-S3-V4 | Tarea | Migrar `AlmacenS3` y su registro a AWSSDK.S3 v4; verificar con el gate | Media | 8 | Dev backend | Pendiente |

Total de puntos comprometidos: 8 SP. Mantenimiento de dependencia mayor; sin lógica de dominio nueva.

## 4. Alcance técnico

1. **Bump** `AWSSDK.S3` 3.7.511.8 → **4.0.24** en `GeoVial.Infrastructure` (arrastra `AWSSDK.Core` 4.0.7.5).
2. **Revisión de la API en `AlmacenS3`** (`Alojamiento/AlmacenS3.cs`) y su registro (`DependencyInjection.cs`): `PutObjectAsync(PutObjectRequest, ct)`, `GetObjectAsync(bucket, key, ct)`, `GetObjectMetadataAsync(bucket, key, ct)`, `DeleteObjectAsync(bucket, key, ct)`, `AmazonS3Exception.StatusCode`, `new AmazonS3Client(RegionEndpoint.GetBySystemName(region))`. La migración se guía por el compilador (build) y los tests mockeados (`AlmacenS3Tests`).
3. **Verificación**: build Release sin warnings (incluido el control de vulnerabilidades bajo `TreatWarningsAsErrors`) y la suite (332) verde.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica. Criterios específicos:

- `AWSSDK.S3` queda en v4 (4.0.24) y la solución compila en Release sin warnings tratados como error.
- `AlmacenS3` y su registro usan la API v4; los tests mockeados de `AlmacenS3` siguen verdes.
- La suite (332) permanece verde; el gate de cobertura no se ve afectado.
- Limitación declarada: los tests de S3 son contra un cliente mockeado (no hay bucket real en CI); la prueba contra S3 real queda fuera del alcance del gate (igual que antes).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| API v4 con cambios incompatibles en los métodos usados | Media | Medio | La migración la guía el compilador; se ajusta el código a la API v4 hasta build verde |
| v4 introduce warnings de vulnerabilidad bajo `TreatWarningsAsErrors` | Baja | Bajo | Build Release lo detecta; se evaluaría una versión sin la alerta |
| El camino S3 no se ejercita con S3 real en CI | Alta | Bajo | Cubierto por el test mockeado (contrato de la API); la prueba real queda documentada como fuera del gate |

## 7. Criterios de hecho del sprint

El Sprint 36 se considera completo cuando `AWSSDK.S3` está en v4, `AlmacenS3` y su registro compilan y sus tests mockeados pasan, la suite (332) sigue verde sin warnings tratados como error, el PR de Dependabot #40 queda saldado, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | PR de Dependabot #40 (AWSSDK.S3 v4), diferido en S35 |
| Componente | `GeoVial.Infrastructure/Alojamiento/AlmacenS3.cs`, `DependencyInjection.cs` |
| Arquitectura | ADR-08 (alojamiento de fotos con backend configurable) |
| Calidad | definition-of-done §1; retro S35 (migración a AWSSDK.S3 v4) |
| Tests previstos | `AlmacenS3Tests` (mockeado): guardar/recuperar/existe/404 |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 36 (migración a AWSSDK.S3 v4): bump 3.7→4.0.24 y revisión de `AlmacenS3`/registro a la API v4, verificado con el gate. Salda el major diferido en S35. Compromete 8 SP. Generado por AG-07 |
