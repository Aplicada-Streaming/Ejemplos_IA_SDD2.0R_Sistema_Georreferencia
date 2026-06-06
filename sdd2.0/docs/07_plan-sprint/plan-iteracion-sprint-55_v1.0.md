# Plan de Iteración — Sprint 55

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-55_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-07-10
**Fecha fin:** 2028-07-21
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 7,0 SP (S52–S54; arrastra el S52=5); se compromete **8 SP**. Round 2 de la revisión on-device (como `campo1`): tres temas. Núcleo testeable en `GeoVial.Sync`/`GeoVial.Revision`.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Cerrar tres observaciones de la revisión on-device:

1. **Relogueo confuso + hueco de seguridad.** S54 restauraba la sesión **en silencio** (cualquiera que abría la app entraba) y "Cerrar sesión" borraba el usuario recordado (desaparecía el patrón). Rearquitecturar según industria: al **reabrir** la app se pide el **patrón/PIN** para volver a la sesión; **"Cerrar sesión" = logout total** (usuario+clave la próxima); la **vuelta de la cámara** no re-pide el método.
2. **No se ve el usuario logueado.** Mostrarlo.
3. **Pines no tocables.** Por spec el marcador se crea solo al capturar (CU-04/RN-02), pero la doc (US-15 + wireframe del carrusel) pide abrir/enriquecer un marcador. Decisión del PO: hacer los pines **tocables → abrir el carrusel**.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-RELOGUEO | Historia | Modelo de arranque/relogueo: patrón al reabrir, logout total, excepción cámara | Alta | 5 | Dev móvil (AG-08) + fullstack (AG-09) | Cerrada |
| US-PIN-CARRUSEL | Historia | Tocar un pin del mapa abre el carrusel de ese marcador | Media | 2 | Dev móvil (AG-08) | Cerrada |
| US-USUARIO-VISIBLE | Tarea | Mostrar el usuario con sesión iniciada | Baja | 1 | Dev móvil (AG-08) | Cerrada |

Total: 8 SP. Núcleo en `GeoVial.Sync`/`GeoVial.Revision` (en el gate); glue en `GeoVial.Mobile`.

## 4. Alcance técnico

**A — Relogueo (núcleo en el gate):**
- `PoliticaArranque` (pura): `DecisionArranque {Entrar, PedirBiometrico, PedirClave, Bloqueado}` + `Decidir(hayToken, hayMetodo, volviendoDeCamara)` + `TrasBiometrico(ResultadoBiometrico)`.
- `IMarcadorCaptura` (marca "captura en curso" que sobrevive a la muerte de proceso) + `CoordinadorArranque` (decide y restaura). `ServicioSesion.HayTokenPersistidoAsync`.
- Glue: `MarcadorCapturaPreferences` (Preferences); `App.CreateWindow` arranca siempre en `LoginPage` (sin restaurar en silencio); `LoginPage.OnAppearing` ejecuta la decisión (entrar / pedir patrón / pedir clave / bloqueado con botón de desbloqueo); `CapturaPage` marca/limpia el flag alrededor de la cámara; `MainPage.CerrarSesion` = logout total (etiquetado claro).

**B — Pin tocable (núcleo en el gate):**
- `MapaRevisionHtml` embebe el `id` del marcador (ya está en `VistaMapa.Pins`) y agrega `marker.on('click')` → esquema centinela `geovial-marcador://abrir?id=…`; `ParseadorMensajeMarcador` (puro).
- Glue: `MapaRevisionPage` intercepta la navegación y devuelve el id; `RevisionPage.OnVerMapa` abre ese marcador (`NavegadorRevision.IrAlMarcador` + `RenderAsync`).

**C — Usuario visible:** `MainPage` muestra `ServicioSesion.Usuario`.

## 5. Definition of Done aplicada

- Reabrir la app pide el patrón para volver; la vuelta de la cámara no; "Cerrar sesión" pide usuario+clave.
- Tocar un pin abre el carrusel del marcador; se ve el usuario logueado.
- Núcleo (políticas, coordinador, parser) cubierto en el gate; suite unitaria verde.
- Cobertura DoD (Domain/Application ≥80/70); `GeoVial.Sync`/`GeoVial.Revision` altas.
- MAUI compila; verificación on-device del arranque/relogueo y del tap del pin.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Pedir el método antes de tener Activity | Media | Alto | La decisión se hace en `LoginPage.OnAppearing` (no en `CreateWindow`); se elimina el `GetAwaiter().GetResult()` |
| Doble `OnAppearing` re-dispara el método | Media | Medio | Guard de instancia `_decisionTomada` |
| Flag de cámara colgado | Media | Bajo | Se limpia en `finally` y al entrar por vuelta de cámara |
| Token persistido vencido offline | Media | Bajo | Tras desbloquear, el primer request 401 → re-login online cuando haya red (deuda; sin refresh token) |

## 7. Criterios de hecho del sprint

Completo cuando: el relogueo sigue el modelo (patrón al reabrir, logout total, excepción cámara), el pin abre el carrusel, se ve el usuario, el núcleo está cubierto en el gate, la cobertura DoD se mantiene, el MAUI compila y se verifica on-device, y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Revisión on-device round 2 (relogueo, pin tocable, usuario visible) |
| CU/RN | CU-02, RN-06 (acceso/relogueo); CU-04/RN-02 (marcador auto); US-15 + wireframe carrusel (enriquecer marcador) |
| Componentes | `GeoVial.Sync` (`PoliticaArranque`, `CoordinadorArranque`, `IMarcadorCaptura`); `GeoVial.Revision` (`ParseadorMensajeMarcador`, `MapaRevisionHtml`); `GeoVial.Mobile` (`App`, `LoginPage`, `CapturaPage`, `MapaRevisionPage`, `RevisionPage`, `MainPage`) |
| Calidad | definition-of-done §1.4 |
| Tests | `PoliticaArranqueTests`, `CoordinadorArranqueTests`, `ParseadorMensajeMarcadorTests`, `MapaRevisionHtmlMarcadorTests` |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 55 (revisión on-device round 2): modelo de arranque/relogueo (patrón al reabrir, logout total, excepción cámara) + pin tocable → carrusel + usuario visible. 8 SP. Generado por AG-07 |
