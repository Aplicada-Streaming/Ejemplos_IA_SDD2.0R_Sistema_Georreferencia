# NB-04 — Revisión y evaluación centralizada sobre mapa para confeccionar informes

| Campo | Valor |
| --- | --- |
| Proyecto | GeoVial |
| Documento | NB-04-revision-centralizada-sobre-mapa_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-05-31 |
| Autor | Analista de Negocio Senior (AG-01), Equipo SDD 2.0 |
| Trazabilidad upstream | PROJECT-BRIEF §1, §6, §4; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-08, CU-09, CU-10 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita un lugar único y centralizado donde el personal experto revise lo recolectado en terreno y arme sus informes rutinarios. Hoy esa evaluación obliga a cruzar a mano una planilla contra una carpeta de fotos dispersas, una tarea lenta y propensa a error que no escala con la cantidad de obras.

El dolor concreto es que la información recolectada llega fragmentada: las descripciones por un lado, las fotos por otro, sin un punto común que las relacione con el lugar de la obra. El experto pierde tiempo reconstruyendo ese vínculo en lugar de evaluar. La necesidad es concentrar la revisión sobre un mapa donde cada punto agrupe sus fotos y comentarios, con un recorrido cómodo entre puntos y entre fotos, de modo que el jefe de área y el área central puedan evaluar y confeccionar informes a partir de una vista consolidada.

Si esta necesidad no se resuelve, la inversión en delegar y georreferenciar la recolección se desperdicia, porque el cuello de botella se traslada a la etapa de evaluación, que sigue siendo manual y artesanal.

## 2. Ejemplo de uso desde la perspectiva del negocio

Un jefe de área tiene que evaluar el estado de cinco puentes ya relevados y preparar su informe trimestral. En el método actual, abre una planilla, busca las fotos en carpetas separadas e intenta adivinar qué imagen corresponde a qué anotación. Con la necesidad resuelta, abre el mapa del relevamiento, toca cada punto, recorre sus fotos y comentarios en un carrusel y evalúa obra por obra sin reconstruir nada a mano, dejando todo listo para volcar al informe.

## 3. Impacto

- Concentra la evaluación experta en una vista única consolidada por punto geográfico.
- Elimina el cruce manual de planilla contra carpeta de fotos.
- Acelera la confección de los informes rutinarios del organismo.
- Si no se resuelve, el cuello de botella se traslada de la recolección a la evaluación.
- Da valor de uso al área central, que recibe información lista para confeccionar informes.

## 4. Problema específico que resuelve

- La información recolectada llega fragmentada entre descripciones y fotos sin un punto común.
- El cruce manual de planilla y fotos es lento y propenso a asociar mal foto y observación.
- No existe una vista centralizada por punto para recorrer y evaluar lo relevado.
- La confección de informes rutinarios carece de una fuente consolidada y navegable.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Reducción del tiempo de evaluación | Tiempo de evaluación de un relevamiento respecto del método manual | − 50 % | 9 meses post-lanzamiento |
| Consolidación por punto | Porcentaje de observaciones accesibles desde su marcador con fotos y comentarios agrupados | 100 % | desde el lanzamiento |
| Cobertura de evaluación centralizada | Porcentaje de relevamientos evaluados desde el mapa sin recurrir a soportes externos | ≥ 95 % | 6 meses post-lanzamiento |
| Soporte a la confección de informes | Relevamientos cerrados con información suficiente para el informe sin pedir datos adicionales | ≥ 90 % | 6 meses post-lanzamiento |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Jefe general | Propietario | Aprueba la prioridad de centralizar la evaluación y confeccionar informes |
| Equipo de desarrollo | Implementador | Construye la revisión sobre mapa con marcadores, carrusel y navegación |
| Jefe de área | Beneficiario | Evalúa sobre el mapa, cierra relevamientos y confecciona los informes |
| Área central de evaluación | Beneficiario | Recibe la información consolidada para sus informes rutinarios |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-04 | CU-08 revisar un relevamiento sobre el mapa por marcadores | a generar |
| NB-04 | CU-09 recorrer fotos y comentarios de un marcador en carrusel | a generar |
| NB-04 | CU-10 transicionar el relevamiento entre recolección, revisión y cierre | a generar |

## 8. Dependencias con otras NB

Depende de NB-02 (la revisión sobre mapa requiere observaciones georreferenciadas) y de NB-03 (la información a revisar llega por sincronización desde el campo). No es prerequisito de otras NB.

## 9. Prioridad MoSCoW

Must Have. Es la etapa que convierte lo recolectado en valor evaluable; sin ella el resto de la cadena no produce el resultado de negocio buscado.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-31 | Versión inicial generada por AG-01 a partir de los intakes y la categoría 00 |
