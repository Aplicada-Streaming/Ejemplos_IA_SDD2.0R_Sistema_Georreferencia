# Sprint Review — Sprint 14

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-14_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-14_v1.0.md`:

> Permitir que el agente de campo capture una observación desde la app móvil tomando o eligiendo una foto: el cliente deriva automáticamente la coordenada de los metadatos EXIF de la foto (RN-03) y arma la petición de captura contra el backend (US-11, CU-04). Si la foto no trae ubicación, la observación se deriva a la bandeja sin georreferenciar a la espera de ubicación manual.

Veredicto: Cumplido.

Explicación corta: el nuevo núcleo `GeoVial.CapturaCampo` extrae la coordenada GPS de los metadatos EXIF de una foto JPEG (`LectorGpsExif`: APP1/Exif → TIFF → GPS IFD → rationals DMS → grados decimales, RN-03), soportando ambos órdenes de bytes y los cuatro hemisferios; ante una foto sin GPS o malformada devuelve `null` y la observación no se pierde. El `ArmadorCapturaCampo` construye la `CapturarObservacionRequest` (contrato compartido), validando el rango geográfico y derivando a la bandeja sin georreferenciar cuando no hay coordenada válida (US-11 CA-02). La pantalla de captura en `GeoVial.Mobile` (MediaPicker → armador → POST al backend) ejercita el flujo en el dispositivo, fuera de CI.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-11 | Historia | Foto con metadatos EXIF de ubicación: el cliente deriva la coordenada (con signo por hemisferio) y arma una captura georreferenciada | Cero carga manual de coordenadas |
| US-11 | Historia | Foto sin metadatos: el cliente arma una captura sin coordenada y el backend la deriva a la bandeja sin georreferenciar | La observación no se pierde |
| US-11 | Historia | Coordenada fuera de rango o foto no-JPEG: se trata como sin georreferencia | Robustez ante datos corruptos |

## 3. Feedback recibido

- La georreferenciación automática por EXIF queda demostrable en el cliente, cerrando la propuesta de valor central (NB-02) sobre el backend ya entregado en el Sprint 03.
- Implementar el lector EXIF sin paquetes externos evita reintroducir vulnerabilidades (NU1902) y deja la lógica RN-03 enteramente bajo el gate de cobertura.
- Lo que resta del flujo de captura completo es la ubicación manual del punto cuando la foto no trae GPS (US-13) y la subida del binario de la foto al alojamiento desde el cliente.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 272 verdes (243 unitarias + 29 de integración), +16 respecto del Sprint 13 (12 de extracción/armado + 4 de robustez del parser). Cobertura del núcleo `GeoVial.CapturaCampo`: 90,7 % líneas / 92,7 % branches (gate ≥ 80 % / ≥ 70 % cumplido). Las pruebas de extracción EXIF son herméticas: construyen el JPEG+EXIF de entrada en el propio test, sin binarios de fixture. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-11 | Historia | Aceptada (frente cliente: extracción EXIF + armado de la captura; la pantalla MAUI compila para `net10.0-android` fuera de CI) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 14 se traslada. |

La pantalla de captura vive en `GeoVial.Mobile`, fuera de la solución/CI por el empaquetado del APK. La ubicación manual del punto (US-13) y la subida del binario de la foto desde el cliente quedan en el backlog.

## 7. Decisiones tomadas durante el review

- Implementar el lector EXIF de forma propia (sin dependencia externa) como política, por el `TreatWarningsAsErrors` y el antecedente NU1902.
- Planificar US-13 (ubicación manual sobre mapa) como continuación natural del flujo de captura del cliente.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 14 (captura de campo móvil con georreferenciación automática por EXIF, US-11). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
