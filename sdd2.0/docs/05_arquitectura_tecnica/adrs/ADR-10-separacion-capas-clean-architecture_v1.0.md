# ADR-10 — Separación de capas Clean Architecture (Domain/Application/Infrastructure/Web-API)

**Proyecto:** GeoVial
**Documento:** ADR-10-separacion-capas-clean-architecture_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Estilo

## 1. Contexto

ADR-01 fija el estilo monolito modular con Clean Architecture. Esta ADR formaliza la categoría obligatoria de separación de capas (§2.2 web-monolith): cómo se reparten las responsabilidades y cómo se controla la dirección de las dependencias para poder testear el dominio sin infraestructura (PROJECT-README §9) y para aislar el motor de persistencia (ADR-09) y los proveedores externos (mapas, almacenamiento, sincronización). Motivan: todos los CU y RN, en particular la testabilidad del dominio exigida desde la fase 2.

## 2. Decisión

Se adoptan cuatro capas con dependencias unidireccionales hacia adentro:

- Domain: entidades, invariantes y reglas de dominio (RC-01 a RC-07). Sin dependencias externas.
- Application: casos de uso, validaciones, orquestación, puertos (interfaces) y el mediador de CQRS ligero. Depende solo de Domain.
- Infrastructure: implementaciones de los puertos (EF Core/SQL Server, FileHosting, integración con Sync). Depende de Application y Domain.
- Web/API: `GeoVial.Api` y `GeoVial.Web`, adaptadores de entrada. Dependen de Application.

Domain no referencia ninguna capa externa. La inversión de dependencias se aplica con puertos en Application implementados por Infrastructure.

## 3. Estado

Aceptado el 2026-06-01. ADR de ingeniería para completar la categoría obligatoria de separación de capas (§2.2). Complementa ADR-01.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Cuatro capas Clean Architecture con puertos (elegido) | Domain testeable sin infraestructura; motor y proveedores aislados; dependencias unidireccionales | Más proyectos y disciplina de referencias |
| Tres capas clásicas (presentación/negocio/datos) acopladas | Menos proyectos | El negocio depende del acceso a datos; impide testear el dominio aislado |
| Vertical slices sin capa de dominio compartida | Cohesión por feature | Duplica invariantes de dominio entre slices; debilita RC transversales |

## 5. Consecuencias positivas

1. El dominio se prueba sin base de datos ni red, habilitando la cobertura de la fase 2.
2. El motor de persistencia (ADR-09) y los proveedores (mapas, FileHosting, Sync) se cambian sin tocar el dominio.
3. Los puertos explícitos hacen visibles las dependencias externas de cada caso de uso.

## 6. Consecuencias negativas y trade-offs

1. Más proyectos y mapeos entre capas; aceptado por la testabilidad y el aislamiento.
2. La disciplina de no filtrar dependencias requiere revisión continua (mitigado por análisis de dependencias en CI).

## 7. Implementación

Proyectos `GeoVial.Domain`, `GeoVial.Application`, `GeoVial.Infrastructure`, `GeoVial.Api`, `GeoVial.Web`, `GeoVial.Shared` (PROJECT-README §5). Los puertos (repositorios, FileHosting, cliente de sync) se declaran en Application y se implementan en Infrastructure. El CQRS ligero del módulo de relevamientos vive en Application.

## 8. Métricas de validación

- Cero referencias salientes de `GeoVial.Domain` (análisis de dependencias en CI).
- Cobertura del dominio ≥ 80% líneas / 70% branches sin infraestructura (gate de CI).
- Un cambio de motor de persistencia no modifica `GeoVial.Domain` ni `GeoVial.Application`.

## 9. Referencias

- PROJECT-README §3, §5, §9.
- RC-01 a RC-07; ADR-01, ADR-09.
- `arquitectura-solucion_v1.0.md` §2, §3.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. ADR de ingeniería que completa la categoría de separación de capas |
