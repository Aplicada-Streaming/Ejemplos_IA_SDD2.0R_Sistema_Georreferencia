# Sprint Retrospectiva — Sprint 23

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-23_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Los dos E2E del ciclo de conflictos pasaron al primer intento: el truco de provocar el conflicto ampliando el radio tras dos capturas distintas fue limpio y determinista.
- El helper de escenario compartido (`EscenarioE2E`), que siembra su propia área, cerró la acción de la retro del Sprint 20 y dejó las pruebas de conflictos legibles y desacopladas del seed.
- Descubrir que `PuedeAccederArea` autoriza al agente en su área evitó montar un jefe aparte: el ciclo entero (capturar, ajustar radio, detectar, resolver) lo ejecuta un solo actor.
- Con captura, ubicación manual, revisión, edición, autorización y conflictos cubiertos por E2E, los flujos centrales del MVP tienen verificación de punta a punta.

## 2. Qué no salió bien

- El E2E del ciclo de **edición** en conflicto (sync con colisión → listar → confirmar) sigue pendiente: requiere orquestar dos sincronizaciones del mismo recurso, más complejo que el conflicto de radio.
- `CapturaE2ETests` (S20) sigue usando su propio helper acoplado al seed; convendría migrarlo al `EscenarioE2E` compartido para unificar.
- La base en memoria se comparte entre todas las pruebas de integración; con el set de E2E creciendo, conviene vigilar el aislamiento.

## 3. Qué probar

- Un helper de sincronización con colisión que habilite el E2E de edición en conflicto.
- Migrar `CapturaE2ETests` al helper `EscenarioE2E` compartido.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Helper de sincronización con colisión + E2E de edición en conflicto | AG-05 / QA | 2027-05-15 | Pendiente |
| Migrar `CapturaE2ETests` al helper `EscenarioE2E` compartido | AG-05 | 2027-05-15 | Pendiente |
| Pulido móvil pendiente (mapa interactivo, caché de fotos) y SBOM firmado del paquete | AG-08 / AG-09 | 2027-05-15 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 22 | Estado actual |
| --- | --- |
| SBOM CycloneDX firmado del paquete junto al `.nupkg` | Pendiente |
| Publicar v1.0.0 (tag) y verificar la firma del paquete publicado | Pendiente (release del Release manager) |
| Pulido móvil pendiente y E2E del ciclo de conflictos | E2E del ciclo de conflictos (radio) completada en este sprint; pulido móvil pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 23 con 3 acciones nuevas y seguimiento de las del Sprint 22 (E2E del ciclo de conflictos por radio completada; acción de retro S20 del helper desacoplado cerrada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
