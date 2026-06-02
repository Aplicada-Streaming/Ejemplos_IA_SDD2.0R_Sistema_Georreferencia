# Sprint Review — Sprint 17

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-17_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-17_v1.0.md`:

> Permitir que el jefe de área revise un relevamiento sobre el mapa desde la app móvil: ver los marcadores con sus fotos y comentarios (US-21) y recorrerlos como un carrusel, navegando entre marcadores y entre las fotos de cada marcador (US-22), consumiendo la API de revisión ya existente (CU-08).

Veredicto: Cumplido.

Explicación corta: el nuevo núcleo `GeoVial.Revision` aporta `ClienteRevisionHttp`, que consume `GET /relevamientos/{id}/revision` (con filtro opcional por etiquetas) y parsea el `RevisionRelevamientoDto`, y `NavegadorRevision`, que recorre los marcadores y sus fotos como un carrusel circular (US-22), reiniciando la foto en foco al cambiar de marcador y tolerando relevamientos sin marcadores o marcadores sin fotos. La pantalla de revisión en `GeoVial.Mobile` carga la revisión, muestra el marcador actual (coordenada, indicador de conflicto), su foto (descargada del endpoint de contenido) y comentarios, y recorre el carrusel con los controles de navegación, fuera de CI.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-21 | Historia | El cliente obtiene la revisión del relevamiento (marcadores con fotos y comentarios) y la muestra | Revisión disponible en el móvil |
| US-22 | Historia | Recorrer el carrusel: navegar entre marcadores y entre las fotos de cada marcador, de forma circular | Navegación fluida |
| US-22 | Historia | Un relevamiento sin marcadores o un marcador sin fotos no rompen la navegación | Robustez |

## 3. Feedback recibido

- La revisión sobre mapa queda disponible en el cliente móvil sobre el backend ya entregado en el Sprint 04.
- Separar la navegación (núcleo testeable) de la pantalla permitió cubrir el carrusel al 100 % de branches sin atar la verificación al empaquetado MAUI.
- La colocación de los marcadores sobre un mapa interactivo (control de mapas con clave de proveedor) queda como evolución de la pantalla; el recorrido por coordenada ya está entregado.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 10 |
| Puntos completados | 10 |
| Velocity efectiva | 10 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 289 verdes (259 unitarias + 30 de integración), +7 respecto del Sprint 16 (navegación del carrusel y cliente HTTP de revisión). Cobertura del núcleo `GeoVial.Revision`: 96,4 % líneas / 100 % branches (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-21 | Historia | Aceptada (frente cliente: cliente de la API de revisión; la pantalla MAUI compila fuera de CI) |
| US-22 | Historia | Aceptada (navegación del carrusel de marcadores y fotos; pantalla con controles fuera de CI) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 17 se traslada. |

El mapa interactivo (control de mapas) para situar los marcadores y la edición sobre el marcador desde el móvil (comentarios/etiquetas) quedan en el backlog.

## 7. Decisiones tomadas durante el review

- Mantener el recorrido del carrusel por coordenada en la pantalla actual y planificar el mapa interactivo como evolución.
- Reusar el endpoint de contenido para mostrar la foto del marcador en foco, descargándola bajo demanda.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 17 (revisión sobre mapa y carrusel en el cliente móvil, US-21/US-22). Veredicto Cumplido, velocity 10, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
