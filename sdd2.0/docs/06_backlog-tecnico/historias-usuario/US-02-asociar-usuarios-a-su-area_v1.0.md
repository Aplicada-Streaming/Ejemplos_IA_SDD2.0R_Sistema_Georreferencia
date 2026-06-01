# US-02 — Asociar usuarios a su área

**Proyecto:** GeoVial
**Documento:** US-02-asociar-usuarios-a-su-area_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-01 Jerarquía, usuarios y acceso
**Prioridad MoSCoW:** Must
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como administrador de jerarquía, quiero que cada jefe de área y cada agente quede asociado a un área existente, para que la autorización por área tenga una base coherente y nadie quede sin ámbito de trabajo.

## 2. Contexto

NB-01 organiza el trabajo por áreas administrativas de vialidad. CU-03 valida que un jefe de área y sus agentes queden asociados a un área existente. Sin esta asociación, la autorización por área (US-31, CU-14) no tiene contra qué validar y se rompe la delegación acotada.

## 3. Criterios de aceptación

- Given un jefe de área que da de alta un agente de su área, When asocia el agente al área "Zona Norte" existente (su propia área), Then el sistema crea el agente asociado a "Zona Norte".
- Given un jefe general que da de alta un jefe de área, When lo asocia a un área inexistente "Zona X", Then el sistema rechaza con `AREA_INEXISTENTE` y no crea el usuario.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-03 |
| BT derivadas | BT-01, BT-07, BT-09, BT-18 |
| Tests previstos | acceptance/AT-03-asociacion-area |

## 5. Prioridad y estimación

Must: la pertenencia a área es precondición de toda autorización acotada. 3 SP (Fibonacci): es una validación de integridad sobre el alta ya construida en US-01.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-03)
- [x] Reglas de negocio identificadas (RN-01)
- [x] Dependencia con US-01 declarada y no bloqueante en refinamiento
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La geometría detallada del área es un dato del dominio; aquí solo se exige que el área referenciada exista y sea coherente con el nivel del usuario.

Corrección (Sprint 01): los criterios de aceptación se ajustaron para respetar la invariante de nivel inmediato inferior (CU-03 §3, BT-02). El alta de un agente la administra el jefe de área (no el jefe general); el camino `AREA_INEXISTENTE` se ilustra con el jefe general dando de alta un jefe de área con un área inexistente, que es la combinación autorizada que alcanza la validación de existencia del área.
