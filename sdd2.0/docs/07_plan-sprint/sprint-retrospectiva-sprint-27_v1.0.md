# Sprint Retrospectiva — Sprint 27

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-27_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- La migración fue de bajo riesgo y alto rédito: `EscenarioE2E` ya cubría todo lo necesario, así que reescribir `CapturaE2ETests` sobre él fue mecánico y dejó la suite verde sin cambios de conteo.
- Sembrar la propia área cerró el último acoplamiento al seed de Development que arrastraba la prueba de captura desde el Sprint 20.
- Modelar la autorización por área con un segundo escenario evitó reintroducir andamiaje específico: la prueba quedó más expresiva.
- Se saldó una deuda que se venía reiterando en tres retros (S24/S25/S26) con un sprint acotado y limpio.

## 2. Qué no salió bien

- La deuda quedó tres sprints en la lista de acciones antes de priorizarse: las tareas de calidad sin presión de producto tienden a postergarse.
- El gate de cobertura no cambia con este tipo de trabajo (es refactor de pruebas), por lo que su valor se evidencia solo en mantenibilidad, no en una métrica.

## 3. Qué probar

- Cuando aparezca una nueva prueba E2E, partir de `EscenarioE2E` desde el inicio para no volver a generar andamiaje paralelo.
- Atacar las deudas de calidad pequeñas en cuanto aparecen, intercalándolas con el frente de producto, para que no se acumulen.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Publicar v1.0.0 (tag) y verificar paquete + firma + SBOM post-publish | AG-09 (Release manager) | 2027-07-10 | Pendiente |
| Gestionar la clave de proveedor de mapas para habilitar el mapa interactivo | AG-08 (móvil) | 2027-07-10 | Pendiente |
| Evaluar SBOM + firma de las imágenes Docker del monolito (stages de imagen) | AG-09 (DevOps) | 2027-07-10 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 26 | Estado actual |
| --- | --- |
| Migrar `CapturaE2ETests` al helper `EscenarioE2E` compartido | Completada (entregada en este sprint) |
| Publicar v1.0.0 (tag) y verificar paquete + firma + SBOM post-publish | Pendiente (se reitera; acto del Release manager) |
| Gestionar la clave de proveedor de mapas para el mapa interactivo | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 27 (migración E2E al helper) con 3 acciones nuevas y seguimiento de las del Sprint 26 (migración completada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
