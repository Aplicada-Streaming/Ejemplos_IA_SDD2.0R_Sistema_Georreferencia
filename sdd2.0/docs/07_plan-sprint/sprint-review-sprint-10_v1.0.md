# Sprint Review — Sprint 10

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-10_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-10_v1.0.md`:

> Hacer operativa la trazabilidad legal (Ley 25.326, NB-06): que el usuario raíz consulte el registro inmutable de accesos y acciones filtrando por usuario, recurso o rango de fechas dentro del período de retención, y que el acceso a los datos personales de un usuario quede acotado por rol y área, limitado a la finalidad del relevamiento y registrado, cerrando la épica de auditoría y datos personales.

Veredicto: Cumplido.

Explicación corta: el usuario raíz consulta el registro inmutable filtrando por autor, recurso y rango de fechas, acotado por defecto a la retención de doce meses (RN-07); un no-raíz queda bloqueado con `ACCESO_NO_AUTORIZADO` y su intento se registra. El acceso a los datos personales de un usuario exige una finalidad permitida (RN-08), se autoriza por rol y área (RN-01) y se registra; una finalidad ajena responde `FINALIDAD_NO_PERMITIDA`. El registro es inmutable por construcción (sin mutadores). Con esto queda cerrada la épica EP-08.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-30 | Historia | El raíz consulta el historial filtrando por recurso y rango; aparece el login auditado | Trazabilidad legal operativa |
| US-30 | Historia | Un jefe general (no raíz) intenta consultar el registro: bloqueado y registrado | Restricción correcta |
| US-31 | Historia | Un jefe de área accede a los datos personales de un agente de su área con finalidad "relevamiento"; queda registrado | Acceso acotado y auditado |
| US-31 | Historia | Una finalidad "marketing" se bloquea; un jefe de otra área no accede | Limitación de finalidad y de área |

## 3. Feedback recibido

- Con la consulta de auditoría y el acceso a datos personales con finalidad, el backend cumple las exigencias de la Ley 25.326 (NB-06): registro inmutable con retención, consulta restringida y acceso acotado por rol, área y finalidad.
- El MVP backend (EP-01 a EP-08, salvo el cliente móvil de EP-04) queda funcionalmente completo; el siguiente foco es incorporar la plataforma móvil (MAUI/SQLite) para EP-04 (US-16/17/19) y la librería publicable `GeoVial.Sync` (BT-15).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 13 |
| Puntos completados | 13 |
| Velocity efectiva | 13 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 234 verdes (205 unitarias + 29 de integración), +18 respecto del Sprint 09. Cobertura: dominio 89,6 % líneas / 79,8 % branches; aplicación 89,9 % / 81,8 %; `GeoVial.FileHosting` 100 % / 100 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-30 | Historia | Aceptada |
| US-31 | Historia | Aceptada (cierre §5.B: acceso a datos personales con finalidad; la autorización transversal se entregó en el Sprint 01) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 10 se traslada. |

EP-08 (auditoría y datos personales) queda cerrada. Lo pendiente del backlog es el cliente móvil de EP-04 (US-16/17/19), la librería publicable `GeoVial.Sync` (BT-15), la demo autónoma de sincronización (US-32) y los Could de notificación (US-20), todos dependientes de la plataforma móvil o de la publicación del paquete.

## 7. Decisiones tomadas durante el review

- Dar por cerrada la épica EP-08 y, con ella, el alcance backend del MVP (EP-01 a EP-08 salvo el cliente móvil de EP-04).
- Planificar la incorporación de un proyecto móvil (MAUI/SQLite) como condición para el resto de EP-04 y la librería de sincronización.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 10 (cierre de EP-08: consulta de auditoría con retención y acceso a datos personales con finalidad). Veredicto Cumplido, velocity 13, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
