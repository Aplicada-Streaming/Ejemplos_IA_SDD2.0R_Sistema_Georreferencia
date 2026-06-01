# US-24 — Visor de fotos a pantalla completa con zoom

**Proyecto:** GeoVial
**Documento:** US-24-visor-fotos-pantalla-completa-zoom_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-05 Revisión sobre mapa
**Prioridad MoSCoW:** Could
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero abrir una foto del carrusel a pantalla completa y hacer zoom, para examinar un detalle de la obra que no se aprecia en la vista reducida.

## 2. Contexto

El BRIEF (§4 Could Have) plantea el visor de fotos a pantalla completa con zoom dentro del carrusel. CU-09 lo deja como detalle visual del carrusel (referido a 03). Es una mejora de la evaluación visual, no un bloqueante.

## 3. Criterios de aceptación

- Given una foto en el carrusel de un marcador, When el jefe la abre a pantalla completa, Then el sistema la muestra ampliada y permite hacer zoom sin salir del carrusel.
- Given la vista a pantalla completa, When el jefe la cierra, Then el sistema vuelve al carrusel en la misma foto.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-09 |
| BT derivadas | BT-11, BT-20 |
| Tests previstos | acceptance/AT-09-visor-pantalla-completa |

## 5. Prioridad y estimación

Could: enriquece la inspección visual; el MVP revisa fotos sin zoom dedicado. 3 SP (Fibonacci): visor de imagen con zoom integrado al carrusel de US-22.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-09)
- [x] Dependencia con US-22 (carrusel) declarada
- [ ] Comportamiento de zoom (niveles, gestos) pendiente de definir con 03
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El detalle de interacción del visor (gestos, niveles de zoom) pertenece a 03; aquí se declara la capacidad funcional.
