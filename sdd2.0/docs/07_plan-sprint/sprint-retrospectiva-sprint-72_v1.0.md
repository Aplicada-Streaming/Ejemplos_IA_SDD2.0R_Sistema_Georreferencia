# Sprint Retrospectiva — Sprint 72

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-72_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Arranque limpio de la nueva épica.** El resumen reusa todo lo que ya existe (repos, autorización por área, CQRS ligero); el núcleo nuevo es una función pura testeable. Sin tocar dominio ni base.
- **Valor visible de inmediato.** El endpoint, verificado en vivo, ya muestra estado + totales + productividad por agente sobre datos reales; es la base concreta del tablero.
- **Cobertura sólida.** Agregado en el gate (+4 unitarias) y comportamiento E2E + autorización (+3 integración: agrega / 401 / 404).

## 2. Qué no salió bien

- **Conteo de fotos/comentarios por marcador (N+1).** El handler lista fotos y comentarios marcador por marcador para contarlos; en relevamientos grandes es ineficiente. Aceptable para el MVP del reporte; conviene un método de conteo en la base si escala.
- **Sin UI todavía.** El sprint entrega el backend; el valor para el jefe se completa con la pantalla web (próximo sprint). Un reporte sin pantalla es media historia, aunque la división backend/UI es razonable.

## 3. Qué probar

- Pedir el resumen de un relevamiento con varios agentes y observaciones: los totales y la productividad por agente deben cuadrar con la revisión.
- Un usuario de otra área: 404 (no debe enterarse de que el relevamiento existe).

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Reporting #2: UI web del tablero (mostrar el resumen en el front) | AG-08 | 2029-04-13 | Planificado |
| Reporting #3: resumen por área (todos los relevamientos) + exportes (CSV/PDF) | AG-08 | 2029-04-27 | Pendiente |
| Reporting #4: mapa de calor de observaciones | AG-08 | — | Backlog |
| Conteo de fotos/comentarios en la base (optimización) si escala | AG-08 | — | Backlog |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 71 | Estado |
| --- | --- |
| Definir la próxima épica | **Hecho**: épica "Reporting / analytics" (elección del usuario), arrancada en S72 |
| Prueba relacional del readiness con migración pendiente | Pendiente (deuda de hardening) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Retro del Sprint 72 (reporting #1). Bien: arranque limpio reusando lo existente, valor visible, buena cobertura. Mejoras: N+1 en el conteo, falta la UI (próximo sprint). Generada por AG-07 |
