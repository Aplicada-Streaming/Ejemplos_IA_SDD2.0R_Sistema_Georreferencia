# Wireframe — Marcador con carrusel de fotos

**Proyecto:** GeoVial
**Documento:** wireframes-marcador-carrusel_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** UX/UI Designer + Frontend Lead (AG-03), Equipo SDD 2.0
**Variante:** UX/UI

## 1. Pantalla y propósito

Vista de un marcador: el carrusel de fotos con sus comentarios por foto, las etiquetas de fotos y comentarios, y la navegación entre marcadores. Sirve tanto al agente de campo (gestión: agregar/quitar fotos, comentar, etiquetar) como al jefe de área (revisión y, si el relevamiento está cerrado, solo lectura).

## 2. Layout

Vista de marcador (aplica a web y a las páginas integradas en móvil):

```
+--------------------------------------------------+
| < Marcador M-03   (3 de 12 marcadores)        >  |
|--------------------------------------------------|
|                                                  |
|   +------------------------------------------+   |
|   |               Foto actual                |   |
|   |               (4 de 8)                   |   |
|   +------------------------------------------+   |
|   [ o ][ o ][#][ o ][ o ][ o ][ o ][ o ]         |
|   (miniaturas, # = posicion actual)              |
|                                                  |
|   Etiquetas de la foto:  [grietas] [+ agregar]   |
|--------------------------------------------------|
| Comentario de la foto:                           |
| "Fisura longitudinal en viga ..."                |
| [ Agregar comentario ]                           |
|--------------------------------------------------|
| [ Agregar foto ]   [ Quitar foto ]               |
+--------------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Encabezado de marcador | Ubicar el marcador y permitir navegar entre marcadores | Identificador del marcador, posición (3 de 12) | Flechas anterior/siguiente cargan el marcador destino con su carrusel (CU-09, 5.B) |
| Carrusel de fotos | Recorrer las fotos del marcador | Foto actual, posición (4 de 8) | Avance/retroceso; miniaturas como índice; la siguiente se precarga |
| Tira de miniaturas | Dar contexto y salto directo | Miniaturas con marca de posición | Clic en miniatura salta a esa foto |
| Etiquetas de foto/comentario | Clasificar para filtrado posterior | Etiquetas aplicadas | Agregar y quitar etiquetas (relevamiento no cerrado) |
| Comentario | Texto asociado a la foto o al marcador | Comentario actual | Agregar, editar; una foto puede o no tener comentario |
| Agregar / quitar foto | Gestionar el contenido del marcador | — | Agrega una foto; quitar elimina la foto con sus comentarios y etiquetas (CU-09, 5.A) |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Avanzar en el carrusel | Gesto/clic siguiente o clic en miniatura | Muestra la foto destino con su comentario y etiquetas | Marcador con fotos |
| Navegar entre marcadores | Flecha anterior/siguiente del encabezado | Carga el marcador destino con su carrusel | Existe marcador destino |
| Agregar foto | Clic en agregar foto | La foto se suma al carrusel | Relevamiento no cerrado (RN-05) |
| Quitar foto | Clic en quitar foto y confirmación | La foto y sus comentarios/etiquetas se eliminan; el resto se conserva | Relevamiento no cerrado |
| Comentar / etiquetar | Acción sobre la foto o el comentario | El comentario o la etiqueta se persiste | Relevamiento no cerrado |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Marcador sin fotos | Marco neutro con mensaje y acción de agregar foto |
| Cargando | Fotos cargándose | Skeleton de miniaturas y del visor |
| Con datos | Fotos con comentarios y etiquetas | Carrusel con posición "X de N" y miniaturas |
| Error | `MARCADOR_INEXISTENTE` | Aviso y retorno a la vista del relevamiento |
| Solo lectura | Relevamiento cerrado (RN-05) | Carrusel y navegación disponibles; agregar/quitar/comentar/etiquetar inhabilitados, con aviso de solo lectura |
| Éxito | Cambio persistido | Confirmación sutil; el carrusel refleja el nuevo conteo |

## 6. Versión móvil o responsive

- En móvil portrait la foto ocupa el ancho útil; el avance del carrusel se hace por gesto lateral además de los controles visibles; las miniaturas se desplazan horizontalmente.
- Los controles de gestión (agregar/quitar/comentar) quedan al alcance del pulgar bajo el visor.
- En anchos amplios de escritorio el comentario y las etiquetas pueden ubicarse al costado de la foto en lugar de debajo.

## 7. Notas de implementación

- Accesibilidad: cada foto expone su comentario y etiquetas como texto asociado (alternativa textual); el carrusel es operable por teclado y anuncia la posición ("foto 4 de 8") por región en vivo; el foco no queda atrapado; los controles tienen nombre accesible; no se depende solo del color para la posición actual (WCAG 2.2 AA).
- Performance percibida: precarga de la foto siguiente; miniaturas livianas; avance por debajo del umbral de respuesta percibido.
- Internacionalización: comentarios y etiquetas en español; contenedores con holgura para textos largos sin truncar el comentario del agente.
- Nota: el visor a pantalla completa con zoom es un Could Have del alcance; se contempla como evolución del carrusel sin fijar su detalle visual fino (eso es 05).

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU origen | CU-09 (gestionar marcador, carrusel y navegación); se invoca desde CU-08 (revisión) y CU-04 (captura) |
| Reglas de negocio | RN-01, RN-05 |
| Marco experiencia-de-uso | §3.4 (revisión), §4.4 (estados del carrusel), §5 (accesibilidad) |
| US a generar | a generar en 06 (carrusel, comentar, etiquetar, agregar/quitar foto, navegación entre marcadores) |
| Tests previstos | a generar en 08 (carrusel, navegación, solo lectura tras cierre, accesibilidad del carrusel) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Wireframe inicial del marcador con carrusel, generado por AG-03 a partir de CU-09 |
