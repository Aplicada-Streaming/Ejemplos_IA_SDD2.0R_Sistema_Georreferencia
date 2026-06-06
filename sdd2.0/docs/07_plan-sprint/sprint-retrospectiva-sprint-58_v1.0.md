# Sprint Retrospectiva — Sprint 58

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-58_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **US-15 cerrada de punta a punta.** Agregar (S57) + quitar (S58) + comentar/etiquetar (S19): la gestión de fotos del marcador queda completa en el móvil.
- **Cascada de borrado sin sorpresas.** Se pensó qué pasa con cada dependencia antes de codear: comentarios → desvincular (no perder texto), foto/observación → borrar (1:1), binario → best-effort, etiquetas → borrar. El E2E sobre la API real confirmó que no quedan registros colgados.
- **Sin migración.** Al ser sólo borrado de filas, no cambió el esquema; se reusó el patrón ya existente (`IMarcadorRepository.EliminarAsync`, `IAlmacenFotos.EliminarAsync`).
- **Cobertura mejoró** (Application a 87,8/77,1) por los 6 tests del handler con los fakes ya existentes (`Dobles.cs`): el handler quedó probado en sus cinco caminos de rechazo + el happy path + la desvinculación.

## 2. Qué no salió bien

- **Detener el backend para el gate.** Correr los tests de integración exige compilar el `Api`, bloqueado por el backend de desarrollo en ejecución; hubo que pararlo, correr el gate, relanzarlo y **re-sembrar** los usuarios (InMemory) con el script de `DATOS-DE-PRUEBA.md`. Es fricción recurrente; conviene un atajo (script que pare/corra/relance/seed) o una base persistente local para QA.
- **Decisión de producto tomada por el equipo.** "Desvincular vs. borrar comentarios" y "conservar el marcador vacío" se resolvieron con criterio de industria, pero son decisiones de producto: convendría confirmarlas con el PO.
- **Verificación interactiva pendiente.** El smoke-test confirma arranque; el flujo de quitar foto se valida con el usuario en el dispositivo (sigue la acción de QA on-device sistemática de S56/S57).

## 3. Qué probar

- On-device: carrusel → foto en foco → "Quitar" → confirmar → la foto desaparece del marcador; el binario ya no se ve.
- Relevamiento **cerrado** → el backend rechaza y la UI lo informa sin romperse.
- Un marcador con comentario "sobre la foto": tras quitar la foto, el comentario **sigue** a nivel marcador.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Verificar on-device "quitar foto" (éxito / cerrado / comentario desvinculado) | AG-05 (QA) | 2028-09-15 | En curso (con el usuario) |
| Script único parar→test→relanzar→seed del backend de desarrollo | AG-09 (backend) | 2028-09-15 | Pendiente |
| Confirmar con el PO las decisiones de cascada (desvincular comentarios; conservar marcador vacío) | AG-07 | 2028-09-15 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 57 | Estado actual |
| --- | --- |
| Verificar on-device "agregar foto al marcador" (cámara/galería/cerrado/offline) | En curso (con el usuario) |
| Documentar el build MAUI con RID explícito (`android-arm64`) y APIs obsoletas net10 | Aplicado este sprint (se compiló con `-p:RuntimeIdentifier=android-arm64`); falta documentarlo |
| Evaluar "quitar foto" de un marcador | **Hecho** (este sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Retrospectiva del Sprint 58 (US-15/CU-09 §5.A: quitar foto). US-15 cerrada de punta a punta; cascada de borrado pensada y verificada por E2E; sin migración. Fricción: parar el backend para el gate + re-seed. Pendiente: confirmar decisiones de cascada con el PO y un script de ciclo del backend. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
