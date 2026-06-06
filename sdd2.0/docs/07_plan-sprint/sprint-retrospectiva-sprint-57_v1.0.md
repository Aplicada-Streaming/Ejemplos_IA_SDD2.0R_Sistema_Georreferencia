# Sprint Retrospectiva — Sprint 57

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-57_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Leer la spec antes de codear pagó.** El pedido del usuario ("mover/crear el marcador y tomarle fotos") se contrastó con CU-04/CU-05/US-15/RN-02 y los wireframes: el marcador nace de la captura (no se "coloca" en el móvil; eso es web), y "agregar foto" (US-15) se modela como una captura en la coordenada del marcador. Ese hallazgo evitó construir un endpoint nuevo y una `Foto` huérfana.
- **Reuso máximo, cambio mínimo.** Cero backend, cero dominio: la agrupación por radio (RN-02) ya hacía el trabajo. Se reusó la cola de capturas offline (S42), el motor de subida y el patrón de cámara/galería (S55). El sprint fue un núcleo pequeño + glue.
- **El EXIF como trampa, resuelto en el núcleo.** Una foto de galería trae su propio EXIF; si se reusaba `ArmadorCapturaCampo` la foto se iría a otra coordenada. `ArmadorFotoMarcador` **descarta el EXIF** y fuerza la del marcador — decisión explícita y cubierta por test.
- **Estimación honesta a 8** (alcance pleno): historia con núcleo nuevo en el gate, a diferencia del S56 acotado.

## 2. Qué no salió bien

- **El build de MAUI necesitó un RID explícito.** Tras limpiar `obj/bin`, el restore implícito no generaba el target `android-arm64` (NETSDK1047); hubo que compilar con `-p:RuntimeIdentifier=android-arm64`. Conviene documentarlo para no repetir el round-trip.
- **net10 obsolete-as-error de nuevo:** `DisplayActionSheet` → `DisplayActionSheetAsync` (misma familia que `DisplayAlert`/`ViewCell` en sprints previos). Vale tener un recordatorio de las APIs MAUI obsoletas en net10.
- **Verificación interactiva pendiente.** El smoke-test confirma arranque sin crash, pero el flujo completo (cámara/galería → foto en el marcador; cerrado → rechazo; offline → cola) se valida con el usuario en el dispositivo. Sigue la acción de S56 de QA on-device sistemática.

## 3. Qué probar

- On-device: mapa → tocar pin → carrusel → "Agregar foto" → **cámara** → la foto aparece bajo ese marcador; ídem **galería** (incluso con EXIF de otro lado: debe caer en el marcador).
- Relevamiento **cerrado**: el backend rechaza (RN-05) y la UI lo informa sin romperse.
- **Offline**: la foto queda en la cola y se sube al reconectar; el indicador de sincronización (S48) lo refleja.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Verificar on-device "agregar foto al marcador" (cámara/galería/cerrado/offline) | AG-05 (QA) | 2028-09-01 | En curso (con el usuario) |
| Documentar el build MAUI con RID explícito (`android-arm64`) y las APIs obsoletas net10 | AG-08 (móvil) | 2028-09-01 | Pendiente |
| Evaluar "quitar foto" de un marcador (US-15; requiere endpoint + dominio) | Equipo | 2028-09-01 | Pendiente (candidato de backlog) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 56 | Estado actual |
| --- | --- |
| Verificar on-device el map-pick de la captura + regresión de la bandeja | En curso (con el usuario) |
| Evaluar centrar el mapa por GPS del dispositivo (Geolocation) | Pendiente (mejora) |
| Pasada de QA on-device sistemática tras cada sprint de UI | En curso (se reitera para S57) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Retrospectiva del Sprint 57 (US-15/CU-09 móvil: agregar foto a un marcador). Leer la spec evitó backend nuevo; reuso máximo (RN-02 + cola de capturas S42 + cámara S55); el EXIF de galería se neutraliza en el núcleo. Fricciones: RID explícito en el build MAUI y obsolete-as-error de `DisplayActionSheet`. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
