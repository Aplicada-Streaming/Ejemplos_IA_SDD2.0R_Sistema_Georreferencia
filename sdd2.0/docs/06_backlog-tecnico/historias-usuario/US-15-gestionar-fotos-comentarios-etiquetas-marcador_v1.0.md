# US-15 — Gestionar fotos, comentarios y etiquetas del marcador

**Proyecto:** GeoVial
**Documento:** US-15-gestionar-fotos-comentarios-etiquetas-marcador_v1.0.md
**Versión:** 1.0
**Estado:** Entregada (backend S04, frente cliente S19)
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-03 Captura y georreferenciación
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero agregar y quitar fotos de un marcador, comentarlas y etiquetarlas, para enriquecer la observación de un punto con todo el detalle que registro en la obra.

## 2. Contexto

NB-04 necesita marcadores ricos para la revisión. CU-09 permite administrar fotos, comentarios y etiquetas del marcador (RN-05 para el bloqueo tras cierre). El glosario del dominio define que una foto puede o no estar ligada a un comentario y que etiquetas se aplican a fotos o comentarios. Sin esta gestión, el marcador sería una coordenada vacía.

## 3. Criterios de aceptación

- Given un marcador con tres fotos en un relevamiento en recolección, When el agente agrega una cuarta foto con un comentario y una etiqueta, Then el sistema persiste la foto, el comentario y la etiqueta y el marcador refleja cuatro fotos.
- Given un marcador en un relevamiento cerrado, When el usuario intenta quitar una foto, Then el sistema responde `RELEVAMIENTO_SOLO_LECTURA` y conserva las fotos.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-09 |
| BT derivadas | BT-05, BT-07, BT-09, BT-19, BT-20 |
| Tests previstos | acceptance/AT-09-gestion-marcador |

## 5. Prioridad y estimación

Must: el contenido del marcador es la observación misma. 8 SP (Fibonacci): alta/baja de fotos, comentarios y etiquetas con sus cardinalidades (RC-04) y el bloqueo de solo lectura.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-09)
- [x] Reglas de negocio identificadas (RN-05, RN-01)
- [x] Dependencia con US-11/US-12 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El recorrido del carrusel y la navegación entre marcadores se cubren en US-22. El visor a pantalla completa es un Could (US-24). El detalle visual del carrusel pertenece a 03.
