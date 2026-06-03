# Sprint Retrospectiva — Sprint 40

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-40_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Los tres defectos reportados por el Product Owner usando la app en su teléfono (sin login, mapa invisible, crash al tomar foto) quedaron resueltos y **verificados en el dispositivo físico** (moto g42), capturas de pantalla incluidas en la verificación.
- La sesión se centralizó en `ServicioSesion` (en `GeoVial.Sync`, dentro del gate): se eliminó el login hardcodeado (`raiz`/`GeoVial.Raiz.2026`) que estaba duplicado en las tres páginas, y la lógica quedó cubierta con 6 pruebas unitarias (login OK asienta el token, credenciales vacías no tocan la red, 401/red en falla dan mensaje sin tumbar la app, `Salir` limpia, primer relevamiento).
- La solapa "Mapa" reusó `MapaRevisionHtml` y el interceptor offline de S38 sin reescribir nada; el mapa quedó accesible de un toque.
- La verificación on-device afloró dos bugs invisibles para la CI y se corrigieron en el mismo sprint: el 403 de OSM por falta de User-Agent y el rebote al login al volver de la cámara. Probar en el dispositivo pagó de inmediato.

## 2. Qué no salió bien

- **La cáscara MAUI compila en CI pero no se ejecuta allí**: login, mapa y cámara sólo se ejercitan en un dispositivo real. Tres bugs llegaron al Product Owner porque los sprints móviles previos se daban por "verificados" con la compilación android-arm64, no con una corrida on-device. Es el límite estructural de tener el móvil fuera del gate.
- El crash de la foto era evitable: el `MediaPicker` se llamaba fuera de `try/catch` y sin pedir el permiso `CAMERA`. Un patrón defensivo (envolver toda E/S de plataforma + pedir permisos) hubiera evitado el cierre desde el inicio.
- El interceptor de teselas de S38 nunca había servido una tesela real en el dispositivo (la caché empezaba vacía y OSM la bloqueaba); el bug del User-Agent estaba latente desde S38 y sólo se vio ahora al mirar el mapa en pantalla.
- **Incidente de git (repetición del patrón de S18), recuperado**: al cerrar el Sprint 39 se encadenó el merge del PR #49 (en segundo plano) con el borrado de la rama `sprint-39-s3-localstack` y el cambio de rama, **sin confirmar primero que el merge había sido exitoso**. El merge falló ("Head branch is out of date") pero la rama ya había sido borrada (local y remota), lo que dejó huérfano el trabajo de S39 y cerró el PR #49. Se recuperó por reflog: se recreó la rama desde el commit `1364292`, se reabrió y re-mergeó el PR #49 (merge `fbff8369`), se sincronizó `main` (quedó con S39 y velocidad 3.19) y recién entonces se borraron las ramas. **Causa raíz: encadenar operaciones destructivas (branch-delete) con un merge en segundo plano y no esperar su confirmación.**

## 3. Qué probar

- Incorporar una **verificación on-device obligatoria** al DoD de cualquier cambio de la app móvil: arrancar la app en el dispositivo y ejercitar el flujo tocado (login, cámara, mapa), con captura de pantalla como evidencia. No dar por verificada la app sólo con la compilación.
- Considerar un smoke test instrumentado mínimo (p. ej. que la app arranque sin abortar) que pueda correrse en un emulador, para atrapar regresiones de arranque sin depender del teléfono.
- Para las teselas: validar que el interceptor sirve una tesela real (status 200, no la imagen de bloqueo) y no cachear respuestas que no sean imágenes válidas, para no envenenar la caché.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Agregar al DoD la verificación on-device para cambios de la app móvil (arrancar + ejercitar el flujo + captura) | AG-07 (SM) / AG-08 (móvil) | 2027-12-30 | Pendiente |
| **Nunca encadenar branch-delete (ni operaciones destructivas) con un merge; SIEMPRE confirmar el merge exitoso (`merged=true`) antes de borrar ramas; no correr el merge en segundo plano y seguir sin esperar su resultado** | AG-07 (SM) | Inmediata / permanente | Reforzada (2ª ocurrencia: S18 y S39→S40) |
| Endurecer el interceptor de teselas: no cachear respuestas no-imagen / de bloqueo (evitar envenenar la caché) | AG-08 (móvil) | 2027-12-30 | Pendiente |
| Unificar la URL/atribución de teselas también en `mapaRevision.js` (web) | AG-08 (fullstack) | 2027-12-30 | Pendiente (se reitera) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 39 | Estado actual |
| --- | --- |
| Confirmar en la CI del PR que la prueba LocalStack pasa y medir el tiempo del job | Completada (verificada verde en la CI del PR #49) |
| Unificar la URL/atribución de teselas también en `mapaRevision.js` (web) | Pendiente (se reitera) |
| Prueba manual de offline en el dispositivo (modo avión) | Pendiente (se reitera) — parcialmente atendida: el mapa se probó on-device en este sprint, falta el modo avión |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 40 (arreglos de la app móvil verificados on-device): la verificación en el dispositivo es necesaria porque el MAUI no se ejecuta en CI; se documenta el incidente de git del cierre de S39 (encadenar branch-delete con un merge no confirmado, recuperado por reflog) con acción preventiva reforzada (2ª ocurrencia tras S18). 4 acciones. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
