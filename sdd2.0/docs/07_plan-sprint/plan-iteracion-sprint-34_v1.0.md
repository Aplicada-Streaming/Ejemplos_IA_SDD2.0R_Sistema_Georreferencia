# Plan de Iteración — Sprint 34

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-34_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-09-21
**Fecha fin:** 2027-10-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S31–S33); capacidad sugerida estricta 9 SP. Se compromete la caché de teselas del mapa web para offline parcial (8 SP). El núcleo testeable (config y contrato de teselas) entra al gate; el Service Worker queda fuera del gate.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Dar **offline parcial** al mapa web de la revisión: un **Service Worker** que cachea las teselas de OpenStreetMap ya visitadas, de modo que las zonas navegadas se vean sin red. Centralizar la configuración de teselas (URL, atribución, host) en un núcleo testeable y reusarla. Cierra la acción de las retros S31/S32/S33 (caché/Service Worker de teselas).

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-21-MAPA-OFFLINE | Tarea | Service Worker de caché de teselas (offline parcial web) + config de teselas centralizada | Media | 8 | Dev fullstack | Pendiente |

Total de puntos comprometidos: 8 SP. Endurece el mapa de la revisión (US-21) hacia offline parcial; sin cambios de backend ni dominio.

## 4. Alcance técnico

1. **`GeoVial.Revision`** (en la solución y el gate):
   - `MapaTeselas`: única fuente de la URL de teselas de OSM, su atribución y el host; `EsUrlDeTesela(url)` define el contrato de qué peticiones son teselas cacheables (host de OSM + `.png`). Lógica pura, testeable.
   - `MapaRevisionHtml` se refactoriza para usar `MapaTeselas.UrlPlantilla`/`Atribucion` (una sola fuente de verdad).
2. **`GeoVial.Web`** (compila en CI; Service Worker fuera del gate):
   - `wwwroot/sw-teselas.js`: Service Worker cache-first que cachea sólo las teselas de OSM (mismo contrato que `MapaTeselas`), con caché acotada (500 entradas, recorte FIFO) y manejo de respuestas opacas (no-cors).
   - Registro del Service Worker en `Components/App.razor`.
3. **Documentación**: `mapa-offline-teselas` a v1.1 (Service Worker implementado; §1/§3 + nuevo §3.1).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- `MapaTeselas` define la URL/atribución/host y `EsUrlDeTesela` distingue teselas de OSM de otras URLs; `MapaRevisionHtml` lo reusa (sin duplicar la URL).
- El Service Worker cachea las teselas OSM visitadas (cache-first), acota la caché (FIFO) y se registra en la web; las zonas ya vistas funcionan sin red.
- El núcleo `GeoVial.Revision` respeta el gate (líneas ≥ 80 %, branches ≥ 70 %); el Service Worker queda fuera del gate.
- La suite .NET permanece verde; `GeoVial.Web` compila en Release y el `publish` bundlea `sw-teselas.js`.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El Service Worker no se prueba con tests automáticos (es JS de navegador) | Alta | Bajo | El contrato (`EsUrlDeTesela`) se testea en C#; el Service Worker se verifica por build + prueba manual documentada |
| Respuestas opacas (no-cors) no se cachean | Media | Medio | Se cachean explícitamente las respuestas `opaque` (status 0) de las teselas |
| Caché de teselas creciendo sin límite | Media | Bajo | Límite de 500 entradas con recorte FIFO |
| Offline sólo en web (no móvil) | Media | Bajo | Alcance declarado web; el offline del WebView móvil queda como mejora futura |

## 7. Criterios de hecho del sprint

El Sprint 34 se considera completo cuando `MapaTeselas` y el refactor de `MapaRevisionHtml` están con sus pruebas verdes; el Service Worker cachea las teselas OSM visitadas y está registrado en la web; la suite .NET sigue verde y el `publish` bundlea el Service Worker; el documento de offline queda en v1.1; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US | US-21 (revisión sobre mapa) — offline parcial del mapa web |
| Núcleo | `GeoVial.Revision` (`MapaTeselas`; `MapaRevisionHtml` reusa) |
| DevOps | `09_devops/mapa-offline-teselas §3.1` (Service Worker) |
| Calidad | definition-of-done §1; retros S31/S32/S33 (caché de teselas) |
| Tests previstos | unit: `MapaTeselas` (config + `EsUrlDeTesela` positivos/negativos) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 34 (caché de teselas para offline parcial del mapa web): núcleo `MapaTeselas` + Service Worker `sw-teselas.js`. Cierra la acción de S31/S32/S33. Compromete 8 SP; sin lógica de dominio nueva. Generado por AG-07 |
