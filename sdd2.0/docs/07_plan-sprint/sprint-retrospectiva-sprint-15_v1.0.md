# Sprint Retrospectiva — Sprint 15

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-15_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- El cambio de contrato para exponer el `FotoId` en la captura fue mínimo y aditivo: una sola propiedad nueva en `ResultadoCaptura`/`CapturaResponse`, sin romper a ningún consumidor (las 30 pruebas de integración siguieron verdes).
- La prueba de integración del multipart (`ConstructorContenidoMultipart` contra el endpoint real) verifica el contrato de la subida de forma barata: un 404 por foto inexistente, en vez de un 400, confirma que la parte `archivo` liga correctamente.
- El núcleo `GeoVial.CapturaCampo` siguió creciendo bajo el gate (91,5 % líneas / 93,5 % branches) con las clases nuevas al 100 %; la separación núcleo/cáscara se mantuvo limpia.
- Reusar los endpoints ya existentes (ubicación manual del Sprint 03, contenido de foto del Sprint 07) dejó el sprint enfocado en el frente cliente, sin reescribir backend.

## 2. Qué no salió bien

- La ubicación manual se ingresa por coordenada (latitud/longitud), no sobre un mapa interactivo: el control de mapas de MAUI requiere una clave de proveedor (Google Maps) que no se provisionó, así que la experiencia "tocar el punto en el mapa" de US-13 queda a medias en la UI (la lógica sí está completa).
- No hay una prueba de integración del happy-path completo de captura (crear área + jefe + relevamiento + agente + asignación + transición), por lo que la exposición del `FotoId` se verifica a nivel del handler y no extremo a extremo por HTTP.
- La subida del binario no comprime ni valida el tipo en el cliente antes de enviar; depende del pipeline del backend (BT-19) para acotar el tamaño.

## 3. Qué probar

- Provisionar la clave del proveedor de mapas y reemplazar las entradas de coordenada por un mapa interactivo para colocar el punto (US-13 completo en UI).
- Añadir un helper de integración que arme el escenario completo de captura (área→jefe→relevamiento→agente→asignación) y verifique captura→`FotoId`→subida→descarga extremo a extremo.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Provisionar clave de mapas y mapa interactivo para la ubicación manual (US-13 UI completa) | AG-08 (móvil) | 2027-01-23 | Pendiente |
| Helper de integración del escenario completo de captura (área→agente asignado) | AG-05 / QA | 2027-01-23 | Pendiente |
| Publicar `GeoVial.Sync` como paquete preview en GitHub Packages | AG-09 (DevOps) | 2027-01-23 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 14 | Estado actual |
| --- | --- |
| Construir la ubicación manual del punto sobre mapa en `GeoVial.Mobile` (US-13) | Completada en lógica y envío; el mapa interactivo se reitera para cuando haya clave de proveedor |
| Encadenar la subida del binario de la foto tras la captura en el cliente | Completada (entregada en este sprint) |
| Validar el lector EXIF con fotos reales y evaluar soporte HEIC | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 15 con 3 acciones nuevas y seguimiento de las del Sprint 14 (subida del binario completada; mapa interactivo reiterado). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
