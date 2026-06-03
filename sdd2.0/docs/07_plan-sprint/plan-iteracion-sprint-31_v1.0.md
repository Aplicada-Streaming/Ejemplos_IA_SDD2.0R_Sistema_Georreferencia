# Plan de Iteración — Sprint 31

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-31_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-08-10
**Fecha fin:** 2027-08-21
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S28–S30); capacidad sugerida estricta 9 SP. Se compromete vendorizar Leaflet y documentar la estrategia de teselas/offline del mapa (8 SP). Es trabajo de frontend/DevOps: sin lógica de dominio nueva, el gate de cobertura se mantiene.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Quitar la dependencia de CDN del mapa de la revisión: servir **Leaflet desde `wwwroot`** (vendorizado) en lugar de unpkg, de modo que la librería del mapa no requiera Internet en runtime; y documentar la estrategia de **teselas/offline** (las teselas de OSM siguen viniendo por red, con opciones de caché/proxy para alto volumen o uso desconectado). Cierra la acción de la retro del Sprint 30.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-MAPA-OFFLINE | Tarea | Vendorizar Leaflet en `wwwroot` y documentar la estrategia de teselas/offline | Media | 8 | Dev fullstack / DevOps | Pendiente |

Total de puntos comprometidos: 8 SP. No agrega historias nuevas: endurece el mapa entregado en S30 (US-21) quitando la dependencia de red para la librería y dejando documentada la estrategia de teselas. Sin cambios de backend ni dominio.

## 4. Alcance técnico

1. **Vendorizado de Leaflet** (`GeoVial.Web/wwwroot/lib/leaflet/`): `leaflet.js`, `leaflet.css` y `images/` (íconos de marcador, sombra, capas) de Leaflet 1.9.4. `App.razor` referencia los archivos locales (sin `unpkg`/integrity). El módulo `mapaRevision.js` fija `L.Icon.Default.imagePath` a la carpeta vendorizada para que los íconos resuelvan sin autodetección.
2. **Estrategia de teselas/offline** (`09_devops/mapa-offline-teselas`): documenta qué queda offline (la librería Leaflet) y qué no (las teselas, que vienen por red de OSM); opciones para offline/alto volumen (caché HTTP/Service Worker, proxy/caché de teselas, o un área acotada pre-bundleada); degradación elegante si no hay red (Leaflet carga, las teselas no, los marcadores se ubican por coordenada).
3. **Verificación**: `GeoVial.Web` compila en Release y el `dotnet publish` incluye los assets vendorizados en `wwwroot/lib/leaflet/` (comprimidos por el pipeline de assets estáticos).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- `App.razor` referencia Leaflet desde `wwwroot` (sin CDN); el `publish` bundlea `leaflet.js`/`.css`/`images`.
- Los íconos de marcador resuelven desde la carpeta vendorizada (`imagePath` fijado).
- La estrategia de teselas/offline queda documentada (qué es offline, qué requiere red, opciones).
- La suite .NET (320 pruebas) permanece verde; el gate de cobertura no se ve afectado (trabajo de assets/frontend, sin lógica nueva).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Íconos de marcador rotos al vendorizar (Leaflet autodetecta la ruta) | Media | Bajo | `L.Icon.Default.imagePath` fijado explícitamente a `lib/leaflet/images/` |
| Confundir "Leaflet offline" con "mapa offline" | Media | Bajo | El documento separa explícitamente la librería (offline) de las teselas (red); offline real de teselas requiere caché/bundle |
| Versión de Leaflet desactualizada con el tiempo | Baja | Bajo | Versión fijada (1.9.4) y registrada; actualización por PR cuando corresponda |

## 7. Criterios de hecho del sprint

El Sprint 31 se considera completo cuando Leaflet se sirve desde `wwwroot` (sin CDN) y el `publish` lo bundlea, los íconos de marcador resuelven desde la carpeta vendorizada, la estrategia de teselas/offline queda documentada, la suite .NET sigue verde, y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US | US-21 (revisión sobre mapa) — endurece el mapa de S30 |
| Componente | `GeoVial.Web/wwwroot/lib/leaflet`, `App.razor`, `mapaRevision.js` |
| DevOps | `09_devops/mapa-offline-teselas` (estrategia de teselas/offline) |
| Calidad | definition-of-done §1; retro S30 (vendorizar Leaflet, evaluar caché de teselas) |
| Tests previstos | sin pruebas .NET nuevas; verificación = build + publish con assets bundleados |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 31 (vendorizar Leaflet en `wwwroot` y documentar la estrategia de teselas/offline del mapa de la revisión). Cierra la acción de la retro S30. Compromete 8 SP; sin lógica de dominio nueva. Generado por AG-07 |
