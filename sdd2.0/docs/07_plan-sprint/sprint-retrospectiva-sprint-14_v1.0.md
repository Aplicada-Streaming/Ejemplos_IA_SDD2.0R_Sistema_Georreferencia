# Sprint Retrospectiva — Sprint 14

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-14_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Implementar el lector EXIF-GPS de forma propia (sin paquete externo) dejó la lógica RN-03 enteramente bajo el gate y evitó reintroducir vulnerabilidades (NU1902); el núcleo `GeoVial.CapturaCampo` alcanzó 90,7 % líneas / 92,7 % branches.
- Las pruebas de extracción son herméticas: un constructor de JPEG+EXIF en el propio test genera la entrada byte a byte según la especificación, sin binarios de fixture. El codificador (helper) y el decodificador (lector) son implementaciones independientes de la misma especificación, lo que da confianza real al round-trip.
- Separar el núcleo testeable (`GeoVial.CapturaCampo`, net10.0) de la pantalla MAUI mantuvo el patrón de S11–S13: el valor verificable entra a CI sin atarlo al empaquetado del APK.
- Reusar el contrato compartido `CapturarObservacionRequest` evitó duplicar el DTO entre el cliente y el backend.

## 2. Qué no salió bien

- El parser EXIF cubre la estructura estándar (GPS IFD con rationals DMS), pero no se validó contra fotos de cámaras reales con variantes (MakerNotes, orientación, formatos no-JPEG como HEIC); ante cualquier variante no soportada degrada a "sin georreferencia", que es seguro pero puede perder coordenadas válidas.
- La pantalla de captura arma y envía la petición, pero la subida del binario de la foto al alojamiento (BT-20/ADR-08) desde el cliente quedó fuera de alcance: la captura referencia un archivo cuyo binario aún no se sube desde el móvil.
- La ubicación manual del punto (US-13) sigue pendiente: hoy la observación sin GPS llega a la bandeja pero el cliente no ofrece todavía colocarla en el mapa.

## 3. Qué probar

- Validar el lector EXIF contra un set de fotos reales de Android/iOS y, si hace falta, soportar HEIC (formato por defecto de varias cámaras) además de JPEG.
- Encadenar en el cliente la subida del binario de la foto (SubirContenidoFotoCommand) tras la captura, para que la observación quede con su imagen alojada.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Construir la ubicación manual del punto sobre mapa en `GeoVial.Mobile` (US-13) | AG-08 (móvil) | 2027-01-09 | Pendiente |
| Encadenar la subida del binario de la foto tras la captura en el cliente | AG-08 (móvil) | 2027-01-09 | Pendiente |
| Validar el lector EXIF con fotos reales y evaluar soporte HEIC | AG-05 | 2027-01-09 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 13 | Estado actual |
| --- | --- |
| Publicar `GeoVial.Sync` como paquete preview en GitHub Packages y que la demo lo consuma | Pendiente (se reitera) |
| Construir la UI de captura de campo (foto/GPS/comentario) en `GeoVial.Mobile` | Completada (entregada en este sprint, frente cliente de US-11) |
| Evaluar la bajada de actualizaciones con cola vacía en el motor (US-19 CA-02 por esa vía) | Pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 14 con 3 acciones nuevas y seguimiento de las del Sprint 13 (UI de captura de campo completada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
