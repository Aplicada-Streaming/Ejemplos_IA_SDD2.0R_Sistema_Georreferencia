# Alcance del Proyecto

**Proyecto:** GeoVial
**Documento:** alcance-proyecto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-31
**Autor:** Product Manager Senior (AG-00) + Analista de Negocio Senior (AG-01)
**Trazabilidad upstream:** PROJECT-BRIEF §3, §4, §6, §8, §9, §10; PROJECT-README §4, §16
**Trazabilidad downstream:** 01_necesidades_negocio, 02_especificacion_funcional, 03_ux_ui_dx, 05_arquitectura_tecnica, 06_backlog-tecnico, 07_plan-sprint

## 1. Propósito

Este documento delimita qué entra y qué no entra en GeoVial en su versión 1, fija los supuestos sobre los que se construye, registra las restricciones del proyecto y define los criterios de aceptación a nivel de proyecto. Su objetivo es evitar el crecimiento descontrolado del alcance y dejar el contexto cerrado para que las categorías downstream trabajen sin volver a consultar al cliente.

## 2. Descripción general

GeoVial es un sistema que permite al organismo de control de infraestructura vial relevar el estado de puentes y caminos delegando la recolección de observaciones en agentes de campo, para que la evaluación experta se concentre después de forma centralizada. El sistema cubre la captura georreferenciada de observaciones (comentarios y fotos) en terreno con operación sin conexión, la organización del trabajo por relevamientos y áreas, la revisión sobre mapa por parte del jefe de área y el ciclo de estados del relevamiento desde la recolección hasta el cierre.

## 3. Objetivos del proyecto

- Habilitar la recolección de observaciones georreferenciadas por personal de campo no experto, sin carga manual de coordenadas.
- Permitir el trabajo de campo sin conexión, con sincronización automática al recuperar señal.
- Concentrar la evaluación experta sobre un mapa centralizado donde cada punto agrupa sus fotos y comentarios.
- Establecer una base de datos georreferenciada y consistente, exportable e importable como unidad completa.
- Dejar el proceso preparado para incorporar a futuro fuentes automatizadas de registros sin rehacer el flujo.

## 4. Alcance incluido

### 4.1 Capacidades (Must Have, v1)

- Captura de observaciones en terreno desde la app móvil, con asignación automática de coordenadas al tomar la foto.
- Operación sin conexión de la app móvil con mecanismo de sincronización.
- Jerarquía de usuarios y roles: usuario raíz, jefe general, jefe de área y agentes de campo.
- Creación de relevamientos y asignación de agentes por parte del jefe de área.
- Marcadores geográficos que agrupan observaciones (comentarios y fotos), todos etiquetables.
- Mapa con puntos para crear y ubicar marcadores, en web y móvil (mover el punto o centrar por posicionamiento en móvil).
- Visualización de un marcador con carrusel de fotos, comentarios por foto, etiquetas y navegación entre marcadores.
- Flujo de estados del relevamiento: recolección, revisión, cierre.
- Carga manual de observaciones priorizando los metadatos de ubicación de la foto, con parámetro de radio para agrupar fotos en un mismo marcador.
- Exportar e importar un relevamiento completo (datos, comentarios, etiquetas y fotos) en un único archivo comprimido.
- Relevamiento manual a través del sistema web.

### 4.2 Capacidades (Should Have)

- Reasignación de agentes de campo a un relevamiento por parte del jefe de área.
- Revisión por el jefe de área de relevamientos en proceso y ya realizados.
- Resolución desde la web de conflictos de sincronización (por ejemplo, dos marcadores dentro de un mismo radio).

### 4.3 Capacidades (Could Have)

- Filtrado de fotos y observaciones por etiquetas en la revisión web.
- Visor de fotos a pantalla completa con zoom dentro del carrusel.
- Aviso al jefe de área cuando un relevamiento pasa de recolección a revisión.

### 4.4 Entregables

- Sistema web administrativo y de revisión para usuario raíz, jefe general y jefe de área.
- App móvil de captura en terreno para agentes de campo.
- Mecanismo de exportación e importación de un relevamiento completo como archivo comprimido único.
- Base de datos georreferenciada de relevamientos, observaciones, marcadores, fotos, comentarios y etiquetas.

### 4.5 Ambientes

- Ambiente de desarrollo y prueba local durante la codificación.
- Ambiente contenerizado objetivo, validado hacia las fases finales del plan de desarrollo.

## 5. Alcance excluido

| Funcionalidad excluida | Justificación | Versión futura tentativa |
|---|---|---|
| Captura automatizada por drones o estaciones | El cliente la plantea como solución futura habilitada por este sistema, no como parte de la v1; el sistema solo deja la base preparada | Sí, futura |
| Generación automática de los informes de evaluación | El sistema soporta la revisión y evaluación; la confección de los informes rutinarios la hace el jefe de área con criterio experto | No planificado para v1 |
| App móvil en plataformas distintas de Android | La depuración y prueba se plantea sobre dispositivo Android; otras plataformas quedan fuera para acotar el esfuerzo de la v1 | Posible v2.0 |
| Integración con sistemas existentes del organismo | No se requiere integración en la v1; ningún requerimiento la menciona y agregarla ampliaría el alcance sin valor pedido | A evaluar según necesidad futura |
| Distribución de la app por tiendas de aplicaciones | El ciclo de desarrollo prueba la app por conexión directa al dispositivo; la publicación en tiendas no está cubierta en la v1 | Posible v2.0 |
| Soporte de inicio de sesión totalmente sin conexión | El primer inicio de sesión requiere internet; el reingreso en campo usa los métodos de seguridad del teléfono, no un alta de credenciales sin conexión | No planificado |

## 6. Supuestos

Derivados de los apéndices de supuestos de ambos intakes (valores ya fijados para destrabar el flujo; el contenido es funcional y debe confirmarse con el cliente):

- El organismo titular y el jefe general que aprueba el proyecto están identificados a nivel de rol; la identidad nominal se confirmará con el cliente.
- El financiamiento proviene del presupuesto operativo del organismo titular; el rango orientativo se confirmará.
- Los targets y plazos de las métricas de éxito son propuestas del equipo a validar con el cliente.
- Las resoluciones de los casos límite (duplicados en un radio, foto sin ubicación, máximo tiempo sin conexión, edición concurrente, observaciones tras el cierre, reingreso sin método de seguridad) son decisiones de producto propuestas a validar.
- iOS queda fuera de alcance en la v1, sujeto a confirmación del cliente.
- El equipo es de cuatro personas, supuesto usado para las decisiones de proceso y de estilo.

## 7. Restricciones

Restricciones del proyecto derivadas de las restricciones del cliente y de los trade-offs de ingeniería declarados:

- Operación sin conexión obligatoria en campo; el inicio de sesión requiere internet solo la primera vez.
- Reingreso en campo mediante los métodos de seguridad del teléfono.
- Tratamiento de datos personales sujeto a la Ley 25.326 de Protección de Datos Personales de Argentina.
- Sin fecha contractual dura: el objetivo es un MVP funcional al cierre de la fase 5, entregando valor por sprint.
- Presupuesto cubierto por el organismo titular, con rango orientativo a confirmar.
- Sin integración obligatoria con sistemas existentes en la v1.
- Desarrollo y prueba íntegramente local durante la codificación; el entorno contenerizado se valida recién hacia las fases finales.
- Prueba de la app móvil únicamente sobre Android mediante conexión directa al dispositivo; otras plataformas y la distribución por tiendas quedan sin cobertura en el ciclo de desarrollo.
- Resolución de conflictos por última escritura, con marca de conflicto y resolución manual desde la web; puede descartar cambios concurrentes.

## 8. Criterios de aceptación del proyecto

- Un agente de campo puede capturar una observación con foto en terreno sin conexión y obtener coordenadas asignadas automáticamente.
- Las observaciones capturadas sin conexión se sincronizan al recuperar señal, subiendo primero los cambios locales y bajando luego las actualizaciones de los relevamientos del agente.
- Un jefe de área puede crear un relevamiento, asignarle agentes de su área y recorrer las observaciones sobre el mapa con carrusel de fotos.
- El relevamiento transita los estados recolección, revisión y cierre, y queda de solo lectura una vez cerrado.
- Un relevamiento completo se puede exportar e importar como un único archivo comprimido con datos, comentarios, etiquetas y fotos.
- La jerarquía de roles aplica autorización: cada rol solo accede a lo que le corresponde según su nivel y su área.
- Los conflictos de sincronización (por ejemplo, dos marcadores en un mismo radio) quedan listados y resolubles desde la web por el jefe de área.

## 9. Gestión de cambios de alcance

Todo pedido que agregue, quite o modifique capacidades respecto de §4 y §5 se trata como un cambio de alcance. El jefe de área eleva la propuesta al jefe general, que en su rol de propietario funcional la aprueba o la rechaza. Los cambios aprobados se reflejan en una nueva versión de este documento (incremento de versión con guion bajo antes de la `v`) y se propagan a las categorías downstream afectadas (01, 02, 06, 07). Un ítem hoy excluido en §5 que se decida incorporar debe pasar por este flujo antes de generar casos de uso en la categoría 02, para evitar que se construya por error.

## 10. Trazabilidad

- Upstream: PROJECT-BRIEF §3 (propuesta de valor), §4 (MoSCoW), §6 (flujos), §8 (métricas), §9 (exclusiones), §10 (restricciones); PROJECT-README §4 (delivery), §16 (restricciones y trade-offs).
- Downstream:
  - 01_necesidades_negocio: traduce las capacidades incluidas (§4) en necesidades de negocio priorizadas.
  - 02_especificacion_funcional: genera casos de uso solo sobre el alcance incluido (§4) y respeta las exclusiones (§5).
  - 03_ux_ui_dx: diseña la experiencia de las capacidades incluidas (§4.1, §4.2, §4.3).
  - 05_arquitectura_tecnica: toma las restricciones (§7) como condicionantes de diseño.
  - 06_backlog-tecnico: deriva épicas e ítems del alcance incluido (§4) y respeta los criterios de aceptación (§8).
  - 07_plan-sprint: ordena el trabajo según prioridad MoSCoW del alcance incluido (§4).

Nota sobre la sección opcional "NFR de compatibilidad" (§4.3 de las reglas): los requisitos no funcionales de plataforma del sub-proyecto móvil se tratan en el documento compatibilidad-plataformas_v1.0.md de esta misma carpeta, para no duplicar la matriz de plataformas en el alcance.

Nota sobre la sección opcional "Estrategia de internacionalización" (§4.3 de las reglas): no aplica. La audiencia es un único organismo en una sola región lingüística (español), por lo que no se declara estrategia de internacionalización.
