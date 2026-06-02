# Sprint Retrospectiva — Sprint 13

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-13_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Separar el núcleo testeable (`SyncDemo.Nucleo`, `net10.0`) de la cáscara MAUI (`SyncDemo.Maui`, `net10.0-android`) dejó las pruebas de aceptación AT-32 corriendo en CI sobre la cola SQLite y el motor reales, contra el `BackendSimulado`; el núcleo alcanzó 97,7 % líneas / 100 % branches sin tocar el empaquetado del APK.
- El `BackendSimulado` reutiliza el mismo contrato `ISyncBackendClient` que el cliente REST: las pruebas no usan dobles del motor ni de la cola, sino las implementaciones reales, de modo que la demo valida la librería tal como la consumiría un integrador.
- La provisión del Android SDK quedó resuelta: la cáscara `SyncDemo.Maui` compila para `net10.0-android` (RID `android-arm64`), destrabando la acción reiterada de los Sprints 11 y 12.
- Mantener la demo íntegramente local (sin red ni backend real) la hace autosuficiente como artefacto de evaluación de EP-09.

## 2. Qué no salió bien

- El motor `MotorSincronizacion` no descarga actualizaciones cuando la cola está vacía (corta antes de llamar al backend): la demo solo evidencia la bajada de cambios en una sincronización que además sube algo. El comportamiento «sin pendientes, solo baja actualizaciones» (US-19 CA-02) no se ejercita por la vía del motor con cola vacía.
- La resolución de conflictos de la demo es básica (mantener/aceptar sobre un recurso); la unificación de marcadores sobre el mapa (US-26, web) sigue sin abordarse.
- `GeoVial.Sync` todavía no se publicó como paquete preview en GitHub Packages; la demo referencia el proyecto por ruta, no el paquete versionado.

## 3. Qué probar

- Evaluar una sincronización «solo bajada» en el motor (llamar al backend aunque la cola esté vacía) para cubrir US-19 CA-02 por esa vía, sin romper la idempotencia ni el corte anti-ciclo.
- Publicar `GeoVial.Sync` como paquete preview (SemVer) y hacer que la demo lo consuma como paquete, validando el reuso real de la librería publicada.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Publicar `GeoVial.Sync` como paquete preview en GitHub Packages y que la demo lo consuma | AG-09 (DevOps) | 2026-12-19 | Pendiente |
| Construir la UI de captura de campo (foto/GPS/comentario) en `GeoVial.Mobile` | AG-08 (móvil) | 2026-12-19 | Pendiente |
| Evaluar la bajada de actualizaciones con cola vacía en el motor (US-19 CA-02 por esa vía) | AG-05 | 2026-12-19 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 12 | Estado actual |
| --- | --- |
| Segmentar la cola/motor por relevamiento para jornadas con varios relevamientos | Pendiente (se reitera para cuando entre la UI multi-relevamiento) |
| Construir la UI de captura de campo (foto/GPS/comentario) en `GeoVial.Mobile` | Pendiente (se reitera) |
| Provisionar el Android SDK en un agente para construir/ejecutar la app | Completada (la cáscara MAUI se compila para `android-arm64`) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 13 con 3 acciones nuevas y seguimiento de las del Sprint 12 (Android SDK provisionado: completada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
