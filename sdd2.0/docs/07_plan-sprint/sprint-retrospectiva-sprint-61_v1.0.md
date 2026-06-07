# Sprint Retrospectiva — Sprint 61

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-61_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Tercer P1 de la auditoría cerrado** (tras H-05 y H-02). El carrusel ahora se siente como una galería: swipe que cruza de marcador.
- **La lógica difícil en el núcleo testeable.** El cruce de marcador (con sus bordes: última/primera foto, marcador sin fotos, circular) quedó en `NavegadorRevision.Avanzar/Retroceder`, cubierto por 5 tests; el gesto es glue de una línea por dirección.
- **Reuso del navegador de S26.** No hubo que rehacer la navegación: se sumaron dos métodos a un núcleo que ya manejaba los índices.
- **Verificación on-device con datos reales.** Se sembraron 2 marcadores por API (uno con 2 fotos) y se probó el swipe con `adb input swipe`: cruzó a "Marcador 2/2" y volvió, confirmando el comportamiento, no sólo el render.

## 2. Qué no salió bien

- **Sin datos, no hay demo.** El relevamiento de prueba estaba vacío; hubo que sembrar marcadores por API para verificar el swipe. Refuerza la necesidad de un set de datos de demo con marcadores y fotos (y binarios) listo para QA.
- **Foto sin binario = caja gris.** Las fotos sembradas no tienen binario subido, así que el visor muestra gris; el carrusel navega igual, pero la demo visual sería más clara con imágenes reales.
- **Relogueo on-device, otra vez.** Se reitera la acción (pendiente) de un script de login/captura para abaratar la verificación.

## 3. Qué probar

- On-device: con un marcador de varias fotos, swipe izquierda/derecha recorre las fotos y, al terminar, cruza de marcador; los botones siguen funcionando.
- Borde: relevamiento de un solo marcador (swipe circular vuelve a la primera foto) y marcador sin fotos (cruza directo).

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Verificar on-device el swipe con fotos reales (binarios subidos) | AG-05 (QA) | 2028-10-27 | En curso (con el usuario) |
| Set de datos de demo con marcadores + fotos + binarios para QA | AG-09 | 2028-10-27 | Pendiente |
| Último P1 de la auditoría: H-01/H-03 (captura sobre el mapa) — sprint de alcance pleno | Equipo | 2028-10-27 | Planificado |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 60 | Estado |
| --- | --- |
| Verificar on-device "Mi ubicación" en mapa y map-pick | En curso (con el usuario) |
| Marca de posición "viva" + control de mapa compartido | Pendiente |
| Próximos P1: H-01/H-03, H-04 | H-04 **hecho** (este sprint); resta H-01/H-03 |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Retro del Sprint 61 (H-04: carrusel deslizable). Bien: lógica de cruce en el núcleo testeable, reuso del navegador de S26, verificado on-device con datos sembrados. A mejorar: set de datos de demo y script de login on-device. Último P1: H-01/H-03 (captura sobre el mapa). Generada por AG-07 |
