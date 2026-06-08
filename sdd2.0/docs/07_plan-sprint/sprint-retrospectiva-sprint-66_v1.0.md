# Sprint Retrospectiva — Sprint 66

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-66_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Refactor respaldado por tests.** Como los tres mapas ya tenían suite propio, el refactor se hizo con red: bastó que esos tests quedaran verdes sin cambios para evidenciar que el HTML observable no cambió (preservación de comportamiento).
- **Riesgo acotado.** No se tocó la API pública de los builders (`Construir`) ni sus consumidores; el cambio quedó contenido en `GeoVial.Revision`.
- **Deuda saldada en el momento justo.** Se atacó apenas el tercer HTML de mapa (S65) la hizo evidente, antes de que se multiplicara más.

## 2. Qué no salió bien

- **El andamiaje genérico tiene su límite.** Unificar la cabecera/teselas/creación del mapa fue claro; el cuerpo y el script siguen siendo propios de cada mapa (es correcto, pero el "control compartido" no elimina toda diferencia, sólo el boilerplate común).
- **Verificación on-device manual de tres mapas.** No hay aserción automatizada del render del WebView; se verificó por construcción (tests del HTML) + inspección manual de los tres mapas en el dispositivo.

## 3. Qué probar

- On-device: el mapa de *Revisión*/*Mapa* (pines clicables → carrusel), el map-pick de la bandeja/captura (barra "Confirmar") y el mapa de *Captura* (contexto + tocar para fijar) siguen funcionando igual.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Pasada de accesibilidad/UI dedicada: H-14 (live regions) + H-07 (tabs) | AG-08 | 2029-01-05 | Pendiente (reiterado) |
| Script de reset/arranque del entorno de desarrollo | AG-09 | 2029-01-05 | Pendiente (reiterado de S64) |
| Evaluar romper la racha de sprints acotados (8 seguidos de 5 SP) con una épica de alcance pleno | AG-07 | 2029-01-05 | Planificado |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 65 | Estado |
| --- | --- |
| Control de mapa compartido (unificar los 3 HTML de mapa) | **Hecho** (este sprint) |
| Pasada de accesibilidad/UI dedicada (H-14 + H-07) | Pendiente (reiterado) |
| Script de reset/arranque del entorno de desarrollo | Pendiente (reiterado) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Retro del Sprint 66 (control de mapa compartido). Bien: refactor con red de tests, riesgo acotado, deuda saldada a tiempo. Pendiente: pasada de accesibilidad y script de entorno; evaluar volver al alcance pleno tras 8 sprints acotados. Generada por AG-07 |
