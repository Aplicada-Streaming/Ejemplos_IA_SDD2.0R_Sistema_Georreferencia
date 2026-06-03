# Sprint Review — Sprint 38

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-38_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-38_v1.0.md`:

> Dar **offline parcial** al mapa del móvil (paralelo al de la web del S34): cachear en disco las teselas de OpenStreetMap ya vistas para mostrarlas sin red.

Veredicto: Cumplido.

Explicación corta: el núcleo testeable `CacheTeselasDisco` (en `GeoVial.Revision`, dentro del gate) cachea las teselas en disco (clave = hash SHA-256 de la URL → archivo) con capacidad máxima y descarte FIFO de las más viejas. En el móvil, `MapaWebViewClient` (Android, `#if ANDROID`) intercepta las peticiones del WebView: para las URL de teselas (`MapaTeselas.EsUrlDeTesela`) sirve desde la caché si están, o las descarga, guarda y devuelve; el resto pasa de largo. `MapaRevisionPage` le asigna ese cliente al WebView nativo cuando está listo, con una caché en `FileSystem.CacheDirectory/teselas`. Así, una zona ya navegada se ve sin red, igual que en la web (S34) pero por un mecanismo distinto porque el Service Worker no corre en el WebView de Android. El núcleo entra al gate; el interceptor queda fuera de CI y se verificó por compilación **y por despliegue en un dispositivo real**.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-21 | Producto | El mapa del móvil cachea las teselas vistas; una zona ya navegada se ve sin red | Offline parcial también en la app |
| US-21 | Calidad | `CacheTeselasDisco` (round-trip, miss, descarte por capacidad) en el gate | Caché de teselas verificada |
| US-21 | Robustez | La app desplegada arranca sin crash en un moto g42 (Android 13) con el interceptor | Sin regresión en el dispositivo |

## 3. Feedback recibido

- El offline del móvil completa el paralelo con la web: ambas plataformas muestran las zonas ya vistas sin red, cada una con el mecanismo que su entorno permite (Service Worker en web; interceptor del WebView en Android).
- Reusar `MapaTeselas.EsUrlDeTesela` mantuvo un único contrato de "qué es una tesela cacheable" entre web (S34) y móvil.
- Verificar con un despliegue real (no sólo compilación) dio confianza de que el `WebViewClient` propio no rompe el arranque del WebView.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 337 verdes (300 unitarias + 37 de integración), +5 unitarias (`CacheTeselasDisco`). Cobertura del gate: Revision 97,0 % líneas / 96,2 % branches (`CacheTeselasDisco` 89,2 % líneas / 100 % branches; las líneas no cubiertas son los `catch (IOException)` defensivos). La pantalla MAUI queda fuera del gate, compila para `net10.0-android` (android-arm64) y la app se desplegó y arrancó sin crash en un dispositivo real.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-21-MAPA-OFFLINE-MOVIL | Tarea | Aceptada (`CacheTeselasDisco` en el gate + interceptor del WebView; offline parcial móvil; app verificada en dispositivo) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 38 se traslada. |

Backlog restante: prueba de S3 contra LocalStack; unificar la URL de teselas en el JS web; y el mantenimiento continuo (ahora con auto-merge de minor/patch).

## 7. Decisiones tomadas durante el review

- Caché de teselas en disco propia (acotada, FIFO) en vez de delegar en el caché HTTP opaco del WebView: control del tamaño y del contenido.
- Interceptar sólo las teselas de OSM (reusando `MapaTeselas`); el resto del WebView intacto.
- Verificar el interceptor por despliegue real además de compilación, dado que su comportamiento de runtime no es cubrible por el gate.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 38 (offline parcial de teselas en el móvil: `CacheTeselasDisco` + interceptor del WebView). Veredicto Cumplido, velocity 8, 0 carry-over, 337 pruebas verdes; app verificada en dispositivo real. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
