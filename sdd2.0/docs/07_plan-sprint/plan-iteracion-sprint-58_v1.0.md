# Plan de Iteración — Sprint 58

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-58_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-08-21
**Fecha fin:** 2028-09-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 7,0 SP (S55–S57, aún arrastra el S56=5 acotado); se compromete un sprint de **alcance pleno (8 SP)** que toca backend + móvil.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

**Completar US-15 / CU-09.** Con S57 ya se puede **agregar** una foto a un marcador; comentar y etiquetar venían de S19. Falta el flujo alternativo **§5.A de CU-09: quitar una foto del marcador**. El objetivo es que el agente, parado en una foto del carrusel, pueda **quitarla** (con confirmación), siempre que el relevamiento **no esté cerrado** (RN-05) y tenga acceso al área (RN-01). Con esto US-15 queda cerrada de punta a punta en el móvil.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-15-QUITAR-FOTO | Historia | Quitar una foto de un marcador (backend + móvil), con confirmación, RN-05/RN-01 | Alta | 5 | Dev backend (AG-09) + Dev móvil (AG-08) | Cerrada |
| BT-CASCADA-BORRADO | Tarea | Cascada de borrado correcta (foto + observación + binario; desvincular comentarios) + E2E | Media | 3 | Dev backend (AG-09) | Cerrada |

Total: 8 SP (alcance pleno; primer sprint que vuelve a tocar dominio/aplicación/API tras una racha móvil).

## 4. Alcance técnico

**Decisión de cascada (CU-09 §5.A).** Quitar una foto F (de su observación O, en el marcador M):

1. **Desvincular** los comentarios que referenciaban F (`Comentario.DesvincularFoto` → `FotoId = null`): el comentario **sobrevive** a nivel marcador (conserva texto y etiquetas) en vez de borrarse.
2. Borrar las uniones `FotoEtiqueta` de F.
3. Borrar la `Foto` y su `Observacion` (relación 1:1 — cada captura crea una observación con su foto; una observación sin foto no tiene sentido).
4. Borrar el binario alojado (best-effort, ADR-08): el registro ya quedó eliminado aunque el alojamiento falle.
5. Auditar `ELIMINAR_FOTO` (RN-07). El **marcador permanece** aunque quede sin fotos (auto-eliminar marcadores queda fuera de alcance; tiene implicancias de conflictos/unificación).

**Componentes:**
- **Domain:** `Comentario.DesvincularFoto()`.
- **Application:** `EliminarFotoCommand` + `EliminarFotoHandler` (reusa `AccesoMarcador` para RN-01 + carga de relevamiento; valida RN-05); registro en DI.
- **Abstracciones (puertos):** `IFotoRepository.EliminarAsync`, `IObservacionRepository.EliminarAsync`, `IEtiquetaRepository.EliminarEtiquetasDeFotoAsync`.
- **Infrastructure:** implementación EF de los tres métodos (Remove / RemoveRange). **Sin migración** (sólo borrado de filas, no cambia el esquema).
- **API:** `DELETE /api/v1/fotos/{fotoId}` → `EliminarFotoCommand`.
- **Móvil:** `ClienteEdicionMarcador.QuitarFotoAsync` (DELETE) + botón "🗑 Quitar la foto en foco" en el carrusel (`RevisionPage`) con confirmación; recarga la revisión al éxito.

## 5. Definition of Done aplicada

- Desde el carrusel, parado en una foto, se la quita (con confirmación): el backend borra foto + observación + binario y **desvincula** los comentarios que la referenciaban (no los pierde).
- Si el relevamiento está cerrado, el backend rechaza (RN-05); un usuario de otra área también (RN-01).
- Suite del gate verde (unit + integración), con tests del handler (éxito, inexistente, sin marcador, cerrado, no autorizado, desvinculación), del cliente (DELETE) y E2E (capturar → quitar → ya no está). Cobertura DoD sin regresión.
- El MAUI compila (`net10.0-android`) y se verifica on-device.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Borrado deja registros colgados (FK) | Media | Alto | Orden hijo→padre (FotoEtiqueta → Foto → Observación) + E2E sobre la API real |
| Perder comentarios al borrar la foto | Media | Medio | Se **desvinculan** (no se borran): el comentario queda a nivel marcador |
| Borrado destructivo sin querer | Media | Medio | Confirmación en la UI; auditoría `ELIMINAR_FOTO` (RN-07) |
| Marcador vacío tras quitar la última foto | Baja | Bajo | Se conserva el marcador (auto-eliminar queda fuera de alcance; documentado) |

## 7. Criterios de hecho del sprint

Completo cuando: se puede quitar una foto de un marcador desde el móvil (con confirmación) respetando RN-05/RN-01; el backed borra foto + observación + binario y desvincula sus comentarios; la suite (unit + integración) queda verde con cobertura sin regresión; el MAUI compila y se verifica on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Cierre de US-15 (tras agregar foto en S57): faltaba "quitar foto" (CU-09 §5.A) |
| CU/RN | US-15 / CU-09 §5.A (quitar foto), RN-05 (relevamiento cerrado = sólo lectura), RN-01 (acceso por área), RN-07 (auditoría) |
| Componentes | `GeoVial.Domain` (`Comentario`), `GeoVial.Application` (`EliminarFotoHandler`), `GeoVial.Infrastructure` (repos), `GeoVial.Api` (DELETE), `GeoVial.Revision` (`ClienteEdicionMarcador`), `GeoVial.Mobile` (`RevisionPage`) |
| Calidad | definition-of-done §1.4; gate Domain/Application líneas ≥80 % / ramas ≥70 % |
| Tests | `EliminarFotoHandlerTests`, `ClienteEdicionMarcadorTests` (DELETE), `CapturaE2ETests.Quitar_foto_del_marcador_completo` |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Plan del Sprint 58 (US-15/CU-09 §5.A: quitar una foto de un marcador). Cascada: desvincular comentarios + borrar foto/observación/binario; RN-05/RN-01/RN-07. Backend (Domain/Application/Infra/API) + móvil (`DELETE` + botón con confirmación). Sin migración (sólo borrado de filas). 8 SP (alcance pleno). Generado por AG-07 |
