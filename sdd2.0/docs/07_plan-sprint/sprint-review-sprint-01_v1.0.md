# Sprint Review — Sprint 01

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-01_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-01_v1.0.md`:

> Entregar el primer slice end-to-end de jerarquía y manejo de usuarios sobre backend, front web y base de datos, de modo que un jefe administre el alta y baja jerárquica de usuarios y su asociación a área, y que cualquier usuario inicie sesión y reloguee en terreno con autorización por rol y área verificada.

Veredicto: Cumplido.

Explicación corta: las 5 historias y 2 BT comprometidas quedaron implementadas y verificadas; el slice es demostrable de extremo a extremo (login ROPC → JWT, alta/baja jerárquica, asociación a área, autorización por rol y área) sobre backend, front web Blazor y base de datos.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-04 | Historia | Login del usuario raíz por ROPC y obtención de JWT con claims sub/role/area | Acceso correcto; se valida la auditoría del acceso (RN-07) |
| US-01 | Historia | Alta de un jefe general por el raíz (201) y rechazo de alta de nivel no inmediato (403) | Cadena jerárquica respetada; baja lógica conserva la información |
| US-02 | Historia | Asociación de un usuario a un área existente y rechazo de área inexistente | La pertenencia a área queda como base de la autorización |
| US-31 | Historia | Listado de usuarios filtrado por ámbito y bloqueo uniforme de accesos fuera de alcance | Sin fuga de recursos de otra área; rechazo auditado (RN-08) |
| US-05 | Historia | Relogueo condicionado al método de seguridad del teléfono (contrato y lógica desde la API) | Aceptado con la integración con el dispositivo Android diferida a la fase 3 |
| BT-02 | Backlog técnico | Validación del nivel jerárquico administrable (cada nivel solo el inmediato inferior) | Reglas de borde cubiertas por pruebas |
| BT-08 | Backlog técnico | Autenticación ROPC/JWT con refresh condicionado y filtro de autorización por rol y área | Errores normalizados con Problem Details (RFC 7807) |

## 3. Feedback recibido

- El front web administrativo cubre alta, baja y asociación de área; se sugiere para próximos sprints sumar gestión de áreas (alta/baja de áreas) hoy resuelta por seed.
- Se solicita revisar el texto de los criterios de aceptación de CU-03 y US-02 que referían "jefe general da de alta un agente" (incoherente con la jerarquía); corregido durante el sprint.
- Confirmar la política de rotación de la clave inicial del usuario raíz antes del primer despliegue.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 40 |
| Puntos completados | 40 |
| Velocity efectiva | 40 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 1 (incoherencia de criterios CU-03 CA-03 / US-02 CA1, corregida) |

Cobertura de pruebas del incremento: dominio 84,8 % líneas / 72,9 % branches; aplicación 87,9 % / 77,5 % (gate ≥ 80 % / ≥ 70 % cumplido). 60 pruebas verdes (55 unitarias + 5 de integración). Build sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-01 | Historia | Aceptada |
| US-02 | Historia | Aceptada |
| US-04 | Historia | Aceptada |
| US-05 | Historia | Aceptada (integración con dispositivo Android diferida a fase 3, según plan §6) |
| US-31 | Historia | Aceptada |
| BT-02 | Backlog técnico | Aceptada |
| BT-08 | Backlog técnico | Aceptada |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 01 se traslada. |

No hay carry-over de ítems comprometidos (0 % < 20 %). Quedan registradas como deuda técnica con BT explícito para sprints de persistencia: completar BT-07 (las 12 entidades del modelo lógico y la suite de integración con Testcontainers; en el Sprint 01 se mapearon las 4 tablas del slice y la migración inicial) y el gate numérico de cobertura de Infrastructure/Api (70 %).

## 7. Decisiones tomadas durante el review

- Corregir CU-03 CA-03 y US-02 CA1 en 02/06 para alinear los criterios con la invariante de nivel inmediato inferior (BT-02). Hecho en el sprint.
- Diferir la cobertura numérica de Infrastructure/Api (70 %) al sprint que complete BT-07 con Testcontainers; en el Sprint 01 se cubre la capa funcionalmente con pruebas de integración.
- Mantener la clave inicial del usuario raíz solo para arranque local; agendar su rotación previa al despliegue.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 01 (slice de jerarquía y manejo de usuarios). Veredicto Cumplido, velocity 40, 0 carry-over de lo comprometido. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
