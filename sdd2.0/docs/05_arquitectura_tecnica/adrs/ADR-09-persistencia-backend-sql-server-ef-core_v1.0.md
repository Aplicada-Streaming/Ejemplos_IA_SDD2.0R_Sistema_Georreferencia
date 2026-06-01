# ADR-09 — Persistencia backend con SQL Server y EF Core

**Proyecto:** GeoVial
**Documento:** ADR-09-persistencia-backend-sql-server-ef-core_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Persistencia

## 1. Contexto

El backend necesita un almacenamiento central transaccional con integridad referencial estricta entre relevamiento, observación, marcador, foto, comentario, etiqueta, usuario y área (modelo conceptual de 02, 12 entidades). El dominio impone restricciones declarativas: valores permitidos de rol y de estado (RC-05, RC-06), integridad observación→marcador del mismo relevamiento (RC-02), integridad de la asignación al área (RC-07) y unicidad para idempotencia (RC-03). Las transiciones de estado y la solo lectura tras el cierre requieren atomicidad (RN-05). El cliente fijó SQL Server con acceso vía EF Core (PROJECT-README §2, §7). NFR: latencia p95 ≤ 500 ms.

## 2. Decisión

Se adopta SQL Server como base de datos central del backend, accedida vía EF Core. Las restricciones de integridad (PK, FK, únicas y check de enums de dominio) se declaran a nivel de motor. El esquema se versiona con migraciones de EF Core, comenzando por una migración inicial identificada. La capa Infrastructure implementa los repositorios y el unit of work por request.

## 3. Estado

Aceptado el 2026-06-01. ADR de ingeniería para completar la categoría obligatoria de persistencia (§2.2 web-monolith); no proviene de PROJECT-README §15 pero formaliza su §7.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| SQL Server + EF Core (elegido) | Transacciones ACID, integridad referencial y check declarativos, migraciones versionadas; fijado por el cliente | Costo de gestión del esquema y de migraciones |
| Base documental | Esquema flexible, escala horizontal | No garantiza integridad referencial ni las restricciones de RC; complica las transiciones de estado |
| Almacenamiento clave-valor | Latencia muy baja | Requiere reimplementar consistencia transaccional en la aplicación |

## 5. Consecuencias positivas

1. Las restricciones de dominio (RC-02, RC-05, RC-06, RC-07) se materializan como FK, check y únicas a nivel de motor.
2. La unicidad de idempotencia (RC-03) se garantiza con restricción única.
3. Las transiciones de estado y la solo lectura tras el cierre (RN-05) obtienen atomicidad transaccional.
4. Las migraciones de EF Core permiten reconstruir y auditar el esquema.

## 6. Consecuencias negativas y trade-offs

1. Requiere un plan de migraciones versionado desde el inicio; aceptado.
2. Acopla el backend al modelo relacional; un cambio de motor implicaría reescribir la capa Infrastructure (aislada por Clean Architecture, ADR-10).

## 7. Implementación

`GeoVial.Infrastructure` define el `DbContext` de EF Core sobre SQL Server, los mapeos de las 12 entidades y la migración inicial `InitialCreate`. El detalle físico (tipos, índices, restricciones, migración) vive en `modelo-datos-logico_v1.0.md`. En desarrollo se usa SQL Server local; en pruebas de integración, Testcontainers (PROJECT-README §9). multi_tenant=false: no hay partición por tenant.

## 8. Métricas de validación

- Latencia p95 de lecturas administrativas ≤ 500 ms (prueba de integración de carga).
- Las restricciones de check rechazan valores de rol/estado fuera del enum (suite de modelo).
- La migración inicial reconstruye el esquema completo desde cero en CI.

## 9. Referencias

- PROJECT-README §2, §7, §9.
- RC-02, RC-03, RC-05, RC-06, RC-07; RN-05.
- ADR-10 (capas), `modelo-datos-logico_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. ADR de ingeniería que completa la categoría de persistencia |
