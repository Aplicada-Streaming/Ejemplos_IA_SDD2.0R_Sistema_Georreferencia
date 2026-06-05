# Sprint Retrospectiva — Sprint 53

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-53_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Se cerró el último ítem de feature del backlog de la revisión funcional (biométrico nativo) convirtiendo RN-06 de un marcador blando en una verificación real.
- El patrón gate/glue funcionó otra vez: la decisión (verificar → reingresar) en `CoordinadorReingreso` (gate, 6 tests con un autenticador falso) y la dependencia de plataforma aislada tras `IAutenticadorBiometrico`.
- Se evitó el riesgo de dependencia usando el `BiometricPrompt` de **plataforma** (API 28+) en vez de un binding NuGet; **el MAUI compiló a la primera** (las firmas de la API nativa, incluido el callback y el listener del botón, estaban bien) — a diferencia de S50/S51 que rompieron por obsoletos. La acción de S51 (compilar el MAUI durante el sprint, no al cierre) ayudó.
- Vuelta al alcance pleno (8 SP) tras el sprint de limpieza acotado (S52=5).

## 2. Qué no salió bien

- La verificación biométrica **real** no se pudo probar on-device: el núcleo cubre la decisión y el build valida la API, pero el prompt de huella/PIN sólo se ve en el dispositivo. Se suma a la verificación on-device acumulada (S49/S50/S51).
- El `SeguridadDispositivo.MetodoPresenteAsync` (marcador blando) quedó **sin uso** en el flujo de reingreso (ahora lo reemplaza la verificación nativa). Quedó en el código por compatibilidad; conviene limpiarlo o repurposearlo en un futuro toque.
- Dispositivos sin huella/PIN configurado no pueden reingresar en terreno (caen a usuario+clave). Es correcto por seguridad, pero conviene comunicarlo bien en la UI (hoy el mensaje lo explica al intentar).

## 3. Qué probar

- **Sesión on-device** (la grande, ya acumulada): login + configurar método (con biométrico) → matar proceso → reingreso con huella/PIN real → entra sin clave; y los casos de cancelar/fallar.
- Verificar el comportamiento en un dispositivo **sin** método configurado (debe ofrecer usuario+clave con mensaje claro).
- Confirmar que el `BiometricPrompt` de plataforma se ve bien (título/descripción/botón "Usar clave") en el moto g42.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Sesión de verificación on-device acumulada (indicador + bandeja + ubicar + **biométrico**) | AG-05 (QA) | 2028-07-07 | Pendiente |
| Definir el rumbo post-backlog con el Product Owner (endurecimiento/escala vs. nuevas historias) | AG-07 (SM) | 2028-07-07 | Pendiente |
| Limpiar/repurposear `SeguridadDispositivo.MetodoPresenteAsync` (sin uso tras S53) | AG-08 (móvil) | 2028-07-07 | Pendiente (no urgente) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 52 | Estado actual |
| --- | --- |
| Sesión de verificación on-device acumulada | Pendiente (se reitera; ahora incluye el biométrico) |
| Definir el rumbo post-backlog con el Product Owner | En curso (se entregó el biométrico, último feature; queda decidir lo siguiente) |
| Evaluar paginación del listado de relevamientos | Pendiente (no urgente) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 53 (biométrico nativo): RN-06 pasa de marcador blando a verificación real; gate/glue limpio; MAUI compiló a la primera. Backlog de la revisión funcional cerrado; pendiente la sesión de verificación on-device acumulada y definir rumbo. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
