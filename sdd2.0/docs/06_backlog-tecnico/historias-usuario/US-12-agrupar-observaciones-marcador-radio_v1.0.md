# US-12 — Agrupar observaciones en marcador por radio

**Proyecto:** GeoVial
**Documento:** US-12-agrupar-observaciones-marcador-radio_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-03 Captura y georreferenciación
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero que las observaciones cercanas se agrupen automáticamente en un mismo marcador según el radio del relevamiento, para que un punto de la obra concentre todas sus fotos y comentarios sin duplicar marcadores.

## 2. Contexto

NB-02 y el glosario del dominio definen el marcador como el punto que agrupa observaciones. CU-04 (pasos 3-4) verifica si existe un marcador dentro del radio de agrupación (RN-02) y asocia o crea según corresponda. Sin esta agrupación, cada foto crearía un marcador y la revisión sobre mapa sería inmanejable.

## 3. Criterios de aceptación

- Given un marcador existente y una foto tomada a 8 m de él con radio 15 m, When el agente registra la observación, Then el sistema la asocia al marcador existente sin crear uno nuevo.
- Given un relevamiento con radio 15 m, When el agente toma una foto cuya coordenada no cae dentro del radio de ningún marcador, Then el sistema crea un marcador nuevo en esa coordenada.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-02 |
| CU cubiertos | CU-04 |
| BT derivadas | BT-05, BT-07, BT-09 |
| Tests previstos | acceptance/AT-04-agrupacion-radio |

## 5. Prioridad y estimación

Must: la agrupación por radio es lo que hace utilizable la revisión por marcadores. 5 SP (Fibonacci): cálculo de cercanía por radio y decisión asociar/crear, regla de dominio RN-02.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-04)
- [x] Reglas de negocio identificadas (RN-02)
- [x] Dependencia con US-11 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

Dos marcadores que tras sincronizar quedan dentro de un mismo radio no se unifican automáticamente: se tratan como conflicto (US-25, US-26).
