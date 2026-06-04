# Sprint Retrospectiva — Sprint 43

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-43_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se cerró F-M-04/05: la app pasó de "siempre el primer relevamiento" a una **selección real** con los asignados marcados y primeros; la elección la usan todas las solapas.
- Se aisló un buen **núcleo testeable** en `GeoVial.Sync` (`LectorTokenJwt` + `SelectorRelevamientos`), con 15 pruebas; la lógica de marcado/orden/resolución del activo quedó protegida por el gate sin depender de la UI.
- Se respetó la acción de la retro: **verificación on-device** (login agente → selector poblado → la sincronización usa el activo elegido).
- El cambio reusó `RelevamientoActivoAsync` como único punto de resolución, reemplazando el `PrimerRelevamientoAsync` repetido en cuatro páginas (menos duplicación).

## 2. Qué no salió bien

- El backend lista los relevamientos del **área**, no sólo los asignados; el cliente marca los asignados (✓) pero muestra todos. Un endpoint "asignados a mí" sería más limpio (queda como mejora futura).
- Leer el JWT en el cliente sin validar firma es pragmático, pero acopla el cliente al formato del token (`sub`); si el backend cambiara el claim, habría que ajustarlo.
- La verificación on-device se trabó por un **flanco de entorno** (el backend de desarrollo se cayó y el túnel `adb reverse` quedó en mal estado → "unexpected end of stream"); se resolvió reiniciando backend + túnel + app. No es un problema del código (el login no cambió desde S40/S42), pero recordó la fragilidad del entorno de prueba manual.

## 3. Qué probar

- Evaluar un endpoint backend "relevamientos asignados a mí" para no depender del marcado client-side.
- Probar la selección con varios relevamientos asignados (elegir uno, cerrar/abrir la app y confirmar que se recuerda).
- Estabilizar el entorno de prueba on-device (script que verifique backend arriba + `adb reverse` antes de desplegar) para evitar los falsos negativos de conexión.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Método de seguridad + reingreso offline (RN-06, F-M-02/03) | AG-08 (móvil) | 2028-02-19 | Pendiente (S44) |
| Evaluar endpoint backend "relevamientos asignados a mí" | AG-06 (backend) | 2028-02-19 | Pendiente |
| Script de pre-chequeo del entorno on-device (backend + adb reverse) | AG-05 (QA) | 2028-02-19 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 42 | Estado actual |
| --- | --- |
| Selección de relevamiento asignado en el móvil (F-M-04/05) | Completada (entregada en este sprint) |
| Probar el ciclo offline en modo avión en el dispositivo | Pendiente (se reitera) |
| Evaluar idempotencia de la captura en el backend (dedup por `CapturaId`) | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 43 (selección de relevamiento asignado): núcleo de selección en el gate + verificación on-device; flanco de entorno (caída de backend + túnel adb) documentado. 3 acciones nuevas. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
