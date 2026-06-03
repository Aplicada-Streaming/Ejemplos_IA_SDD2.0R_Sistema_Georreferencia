# Plan de Iteración — Sprint 38

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-38_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-11-16
**Fecha fin:** 2027-11-27
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S35–S37); capacidad sugerida estricta 9 SP. Se compromete el offline parcial de teselas en el móvil (8 SP). El núcleo testeable (caché de teselas en disco) entra al gate; el interceptor del WebView vive en la cáscara MAUI.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Dar **offline parcial** al mapa del móvil (paralelo al de la web del S34): cachear en disco las teselas de OpenStreetMap ya vistas para mostrarlas sin red. Como el Service Worker (web) no corre en el WebView de Android, se usa un interceptor de peticiones del WebView que sirve las teselas desde una caché en disco. Cierra la acción reiterada de las retros S32/S34/S35/S37.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-21-MAPA-OFFLINE-MOVIL | Tarea | Caché en disco de teselas + interceptor del WebView para offline parcial en el móvil | Media | 8 | Dev móvil | Pendiente |

Total de puntos comprometidos: 8 SP. Extiende el offline de teselas (US-21) al móvil; sin backend ni dominio nuevos.

## 4. Alcance técnico

1. **`GeoVial.Revision`** (en la solución y el gate):
   - `CacheTeselasDisco`: caché de teselas en disco (clave = hash de la URL → archivo) con capacidad máxima y descarte de las más viejas (FIFO por fecha de escritura). Lógica pura sobre el sistema de archivos, testeable con una carpeta temporal. Reusa `MapaTeselas.EsUrlDeTesela` para el contrato de qué es tesela.
2. **`GeoVial.Mobile`** (fuera de CI; compila/deploya android-arm64):
   - `MapaWebViewClient` (Android, `#if ANDROID`): override de `ShouldInterceptRequest`; para las URL de teselas (`MapaTeselas.EsUrlDeTesela`), sirve desde `CacheTeselasDisco` si está; si no, la descarga, la guarda y la devuelve. El resto de las peticiones pasan de largo.
   - `MapaRevisionPage`: al obtener el handler en Android, asigna el `MapaWebViewClient` con una caché en `FileSystem.CacheDirectory/teselas`.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica. Criterios específicos:

- `CacheTeselasDisco` guarda/recupera teselas por URL y acota la caché (descarta las más viejas); 100 % cubierto.
- El interceptor del WebView sirve las teselas cacheadas (offline) y descarga+cachea las nuevas; sólo intercepta teselas de OSM.
- El núcleo `GeoVial.Revision` respeta el gate de cobertura; la pantalla MAUI queda fuera del gate y compila para android-arm64.
- Verificación adicional: la app se despliega y arranca en un dispositivo real sin romper.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El `WebViewClient` propio rompe el comportamiento del WebView de MAUI | Media | Medio | Sólo intercepta teselas; el resto delega en `base`. Se valida desplegando en un dispositivo real (arranque sin crash) |
| La intercepción no se prueba con tests automáticos (código de plataforma) | Alta | Bajo | La caché (`CacheTeselasDisco`) se testea en el gate; el interceptor se verifica por compilación + deploy |
| La caché en disco crece sin límite | Media | Bajo | Capacidad máxima con descarte FIFO; cubierto por pruebas |

## 7. Criterios de hecho del sprint

El Sprint 38 se considera completo cuando `CacheTeselasDisco` está terminado con sus pruebas verdes; el WebView del móvil intercepta y cachea las teselas de OSM para offline parcial; la pantalla MAUI compila para android-arm64 y la app arranca en un dispositivo real; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US | US-21 (revisión sobre mapa) — offline parcial del mapa móvil |
| Núcleo | `GeoVial.Revision` (`CacheTeselasDisco`; reusa `MapaTeselas`) |
| Paralelo | S34 (offline de teselas en la web por Service Worker) |
| Calidad | definition-of-done §1; retros S32/S34/S35/S37 (offline de teselas en el móvil) |
| Tests previstos | unit: `CacheTeselasDisco` (round-trip, miss, capacidad/descarte, args) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 38 (offline parcial de teselas en el móvil): núcleo `CacheTeselasDisco` en el gate + interceptor `MapaWebViewClient` (Android) en la cáscara MAUI. Cierra la acción de S32/S34/S35/S37. Compromete 8 SP. Generado por AG-07 |
