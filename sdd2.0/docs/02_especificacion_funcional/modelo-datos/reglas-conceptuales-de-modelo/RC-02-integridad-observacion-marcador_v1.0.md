# RC-02 — Integridad referencial de observación a marcador del mismo relevamiento

**Proyecto:** GeoVial
**Documento:** RC-02-integridad-observacion-marcador_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Toda Observación se asocia a exactamente un Marcador, y ese Marcador pertenece al mismo Relevamiento que la Observación; una Observación sin georreferenciar permanece en la bandeja sin georreferenciar del Relevamiento hasta asociarse a un Marcador.

## 2. Entidades involucradas

Observación, Marcador, Relevamiento.

## 3. Tipo de restricción

Referencial.

## 4. Mecanismo de verificación conceptual

Al asentar o consolidar una Observación, se verifica que su Marcador exista y pertenezca al mismo Relevamiento. Si la Observación no tiene Marcador resuelto, queda señalada como sin georreferenciar dentro del Relevamiento, sin referencia a un Marcador de otro Relevamiento.

## 5. RN o CU que la justifican

RN-03, RN-05; CU-04, CU-05, CU-08.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 |
