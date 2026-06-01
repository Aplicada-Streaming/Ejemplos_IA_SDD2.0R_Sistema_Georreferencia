# ADR-14 — Compliance Ley 25.326 (retención de auditoría, tratamiento y minimización de datos personales)

**Proyecto:** GeoVial
**Documento:** ADR-14-compliance-ley-25326_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Seguridad

## 1. Contexto

GeoVial trata datos personales de los agentes y registros del organismo, sujetos a la Ley 25.326 de Protección de Datos Personales de Argentina (PROJECT-BRIEF §10, vision §7, alcance §7; flag `requiere_compliance: true`). Tres reglas de negocio gobiernan el cumplimiento: registro inalterable de accesos y acciones con retención ≥ 12 meses (RN-07), acceso a datos personales acotado por rol y área (RN-08) y autorización jerárquica (RN-01). Los CU afectados son CU-03, CU-08, CU-13 y CU-14, más la transversalidad de la autorización. Se necesita una decisión de arquitectura que gobierne retención, tratamiento y minimización.

## 2. Decisión

Se adopta un esquema de compliance con tres pilares:

- Auditoría inmutable: todo acceso autenticado y toda acción administrativa se asientan en RegistroAuditoría con autor, momento y operación, sin posibilidad de alteración ni borrado durante un período de retención no menor a 12 meses (RN-07).
- Acceso acotado: el acceso a datos personales se restringe por rol y área mediante el módulo transversal de autorización; la exportación/importación respeta ese acotamiento (RN-08, RN-01).
- Minimización y finalidad: solo se tratan los datos personales necesarios para el relevamiento y su evaluación; ningún tratamiento ajeno a esa finalidad se permite.

## 3. Estado

Aceptado el 2026-06-01. ADR de gobernanza de compliance (`requiere_compliance: true`).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Auditoría inmutable + acceso acotado + minimización (elegido) | Cumple RN-07, RN-08, RN-01; demostrable ante auditoría | Costo de almacenamiento de auditoría y de garantizar inmutabilidad |
| Auditoría mutable con backups | Más simple | No garantiza inmutabilidad; insuficiente para demostrar el tratamiento ante la Ley 25.326 |
| Sin retención mínima definida | Menos almacenamiento | Incumple RN-07; impide reconstruir el historial dentro del período legal |

## 5. Consecuencias positivas

1. El organismo puede demostrar quién hizo qué y cuándo dentro del período de retención (RN-07).
2. Los datos personales solo son accesibles según rol y área, reduciendo el riesgo de exposición (RN-08, RA-08).
3. La minimización y la finalidad acotada limitan el tratamiento a lo necesario.

## 6. Consecuencias negativas y trade-offs

1. La auditoría inmutable y la retención ≥ 12 meses consumen almacenamiento creciente; aceptado por obligación legal.
2. Garantizar la inmutabilidad impone controles (rechazo `AUDITORIA_INMUTABLE`) que agregan lógica al módulo de auditoría.

## 7. Implementación

El módulo de auditoría y retención (CU-13) asienta cada evento de forma inalterable; los intentos de alterar o borrar dentro del período se rechazan con `AUDITORIA_INMUTABLE`, y una acción no registrada se rechaza con `ACCION_NO_AUDITADA`. El módulo de autorización (CU-14) acota el acceso a datos personales (`ACCESO_DATO_PERSONAL_NO_AUTORIZADO`, `FINALIDAD_NO_PERMITIDA`). La exportación de relevamientos respeta el acotamiento por área (CU-08). El esquema de auditoría se materializa en `modelo-datos-logico_v1.0.md`.

## 8. Métricas de validación

- Un registro de auditoría no puede modificarse ni eliminarse dentro de los 12 meses (suite de auditoría e inmutabilidad, 08).
- Un acceso a datos personales fuera del rol/área queda bloqueado y registrado (RN-08).
- La exportación de un relevamiento respeta el acotamiento de acceso por rol y área (CA-02/CA-03 de CU-08).

## 9. Referencias

- PROJECT-BRIEF §10; vision §7; alcance §7.
- RN-01, RN-07, RN-08; CU-03, CU-08, CU-13, CU-14.
- ADR-03 (auth), ADR-11 (códigos de error), `modelo-datos-logico_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. ADR de gobernanza de compliance Ley 25.326 |
