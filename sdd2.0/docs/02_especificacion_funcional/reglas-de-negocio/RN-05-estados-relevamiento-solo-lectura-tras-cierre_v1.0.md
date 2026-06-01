# RN-05 — Estados del relevamiento y solo lectura tras el cierre

**Proyecto:** GeoVial
**Documento:** RN-05-estados-relevamiento-solo-lectura-tras-cierre_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Un relevamiento se encuentra siempre en exactamente uno de los estados recolección, revisión o cerrado; las transiciones válidas son recolección → revisión, revisión → cierre y, de forma explícita, cerrado → recolección por reapertura; un relevamiento cerrado es de solo lectura y no admite captura, edición ni resolución de conflictos hasta que el jefe de área lo reabra.

## 2. Justificación

Origen de negocio. El ciclo de estados ordena el trabajo entre la recolección de campo y la evaluación experta, y el bloqueo de solo lectura protege la integridad de lo evaluado. La reapertura debe ser una acción deliberada para evitar modificaciones accidentales de un relevamiento dado por terminado (NB-04).

## 3. Ámbito de aplicación

Se evalúa en cada intento de transición de estado y en cada operación de escritura sobre un relevamiento, sus observaciones, marcadores, fotos, comentarios y etiquetas. Aplica tanto a la app de campo como a la web.

## 4. Consecuencia si se viola

Una transición no contemplada se rechaza con el código `TRANSICION_INVALIDA`. Una escritura sobre un relevamiento cerrado se rechaza con el código `RELEVAMIENTO_SOLO_LECTURA`. La reapertura sin la acción explícita del jefe de área se rechaza con el código `REAPERTURA_NO_AUTORIZADA`.

## 5. CU afectados

CU-01, CU-04, CU-05, CU-06, CU-08, CU-09, CU-10, CU-12.

## 6. Pruebas que la verifican

- Una transición recolección → cierre directa se rechaza con `TRANSICION_INVALIDA`.
- Una captura sobre un relevamiento cerrado se rechaza con `RELEVAMIENTO_SOLO_LECTURA`.
- La reapertura solo procede con acción explícita del jefe de área y deja el relevamiento en recolección.
- Referencia a casos de prueba previstos en 08, suite de estados del relevamiento.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-04 |
