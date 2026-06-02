# Plan de Iteración — Sprint 15

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-15_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-12-22
**Fecha fin:** 2027-01-09
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 9,7 SP (S12–S14); capacidad sugerida estricta 11 SP. Se comprometen US-13 (5 SP) y la subida del binario de la foto desde el cliente (6 SP), 11 SP en total. El valor testeable —el armado y la validación de la ubicación manual, el armado de la subida del binario y la exposición del `FotoId` en la captura— entra al gate; las pantallas MAUI quedan fuera de CI.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cerrar el flujo de captura de campo en el cliente móvil: que el agente pueda ubicar manualmente sobre el mapa el punto de una observación cuya foto no trae GPS (US-13, CU-05) y que, tras capturar, la imagen quede alojada subiendo su binario al backend de alojamiento (BT-20/ADR-08). Para encadenar la subida, la respuesta de captura expone el `FotoId` de la foto creada.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-13 | Historia | Ubicar manualmente el punto de la observación (frente cliente) | Alta (Must) | 5 | Dev móvil / Dev fullstack | Pendiente |
| BT-20 | Tarea | Subir el binario de la foto al alojamiento tras la captura (frente cliente) | Alta | 6 | Dev móvil / Dev backend | Pendiente |

Total de puntos comprometidos: 11 SP. US-13 ya está entregada en su lógica de backend (CU-05, Sprint 03); este sprint construye el frente cliente. La subida del binario reusa el endpoint de contenido (CU-04, ADR-08) ya existente; el sprint añade la exposición del `FotoId` en la captura para encadenarla.

## 4. Alcance técnico

Se extiende el núcleo testeable del cliente de captura y se hace una pequeña ampliación de contrato en el backend:

1. **Backend — `FotoId` en la captura**: `ResultadoCaptura` y `CapturaResponse` exponen el `FotoId` de la foto creada, para que el cliente pueda subir su binario al endpoint de contenido. Cambio de contrato aditivo; cubierto por prueba de integración.
2. **`GeoVial.CapturaCampo` (en el gate)**:
   - `ArmadorUbicacionManual`: valida la coordenada manual (latitud −90..90, longitud −180..180) y arma la `UbicarManualRequest`; rechaza coordenadas fuera de rango (RN-03).
   - `ConstructorContenidoMultipart`: arma el `MultipartFormDataContent` de la subida del binario (parte `archivo` con nombre y bytes) que consume el endpoint de contenido.
3. **Pantallas MAUI (fuera de CI)**:
   - `GeoVial.Mobile` sube el binario de la foto tras una captura exitosa (usando el `FotoId` devuelto) y muestra el estado.
   - Para una observación sin georreferenciar, ofrece ubicar manualmente el punto (coordenada) y envía la `UbicarManualRequest`; la colocación sobre un mapa interactivo (control de mapas con clave de proveedor) queda como evolución de la pantalla.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- La captura devuelve el `FotoId` de la foto creada (prueba de integración).
- Subir el binario al endpoint de contenido con ese `FotoId` aloja la imagen y la foto queda con su referencia (prueba de integración).
- El cliente valida la coordenada manual y arma la petición de ubicación; una coordenada fuera de rango se rechaza.
- El núcleo `GeoVial.CapturaCampo` respeta el gate de cobertura (líneas ≥ 80 %, branches ≥ 70 %); las pantallas MAUI quedan fuera del gate.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El control de mapas de MAUI requiere clave de proveedor (Google Maps) | Media | Bajo | La ubicación manual del cliente arma y envía la coordenada; el mapa interactivo es evolución de la pantalla, no bloquea la lógica testeable |
| El cambio de contrato (`FotoId`) rompe consumidores existentes | Baja | Bajo | Cambio aditivo (campo nuevo); las pruebas de integración existentes siguen verdes |
| Las pantallas MAUI no compilan en CI (Android SDK) | Alta | Bajo | `GeoVial.Mobile` queda fuera de la solución/CI; el núcleo testeable vive en `GeoVial.CapturaCampo` |

## 7. Criterios de hecho del sprint

El Sprint 15 se considera completo cuando US-13 (frente cliente) y la subida del binario están terminadas según la DoD con sus pruebas verdes: la captura expone el `FotoId`, el cliente sube el binario y aloja la imagen, y arma/valida la ubicación manual de una observación sin georreferenciar; `GeoVial.CapturaCampo` está dentro del gate y las pantallas viven en `GeoVial.Mobile` fuera de CI; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que avanzan | US-13 (ubicación manual del punto, frente cliente) |
| BT que avanzan | BT-20 (subida del binario al alojamiento, frente cliente) |
| CU que avanzan | CU-05 (ubicación manual), CU-04 (captura/contenido) |
| EP | EP-03 (Captura y georreferenciación) |
| NB que avanzan | NB-02 (georreferenciación), NB-01 (jerarquía/autorización) |
| RN aplicadas | RN-03 (fuente de la coordenada), RN-02 (agrupación por radio), RN-05 (solo lectura), RN-01 (autorización) |
| ADRs que gobiernan | ADR-08 (alojamiento de fotos) |
| Tests previstos | acceptance/AT-05-ubicacion-manual, integración de subida de contenido |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 15 (ubicación manual del punto + subida del binario de la foto, frente cliente). Compromete US-13 (5 SP) y BT-20 cliente (6 SP). El núcleo `GeoVial.CapturaCampo` entra al gate; las pantallas MAUI quedan fuera de CI. Generado por AG-07 |
