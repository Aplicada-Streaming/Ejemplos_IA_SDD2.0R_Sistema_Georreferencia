# Sprint Review — Sprint 36

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-36_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-36_v1.0.md`:

> Migrar el backend de alojamiento de fotos sobre S3 (`AlmacenS3`) de **AWSSDK.S3 v3 a v4** (el PR de Dependabot #40 diferido en S35), ajustando el código a la API v4 si hace falta y verificándolo con el gate.

Veredicto: Cumplido.

Explicación corta: se subió `AWSSDK.S3` de 3.7.511.8 a **4.0.24** (arrastra `AWSSDK.Core` 4.0.7.5). La migración la guió el compilador: el código de `AlmacenS3` y su registro **ya eran compatibles con la API v4** —`PutObjectAsync(request, ct)`, `GetObjectAsync(bucket, key, ct)`, `GetObjectMetadataAsync`, `DeleteObjectAsync`, `AmazonS3Exception.StatusCode`, `new AmazonS3Client(RegionEndpoint.GetBySystemName(region))`— sin cambios de código. Esto fue posible porque `AlmacenS3` aísla bien el cliente (`IAmazonS3` inyectado, responsabilidad única, superficie mínima de la API), tal como su propio diseño preveía (ADR-08, testeable sin cuenta real). La solución compila en Release sin warnings tratados como error y la suite (332) queda verde. Se salda así el major diferido en S35.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Mantenimiento | `AWSSDK.S3` en v4 (4.0.24); build Release limpio + 332 pruebas verdes | Major del SDK al día sin romper |
| ADR-08 | Calidad | `AlmacenS3` migró sin cambios de código por su aislamiento del cliente S3 | El buen diseño pagó la migración |
| EP-09 | Supply-chain | El PR de Dependabot #40 (diferido en S35) queda saldado | Deuda de dependencia cerrada |

## 3. Feedback recibido

- La migración de un major suele doler; acá fue casi indolora porque `AlmacenS3` usa una superficie chica y estable de la API y recibe el cliente por inyección. El aislamiento de la dependencia (ADR-08) se justificó solo.
- Honestidad sobre el alcance de la verificación: los tests de S3 son contra un cliente **mockeado** (contrato de la API); no hay bucket real en CI, así que el comportamiento real de v4 contra S3 no se ejercita —igual que antes—.
- Quedó claro el valor de diferir en S35 y migrar acá con criterio en vez de auto-mergear el PR.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 332 verdes (295 unitarias + 37 de integración), sin cambio de conteo: es una migración de dependencia sin lógica nueva. Verificación: `AWSSDK.S3` resuelto a 4.0.24 (`AWSSDK.Core` 4.0.7.5), build Release sin warnings tratados como error, y los tests mockeados de `AlmacenS3` (guardar/recuperar/existe/404) verdes.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-AWS-S3-V4 | Tarea | Aceptada (AWSSDK.S3 a v4 sin cambios de código; suite verde; #40 saldado) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 36 se traslada. |

Backlog restante: prueba de `AlmacenS3` contra un S3 real/LocalStack (fuera del gate, si se quiere cubrir el comportamiento de v4 end-to-end); mejoras opcionales del mapa (offline de teselas en el móvil); auto-merge de Dependabot; y el mantenimiento continuo de los próximos lotes.

## 7. Decisiones tomadas durante el review

- Aceptar la migración sin cambios de código, verificada por el compilador + tests mockeados + suite.
- Mantener la prueba contra S3 real fuera del gate (no hay bucket en CI); dejarla como opción documentada (LocalStack) si se quiere cobertura end-to-end de v4.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 36 (migración a AWSSDK.S3 v4). Veredicto Cumplido, velocity 8, 0 carry-over, 332 pruebas verdes; migración sin cambios de código por el aislamiento de `AlmacenS3`. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
