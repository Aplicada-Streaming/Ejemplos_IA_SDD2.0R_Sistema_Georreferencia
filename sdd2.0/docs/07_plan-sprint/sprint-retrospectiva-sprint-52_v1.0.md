# Sprint Retrospectiva — Sprint 52

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-52_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se cerró por fin la deuda que se arrastraba desde S47 (cinco retros): el listado por área filtra en la base. Cuando una acción reaparece sprint tras sprint, comprometerla como trabajo y cerrarla es lo sano.
- Refactor **sin cambios observables**: el test de filtro por área previo siguió verde sin tocarlo; se preservó el guard de vigencia. La cobertura de ramas de Domain subió a 80,5 %.
- Decisión de rol centralizada en `Autorizacion.AccedeATodasLasAreas` (no duplicada en el handler), consistente con cómo S47 resolvió "asignados a mí".
- Honestidad de estimación: se comprometieron **5 SP** (no 8) porque es deuda de bajo riesgo; inflarla habría falseado la métrica. Es el primer sprint que rompe la racha de 8, y por la razón correcta.

## 2. Qué no salió bien

- El backlog de la revisión funcional quedó esencialmente vacío: lo que viene (biométrico nativo) es plataforma pura y de difícil cobertura en el gate, y la **verificación on-device** se viene acumulando (indicador S48, bandeja S50, ubicar S51) sin un dispositivo conectado en estos sprints. Hay que decidir dirección con el Product Owner.
- El listado para raíz/jefe general sigue trayendo **todo** (`ListarTodosAsync`): es correcto (ven todo) pero, a futuro, si crece mucho, convendría paginar. Fuera de alcance hoy; anotado.
- `SelectorRelevamientos` (cliente) quedó como está: desde que el móvil usa `/mios` (S47) su marcado "asignado" es trivialmente verdadero, pero simplificarlo rompería tests de S43 sin valor real. Decisión consciente de no tocarlo.

## 3. Qué probar

- Sesión de **verificación on-device** dedicada cuando el dispositivo esté disponible: indicador de sync en vivo (S49), solapa bandeja (S50) y ubicar desde la app (S51), de corrido.
- Evaluar **paginación** del listado (área y "todos") si se prevé alto volumen de relevamientos.
- Definir con el Product Owner el rumbo: ¿biométrico nativo (requiere dispositivo), endurecimiento/paginación, o nuevas historias fuera del backlog de la revisión funcional?

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Sesión de verificación on-device acumulada (indicador + bandeja + ubicar) | AG-05 (QA) | 2028-06-23 | Pendiente |
| Definir el rumbo post-backlog con el Product Owner | AG-07 (SM) | 2028-06-23 | Pendiente |
| Evaluar paginación del listado de relevamientos | AG-06 (backend) | 2028-06-23 | Pendiente (no urgente) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 51 | Estado actual |
| --- | --- |
| Verificar on-device el flujo de ubicación desde la app | Pendiente (se reitera; se agrupa en una sesión on-device) |
| Compilar el MAUI al inicio de los sprints de UI | Aplicada (este sprint no tocó UI; build de control verde) |
| Sprint de limpieza: unificar el listado de área al filtro en base | Completada (entregada en este sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 52 (limpieza): se cerró la deuda arrastrada desde S47; estimación honesta de 5 SP. El backlog de la revisión funcional queda cerrado; se necesita definir rumbo y agrupar la verificación on-device. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
