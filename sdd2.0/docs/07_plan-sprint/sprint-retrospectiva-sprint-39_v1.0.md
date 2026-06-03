# Sprint Retrospectiva — Sprint 39

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-39_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se cerró la única brecha de verificación que quedaba de la migración a AWS SDK v4 (S36): ahora `AlmacenS3` se ejercita contra un S3 real (LocalStack) en CI, no sólo con mocks.
- El service container de LocalStack evitó agregar una dependencia NuGet (Testcontainers) y su riesgo de warnings de vulnerabilidad; la prueba quedó simple.
- El patrón "no-op si no hay LocalStack, corre en CI" deja la prueba sin imponer Docker en cada máquina de desarrollo (acá no hay Docker local).
- Reusó `AlmacenS3` tal cual (sin tocarlo): la prueba confirma que la abstracción de S36 funciona contra el servicio real.

## 2. Qué no salió bien

- No se puede verificar localmente (sin Docker): la confianza viene de la CI del PR, no de una corrida local. Es el mismo límite de los sprints de workflow.
- LocalStack agrega tiempo de arranque al job de CI; mitigado con reintentos en la prueba, pero el job tarda algo más.
- La prueba depende de un service container; si LocalStack cambia su API/imagen, habría que ajustar (versión de imagen anclada a `:3` para acotarlo).

## 3. Qué probar

- Confirmar en la CI del PR que la prueba pasa contra LocalStack (round-trip + 404 + eliminar) y revisar el tiempo añadido al job.
- Si se quiere reducir el flake, evaluar un health check del service container además del reintento en la prueba.
- Considerar extender la prueba a casos de borde (objetos grandes, claves con caracteres especiales) si el uso real lo amerita.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Confirmar en la CI del PR que la prueba LocalStack pasa y medir el tiempo del job | AG-09 (DevOps) | 2027-12-25 | En curso |
| Unificar la URL/atribución de teselas también en `mapaRevision.js` (web) | AG-08 (fullstack) | 2027-12-25 | Pendiente |
| Prueba manual de offline en el dispositivo (modo avión) | AG-05 (QA) | 2027-12-25 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 38 | Estado actual |
| --- | --- |
| Prueba manual de offline en el dispositivo (modo avión) | Pendiente (se reitera) |
| Prueba de `AlmacenS3` contra LocalStack | Completada (entregada en este sprint) |
| Unificar la URL/atribución de teselas también en `mapaRevision.js` (web) | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 39 (prueba de `AlmacenS3` contra LocalStack): cierra la brecha de S36; 3 acciones nuevas. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
