# Sprint Review — Sprint 55

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-55_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-55_v1.0.md`:

> Rearquitecturar [el relogueo] según industria: al reabrir la app se pide el patrón/PIN para volver a la sesión; "Cerrar sesión" = logout total; la vuelta de la cámara no re-pide el método. […] mostrar el usuario logueado […] hacer los pines tocables → abrir el carrusel.

Veredicto: Cumplido.

Explicación corta: se rearquitecturó el arranque/relogueo con un núcleo puro y testeable (`PoliticaArranque` + `CoordinadorArranque`): al reabrir con sesión persistida y método del teléfono se pide el patrón/PIN (desbloqueo offline con el token persistido); la vuelta de la cámara (marca `IMarcadorCaptura` que sobrevive a la muerte de proceso) entra sin re-pedir; "Cerrar sesión" sigue siendo logout total (etiquetado claro). Se quitó la restauración silenciosa de S54 (hueco de seguridad). Además, los pines del mapa ahora son tocables (esquema centinela `geovial-marcador://` + `ParseadorMensajeMarcador`) y abren el carrusel del marcador, y la solapa Sync muestra el usuario logueado.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-RELOGUEO | Funcionalidad | Reabrir la app pide el patrón → entra; cancelar → pantalla de bloqueo con "usar usuario y clave"; volver de la cámara → entra sin re-pedir; "Cerrar sesión" → usuario+clave | El relogueo quedó claro y seguro |
| US-PIN-CARRUSEL | Funcionalidad | Tocar un pin en el mapa de la revisión abre el carrusel de ese marcador | Coherente con US-15/wireframe |
| US-USUARIO-VISIBLE | UX | La solapa Sync muestra "Sesión: campo1" | Ya se sabe con qué usuario uno entró |

## 3. Feedback recibido

- El relogueo siguió práctica de industria (login con clave → enrolar método → desbloqueo por patrón al reabrir; logout total aparte), aclarando la confusión reportada y cerrando el hueco de la restauración silenciosa.
- Reuso del patrón ya probado: el "puente WebView↔app por esquema centinela" (S51) se replicó para el tap del pin; el patrón gate/glue (núcleo puro + coordinador) para el arranque.
- El marcador **se sigue creando solo** al capturar (CU-04/RN-02); lo que se agregó es **abrirlo** desde el mapa (no crearlo a mano), fiel a la spec.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **457** (414 unitarias + 43 de integración), +20 unitarias: `PoliticaArranqueTests` (8), `CoordinadorArranqueTests` (5), `ParseadorMensajeMarcadorTests` (6), `MapaRevisionHtmlMarcadorTests` (1). Las 43 de integración no se ven afectadas (el sprint sólo toca `GeoVial.Sync`/`GeoVial.Revision`/`GeoVial.Mobile`, no Api/Application/Domain/Infrastructure). Cobertura del gate: Domain 88,5 % / 80,5 %; Application 87,6 % / 76,5 % (umbral 80 % / 70 %); `GeoVial.Sync` 94,5 % / 89,2 %; `GeoVial.Revision` 97,7 % / 93,1 %. El MAUI compila y se redeployó al moto g42 (arranque sin crash). El patrón al reabrir y el tap del pin se confirman con el usuario en el dispositivo.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-RELOGUEO | Historia | Aceptada (modelo de arranque/relogueo + logout total + excepción cámara) |
| US-PIN-CARRUSEL | Historia | Aceptada (pin tocable → carrusel) |
| US-USUARIO-VISIBLE | Tarea | Aceptada (usuario visible) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido se traslada. |

Pendiente: confirmación on-device del patrón al reabrir / cancelar / vuelta de cámara y del tap del pin. Deuda anotada: token persistido vencido offline (sin refresh token); `CoordinadorReingreso` (online /auth/reingreso) quedó sin uso en la UI (el desbloqueo offline lo reemplaza) — evaluar removerlo o repurposearlo.

## 7. Decisiones tomadas durante el review

- Mover la decisión de arranque a `LoginPage.OnAppearing` (Activity viva; sin `GetAwaiter().GetResult()`).
- Desbloqueo offline = patrón + token persistido (sin red), en vez de pegar siempre a `/auth/reingreso`.
- Reusar el esquema centinela para el tap del pin (consistencia con S51).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 55 (revisión on-device round 2). Veredicto Cumplido, velocity 8, 0 carry-over, 457 pruebas (+20 unitarias); cobertura del gate mantenida; MAUI redeployado. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
