# RC-05 — Valores permitidos del rol jerárquico del usuario

**Proyecto:** GeoVial
**Documento:** RC-05-valores-rol-jerarquico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

El rol jerárquico de un Usuario toma exactamente uno de los valores permitidos: raíz, jefe general, jefe de área o agente de campo; ningún Usuario carece de rol ni ostenta más de uno.

## 2. Entidades involucradas

Usuario.

## 3. Tipo de restricción

Valor permitido.

## 4. Mecanismo de verificación conceptual

Al dar de alta o modificar un Usuario, se comprueba que su rol jerárquico pertenece al conjunto de valores permitidos y es único. Un jefe de área y un agente de campo quedan, además, asociados a un Área existente.

## 5. RN o CU que la justifican

RN-01; CU-03, CU-14.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 |
