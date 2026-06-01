# Plan de Iteración — Sprint 01

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-01_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha inicio:** 2026-06-09
**Fecha fin:** 2026-06-20
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles), duración estándar del sprint (§3.2 de las reglas).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), conforme `equipo_n: 4` de PROJECT-README §1.
- Unidad de estimación: story points (Fibonacci), heredada de 06; no se re-estima en el sprint.
- Capacidad declarada: 42 story points. Factor de focus aún conservador por ser el segundo sprint sin velocity histórica consolidada; se recalibrará con el promedio móvil a partir de S03.

Tabla de capacidad del equipo (sprint de 10 días hábiles, jornada de 6 h efectivas):

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 120 | 0,70 | 84 h |
| Dev frontend | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

La capacidad en horas (≈145,5 h efectivas) se traduce a la capacidad declarada en puntos (42 SP) con factor de focus conservador; los 40 SP comprometidos quedan dentro de la capacidad y por debajo del tope del 110 %.

## 2. Objetivo del sprint

Entregar el primer slice end-to-end de jerarquía y manejo de usuarios sobre backend, front web y base de datos, de modo que un jefe administre el alta y baja jerárquica de usuarios y su asociación a área, y que cualquier usuario inicie sesión y reloguee en terreno con autorización por rol y área verificada.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-01 | Historia | Administrar alta y baja jerárquica de usuarios | Alta | 8 | Dev backend A | Pendiente |
| US-02 | Historia | Asociar usuarios a su área | Alta | 3 | Dev backend A | Pendiente |
| US-04 | Historia | Iniciar sesión y seleccionar relevamiento asignado | Alta | 5 | Dev backend B | Pendiente |
| US-05 | Historia | Relogueo en terreno con método de seguridad | Alta | 5 | Dev backend B | Pendiente |
| US-31 | Historia | Autorizar cada acceso por rol y área | Alta | 8 | Dev frontend / Dev backend A | Pendiente |
| BT-02 | Backlog técnico | Validación del nivel jerárquico administrable | Alta | 3 | Dev backend A | Pendiente |
| BT-08 | Backlog técnico | Autenticación ROPC/JWT con refresh condicionado y autorización por rol y área | Alta | 8 | Dev backend B | Pendiente |

Total de puntos comprometidos: 40 SP (dentro de la capacidad declarada de 42 SP; por debajo del tope del 110 %).

## 4. Alcance técnico

Componentes que se construyen o modifican, en orden de dependencias (referidos a la arquitectura de 05, sin redefinirla):

1. BT-02 — Validación del nivel jerárquico administrable: cada nivel solo administra el inmediato inferior, devolviendo `ACCESO_NO_AUTORIZADO` ante un nivel no autorizado. Depende de BT-01 (entregada en Sprint 00). Soporta US-01 y US-02.
2. BT-08 — Autenticación ROPC/JWT con refresh condicionado al método de seguridad del teléfono y filtro transversal de autorización por rol y área antes de cada handler. Depende de BT-09 y BT-18 (entregadas en Sprint 00). Soporta US-04, US-05 y US-31.
3. US-01 y US-02 — Sobre el módulo de usuarios y jerarquía (BT-01) y la validación BT-02: alta/baja jerárquica de usuarios y asociación a su área, con front web Blazor y pruebas unitarias de dominio y aplicación.
4. US-04 y US-05 — Inicio de sesión (ROPC → JWT) con selección de relevamiento asignado y relogueo en terreno apoyado en el método de seguridad del teléfono; consumen BT-08.
5. US-31 — Autorización por rol y área aplicada en cada acceso, sobre el filtro transversal de BT-08.

Dependencias inter-BT: BT-02 depende de BT-01; BT-08 depende de BT-09 y BT-18. Todas las dependencias upstream se entregaron en el Sprint 00. La arquitectura de capas, autenticación y autorización se gobierna por 05 (`arquitectura-solucion_v1.0.md` §7, `contratos-rest_v1.0.md`) y las ADR de §8. El slice incluye front web Blazor Interactive Server y pruebas unitarias, conforme PROJECT-README §4 fase 2.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica del proyecto, que reside en `08_calidad_y_pruebas/definition-of-done_v1.0.md` (categoría 08). Este plan no la redefine: la referencia por nombre y ubicación.

Criterios específicos del Sprint 01, adicionales a la DoD canónica:

- Cada US del slice cuenta con pruebas unitarias de dominio y aplicación (PROJECT-README §4 fase 2 exige pruebas unitarias en este slice).
- El alta/baja jerárquica y los logueos quedan demostrables end-to-end sobre backend, front web y base de datos en el sprint review.
- Cada acceso fuera del rol o del área queda bloqueado y registrado (criterio de aceptación de BT-08).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El refresh condicionado al método de seguridad del teléfono (ADR-03) introduce complejidad de autenticación móvil no probada antes de existir la app MAUI (fase 3) | Media | Alto | Acotar US-05 al contrato y la lógica de refresh condicionado verificables desde la API y pruebas, dejando la integración con el dispositivo Android para la fase 3; spike de validación del flujo ROPC + refresh el día 1 |
| La autorización transversal por rol y área (BT-08, US-31) puede filtrar accesos legítimos o dejar pasar accesos indebidos si la regla jerárquica de BT-02 no está consolidada | Media | Alto | Entregar BT-02 antes que US-31; batería de pruebas unitarias de los casos de borde de jerarquía y área; revisión acotada del arquitecto (AG-05) sobre el filtro transversal |
| Carry-over desde el Sprint 00 si el walking skeleton (BT-09, BT-18) no quedó completamente verde, ya que BT-08 depende de ambos | Media | Medio | Verificar el cierre del Sprint 00 en el sprint review previo; si hay carry-over, repriorizar BT-08 y diferir US-31 al siguiente sprint antes de comprometer scope nuevo |

## 7. Criterios de hecho del sprint

El Sprint 01 se considera completo cuando: todas las US y BT comprometidas están en estado terminado según la DoD canónica, con sus pruebas unitarias verdes; el slice de jerarquía y manejo de usuarios (alta/baja jerárquica, asociación a área, login y relogueo, autorización por rol y área) queda demostrado end-to-end en el sprint review sobre el entorno de prueba; y se facilitan el sprint review y la retrospectiva con sus artefactos completados a partir de las plantillas de la sección.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que avanzan | CU-03 (administrar la jerarquía de usuarios y áreas: US-01, US-02), CU-02 (iniciar sesión y seleccionar relevamiento asignado: US-04, US-05), CU-14 (aplicar autorización por rol y área en cada acceso: US-31) |
| NB que avanzan | NB-01 (delegación de la recolección en personal no experto: la jerarquía de usuarios y áreas queda operativa), NB-06 (trazabilidad y protección de datos personales: la autenticación y la autorización por rol y área quedan aplicadas en cada acceso) |
| ADRs que gobiernan | ADR-03 (autenticación ROPC + JWT con refresh condicionado), ADR-14 (compliance Ley 25.326), ADR-10 (separación de capas), ADR-01 (estilo monolito modular + CQRS) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 01 (primer slice end-to-end de jerarquía y manejo de usuarios). Compromete US-01, US-02, US-04, US-05, US-31 y las BT de soporte BT-02 y BT-08. Generado por AG-07 |
