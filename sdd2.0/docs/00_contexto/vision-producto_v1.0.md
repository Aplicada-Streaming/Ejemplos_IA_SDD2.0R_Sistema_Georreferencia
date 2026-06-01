# Visión de Producto

**Proyecto:** GeoVial
**Documento:** vision-producto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-31
**Autor:** Product Manager Senior (AG-00) + Analista de Negocio Senior (AG-01)
**Trazabilidad upstream:** PROJECT-BRIEF §1, §2, §3, §8, §10, §11, §12; PROJECT-README §1
**Trazabilidad downstream:** 01_necesidades_negocio, 02_especificacion_funcional, 03_ux_ui_dx, 05_arquitectura_tecnica, 07_plan-sprint, 11_examples

## 1. Problema de negocio

Los organismos que controlan obras de infraestructura pública —puentes y caminos— necesitan registrar de forma sistemática el estado de esas obras. Hoy esa tarea se hace a mano: el personal en terreno completa planillas y formularios, y guarda las fotos en soportes digitales sueltos. La información queda dispersa, no está georreferenciada de forma confiable y depende de que la persona que la recolecta tenga criterio experto.

El problema concreto es que la recolección descriptiva y fotográfica de observaciones en obra es manual, lenta y atada a personal especializado. La organización quiere poder delegar la recolección de registros digitales en personal menos especializado (agentes de campo), para que la evaluación experta se concentre después, de forma centralizada, en las áreas correspondientes.

Si esto no se construye en los próximos meses, la organización seguirá dependiendo de que personal experto vaya al terreno, no podrá escalar la cantidad de obras relevadas y quedará sin una base de datos georreferenciada y consistente sobre la cual evaluar el estado de la infraestructura. Además, no podrá dar el paso siguiente que la motiva: incorporar a futuro fuentes automatizadas de registros, como drones o estaciones, alimentando el mismo proceso de recolección.

## 2. Audiencia y stakeholders

La audiencia primaria son los equipos del organismo de control de infraestructura vial: la conducción funcional, los jefes de área administrativa de vialidad, las cuadrillas de relevamiento en terreno y el área central que confecciona informes. La audiencia secundaria es el equipo que construye y mantiene el sistema.

| Rol | Nombre o cargo | Categoría | Nivel de involucramiento | Responsabilidad principal |
|---|---|---|---|---|
| Usuario raíz | Administrador técnico del sistema | Propietario | Alto | Configura el sistema, da de alta al jefe general e interviene para resolver incoherencias |
| Jefe general | Responsable funcional del organismo | Propietario | Alto (aprobador) | Administra el alta y baja de los jefes de área; aprueba el brief y la visión |
| Jefe de área | Jefe de área administrativa de vialidad | Implementador / Beneficiario | Alto | Crea relevamientos, gestiona agentes de su área, evalúa la información y cierra relevamientos |
| Agente de campo | Relevador / cuadrilla técnica | Beneficiario | Alto (operativo) | Recolecta comentarios y fotos en terreno, etiqueta, comenta y administra fotos de un marcador |
| Área central de evaluación | Equipo técnico central del organismo | Beneficiario | Medio | Recibe la información recolectada para confeccionar informes rutinarios |
| Equipo de desarrollo | Equipo SDD 2.0 (4 personas) | Implementador | Alto | Construye y mantiene el sistema |

El propietario que aprueba esta visión es el jefe general del organismo. El financiamiento proviene del presupuesto operativo del organismo titular.

## 3. Propuesta de valor

GeoVial entrega valor diferencial en cinco frentes que la solución manual actual no resuelve:

- Permite delegar la recolección de registros en personal menos especializado, reservando el criterio experto para una evaluación centralizada posterior.
- Georreferencia automáticamente las observaciones al capturar la foto, eliminando la carga manual de coordenadas y la dispersión de las fotos en soportes sueltos.
- Funciona sin conexión en el terreno y sincroniza después, cosa que una planilla y un álbum de fotos no resuelven.
- Centraliza la revisión sobre un mapa, donde cada punto agrupa sus fotos y comentarios, frente al cruce manual actual de planilla contra carpeta de fotos.
- Sienta la base para incorporar a futuro fuentes automatizadas (drones, estaciones) que aporten registros al mismo proceso, algo imposible con la solución manual actual.

## 4. Visión a 3 años

A tres años, GeoVial es la herramienta de referencia del organismo para relevar el estado de puentes y caminos. La recolección de observaciones en terreno se hace de forma mayoritaria por agentes de campo no expertos, mediante captura fotográfica georreferenciada que opera con normalidad en zonas sin cobertura y sincroniza al recuperar señal. Los jefes de área concentran su tiempo en la evaluación sobre mapa y en la confección de informes rutinarios, en lugar de cruzar planillas con carpetas de fotos.

Sobre esa base consolidada, el organismo cuenta con un repositorio georreferenciado, consistente y exportable de la infraestructura relevada, que habilita el siguiente paso estratégico: incorporar fuentes automatizadas de registros (drones, estaciones) que alimenten el mismo proceso de recolección sin rehacer el flujo de trabajo. La visión no contempla, en este horizonte, reemplazar el criterio experto en la evaluación ni automatizar la confección de los informes; el foco se mantiene en escalar la recolección confiable y en preparar al organismo para esas fuentes futuras.

## 5. Objetivos SMART

Objetivos derivados de las métricas de éxito declaradas por el negocio. Los plazos se cuentan desde el lanzamiento del MVP funcional.

| Objetivo | Métrica | Target | Plazo | Responsable |
|---|---|---|---|---|
| Delegar la recolección en personal no experto | Porcentaje de relevamientos recolectados por agentes de campo sobre el total | ≥ 80% | 6 meses post-lanzamiento | Jefe general |
| Aumentar la productividad del relevamiento | Cantidad de obras relevadas por trimestre vs. método manual | +50% | 9 meses post-lanzamiento | Jefe de área |
| Asegurar la confiabilidad de la georreferenciación | Porcentaje de observaciones con coordenada válida asignada automáticamente | ≥ 95% | Continuo, revisión mensual | Jefe de área |
| Hacer eficiente la sincronización de campo | Tiempo de sincronización de una jornada típica de campo | ≤ 5 minutos | 3 meses post-lanzamiento | Equipo de desarrollo |

## 6. Métricas de éxito

| Criterio | Métrica | Target | Plazo | Fuente del dato |
|---|---|---|---|---|
| Delegación de la recolección | Relevamientos recolectados por agentes de campo / total | ≥ 80% | 6 meses post-lanzamiento | Registro de relevamientos del sistema |
| Productividad del relevamiento | Obras relevadas por trimestre vs. línea de base manual | +50% | 9 meses post-lanzamiento | Conteo trimestral de relevamientos cerrados |
| Confiabilidad de la georreferenciación | Observaciones con coordenada válida automática / total | ≥ 95% | Continuo, revisión mensual | Metadatos de las observaciones capturadas |
| Eficiencia de sincronización | Tiempo de sincronización de una jornada típica | ≤ 5 minutos | 3 meses post-lanzamiento | Telemetría de sincronización de la app |
| Adopción del personal de campo | Agentes activos que completan al menos un relevamiento | ≥ 70% (supuesto a validar) | 6 meses post-lanzamiento | Registro de actividad de agentes |

## 7. Restricciones

Restricciones de negocio que condicionan la visión, derivadas de las restricciones declaradas por el cliente:

- La captura en campo debe poder hacerse sin conexión durante el trabajo de terreno; el inicio de sesión requiere internet solo la primera vez.
- El reingreso en campo se hace con los métodos de seguridad del teléfono del agente.
- El tratamiento de datos personales de los agentes y de los registros del organismo está sujeto a la Ley 25.326 de Protección de Datos Personales de Argentina.
- No hay fecha contractual dura: el objetivo es un MVP funcional al cierre de la fase 5 del plan de desarrollo, entregando valor por sprint.
- El presupuesto está cubierto por el presupuesto operativo del organismo titular; el rango orientativo queda a confirmar.
- No se requiere integración con sistemas existentes en la v1.

## 8. Riesgos

| ID | Riesgo | Probabilidad | Impacto | Mitigación | Responsable |
|---|---|---|---|---|---|
| R-01 | Marcadores duplicados dentro de un mismo radio que generan datos inconsistentes | Media | Alto | Resolución manual de conflictos desde la web, con parámetro de radio configurable | Jefe de área |
| R-02 | Pérdida de observaciones capturadas sin conexión antes de sincronizar (pérdida o daño del dispositivo) | Media | Alto | Recordatorio de sincronización al recuperar señal, sincronización automática y política de sincronizar al menos una vez por jornada | Agente de campo / Equipo de desarrollo |
| R-03 | Georreferenciación de baja calidad cuando la foto no trae metadatos de ubicación o el posicionamiento es impreciso | Media | Medio | Priorizar los metadatos de la foto, permitir ajuste manual del punto y agrupar por radio de área | Jefe de área |
| R-04 | Baja adopción por parte del personal de campo menos especializado | Media | Medio | Experiencia de uso simple basada en mapa y puntos, foco en captura rápida, capacitación inicial | Jefe general |

## 9. Glosario del dominio

| Término | Definición | Sinónimos o notas |
|---|---|---|
| Relevamiento | Tarea que consiste en registrar observaciones sobre el estado de un puente o camino; se estructura como una serie de marcadores geográficos y tiene estados (recolección, revisión, cerrado) | — |
| Observación | Conjunto de notas (comentarios) y fotos asociadas a un punto geográfico; las fotos pueden o no estar ligadas a un comentario | — |
| Marcador geográfico | Punto en el mapa que agrupa observaciones (comentarios y fotos) y es etiquetable; varias observaciones pueden compartir un mismo marcador | Pin, punto |
| Etiqueta | Marca que se aplica a fotos o comentarios para su posterior filtrado | Tag |
| Sincronización | Proceso por el cual la app sube primero los cambios locales y luego baja las últimas actualizaciones de los relevamientos del agente | Sync |
| Agente de campo | Persona física que recolecta comentarios y fotos en terreno, etiqueta y administra las fotos de un marcador | Relevador, cuadrilla |
| Jefe de área | Usuario que crea relevamientos, gestiona a los agentes de su área, evalúa la información y cierra relevamientos | — |
| Jefe general | Responsable funcional del organismo que administra el alta y baja de los jefes de área | — |
| Usuario raíz | Usuario ajeno a la administración funcional que configura el sistema, da de alta al jefe general y puede intervenir para resolver incoherencias | — |
| Georreferenciación | Asignación automática de coordenadas geográficas a una observación al momento de capturar la foto | — |
| Modo sin conexión | Capacidad de la app de operar y guardar observaciones localmente en terreno sin señal, para sincronizar luego | Offline |
| Conflicto de sincronización | Situación en la que coexisten cambios incompatibles (por ejemplo, dos marcadores dentro de un mismo radio o ediciones concurrentes del mismo marcador) que requiere decisión desde la web | — |
| Área | Ámbito administrativo de vialidad bajo la responsabilidad de un jefe de área; delimita la autorización sobre relevamientos y agentes | — |

## 10. Trazabilidad

- Upstream: PROJECT-BRIEF §1 (problema), §2 (stakeholders), §3 (propuesta de valor), §8 (métricas SMART), §10 (restricciones), §11 (riesgos), §12 (glosario); PROJECT-README §1 (tipo de proyecto y flags de gating).
- Downstream:
  - 01_necesidades_negocio: deriva necesidades de negocio del problema (§1), la propuesta de valor (§3) y los objetivos SMART (§5).
  - 02_especificacion_funcional: toma el glosario (§9) como vocabulario base para casos de uso y reglas de negocio.
  - 03_ux_ui_dx: parte de la visión (§4) y la mitigación R-04 (§8) para la experiencia de captura simple basada en mapa.
  - 05_arquitectura_tecnica: recibe las restricciones (§7) y los riesgos (§8) como insumos de decisiones de arquitectura.
  - 07_plan-sprint: alinea las prioridades de sprint con los objetivos SMART (§5) y sus plazos.
  - 11_examples: usa el glosario (§9) y la propuesta de valor (§3) para construir ejemplos coherentes con el dominio.

Nota sobre la sección opcional "Modelo operativo y SLA" (§4.3 de las reglas): no se incluye. El proyecto es de uso interno del organismo y no expone una API a consumidores externos; por lo tanto no aplica el criterio que la habilita.
