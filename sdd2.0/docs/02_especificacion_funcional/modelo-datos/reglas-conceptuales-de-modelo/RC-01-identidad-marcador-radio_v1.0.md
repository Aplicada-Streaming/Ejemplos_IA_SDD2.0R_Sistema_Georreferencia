# RC-01 — Identidad de marcador y radio de agrupación

**Proyecto:** GeoVial
**Documento:** RC-01-identidad-marcador-radio_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Cada Marcador tiene identidad propia dentro de su Relevamiento y una coordenada; dos Marcadores del mismo Relevamiento cuyas coordenadas disten menos que el radio de agrupación configurado se consideran candidatos al mismo punto y no se unifican ni descartan sin decisión humana.

## 2. Entidades involucradas

Marcador, Relevamiento.

## 3. Tipo de restricción

Identidad y derivación (la condición de candidato a duplicado se deriva de la distancia entre coordenadas frente al radio del relevamiento).

## 4. Mecanismo de verificación conceptual

Al crear o reubicar un Marcador, se compara su coordenada con las de los demás Marcadores del Relevamiento contra el radio de agrupación. Si la distancia es menor que el radio, se genera un ConflictoSync de tipo "marcadores en un mismo radio"; ninguna unificación o descarte ocurre de forma automática.

## 5. RN o CU que la justifican

RN-02; CU-04, CU-05, CU-07, CU-11, CU-12.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 |
