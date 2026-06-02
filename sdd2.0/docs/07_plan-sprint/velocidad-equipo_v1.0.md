# Velocidad del equipo — GeoVial

**Proyecto:** GeoVial
**Documento:** velocidad-equipo_v1.0.md
**Versión:** 3.2
**Estado:** En curso
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

> Tracking actualizado al cierre del Sprint 22. La tabla §1 registra la velocity efectiva de los Sprint 00 a 22 ya ejecutados. El promedio móvil de 3 sprints se puebla desde S02 (con S00, S01, S02).

## 1. Por sprint

| Sprint | Comprometidos | Completados | Velocity | Promedio móvil 3 sprints | Notas |
| --- | --- | --- | --- | --- | --- |
| S00 | 29 | 21 | 21 | — | Walking skeleton (1 semana). BT-09, BT-01 y BT-18 cerrados; BT-07 parcial (4 tablas del slice + migración inicial), el resto carry-over al sprint de persistencia |
| S01 | 40 | 40 | 40 | — | Primer slice end-to-end (jerarquía y usuarios); 0 carry-over de lo comprometido; 60 pruebas verdes |
| S02 | 27 | 27 | 27 | 29,3 | Slice de relevamientos (CQRS ligero); 0 carry-over; 91 pruebas verdes; BT-07 avanza con Relevamiento/AsignacionAgente |
| S03 | 28 | 28 | 28 | 31,7 | Slice de captura y georreferenciación; 0 carry-over; 107 pruebas verdes; BT-07 avanza con Marcador/Observacion/Foto |
| S04 | 24 | 24 | 24 | 26,3 | Revisión sobre mapa + comentarios/etiquetas + provisión de credenciales; 0 carry-over; 130 pruebas verdes; BT-07 completa las 12 entidades backend |
| S05 | 13 | 13 | 13 | 21,7 | Detección por radio + resolución de conflictos (EP-06); compromiso acotado (2 historias) con margen reservado por la fusión de marcadores; 0 carry-over; 159 pruebas verdes |
| S06 | 13 | 13 | 13 | 16,7 | Exportación/importación del relevamiento completo en ZIP (EP-07); compromiso acotado (2 historias); 0 carry-over; 176 pruebas verdes |
| S07 | 16 | 16 | 16 | 14,0 | Alojamiento de fotos con backends configurables (BT-20) + binarios en el ZIP (cierre BT-21); compromiso ampliado por la acción de retro; 0 carry-over; 192 pruebas verdes |
| S08 | 11 | 11 | 11 | 13,3 | Pipeline de imágenes (BT-19) + cierre de EP-05 (filtrado US-23 + visor US-24); 0 carry-over; 205 pruebas verdes |
| S09 | 13 | 13 | 13 | 13,3 | Sincronización backend: consolidación last-write-wins + idempotencia + conflictos (US-18, EP-04); 0 carry-over; 216 pruebas verdes |
| S10 | 13 | 13 | 13 | 12,3 | Cierre de EP-08: consulta de auditoría con retención (US-30) + acceso a datos personales con finalidad (US-31 §5.B); 0 carry-over; 234 pruebas verdes |
| S11 | 13 | 13 | 13 | 13,0 | Spike móvil: librería GeoVial.Sync + cola SQLite (BT-15, US-17) + cáscara MAUI; 0 carry-over; 242 pruebas verdes |
| S12 | 13 | 13 | 13 | 13,0 | Captura offline (US-16) + sincronización automática por conectividad (US-19) sobre GeoVial.Sync; 0 carry-over; 250 pruebas verdes |
| S13 | 8 | 8 | 8 | 11,3 | Demo autónoma de evaluación de la librería (US-32, EP-09): backend simulado + resolución básica de conflictos; cáscara MAUI compilada para android-arm64; 0 carry-over; 256 pruebas verdes |
| S14 | 8 | 8 | 8 | 9,7 | Captura de campo móvil con georreferenciación automática por EXIF (US-11, EP-03): lector EXIF-GPS propio + armado de la captura; pantalla MAUI; 0 carry-over; 272 pruebas verdes |
| S15 | 11 | 11 | 11 | 9,0 | Ubicación manual del punto (US-13) + subida del binario de la foto (BT-20) en el cliente, con FotoId expuesto en la captura; 0 carry-over; 282 pruebas verdes |
| S16 | 8 | 8 | 8 | 9,0 | Publicación de GeoVial.Sync como paquete preview (EP-09): metadatos NuGet + MinVer + workflow/scripts + consumidor samples/01-sync-basico; 0 carry-over; 282 pruebas verdes (sprint de empaquetado, sin lógica de dominio nueva) |
| S17 | 10 | 10 | 10 | 9,7 | Revisión sobre mapa en el cliente móvil (US-21/US-22, CU-08): cliente de la API de revisión + navegación del carrusel (GeoVial.Revision); pantalla MAUI; 0 carry-over; 289 pruebas verdes |
| S18 | 8 | 8 | 8 | 8,7 | Cierre de US-26 (EP-06): resolución de ediciones en conflicto desde la web + distinción de tipos + corrección del listado; 0 carry-over; 293 pruebas verdes |
| S19 | 8 | 8 | 8 | 8,7 | Edición sobre el marcador desde el móvil (US-15, frente cliente): comentarios y etiquetas (ClienteEdicionMarcador); pantalla de revisión; 0 carry-over; 299 pruebas verdes |
| S20 | 8 | 8 | 8 | 8,0 | Endurecimiento E2E del MVP: helper de escenario completo + 3 pruebas de integración por HTTP (captura, ubicación manual, autorización); cierra deuda de E2E de S15/S18/S19; 0 carry-over; 302 pruebas verdes |
| S21 | 8 | 8 | 8 | 8,0 | Preparación del primer release stable v1.0.0 de GeoVial.Sync (EP-09): empaquetado en build verificado por prueba del contenido del .nupkg + CHANGELOG/notas/checklist; 0 carry-over; 303 pruebas verdes |
| S22 | 8 | 8 | 8 | 8,0 | Firma del paquete con cosign keyless (supply-chain §2): firma + verificación en el workflow de publicación + documentación; 0 carry-over; 303 pruebas verdes (sprint DevOps, sin lógica nueva) |

El promedio móvil de 3 sprints queda disponible en S02 (29,3 SP, sobre S00/S01/S02).

## 2. Tendencia

Veintitrés sprints registrados (S00: 21, S01: 40, S02: 27, S03: 28, S04: 24, S05: 13, S06: 13, S07: 16, S08: 11, S09: 13, S10: 13, S11: 13, S12: 13, S13: 8, S14: 8, S15: 11, S16: 8, S17: 10, S18: 8, S19: 8, S20: 8, S21: 8, S22: 8). El promedio móvil de 3 sprints se mantuvo en 8,0 SP (ventana S20–S22): cinco sprints consecutivos de 8 SP (S18–S22), una cadencia estable de cierres de alcance acotado (conflictos web, edición móvil, E2E, release prep, firma). La desviación de S22 respecto de su ventana previa es 0 %. Descontados los efectos de planificación, los sprints de módulo completo del arranque (S02: 27, S03: 28, S04: 24) siguen marcando el techo de 24–28 SP. Con el MVP funcional/verificado E2E y la librería lista, firmada y verificable, el trabajo restante es la publicación efectiva de v1.0.0 (tag) y el pulido (mapa interactivo, caché, E2E de conflictos, SBOM).

## 3. Capacidad ajustada

Con el promedio móvil de 3 sprints en 8,0 SP (S22), la capacidad sugerida estricta para S23 sería de hasta 9 SP (110 % del promedio móvil). Con la librería lista, firmada y verificable, S23 puede abordar el pulido restante (mapa interactivo con clave de proveedor, caché de fotos, E2E del ciclo de conflictos) o el SBOM firmado del paquete; la publicación efectiva de v1.0.0 es un acto del Release manager (tag). La velocity se sigue comparando sobre el equipo con frente backend + móvil; el promedio móvil de 8,0 SP refleja la cadencia de alcance acotado del frente de cierre/calidad (una historia por sprint), no un límite real de capacidad: los módulos completos del arranque marcaron 24–28 SP.

## 4. Outliers explicados

| Sprint | Velocity | Promedio móvil | Desviación | Causa |
| --- | --- | --- | --- | --- |
| S00 | 21 | — | — | Sprint inaugural y corto (1 semana, walking skeleton); factor de focus conservador por arranque del equipo. Se trata como outlier hasta consolidar el promedio móvil |
| S05 | 13 | 26,3 (ventana previa) | −50 % | Compromiso deliberadamente acotado: EP-06 aportó solo 2 historias (US-25/US-26, 13 SP) y se reservó margen respecto del tope de 29 SP por la complejidad estructural de la fusión de marcadores. No refleja una caída de capacidad sino una decisión de alcance |
| S06 | 13 | 21,7 (ventana previa) | −40 % | Segundo compromiso acotado seguido: EP-07 aportó 2 historias (US-27/US-28, 13 SP). Decisión de alcance por tamaño de épica, no de capacidad; la acción de la retro es combinar épica acotada + ítem de backlog para llenar la capacidad |
| S13 | 8 | 13,0 (ventana previa) | −38 % | Compromiso acotado: EP-09 aportó una sola historia (US-32, demo de evaluación, 8 SP). Decisión deliberada de no abrir el frente de captura de campo (EP-03) a medio refinar; no refleja una caída de capacidad |

A medida que se registren velocities, todo sprint cuyo valor se desvíe más del 30 % del promedio móvil se documenta en una fila aparte con su causa (vacaciones, incidente operativo, sprint inaugural, cambio de equipo, compromiso acotado por alcance).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Tracking de velocidad inicial con la estructura de §4.5 y los puntos comprometidos proyectados de S00 (29) y S01 (40). Velocity efectiva por registrar; promedio móvil de 3 sprints a poblar desde S03. Generado por AG-07 |
| 1.1 | 2026-06-01 | Registro de la velocity efectiva al cierre del Sprint 01: S00=21 (BT-07 parcial, resto carry-over) y S01=40 (0 carry-over). Tendencia y outlier de S00 actualizados. Por AG-07 |
| 1.2 | 2026-06-01 | Registro de la velocity efectiva del Sprint 02: S02=27 (0 carry-over). Promedio móvil de 3 sprints = 29,3; capacidad sugerida para S03 = 32 SP. Por AG-07 |
| 1.3 | 2026-06-01 | Registro de la velocity efectiva del Sprint 03: S03=28 (0 carry-over). Promedio móvil de 3 sprints (S01–S03) = 31,7; capacidad sugerida para S04 = 35 SP. Por AG-07 |
| 1.4 | 2026-06-01 | Registro de la velocity efectiva del Sprint 04: S04=24 (0 carry-over). Promedio móvil de 3 sprints (S02–S04) = 26,3; capacidad sugerida para S05 = 29 SP. Por AG-07 |
| 1.5 | 2026-06-01 | Registro de la velocity efectiva del Sprint 05: S05=13 (0 carry-over). Promedio móvil de 3 sprints (S03–S05) = 21,7; capacidad sugerida para S06 = 24 SP. S05 documentado como outlier por compromiso acotado de alcance (EP-06, 2 historias). Por AG-07 |
| 1.6 | 2026-06-01 | Registro de la velocity efectiva del Sprint 06: S06=13 (0 carry-over). Promedio móvil de 3 sprints (S04–S06) = 16,7; capacidad sugerida estricta para S07 = 18 SP, con recomendación de combinar épica acotada + ítem de backlog hacia el rango estable 24–28 SP. S06 documentado como outlier por compromiso acotado (EP-07, 2 historias). Por AG-07 |
| 1.7 | 2026-06-01 | Registro de la velocity efectiva del Sprint 07: S07=16 (0 carry-over). Promedio móvil de 3 sprints (S05–S07) = 14,0; capacidad sugerida hacia 16–24 SP para S08 (la ventana está deprimida por S05/S06; S07 ya repuntó a 16). Por AG-07 |
| 1.8 | 2026-06-01 | Registro de la velocity efectiva del Sprint 08: S08=11 (0 carry-over). Promedio móvil de 3 sprints (S06–S08) = 13,3; capacidad sugerida hacia 13–24 SP para S09 según cuántas historias de EP-04 (sincronización) entren refinadas. Por AG-07 |
| 1.9 | 2026-06-01 | Registro de la velocity efectiva del Sprint 09: S09=13 (0 carry-over). Promedio móvil de 3 sprints (S07–S09) = 13,3; capacidad sugerida estricta para S10 = 15 SP. US-18 entregada en su alcance backend; el resto de EP-04 (cliente móvil) espera la plataforma MAUI. Por AG-07 |
| 2.0 | 2026-06-01 | Registro de la velocity efectiva del Sprint 10: S10=13 (0 carry-over). Promedio móvil de 3 sprints (S08–S10) = 12,3; capacidad sugerida estricta para S11 = 14 SP. Cierra EP-08 y el backend del MVP (EP-01 a EP-08 salvo cliente móvil de EP-04); el trabajo restante requiere incorporar la plataforma MAUI. Por AG-07 |
| 2.1 | 2026-06-01 | Registro de la velocity efectiva del Sprint 11: S11=13 (0 carry-over). Promedio móvil de 3 sprints (S09–S11) = 13,0; capacidad sugerida estricta para S12 = 14 SP. Spike de plataforma móvil: librería GeoVial.Sync + cola SQLite (BT-15, US-17); habilita US-16/US-19 para S12. Por AG-07 |
| 2.2 | 2026-06-02 | Registro de la velocity efectiva del Sprint 12: S12=13 (0 carry-over). Promedio móvil de 3 sprints (S10–S12) = 13,0; capacidad sugerida estricta para S13 = 14 SP. Captura offline (US-16) + sync automática por conectividad (US-19) sobre GeoVial.Sync; resta la UI de captura de campo. Por AG-07 |
| 2.3 | 2026-06-02 | Registro de la velocity efectiva del Sprint 13: S13=8 (0 carry-over). Promedio móvil de 3 sprints (S11–S13) = 11,3; capacidad sugerida estricta para S14 = 12 SP. Demo autónoma de evaluación de la librería (US-32, EP-09); S13 documentado como outlier por compromiso acotado (una sola historia de EP-09). Android SDK provisionado. Por AG-07 |
| 2.4 | 2026-06-02 | Registro de la velocity efectiva del Sprint 14: S14=8 (0 carry-over). Promedio móvil de 3 sprints (S12–S14) = 9,7; capacidad sugerida estricta para S15 = 11 SP. Captura de campo móvil con georreferenciación automática por EXIF (US-11, EP-03, frente cliente); desviación −29 % (bajo el umbral, no outlier). Por AG-07 |
| 2.5 | 2026-06-02 | Registro de la velocity efectiva del Sprint 15: S15=11 (0 carry-over). Promedio móvil de 3 sprints (S13–S15) = 9,0; capacidad sugerida estricta para S16 = 10 SP. Ubicación manual del punto (US-13) + subida del binario de la foto (BT-20), frente cliente; desviación +13 % (dentro del umbral). Por AG-07 |
| 2.6 | 2026-06-02 | Registro de la velocity efectiva del Sprint 16: S16=8 (0 carry-over). Promedio móvil de 3 sprints (S14–S16) = 9,0; capacidad sugerida estricta para S17 = 10 SP. Publicación de GeoVial.Sync como paquete preview (EP-09, empaquetado/DevOps); desviación −11 % (dentro del umbral). Por AG-07 |
| 2.7 | 2026-06-02 | Registro de la velocity efectiva del Sprint 17: S17=10 (0 carry-over). Promedio móvil de 3 sprints (S15–S17) = 9,7; capacidad sugerida estricta para S18 = 11 SP. Revisión sobre mapa en el cliente móvil (US-21/US-22); desviación +11 % (dentro del umbral). Por AG-07 |
| 2.8 | 2026-06-02 | Registro de la velocity efectiva del Sprint 18: S18=8 (0 carry-over). Promedio móvil de 3 sprints (S16–S18) = 8,7; capacidad sugerida estricta para S19 = 10 SP. Cierre de US-26 (EP-06, resolución de ediciones en conflicto web); desviación −17 % (dentro del umbral). Por AG-07 |
| 2.9 | 2026-06-02 | Registro de la velocity efectiva del Sprint 19: S19=8 (0 carry-over). Promedio móvil de 3 sprints (S17–S19) = 8,7; capacidad sugerida estricta para S20 = 10 SP. Edición sobre el marcador desde el móvil (US-15 frente cliente); desviación −8 % (dentro del umbral). Por AG-07 |
| 3.0 | 2026-06-02 | Registro de la velocity efectiva del Sprint 20: S20=8 (0 carry-over). Promedio móvil de 3 sprints (S18–S20) = 8,0; capacidad sugerida estricta para S21 = 9 SP. Endurecimiento E2E del MVP (helper de escenario + 3 pruebas de integración por HTTP); desviación −8 % (dentro del umbral). Por AG-07 |
| 3.1 | 2026-06-02 | Registro de la velocity efectiva del Sprint 21: S21=8 (0 carry-over). Promedio móvil de 3 sprints (S19–S21) = 8,0; capacidad sugerida estricta para S22 = 9 SP. Preparación del primer release stable v1.0.0 de GeoVial.Sync (EP-09); desviación 0 %. Por AG-07 |
| 3.2 | 2026-06-02 | Registro de la velocity efectiva del Sprint 22: S22=8 (0 carry-over). Promedio móvil de 3 sprints (S20–S22) = 8,0; capacidad sugerida estricta para S23 = 9 SP. Firma del paquete con cosign keyless (supply-chain §2); desviación 0 %. Por AG-07 |
