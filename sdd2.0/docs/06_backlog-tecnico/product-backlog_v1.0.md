# Product Backlog — GeoVial

**Proyecto:** GeoVial
**Documento:** product-backlog_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Scrum Master / Agile Coach (AG-06), Equipo SDD 2.0
**Tipo de proyecto:** web-monolith
**Estimación adoptada:** Fibonacci (1, 2, 3, 5, 8, 13, 21)
**Trazabilidad upstream:** 01 (NB-01 a NB-06); 02 (CU-01 a CU-14, RN-01 a RN-08); 05 (arquitectura-solución, ADR-01 a ADR-14, contratos)
**Trazabilidad downstream:** backlog-tecnico_v1.0.md, definition-of-ready_v1.0.md, 07_plan-sprint, 08_calidad_y_pruebas

## 1. Objetivos del producto y MVP

El backlog de GeoVial ordena la construcción de un sistema que delega la recolección georreferenciada de observaciones sobre puentes y caminos en agentes de campo no expertos, operando sin conexión en terreno y concentrando la revisión y evaluación sobre un mapa en el jefe de área. El MVP buscado —objetivo de cierre de la fase de delivery— cubre las necesidades Must del catálogo de negocio: delegación de la recolección con jerarquía de usuarios (NB-01), georreferenciación automática y confiable (NB-02), continuidad operativa sin conexión con sincronización (NB-03), revisión centralizada sobre mapa con exportación e importación (NB-04) y trazabilidad y protección de datos personales (NB-06). La consistencia de datos ante conflictos (NB-05) entra como Should: el MVP funciona con detección y resolución manual de conflictos, pero el sistema es defendible sin ellos en el primer corte.

El MVP queda definido por las US Must de las épicas EP-01 a EP-05, EP-07 y EP-08. Las épicas EP-06 (resolución de conflictos) y EP-09 (librería de sincronización publicada) aportan valor incremental Should sobre ese núcleo.

## 2. Épicas

| EP | Nombre | Descripción | NB origen | Sprints estimados |
| --- | --- | --- | --- | --- |
| EP-01 | Jerarquía, usuarios y acceso | Alta y baja jerárquica de usuarios, asociación a área, inicio de sesión y relogueo en terreno | NB-01 | 1-2 |
| EP-02 | Relevamientos y asignación | Creación de relevamientos, radio de agrupación, asignación y reasignación de agentes, estados | NB-01, NB-04 | 1-2 |
| EP-03 | Captura y georreferenciación | Captura de observaciones con coordenada automática, ubicación manual, gestión de marcador | NB-02, NB-04 | 2 |
| EP-04 | Sincronización offline | Recolección sin conexión, cola de cambios, subir/bajar y detección de conectividad | NB-03 | 2 |
| EP-05 | Revisión sobre mapa | Revisión por marcadores sobre mapa, carrusel y filtrado por etiquetas | NB-04 | 1-2 |
| EP-06 | Resolución de conflictos | Detección de marcadores en un mismo radio y resolución manual desde la web | NB-05 | 1 |
| EP-07 | Exportación e importación | Exportar e importar el relevamiento completo en un único archivo | NB-04 | 1 |
| EP-08 | Auditoría y datos personales | Registro inalterable de accesos y acciones, retención y autorización por rol y área | NB-06 | 1-2 |
| EP-09 | Librería de sincronización publicada | Demo autónoma y superficie pública reutilizable de la librería de sincronización | NB-03 | 1 |

## 3. Historias por épica

Estimación en story points (SP) con técnica Fibonacci. Estado del enum: `Borrador | Ready | En curso | Done | Descartada`. Las US viven en archivos individuales bajo `historias-usuario/` por superar el umbral de 20 US (§3.3 de las reglas; 32 US).

### EP-01 Jerarquía, usuarios y acceso

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-01 | Administrar alta y baja jerárquica de usuarios | Must | 8 | Ready | CU-03 | EP-01 |
| US-02 | Asociar usuarios a su área | Must | 3 | Ready | CU-03 | EP-01 |
| US-03 | Intervención del usuario raíz por incoherencia | Should | 5 | Borrador | CU-03 | EP-01 |
| US-04 | Iniciar sesión y seleccionar relevamiento asignado | Must | 5 | Ready | CU-02 | EP-01 |
| US-05 | Relogueo en terreno con método de seguridad | Must | 5 | Ready | CU-02 | EP-01 |

### EP-02 Relevamientos y asignación

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-06 | Crear relevamiento con radio de agrupación | Must | 5 | Ready | CU-01 | EP-02 |
| US-07 | Asignar agentes al relevamiento | Must | 3 | Ready | CU-01 | EP-02 |
| US-08 | Reasignar agentes de un relevamiento | Should | 3 | Borrador | CU-01 | EP-02 |
| US-09 | Transicionar estados del relevamiento | Must | 5 | Ready | CU-10 | EP-02 |
| US-10 | Reabrir un relevamiento cerrado | Should | 3 | Borrador | CU-10 | EP-02 |

### EP-03 Captura y georreferenciación

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-11 | Capturar observación con georreferenciación automática | Must | 8 | Ready | CU-04 | EP-03 |
| US-12 | Agrupar observaciones en marcador por radio | Must | 5 | Ready | CU-04 | EP-03 |
| US-13 | Ubicar manualmente el punto de la observación | Must | 5 | Ready | CU-05 | EP-03 |
| US-14 | Cargar fotos desde la web priorizando metadatos | Must | 5 | Ready | CU-05 | EP-03 |
| US-15 | Gestionar fotos, comentarios y etiquetas del marcador | Must | 8 | Ready | CU-09 | EP-03 |

### EP-04 Sincronización offline

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-16 | Recolectar observaciones sin conexión | Must | 8 | Ready | CU-06 | EP-04 |
| US-17 | Encolar los cambios locales para sincronizar | Must | 5 | Ready | CU-06 | EP-04 |
| US-18 | Sincronizar subiendo y bajando cambios | Must | 13 | Ready | CU-07 | EP-04 |
| US-19 | Detectar la conectividad y sincronizar automáticamente | Must | 5 | Ready | CU-07 | EP-04 |
| US-20 | Avisar al jefe de área cuando el relevamiento pasa a revisión | Could | 2 | Borrador | CU-10 | EP-04 |

### EP-05 Revisión sobre mapa

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-21 | Revisar el relevamiento sobre el mapa por marcadores | Must | 8 | Ready | CU-08 | EP-05 |
| US-22 | Recorrer el carrusel y navegar entre marcadores | Must | 5 | Ready | CU-09 | EP-05 |
| US-23 | Filtrar fotos y observaciones por etiquetas | Could | 3 | Borrador | CU-08 | EP-05 |
| US-24 | Visor de fotos a pantalla completa con zoom | Could | 3 | Borrador | CU-09 | EP-05 |

### EP-06 Resolución de conflictos

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-25 | Detectar marcadores en conflicto por radio configurable | Should | 5 | Borrador | CU-11 | EP-06 |
| US-26 | Resolver conflictos de sincronización desde la web | Should | 8 | Borrador | CU-12 | EP-06 |

### EP-07 Exportación e importación

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-27 | Exportar el relevamiento completo en un único archivo | Must | 5 | Ready | CU-08 | EP-07 |
| US-28 | Importar el relevamiento completo desde un archivo | Must | 8 | Ready | CU-08 | EP-07 |

### EP-08 Auditoría y datos personales

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-29 | Registrar accesos y acciones administrativas en auditoría | Must | 8 | Ready | CU-13 | EP-08 |
| US-30 | Consultar el historial de auditoría con retención | Must | 5 | Ready | CU-13 | EP-08 |
| US-31 | Autorizar cada acceso por rol y área | Must | 8 | Ready | CU-14 | EP-08 |

### EP-09 Librería de sincronización publicada

| US | Título | MoSCoW | SP | Estado | CU relacionados | Épica |
| --- | --- | --- | --- | --- | --- | --- |
| US-32 | Evaluar la librería de sincronización con una demo autónoma | Should | 8 | Borrador | CU-06, CU-07 | EP-09 |

Nota de numeración: la secuencia de US es continua y de dos dígitos (US-01 a US-32). EP-04 incluye una US `Could` (US-20) de aviso que comparte CU con la transición de estados pero aporta valor de notificación al jefe de área.

## 4. Métricas de avance

Total de US: 32.

| Prioridad | Cantidad de US | Porcentaje de US |
| --- | --- | --- |
| Must | 23 | 71,9 % |
| Should | 6 | 18,8 % |
| Could | 3 | 9,4 % |
| Won't (v1.0) | 0 | 0 % |

- US en estado `Ready`: 23 (todas Must del MVP). US en `Borrador`: 9 (Should y Could pendientes de refinamiento).
- Porcentaje cerrado (`Done`): 0 % (backlog inicial, sin sprints ejecutados).
- Deuda en backlog: 9 US en `Borrador` (Should/Could) pendientes de pasar la Definition of Ready; ninguna US Must queda sin refinar.
- Won't (v1.0) explícitos fuera de alcance (BRIEF §9, no entran como US): captura por drones o estaciones, generación automática de informes de evaluación, soporte móvil fuera de Android.

## 5. Refinamiento

- Cadencia: una sesión de Backlog Refinement por sprint, como mínimo (§2.2 web-monolith), con foco en pasar las US `Borrador` (Should/Could) a `Ready` antes de su Sprint Planning.
- Responsables: el Scrum Master (AG-06) facilita; el rol de Product Owner funcional (jefe general / jefe de área) prioriza el valor; AG-02 firma la trazabilidad US↔CU; AG-05 valida que las BT tracen a componentes y ADR; AG-08 valida que los criterios de aceptación alimenten los acceptance tests de 08.
- Técnica de estimación: Fibonacci (1, 2, 3, 5, 8, 13, 21) mediante Planning Poker. La técnica se declara aquí y se mantiene en todo el backlog y el backlog-tecnico, sin mezclar con horas ideales (anti-patrón §4.8).
- Criterio de corte: una US que supere 13 SP en refinamiento se descompone; ninguna US debe exceder un sprint (atributo Small de INVEST).

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Product backlog inicial: 9 épicas, 32 US derivadas de los 14 CU y las 6 NB, estimación Fibonacci, MoSCoW 71,9/18,8/9,4. Generado por AG-06 |
