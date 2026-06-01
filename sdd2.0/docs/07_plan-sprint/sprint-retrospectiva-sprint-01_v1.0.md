# Sprint Retrospectiva — Sprint 01

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-01_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- La separación de capas (Clean Architecture) permitió cubrir el dominio y la aplicación con pruebas unitarias rápidas y deterministas, sin infraestructura.
- La trazabilidad de la documentación SDD (CU/RN/ADR) se materializó casi uno a uno en código y tests, lo que aceleró la verificación de los criterios de aceptación.
- El gate de "compila sin warnings tratados como error" desde el primer día evitó deuda de calidad acumulada.
- El walking skeleton del Sprint 00 estaba lo bastante firme como para construir el slice sin retrabajo de scaffolding.

## 2. Qué no salió bien

- Aparecieron incoherencias en los criterios de aceptación (CU-03 CA-03 y US-02 CA1 referían "jefe general da de alta un agente", contra la invariante de nivel inmediato inferior). Se detectaron recién al codificar, no en el audit documental.
- La cobertura por capa quedó por debajo del gate en la primera medición; faltaban tests de caminos de borde que hubo que sumar después.
- La cobertura de Infrastructure depende de una base real (Testcontainers) que no se pudo ejercitar en el entorno de trabajo; se difirió.

## 3. Qué probar

- Incorporar una verificación liviana de consistencia entre los criterios de aceptación y las reglas jerárquicas durante el refinamiento, para detectar incoherencias antes de codificar.
- Definir los casos de borde y su cobertura objetivo en la Definition of Ready de la US, no al final del sprint.
- Validar la imagen de Testcontainers en cada máquina del equipo en el Sprint 00 del próximo módulo de persistencia.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Corregir CU-03 CA-03 y US-02 CA1 y comunicar la invariante de jerarquía al equipo | AG-02 / AG-06 | 2026-06-01 | Completada |
| Agregar a la DoR de cada US la lista de casos de borde con cobertura objetivo | AG-06 | 2026-06-09 | Pendiente |
| Planificar BT-07 completo (12 entidades + integración con Testcontainers) en el próximo sprint de persistencia | AG-05 / AG-09 | 2026-06-09 | Pendiente |
| Agendar la rotación de la clave inicial del usuario raíz antes del despliegue | AG-09 | 2026-06-20 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 00 | Estado actual |
| --- | --- |
| Acordar y formalizar la DoD canónica del proyecto | Completada (vive en `08_calidad_y_pruebas/definition-of-done_v1.0.md`) |
| Levantar el walking skeleton verde en CI | Completada (pipeline CI inicial en `.github/workflows/ci.yml`) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 01 con 4 acciones (una completada en el sprint, tres con responsable y fecha). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
