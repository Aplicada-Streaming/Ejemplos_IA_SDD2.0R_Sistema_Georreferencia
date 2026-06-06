# Sprint Retrospectiva — Sprint 56

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-56_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Reuso real: el map-pick de S51 (núcleo puro ya en el gate) se aplicó a la captura **sin tocar lógica probada**; el sprint fue glue de UI. El haber construido S51 como núcleo testeable + página delgada pagó al reusarlo.
- La consolidación (separar "elegir punto" de "postear") dejó una `MapaSeleccionPuntoPage` reusable y **eliminó** una página duplicada (`MapaUbicacionPage`); menos código que mantener y un solo lugar para futuros arreglos del WebView.
- Estimación honesta: 5 SP por ser glue/UX sin núcleo nuevo, en vez de inflarlo a 8.
- Se conservó la carga manual: el agente elige mapa o tipeo según el caso (foto del catálogo de un lugar lejano).

## 2. Qué no salió bien

- **Sin tests nuevos en el gate:** el sprint reusa núcleo ya cubierto, así que el grueso (las páginas) es glue no cubierto. Es correcto, pero acumula superficie de UI que sólo se valida on-device.
- El **centro del mapa** en la captura es el por defecto (Argentina); el agente tiene que hacer zoom hasta la obra. Centrar por la ubicación del dispositivo sería más cómodo pero implica pedir el permiso de Geolocation (hoy no se usa).
- Tres sprints seguidos (S54-S56) salieron de la verificación on-device: es señal de que conviene una pasada de QA en dispositivo más temprana y sistemática, no reactiva.

## 3. Qué probar

- On-device: foto de galería sin GPS → elegir en el mapa → mover el marcador → confirmar → la captura se encola con la coordenada (no a la bandeja); y la carga manual como alternativa.
- Regresión de la bandeja: ubicar una observación con la página consolidada.
- Evaluar centrar el mapa de la captura en la ubicación del dispositivo (Geolocation) si el agente lo pide.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Verificar on-device el map-pick de la captura + regresión de la bandeja | AG-05 (QA) | 2028-08-18 | En curso (con el usuario) |
| Evaluar centrar el mapa por GPS del dispositivo (Geolocation) | AG-08 (móvil) | 2028-08-18 | Pendiente (mejora) |
| Pasada de QA on-device sistemática (no reactiva) tras cada sprint de UI | Equipo | 2028-08-18 | Acordado |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 55 | Estado actual |
| --- | --- |
| Confirmar on-device el modelo de relogueo + tap del pin | En curso (con el usuario; el tap del pin se arregló — bug del WebViewClient) |
| Consolidar los dos caminos de re-entrada (`CoordinadorReingreso` online) | Pendiente |
| Evaluar refresh token para jornadas offline largas | Pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 56 (UX: ubicar la captura en el mapa): buen reuso del núcleo de S51 + consolidación que elimina duplicación. Pendiente: centrar por GPS y una pasada de QA on-device sistemática. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
