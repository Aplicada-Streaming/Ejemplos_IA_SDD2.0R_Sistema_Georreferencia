# Sprint Retrospectiva — Sprint 32

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-32_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Reusar `VistaMapa` (ya en el gate desde S30) hizo que el mapa móvil fuera casi sólo armar el HTML y mostrarlo en un WebView: poco código nuevo, mucho reuso.
- WebView + Leaflet evitó sumar una librería de mapa nativa (Mapsui) y su riesgo de warnings de vulnerabilidad bajo `TreatWarningsAsErrors`; el build móvil quedó limpio (android-arm64, 0 warnings).
- `MapaRevisionHtml` quedó 100 % cubierto con casos claros (vacío, 1, varios, atribución, vista nula); la plantilla con tokens evitó el dolor del escape de llaves en C#.
- Unificó la visualización geográfica entre web y móvil con un único enfoque OSM, sin credenciales.

## 2. Qué no salió bien

- La pantalla MAUI sólo se valida por compilación (fuera del gate); la interacción del WebView no se prueba automáticamente, sólo el HTML que lo alimenta.
- Leaflet se carga por CDN dentro del WebView: la app necesita red para el mapa (igual que para las teselas), pero no queda autocontenida; bundlear Leaflet en la app es un paso aparte.
- Persisten dos enfoques de render distintos (módulo JS en web, HTML completo en móvil) para el mismo mapa; comparten `VistaMapa` pero no el render.

## 3. Qué probar

- Evaluar bundlear Leaflet dentro de la app móvil (assets de MAUI) para no depender del CDN, junto con la caché de teselas para offline.
- Si crece la lógica del mapa, considerar unificar el armado (web y móvil) sobre `MapaRevisionHtml` para tener un solo origen del render.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Caché/Service Worker de teselas para offline real (web) y evaluar bundle de Leaflet en el móvil | AG-08 (móvil) / AG-09 (DevOps) | 2027-09-18 | Pendiente |
| Sumar las libs vendorizadas/CDN del mapa al seguimiento de versiones/seguridad | AG-09 (DevOps) | 2027-09-18 | Pendiente |
| Mantenimiento post-release: atender alertas Dependabot/CVE por SLA | AG-09 (DevOps) | 2027-09-18 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 31 | Estado actual |
| --- | --- |
| Llevar el mapa OSM al móvil (Mapsui o WebView) | Completada (WebView + Leaflet, sin clave ni dependencia nueva) |
| Sumar las libs vendorizadas de `wwwroot` al seguimiento de versiones/seguridad | Pendiente (se reitera) |
| Service Worker/caché de teselas para offline real | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 32 (mapa OSM en el móvil): reuso de `VistaMapa`, WebView+Leaflet sin dependencia nueva; 3 acciones nuevas y cierre de la acción de "mapa al móvil" de S30/S31. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
