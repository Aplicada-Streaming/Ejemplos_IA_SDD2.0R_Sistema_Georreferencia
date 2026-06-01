# Reglas constructivas — README raíz del proyecto

**Carpeta target:** `/sdd2.0/docs/`
**Archivo target:** `/sdd2.0/docs/README.md`
**Subagente target del orquestador:** Arquitecto de Soluciones Senior (AG-ROOT)
**Versión de las reglas:** 1.0

---

## 1. Especialidad asignada

### 1.1 Especialidad base

Arquitecto de Soluciones Senior, equivalente al AG-ROOT del catálogo de especialidades. Su rol es garantizar la coherencia integral del proyecto desde una perspectiva sistémica, asegurando que el README raíz funcione como punto de entrada efectivo, narrativa técnica ejecutiva y mapa navegable hacia las 12 categorías de documentación. Su responsabilidad no es producir contenido de detalle (eso corresponde a los AG-00 a AG-11), sino integrar, vincular y validar la coherencia transversal. Su alcance abarca: definición de la estructura documental, redacción del README maestro, validación de enlaces internos, diseño del flujo de lectura por audiencia y alineación entre la visión del producto y la organización de las carpetas.

### 1.2 Variantes según tipo de proyecto

| Tipo de proyecto (D8) | Especialidad específica | Justificación |
| --- | --- | --- |
| library | Arquitecto de Soluciones + Curador de Librería | El README debe priorizar instalación, API pública, ejemplos de consumo y compatibilidad semántica. |
| web-monolith | Arquitecto de Soluciones Senior | Estructura clásica; el énfasis está en flujos funcionales y onboarding general. |
| web-microservices | Arquitecto de Soluciones + Arquitecto Distribuido | Requiere mapa de servicios, diagrama de despliegue y matriz de contratos entre componentes. |
| desktop-app | Arquitecto de Soluciones + Especialista Cross-Platform | Debe declarar compatibilidad por sistema operativo, empaquetado y dependencias nativas. |
| mobile-app-maui | Arquitecto de Soluciones + Mobile Lead | Foco en plataformas objetivo, ciclo de publicación en tiendas y permisos del dispositivo. |
| rest-api | Arquitecto de Soluciones + API Designer | Énfasis en quick-start de consumo, autenticación, versionado de endpoints y referencia OpenAPI. |
| cli-tool | Arquitecto de Soluciones + CLI Designer | El README es la primera ayuda visible; debe contener instalación, comandos y ejemplos de uso. |
| worker-service | Arquitecto de Soluciones + Streaming/Event Engineer | Requiere descripción del modelo de eventos, fuentes, sinks y reintentos. |

El orquestador lee esta tabla y, según el campo `Tipo de proyecto` declarado en PROJECT-README durante el intake, selecciona la variante correspondiente y la combina con la especialidad base.

### 1.3 Multi-especialidad

Cuando el README raíz necesita atender audiencias mixtas, se admite combinar AG-ROOT con el Technical Writer (AG-10) para refinar la narrativa orientada a desarrollador externo, y con el Analista de Negocio (AG-01) para validar que la propuesta de valor expuesta en la sección de identidad coincide con la visión declarada en `/sdd2.0/docs/00_contexto/`. En proyectos `library` y `cli-tool` se recomienda incorporar a AG-11 (Developer Advocate) para curar el bloque de quick-start. La regla es: AG-ROOT mantiene la propiedad del documento; las multi-especialidades aportan revisión y enmienda, no autoría compartida.

---

## 2. Documentos que produce esta categoría

### 2.1 Tabla maestra de documentos

| Archivo | Obligatorio para | Recomendado para | Omitir para | Descripción |
| --- | --- | --- | --- | --- |
| `/sdd2.0/docs/README.md` | Todos los tipos D8 | — | — | Punto de entrada de la documentación del proyecto y ancla del árbol SDD. |
| `/sdd2.0/docs/CHANGELOG.md` | library, rest-api, cli-tool | web-monolith, web-microservices, worker-service, desktop-app, mobile-app-maui | — | Bitácora de cambios con relevancia para consumidores externos. |
| `/sdd2.0/docs/CONTRIBUTING.md` | library, cli-tool | rest-api, worker-service | web-monolith | Guía de contribución cuando el proyecto admite aportes externos. |
| `/sdd2.0/docs/LICENSE.md` | library, cli-tool | rest-api | — | Texto de licencia visible desde el árbol de documentación. |

### 2.2 Reglas de inclusión/exclusión por tipo de proyecto

El README raíz es siempre obligatorio. Lo que varía es la presencia de bloques internos según el tipo:

- `library`: incluye sección de instalación, API pública y compatibilidad de versiones.
- `web-monolith`: incluye flujos de negocio principales y enlace a la guía de despliegue.
- `web-microservices`: incluye listado de servicios y diagrama de interacción.
- `desktop-app`: incluye matriz de sistemas operativos soportados y empaquetadores.
- `mobile-app-maui`: incluye plataformas objetivo, versiones mínimas y ciclo de publicación.
- `rest-api`: incluye quick-start con `curl`, autenticación y referencia al contrato.
- `cli-tool`: incluye instalación, comandos principales y ejemplos.
- `worker-service`: incluye fuentes y sinks de eventos, política de reintentos y observabilidad.

Los archivos `CHANGELOG.md`, `CONTRIBUTING.md` y `LICENSE.md` se incluyen en `/sdd2.0/docs/` solo cuando el proyecto requiere comunicación con consumidores externos al equipo.

---

## 3. Nomenclatura y vinculación

### 3.1 Patrón de nombres

El archivo es `README.md` literal, sin versión en el nombre. El versionado vive en la cabecera del documento mediante el campo `Versión` y se actualiza siguiendo la regla D5 (inicio en v1.0). Los archivos satélite mencionados en §2.1 también van en mayúsculas convencionales: `CHANGELOG.md`, `CONTRIBUTING.md`, `LICENSE.md`.

### 3.2 Convenciones de prefijos / sufijos

El propio README raíz no usa prefijos. Para los archivos linkeados desde el README, se respetan los patrones canónicos de las 12 categorías:

- `NB-XX-<kebab>_v<X.Y>.md` (necesidades de negocio).
- `CU-XX-<kebab>_v<X.Y>.md` (casos de uso).
- `RN-XX-<kebab>_v<X.Y>.md` (reglas de negocio).
- `ADR-XX-<kebab>_v<X.Y>.md` (decisiones de arquitectura).
- `US-XX-<kebab>_v<X.Y>.md` (historias de usuario).
- `BT-XX-<kebab>_v<X.Y>.md` (backlog técnico).
- `sprint-XX-<kebab>_v<X.Y>.md` (planes de sprint).
- `ejemplo-XX-<kebab>_v<X.Y>.md` (ejemplos progresivos).

Todos los nombres respetan kebab-case estricto en minúsculas (D3) y sufijo de versión con guion bajo (D4).

### 3.3 Vinculación cross-doc (trazabilidad upstream/downstream)

- Upstream: el README raíz consume `PROJECT-BRIEF` y `PROJECT-README` producidos en la fase de intake. De allí extrae el nombre del proyecto, la propuesta de valor, el tipo D8 y el stack declarado.
- Downstream: el README raíz enlaza a las 12 carpetas de categoría (`00_contexto` a `11_examples`) y, dentro de cada una, a su `README.md` de sección. No enlaza directamente a artefactos internos de las categorías; eso lo hace el README de cada sección.

### 3.4 README de la sección

No aplica. Este archivo es el README de la raíz de `/sdd2.0/docs/`. Los README de sección (uno por cada carpeta numerada) son responsabilidad de los respectivos AG-00 a AG-11 y se rigen por su propio archivo de reglas.

---

## 4. Estructura de redacción del documento

### 4.1 Cabecera obligatoria

La cabecera del `README.md` generado debe seguir este bloque, completando los valores entre llaves dobles a partir de PROJECT-README:

```markdown
# {{nombre-proyecto}}

| Campo | Valor |
| --- | --- |
| Proyecto | {{nombre-proyecto}} |
| Versión del documento | 1.0 |
| Estado | Borrador / Propuesto / Aprobado / Vigente / Superado / Archivado |
| Fecha | YYYY-MM-DD |
| Stack | {{stack-declarado}} |
| Tipo de proyecto | {{tipo-d8}} |
| Documento | README raíz |
```

Nota: el README raíz, por ser el ancla del árbol, no declara un bloque "Trazabilidad upstream/downstream" en su cabecera. Esa trazabilidad se materializa en la sección §3 del documento generado, donde se enumeran las 12 categorías downstream.

### 4.2 Secciones obligatorias

El README generado debe contener, como mínimo, las siguientes secciones en este orden:

1. Identidad del proyecto: propósito en 2 a 3 párrafos, propuesta de valor, audiencia objetivo.
2. Stack y tipo de proyecto: tabla con tecnologías y versiones, tipo D8 y plataformas soportadas.
3. Mapa de la documentación: las 12 categorías como bullets, cada una con descripción de propósito y enlace al `README.md` de la sección.
4. Flujo de lectura recomendado por audiencia: al menos 3 audiencias diferenciadas, con orden de lectura sugerido y justificación.
5. Cómo contribuir y cómo regenerar la documentación: enlace a `CONTRIBUTING.md` si aplica y proceso de regeneración con los subagentes SDD.
6. Estado actual y roadmap: tabla de estado de cada categoría y enlace al roadmap detallado en `00_contexto`.
7. Glosario rápido: mínimo 10 términos del dominio del proyecto, breves, sin reemplazar el glosario completo de la categoría UX/UI.
8. Contacto y responsables: tabla con rol, responsable y canal de comunicación.
9. Control de cambios: tabla con versión, fecha y descripción del cambio.

### 4.3 Secciones opcionales según tipo de proyecto

| Sección | Aplica a | Notas |
| --- | --- | --- |
| Diagrama de despliegue | web-microservices, worker-service | Incluir vista de servicios, colas, almacenamiento y red. |
| Compatibilidad de plataformas | mobile-app-maui, desktop-app, cli-tool | Tabla con sistema operativo, versión mínima y observaciones. |
| Cómo consumir como dependencia | library | Comando de instalación, importación mínima y ejemplo de 5 líneas. |
| Quick-start | rest-api, cli-tool | Bloque ejecutable con 3 a 5 comandos para validar el camino feliz. |
| Modelo de eventos | worker-service | Listar fuentes, sinks, formato de payload y política de reintentos. |
| Política de versionado y soporte | library, rest-api | Tabla con versiones vigentes, fin de soporte y ruta de migración. |

### 4.4 Tablas tipo y formatos recurrentes

El documento debe usar las siguientes tablas estandarizadas:

Tabla A: Mapa de documentación.

| Categoría | Propósito | Responsable | Enlace |
| --- | --- | --- | --- |
| 00_contexto | Visión, alcance, roadmap | AG-00 | [00_contexto](00_contexto/) |
| 01_necesidades_negocio | Necesidades de negocio | AG-01 | [01_necesidades_negocio](01_necesidades_negocio/) |

Tabla B: Flujo de lectura por audiencia.

| Rol | Orden recomendado | Por qué |
| --- | --- | --- |
| Product Manager | 00 → 01 → 06 → 07 | Necesita entender visión, necesidades y backlog. |
| Desarrollador | 00 → 02 → 05 → 10 → 11 | Necesita contexto, especificación, arquitectura y ejemplos. |
| QA | 00 → 02 → 08 | Necesita ver requisitos y estrategia de pruebas. |
| DevOps | 00 → 05 → 09 | Necesita arquitectura y pipeline. |

Tabla C: Estado actual.

| Categoría | Estado | Versión vigente |
| --- | --- | --- |
| 00_contexto | Vigente | 1.0 |
| 01_necesidades_negocio | Borrador | 0.9 |

### 4.5 Anti-patrones a evitar

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| README sin enlaces a las 12 categorías | Rompe la navegación SDD y obliga a explorar el árbol manualmente | Incluir Tabla A completa con las 12 entradas, aunque alguna categoría esté en borrador. |
| Stack mencionado sin versión | Imposible reproducir entornos y validar compatibilidad | Declarar siempre `tecnología @ versión` en la cabecera y en §2. |
| Flujo de lectura único sin variantes por rol | Cada audiencia se pierde en información no relevante | Producir mínimo 3 flujos por rol en Tabla B. |
| README como wiki extensa | Duplica contenido de las categorías y se desactualiza primero | Mantener el README en 200 a 400 líneas y delegar el detalle a cada categoría. |
| Roadmap inline en el README | Genera dos fuentes de verdad sobre el roadmap | Enlazar a `00_contexto/roadmap-producto_v1.0.md` y no replicar contenido. |
| Glosario que reemplaza al de UX/UI | El glosario rápido se convierte en glosario completo y diverge | Limitar a 10 a 20 términos esenciales y enlazar al glosario de la categoría UX/UI. |
| Estado libre fuera del enum | Estados ambiguos como "casi listo" o "WIP" | Usar exclusivamente: Borrador, Propuesto, Aprobado, Vigente, Superado, Archivado. |

---

## 5. Preguntas guía para el subagente

### 5.1 Comprensión del input upstream

- ¿Qué nombre canónico tiene el proyecto en kebab-case según PROJECT-README?
- ¿Cuál es el tipo D8 declarado y qué variantes de §1.2 corresponden?
- ¿Cuál es el stack con versiones y plataformas objetivo?
- ¿Cuál es la propuesta de valor en una sola línea y en un párrafo?
- ¿Qué audiencias se han identificado en el intake como prioritarias?

### 5.2 Decisiones de scope

- ¿Qué contenido es propio del README raíz y qué debe quedar en la categoría correspondiente?
- ¿Se incluyen secciones opcionales de §4.3 según el tipo D8?
- ¿Qué archivos satélite de §2.1 acompañan al README en este proyecto?
- ¿El README aporta valor en cada bloque o algún bloque está duplicando categorías?

### 5.3 Trazabilidad

- ¿Las 12 categorías están enlazadas con un párrafo breve de propósito?
- ¿Los enlaces apuntan a rutas existentes en `/sdd2.0/docs/`?
- ¿La cadena Visión → NB → CU → RN → ADR → US → BT → Sprint → Test → Pipeline está visible al menos como referencia conceptual en §3?
- ¿El roadmap del README es un enlace y no una copia?

### 5.4 Calidad

- ¿La longitud final está entre 200 y 400 líneas?
- ¿Se respetó D1 (español rioplatense neutro, sin emojis, sin negritas decorativas)?
- ¿Las tablas tienen filas completas sin valores "TBD" ni placeholders sin cerrar?
- ¿El glosario tiene al menos 10 términos del dominio?
- ¿Cada audiencia de §4.4 Tabla B tiene un orden de lectura justificado?
- ¿Se evitaron los anti-patrones de §4.5?
- ¿Se respetó el enum cerrado de estados?
- ¿Se respetó D7 evitando ejemplos prohibidos en el contenido?

---

## 6. Criterios de aceptación del entregable

- [ ] Las 12 categorías SDD están enlazadas en la Tabla A con su path correcto, en orden de 00 a 11.
- [ ] El tipo de proyecto declarado en PROJECT-README está reflejado en la cabecera y condiciona las secciones opcionales incluidas.
- [ ] El flujo de lectura está diferenciado para al menos 3 audiencias en la Tabla B, con justificación por rol.
- [ ] El glosario rápido tiene mínimo 10 términos del dominio del proyecto, definidos en una línea cada uno.
- [ ] Todos los enlaces internos del README apuntan a rutas que existen en `/sdd2.0/docs/`; no hay enlaces rotos.
- [ ] La cabecera respeta el bloque obligatorio de §4.1 con todos los campos completos.
- [ ] El documento tiene entre 200 y 400 líneas en su versión final.
- [ ] No aparecen emojis, negritas decorativas, ni términos del dominio prohibido por D7.
- [ ] El control de cambios al pie del documento tiene al menos una entrada inicial v1.0.
- [ ] El estado declarado en la cabecera pertenece al enum cerrado: Borrador, Propuesto, Aprobado, Vigente, Superado o Archivado.

---

## 7. Ejemplos genéricos

### 7.1 Ejemplo A: Sistema de turnos médicos (web-monolith)

```markdown
# turnos-medicos

| Campo | Valor |
| --- | --- |
| Proyecto | turnos-medicos |
| Versión del documento | 1.0 |
| Estado | Vigente |
| Fecha | 2026-03-10 |
| Stack | Python 3.12, Django 5.1, PostgreSQL 16 |
| Tipo de proyecto | web-monolith |
| Documento | README raíz |

## 1. Identidad del proyecto

Aplicación web monolítica para la gestión de turnos médicos en centros de salud
de mediana escala. Permite a pacientes solicitar turnos por especialidad,
a profesionales gestionar su agenda y a la administración generar reportes.

## 3. Mapa de la documentación

| Categoría | Propósito | Responsable | Enlace |
| --- | --- | --- | --- |
| 00_contexto | Visión, alcance, roadmap | AG-00 | [00_contexto](00_contexto/) |
| 01_necesidades_negocio | Necesidades del centro de salud | AG-01 | [01_necesidades_negocio](01_necesidades_negocio/) |

## 4. Flujo de lectura recomendado

| Rol | Orden recomendado | Por qué |
| --- | --- | --- |
| Director del centro | 00 → 01 → 06 | Visión y backlog estratégico |
| Desarrollador | 00 → 02 → 05 → 10 | Necesita contexto y arquitectura |
| QA | 00 → 02 → 08 | Especificación y pruebas |
```

### 7.2 Ejemplo B: Librería de parsing CSV (library)

```markdown
# csv-parser-lib

| Campo | Valor |
| --- | --- |
| Proyecto | csv-parser-lib |
| Versión del documento | 1.0 |
| Estado | Vigente |
| Fecha | 2026-04-22 |
| Stack | TypeScript 5.5, Node 20 |
| Tipo de proyecto | library |
| Documento | README raíz |

## 1. Identidad del proyecto

Librería liviana para parseo y validación de archivos CSV con soporte de
streaming, inferencia de tipos opcional y reporte estructurado de errores.
Pensada para integrarse en pipelines de ingesta de datos.

## 2. Cómo consumir como dependencia

Instalación mínima desde el registro de paquetes del lenguaje, importación
del módulo y ejemplo de 5 líneas que parsea un archivo y devuelve filas.

## 3. Mapa de la documentación

| Categoría | Propósito | Responsable | Enlace |
| --- | --- | --- | --- |
| 02_especificacion_funcional | Contrato de la API pública | AG-02 | [02_especificacion_funcional](02_especificacion_funcional/) |
| 10_developer_guide | Guía de integración | AG-10 | [10_developer_guide](10_developer_guide/) |
| 11_examples | Ejemplos progresivos | AG-11 | [11_examples](11_examples/) |

## 4. Flujo de lectura recomendado

| Rol | Orden recomendado | Por qué |
| --- | --- | --- |
| Desarrollador consumidor | 11 → 10 → 02 | Empezar por ejemplos y luego ver el contrato |
| Mantenedor de la librería | 00 → 05 → 06 → 09 | Arquitectura, backlog y pipeline |
| QA | 02 → 08 | Validar contrato y matriz de pruebas |
```

---

## 8. Prompt-snippet sugerido para el subagente

```text
Sos un {{ESPECIALIDAD-VARIANTE}} responsable de redactar el README raíz del proyecto {{NOMBRE_PROYECTO}}.

Insumos:
- PROJECT-BRIEF: {{path}}
- PROJECT-README: {{path}}
- Documentos upstream ya generados: ninguno (sos el ancla).
- Documentos downstream que vas a enlazar: /sdd2.0/docs/00..11/README.md

Reglas de redacción: §4 de _root_rules.md.
Trazabilidad esperada: enlazar las 12 categorías con descripción de propósito.
Criterios de calidad: §6 de _root_rules.md (10 ítems verificables).
Restricciones: respetar D1 a D8; no incluir emojis, negritas decorativas, ni términos del dominio prohibido por D7.

Salida: /sdd2.0/docs/README.md (sin versión en el nombre, con versión 1.0 en la cabecera).
```

---

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-17 | Reglas iniciales generadas durante bootstrap SDD 2.0 |
