# Sprint Retrospectiva — Sprint 31

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-31_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Vendorizar Leaflet fue de bajo riesgo y alto rédito: el front quedó autocontenido para la librería del mapa, sin punto de fallo por CDN.
- El pipeline de static web assets de Blazor bundleó y comprimió los archivos sin configuración extra; el `publish` quedó verificado.
- Fijar `L.Icon.Default.imagePath` evitó de raíz el clásico bug de íconos rotos al vendorizar Leaflet.
- El documento dejó clara la distinción librería vs. teselas, que era justo la confusión que originó la duda de "offline".

## 2. Qué no salió bien

- "Offline" sigue siendo parcial: las teselas requieren red; el offline real de teselas (caché/bundle) es un esfuerzo aparte que se documentó pero no se implementó.
- La versión de Leaflet queda fijada manualmente; sin un chequeo, puede quedar atrás respecto de parches de seguridad upstream.

## 3. Qué probar

- Si surge un requisito de offline real, implementar un Service Worker que cachee las teselas visitadas (cubre zonas ya navegadas) y evaluar un proxy/caché de teselas para producción.
- Sumar Leaflet (y otras libs de `wwwroot`) al alcance de Dependabot/seguimiento de versiones para no quedar atrás en parches.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Sumar las libs vendorizadas de `wwwroot` al seguimiento de versiones/seguridad | AG-09 (DevOps) | 2027-09-04 | Pendiente |
| Llevar el mapa OSM al móvil (Mapsui o WebView), sin clave | AG-08 (móvil) | 2027-09-04 | Pendiente |
| Mantenimiento post-release: atender alertas Dependabot/CVE por SLA | AG-09 (DevOps) | 2027-09-04 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 30 | Estado actual |
| --- | --- |
| Vendorizar Leaflet en `wwwroot` y evaluar caché de teselas | Completada (Leaflet vendorizado; caché de teselas documentada como opción, no implementada) |
| Llevar el mapa OSM al móvil (Mapsui o WebView) | Pendiente (se reitera) |
| Revisar ítems "bloqueado" del backlog para validar que el bloqueo es real | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 31 (vendorizado de Leaflet + offline): librería autocontenida, distinción librería/teselas documentada; 3 acciones nuevas y cierre de la acción de vendorizado de S30. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
