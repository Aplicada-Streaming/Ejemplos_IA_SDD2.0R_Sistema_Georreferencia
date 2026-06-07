# Sprint Retrospectiva — Sprint 60

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-60_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Se cerró el hallazgo más cercano a P0.** La app pasó de no usar la ubicación (manifiesto sólo `CAMERA`) a tener "Centrar por GPS" real, base de cualquier app GIS de campo.
- **Núcleo testeable de un detalle de plataforma.** La parte "difícil de testear" (GPS) quedó como glue estático fino (`UbicacionDispositivo`), y la lógica de centrado se aisló en `ScriptUbicacionDispositivo` (gate), cubriendo el bug clásico de coma decimal por cultura.
- **Reuso del HTML de mapa.** No hubo que rehacer el mapa: el script se inyecta con `EvaluateJavaScriptAsync` sobre la variable global `mapa` que ya exponen los dos HTML (revisión y map-pick).
- **Verificado on-device de verdad.** Se confirmó el centrado contra el GPS real del dispositivo (no un mock), con la marca "Tu posición".

## 2. Qué no salió bien

- **Marca de posición estática.** La posición se fija al tocar el botón; no sigue al agente en vivo. Para captura caminando, un seguimiento continuo sería más útil (pendiente, con cuidado de batería/permiso).
- **Relogueo on-device sigue costando.** Cada verificación implica biométrico→cancelar→bloqueado→clave + manejo de capturas por adb; se reitera la acción de tener un script de login/captura reutilizable.
- **Dos superficies de mapa casi iguales.** `MapaPage` y `MapaSeleccionPuntoPage` repiten el armado del WebView + el botón de ubicación; convendría un control de mapa compartido.

## 3. Qué probar

- On-device: en el mapa y en el map-pick, "📍 Mi ubicación" recentra en la posición real con la marca azul; negar el permiso muestra el mensaje llano sin romper.
- En es-AR (coma decimal): el centrado funciona (el JS usa punto) — cubierto por test, confirmar visualmente.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Verificar on-device "Mi ubicación" en mapa y map-pick (permiso concedido/denegado) | AG-05 (QA) | 2028-10-13 | En curso (con el usuario) |
| Evaluar marca de posición "viva" (seguimiento) y control de mapa compartido | AG-08 | 2028-10-13 | Pendiente |
| Próximos P1 de la auditoría: H-01/H-03 (captura sobre el mapa), H-04 (carrusel deslizable) | Equipo | 2028-10-13 | Planificado |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 59 | Estado |
| --- | --- |
| Verificar on-device la cinta en las 4 solapas | En curso (con el usuario) |
| Layout base / Shell con cinta única | Pendiente |
| Próximo P1: H-02 | **Hecho** (este sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Retro del Sprint 60 (H-02: permiso de ubicación + "Centrar por GPS"). Bien: cierra el hallazgo casi-P0, núcleo testeable + glue fino, reuso del HTML de mapa, verificado contra GPS real. A mejorar: posición "viva", control de mapa compartido, script de login on-device. Próximos P1: H-01/H-03 y H-04. Generada por AG-07 |
