# RC-03 — Unicidad del identificador en la cola de sincronización para idempotencia

**Proyecto:** GeoVial
**Documento:** RC-03-unicidad-identificador-cola-sync_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Cada RegistroCambioSync tiene un identificador único que no se repite en la cola, de modo que un mismo cambio no se aplique más de una vez al sincronizar, aun ante reintentos o sincronizaciones parciales.

## 2. Entidades involucradas

RegistroCambioSync.

## 3. Tipo de restricción

Identidad (unicidad del identificador, garantía de idempotencia).

## 4. Mecanismo de verificación conceptual

Al encolar un cambio se le asigna un identificador único; al consolidarlo durante la sincronización se comprueba que ese identificador no se haya aplicado antes. Si ya se aplicó, la consolidación lo omite sin duplicar el efecto; al confirmarse, el cambio se retira de la cola.

## 5. RN o CU que la justifican

RN-04; CU-06, CU-07.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 |
