# Necesidades de Negocio — GeoVial

| Campo | Valor |
| --- | --- |
| Proyecto | GeoVial |
| Documento | necesidades-negocio_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-05-31 |
| Autor | Analista de Negocio Senior (AG-01), Equipo SDD 2.0 |
| Cantidad de NB | 6 |
| Versión del catálogo de NB | 1.0 |
| Trazabilidad upstream | PROJECT-BRIEF §1, §3, §4, §7, §8, §10, §11; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | 02_especificacion_funcional (CU), 06_backlog-tecnico, 07_plan-sprint, 08_calidad_y_pruebas |

## 1. Propósito del catálogo

Este índice maestro consolida las necesidades de negocio de GeoVial derivadas del problema declarado en PROJECT-BRIEF §1, la propuesta de valor de §3, el alcance MoSCoW de §4, los casos límite de §7, las métricas de §8 y las restricciones legales de §10, junto con la visión y el alcance ya consolidados en la categoría 00. Cada necesidad articula un problema de negocio con su métrica de éxito y su prioridad relativa, y declara las CU previstas que la implementarán en la categoría 02.

## 2. Resumen de NB (tabla D)

| ID | Necesidad | Prioridad MoSCoW | CU previstas | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| NB-01 | Delegación de la recolección en personal no experto | Must | CU-01, CU-02, CU-03 | Propuesto | [NB-01](necesidades-de-negocio/NB-01-delegacion-recoleccion-personal-no-experto_v1.0.md) |
| NB-02 | Georreferenciación automática y confiable de las observaciones | Must | CU-04, CU-05 | Propuesto | [NB-02](necesidades-de-negocio/NB-02-georreferenciacion-automatica-confiable_v1.0.md) |
| NB-03 | Continuidad operativa sin conexión con sincronización confiable | Must | CU-06, CU-07 | Propuesto | [NB-03](necesidades-de-negocio/NB-03-continuidad-operativa-sin-conexion_v1.0.md) |
| NB-04 | Revisión y evaluación centralizada sobre mapa para informes | Must | CU-08, CU-09, CU-10 | Propuesto | [NB-04](necesidades-de-negocio/NB-04-revision-centralizada-sobre-mapa_v1.0.md) |
| NB-05 | Consistencia de datos ante marcadores duplicados y conflictos | Should | CU-11, CU-12 | Propuesto | [NB-05](necesidades-de-negocio/NB-05-consistencia-datos-ante-conflictos_v1.0.md) |
| NB-06 | Trazabilidad de acciones y protección de datos personales | Must | CU-13, CU-14 | Propuesto | [NB-06](necesidades-de-negocio/NB-06-trazabilidad-proteccion-datos-personales_v1.0.md) |

## 3. Mapa de dependencias entre NB

Las dependencias declaradas en la §8 de cada NB son acíclicas y ninguna NB depende de más de tres. Cada flecha indica "depende de".

- NB-01 — sin dependencias (necesidad raíz).
- NB-02 → NB-01.
- NB-03 → NB-01, NB-02.
- NB-04 → NB-02, NB-03.
- NB-05 → NB-03, NB-02.
- NB-06 → NB-01.

Orden topológico de lectura: NB-01, NB-02, NB-03, NB-04, NB-05, NB-06. No existen ciclos: todas las flechas apuntan hacia índices ya resueltos.

## 4. Trazabilidad agregada

### 4.1 Upstream

| NB | Origen en intakes y categoría 00 |
| --- | --- |
| NB-01 | PROJECT-BRIEF §1, §3, §4, §8; vision §1, §3, §5; alcance §3, §4.1 |
| NB-02 | PROJECT-BRIEF §3, §8, §11 (R-03); vision §3, §6; alcance §4.1 |
| NB-03 | PROJECT-BRIEF §3, §6, §8, §11 (R-02); vision §3, §6; alcance §4.1, §7 |
| NB-04 | PROJECT-BRIEF §1, §6, §4; vision §3, §4; alcance §3, §4.1 |
| NB-05 | PROJECT-BRIEF §7 (casos 1 y 4), §11 (R-01); vision §8; alcance §8 |
| NB-06 | PROJECT-BRIEF §10, §2; vision §7; alcance §7 |

### 4.2 Downstream

- 02_especificacion_funcional: las catorce CU previstas (CU-01 a CU-14) se generan a partir de la §7 de cada NB con estado `a generar`.
- 06_backlog-tecnico y 07_plan-sprint: la prioridad MoSCoW de la §9 de cada NB ordena el backlog y la secuencia de sprints; las cinco Must Have anteceden a la Should Have NB-05.
- 08_calidad_y_pruebas: los criterios de éxito de la §5 de cada NB alimentan directamente los criterios de aceptación de la categoría 08.

## 5. Decisiones de scope

Se consolidaron seis NB. La candidata de productividad y escala del relevamiento (PROJECT-BRIEF §8, +50 %) se fusionó dentro de NB-01 porque comparte el dolor central de delegar la recolección para escalar: la productividad es una métrica del mismo problema, con el mismo público, no un problema con público distinto. Las cinco restantes se mantienen separadas porque cada una tiene una métrica y un foco de público diferenciados: confiabilidad de la georreferenciación (NB-02), continuidad y sincronización (NB-03), evaluación centralizada (NB-04), consistencia ante conflictos (NB-05) y cumplimiento de la Ley 25.326 (NB-06).

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-31 | Índice maestro inicial con seis NB generado por AG-01 a partir de los intakes y la categoría 00 |
