# Plan de Iteración — Sprint 40

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-40_v1.0.md
**Versión:** 1.1
**Estado:** Cerrado
**Fecha inicio:** 2027-12-14
**Fecha fin:** 2027-12-26
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S37–S39); capacidad sugerida estricta 9 SP. Se comprometen tres arreglos de la **app móvil** detectados al probarla en un dispositivo real (8 SP). Es trabajo de cáscara MAUI (fuera de CI), pero con un núcleo testeable nuevo (`ServicioSesion`) que sí entra al gate.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cerrar tres defectos reales de la **app móvil** que sólo se vieron al ejecutarla en un dispositivo Android físico (moto g42), no en la cáscara que compila en CI:

1. **La app no tiene login**: arrancaba directo en las solapas y cada página se logueaba sola con credenciales hardcodeadas (`raiz`/`GeoVial.Raiz.2026`), sin posibilidad de usar otro usuario.
2. **El mapa no se ve**: el mapa interactivo (US-21, ya existente) sólo era accesible con un botón "Ver en mapa" enterrado dentro de la solapa Revisión; no había forma directa de verlo.
3. **Sacar una foto cierra la app**: `Tomar foto` invocaba `MediaPicker.CapturePhotoAsync()` fuera de un `try/catch` y sin pedir el permiso `CAMERA`, así que la excepción no controlada tumbaba el proceso.

El defecto **(3) es el bug prioritario** (cierre de la app); (1) y (2) son brechas de UX que hacían la app inusable de punta a punta.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-MOV-FOTOCRASH | Bug | `Tomar foto` no cierra la app: permiso `CAMERA` en runtime + `try/catch` alrededor del `MediaPicker` | Alta | 2 | Dev móvil (AG-08) | En curso |
| US-40-LOGIN | Historia | Login real: `ServicioSesion` + `LoginPage`; la app arranca en login y las solapas reusan la sesión; "Cerrar sesión" | Alta | 4 | Dev móvil (AG-08) | En curso |
| US-40-MAPA | Historia | Solapa "Mapa" que carga el relevamiento y muestra el mapa interactivo de forma directa | Media | 2 | Dev móvil (AG-08) | En curso |

Total de puntos comprometidos: 8 SP. App móvil; el único núcleo nuevo con lógica testeable es `ServicioSesion` (en `GeoVial.Sync`, dentro del gate).

## 4. Alcance técnico

1. **`ServicioSesion` (núcleo testeable, `src/GeoVial.Sync/ServicioSesion.cs`)**: encapsula la sesión única del cliente. `IngresarAsync(usuario, clave)` postea a `/api/v1/auth/login`, valida credenciales vacías sin tocar la red, traduce 401/errores de red/token inválido a un `ResultadoSesion` con mensaje (no lanza), y asienta el `Bearer` en el `HttpClient` compartido (singleton). `Salir()` limpia el token; `PrimerRelevamientoAsync()` devuelve el relevamiento destino por defecto. Reemplaza el login hardcodeado que cada página hacía por su cuenta.
2. **`LoginPage` (`src/GeoVial.Mobile/LoginPage.cs`)**: primera pantalla de la app. Usa `ServicioSesion`; tras un login exitoso reemplaza la página de la ventana por el `AppShell`. Errores de credenciales/red se muestran en pantalla, no tumban la app. `App.CreateWindow` arranca en `LoginPage` (resuelta por DI).
3. **Refactor de las solapas** (`MainPage`, `CapturaPage`, `RevisionPage`): se les inyecta `ServicioSesion`, se elimina el login hardcodeado (`AutenticarYElegirRelevamientoAsync`) y usan `PrimerRelevamientoAsync()` sobre la sesión ya iniciada. `MainPage` agrega "Cerrar sesión" (vuelve al login).
4. **Solapa "Mapa" (`src/GeoVial.Mobile/MapaPage.cs`)**: nueva `ShellContent` en `AppShell`. Carga el primer relevamiento y arma el HTML con `MapaRevisionHtml` (núcleo testeable existente) en un `WebView`; en Android reusa el interceptor de teselas offline (`MapaWebViewClient` + `CacheTeselasDisco`, S38).
5. **Fix del crash de la foto** (`CapturaPage`, `AndroidManifest.xml`): se agrega `<uses-permission android:name="android.permission.CAMERA" />` (+ `uses-feature` no requerido); `OnTomarFoto`/`OnElegirFoto` se envuelven en `try/catch` y se pide el permiso `CAMERA` en runtime antes de capturar. El `FileProvider` lo aporta MAUI Essentials automáticamente.

**Hallazgos durante la verificación on-device (moto g42), corregidos en el sprint:**

6. **Teselas de OSM bloqueadas (403) en el móvil** (`MapaWebViewClient.cs`): la solapa Mapa cargaba Leaflet pero las teselas venían como imagen de bloqueo ("App is not following the tile usage policy"). Causa: el interceptor de S38 las descargaba con un `HttpClient` **sin User-Agent**, y la política de OSM exige un UA que identifique la app (la web no sufre esto porque el navegador manda el suyo). Fix: se asienta un User-Agent (`GeoVial/1.0 (…)`) en el `HttpClient` del interceptor. **Sólo Android**; la web no se toca.
7. **Volver de la cámara rebotaba al login** (`App.xaml.cs`): al lanzar la cámara, Android recrea la actividad y `CreateWindow` mostraba de nuevo el `LoginPage`. Fix: `CreateWindow` consulta `ServicioSesion.Autenticado` y, si la sesión sigue activa (el proceso sobrevivió), arranca directo en el `AppShell` en vez de rebotar al login (también cubre la rotación).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica. Criterios específicos:

- `ServicioSesion` cubierto por pruebas unitarias (login OK asienta token, credenciales vacías no llaman a la red, 401 da mensaje claro, fallo de red no lanza, `Salir` limpia, primer relevamiento). La suite .NET pasa de 338 a **344** y sigue verde; el gate (Domain/Application ≥80 % líneas, ≥70 % ramas) no se ve afectado (la lógica nueva vive en `GeoVial.Sync`, ya cubierta).
- El proyecto MAUI compila para `net10.0-android` (`android-arm64`).
- **Verificación on-device (moto g42, Android 13) — REALIZADA**: la app arranca en el login; un login válido (`raiz`) entra a las solapas; un login inválido muestra "Usuario o clave incorrectos." sin tumbar la app; "Tomar foto" pide el permiso `CAMERA`, abre la cámara, captura y vuelve a la app **sin cerrarse**; al volver de la cámara se mantiene en las solapas (no rebota al login); la solapa "Mapa" muestra las teselas reales de OpenStreetMap; "Cerrar sesión" vuelve al login.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El crash de la foto reaparezca por otra causa (FileProvider/SDK) | Media | Alto | `try/catch` envuelve toda la operación: cualquier fallo se muestra como mensaje, nunca tumba la app; permiso `CAMERA` en runtime |
| La app móvil no entra al gate de CI (cáscara MAUI) | Alta | Medio | El núcleo con lógica (`ServicioSesion`) se extrajo a `GeoVial.Sync` y se cubrió con tests; el resto se verifica on-device |
| Verificación on-device dependiente del hardware | Alta | Medio | El sprint no se cierra hasta desplegar y verificar en el moto g42; entre tanto, compilación android-arm64 + tests verdes |
| Regresión en el login compartido (token no se propaga) | Baja | Medio | `HttpClient` es singleton; el token se asienta una vez y lo reusan todas las páginas y el cliente de sync; test de `ServicioSesion` lo cubre |

## 7. Criterios de hecho del sprint

El Sprint 40 se considera completo cuando: el crash de la foto está corregido (permiso + `try/catch`), la app arranca en un login real con sesión compartida, existe una solapa "Mapa" que muestra el mapa directo, `ServicioSesion` está cubierto por tests y la suite .NET sigue verde (344), el MAUI compila para android-arm64, **se verifica en el dispositivo físico** que la app loguea, no se cierra al tomar foto y muestra el mapa, y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Prueba en dispositivo real (moto g42): 3 defectos reportados (sin login, sin mapa visible, crash al tomar foto) |
| Componentes | `GeoVial.Sync/ServicioSesion.cs`; `GeoVial.Mobile` (`LoginPage`, `MapaPage`, `App`, `MauiProgram`, `MainPage`, `CapturaPage`, `RevisionPage`, `AppShell`, `AndroidManifest.xml`) |
| Reuso | `MapaRevisionHtml`/`VistaMapa` (US-21); `MapaWebViewClient`/`CacheTeselasDisco` (S38, offline teselas) |
| Calidad | definition-of-done §1.4 |
| Tests previstos | `ServicioSesionTests` (6 casos); verificación on-device del crash, login y mapa |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Plan inicial del Sprint 40 (tres arreglos de la app móvil hallados al probarla en dispositivo real: login real, mapa visible, crash al tomar foto). Núcleo testeable `ServicioSesion` en el gate; resto verificado on-device. Compromete 8 SP. Generado por AG-07 |
| 1.1 | 2026-06-03 | Cierre del Sprint 40. Verificación on-device completada en el moto g42; durante la prueba surgieron y se corrigieron dos defectos sólo visibles en el dispositivo: teselas de OSM bloqueadas (403) por falta de User-Agent en el interceptor (§4.6) y rebote al login al volver de la cámara (§4.7). Suite .NET en 344 verdes. Estado: Cerrado |
