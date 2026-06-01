# Roadmap del Producto

**Proyecto:** GeoVial
**Documento:** roadmap-producto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-31
**Autor:** Product Manager Senior (AG-00) + Analista de Negocio Senior (AG-01)
**Trazabilidad upstream:** PROJECT-BRIEF §4, §6, §10; PROJECT-README §4, §16
**Trazabilidad downstream:** 06_backlog-tecnico, 07_plan-sprint

## 1. Propósito

Este documento ordena la construcción de GeoVial en fases con criterios de transición verificables, para que cada fase quede funcional respecto de la anterior y entregue valor demostrable por sprint. Mapea las fases de delivery declaradas en el plan del proyecto como hitos con su objetivo, sus épicas asociadas y su entregable, y deja la base para que las categorías 06 (backlog) y 07 (plan de sprint) deriven el detalle. No fija fechas de calendario porque no hay fecha contractual dura; el horizonte es el MVP funcional al cierre de la fase 5, con valor entregado por sprint.

## 2. Fases del producto

Se adopta vertical slicing dentro de un flujo Scrum: cada fase entrega un incremento funcional probado. Las épicas se nombran a nivel de negocio; su descomposición fina es responsabilidad de la categoría 06.

| Fase | Objetivo | Épicas asociadas | Sprints estimados | Entregable | Release target |
|---|---|---|---|---|---|
| F0 — Scaffolding | Crear el esqueleto de todos los proyectos (walking skeleton) que arranca de punta a punta | Esqueleto de solución y ambientes | 1 | Esqueleto navegable que compila y arranca | Interno (Sprint 0) |
| F1 — Usuarios y jerarquía (web + base) | Primer slice end-to-end: jerarquía y manejo de usuarios sobre backend, front y base de datos | Gestión de usuarios y jerarquía; autenticación y autorización por rol | 1 a 2 | Alta/baja según jerarquía e inicios de sesión funcionando en web | MVP incremental 1 |
| F2 — Usuarios y jerarquía (móvil) | Llevar la jerarquía y el manejo de usuarios a la app móvil, alineada al backend y al front | Manejo de usuarios en móvil; inicio de sesión móvil | 1 | App móvil con inicio de sesión y jerarquía coherente con el backend | MVP incremental 2 |
| F3 — Relevamientos (jefe de área) | Alta, baja, visualización y creación de marcadores; asignación de agentes | Gestión de relevamientos; marcadores y mapa; asignación de agentes | 2 | Jefe de área crea relevamientos, ubica marcadores y asigna agentes | MVP incremental 3 |
| F4 — Recolección (agente de campo) | Visualización y recolección en terreno, con datos de prueba y previsualización manual | Captura georreferenciada; operación sin conexión y sincronización; observaciones, fotos, comentarios y etiquetas | 2 a 3 | Agente recolecta observaciones sin conexión y sincroniza; revisión web sobre mapa | MVP funcional (cierre fase 5 del plan) |
| F5 — Cierre | Completar los puntos restantes para finalizar el desarrollo y consolidar el ciclo de estados | Cierre de relevamiento; exportación/importación; resolución de conflictos | 1 a 2 | Ciclo recolección-revisión-cierre completo; exportar/importar relevamiento | MVP funcional consolidado |
| F6 — Imágenes de servicios | Generar las imágenes de los servicios para el entorno objetivo | Empaquetado de imágenes de los servicios | 1 | Imágenes de los servicios listas para el entorno contenerizado | Entrega final |

## 3. Matriz fase → épica → sprint → release

| Fase | Épica (nivel negocio) | Sprint estimado | Release |
|---|---|---|---|
| F0 | Esqueleto de solución y ambientes | S0 | Interno |
| F1 | Gestión de usuarios y jerarquía | S1 | MVP incremental 1 |
| F1 | Autenticación y autorización por rol | S1–S2 | MVP incremental 1 |
| F2 | Manejo de usuarios en móvil | S2 | MVP incremental 2 |
| F3 | Gestión de relevamientos | S3 | MVP incremental 3 |
| F3 | Marcadores y mapa | S3–S4 | MVP incremental 3 |
| F3 | Asignación de agentes | S4 | MVP incremental 3 |
| F4 | Captura georreferenciada | S4–S5 | MVP funcional |
| F4 | Operación sin conexión y sincronización | S5 | MVP funcional |
| F4 | Observaciones, fotos, comentarios y etiquetas | S5–S6 | MVP funcional |
| F5 | Cierre de relevamiento | S6 | MVP funcional consolidado |
| F5 | Exportación / importación | S6–S7 | MVP funcional consolidado |
| F5 | Resolución de conflictos | S7 | MVP funcional consolidado |
| F6 | Empaquetado de imágenes de los servicios | S7 | Entrega final |

La numeración de sprints es estimada y orientativa; la categoría 07 fija la asignación definitiva. Las épicas son provisionales hasta su confirmación en la categoría 06.

## 4. Dependencias entre fases

- F1 depende de F0: requiere el esqueleto de proyectos y ambientes para construir el primer slice end-to-end.
- F2 depende de F1: la jerarquía y el manejo de usuarios en móvil deben alinearse al backend y al front ya construidos.
- F3 depende de F1: la gestión de relevamientos se apoya en los usuarios, las áreas y la autorización por rol.
- F4 depende de F2 y F3: la recolección en campo necesita el manejo de usuarios en móvil y los relevamientos con marcadores y agentes asignados.
- F5 depende de F4: el cierre, la exportación/importación y la resolución de conflictos operan sobre relevamientos ya recolectados y sincronizados.
- F6 depende de F5: las imágenes de los servicios se generan sobre el desarrollo ya finalizado.

## 5. Criterios de transición entre fases

| Fase origen | Fase destino | Criterios verificables |
|---|---|---|
| F0 | F1 | - [ ] Todos los proyectos de la solución creados y compilando<br>- [ ] El esqueleto arranca de punta a punta en el ambiente local<br>- [ ] Pipeline de compilación verde sobre el esqueleto |
| F1 | F2 | - [ ] Alta y baja de usuarios según jerarquía funcionando en web<br>- [ ] Inicio de sesión por rol operativo<br>- [ ] Autorización por rol verificada con pruebas unitarias |
| F2 | F3 | - [ ] Inicio de sesión móvil funcionando y alineado al backend<br>- [ ] Jerarquía de usuarios coherente entre web y móvil<br>- [ ] Slice de usuarios móvil probado |
| F3 | F4 | - [ ] Jefe de área crea, da de baja y visualiza relevamientos<br>- [ ] Marcadores creados y ubicados sobre el mapa<br>- [ ] Asignación de agentes a un relevamiento operativa |
| F4 | F5 | - [ ] Captura georreferenciada de observaciones desde la app<br>- [ ] Operación sin conexión y sincronización al recuperar señal probadas<br>- [ ] Revisión sobre mapa con carrusel de fotos disponible<br>- [ ] Datos de prueba y previsualización manual disponibles |
| F5 | F6 | - [ ] Ciclo de estados recolección-revisión-cierre completo, con cierre de solo lectura<br>- [ ] Exportar e importar un relevamiento completo como archivo único<br>- [ ] Resolución de conflictos desde la web operativa |
| F6 | Entrega | - [ ] Imágenes de los servicios generadas por script<br>- [ ] Servicios levantando en el entorno contenerizado objetivo<br>- [ ] Almacenamiento local de archivos sobre el servicio de backend verificado |

## 6. Trazabilidad downstream

- Upstream: PROJECT-BRIEF §4 (capacidades MoSCoW que alimentan las épicas), §6 (flujos de punta a punta que ordenan F3 a F5), §10 (sin fecha dura, valor por sprint); PROJECT-README §4 (las seis fases de delivery más la fase de imágenes), §16 (restricción de desarrollo local y validación contenerizada tardía que ubica F6 al final).
- Downstream:
  - 06_backlog-tecnico: convierte las épicas de §2 y §3 en ítems de backlog con su Definition of Ready; confirma la descomposición provisional aquí declarada.
  - 07_plan-sprint: toma la matriz de §3 y los criterios de transición de §5 para planificar sprints y validar el cierre de cada fase contra su checklist.
