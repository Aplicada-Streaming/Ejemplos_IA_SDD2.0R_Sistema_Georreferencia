# Sprint Retrospectiva — Sprint 54

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-54_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- La **sesión de verificación on-device** (postergada desde S48) por fin se hizo y pagó: encontró dos defectos reales que el gate no podía ver —el reseteo al volver de la cámara y el biométrico que no aceptaba patrón— y se corrigieron en el mismo sprint.
- El reseteo se atacó de raíz (persistencia de sesión), no con un parche: ahora la sesión sobrevive a que el SO mate el proceso. El núcleo quedó testeable (`IAlmacenTokenSesion` + restauración) con 3 casos en el gate.
- El fix del biométrico (aceptar patrón/PIN) además acerca la implementación a la intención real de RN-06 ("método de seguridad **del teléfono**", no sólo huella).
- Verificación parcial autónoma: pude confirmar por adb el deploy, el arranque limpio y que la ruta de restauración no crashea, sin depender de cada toque del usuario.

## 2. Qué no salió bien

- El biométrico de S53 se entregó **sin** verificación on-device y resultó que no funcionaba con patrón (el equipo de prueba no tiene huella). Es exactamente el riesgo que la retro de S53 anotó: "la verificación real no se pudo probar on-device". Confirma que las features de plataforma deberían verificarse en dispositivo **antes** de cerrarse, no un sprint después.
- La **foto en vuelo** puede perderse si el proceso muere en el peor caso de memoria; sólo garantizamos no perder la sesión. Persistir la captura en curso (guardar el estado de la intención de cámara) queda como mejora futura si se observa pérdida frecuente.
- Dos iteraciones de build por los nombres/versiones de la API de biométrico (`BiometricManager.Authenticators`, `SetDeviceCredentialAllowed` por nivel de API). Compilar el MAUI temprano (acción de S51) ayudó a detectarlo rápido.

## 3. Qué probar

- Confirmar on-device el camino positivo: login → sacar foto (o matar el proceso a mano) → la app vuelve a las **solapas** logueada.
- Confirmar el reingreso en terreno con **patrón** end-to-end (cancelar también).
- Observar si, tras el kill, alguna vez se pierde la foto recién tomada (para decidir si vale persistir la captura en vuelo).

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Confirmar on-device camino positivo de restauración + reingreso con patrón | AG-05 (QA) | 2028-07-21 | En curso (con el usuario) |
| Verificar features de plataforma **en dispositivo dentro del mismo sprint** (no después) | Equipo | 2028-07-21 | Acordado |
| Evaluar persistir la captura en vuelo si se observa pérdida de foto tras kill | AG-08 (móvil) | 2028-07-21 | Pendiente (condicional) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 53 | Estado actual |
| --- | --- |
| Sesión de verificación on-device acumulada (indicador + bandeja + ubicar + biométrico) | En curso (este sprint hizo la sesión; encontró y corrigió 2 defectos; quedan toques del usuario por confirmar) |
| Definir el rumbo post-backlog con el Product Owner | Resuelto por ahora: verificación on-device + fixes |
| Limpiar/repurposear `SeguridadDispositivo.MetodoPresenteAsync` (sin uso tras S53) | Pendiente (no urgente) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 54 (fixes on-device): la verificación en dispositivo encontró y corrigió el reseteo por cámara (persistencia de sesión) y el biométrico que no aceptaba patrón. Lección: verificar plataforma en dispositivo dentro del mismo sprint. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
