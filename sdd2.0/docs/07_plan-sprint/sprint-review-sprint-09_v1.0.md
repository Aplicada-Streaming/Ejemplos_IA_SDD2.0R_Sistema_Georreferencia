# Sprint Review — Sprint 09

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-09_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-09_v1.0.md`:

> Permitir que la app de campo, al recuperar conexión, suba sus cambios locales encolados y el backend los consolide contra el estado central aplicando la última escritura por marca temporal (RN-04), marcando como conflicto todo recurso resuelto por esa vía y los marcadores que queden dentro de un mismo radio (RN-02), de forma idempotente por identificador de cambio, y luego baje las actualizaciones recientes del relevamiento; todo verificado por área y auditado.

Veredicto: Cumplido (alcance backend de US-18).

Explicación corta: el endpoint de sincronización recibe un lote de cambios, los aplica una sola vez por `CambioId` (idempotencia, RC-03), consolida con last-write-wins por marca temporal y marca como conflicto de edición todo comentario editado de forma concurrente (RN-04), reutiliza la detección de marcadores en radio (RN-02, Sprint 05) y baja las actualizaciones posteriores a la marca del cliente, autorizado por área (RN-01) y auditado (RN-07). Con esto la regla RN-04 (última escritura efectiva), diferida desde la resolución de conflictos del Sprint 05, queda implementada.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-18 | Historia | Subir un lote de cambios: se aplican, se confirman y bajan las actualizaciones del relevamiento | Consolidación sin pérdida |
| US-18 | Historia | Reenviar el mismo `CambioId`: no se aplica dos veces (idempotencia, reanudación sin duplicar) | Reintento seguro |
| US-18 | Historia | Dos ediciones del mismo comentario: prevalece la de marca más reciente y queda marcado como conflicto | Last-write-wins + marca de conflicto |
| US-18 | Historia | Sincronizar sobre relevamiento cerrado se rechaza (RN-05); un agente de otra área no sincroniza (RN-01) | Acotamiento respetado |

## 3. Feedback recibido

- La consolidación backend cierra RN-04 y deja el ciclo capturar (campo) → sincronizar → resolver (web) demostrable end-to-end en el backend.
- Para completar EP-04 falta el cliente móvil (captura offline US-16, cola US-17, conectividad US-19) y la librería publicable `GeoVial.Sync` (BT-15), que requieren la plataforma MAUI/SQLite; se planifican cuando se incorpore el proyecto móvil.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 13 |
| Puntos completados | 13 |
| Velocity efectiva | 13 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 216 verdes (191 unitarias + 25 de integración), +11 respecto del Sprint 08. Cobertura: dominio 89,6 % líneas / 79,8 % branches; aplicación 89,4 % / 80,7 %; `GeoVial.FileHosting` 100 % / 100 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-18 | Historia | Aceptada (alcance backend de consolidación; cliente móvil y librería de sync fuera de alcance por plataforma) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 09 se traslada. |

El cliente móvil de captura offline (US-16), el encolado local (US-17), la detección de conectividad (US-19) y la librería publicable `GeoVial.Sync` con `IChangeQueue`/`ISyncEngine` (BT-15) requieren la plataforma móvil (MAUI/SQLite) y la publicación del paquete; quedan en el backlog hasta incorporar ese proyecto. La consolidación de este sprint cubre los comentarios como recurso editable (CA-02); extender last-write-wins a otras entidades editables es trabajo aditivo posterior.

## 7. Decisiones tomadas durante el review

- Dar por implementada la regla RN-04 (última escritura efectiva con marca de conflicto) en el backend.
- Diferir el resto de EP-04 (cliente móvil y librería de sync) a un sprint con la plataforma móvil incorporada.
- Considerar para los próximos sprints la consulta del historial de auditoría con retención (US-30, EP-08).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 09 (backend de consolidación de la sincronización, US-18). Veredicto Cumplido, velocity 13, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
