# Sprint Retrospectiva — Sprint 35

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-35_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Dependabot demostró su valor de inmediato: 7 PRs reales (patch/seguridad/tooling) integrados en un sprint, con la config de S33 validada en vivo (NuGet agrupado, Actions, Docker).
- El gate hizo su trabajo: integrar los bumps y correr 332 pruebas + cobertura dio confianza para mergear sin sorpresas.
- La revisión de las mayores evitó un problema real: FluentAssertions 8 trae **licencia comercial**; auto-mergearlo habría metido una dependencia de pago sin que nadie lo decidiera.
- Diferir AWSSDK.S3 v4 (en lugar de forzarlo) mantuvo el sistema estable y dejó la migración como trabajo consciente.

## 2. Qué no salió bien

- Los bumps de Actions de los workflows de publicación no se validan en este PR (sólo en tag); su verdad se sabrá en el próximo release `-rc`. Aceptable, pero es deuda de validación diferida.
- El triage fue manual (leer cada PR, decidir, aplicar en una rama consolidada); con lotes más grandes esto escala mal sin reglas de auto-merge.
- AWSSDK.S3 v4 quedó pendiente; mientras tanto, Dependabot seguirá reabriendo su PR hasta que se haga la migración.

## 3. Qué probar

- Configurar auto-merge de Dependabot para minor/patch que pasen el gate de CI, dejando sólo las mayores para revisión manual (reduce el trabajo de triage de los próximos lotes).
- Agendar un sprint de migración a AWSSDK.S3 v4 con prueba real contra S3 (o el backend de fotos configurado).
- Confirmar en el próximo release `-rc` que los bumps de Actions de los workflows de publicación siguen verdes.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Evaluar auto-merge de Dependabot para minor/patch que pasen CI | AG-09 (DevOps) | 2027-10-30 | Pendiente |
| Sprint de migración a AWSSDK.S3 v4 (con prueba contra S3) | AG-04 (backend) | 2027-10-30 | Pendiente |
| Indicar a Dependabot que ignore el major de FluentAssertions (licencia) | AG-09 (DevOps) | 2027-10-30 | En curso |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 34 | Estado actual |
| --- | --- |
| Unificar la URL/atribución de teselas también en `mapaRevision.js` (web) | Pendiente (se reitera) |
| Evaluar offline de teselas en el móvil | Pendiente (se reitera) |
| Mantenimiento continuo: atender PRs/alertas de Dependabot por SLA | Completada para el primer lote (este sprint); continúa con los próximos |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 35 (triage de Dependabot): lote seguro integrado, FluentAssertions 8 declinado por licencia, AWSSDK.S3 v4 diferido; 3 acciones nuevas. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
