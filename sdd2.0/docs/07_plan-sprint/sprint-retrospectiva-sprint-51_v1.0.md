# Sprint Retrospectiva — Sprint 51

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-51_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se cerró el flujo de la bandeja prometido en S50: ahora el agente **ubica** desde la app, no sólo ve. La separación ver (S50) → ubicar (S51) demostró ser un buen corte: cada mitad entró completa en su sprint.
- El puente WebView↔app —lo más frágil y dependiente de plataforma— se encapsuló en un **núcleo testeable**: un esquema centinela + un parser puro, con un test que verifica que el HTML y el parser **acuerdan** el contrato. `GeoVial.Revision` quedó a 97,6 % líneas.
- No hizo falta tocar el backend: la ubicación manual (CU-05) ya existía; el sprint fue todo cliente + núcleo. Buen aprovechamiento de lo ya construido.
- El mapa de ubicación reusa `MapaTeselas` y el centro de los marcadores existentes: consistente con el mapa de la revisión.

## 2. Qué no salió bien

- Segundo sprint seguido con un **defecto de API obsoleta** que rompió el build MAUI por warnings-as-errors (S50: `ViewCell`; S51: `DisplayAlert`). Es un patrón: net10 marca obsoletos que en versiones previas eran válidos. Conviene compilar el MAUI temprano (no al final) o fijar una lista de APIs a evitar.
- No se verificó **on-device**: el puente se confió al núcleo cubierto + el build. La interacción real (toque en el mapa → intercepción → POST) es justo la que conviene ver en el dispositivo.
- El parser depende de que el WebView entregue la URL del esquema centinela en `Navigating`; si alguna plataforma no dispara el evento para esquemas desconocidos, el puente no funcionaría. En Android (objetivo) funciona; quedó como supuesto a verificar on-device.

## 3. Qué probar

- Verificar on-device el flujo completo: sembrar una captura sin GPS → solapa Bandeja → tocar → elegir punto → confirmar → la observación sale de la bandeja y aparece como marcador.
- Confirmar que `WebView.Navigating` intercepta el esquema centinela en el dispositivo real (Android) y que `e.Cancel` evita el "no se puede abrir la página".
- Compilar el MAUI **al inicio** de cada sprint que toque la UI, para detectar obsoletos antes (no al cierre).

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Verificar on-device el flujo de ubicación desde la app | AG-05 (QA) | 2028-06-09 | Pendiente |
| Compilar el MAUI al inicio de los sprints de UI (detectar obsoletos temprano) | AG-08 (móvil) | 2028-06-09 | Pendiente |
| Sprint de limpieza: unificar el listado de área al filtro en base (se arrastra de S47-S50) | AG-06 (backend) | 2028-06-09 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 50 | Estado actual |
| --- | --- |
| S51: ubicar desde la app (tap en mapa → CU-05) con núcleo testeable del puente | Completada (entregada en este sprint) |
| Verificar on-device la solapa Bandeja | Pendiente (se reitera, ahora junto con el flujo de ubicación) |
| Sprint de limpieza: unificar el listado de área al filtro en base | Pendiente (se reitera de S47) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 51 (ubicar desde la app): el puente WebView↔app se encapsuló en un núcleo testeable (esquema centinela + parser). Patrón detectado: obsoletos de net10 rompen el build MAUI al cierre (acción: compilar temprano). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
