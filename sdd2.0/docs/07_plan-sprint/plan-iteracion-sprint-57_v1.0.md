# Plan de Iteración — Sprint 57

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-57_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-08-07
**Fecha fin:** 2028-08-18
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 7,0 SP (S54–S56, deprimido por el sprint de UX acotado S56=5); se vuelve al **alcance pleno (8 SP)** con una historia con núcleo nuevo. La ventana se normalizará al salir S56.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cerrar el hueco móvil de **US-15 / CU-09**: hoy el carrusel de revisión permite **comentar** y **etiquetar** un marcador (S19), y desde S55 se llega al marcador **tocando su pin en el mapa**, pero **no se puede agregar una foto** a un marcador existente desde la app. El objetivo es que el agente, parado en un marcador del carrusel, pueda **agregar una foto** —tomándola con la **cámara** o eligiéndola del **catálogo/galería**— en la **posición de ese marcador**, siempre que el relevamiento **no esté cerrado** (RN-05). Esto materializa el flujo que pidió el usuario: tocar el marcador del mapa y, sobre él, sumar fotos en esa posición y/o imágenes del dispositivo.

Hallazgo de especificación que enmarca el sprint: la spec crea el marcador **a partir de la captura** (CU-04, el marcador nace de la foto; no se "coloca" un marcador vacío en el móvil — eso es función web, `wireframes-mapa-revision-web`). "Agregar foto a un marcador" (US-15) se modela, por tanto, como **una captura en la coordenada del marcador**: la agrupación por radio del backend (RN-02) la asocia al mismo marcador, sin tocar dominio ni crear fotos huérfanas.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-15-FOTO-MOVIL | Historia | Agregar una foto (cámara o galería) a un marcador existente desde el carrusel, en la posición del marcador, si el relevamiento no está cerrado | Alta | 5 | Dev móvil (AG-08) | Cerrada |
| BT-FOTO-MARCADOR-NUCLEO | Tarea | Núcleo testeable `ArmadorFotoMarcador` (fuerza la coordenada del marcador, descarta EXIF, valida) + tests del gate | Media | 3 | Dev móvil (AG-08) | Cerrada |

Total: 8 SP (alcance pleno; historia con núcleo nuevo en el gate + glue móvil verificado on-device).

## 4. Alcance técnico

- **Núcleo (en el gate, `GeoVial.Sync`):** `ArmadorFotoMarcador.Armar(capturaId, relevamientoId, latMarcador, lonMarcador, foto, referenciaArchivo, momento) → CapturaPendiente?`. A diferencia de `ArmadorCapturaCampo` (deriva del EXIF, RN-03), **siempre** usa la coordenada del marcador: así una foto del catálogo tomada en otro lugar igual cae en *este* marcador. Devuelve `null` si el binario está vacío o la coordenada del marcador está fuera de rango. `capturaId`/`momento` son parámetros (armado determinista y testeable).
- **Reuso (sin cambios):** la cola de capturas offline (`IColaCapturas`/`ColaCapturasSqlite`, S42), el motor de subida (`MotorCapturas` → observación + binario), el handler de captura del backend (`CapturarObservacionHandler`: agrupa por radio RN-02 y **rechaza si el relevamiento está cerrado**, RN-05) y el patrón de cámara/galería de `CapturaPage` (`MediaPicker` + permiso runtime + flag `IMarcadorCaptura` para no rebotar al login al volver de la cámara, S55).
- **`RevisionPage` (glue móvil):** botón **"📷 Agregar foto al marcador"** en `EdicionPanel`; `OnAgregarFoto` ofrece cámara/galería (`DisplayActionSheetAsync`), arma con `ArmadorFotoMarcador` usando la coordenada del marcador en foco, **encola** y **drena** la cola, y **recarga** la revisión para que la foto aparezca bajo el marcador. Maneja offline (queda en cola) y errores sin tumbar la app.
- **DI:** se registra `ArmadorFotoMarcador`; `RevisionPage` recibe además `IColaCapturas`, `MotorCapturas`, `IMarcadorCaptura` (todos ya registrados para `CapturaPage`).
- **Sin cambios de backend / Application / Domain.** Comentar y etiquetar la nueva foto reusan lo ya existente (S19).

## 5. Definition of Done aplicada

- Desde el carrusel, parado en un marcador, se agrega una foto de cámara o de galería; la foto queda asociada a ese marcador (por radio, RN-02) en su posición y aparece al recargar.
- Si el relevamiento está cerrado, el backend rechaza la captura (RN-05) y la UI lo informa sin romperse.
- Sin conexión, la foto queda en la cola de capturas y se sube al reconectar (no se pierde).
- Núcleo `ArmadorFotoMarcador` cubierto por tests del gate; suite verde; cobertura DoD sin regresión.
- El MAUI compila (`net10.0-android`) y se verifica on-device.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El EXIF de una foto de galería desviaría la foto a otra coordenada | Media | Alto | `ArmadorFotoMarcador` **descarta el EXIF** y fuerza la coordenada del marcador; cubierto por test |
| Dos marcadores muy cerca → la foto cae en el vecino | Baja | Medio | Se manda la coordenada **exacta** del marcador (distancia 0): siempre es el más cercano dentro del radio |
| Muerte de proceso al abrir la cámara (rebote al login) | Media | Medio | Flag `IMarcadorCaptura` alrededor de la cámara (mismo patrón que S55) |
| Glue de UI sólo validable on-device | Media | Bajo | Núcleo en el gate + smoke-test de arranque + verificación on-device del flujo completo |

## 7. Criterios de hecho del sprint

Completo cuando: desde el carrusel se puede agregar una foto (cámara o galería) a un marcador en su posición; se respeta el cierre del relevamiento (RN-05); funciona offline (cola) y online (subida + recarga); el núcleo está cubierto en el gate con la suite verde y cobertura sin regresión; el MAUI compila y se verifica on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Pedido del usuario: tocar el marcador del mapa y, sobre él, agregar fotos en esa posición y/o imágenes del dispositivo (catálogo). Hueco confirmado: US-15 "agregar foto" no estaba en el móvil |
| CU/RN | US-15 / CU-09 (gestionar marcador: agregar fotos), CU-04 + RN-02 (agrupación por radio), RN-03 (rango de coordenada), RN-05 (relevamiento cerrado = sólo lectura) |
| Componentes | `GeoVial.Sync` (`ArmadorFotoMarcador`); `GeoVial.Mobile` (`RevisionPage`, `MauiProgram`); reuso de la cola/motor de capturas (S42) y del patrón de cámara (S55) |
| Calidad | definition-of-done §1.4; gate Domain/Application líneas ≥80 % / ramas ≥70 % |
| Tests | `ArmadorFotoMarcadorTests` (coordenada forzada, binario vacío, rango inválido, nombre por defecto) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Plan del Sprint 57 (US-15/CU-09 móvil: agregar foto a un marcador desde el carrusel, cámara o galería, en la posición del marcador, RN-05). Núcleo `ArmadorFotoMarcador` en el gate (fuerza la coordenada del marcador, descarta EXIF) + glue en `RevisionPage` que reusa la cola de capturas (S42) y el patrón de cámara (S55). Sin cambios de backend (agrupación por radio RN-02). 8 SP (alcance pleno). Generado por AG-07 |
