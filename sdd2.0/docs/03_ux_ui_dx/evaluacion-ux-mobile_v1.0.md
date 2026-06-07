# Evaluación UX — App móvil GeoVial (MAUI)

**Proyecto:** GeoVial
**Documento:** evaluacion-ux-mobile_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-06
**Autor:** AG-03 (Especialista UX/UI móvil + Accessibility Specialist)
**Variante:** UX/UI
**Trazabilidad upstream:** experiencia-de-uso_v1.0.md, wireframes-captura-movil_v1.0.md, wireframes-marcador-carrusel_v1.0.md, wireframes-login-relogueo_v1.0.md, geovial-requerimientos §5/§6
**Dispositivo:** moto g42 (Android 13, 1080×2400), `com.companyname.geovial.mobile`, build Debug `net10.0-android` (arm64)

---

## 0. Nota de plataforma (corrige el supuesto del encargo)

El encargo asume un frontend **Blazor/MudBlazor** (`.razor`). **La app móvil de GeoVial es .NET MAUI con XAML**; el mapa es un **WebView con Leaflet+OSM**. Por lo tanto las mejoras de presentación tocan archivos `*.xaml` / `*.xaml.cs`, no `.razor`. El stack MudBlazor aplica al **frontend web**, fuera del alcance de esta auditoría móvil. `(Supuesto a validar)` resuelto: se audita la app MAUI.

## 1. Resumen ejecutivo

La app cumple la **función** del MVP de campo (login, selección de relevamiento, captura offline encolada, revisión con carrusel, bandeja sin georreferenciar, mapa OSM, relogueo con patrón/huella), pero su **arquitectura de UX diverge del wireframe de captura** y de varios patrones de la industria GIS de campo:

- La **captura no ocurre sobre el mapa**: es un formulario en una pestaña separada del mapa, sin posición del agente ni "Centrar por GPS". El permiso de ubicación **ni siquiera se solicita** (el manifiesto sólo pide `CAMERA`).
- **No hay cinta de estado de conexión persistente**; el estado offline/pendientes sólo aparece en la pestaña *Sync*, no en la pantalla crítica de *Captura*.
- El **carrusel se navega con botones** (◀ Marcador / ◀ Foto), no con gesto deslizable como pide la spec §6.
- Un **mensaje técnico crudo** (`401 Unauthorized`) llegaba al usuario, violando experiencia-de-uso §8.

Se **implementaron 3 mejoras seguras** (presentación, sin tocar dominio/sync): mensajes en lenguaje llano sin código crudo (§8), título del hub reducido para priorizar acciones, y texto de botón sin truncar. Lo estructural (captura-sobre-mapa, centrar-GPS, carrusel deslizable, cinta persistente) queda **propuesto** porque toca comportamiento/arquitectura.

**Conteo:** 3 P1 implementables/implementados · 4 P1 propuestos (estructurales) · 6 P2. Cobertura: 5 superficies principales + 4 estados (login, bloqueado, offline, error-401, vacío) capturados; 3 estados/flujos NO ALCANZADOS (ver §3).

## 2. Rúbrica aplicada

Nielsen + Shneiderman + WCAG 2.2 AA + patrones GIS de campo (Google Maps / ArcGIS Field Maps / QField / Fulcrum):

- Visibilidad del estado (conexión/sync **persistente**), correspondencia con el dominio, control y libertad, prevención de errores, reconocer antes que recordar, minimalismo (una acción primaria en captura).
- Ley de Fitts (disparador grande y al pulgar), Ley de Hick (una decisión por pantalla), Ley de Jakob (mapa+pines+carrusel convencionales), Ley de Doherty (feedback < 0,5 s).
- WCAG 2.2 AA: contraste 4.5:1 texto / 3:1 componentes, foco visible, nombres accesibles, anuncios por región en vivo, no depender sólo del color, **tamaño de objetivo ≥ 48 dp**.
- Industria GIS: centrar-GPS al pulgar (abajo-derecha), indicador persistente de offline y de señal GPS, affordance de pin arrastrable, clustering con muchos pines, legibilidad a pleno sol.

## 3. Log del recorrido

| # | Superficie / estado | Captura | Resultado |
| --- | --- | --- | --- |
| 1 | Hub *Sync* (sesión restaurada, token viejo) | `01-login-inicial.png` | Alcanzada; afloró error de carga |
| 2 | Hub *Sync* — error carga relevamientos (401 crudo) | `02-picker-relevamiento.png` | Alcanzada (estado **error**) |
| 3 | Login | `03-login.png` | Alcanzada |
| 4 | Login — campos | `04-post-login.png` | Alcanzada |
| 5 | Hub *Sync* (logueado, online) | `06-main.png` | Alcanzada (estado **con datos**) |
| 6 | Picker de relevamiento (diálogo) | `07-picker-abierto.png` | Alcanzada |
| 7 | *Captura* | `08-captura.png` | Alcanzada |
| 8 | *Mapa* (Leaflet+OSM) | `09-mapa.png` | Alcanzada |
| 9 | *Revisión* (vacío) | `10`,`11-revision*.png` | Alcanzada (estado **vacío**) |
| 10 | *Bandeja* (vacío) | `12-bandeja.png` | Alcanzada (estado **vacío**) |
| 11 | Hub *Sync* — **offline** (modo avión) | `13-sync-offline.png` | Alcanzada (estado **sin conexión**) |
| 12 | *Bloqueado* (relogueo, biométrico cancelado) | `15-bloqueado.png` | Alcanzada (estado **bloqueado**) |
| 13 | Hub y *Captura* — **después** de las mejoras | `14`,`16-after-*.png` | Alcanzada |

**NO ALCANZADAS / no ejecutadas (con motivo):**
- **Estado "sin GPS"**: N/A — la app **no usa ubicación** (no declara ni solicita `ACCESS_FINE/COARSE_LOCATION`), así que no hay estado de GPS que forzar. El propio hecho es un hallazgo (H-02).
- **Estado "permiso de cámara denegado"**: no ejecutado por presupuesto de tiempo; queda propuesto como prueba de 08.
- **Crawl BFS automático (≤400 taps)**: se ejecutó un recorrido **dirigido** de las 5 superficies + estados clave en lugar del crawl exhaustivo, priorizando cobertura de superficies y análisis visual; el mapa es un WebView (nodo único para `uiautomator`), navegado por coordenada.
- **Flujo "tomar foto real → mover pin → sincronización en curso"**: no ejecutado end-to-end (requiere cámara real y un relevamiento con marcadores; *Puente Río 12* estaba sin marcadores). `(Supuesto a validar)`.

> Nota de entorno: la sesión persistida traía un token del backend previo (reinicio de InMemory) → primer arranque mostró `401`. Es **artefacto de dev**, no defecto de producto; pero el **mensaje crudo** sí es defecto (H-06).

## 4. Hallazgos (spec + industria)

| ID | Superficie | Spec / industria dice | Observado | Heurística/criterio | Sev |
|----|------------|------------------------|-----------|---------------------|-----|
| H-01 | Captura | wireframe-captura-movil §2: la captura ocurre **sobre el mapa** (posición + pines + disparador + bandeja) | *Captura* es un formulario sin mapa; el mapa es **otra pestaña** | Jakob; minimalismo; flujo §3.2 | **P1** |
| H-02 | Mapa/Captura | §6 + wireframe: "Centrar por GPS" y "mapa con tu posición" | No hay centrar-GPS ni posición; **el permiso de ubicación no se solicita** (manifest sólo `CAMERA`) | Visibilidad; Fitts; GIS de campo | **P1** (raya P0) |
| H-03 | Mapa | §6/industria: mapa centrado en los datos; centrar-GPS al **pulgar** (abajo-derecha) | Mapa abre en **toda Argentina**; zoom de Leaflet arriba-izquierda; sin clustering | Fitts; Doherty; GIS | **P1** |
| H-04 | Revisión | §6: **carrusel deslizable** que al terminar pasa al marcador siguiente/anterior | Navegación por **botones** ◀ Marcador / ◀ Foto; sin gesto | Jakob; spec §6 | **P1** |
| H-05 | Todas | experiencia §4.1: **indicador de conexión persistente** | Estado offline/pendientes sólo en pestaña *Sync*; *Captura* no muestra conexión | Visibilidad del estado | **P1** |
| H-06 | Hub | experiencia §8: nunca mostrar el código crudo al usuario | "Response status code does not indicate success: **401 (Unauthorized)**" en pantalla | Recuperación de errores §8 | **P1** → *implementado* |
| H-07 | Tab bar | Etiquetas legibles | 5 pestañas no entran: "**Captu… / Revis… / Band…**" truncadas | Estética; reconocer | P2 |
| H-08 | Captura | Etiquetas completas | Botón "**Enviar captura (y subir la**" truncado | Estética | P2 → *implementado* |
| H-09 | Hub | Minimalismo; jerarquía | Título estilo *Headline* gigante empuja el selector y el estado fuera del viewport inicial | Hick; minimalismo | P2 → *implementado* |
| H-10 | Mapa/Revisión/Bandeja | Doherty: carga al entrar | Requieren tocar "**Recargar**"/"**Cargar revisión**" manual al entrar a la pestaña | Visibilidad; eficiencia | P2 |
| H-11 | Captura | Fitts: disparador grande y central | "Tomar foto" es un botón apilado normal, no un objetivo central destacado | Fitts | P2 |
| H-12 | Toolbar | Prevención de errores | "Cerrar sesión (pedirá clave)" muy prominente en la barra → toque accidental | Prevención de errores | P2 |
| H-13 | Login | WCAG 1.4.3 contraste | Placeholders gris claro ("Usuario"/"Clave") posiblemente < 4.5:1 | WCAG 2.2 AA | P2 `(Supuesto a validar)` |
| H-14 | Sync/anuncios | WCAG 4.1.3 mensajes de estado | Estado de sync/offline como `Label`, sin región en vivo (`role=alert`) verificable | WCAG 2.2 AA | P2 `(Supuesto a validar)` |

**Positivos:** estados **vacío** con copy correcto ("El relevamiento no tiene marcadores", "Sin observaciones por ubicar…"); estado **bloqueado** bien resuelto (ofrece patrón/huella **o** usuario+clave); el indicador offline del hub **sí** refleja "Sin conexión · N pendientes"; microcopy de captura explica RN-03; 👁 para ver la clave en login.

## 5. Backlog priorizado

**P1 — propuestos (tocan comportamiento/arquitectura; no auto-implementados):**
1. **H-01/H-03** Fusionar *Captura* y *Mapa* en una sola superficie de captura sobre el mapa (posición + pines + disparador + bandeja), centrada en los marcadores. Archivos: `CapturaPage.xaml(.cs)`, `MapaPage`, `MapaRevisionHtml`.
2. **H-02** Declarar `ACCESS_FINE/COARSE_LOCATION`, pedir el permiso y agregar **"Centrar por GPS"** (Geolocation) al pulgar. Archivos: `Platforms/Android/AndroidManifest.xml`, `MauiProgram`, página de mapa/captura.
3. **H-04** Carrusel **deslizable** (swipe) que cruza al marcador siguiente/anterior. Archivo: `RevisionPage.xaml(.cs)` (+ `NavegadorRevision` ya soporta el índice).
4. **H-05** **Cinta de estado de conexión persistente** (reusar `MonitorSincronizacion`) visible en *Captura* (y, idealmente, global). Archivos: `CapturaPage.xaml`, control compartido.

**P2 — pulido:**
- H-10 cargar contenido al entrar a la pestaña (quitar "Recargar"/"Cargar" manual).
- H-11 disparador de foto como objetivo central grande.
- H-12 mover "Cerrar sesión" a un overflow.
- H-13/H-14 contraste de placeholders y anuncios por región en vivo.
- H-07 acortar/abreviar etiquetas o reducir el nº de pestañas para que no trunquen.

## 6. Antes / después implementado (mejoras seguras)

Sólo presentación/feedback; no tocan dominio, datos ni sincronización. Redeploy verificado on-device.

| Hallazgo | Antes | Después | Archivo |
|----------|-------|---------|---------|
| **H-06** mensaje técnico crudo | "Response status code does not indicate success: 401 (Unauthorized)" (`02-picker-relevamiento.png`) | "Tu sesión expiró. Cerrá sesión y volvé a ingresar con conexión." / "No se pudieron cargar los relevamientos. Revisá tu conexión…" (verificado por código) | `MainPage.xaml.cs` |
| **H-06b** error de sync crudo | "No se pudo sincronizar: {ex.Message}" | "No se pudo sincronizar. No se perdió nada; reintentá cuando tengas señal." | `MainPage.xaml.cs` |
| **H-09** título gigante | `06-main.png` (título *Headline* empuja acciones) | `14-after-mainpage.png` (FontSize 22; selector + estado + acción en el viewport) | `MainPage.xaml` |
| **H-08** botón truncado | `08-captura.png` ("Enviar captura (y subir la") | `16-after-captura.png` ("Enviar captura") | `CapturaPage.xaml` |

## 7. Trazabilidad (CU/wireframe → hallazgo → mejora → test 08)

| CU / wireframe | Hallazgo | Mejora | Test sugerido (08) |
|----------------|----------|--------|--------------------|
| CU-04 / wireframes-captura-movil §2,§4 | H-01, H-03, H-11 | Captura-sobre-mapa + centrar-datos (propuesto) | UI: la captura muestra mapa+posición+disparador |
| CU-04 / §6, RN-03 | H-02 | Permiso de ubicación + centrar-GPS (propuesto) | Permiso solicitado; centrar-GPS recentra |
| CU-09 / wireframes-marcador-carrusel §4,§6 | H-04 | Carrusel deslizable (propuesto) | Gesto lateral avanza foto y cruza de marcador |
| experiencia §4.1 | H-05 | Cinta de conexión persistente (propuesto) | La cinta es visible en Captura offline |
| experiencia §8 | H-06 | Mensajes llanos sin código (implementado) | El 401/red no muestra texto técnico |
| Estética/legibilidad | H-07,H-08,H-09 | Copy/tamaños (H-08/H-09 implementados) | Snapshot sin truncamiento; jerarquía del hub |

## 8. Supuestos

- `(Supuesto a validar)` Contraste exacto de placeholders y tokens de color: no medido pixel-a-pixel; estimado por inspección visual (H-13).
- `(Supuesto a validar)` Anuncios por región en vivo / `role=alert`: no verificable sólo por captura; requiere inspección de accesibilidad nativa (H-14).
- `(Supuesto a validar)` Flujo de cámara real, mover pin y sincronización en curso: no ejecutado end-to-end.
- Credencial usada: `campo1` (DATOS-DE-PRUEBA.md), agente de campo de prueba del repo (no había `GEOVIAL_TEST_USER` en entorno/appsettings/.env).

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Auditoría UX autónoma de la app móvil MAUI (recorrido por adb, 16 capturas + dumps). 14 hallazgos (4 P1 estructurales propuestos, H-06 P1 implementado, 6 P2); 3 mejoras seguras implementadas con antes/después y redeploy on-device. Generado por AG-03 |
