# Sprint Review — Sprint 15

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-15_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-15_v1.0.md`:

> Cerrar el flujo de captura de campo en el cliente móvil: que el agente pueda ubicar manualmente sobre el mapa el punto de una observación cuya foto no trae GPS (US-13, CU-05) y que, tras capturar, la imagen quede alojada subiendo su binario al backend de alojamiento (BT-20/ADR-08). Para encadenar la subida, la respuesta de captura expone el `FotoId` de la foto creada.

Veredicto: Cumplido.

Explicación corta: la captura ahora expone el `FotoId` de la foto creada (`ResultadoCaptura` y `CapturaResponse`, cambio aditivo), de modo que el cliente puede subir su binario. El `ConstructorContenidoMultipart` arma el contenido multipart (parte `archivo`) que consume el endpoint de contenido; una prueba de integración confirma que ese multipart liga la parte correcta del endpoint real. El `ArmadorUbicacionManual` valida la coordenada manual (rango geográfico, RN-03) y arma la `UbicarManualRequest`. La pantalla de captura en `GeoVial.Mobile` encadena, tras la captura, la subida del binario y —si la observación quedó sin georreferenciar— ofrece ubicar el punto a mano, fuera de CI.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| BT-20 | Tarea | Tras capturar, el cliente sube el binario de la foto al alojamiento usando el `FotoId` devuelto | La imagen queda alojada, no solo referenciada |
| US-13 | Historia | Una observación sin GPS se ubica a mano: el cliente valida la coordenada y la envía; el backend reagrupa por radio | Ninguna observación se pierde por falta de metadatos |
| US-13 | Historia | Una coordenada fuera de rango se rechaza en el cliente antes de enviarla | Validación temprana |

## 3. Feedback recibido

- El flujo de captura de campo queda cerrado de punta a punta en el cliente: foto + EXIF (o punto manual) y la imagen alojada.
- Exponer el `FotoId` en la captura fue el ajuste mínimo de contrato que destrabó el encadenado de la subida sin romper consumidores existentes.
- La colocación del punto sobre un mapa interactivo (control de mapas con clave de proveedor) queda como evolución de la pantalla; la lógica de validación y envío ya está entregada y testeada.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 11 |
| Puntos completados | 11 |
| Velocity efectiva | 11 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 282 verdes (252 unitarias + 30 de integración), +10 respecto del Sprint 14 (núcleo de ubicación manual y multipart, `FotoId` en el handler, y la integración del multipart contra el endpoint). Cobertura del núcleo `GeoVial.CapturaCampo`: 91,5 % líneas / 93,5 % branches (gate ≥ 80 % / ≥ 70 % cumplido); las clases nuevas al 100 %. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-13 | Historia | Aceptada (frente cliente: validación y armado de la ubicación manual; la pantalla MAUI con coordenada compila fuera de CI) |
| BT-20 | Tarea | Aceptada (subida del binario tras la captura, con `FotoId` expuesto; integración del multipart verificada) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 15 se traslada. |

La colocación del punto sobre un mapa interactivo (control de mapas) y la validación del lector EXIF contra fotos reales / HEIC quedan en el backlog.

## 7. Decisiones tomadas durante el review

- Aceptar el cambio aditivo de contrato (`FotoId` en la captura) como parte del flujo de subida del binario.
- Mantener la ubicación manual por coordenada en la pantalla actual y planificar el mapa interactivo como evolución.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 15 (ubicación manual del punto + subida del binario de la foto, frente cliente; US-13/BT-20). Veredicto Cumplido, velocity 11, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
