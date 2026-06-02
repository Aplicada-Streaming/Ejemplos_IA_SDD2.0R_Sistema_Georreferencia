# Sprint Retrospectiva — Sprint 28

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-28_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- El patrón keyless del paquete (cosign OIDC + Rekor + verificación in-pipeline) se trasladó casi sin cambios a las imágenes: firma por digest + atestación CycloneDX del SBOM.
- Modelar las tres imágenes como una matriz dejó el workflow compacto y uniforme; agregar una imagen futura es una fila más.
- Crear los Dockerfiles cerró la brecha entre la guía (que ya los referenciaba) y el repo, habilitando build/deploy reproducible.
- El endurecimiento de supply-chain queda completo y simétrico para los dos artefactos del proyecto (paquete e imágenes), cerrando la acción que venía de S25 y S27.

## 2. Qué no salió bien

- Como los sprints de supply-chain previos, el workflow de imágenes sólo se ejercita al taggear `v*` en Actions; no hay build de imagen en el gate local, así que la validación de este sprint fue estructural.
- Los Dockerfiles no se pudieron construir en el entorno de trabajo (sin Docker), por lo que su corrección se apoya en convenciones idiomáticas y en la revisión, no en un build verde local.
- Cuatro de los últimos siete sprints fueron de calidad/DevOps/cierre; el único frente de producto vivo (mapa interactivo) sigue bloqueado por una credencial externa.

## 3. Qué probar

- En el primer tag `v*` (release v1.0.0), validar end-to-end el workflow de imágenes: build, SBOM, firma, atestación y `cosign verify` desde un consumidor.
- Desbloquear la clave de proveedor de mapas para poder retomar un frente de producto (mapa interactivo) y equilibrar la mezcla de sprints.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Publicar v1.0.0 (tag) y validar paquete + imágenes (firma + SBOM) post-publish | AG-09 (Release manager) | 2027-07-24 | Pendiente |
| Gestionar la clave de proveedor de mapas para habilitar el mapa interactivo | AG-08 (móvil) | 2027-07-24 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 27 | Estado actual |
| --- | --- |
| Evaluar SBOM + firma de las imágenes Docker del monolito | Completada (entregada en este sprint) |
| Publicar v1.0.0 (tag) y verificar paquete + firma + SBOM post-publish | Pendiente (se reitera; ahora el tag también dispara las imágenes) |
| Gestionar la clave de proveedor de mapas para el mapa interactivo | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 28 (SBOM + firma de imágenes) con 2 acciones nuevas y seguimiento de las del Sprint 27 (supply-chain de imágenes completada). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
