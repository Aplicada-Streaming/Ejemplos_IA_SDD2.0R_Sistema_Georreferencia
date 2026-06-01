# RC-04 — Cardinalidad de fotos y comentarios y su etiquetado

**Proyecto:** GeoVial
**Documento:** RC-04-cardinalidad-foto-comentario-etiqueta_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Una Foto pertenece a un Marcador y puede tener varios Comentarios; un Comentario pertenece a un Marcador y se liga a cero o una Foto; tanto Fotos como Comentarios pueden tener varias Etiquetas, y una Etiqueta puede aplicarse a varias Fotos y Comentarios.

## 2. Entidades involucradas

Foto, Comentario, Etiqueta, Marcador.

## 3. Tipo de restricción

Cardinalidad.

## 4. Mecanismo de verificación conceptual

Al asociar un Comentario, se admite que no se ligue a ninguna Foto o que se ligue a una sola. Al aplicar una Etiqueta a una Foto o a un Comentario, se admite la relación muchos a muchos. Al quitar una Foto, se quitan sus Comentarios ligados y se desvinculan sus Etiquetas, conservando el resto del Marcador.

## 5. RN o CU que la justifican

RN-05; CU-09.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 |
