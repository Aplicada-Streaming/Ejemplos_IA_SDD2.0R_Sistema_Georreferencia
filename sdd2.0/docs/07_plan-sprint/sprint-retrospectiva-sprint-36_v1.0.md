# Sprint Retrospectiva — Sprint 36

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-36_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- La migración del major de AWS SDK fue casi indolora: `AlmacenS3` aísla el cliente `IAmazonS3` (inyectado, responsabilidad única, superficie mínima), así que v4 compiló sin tocar código.
- Dejar que el compilador guiara la migración (bump → build → ver errores) evitó suposiciones sobre la API v4; resultó que no había errores.
- Diferir en S35 y migrar acá con criterio fue el flujo correcto: el major no entró por descuido y se hizo cuando tocaba.
- El gate (build + 332 pruebas) dio confianza para mergear un cambio de dependencia mayor.

## 2. Qué no salió bien

- La verificación es por compilación + tests mockeados; el comportamiento real de v4 contra un bucket S3 no se ejercita (no hay S3 en CI). Si v4 cambió algún comportamiento en runtime, no lo veríamos hasta producción.
- Fue un sprint de 8 SP para un cambio que, por el buen diseño, resultó de bajo esfuerzo real; el valor estuvo más en la verificación y la decisión que en el código.

## 3. Qué probar

- Evaluar una prueba de integración de `AlmacenS3` contra **LocalStack** (S3 emulado) para cubrir el comportamiento real de v4 sin una cuenta AWS, si se quiere cerrar esa brecha.
- Capitalizar el aprendizaje: aislar las dependencias externas detrás de una interfaz pequeña (como `IAlmacenFotos`/`IAmazonS3`) hace baratas las migraciones de major; aplicarlo al resto de integraciones externas.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Evaluar prueba de `AlmacenS3` contra LocalStack (cobertura real de v4) | AG-05 (QA) | 2027-11-13 | Pendiente |
| Auto-merge de Dependabot para minor/patch que pasen CI | AG-09 (DevOps) | 2027-11-13 | Pendiente |
| Mejoras del mapa: offline de teselas en el móvil | AG-08 (móvil) | 2027-11-13 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 35 | Estado actual |
| --- | --- |
| Sprint de migración a AWSSDK.S3 v4 (con prueba contra S3) | Completada la migración (bump + verificación); la prueba contra S3 real queda como opción (LocalStack) |
| Evaluar auto-merge de Dependabot para minor/patch que pasen CI | Pendiente (se reitera) |
| Indicar a Dependabot que ignore el major de FluentAssertions | Completada (comentario `@dependabot ignore` en #42) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 36 (migración AWSSDK.S3 v4): migración indolora por el aislamiento de `AlmacenS3`; 3 acciones nuevas (LocalStack, auto-merge, mapa móvil). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
