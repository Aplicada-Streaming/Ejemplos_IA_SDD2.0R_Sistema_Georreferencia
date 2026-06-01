# GeoVial

| Campo | Valor |
| --- | --- |
| Proyecto | GeoVial |
| Versión del documento | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-01 |
| Stack | C# / .NET 9+, Blazor Interactive Server + MudBlazor 7.x, .NET MAUI, SQL Server 2022 + EF Core, SQLite (móvil), OpenStreetMap + Leaflet 1.9.x, ROPC/JWT, Docker, GitHub Actions/Packages, MinVer |
| Tipo de proyecto | web-monolith |
| Documento | README raíz |

Punto de entrada de la documentación SDD de GeoVial. Este README integra y enlaza las categorías generadas; el detalle vive en cada sección y no se replica aquí.

## 1. Identidad del proyecto

GeoVial es un sistema de relevamiento georreferenciado del estado de la infraestructura vial pública (puentes y caminos). Su propósito es reemplazar la recolección manual con planillas y fotos sueltas por un proceso digital en el que agentes de campo capturan observaciones en terreno —fotos, comentarios y etiquetas ancladas a un punto del mapa— con asignación automática de coordenadas, operando sin conexión y sincronizando luego contra un backend central. La evaluación experta se concentra después, de forma centralizada, sobre un mapa donde cada marcador agrupa sus fotos y comentarios.

La propuesta de valor es delegar la recolección en personal menos especializado, georreferenciar automáticamente cada observación al capturar la foto, sostener la operación sin conexión en el terreno y centralizar la revisión sobre un mapa. Sobre esa base, el sistema deja preparada la incorporación futura de fuentes automatizadas de registros (drones, estaciones) que alimenten el mismo proceso, algo que la solución manual actual no permite.

Las audiencias del proyecto son:

- Usuario raíz: administrador técnico que configura el sistema y resuelve incoherencias.
- Jefe general: responsable funcional que administra el alta y baja de los jefes de área.
- Jefe de área: crea relevamientos, gestiona a sus agentes, evalúa y cierra relevamientos.
- Agente de campo: recolecta observaciones en terreno con la app móvil offline.
- Área central de evaluación: consume la información relevada para sus informes rutinarios.
- Equipo de desarrollo SDD 2.0: construye y mantiene el sistema.
- Developers externos: reutilizan la librería de sincronización publicada como paquete.

## 2. Stack y tipo de proyecto

| Componente | Tecnología @ versión | Plataforma soportada |
| --- | --- | --- |
| Lenguaje / plataforma | C# / .NET 9+ (LTS vigente) | Contenedores Linux |
| Front web | Blazor Interactive Server + MudBlazor 7.x | Navegadores evergreen (últimas 2 de Chrome, Edge, Firefox, Safari) |
| App móvil | .NET MAUI con Blazor integrado + MudBlazor | Android 8.0+ (API 26) |
| Base de datos backend | SQL Server 2022 vía EF Core | Contenedor Linux dedicado |
| Persistencia local móvil | SQLite (sqlite-net-pcl) | Almacenamiento del dispositivo Android |
| Mapas | OpenStreetMap + Leaflet 1.9.x | Web y móvil |
| Autenticación | Flujo ROPC con bearer token JWT | API REST |
| Empaquetado y CI/CD | Docker (3 contenedores), GitHub Actions/Packages, MinVer | GitHub |

Tipo D8: `web-monolith`. El artefacto dominante es un backend monolítico modular (Clean Architecture + CQRS ligero) con front web administrativo en Blazor Interactive Server, que además expone una API REST. La multiplicidad de artefactos no se pierde: se elige el tipo dominante y los demás se declaran como sub-proyectos, cada uno con su propia clasificación D8.

| Sub-proyecto | Tipo D8 | Rol en la solución |
| --- | --- | --- |
| App de captura en terreno | mobile-app-maui | Recolección georreferenciada offline con persistencia SQLite y cola de sincronización. |
| Librería de sincronización `GeoVial.Sync` | library | Lógica offline-first publicable en GitHub Packages para reuso por terceros. |
| Librería de alojamiento de archivos `GeoVial.FileHosting` | library | Backends de archivos configurables (local / S3 / otro); interna del backend. |

Las plataformas objetivo son Android 8.0+ en móvil, navegadores evergreen en web y contenedores Linux en despliegue; iOS, tablets y distribución por tiendas quedan fuera de la v1. El despliegue de referencia son tres contenedores —front, backend y base de datos— con el almacenamiento local de archivos sobre el contenedor del backend.

El versionado adopta SemVer 2.0.0 con Conventional Commits y cálculo automático mediante MinVer; la librería de sincronización se publica en GitHub Packages con canales preview y stable, y cualquier cambio incompatible de su superficie pública incrementa la versión mayor.

Flujos de negocio principales (sección opcional web-monolith, §4.3):

Ciclo de un relevamiento de punta a punta:

1. El jefe de área crea un relevamiento y le asigna los agentes de campo de su área.
2. Cada agente inicia sesión y elige uno de los relevamientos asignados.
3. La cuadrilla recolecta observaciones en terreno: fotos, comentarios y etiquetas anclados a marcadores.
4. Al terminar la recolección, el relevamiento transiciona de recolección a revisión.
5. El jefe de área evalúa sobre el mapa, confecciona sus informes y cierra el relevamiento.

Captura en campo sin conexión:

1. El agente inicia sesión con internet la primera vez y configura el método de seguridad del teléfono.
2. En terreno, sin señal, reingresa con PIN o biometría, lo que habilita el modo offline.
3. Por cada foto el sistema resuelve las coordenadas y crea la observación con su marcador, persistida localmente.
4. Al recuperar señal, la app sube primero los cambios locales y luego baja las actualizaciones de los relevamientos del agente.

La guía de despliegue (pipeline, entornos y publicación) vive en [09_devops](09_devops/README.md).

## 3. Mapa de la documentación

Tabla A. Las 12 categorías SDD en orden. La categoría 04 fue omitida por gating y no tiene enlace.

| Categoría | Propósito | Responsable | Enlace |
| --- | --- | --- | --- |
| 00_contexto | Visión, alcance, roadmap, compatibilidad y acuerdo de equipo | AG-00 | [00_contexto](00_contexto/README.md) |
| 01_necesidades_negocio | Necesidades de negocio (NB-01 a NB-06) con dependencias y RACI | AG-01 | [01_necesidades_negocio](01_necesidades_negocio/README.md) |
| 02_especificacion_funcional | Casos de uso (14), reglas de negocio (8), modelo conceptual y requisitos complementarios | AG-02 | [02_especificacion_funcional](02_especificacion_funcional/README.md) |
| 03_ux_ui_dx | Experiencia de uso, 6 wireframes, glosario UX y portal de developers | AG-03 | [03_ux_ui_dx](03_ux_ui_dx/README.md) |
| 04_prompts_ai | Prompts/IA | AG-04 | Omitida por gating (usa_llm=false); decisión formalizada en ADR-12 de 05_arquitectura_tecnica |
| 05_arquitectura_tecnica | Arquitectura de solución, 14 ADR, modelo lógico, flujo de ejecución, contratos y extensibilidad | AG-05 | [05_arquitectura_tecnica](05_arquitectura_tecnica/README.md) |
| 06_backlog-tecnico | Product backlog, backlog técnico, 32 US y Definition of Ready | AG-06 | [06_backlog-tecnico](06_backlog-tecnico/README.md) |
| 07_plan-sprint | Planes Sprint 00 y Sprint 01, plantillas de ceremonias y velocidad | AG-07 | [07_plan-sprint](07_plan-sprint/README.md) |
| 08_calidad_y_pruebas | Estrategia de calidad y testing, plan de pruebas, matriz de cobertura y Definition of Done | AG-08 | [08_calidad_y_pruebas](08_calidad_y_pruebas/README.md) |
| 09_devops | Pipeline CI/CD, versionado, entornos, guías de publicación y supply-chain | AG-09 | [09_devops](09_devops/README.md) |
| 10_developer_guide | Developer guide de la librería de sincronización publicada `GeoVial.Sync` | AG-10 | [10_developer_guide](10_developer_guide/README.md) |
| 11_examples | Samples ejecutables de las librerías de sincronización y de alojamiento | AG-11 | [11_examples](11_examples/README.md) |

Cadena de trazabilidad D6 (referencia conceptual): Visión → NB → CU → RN → ADR → US → BT → Sprint → Test → Pipeline. Cada eslabón se materializa en su categoría: la visión y el alcance en 00, las necesidades en 01, los casos y reglas en 02, las decisiones de arquitectura en 05, las historias y el backlog técnico en 06, la planificación en 07, las pruebas en 08 y la automatización en 09.

Orientación para navegar las secciones generadas:

- 00_contexto define el qué y el porqué en lenguaje de negocio (visión, alcance, roadmap), más el marco de plataformas y el acuerdo de equipo. Es el inicio de la cadena de trazabilidad.
- 01_necesidades_negocio expresa seis necesidades (NB-01 a NB-06) con su impacto, prioridad MoSCoW y mapa de dependencias acíclico.
- 02_especificacion_funcional traduce las necesidades a 14 casos de uso, 8 reglas de negocio, un modelo conceptual de 12 entidades y 7 reglas conceptuales del modelo.
- 03_ux_ui_dx fija la experiencia de uso, seis wireframes sobre los casos con interacción humana y el portal de developers de la librería publicada.
- 05_arquitectura_tecnica concentra la arquitectura de solución, catorce ADR, el modelo lógico, el flujo de ejecución, los contratos y la extensibilidad; ahí vive ADR-12, que formaliza la omisión de la categoría 04.
- 06_backlog-tecnico deriva el product backlog, el backlog técnico y 32 historias de usuario con su Definition of Ready.
- 07_plan-sprint planifica el Sprint 00 (walking skeleton) y el Sprint 01 (primer slice end-to-end de usuarios y jerarquía).
- 08_calidad_y_pruebas establece la estrategia 70/20/10, la matriz de cobertura y la Definition of Done canónica que los gates del pipeline ejecutan.
- 09_devops automatiza el ciclo de vida de los dos tipos de artefacto: las imágenes Docker del monolito y el paquete de la librería de sincronización.
- 10_developer_guide y 11_examples cubren al integrador externo: la primera explica la librería publicada, la segunda la ejecuta en samples progresivos.

## 4. Flujo de lectura recomendado por audiencia

Tabla B. Orden de lectura sugerido según el rol.

| Rol | Orden recomendado | Por qué |
| --- | --- | --- |
| Jefe general / PM | 00 → 01 → 06 → 07 | Necesita la visión y el alcance, las necesidades de negocio y cómo se traducen en backlog y plan de sprints. |
| Desarrollador | 00 → 02 → 05 → 10 → 11 | Necesita contexto, la especificación funcional, la arquitectura y luego la guía e ejemplos para construir. |
| QA | 00 → 02 → 08 | Necesita el contexto, los requisitos (CU, RN, NFR) y la estrategia, matriz y planes de prueba. |
| DevOps | 00 → 05 → 09 | Necesita el contexto, la arquitectura y NFR, y el pipeline, versionado y entornos de despliegue. |
| Developer integrador de la librería de sync | 11 → 10 → 05 | Arranca por los samples ejecutables, pasa a la developer guide y termina en el contrato de la librería en arquitectura. |

Los flujos del organismo (jefe general, jefe de área, agente de campo) priorizan el porqué del producto y su evaluación antes que el detalle técnico, por eso parten de 00 y se apoyan en 01, 06 y 07. Los flujos de construcción (desarrollador, QA, DevOps) convergen en 00 como contexto común y se ramifican según la responsabilidad: especificación y arquitectura para construir, requisitos y matriz de cobertura para validar, arquitectura y pipeline para operar. El flujo del integrador externo es el único que invierte el orden y empieza por lo ejecutable (11), porque su objetivo no es entender GeoVial completo sino reutilizar la librería publicada.

## 5. Cómo contribuir y cómo regenerar la documentación

Esta documentación se produce y regenera con el orquestador SDD 2.0, que asigna un subagente por categoría más el Arquitecto de Soluciones (AG-ROOT) para el README raíz. Cada subagente:

- Lee sus reglas constructivas en `/sdd2.0/devs/rules/`.
- Consume los insumos de intake (`PROJECT-BRIEF` y `PROJECT-README` en `/sdd2.0/devs/intake/`).
- Consume los artefactos upstream ya generados por las categorías anteriores.
- Produce los documentos de su carpeta respetando los decisores D1 a D8 (idioma, nomenclatura, versionado, tipo de proyecto y gating).

Para regenerar: actualizar el insumo de intake correspondiente, reejecutar el subagente de la categoría afectada y, si cambian enlaces, propósitos o estados de sección, reejecutar AG-ROOT para reconciliar este README. Los cambios se versionan en la cabecera de cada documento (campo Versión) y se registran en el control de cambios al pie. El proyecto no incluye `CONTRIBUTING.md` (para `web-monolith` ese archivo se omite según §2.1 de las reglas raíz); la contribución se canaliza por el flujo del equipo descrito en [acuerdo-equipo_v1.0.md](00_contexto/acuerdo-equipo_v1.0.md).

El orden de generación respeta la dependencia upstream/downstream: 00 es el ancla y no tiene upstream generado; cada categoría siguiente consume las anteriores y nunca al revés, lo que mantiene la cadena de trazabilidad coherente. El gating de categorías se decide a partir de los flags declarados en el intake: aquí, `usa_llm=false` omite la categoría 04, mientras que `tiene_portal_developers=true` y la presencia de sub-proyectos `library` promueven la generación de las categorías 10 y 11, que para un `web-monolith` puro serían solo recomendadas. Toda regeneración debe preservar estos invariantes: idioma técnico rioplatense neutro, nomenclatura kebab-case con sufijo de versión, estados dentro del enum cerrado y enlaces relativos sin romper.

## 6. Estado actual y roadmap

Tabla C. Estado y versión vigente por categoría. El detalle de fases y entregables vive en el roadmap enlazado, no se replica aquí.

| Categoría | Estado | Versión vigente |
| --- | --- | --- |
| 00_contexto | Propuesto | 1.0 |
| 01_necesidades_negocio | Propuesto | 1.0 |
| 02_especificacion_funcional | Propuesto | 1.0 |
| 03_ux_ui_dx | Propuesto | 1.0 |
| 04_prompts_ai | Omitida | — |
| 05_arquitectura_tecnica | Propuesto | 1.0 |
| 06_backlog-tecnico | Propuesto | 1.0 |
| 07_plan-sprint | Propuesto | 1.0 |
| 08_calidad_y_pruebas | Propuesto | 1.0 |
| 09_devops | Propuesto | 1.0 |
| 10_developer_guide | Propuesto | 1.0 |
| 11_examples | Propuesto | 1.0 |

El roadmap detallado, con las fases de delivery (F0 scaffolding a F5 cierre), sus criterios de transición y la matriz fase-épica-sprint-release, está en [roadmap-producto_v1.0.md](00_contexto/roadmap-producto_v1.0.md). Como orientación, el horizonte de la v1 es un MVP funcional al cierre de la fase 5, con valor entregado por sprint mediante vertical slicing: el esqueleto navegable arranca en la fase de scaffolding, la jerarquía y el manejo de usuarios se construyen primero sobre web y base y luego sobre móvil, y recién entonces se incorporan los relevamientos del jefe de área y la recolección del agente de campo. No se fijan fechas de calendario porque no hay fecha contractual dura; los criterios de transición entre fases son verificables y viven en el documento enlazado, que es la única fuente de verdad del roadmap.

El estado `Propuesto` de todas las categorías refleja que la documentación está completa y consistente pero pendiente de aprobación formal por el propietario funcional. La categoría 04 figura como `Omitida` porque el flag de gating `usa_llm=false` la excluye; esa decisión está formalizada y trazada en ADR-12 de 05_arquitectura_tecnica y no se reabre desde este README.

## 7. Glosario rápido

Términos esenciales del dominio, uno por línea, como referencia rápida. El glosario completo de presentación vive en [glosario-ux_v1.0.md](03_ux_ui_dx/glosario-ux_v1.0.md) y los términos de dominio funcional, en la categoría [02_especificacion_funcional](02_especificacion_funcional/README.md); este bloque no los reemplaza ni los redefine, solo los resume para orientar al lector que recién ingresa.

- Relevamiento: tarea de registrar el estado de un puente o camino como una serie de marcadores, con estados recolección, revisión y cerrado.
- Observación: conjunto de comentarios y fotos asociados a un punto geográfico.
- Marcador: punto del mapa que agrupa observaciones (fotos y comentarios) y es etiquetable.
- Etiqueta: marca aplicada a fotos o comentarios para su filtrado posterior.
- Sincronización: proceso por el que la app sube primero los cambios locales y luego baja las actualizaciones de los relevamientos del agente.
- Agente de campo: persona que recolecta fotos y comentarios en terreno, etiqueta y administra las fotos de un marcador.
- Jefe de área: usuario que crea relevamientos, gestiona a sus agentes, evalúa la información y cierra relevamientos.
- Jefe general: responsable funcional que administra el alta y baja de los jefes de área.
- Usuario raíz: administrador técnico que configura el sistema, da de alta al jefe general e interviene para resolver incoherencias.
- Georreferenciación: asignación automática de coordenadas a una observación al capturar la foto, priorizando los metadatos EXIF.
- Modo sin conexión: operación de la app en terreno sin señal, con persistencia local y cola de cambios pendientes.
- Conflicto de sincronización: situación —marcadores en un mismo radio o doble edición offline— resuelta con last-write-wins y resolución manual desde la web.

## 8. Contacto y responsables

Cada categoría de documentación tiene un responsable del catálogo de subagentes SDD; los stakeholders del organismo se listan a nivel de rol, sin datos personales, según el BRIEF §2.

| Rol | Responsable | Canal |
| --- | --- | --- |
| Product Manager / Contexto | AG-00 | Categoría 00_contexto |
| Analista de Negocio | AG-01 | Categoría 01_necesidades_negocio |
| Analista Funcional | AG-02 | Categoría 02_especificacion_funcional |
| Diseñador UX/UI/DX | AG-03 | Categoría 03_ux_ui_dx |
| Arquitecto de Software | AG-05 | Categoría 05_arquitectura_tecnica |
| Scrum Master / Backlog | AG-06 / AG-07 | Categorías 06 y 07 |
| QA / SDET | AG-08 | Categoría 08_calidad_y_pruebas |
| DevOps | AG-09 | Categoría 09_devops |
| Technical Writer / Developer Advocate | AG-10 / AG-11 | Categorías 10 y 11 |
| Arquitecto de Soluciones (README raíz) | AG-ROOT | Este documento |
| Propietario funcional | Jefe general del organismo | Aprobación del brief y de la visión |
| Propietario técnico | Usuario raíz | Configuración y resolución de incoherencias |

## 9. Control de cambios

Las modificaciones de este README se registran a continuación; el versionado de cada categoría se controla en su propio documento.

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | README raíz inicial: identidad, stack y tipo D8, mapa de las 12 categorías (04 omitida por ADR-12), flujos de lectura por audiencia, estado y roadmap, glosario rápido, responsables y control de cambios. |
