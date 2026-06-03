# Sprint Review — Sprint 35

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-35_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-35_v1.0.md`:

> Consumir el primer lote de Dependabot: integrar las actualizaciones de bajo riesgo verificándolas con el gate, y **decidir con criterio** las mayores —no auto-mergear breaking changes—.

Veredicto: Cumplido.

Explicación corta: Dependabot (configurado en S33) abrió 7 PRs (#37–#43) —lo que de paso **confirmó en vivo** que la configuración funciona (NuGet con agrupación, Actions y Docker)—. Se integraron los de bajo riesgo verificándolos con el gate (332 pruebas + cobertura verdes): el grupo minor/patch (#39), `coverlet.collector` 6→10 (#41) y `Microsoft.NET.Test.Sdk` 17→18 (#43), el grupo de Actions (#38) y la imagen de la db `mssql/server` 2022→2025 (#37). Se **difirió** `AWSSDK.S3` v4 (#40) por ser un salto de major del SDK que necesita migración y prueba real contra S3 (se queda en el último patch de v3 que trae #39). Y se **declinó** `FluentAssertions` v8 (#42) porque la v8 pasó a **licencia comercial**; se queda en la última 7.x libre (7.2.2). Los bumps de `ci.yml` los valida el propio PR (corre en `pull_request`); los de los workflows de publicación se validarán en el próximo release con un tag `-rc` (checklist de release de S33).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Supply-chain | Dependabot abrió 7 PRs (NuGet agrupado, Actions, Docker): la config de S33 funciona | Vigilancia de dependencias confirmada |
| EP-09 | Mantenimiento | Lote seguro integrado con el gate verde (332 pruebas + cobertura) | Dependencias al día sin romper |
| EP-09 | Criterio | FluentAssertions 8 declinado por licencia comercial; AWSSDK.S3 v4 diferido | Decisiones de major con fundamento |

## 3. Feedback recibido

- El valor de Dependabot quedó claro: 7 actualizaciones reales esperando, varias de seguridad/patch, integradas en un solo sprint.
- La decisión sobre las mayores fue el punto clave: auto-mergear FluentAssertions 8 habría metido una dependencia de **licencia de pago**; revisarlo lo evitó.
- Diferir AWSSDK.S3 v4 (en vez de forzarlo) es lo correcto: un salto de major de un SDK con backend real merece su propio sprint y prueba contra S3.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 332 verdes (295 unitarias + 37 de integración), sin cambio de conteo: es mantenimiento de dependencias sin lógica nueva. Cobertura del gate (run unitario): Domain líneas 89,7 % / branches 79,8 %; Application 90,0 % / 82,0 % —se mantiene con `coverlet` 10 y `Test.Sdk` 18—. Build Release sin warnings tratados como error con las nuevas versiones.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-DEPS-TRIAGE | Tarea | Aceptada (lote seguro integrado y verificado; AWSSDK.S3 v4 diferido, FluentAssertions v8 declinado por licencia) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 35 se traslada. |

Backlog: migración a AWSSDK.S3 v4 (sprint dedicado con prueba contra S3); mejoras opcionales del mapa (offline de teselas en el móvil); y el mantenimiento continuo de los próximos lotes de Dependabot.

## 7. Decisiones tomadas durante el review

- Integrar el grupo minor/patch y el tooling de tests verificándolos con el gate; tomar los bumps de Actions y de la imagen db.
- **Declinar** FluentAssertions 8 (licencia comercial) y fijar en 7.2.2; **diferir** AWSSDK.S3 v4 a un sprint de migración.
- Cerrar los PRs de Dependabot consumidos (auto-cerrados al integrar las mismas versiones) e indicar a Dependabot que ignore el major de FluentAssertions.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 35 (triage del primer lote de Dependabot). Veredicto Cumplido, velocity 8, 0 carry-over, 332 pruebas verdes. FluentAssertions 8 declinado (licencia), AWSSDK.S3 v4 diferido. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
