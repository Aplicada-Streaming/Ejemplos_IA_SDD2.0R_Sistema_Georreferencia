# RC-07 — Integridad de la asignación de agente al área del relevamiento

**Proyecto:** GeoVial
**Documento:** RC-07-integridad-asignacion-agente-area_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Una AsignaciónAgente vincula un Relevamiento con un Usuario de rol agente de campo que pertenece a la misma Área que el Relevamiento; no se admite asignar un agente de un Área distinta a la del Relevamiento.

## 2. Entidades involucradas

AsignaciónAgente, Relevamiento, Usuario, Área.

## 3. Tipo de restricción

Referencial y cardinalidad (un Relevamiento puede tener varias asignaciones; cada asignación referencia un agente del Área del Relevamiento).

## 4. Mecanismo de verificación conceptual

Al crear o actualizar una AsignaciónAgente, se verifica que el Usuario tiene rol agente de campo y que su Área coincide con el Área del Relevamiento. Si no coincide, la asignación se rechaza.

## 5. RN o CU que la justifican

RN-01; CU-01, CU-02.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 |
