# Sprint Review — Sprint 40

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-40_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-40_v1.0.md`:

> Cerrar tres defectos reales de la **app móvil** que sólo se vieron al ejecutarla en un dispositivo Android físico (moto g42), no en la cáscara que compila en CI: (1) la app no tiene login; (2) el mapa no se ve; (3) sacar una foto cierra la app.

Veredicto: Cumplido.

Explicación corta: se agregó una **sesión única** (`ServicioSesion`, en `GeoVial.Sync`, dentro del gate) que autentica una vez y asienta el token en el `HttpClient` compartido, y una `LoginPage` como primera pantalla; las tres solapas (`Sync`/`Captura`/`Revisión`) dejaron de loguearse con credenciales hardcodeadas y reusan la sesión, con "Cerrar sesión" en la barra. Se sumó una solapa **"Mapa"** que muestra el mapa interactivo del relevamiento de forma directa (reusa `MapaRevisionHtml` + el interceptor de teselas offline de S38). El **crash al tomar foto** se corrigió pidiendo el permiso `CAMERA` en runtime y envolviendo el `MediaPicker` en `try/catch` (+ permiso en el manifiesto). Todo se **verificó en el dispositivo físico**. Durante esa verificación aparecieron y se corrigieron dos defectos sólo visibles on-device: las teselas de OSM venían bloqueadas (403) por falta de User-Agent en el interceptor, y al volver de la cámara la app rebotaba al login por la recreación de la actividad.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-40-LOGIN | Funcionalidad | La app arranca en un login real; `raiz` entra a las solapas; credenciales malas muestran "Usuario o clave incorrectos." sin cerrarse; "Cerrar sesión" vuelve al login | Ahora se puede iniciar sesión como cualquier usuario |
| BT-MOV-FOTOCRASH | Bug | "Tomar foto" pide permiso de cámara, abre la cámara, captura y vuelve a la app **sin cerrarse** | El bug que tumbaba la app quedó resuelto |
| US-40-MAPA | Funcionalidad | La solapa "Mapa" muestra las teselas reales de OpenStreetMap con los marcadores | El mapa por fin es visible de un toque |
| §4.7 | Robustez | Volver de la cámara mantiene las solapas (no rebota al login); cubre también la rotación | La sesión no se pierde al usar la cámara |

## 3. Feedback recibido

- Los tres defectos los encontró el Product Owner **usando la app en su teléfono**, no la CI: la cáscara MAUI compila en CI pero no se ejecuta allí, así que login/mapa/cámara sólo se ejercitan en un dispositivo real. La verificación on-device es parte del Definition of Done de cambios de la app móvil.
- Centralizar la sesión en `ServicioSesion` no sólo arregla el login: elimina las credenciales hardcodeadas que estaban repetidas en cada página y deja la lógica cubierta por pruebas en el gate.
- El bloqueo de teselas (403) de OSM en el móvil enseñó que su política exige un **User-Agent** que identifique la app; la web no sufría esto porque el navegador manda el suyo. Mismo backend de mapa (OSM), distinta forma de pedir las teselas.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 2 (teselas OSM 403 por falta de User-Agent; rebote al login al volver de la cámara) — ambos corregidos dentro del sprint |

Pruebas: **344** (306 unitarias + 38 de integración), +6 unitarias (`ServicioSesionTests`). El MAUI compila para `net10.0-android` (`android-arm64`). La lógica nueva con valor de prueba (`ServicioSesion`) vive en `GeoVial.Sync` y está cubierta; el resto es cáscara MAUI (fuera de CI) verificada **on-device**. El gate de cobertura se mantiene.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-MOV-FOTOCRASH | Bug | Aceptado (permiso `CAMERA` en runtime + `try/catch`; verificado on-device: la cámara abre, captura y vuelve sin crash) |
| US-40-LOGIN | Historia | Aceptada (`ServicioSesion` + `LoginPage` + arranque en login + refactor de las 3 solapas + "Cerrar sesión"; verificada on-device) |
| US-40-MAPA | Historia | Aceptada (solapa "Mapa" con teselas reales de OSM; incluyó el fix del User-Agent hallado on-device) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 40 se traslada. |

Backlog restante: unificar la URL/atribución de teselas en el JS web (`mapaRevision.js`); prueba manual de offline en el dispositivo (modo avión); selección de relevamiento destino cuando hay más de uno (hoy se toma el primero); y el mantenimiento continuo (auto-merge de minor/patch).

## 7. Decisiones tomadas durante el review

- Extraer `ServicioSesion` a `GeoVial.Sync` (no a la cáscara MAUI) para que la lógica de sesión quede cubierta por pruebas dentro del gate, igual que se hizo con `MapaRevisionHtml`/`CacheTeselasDisco` en sprints de mapa.
- Setear un User-Agent en el interceptor de teselas del móvil (no falsear el de otra app) para cumplir la política de OSM; la web no se toca (su UA es el del navegador).
- Que `CreateWindow` consulte `ServicioSesion.Autenticado` para no rebotar al login al recrear la actividad (cámara/rotación), aprovechando que el token vive en el singleton compartido.
- Reafirmar la verificación **on-device** como parte del DoD de cambios de la app móvil: los tres bugs eran invisibles para la CI.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 40 (tres arreglos de la app móvil verificados on-device: login real, mapa visible, crash al tomar foto). Veredicto Cumplido, velocity 8, 0 carry-over, 344 pruebas (+6 unitarias de `ServicioSesion`); 2 defectos hallados y corregidos durante la verificación on-device. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
