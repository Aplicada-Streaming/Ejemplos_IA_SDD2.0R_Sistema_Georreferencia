# NB-05 — Consistencia de datos ante marcadores duplicados y conflictos

| Campo | Valor |
| --- | --- |
| Proyecto | GeoVial |
| Documento | NB-05-consistencia-datos-ante-conflictos_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-05-31 |
| Autor | Analista de Negocio Senior (AG-01), Equipo SDD 2.0 |
| Trazabilidad upstream | PROJECT-BRIEF §7, §11; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-11, CU-12 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita que la base de datos relevada se mantenga consistente cuando varias cuadrillas trabajan en paralelo y sin conexión sobre la misma zona. Cuando el trabajo se consolida, pueden aparecer dos puntos casi en el mismo lugar que en realidad refieren a la misma obra, o ediciones del mismo punto hechas por personas distintas, y eso introduce datos contradictorios que ensucian la evaluación.

El dolor concreto es la ambigüedad: sin un criterio claro, el experto que evalúa no sabe si dos puntos cercanos son dos observaciones legítimas o un duplicado, ni cuál de dos ediciones en conflicto es la válida. Resolver esto de forma automática y silenciosa sería peor, porque podría unificar o descartar información que importa. La necesidad es que esas situaciones se detecten, se señalen y queden a decisión del jefe de área desde un lugar central, con un parámetro de cercanía configurable para detectar los duplicados.

Si esta necesidad no se resuelve, la base georreferenciada acumula duplicados y contradicciones, la confianza en los datos cae y la evaluación experta vuelve a apoyarse en revisión manual para discriminar qué es válido.

## 2. Ejemplo de uso desde la perspectiva del negocio

Dos cuadrillas relevan tramos contiguos de un mismo camino sin señal y ambas registran un punto sobre la misma alcantarilla. Al consolidarse el trabajo, aparecen dos marcadores casi superpuestos. En lugar de fusionarlos a ciegas o duplicar la observación en el informe, el sistema los lista como un conflicto y el jefe de área decide desde la web si los unifica o los mantiene separados, según conozca el terreno.

## 3. Impacto

- Preserva la consistencia de la base georreferenciada frente al trabajo paralelo y sin conexión.
- Evita unificaciones o descartes automáticos que destruirían información válida.
- Da al jefe de área control explícito sobre la resolución de cada conflicto.
- Si no se resuelve, se acumulan duplicados y contradicciones que degradan la confianza en los datos.
- Reduce el retrabajo de depurar manualmente la base antes de evaluar.

## 4. Problema específico que resuelve

- Aparecen dos o más marcadores dentro de un mismo radio que pueden referir a la misma obra.
- Dos agentes editan el mismo marcador sin conexión y generan versiones en conflicto.
- No hay un criterio configurable de cercanía para detectar posibles duplicados.
- Sin un punto central de resolución, los conflictos quedan ocultos y ensucian la evaluación.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Detección de duplicados | Porcentaje de marcadores dentro del radio configurado señalados como conflicto | 100 % | desde el lanzamiento |
| Resolución de conflictos | Porcentaje de conflictos resueltos por el jefe de área antes del cierre del relevamiento | ≥ 95 % | 6 meses post-lanzamiento |
| Unificaciones automáticas no deseadas | Cantidad de marcadores unificados o descartados sin decisión humana | 0 | continuo |
| Consistencia de la base | Porcentaje de relevamientos cerrados sin conflictos pendientes | ≥ 98 % | 6 meses post-lanzamiento |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Jefe general | Propietario | Exige consistencia de la base como condición de confianza del organismo |
| Equipo de desarrollo | Implementador | Construye la detección por radio configurable y el flujo de resolución manual |
| Jefe de área | Beneficiario | Resuelve cada conflicto desde la web con criterio sobre el terreno |
| Usuario raíz | Beneficiario | Interviene para resolver incoherencias que excedan al jefe de área |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-05 | CU-11 detectar y listar marcadores en conflicto por radio configurable | a generar |
| NB-05 | CU-12 resolver conflictos de sincronización desde la web | a generar |

## 8. Dependencias con otras NB

Depende de NB-03 (los conflictos surgen al sincronizar el trabajo sin conexión) y de NB-02 (la cercanía se evalúa sobre observaciones georreferenciadas). No es prerequisito de otras NB.

## 9. Prioridad MoSCoW

Should Have. Mejora sustancialmente la confianza en la base y atiende los riesgos R-01 y de edición concurrente, pero el MVP de recolección y evaluación puede demostrarse con resolución acotada.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-31 | Versión inicial generada por AG-01 a partir de los intakes y la categoría 00 |
