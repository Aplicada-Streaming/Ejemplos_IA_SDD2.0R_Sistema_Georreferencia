# Sprint Retrospectiva — Sprint 65

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-65_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Reuso del puente WebView→app.** Tocar el mapa de Captura reusa el esquema centinela y `ParseadorMensajeUbicacion` de S51, la intercepción del `MapaWebViewClient` (S55) y `FijarCoordenada` (S56): el cambio quedó chico y testeable, con núcleo nuevo cubierto (+7).
- **Cierra la visión de S62.** Con S64 (el mapa renderiza) + S65 (el mapa es interactivo), "capturar sobre el mapa" queda completo: contexto de lo ya capturado + fijar el punto sin página aparte ni tipear.
- **Sin tocar backend.** Toda la historia es núcleo del gate (`MapaCapturaHtml`) + glue de UI; dominio/datos/sync intactos.

## 2. Qué no salió bien

- **Tres HTML de mapa casi iguales.** `MapaRevisionHtml`, `MapaUbicacionHtml` y ahora `MapaCapturaHtml` comparten el grueso del boilerplate de Leaflet/OSM. La deuda del **control de mapa compartido** crece; conviene unificarlos en un próximo sprint.
- **Verificación on-device del toque.** Confirmar la colocación del pin por toque exige interacción real; el dump no observa el contenido del WebView. Se verificó por construcción + interacción manual, no por aserción automatizada.

## 3. Qué probar

- On-device: en Captura, tocar el mapa coloca el pin y el estado muestra "Punto listo: …"; arrastrar el pin actualiza la coordenada; "Enviar captura" encola con esa coordenada.
- Una foto con EXIF ignora el punto del mapa (precedencia RN-03); una foto sin EXIF usa el punto tocado.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Control de mapa compartido: unificar `MapaRevisionHtml`/`MapaUbicacionHtml`/`MapaCapturaHtml` | AG-08 | 2028-12-22 | Pendiente (prioridad sube) |
| Pasada de accesibilidad/UI dedicada: H-14 (live regions) + H-07 (tabs) | AG-08 | 2028-12-22 | Pendiente (reiterado) |
| Script de reset/arranque del entorno de desarrollo | AG-09 | 2028-12-22 | Pendiente (reiterado de S64) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 64 | Estado |
| --- | --- |
| Script de reset/arranque del entorno de desarrollo | Pendiente (reiterado) |
| Verificar layouts móviles con datos vacíos (sin marcadores) | **Hecho** (`MapaCapturaHtml` testea el caso sin marcadores; verificado el mapa con relevamiento sin pines) |
| Control de mapa compartido | Pendiente (prioridad sube) |
| Pasada de accesibilidad/UI dedicada | Pendiente (reiterado) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Retro del Sprint 65 (evolución de H-01: capturar tocando el mapa). Bien: reuso del puente de S51/S55/S56, cierra la visión de S62, sin backend. Fricción: tres HTML de mapa casi iguales (sube la prioridad del control compartido). Generada por AG-07 |
