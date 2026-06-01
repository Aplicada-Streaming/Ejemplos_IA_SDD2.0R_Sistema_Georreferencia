# Especificación funcional — GeoVial

**Proyecto:** GeoVial
**Documento:** especificacion-funcional_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0
**Trazabilidad upstream:** vision-producto_v1.0.md; alcance-proyecto_v1.0.md; necesidades-negocio_v1.0.md (NB-01 a NB-06)
**Trazabilidad downstream:** 03_ux_ui_dx, 05_arquitectura_tecnica, 06_backlog-tecnico, 07_plan-sprint, 08_calidad_y_pruebas, 11_examples

## 1. Propósito del índice maestro

Este documento consolida la especificación funcional de GeoVial: los catorce casos de uso (CU), las ocho reglas de negocio (RN), el modelo conceptual de datos y sus siete reglas conceptuales (RC). Define el qué del sistema sin invadir el cómo (03 para interfaz, 05 para arquitectura y tipos físicos). El catálogo de CU honra el mapeo bidireccional NB → CU declarado en la §7 de cada NB de la categoría 01.

## 2. Catálogo de casos de uso

| CU | Título | Propósito en una línea | Estado |
| --- | --- | --- | --- |
| CU-01 | Crear relevamiento y asignar agentes | Un jefe de área crea un relevamiento de su área y asigna o reasigna agentes | Propuesto |
| CU-02 | Iniciar sesión y seleccionar relevamiento asignado | Un agente inicia sesión y elige un relevamiento asignado para recolectar | Propuesto |
| CU-03 | Administrar la jerarquía de usuarios y áreas | Un administrador da de alta y de baja usuarios según la jerarquía y el área | Propuesto |
| CU-04 | Capturar observación con georreferenciación automática | Un agente registra una observación tomando una foto y el sistema asigna la coordenada | Propuesto |
| CU-05 | Ubicar manualmente el punto | Un usuario ubica el punto cuando la foto no trae metadatos, o lo deja sin georreferenciar | Propuesto |
| CU-06 | Recolectar observaciones sin conexión | Un agente recolecta sin señal y encola los cambios para sincronizar | Propuesto |
| CU-07 | Sincronizar cambios locales | La app sube los cambios locales y baja las actualizaciones al recuperar conexión | Propuesto |
| CU-08 | Revisar relevamiento sobre el mapa | Un jefe de área revisa sobre el mapa y exporta o importa el relevamiento completo | Propuesto |
| CU-09 | Gestionar marcador (fotos, comentarios, etiquetas, carrusel) | Gestionar el contenido de un marcador y recorrer su carrusel y la navegación | Propuesto |
| CU-10 | Transicionar estados del relevamiento | Un jefe de área avanza el relevamiento por sus estados y lo reabre de forma explícita | Propuesto |
| CU-11 | Detectar marcadores en conflicto | Detectar y listar marcadores en un mismo radio y ediciones en conflicto | Propuesto |
| CU-12 | Resolver conflictos desde la web | Un jefe de área unifica o separa marcadores y dirime ediciones en conflicto | Propuesto |
| CU-13 | Registrar accesos y acciones administrativas | Registrar de forma inalterable accesos y acciones con retención por el período legal | Propuesto |
| CU-14 | Aplicar autorización por rol y área | Autorizar cada acceso por rol y área, transversal a todos los CU | Propuesto |

## 3. Catálogo de reglas de negocio

| RN | Título | Propósito en una línea | Estado |
| --- | --- | --- | --- |
| RN-01 | Jerarquía y autorización por rol y área | Cada acción es legítima solo si el rol y el área del usuario la habilitan | Propuesto |
| RN-02 | Identidad de marcador y radio de agrupación | Marcadores dentro de un mismo radio no se unifican ni descartan automáticamente | Propuesto |
| RN-03 | Prioridad de los metadatos de ubicación de la foto | La coordenada se deriva primero de los metadatos de la foto, luego de la ubicación manual | Propuesto |
| RN-04 | Última escritura prevalece con marca de conflicto | Ante choque, prevalece la última escritura y el recurso queda marcado como conflicto | Propuesto |
| RN-05 | Estados del relevamiento y solo lectura tras el cierre | El relevamiento transita estados válidos y queda de solo lectura tras el cierre | Propuesto |
| RN-06 | Método de seguridad del teléfono como precondición del modo sin conexión | El modo sin conexión solo se habilita con el método de seguridad configurado | Propuesto |
| RN-07 | Retención del registro de auditoría | Accesos y acciones se registran de forma inalterable y se conservan ≥ 12 meses | Propuesto |
| RN-08 | Tratamiento de datos personales bajo la Ley 25.326 | Los datos personales solo son accesibles según rol y área, con finalidad acotada | Propuesto |

## 4. Modelo de datos

| Artefacto | Propósito | Estado |
| --- | --- | --- |
| modelo-conceptual_v1.0.md | Modelo conceptual del dominio: 12 entidades, relaciones, cardinalidades, glosario y diagrama | Propuesto |

Conteo de entidades del modelo conceptual: 12 (Usuario, Área, Relevamiento, AsignaciónAgente, Observación, Marcador, Foto, Comentario, Etiqueta, RegistroCambioSync, ConflictoSync, RegistroAuditoría). Por superar diez entidades (§2.2 web-monolith), se generan reglas conceptuales (RC).

## 5. Catálogo de reglas conceptuales del modelo

| RC | Título | Propósito en una línea | Estado |
| --- | --- | --- | --- |
| RC-01 | Identidad de marcador y radio | Marcadores dentro del radio son candidatos al mismo punto, sin unificación automática | Propuesto |
| RC-02 | Integridad observación → marcador | Toda observación referencia un marcador de su propio relevamiento | Propuesto |
| RC-03 | Unicidad del identificador en la cola de sync | Cada cambio encolado tiene identificador único para idempotencia | Propuesto |
| RC-04 | Cardinalidad foto/comentario/etiqueta | Reglas de pertenencia y etiquetado de fotos y comentarios | Propuesto |
| RC-05 | Valores del rol jerárquico | El rol del usuario es uno y solo uno de los valores permitidos | Propuesto |
| RC-06 | Estados y transiciones del relevamiento | El estado toma valores permitidos y solo evoluciona por transiciones válidas | Propuesto |
| RC-07 | Integridad de la asignación de agente al área | El agente asignado pertenece a la misma área que el relevamiento | Propuesto |

## 6. Matriz de trazabilidad NB → CU → RN → US

La columna US queda como "a generar en 06".

| NB | CU | RN aplicables | US (a generar en 06) |
| --- | --- | --- | --- |
| NB-01 | CU-01 | RN-01, RN-02, RN-05, RN-07 | a generar en 06 |
| NB-01 | CU-02 | RN-01, RN-06, RN-07 | a generar en 06 |
| NB-01 | CU-03 | RN-01, RN-07, RN-08 | a generar en 06 |
| NB-02 | CU-04 | RN-01, RN-02, RN-03, RN-05 | a generar en 06 |
| NB-02 | CU-05 | RN-02, RN-03, RN-05 | a generar en 06 |
| NB-03 | CU-06 | RN-03, RN-05, RN-06 | a generar en 06 |
| NB-03 | CU-07 | RN-01, RN-02, RN-04 | a generar en 06 |
| NB-04 | CU-08 | RN-01, RN-05, RN-07, RN-08 | a generar en 06 |
| NB-04 | CU-09 | RN-01, RN-05 | a generar en 06 |
| NB-04 | CU-10 | RN-01, RN-05, RN-07 | a generar en 06 |
| NB-05 | CU-11 | RN-01, RN-02, RN-04 | a generar en 06 |
| NB-05 | CU-12 | RN-01, RN-02, RN-04, RN-05, RN-07 | a generar en 06 |
| NB-06 | CU-13 | RN-01, RN-07, RN-08 | a generar en 06 |
| NB-06 | CU-14 | RN-01, RN-08 | a generar en 06 |

Cobertura bidireccional: cada NB tiene al menos un CU y cada CU declara al menos una NB. No hay NB huérfana ni CU huérfano.

## 7. Reconciliaciones de cobertura

El mapeo de CU se mantuvo dentro de los IDs CU-01 a CU-14 previstos por las NB. Tres capacidades del alcance se absorbieron en CU existentes para no alterar el mapeo declarado:

- Inicio de sesión y relogueo en terreno con método de seguridad del teléfono: absorbidos en CU-02 (precondición y flujos alternativos). Documentado en §10 de CU-02.
- Reasignación de agentes de campo (Should Have): absorbida en CU-01 como flujo alternativo 5.A. Documentado en §10 de CU-01.
- Exportación e importación del relevamiento completo en un único archivo comprimido (Must Have): absorbidas en CU-08 como flujos alternativos 5.A y 5.B. Documentado en §10 de CU-08.
- Manejo uniforme de los errores de autorización: absorbido en CU-14 como caso de uso transversal. Documentado en §10 de CU-14.

Ninguna NB queda con su mapeo declarado alterado: los catorce IDs y títulos coinciden con lo previsto en la §7 de las NB.

## 8. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Índice maestro inicial con 14 CU, 8 RN, modelo conceptual de 12 entidades y 7 RC, generado por AG-02 |
