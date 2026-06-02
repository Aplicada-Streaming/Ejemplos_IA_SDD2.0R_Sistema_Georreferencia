# Sprint Retrospectiva — Sprint 18

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-18_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Completar US-26 destapó y corrigió un fallo latente: el listado de pendientes asumía que todo conflicto era de radio (`Marcadores()` para todos), lo que habría roto con una edición en conflicto. La corrección quedó cubierta por una prueba específica.
- La resolución quedó type-aware con un guard explícito (`DECISION_CONFLICTO_INAPLICABLE`): cada tipo solo admite su decisión, evitando estados inconsistentes.
- El backend de conflictos del Sprint 05 era extensible: agregar el tipo edición fue sumar una rama, sin reescribir la unificación de marcadores.
- El sprint sumó valor verificable en dominio/aplicación (dentro del gate), no solo UI: el gate se mantuvo holgado (89,7/79,8 dominio; 90,0/82,0 aplicación).

## 2. Qué no salió bien

- No hay una prueba de integración HTTP que recorra el ciclo completo de una edición en conflicto (sincronizar con colisión → listar → confirmar); la verificación es a nivel de handler.
- La pantalla web de conflictos sigue pidiendo el GUID del relevamiento a mano; falta un selector de relevamientos del área.
- La resolución de edición no muestra el valor en conflicto ni el previo (solo el recurso): el jefe confirma sin ver el diff, porque el modelo conserva únicamente el valor ganador (RN-04).

## 3. Qué probar

- Una prueba de integración que genere una edición en conflicto vía sync y la dirima por HTTP de punta a punta.
- Mostrar en la web el valor consolidado del recurso en conflicto (texto del comentario ganador) para que la confirmación sea informada.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Edición sobre el marcador desde el móvil (comentario/etiqueta, US-15 cliente) | AG-08 (móvil) | 2027-03-06 | Pendiente |
| Prueba de integración del ciclo de edición en conflicto (sync → listar → confirmar) | AG-05 / QA | 2027-03-06 | Pendiente |
| Selector de relevamientos del área en la pantalla web de conflictos | AG-08 (web) | 2027-03-06 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 17 | Estado actual |
| --- | --- |
| Provisionar clave de mapas y mapa interactivo (unifica US-13 y US-21) | Pendiente (se reitera) |
| Cachear las fotos descargadas del carrusel de revisión | Pendiente |
| Edición sobre el marcador desde el móvil (US-15 cliente) | Pendiente (se reitera; planificada para el próximo sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 18 con 3 acciones nuevas y seguimiento de las del Sprint 17; se corrigió un fallo latente del listado de conflictos. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
