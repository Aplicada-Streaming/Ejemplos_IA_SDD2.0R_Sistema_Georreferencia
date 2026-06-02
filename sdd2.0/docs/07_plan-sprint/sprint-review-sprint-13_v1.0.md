# Sprint Review — Sprint 13

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-13_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-13_v1.0.md`:

> Publicar una demo autónoma que ejercite la librería de sincronización `GeoVial.Sync` contra un backend simulado (US-32, EP-09), de modo que un integrador externo pueda evaluar el reuso sin depender de GeoVial: alta de registros locales, sincronización contra un mock con visualización del estado de la cola (pendiente → sincronizado) y resolución básica de conflictos.

Veredicto: Cumplido.

Explicación corta: la demo vive en `samples/02-sync-maui-demo` y ejercita solo la superficie pública (`Abstractions`) de la librería. El `BackendSimulado` implementa el mismo `ISyncBackendClient` que el cliente REST real: consolida los cambios subidos y, para los recursos marcados en conflicto, devuelve un `ConflictInfo` sin confirmar (RN-04, queda pendiente hasta resolver). El `ResolutorConflictos` ofrece la resolución básica —mantener lo local (la próxima sincronización confirma) o aceptar lo remoto (se descarta de la cola)—. El `CoordinadorDemo` orquesta capturar → sincronizar → estado de la cola, consumido tanto por la cáscara MAUI (`SyncDemo.Maui`) como por las pruebas de aceptación AT-32, que corren sobre la **cola SQLite y el motor reales** de la librería.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-32 | Historia | Alta de registros locales + sincronización: la cola pasa de pendiente a sincronizado | Estado de la cola claro y evaluable |
| US-32 | Historia | Un cambio que choca con el mock se reporta como conflicto y queda pendiente | Conflicto visible, no se pierde el cambio |
| US-32 | Historia | Resolución básica «mantener lo local»: la siguiente sincronización confirma el cambio | Última escritura (RN-04) demostrada |
| US-32 | Historia | Resolución básica «aceptar lo remoto»: el cambio local se descarta de la cola | Cierre del ciclo de evaluación |

## 3. Feedback recibido

- La demo permite a un integrador externo evaluar la librería de forma íntegramente local (sin red ni backend real), cubriendo el requisito de reuso de EP-09 (BRIEF §1, §14).
- La separación núcleo testeable (`SyncDemo.Nucleo`, en CI) / cáscara visual (`SyncDemo.Maui`, fuera de CI) confirmó el patrón de S11/S12: el valor verificable entra al gate sin atarlo al empaquetado del APK.
- Lo que resta para la app de campo completa sigue siendo la UI de captura (foto/GPS/comentario) y la pantalla de resolución de conflictos en `GeoVial.Mobile`.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 256 verdes (227 unitarias + 29 de integración), +6 respecto del Sprint 12 (las pruebas de aceptación AT-32). Cobertura del núcleo de la demo `SyncDemo.Nucleo`: 97,7 % líneas / 100 % branches (gate ≥ 80 % / ≥ 70 % cumplido). El dominio, la aplicación y `GeoVial.Sync` no cambiaron código de producción (solo se agregó un sample y sus pruebas), por lo que sus coberturas se mantienen sobre el gate. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-32 | Historia | Aceptada (demo autónoma sobre la superficie pública + resolución básica de conflictos; la cáscara MAUI compila para `net10.0-android` fuera de CI) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 13 se traslada. |

La cáscara `SyncDemo.Maui` queda fuera de la solución/CI por el empaquetado del APK (Android SDK provisionado), igual que `GeoVial.Mobile`. La UI de captura de campo (foto/GPS) y la pantalla de resolución de conflictos sobre el mapa (US-26, web) quedan en el backlog.

## 7. Decisiones tomadas durante el review

- Dar por cubierto EP-09 en su alcance de demo de evaluación (US-32); la publicación del paquete preview en GitHub Packages sigue pendiente como tarea de DevOps.
- Mantener la demo íntegramente local (backend simulado) como artefacto de evaluación de la librería, separado de la app de producto `GeoVial.Mobile`.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 13 (demo autónoma de evaluación de la librería de sincronización, US-32). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
