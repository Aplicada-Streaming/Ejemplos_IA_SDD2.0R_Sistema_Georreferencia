# Sprint Review — Sprint 58

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-58_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-58_v1.0.md`:

> Falta el flujo alternativo §5.A de CU-09: quitar una foto del marcador. El objetivo es que el agente, parado en una foto del carrusel, pueda quitarla (con confirmación), siempre que el relevamiento no esté cerrado (RN-05) y tenga acceso al área (RN-01).

Veredicto: Cumplido.

Explicación corta: el carrusel de revisión suma un botón **"🗑 Quitar la foto en foco"** que, con confirmación, llama a un nuevo `DELETE /api/v1/fotos/{id}`. El backend ejecuta la cascada de CU-09 §5.A: **desvincula** los comentarios que referenciaban la foto (sobreviven a nivel marcador), borra las etiquetas de la foto, la **foto** y su **observación** (1:1), y el **binario** alojado (best-effort), todo auditado (`ELIMINAR_FOTO`) y bloqueado si el relevamiento está cerrado (RN-05) o el usuario no accede al área (RN-01). Con "agregar" (S57) + "quitar" (S58) + comentar/etiquetar (S19), **US-15 queda cerrada de punta a punta en el móvil**. No hizo falta migración (sólo borrado de filas).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-15-QUITAR-FOTO | Funcionalidad | Carrusel → foto en foco → "Quitar" → confirmar → la foto desaparece del marcador | Cierra el ciclo de gestión de fotos del marcador |
| BT-CASCADA-BORRADO | Backend | E2E: capturar → quitar → el binario ya no se descarga y la revisión no muestra la foto | Borrado limpio, sin registros colgados |
| US-15-QUITAR-FOTO | Regla | Relevamiento cerrado / usuario de otra área → rechazo (RN-05/RN-01) | Correcto |

## 3. Feedback recibido

- **Decisión de no perder datos:** los comentarios que referenciaban la foto se **desvinculan** (quedan a nivel marcador) en vez de borrarse; el agente no pierde lo que escribió.
- **Relación 1:1 foto↔observación:** quitar la foto borra también su observación (una observación sin foto no aporta); evita marcadores con observaciones fantasma.
- El **marcador se conserva** aunque quede sin fotos (auto-eliminar marcadores tiene implicancias de conflictos/unificación; se deja fuera de alcance, documentado).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **475** (431 unitarias + 44 de integración), **+9** (6 del handler `EliminarFotoHandler` —éxito, inexistente, sin marcador, cerrado, no autorizado, desvinculación—, 2 del cliente `QuitarFotoAsync` —DELETE + rechazo—, 1 E2E "capturar → quitar foto"). Suite verde y cobertura del gate **sin regresión y con leve mejora**: Domain 88,5/80,5, Application 87,8/**77,1** (sube por el handler nuevo bien cubierto), `GeoVial.Revision` 97,8/93,3, `GeoVial.Sync` 94,5/90,1. El MAUI compila (`net10.0-android`, arm64) y se redeployó al moto g42 (smoke-test de arranque OK; verificación interactiva con el usuario).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-15-QUITAR-FOTO | Historia | Aceptada (quitar foto del marcador, backend + móvil, RN-05/RN-01) |
| BT-CASCADA-BORRADO | Tarea | Aceptada (cascada correcta + E2E) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido se traslada. |

Pendiente: verificación on-device interactiva (quitar foto → desaparece; cerrado → rechazo). Mejora futura no comprometida: evaluar **auto-eliminar un marcador** que quede sin fotos ni comentarios (requiere considerar conflictos/unificación, CU-12).

## 7. Decisiones tomadas durante el review

- Quitar foto = **borrar foto + observación + binario** y **desvincular** (no borrar) sus comentarios.
- **Sin migración:** la operación sólo borra filas; no cambia el esquema.
- El **marcador permanece** aunque quede vacío (auto-eliminación fuera de alcance).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Sprint review del Sprint 58 (US-15/CU-09 §5.A: quitar una foto). Veredicto Cumplido, velocity 8, 0 carry-over, 475 pruebas (+9: handler, cliente, E2E); cierra US-15 de punta a punta; MAUI redeployado. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
