# RN-03 — Prioridad de los metadatos de ubicación de la foto para la georreferenciación

**Proyecto:** GeoVial
**Documento:** RN-03-prioridad-exif-georreferenciacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Cuando una observación se origina a partir de una foto, la coordenada del marcador se deriva en primer lugar de los metadatos de ubicación incrustados en la foto (EXIF); solo si la foto carece de esos metadatos se recurre a la ubicación manual del punto sobre el mapa, y si tampoco se ubica manualmente la observación queda sin georreferenciar.

## 2. Justificación

Origen de negocio. La confiabilidad de la base depende de un dato de ubicación robusto. Los metadatos de la foto son la fuente más fiel del lugar de captura; la ubicación manual es el recurso de respaldo cuando esa fuente falta (NB-02).

## 3. Ámbito de aplicación

Se evalúa al capturar una observación en terreno y al cargar manualmente una observación desde la web a partir de una foto existente. Define el orden de fuentes de ubicación: metadatos de la foto, luego ubicación manual, luego bandeja sin georreferenciar.

## 4. Consecuencia si se viola

Si la foto trae metadatos de ubicación y el sistema no los usa como fuente primaria, la observación se considera mal georreferenciada y se rechaza con el código `FUENTE_UBICACION_INCORRECTA`. Si la foto no trae metadatos y no se ubica el punto manualmente, la observación se asienta en la bandeja sin georreferenciar del relevamiento con el código `OBSERVACION_SIN_GEORREFERENCIA`.

## 5. CU afectados

CU-04, CU-05, CU-06.

## 6. Pruebas que la verifican

- Una foto con metadatos de ubicación produce un marcador en esa coordenada sin pedir ubicación manual.
- Una foto sin metadatos solicita ubicación manual del punto.
- Una foto sin metadatos no ubicada manualmente cae en la bandeja sin georreferenciar.
- Referencia a casos de prueba previstos en 08, suite de georreferenciación.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-02 |
