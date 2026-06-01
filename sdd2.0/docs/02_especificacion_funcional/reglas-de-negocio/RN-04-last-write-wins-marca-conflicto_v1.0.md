# RN-04 — Última escritura prevalece con marca de conflicto

**Proyecto:** GeoVial
**Documento:** RN-04-last-write-wins-marca-conflicto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Cuando dos o más cambios sobre el mismo recurso entran en conflicto al sincronizarse, prevalece el cambio con la marca temporal más reciente a nivel de campo, y todo recurso resuelto por esta vía queda marcado como conflicto resoluble manualmente desde la web hasta que un usuario autorizado lo dé por resuelto.

## 2. Justificación

Origen de negocio y de ingeniería. El trabajo concurrente y sin conexión genera ediciones incompatibles del mismo recurso. La última escritura ofrece un criterio determinista de consolidación, y la marca de conflicto preserva la posibilidad de revisión humana para no perder de vista cambios descartados (NB-05).

## 3. Ámbito de aplicación

Se evalúa durante la sincronización, al consolidar cambios locales contra el estado central, y durante la resolución de conflictos desde la web. Aplica a marcadores, observaciones, comentarios, etiquetas y fotos editados de forma concurrente.

## 4. Consecuencia si se viola

Si la consolidación no aplica la última escritura, la sincronización se rechaza con el código `CONSOLIDACION_INVALIDA`. Si un recurso consolidado por última escritura no queda marcado como conflicto, la operación se rechaza con el código `CONFLICTO_NO_MARCADO`; el recurso no se considera sincronizado hasta que la marca exista.

## 5. CU afectados

CU-07, CU-11, CU-12.

## 6. Pruebas que la verifican

- Dos ediciones del mismo campo: prevalece la de marca temporal más reciente.
- El recurso consolidado por última escritura queda marcado como conflicto.
- La marca de conflicto persiste hasta que un usuario autorizado lo resuelve desde la web.
- Referencia a casos de prueba previstos en 08, suite de consolidación y conflictos.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-03 y NB-05 |
