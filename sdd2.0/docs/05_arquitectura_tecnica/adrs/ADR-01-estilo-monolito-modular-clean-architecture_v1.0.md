# ADR-01 — Estilo arquitectónico: monolito modular con Clean Architecture y CQRS ligero

**Proyecto:** GeoVial
**Documento:** ADR-01-estilo-monolito-modular-clean-architecture_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Estilo

## 1. Contexto

GeoVial es un sistema interno del organismo de control de infraestructura vial, con un único bounded context (relevamiento georreferenciado de puentes y caminos) y un equipo de ≈4 personas (PROJECT-README §3). El backend concentra administración, revisión y exportación, y expone una API REST consumida por el front web y la app móvil. El dominio de relevamientos/observaciones/marcadores es rico y con asimetría entre escritura (captura, consolidación, transiciones de estado) y lectura (revisión sobre mapa, carrusel). Se requiere testear el dominio sin infraestructura desde la fase 2 (PROJECT-README §9). Motivan esta decisión los CU-01 a CU-14, las RN-01 a RN-08 y los NFR de latencia (p95 ≤ 500 ms) y disponibilidad (SLO 99%).

## 2. Decisión

Se adopta un monolito modular con Clean Architecture (capas Domain, Application, Infrastructure y Web/API) y módulos por contexto (usuarios y jerarquía, relevamientos, observaciones y marcadores, sincronización, alojamiento de archivos). Se aplica CQRS ligero —separación de commands y queries vía mediador in-process— solo en el módulo de relevamientos/observaciones/marcadores; el resto usa servicios de aplicación directos.

## 3. Estado

Aceptado el 2026-06-01. Renumeración de `ADR-001` (PROJECT-README §15, estado original Propuesto) a `ADR-01`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Monolito modular con Clean Architecture + CQRS ligero (elegido) | Dominio testeable sin infraestructura; un solo deploy; encaja en equipo de 4; CQRS solo donde hay asimetría real | Disciplina de capas requerida; el mediador agrega indirección en el módulo rico |
| Microservicios | Deploy independiente; aislamiento por contexto | Sobredimensionado para 1 contexto y 4 devs; gateway, mesh y sagas elevan la complejidad operativa; el cliente pide backend monolítico y tres contenedores |
| Monolito en capas tradicional acoplado al ORM | Más simple de arrancar | Impide testear el dominio sin base de datos; incumple la cobertura mínima de dominio de la fase 2 |

## 5. Consecuencias positivas

1. El dominio (`GeoVial.Domain`) se prueba sin infraestructura, habilitando la cobertura de la fase 2.
2. Un único artefacto de backend a desplegar, coherente con la topología de tres contenedores pedida por el cliente.
3. El CQRS ligero acota la complejidad: solo el módulo de mayor asimetría lectura/escritura la paga.
4. La modularización por contexto facilita el vertical slicing del delivery (PROJECT-README §4).

## 6. Consecuencias negativas y trade-offs

1. La separación de capas exige disciplina y revisión para no filtrar dependencias hacia adentro (mitigado por ADR-10).
2. El mediador in-process agrega indirección en el módulo de relevamientos; trade-off aceptado por la claridad de la separación command/query.
3. Un monolito limita el escalado independiente por contexto; aceptado por carga interna y SLO 99% no crítico.

## 7. Implementación

Solución `.NET` con proyectos `GeoVial.Domain`, `GeoVial.Application`, `GeoVial.Infrastructure`, `GeoVial.Api`, `GeoVial.Web` (PROJECT-README §5). El mediador in-process despacha commands y queries en el módulo de relevamientos/observaciones. La regla de dependencias se materializa en ADR-10. Convención: ningún proyecto de capa externa es referenciado por Domain.

## 8. Métricas de validación

- Cobertura de tests del dominio sin infraestructura ≥ 80% líneas / 70% branches (gate de CI, PROJECT-README §9).
- Latencia p95 de lecturas administrativas ≤ 500 ms (NFR §8 de arquitectura).
- Cero referencias de `GeoVial.Domain` hacia capas externas (verificable por análisis de dependencias).

## 9. Referencias

- PROJECT-README §3 (estilo), §4 (delivery), §9 (testing).
- CU-01 a CU-14; RN-01 a RN-08.
- ADR-10 (separación de capas), ADR-02 (API REST), ADR-09 (persistencia).
- `arquitectura-solucion_v1.0.md` §2, §3, §4.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Decisión aceptada. Renumeración de ADR-001 a ADR-01 |
