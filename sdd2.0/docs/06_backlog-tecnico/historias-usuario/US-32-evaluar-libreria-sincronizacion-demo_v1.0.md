# US-32 — Evaluar la librería de sincronización con una demo autónoma

**Proyecto:** GeoVial
**Documento:** US-32-evaluar-libreria-sincronizacion-demo_v1.0.md
**Versión:** 1.0
**Estado:** Entregada (Sprint 13)
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-09 Librería de sincronización publicada
**Prioridad MoSCoW:** Should
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como integrador de otro proyecto, quiero una demo autónoma que ejercite la librería de sincronización contra un backend simulado, para evaluar si la reutilizo en mi propio sistema sin depender de GeoVial.

## 2. Contexto

NB-03 sustenta la sincronización confiable, y el cliente pide explícitamente que la librería de sincronización se publique para reuso e incluya una demo autónoma (BRIEF de README §1, §14). CU-06 y CU-07 son la capacidad que la librería materializa. La demo permite alta de registros locales, sincronización contra un mock, visualización del estado de la cola y resolución básica de conflictos.

## 3. Criterios de aceptación

- Given la demo autónoma con un backend simulado, When el integrador da de alta registros locales y dispara la sincronización, Then la demo muestra el estado de la cola pasando de pendiente a sincronizado.
- Given un cambio que choca con el estado del mock, When la demo sincroniza, Then la demo reporta el conflicto y permite una resolución básica.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-06, CU-07 |
| BT derivadas | BT-15, BT-16, BT-17, BT-22 |
| Tests previstos | acceptance/AT-32-demo-sync |

## 5. Prioridad y estimación

Should: el sistema funciona sin la demo, pero el cliente la pide como requisito de reuso de la librería publicada. 8 SP (Fibonacci): demo MAUI ajena al sistema sobre la superficie pública de la librería.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-06, CU-07)
- [x] Fuente upstream identificada (ADR-07, contrato de Abstractions de sync)
- [x] Dependencia con US-17/US-18 (cola y motor de sync) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La demo es ajena al sistema GeoVial y ejercita solo la superficie pública (Abstractions) de la librería. Vive en `samples/02-sync-maui-demo`. El versionado de la librería es SemVer estricto (ADR-07).
