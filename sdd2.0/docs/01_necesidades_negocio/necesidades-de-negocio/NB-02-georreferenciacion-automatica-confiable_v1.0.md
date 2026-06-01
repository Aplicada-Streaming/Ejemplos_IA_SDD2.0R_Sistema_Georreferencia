# NB-02 — Georreferenciación automática y confiable de las observaciones

| Campo | Valor |
| --- | --- |
| Proyecto | GeoVial |
| Documento | NB-02-georreferenciacion-automatica-confiable_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-05-31 |
| Autor | Analista de Negocio Senior (AG-01), Equipo SDD 2.0 |
| Trazabilidad upstream | PROJECT-BRIEF §3, §8, §11; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-04, CU-05 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita que cada observación de campo quede ubicada en el mapa de forma automática y confiable, sin que el relevador cargue coordenadas a mano. Hoy la ubicación se transcribe manualmente o se infiere de fotos guardadas en soportes sueltos, lo que produce registros sin posición, con posición errónea o imposibles de cruzar contra el punto físico de la obra.

El dolor concreto es que la calidad de la base de datos depende de un dato de ubicación que hoy es frágil: se pierde, se carga mal o no existe. Como la evaluación posterior se hace sobre el mapa, una observación mal georreferenciada vale poco o nada para el experto que evalúa. La necesidad es que la asignación de coordenadas ocurra en el momento de capturar la observación y que el porcentaje de observaciones con coordenada válida sea alto y medible.

Si esta necesidad no se resuelve, la base georreferenciada nace inconsistente, la revisión sobre mapa pierde valor y la organización no puede confiar en la ubicación de lo relevado para tomar decisiones de control.

## 2. Ejemplo de uso desde la perspectiva del negocio

Una cuadrilla recorre un camino y registra una fisura en un punto concreto. En el método actual, la persona anota una referencia aproximada en la planilla y saca una foto que luego nadie sabe ubicar con precisión. Con la necesidad resuelta, al registrar la observación la ubicación queda asignada automáticamente al punto donde se capturó, de modo que el jefe de área la encuentra exactamente sobre el mapa cuando evalúa.

## 3. Impacto

- Eleva la confiabilidad de la base de datos georreferenciada que sostiene toda la evaluación.
- Elimina la carga manual de coordenadas y los errores de transcripción de ubicación.
- Hace utilizable la revisión sobre mapa, porque cada observación cae donde corresponde.
- Si no se resuelve, la base nace con ubicaciones dudosas y la evaluación experta pierde sustento.
- Reduce el retrabajo de reubicar o descartar observaciones mal posicionadas.

## 4. Problema específico que resuelve

- Las observaciones se registran hoy sin coordenada o con coordenada cargada a mano y propensa a error.
- La ubicación se pierde cuando la foto queda en un soporte suelto sin metadatos asociados.
- No hay forma medible de saber qué proporción de observaciones tiene posición válida.
- En carga manual, cuando la foto no trae metadatos de ubicación, no existe un mecanismo para resolver el punto.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Confiabilidad de la georreferenciación | Porcentaje de observaciones con coordenada válida asignada automáticamente | ≥ 95 % | continuo, revisión mensual |
| Carga manual sin posición resuelta | Porcentaje de observaciones sin ubicación resueltas con ajuste manual del punto | ≥ 90 % | 6 meses post-lanzamiento |
| Eliminación de carga manual de coordenadas | Coordenadas tipeadas a mano por el relevador por observación | 0 | desde el lanzamiento |
| Observaciones sin georreferenciar pendientes | Porcentaje de observaciones que quedan en bandeja sin georreferenciar al cerrar el relevamiento | ≤ 5 % | continuo, revisión mensual |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Jefe general | Propietario | Exige una base georreferenciada confiable como activo del organismo |
| Equipo de desarrollo | Implementador | Construye la asignación automática de coordenadas y el ajuste manual del punto |
| Agente de campo | Beneficiario | Captura sin cargar coordenadas y resuelve los casos sin ubicación |
| Jefe de área | Beneficiario | Evalúa sobre el mapa confiando en la posición de cada observación |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-02 | CU-04 capturar observación con asignación automática de coordenadas | a generar |
| NB-02 | CU-05 ubicar manualmente el punto cuando falta la posición de la foto | a generar |

## 8. Dependencias con otras NB

Depende de NB-01 (la recolección la realiza personal de campo no experto, que es quien captura las observaciones georreferenciadas). Es prerequisito de NB-04, que evalúa sobre mapa lo georreferenciado.

## 9. Prioridad MoSCoW

Must Have. Una base con ubicaciones poco confiables invalida la revisión sobre mapa y la propuesta de valor central del producto.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-31 | Versión inicial generada por AG-01 a partir de los intakes y la categoría 00 |
