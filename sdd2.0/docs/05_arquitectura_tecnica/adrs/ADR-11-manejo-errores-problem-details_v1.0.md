# ADR-11 — Manejo de errores con Problem Details RFC 7807

**Proyecto:** GeoVial
**Documento:** ADR-11-manejo-errores-problem-details_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Observabilidad

## 1. Contexto

La API REST es consumida por el front web y la app móvil (ADR-02). Las RN y los CU definen un catálogo de códigos de error estables (`ACCESO_NO_AUTORIZADO`, `RELEVAMIENTO_SOLO_LECTURA`, `TRANSICION_INVALIDA`, `MARCADORES_EN_RADIO`, `UNIFICACION_NO_AUTORIZADA`, `OBSERVACION_SIN_GEORREFERENCIA`, `CONSOLIDACION_INVALIDA`, `CONFLICTO_NO_MARCADO`, `OFFLINE_NO_HABILITADO`, `ACCION_NO_AUDITADA`, `ARCHIVO_EXPORTACION_INVALIDO`, entre otros). Los consumidores necesitan un formato uniforme y legible por máquina para distinguir cada error. El cliente fijó Problem Details RFC 7807 (PROJECT-README §6). Esta ADR completa la categoría obligatoria de manejo de errores (§2.2).

## 2. Decisión

Se adopta Problem Details RFC 7807 (`application/problem+json`) como formato uniforme de respuesta de error de la API REST. Cada error de dominio se mapea a un Problem Details con un campo de código estable (extensión `code`) tomado de los catálogos de las RN y los CU, además de `type`, `title`, `status` y `detail`. El catálogo de códigos se documenta en el contrato REST.

## 3. Estado

Aceptado el 2026-06-01. ADR de ingeniería para completar la categoría obligatoria de manejo de errores (§2.2). Formaliza PROJECT-README §6.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Problem Details RFC 7807 con código estable (elegido) | Estándar, legible por máquina, uniforme entre web y móvil; fijado por el cliente | Requiere mapear cada error de dominio al formato |
| Cuerpo de error ad-hoc por endpoint | Flexible | Inconsistente; rompe consumidores; difícil de documentar |
| Solo códigos HTTP sin cuerpo estructurado | Simple | Insuficiente para distinguir los códigos de dominio de las RN |

## 5. Consecuencias positivas

1. Formato de error uniforme y estándar para web y móvil.
2. El campo `code` permite a los clientes reaccionar a cada error de dominio sin parsear texto.
3. El catálogo de códigos queda documentado y versionado en el contrato REST.

## 6. Consecuencias negativas y trade-offs

1. Cada error de dominio debe mapearse al formato; aceptado por la uniformidad.
2. Mantener el catálogo de códigos sincronizado con las RN exige disciplina; mitigado por su origen único en 02.

## 7. Implementación

Un middleware de la API traduce las excepciones de dominio y de autorización a respuestas `application/problem+json` con el `code` correspondiente y el `status` HTTP adecuado (por ejemplo 403 para `ACCESO_NO_AUTORIZADO`, 409 para `MARCADORES_EN_RADIO`, 422 para `OBSERVACION_SIN_GEORREFERENCIA`). El catálogo completo y los ejemplos se formalizan en `contratos-rest_v1.0.md` §5.

## 8. Métricas de validación

- Toda respuesta de error de la API tiene `content-type: application/problem+json` y campo `code` (suite de contrato, 08).
- Cada código del catálogo de las RN tiene un mapeo a Problem Details verificado por prueba de integración.

## 9. Referencias

- PROJECT-README §6.
- RN-01 a RN-08; catálogos de error de CU-04, CU-05, CU-07, CU-08, CU-10, CU-12.
- ADR-02, `contratos-rest_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. ADR de ingeniería que completa la categoría de manejo de errores |
