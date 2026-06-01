# PROJECT BRIEF — GeoVial

```yaml
Nombre del Proyecto: geovial
Cliente / Stakeholder principal: Organismo de control de infraestructura vial (titular del sistema) — (Supuesto a validar)
Documento: PROJECT-BRIEF-geovial_v1.0.md
Versión: 1.0
Fecha: 2026-05-31
Autor del brief: Equipo SDD 2.0 — Analista de negocio (AG-01) y Product Manager (AG-00)
Estado: En revisión
```

> Este documento captura **lo que el cliente quiere** en lenguaje de negocio.
> NO incluye decisiones técnicas (stack, arquitectura, deployment, etc.).
> Para eso ver el documento PROJECT-README.

> **Nota de procedencia.** Los valores marcados **(Supuesto a validar)** son propuestas
> del equipo que reemplazan datos que solo el cliente puede confirmar. Están listados en
> el apéndice "Supuestos a validar con el cliente". El resto son hechos derivados de
> `geovial-requerimientos.md` o decisiones de producto tomadas por el equipo.

---

## §1 Idea y problema

Los organismos que controlan obras de infraestructura pública —puentes y caminos— necesitan registrar de forma sistemática el estado de esas obras. Hoy esa tarea se hace a mano: el personal en terreno completa planillas Excel y formularios, y guarda las fotos en soportes digitales sueltos. La información queda dispersa, no está georreferenciada de forma confiable y depende de que la persona que la recolecta tenga criterio experto.

El problema concreto es que la recolección descriptiva y fotográfica de observaciones en obra es manual, lenta y atada a personal especializado. La organización quiere poder delegar la recolección de registros digitales en personal menos especializado (agentes de campo), para que la evaluación experta se concentre después, de forma centralizada, en las áreas correspondientes.

Si esto no se construye en los próximos meses, la organización seguirá dependiendo de que personal experto vaya al terreno, no podrá escalar la cantidad de obras relevadas y quedará sin una base de datos georreferenciada y consistente sobre la cual evaluar el estado de la infraestructura. Además, no podrá dar el paso siguiente que la motiva: incorporar a futuro fuentes automatizadas de registros, como drones o estaciones, alimentando el mismo proceso de recolección.

---

## §2 Audiencia y stakeholders

| Rol | Nombre o cargo | Categoría | Responsabilidad principal |
|---|---|---|---|
| Usuario raíz | Administrador técnico del sistema | Propietario / Operador | Configura el sistema, da de alta al jefe general e interviene para resolver incoherencias |
| Jefe general | Responsable funcional del organismo | Propietario funcional | Administra el alta y baja de los jefes de área; aprueba el brief |
| Jefe de área | Jefe de área administrativa de vialidad | Implementador del proceso / Beneficiario | Crea relevamientos, gestiona agentes de su área, evalúa la información y cierra relevamientos |
| Agente de campo | Relevador / cuadrilla técnica | Beneficiario / Usuario operativo | Recolecta comentarios y fotos en terreno, etiqueta, comenta y administra fotos de un marcador |
| Área central de evaluación | Equipo técnico central del organismo | Beneficiario | Recibe la información recolectada para confeccionar informes rutinarios |
| Equipo de desarrollo | Equipo SDD 2.0 | Implementador | Construye y mantiene el sistema |

Propietario que aprueba el brief: el jefe general del organismo. Financiamiento: presupuesto del organismo titular **(Supuesto a validar)**.

---

## §3 Propuesta de valor y diferenciación

- Permite **delegar la recolección de registros en personal menos especializado**, reservando el criterio experto para una evaluación centralizada posterior.
- **Georreferencia automáticamente** las observaciones al capturar la foto, eliminando la carga manual de coordenadas y la dispersión de las fotos en soportes sueltos.
- Funciona **sin conexión en el terreno** y sincroniza después, cosa que una planilla y un álbum de fotos no resuelven.
- Centraliza la **revisión sobre un mapa**, donde cada punto agrupa sus fotos y comentarios, frente al cruce manual actual de planilla contra carpeta de fotos.
- Sienta la base para **incorporar a futuro fuentes automatizadas** (drones, estaciones) que aporten registros al mismo proceso, algo imposible con la solución manual actual.

---

## §4 Alcance funcional pretendido (alto nivel)

**Must Have (v1)**

- Captura de observaciones en terreno desde app móvil, con asignación automática de coordenadas al tomar la foto.
- Operación offline de la app móvil con mecanismo de sincronización.
- Jerarquía de usuarios y roles: usuario raíz, jefe general, jefe de área y agentes de campo.
- Creación de relevamientos y asignación de agentes por parte del jefe de área.
- Marcadores geográficos que agrupan observaciones (comentarios y fotos), todos etiquetables.
- Mapa con pines para crear y ubicar puntos, en web y móvil (mover el pin o centrar por GPS en móvil).
- Visualización de un marcador con carrusel de fotos, comentarios por foto, etiquetas y navegación entre marcadores.
- Flujo de estados del relevamiento: recolección → revisión → cierre.
- Carga manual de observaciones priorizando los metadatos EXIF de la foto, con parámetro de radio para agrupar fotos en un mismo marcador.
- Exportar e importar un relevamiento completo (datos, comentarios, etiquetas y fotos) en un único archivo ZIP.
- Relevamiento manual a través del sistema web.

**Should Have**

- Reasignación de agentes de campo a un relevamiento por parte del jefe de área.
- Revisión por el jefe de área de relevamientos en proceso y ya realizados.
- Resolución desde la web de conflictos de sincronización (por ejemplo, dos marcadores dentro de un mismo radio).

**Could Have**

- Filtrado de fotos y observaciones por etiquetas en la revisión web.
- Visor de fotos a pantalla completa con zoom dentro del carrusel.
- Aviso al jefe de área cuando un relevamiento pasa de recolección a revisión.

**Won't Have (v1)**

- Captura automatizada mediante drones o estaciones (declarada explícitamente como solución futura).
- Generación automática de los informes de evaluación.
- Soporte de la app móvil en plataformas distintas de Android.

---

## §5 Historias de usuario

- Como **agente de campo**, quiero tomar una foto y que el sistema le asigne automáticamente las coordenadas geográficas, para registrar la observación sin cargar la ubicación a mano.
- Como **agente de campo**, quiero relevar sin conexión a internet y que mis observaciones se sincronicen cuando recupero la señal, para poder trabajar en zonas sin cobertura.
- Como **agente de campo**, quiero agregar y quitar fotos, comentarlas y etiquetarlas dentro de un marcador, para enriquecer la observación de un punto.
- Como **jefe de área**, quiero crear un relevamiento y asignarle agentes de campo, para organizar el trabajo de mi área.
- Como **jefe de área**, quiero revisar las observaciones sobre un mapa y recorrer las fotos en un carrusel, para evaluar el estado de la obra y confeccionar mis informes.
- Como **jefe de área**, quiero exportar un relevamiento completo en un único archivo, para resguardarlo o compartirlo con el área central.

---

## §6 Flujos típicos

**Flujo de un relevamiento, de punta a punta**

El jefe de área crea un relevamiento y le asigna los agentes de campo de su área. Cada agente entra al sistema y elige uno de los relevamientos que le asignaron. La cuadrilla sale a terreno y va recolectando observaciones: en cada punto saca fotos, agrega comentarios y etiqueta lo que registra; todo eso queda enganchado al relevamiento. Cuando termina la etapa de recolección, el relevamiento pasa de "recolección" a "revisión". El jefe de área revisa la información sobre el mapa, evalúa el estado de la obra y arma sus informes rutinarios. Cuando lo considera terminado, cierra el relevamiento.

**Flujo de captura en campo sin conexión**

El agente se loguea con internet la primera vez. Ya en el terreno, sin señal, vuelve a entrar usando los métodos de seguridad del teléfono. Va tomando fotos: por cada foto el sistema resuelve las coordenadas y crea la observación con su marcador. Todo se guarda localmente. Cuando el teléfono vuelve a tener internet, la app detecta la conexión sola, sube primero los cambios locales y después baja las últimas actualizaciones de los relevamientos asignados a ese agente.

---

## §7 Casos límite (con resolución propuesta)

Las resoluciones son **decisiones de producto propuestas (a validar con el cliente)**.

| # | Caso límite | Resolución propuesta |
|---|---|---|
| 1 | Dos o más marcadores dentro de un mismo radio | No se unifican automáticamente: se listan como conflicto y el jefe de área decide desde la web si unificar o mantener separados |
| 2 | Foto sin EXIF de ubicación en carga manual | Se solicita ubicar el punto manualmente con el pin sobre el mapa; si no se ubica, la foto queda en una bandeja "sin georreferenciar" del relevamiento |
| 3 | Tiempo máximo offline | Se soporta al menos una jornada laboral completa; no hay límite duro de observaciones más allá del almacenamiento del dispositivo |
| 4 | Dos agentes modifican el mismo marcador offline | Al sincronizar prevalece el último cambio (last-write-wins) y queda marcado como conflicto resoluble desde la web |
| 5 | Observaciones después de cerrar el relevamiento | El relevamiento cerrado queda de solo lectura; reabrirlo requiere una acción explícita del jefe de área |
| 6 | Relogueo en campo sin método de seguridad configurado | Se exige configurar PIN/biometría del teléfono en el primer login online; sin eso, no se habilita el modo offline |

---

## §8 Métricas de éxito desde el negocio

Objetivos SMART **propuestos por el equipo (a validar con el cliente)**.

| Criterio | Métrica | Target | Plazo |
|---|---|---|---|
| Delegación de la recolección | Porcentaje de relevamientos recolectados por agentes de campo (no expertos) sobre el total | ≥ 80% | 6 meses post-lanzamiento |
| Productividad del relevamiento | Cantidad de obras relevadas por trimestre vs. método manual | +50% | 9 meses post-lanzamiento |
| Confiabilidad de la georreferenciación | Porcentaje de observaciones con coordenada válida asignada automáticamente | ≥ 95% | continuo, revisión mensual |
| Eficiencia de sincronización | Tiempo de sincronización de una jornada típica de campo | ≤ 5 minutos | 3 meses post-lanzamiento |

---

## §9 Lo que NO es este sistema (exclusiones)

| Funcionalidad excluida | Justificación | Versión futura |
|---|---|---|
| Captura automatizada por drones o estaciones | El cliente la plantea como solución futura habilitada por este sistema, no como parte de la v1 | Sí, futura |
| Generación automática de los informes de evaluación | El sistema soporta la revisión y evaluación; la confección de los informes rutinarios la hace el jefe de área | No planificado para v1 |
| App móvil en plataformas distintas de Android | La depuración y prueba se plantea sobre dispositivo Android por USB; iOS y otras plataformas quedan fuera de la v1 | Posible v2.0 |

---

## §10 Restricciones del cliente

| Tipo de restricción | Detalle | Origen |
|---|---|---|
| Operativa | La captura en campo debe poder hacerse sin conexión durante el trabajo de terreno; el login requiere internet solo la primera vez | Requerimiento funcional explícito |
| Operativa | El relogueo en campo se hace con los métodos de seguridad del teléfono | Requerimiento funcional explícito |
| Legal / regulatoria | Tratamiento de datos personales de los agentes y de registros del organismo sujeto a la Ley 25.326 de Protección de Datos Personales (Argentina) | Marco regulatorio nacional |
| Fecha objetivo | Sin fecha contractual dura: el objetivo es MVP funcional al cierre de la fase 5 del plan de desarrollo, entregando valor por sprint | Plan de fases del proyecto |
| Presupuesto | Cubierto por el presupuesto operativo del organismo titular; rango orientativo a confirmar | (Supuesto a validar) |
| Integración obligatoria | No se requiere integración con sistemas existentes en la v1 | Derivado de los requerimientos (no se mencionan integraciones) |

---

## §11 Riesgos detectados desde el negocio

| ID | Riesgo | Probabilidad | Impacto | Mitigación propuesta |
|---|---|---|---|---|
| R-01 | Marcadores duplicados dentro de un mismo radio que generan datos inconsistentes | Media | Alto | Resolución manual de conflictos desde la web, con parámetro de radio configurable |
| R-02 | Pérdida de observaciones capturadas offline antes de sincronizar (pérdida o daño del dispositivo) | Media | Alto | Recordatorio de sincronización al recuperar señal, sincronización automática y política de sincronizar al menos una vez por jornada |
| R-03 | Georreferenciación de baja calidad cuando la foto no trae EXIF o el GPS es impreciso | Media | Medio | Priorizar EXIF, permitir ajuste manual del pin y agrupar por radio de área |
| R-04 | Baja adopción por parte de personal de campo menos especializado | Media | Medio | UX simple basada en mapa y pines, foco en captura rápida, capacitación inicial |

---

## §12 Glosario del dominio del cliente

| Término | Definición | Sinónimos / Notas |
|---|---|---|
| **Relevamiento** | Tarea que consiste en registrar observaciones sobre el estado de un puente o camino; se estructura como una serie de marcadores geográficos y tiene estados (recolección, revisión, cerrado) | — |
| **Observación** | Conjunto de notas (comentarios) y fotos asociadas a un punto geográfico; las fotos pueden o no estar ligadas a un comentario | — |
| **Marcador geográfico** | Punto en el mapa que agrupa observaciones (comentarios y fotos) y es etiquetable; varias observaciones pueden compartir un mismo marcador | "Pin", "punto" |
| **Etiqueta** | Marca que se aplica a fotos o comentarios para su posterior filtrado | "Tag" |
| **Sincronización** | Proceso por el cual la app sube primero los cambios locales y luego baja las últimas actualizaciones de los relevamientos del agente | "Sync" |
| **Agente de campo** | Persona física que recolecta comentarios y fotos en terreno, etiqueta y administra las fotos de un marcador | "Relevador", "cuadrilla" |
| **Jefe de área** | Usuario que crea relevamientos, gestiona a los agentes de su área, evalúa la información y cierra relevamientos | — |
| **Usuario raíz** | Usuario ajeno a la administración funcional que configura el sistema, da de alta al jefe general y puede intervenir para resolver incoherencias | — |

---

## §13 Checklist de completitud del brief

- [x] El bloque de cabecera tiene nombre de proyecto, cliente, fecha, autor y estado completos.
- [x] §1 describe un problema concreto en lenguaje del cliente, no una solución técnica.
- [x] §1 responde explícitamente qué pasa si NO se construye el sistema.
- [x] §2 contiene al menos un stakeholder por categoría (propietario, implementador, beneficiario), con rol explícito.
- [x] §3 articula una propuesta de valor que no aplica a "cualquier producto".
- [x] §4 tiene al menos un ítem en cada categoría MoSCoW (Must, Should, Could, Won't).
- [x] §4 no contiene más Must Have que el mínimo razonable para el problema central.
- [x] §5 incluye al menos 3 historias de usuario en formato `Como [rol], quiero [acción], para [valor]`.
- [x] §5 cubre al menos 2 roles distintos.
- [x] §6 describe al menos 2 flujos típicos en lenguaje coloquial, sin diagramas formales.
- [x] §7 lista al menos 5 casos límite con su resolución.
- [x] §8 tiene al menos 3 métricas SMART con criterio, target y plazo numérico.
- [x] §9 lista al menos 3 exclusiones con justificación explícita.
- [x] §10 tiene definidos presupuesto orientativo y fecha objetivo (fecha justificada como "sin fecha contractual dura").
- [x] §11 lista al menos 3 riesgos con probabilidad, impacto y mitigación accionable.
- [x] §12 define al menos 5 términos del dominio del cliente.
- [x] El documento no contiene referencias a stack tecnológico, frameworks ni decisiones de arquitectura.
- [x] El estado del documento se actualizó de "Borrador" a "En revisión".
- [x] La tabla de control de cambios refleja la versión actual del documento.

---

## Apéndice — Supuestos a validar con el cliente

Estos valores se fijaron para destrabar el flujo SDD; el contenido es funcional pero debe confirmarse:

1. Identidad del organismo titular y del jefe general que aprueba el brief (§1, §2).
2. Rango de presupuesto orientativo (§10).
3. Targets y plazos exactos de las métricas de éxito (§8).
4. Las seis resoluciones de casos límite de §7 (son decisiones de producto propuestas).
5. Confirmación de que iOS queda fuera de alcance en v1 (§9).

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | 2026-05-31 | Brief inicial generado a partir de `geovial-requerimientos.md` | Equipo SDD 2.0 |
| 1.0 | 2026-05-31 | Resolución de placeholders bloqueantes: MoSCoW completo (§4), métricas SMART (§8), casos límite con resolución (§7), restricciones legal/fecha/presupuesto (§10), riesgos completos (§11), estado a "En revisión", checklist tildado | Equipo SDD 2.0 (AG-00 / AG-01) |

---

**Fin del documento**
