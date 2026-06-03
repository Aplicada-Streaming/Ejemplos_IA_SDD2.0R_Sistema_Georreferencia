# Sprint Retrospectiva — Sprint 34

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-34_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- El offline parcial salió con poco riesgo: un Service Worker acotado (cache-first, FIFO 500) cubre el caso real de volver a una zona ya navegada sin red.
- Centralizar la config de teselas en `MapaTeselas` quitó duplicación y dejó el contrato del caché (`EsUrlDeTesela`) testeado al 100 % en el gate, aunque el Service Worker viva en JS.
- Manejar las respuestas opacas (no-cors) evitó el error típico de "el Service Worker no cachea las teselas".
- Se cerró una acción que venía de tres retros (S31/S32/S33) con alcance acotado.

## 2. Qué no salió bien

- El Service Worker no tiene pruebas automáticas (es JS de navegador); su verificación es por build + prueba manual, y el contrato testeado en C# es una réplica del que implementa el JS (no el mismo código).
- El offline quedó sólo en la web; el WebView móvil no usa Service Worker, así que su offline es otro esfuerzo.
- Persiste duplicación parcial del render del mapa: el JS de la web (`mapaRevision.js`) sigue con su propia URL de teselas hardcodeada; sólo el HTML móvil (C#) usa `MapaTeselas`.

## 3. Qué probar

- Unificar también `mapaRevision.js` (web) para que tome la URL/atribución desde una sola fuente (p. ej. inyectada por el front), cerrando la última duplicación.
- Para el móvil, evaluar caché de teselas nativa (HttpClient + almacenamiento) o un WebView con su propio almacenamiento, si se requiere offline en la app.
- Medir el tamaño real de la caché de teselas en uso para ajustar el límite (500) si hace falta.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Unificar la URL/atribución de teselas también en `mapaRevision.js` (web) | AG-08 (fullstack) | 2027-10-16 | Pendiente |
| Evaluar offline de teselas en el móvil (caché nativa o WebView) | AG-08 (móvil) | 2027-10-16 | Pendiente |
| Mantenimiento continuo: atender PRs/alertas de Dependabot por SLA | AG-09 (DevOps) | 2027-10-16 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 33 | Estado actual |
| --- | --- |
| Revisar el primer lote de PRs de Dependabot y ajustar agrupación/cadencia | Pendiente (al primer ciclo semanal) |
| Evaluar automatizar la regla `-rc` antes de stable como check de CI | Pendiente (se reitera) |
| Mejoras del mapa: caché de teselas (offline) y/o bundle de Leaflet en el móvil | Parcial: caché de teselas web entregada; bundle móvil pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 34 (caché de teselas offline web): Service Worker acotado + `MapaTeselas` testeado; 3 acciones nuevas y seguimiento de las de S33. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
