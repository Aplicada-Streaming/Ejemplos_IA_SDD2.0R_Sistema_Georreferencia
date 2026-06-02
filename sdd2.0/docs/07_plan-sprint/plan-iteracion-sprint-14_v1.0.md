# Plan de Iteración — Sprint 14

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-14_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-12-08
**Fecha fin:** 2026-12-19
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 11,3 SP (S11–S13); capacidad sugerida estricta 12 SP. Se compromete US-11 (8 SP). El valor testeable —la extracción de la coordenada desde los metadatos de la foto y el armado de la petición de captura— entra al gate de cobertura; la pantalla de captura MAUI (cámara/galería) queda fuera de CI, igual que el resto de `GeoVial.Mobile`.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Permitir que el agente de campo capture una observación desde la app móvil tomando o eligiendo una foto: el cliente deriva automáticamente la coordenada de los metadatos EXIF de la foto (RN-03) y arma la petición de captura contra el backend (US-11, CU-04). Si la foto no trae ubicación, la observación se deriva a la bandeja sin georreferenciar (`OBSERVACION_SIN_GEORREFERENCIA`) a la espera de ubicación manual (US-13).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-11 | Historia | Capturar observación con georreferenciación automática (cliente móvil) | Alta (Must) | 8 | Dev móvil / Dev fullstack | Pendiente |

Total de puntos comprometidos: 8 SP. US-11 (Must de EP-03) ya está entregada en su lógica de backend (CU-04, Sprint 03); este sprint construye el frente cliente: extracción de coordenada desde EXIF y armado de la petición, más la pantalla de captura MAUI. La ubicación manual completa (US-13) queda para un sprint posterior; aquí solo se deriva la observación sin metadatos a la bandeja.

## 4. Alcance técnico

Se construye el núcleo testeable del cliente de captura, separado de la cáscara MAUI:

1. **`GeoVial.CapturaCampo`** (biblioteca `net10.0`, dentro de la solución y del gate de CI): la lógica de captura del cliente sobre la superficie pública del dominio/contratos, sin dependencias de plataforma ni paquetes externos.
   - `LectorGpsExif` (`IExtractorGpsExif`): extrae la coordenada GPS de los metadatos EXIF de una foto JPEG (segmento APP1/Exif → TIFF → GPS IFD → rationals DMS → grados decimales, RN-03). Soporta ambos órdenes de bytes (II/MM) y los hemisferios (N/S, E/W). Devuelve `null` si la foto no trae GPS o no es un JPEG/EXIF válido. Implementación propia (sin paquete externo) por el `TreatWarningsAsErrors` del repo.
   - `ArmadorCapturaCampo`: toma el binario de la foto y la referencia de archivo, extrae la coordenada y arma la `CapturarObservacionRequest` (contrato compartido); informa si quedó georreferenciada o si va a la bandeja sin georreferenciar (US-11 CA-02). Valida el rango de la coordenada (latitud −90..90, longitud −180..180): una coordenada fuera de rango se trata como sin georreferencia.
2. **Pantalla de captura en `GeoVial.Mobile`** (fuera de la solución/CI): toma o elige una foto (MediaPicker), permite un comentario, usa `ArmadorCapturaCampo` para derivar la coordenada y envía la captura al backend (o la encola offline sobre `GeoVial.Sync`). Muestra si la observación quedó georreferenciada o derivada a la bandeja.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- De una foto con metadatos EXIF de ubicación, el cliente deriva la coordenada correcta (latitud/longitud en grados decimales, con signo según hemisferio) y arma una petición georreferenciada.
- De una foto sin metadatos de ubicación, el cliente arma una petición sin coordenada, que el backend deriva a la bandeja sin georreferenciar (US-11 CA-02).
- El núcleo `GeoVial.CapturaCampo` respeta el gate de cobertura (líneas ≥ 80 %, branches ≥ 70 %); la pantalla MAUI queda fuera del gate.
- Las pruebas de extracción EXIF son herméticas: construyen el JPEG+EXIF de entrada en el propio test, sin binarios de fixture.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El parser EXIF propio no cubre variantes de cámaras reales | Media | Medio | Soportar ambos órdenes de bytes y la estructura estándar GPS IFD; ante cualquier malformación, devolver `null` (la observación va a la bandeja, no se pierde) |
| La pantalla MAUI no compila en CI (Android SDK) | Alta | Bajo | Mantener `GeoVial.Mobile` fuera de la solución/CI; el núcleo testeable vive en `GeoVial.CapturaCampo` (net10.0 puro) |
| Acoplar el cliente a un paquete con vulnerabilidades (NU1902) | Baja | Medio | Implementación propia del lector EXIF, sin dependencias externas |

## 7. Criterios de hecho del sprint

El Sprint 14 se considera completo cuando US-11 (frente cliente) está terminada según la DoD con sus pruebas verdes: el cliente deriva la coordenada de los metadatos EXIF y arma la petición de captura, derivando a la bandeja sin georreferenciar cuando no hay metadatos; `GeoVial.CapturaCampo` está dentro del gate de cobertura y la pantalla de captura vive en `GeoVial.Mobile` fuera de CI; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que avanzan | US-11 (captura con georreferenciación automática, frente cliente) |
| CU que avanzan | CU-04 (captura y georreferenciación) |
| EP | EP-03 (Captura y georreferenciación) |
| NB que avanzan | NB-02 (georreferenciación automática y confiable) |
| RN aplicadas | RN-03 (metadatos como fuente primaria), RN-02 (agrupación por radio, backend), RN-05 (solo lectura) |
| BT derivadas | BT-05, BT-06, BT-07 |
| Tests previstos | acceptance/AT-04-captura-georreferenciada |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 14 (captura de campo móvil con georreferenciación automática por EXIF, US-11). Compromete US-11 (8 SP) en su frente cliente. El núcleo `GeoVial.CapturaCampo` entra al gate; la pantalla de captura MAUI queda fuera de CI. Generado por AG-07 |
