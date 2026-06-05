# Sprint Retrospectiva — Sprint 49

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-49_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Un solo cambio (el evento `SincronizacionCompletada`) cerró **dos** cosas: la salvedad de S48 (indicador no se actualizaba solo) y un ítem del backlog (avisos de conflictos). Buena economía: un evento, dos usos.
- Cambio **aditivo** al `CoordinadorAutoSync` (sin tocar ctor ni retorno): los tests de auto-sync de S12 y S45 siguieron verdes sin tocarlos; +3 pruebas cubren el evento (éxito/​error/​sin-relevamiento).
- Se aprovechó para **corregir el patrón de fuga**: las suscripciones de la página (indicador S48 + evento S49) pasaron a `OnAppearing`/`OnDisappearing`, en vez de quedar en el constructor de una página transitoria.
- Quedó cerrada la visibilidad de la sincronización: capturar (S42) → auto-sync (S45) → indicador (S48) → **en vivo + aviso de conflictos (S49)**.

## 2. Qué no salió bien

- No se verificó **on-device**: el flujo (modo avión → reconectar → ver el indicador cambiar solo y el aviso) se confió al gate. El núcleo está cubierto, pero la verificación visual quedó pendiente otra vez.
- El aviso de conflictos es informativo ("revisalos en la web/revisión"); el agente **no puede resolverlos desde la app**. Es correcto para el alcance (la resolución es web, CU-12), pero el agente queda dependiendo de otro canal.
- El payload `ResultadoAutoSync` trae conteos crudos; no distingue, p. ej., tipos de conflicto. Suficiente para avisar, pero si se quisiera más detalle haría falta enriquecerlo.

## 3. Qué probar

- Verificar on-device: capturar sin señal, reconectar, y confirmar que el indicador pasa a "Todo sincronizado" **solo** y que, ante un conflicto sembrado, aparece el aviso.
- Evaluar un acceso directo desde el aviso a la pantalla de revisión (aunque la resolución siga siendo web), para acortar el camino del agente.
- Revisar si conviene un pequeño log/timestamp de "último sync" junto al indicador para más confianza.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Verificar on-device el refresco en vivo + el aviso de conflictos | AG-05 (QA) | 2028-05-12 | Pendiente |
| Sprint de limpieza: unificar el listado de área al filtro en base (se arrastra de S47/S48) | AG-06 (backend) | 2028-05-12 | Pendiente |
| Evaluar bandeja sin georreferenciar navegable (gap de la revisión funcional) | AG-08 (móvil) | 2028-05-12 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 48 | Estado actual |
| --- | --- |
| Evento "auto-sync terminó" en `CoordinadorAutoSync` + auto-refresco del indicador | Completada (entregada en este sprint) |
| Verificar on-device los cinco estados del indicador | Pendiente (se reitera, ahora junto con el refresco en vivo) |
| Sprint de limpieza: unificar el listado de área al filtro en base | Pendiente (se reitera de S47) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 49 (auto-sincronización observable): un evento cerró la salvedad de S48 y el aviso de conflictos; se corrigió el patrón de fuga de suscripciones. Pendiente: verificación on-device y el sprint de limpieza arrastrado. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
