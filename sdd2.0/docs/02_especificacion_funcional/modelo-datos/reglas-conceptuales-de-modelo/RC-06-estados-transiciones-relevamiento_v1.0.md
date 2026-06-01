# RC-06 — Valores y transiciones permitidas del estado del relevamiento

**Proyecto:** GeoVial
**Documento:** RC-06-estados-transiciones-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

El estado de un Relevamiento toma exactamente uno de los valores recolección, revisión o cerrado, y solo evoluciona por las transiciones permitidas recolección → revisión, revisión → cierre y cerrado → recolección (reapertura explícita); un Relevamiento cerrado no admite escritura sobre sus Observaciones, Marcadores, Fotos, Comentarios ni Etiquetas.

## 2. Entidades involucradas

Relevamiento, Observación, Marcador, Foto, Comentario, Etiqueta.

## 3. Tipo de restricción

Valor permitido y derivación (la posibilidad de escritura se deriva del estado).

## 4. Mecanismo de verificación conceptual

Al transicionar el estado, se comprueba que la transición pertenece al conjunto permitido; cualquier otra se rechaza. Al intentar escribir sobre recursos de un Relevamiento cerrado, la escritura se bloquea hasta una reapertura explícita.

## 5. RN o CU que la justifican

RN-05; CU-04, CU-06, CU-08, CU-09, CU-10, CU-12.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 |
