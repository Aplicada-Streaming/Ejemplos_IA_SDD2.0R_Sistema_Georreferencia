# NB-01 — Delegación de la recolección en personal no experto

| Campo | Valor |
| --- | --- |
| Proyecto | GeoVial |
| Documento | NB-01-delegacion-recoleccion-personal-no-experto_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-05-31 |
| Autor | Analista de Negocio Senior (AG-01), Equipo SDD 2.0 |
| Trazabilidad upstream | PROJECT-BRIEF §1, §3, §4, §8; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-01, CU-02, CU-03 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita dejar de depender de personal experto para ir al terreno a recolectar el estado de puentes y caminos. Hoy la recolección descriptiva y fotográfica de observaciones es manual, lenta y atada a una persona con criterio especializado, lo que limita la cantidad de obras que se pueden relevar en un período y concentra un trabajo de campo costoso en perfiles escasos.

El dolor concreto es doble y comparte una misma raíz: por un lado, el criterio experto se gasta caminando obra en lugar de evaluar; por otro, la cantidad de obras relevadas por trimestre queda topada por la disponibilidad de esos perfiles. La organización quiere que la recolección la haga personal de campo menos especializado, de modo que el experto reciba después, de forma centralizada, la información ya capturada y se concentre en evaluar. Escalar la recolección y aumentar la productividad son, en este caso, la misma necesidad medida con dos métricas distintas.

Si esta necesidad no se resuelve, la organización seguirá sin poder ampliar la cobertura de relevamientos, mantendrá el cuello de botella en el personal experto y no podrá construir la base de datos consistente que habilita los pasos siguientes del negocio.

## 2. Ejemplo de uso desde la perspectiva del negocio

Un jefe de área necesita relevar quince puentes de su zona en un trimestre, pero solo cuenta con dos evaluadores expertos que además deben confeccionar informes. Hoy esos expertos viajan a cada obra, completan planillas y juntan fotos sueltas, y apenas llegan a relevar la mitad. Con la necesidad resuelta, el jefe de área asigna cuadrillas de campo no expertas para que recorran las obras y registren las observaciones, y reserva a sus dos expertos para evaluar lo recolectado sin moverse de la oficina.

## 3. Impacto

- Se libera tiempo del personal experto, que pasa de recolectar en terreno a evaluar de forma centralizada.
- Se incrementa la cantidad de obras que la organización puede relevar por trimestre con la misma dotación experta.
- Se amplía la base de personal habilitado para recolectar, reduciendo la dependencia de perfiles escasos.
- Si no se resuelve, persiste el cuello de botella de relevamiento y la organización no escala su cobertura de control.
- Habilita aguas abajo la consolidación de una base de datos uniforme sobre la cual evaluar el estado de la infraestructura.

## 4. Problema específico que resuelve

- La recolección depende de personal experto escaso, lo que topa la cantidad de obras relevables.
- El criterio experto se consume en tareas de campo en lugar de en la evaluación.
- La productividad de relevamiento está limitada por la dotación de perfiles especializados.
- No existe un mecanismo que permita asignar la recolección a cuadrillas no expertas con un resultado utilizable después.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Delegación de la recolección | Porcentaje de relevamientos recolectados por agentes de campo no expertos sobre el total | ≥ 80 % | 6 meses post-lanzamiento |
| Productividad del relevamiento | Cantidad de obras relevadas por trimestre respecto de la línea de base manual | + 50 % | 9 meses post-lanzamiento |
| Adopción del personal de campo | Agentes de campo que completan al menos un relevamiento sobre el total asignado | ≥ 70 % | 6 meses post-lanzamiento |
| Liberación del criterio experto | Reducción de horas de personal experto dedicadas a recolección en terreno | ≥ 50 % | 9 meses post-lanzamiento |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Jefe general | Propietario | Aprueba la prioridad de delegar la recolección y financia el cambio operativo |
| Equipo de desarrollo | Implementador | Construye y mantiene el mecanismo de asignación de agentes a relevamientos |
| Jefe de área | Beneficiario | Asigna cuadrillas no expertas y valida que se libera tiempo experto para evaluar |
| Agente de campo | Beneficiario | Recolecta en terreno y valida que puede operar sin criterio experto previo |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-01 | CU-01 crear relevamiento y asignar agentes de campo del área | a generar |
| NB-01 | CU-02 seleccionar un relevamiento asignado para recolectar | a generar |
| NB-01 | CU-03 administrar la jerarquía de usuarios y áreas | a generar |

## 8. Dependencias con otras NB

Sin dependencias. Es la necesidad raíz del proyecto y prerequisito de NB-02, NB-03 y NB-06.

## 9. Prioridad MoSCoW

Must Have. Es el problema central declarado por el negocio; sin delegar la recolección no hay MVP defendible ni escala posible del relevamiento.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-31 | Versión inicial generada por AG-01 a partir de los intakes y la categoría 00 |
