# Sprint Retrospectiva — Sprint 30

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-30_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Cuestionar el supuesto "mapa = Google Maps" desbloqueó un ítem que arrastrábamos como bloqueado desde S25: OpenStreetMap es libre y sin clave, y resolvió el caso de uso.
- El núcleo `VistaMapa` quedó cubierto al 100 % con pocos casos (vacío, 1 marcador, varios, conflicto/conteos), y la página Blazor quedó como una cáscara fina de render: el patrón núcleo-testeable se volvió a aplicar limpio.
- La revisión ya exponía las coordenadas, así que el mapa se alimentó de datos existentes sin tocar backend ni dominio.
- Entregó la última pieza de producto del MVP en un sprint acotado y verificable.

## 2. Qué no salió bien

- El supuesto erróneo ("bloqueado por clave de proveedor") sobrevivió cuatro sprints en el backlog antes de revisarse; faltó cuestionar antes la premisa.
- El render Leaflet sólo se valida por compilación e inspección (fuera del gate); la lógica testeable se acota a `VistaMapa`, no a la interacción del mapa en sí.
- Leaflet se carga por CDN: simple para el MVP, pero introduce una dependencia de red en runtime que habría que vendorizar para escenarios offline.

## 3. Qué probar

- Llevar el mismo mapa OSM al móvil (Mapsui o WebView con Leaflet), también sin clave, para unificar la visualización geográfica entre web y app.
- Vendorizar Leaflet en `wwwroot` y evaluar caché de teselas si se requiere uso offline o de alto volumen.
- Como práctica general: revisar periódicamente los ítems marcados "bloqueado" para confirmar que el bloqueo sigue siendo real (a veces es un supuesto, no un hecho).

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Llevar el mapa OSM al móvil (Mapsui o WebView), sin clave | AG-08 (móvil) | 2027-08-21 | Pendiente |
| Revisar ítems "bloqueado" del backlog para validar que el bloqueo es real | AG-06 (Product Owner) | 2027-08-21 | Pendiente |
| Mantenimiento post-release: atender alertas Dependabot/CVE por SLA | AG-09 (DevOps) | 2027-08-21 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 29 | Estado actual |
| --- | --- |
| Institucionalizar la validación con tag preview `-rc` antes de cada stable | Pendiente (se reitera; formalizar en la guía/checklist) |
| Gestionar la clave de proveedor de mapas para el mapa interactivo | Cerrada como no aplicable: se usó OpenStreetMap (libre, sin clave); no se requiere credencial |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 30 (mapa interactivo OSM): desbloqueo del ítem al descartar el supuesto de Google Maps; 3 acciones nuevas y cierre de la acción de "clave de mapas" como no aplicable. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
