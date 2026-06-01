# Glosario UX — GeoVial

**Proyecto:** GeoVial
**Documento:** glosario-ux_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** UX/UI Designer + Frontend Lead (AG-03), Equipo SDD 2.0
**Variante:** UX/UI

## 1. Propósito y alcance

Este glosario fija la terminología canónica de la categoría 03 (vocabulario de interfaz y de experiencia) para GeoVial. No redefine los términos del dominio que ya están en el glosario de 02 (modelo-conceptual_v1.0.md) ni en el de 00 (vision-producto_v1.0.md §9) con la misma semántica: esos se referencian. Acá solo se agregan los términos propios de la capa de presentación.

## 2. Términos del dominio referenciados (no se redefinen)

Estos términos conservan su semántica de 00/02 y se usan con ese significado en 03. Para su definición autoritativa, ver el glosario del dominio (vision-producto_v1.0.md §9 y modelo-conceptual_v1.0.md).

| Término | Fuente autoritativa | Uso en 03 |
| --- | --- | --- |
| Relevamiento | 00/02 | Unidad de trabajo que se crea, revisa y cierra en la web y se recolecta en móvil |
| Observación | 00/02 | Contenido capturado (foto, comentario) que se muestra y gestiona en el carrusel |
| Marcador geográfico | 00/02 | Punto en el mapa; en la interfaz se representa como pin y se gestiona en la vista de marcador. Ver término de UI "pin" más abajo |
| Etiqueta | 00/02 | Ficha aplicable a fotos o comentarios; en la interfaz se muestra como chip seleccionable y se usa para filtrar |
| Sincronización | 00/02 | Proceso cuya progresión y resultado se reflejan en estados y feedback de la app móvil |
| Georreferenciación | 00/02 | Asignación automática de coordenadas al capturar; en la interfaz dispara la creación o asociación del marcador |
| Modo sin conexión | 00/02 | Capacidad de operar sin señal; en 03 es un estado de interfaz central de la captura móvil |
| Conflicto de sincronización | 00/02 | Situación que la interfaz lista y permite resolver en la web |
| Bandeja sin georreferenciar | 02 (CU-05) | Agrupación lógica de observaciones sin coordenada dentro de un relevamiento; en 03 es una superficie de acceso en web y móvil |
| Radio de agrupación | 02 (RN-02) | Parámetro del relevamiento; en la interfaz se ingresa en metros y condiciona la creación de marcadores y la detección de conflictos |
| Agente de campo / Jefe de área / Usuario raíz / Jefe general | 00/02 | Roles cuya experiencia se diseña en 03 |

## 3. Términos propios de la capa de presentación (nuevos en 03)

Términos de interfaz y experiencia que no existen como tales en 00/02 y que esta categoría define para evitar ambigüedad cross-doc.

| Término | Definición (UX) | Notas |
| --- | --- | --- |
| Pantalla | Superficie completa que ocupa la vista del usuario en un momento dado (por ejemplo, la captura móvil). En móvil, una pantalla por tarea | Unidad de los wireframes |
| Vista | Disposición de contenido dentro de una pantalla que puede cambiar sin navegar a otra pantalla (por ejemplo, la vista de un marcador) | — |
| Pin | Representación de interfaz de un marcador geográfico sobre el mapa | Es la forma visual del término de dominio "marcador"; se nombra "pin" en la interfaz por convención conocida (Ley de Jakob) |
| Carrusel | Componente que muestra las fotos de un marcador de a una, con miniaturas como índice y navegación adelante/atrás | Materializa la visualización de observaciones de CU-09 |
| Modal | Capa superpuesta que concentra una tarea acotada y bloquea el fondo hasta resolverla (por ejemplo, confirmar el cierre de un relevamiento) | Se usa con moderación; en móvil se prefiere pantalla a página completa |
| Toast | Confirmación efímera y no intrusiva de una acción (por ejemplo, "Observación guardada") | Para feedback de éxito sutil; no para errores que requieren acción |
| Banner inline | Aviso persistente dentro del contenido, con causa y acción siguiente, para errores recuperables | Para errores que el usuario debe atender |
| Cinta de estado de conexión | Indicador persistente del estado de conectividad y de pendientes en la app móvil (sin conexión / sincronizando / sincronizado) | Central en la experiencia offline |
| Estado vacío | Representación de una superficie sin datos aún, con texto orientativo y acción siguiente | Estado mínimo obligatorio en cada wireframe |
| Estado sin conexión | Representación de interfaz cuando no hay señal; en captura móvil es un estado de primera clase, no un error | Distinto de "error" |
| Estado solo lectura | Representación de una superficie cuyo relevamiento está cerrado (RN-05): se consulta pero no se edita | Acompañado de aviso textual, no solo visual |
| Skeleton | Marcador de posición de la estructura del contenido mientras carga, en lugar de un spinner | Para performance percibida en cargas de mapa, lista y carrusel |
| Panel lista | Lista de marcadores sincronizada con el mapa en la revisión web; seleccionar en uno resalta en el otro | Permite recorrer sin depender solo del mapa |
| Miniatura | Versión reducida de una foto usada como índice del carrusel | — |
| Chip de etiqueta | Representación de interfaz de una etiqueta como ficha seleccionable | Forma visual del término de dominio "etiqueta" |
| Microcopy | Textos breves de interfaz que guían y explican en el punto de uso (por ejemplo, por qué se pide el método de seguridad) | Tono llano, español rioplatense |
| Disparador de foto | Control primario de la captura móvil que toma la foto y dispara la georreferenciación | Objetivo táctil grande (Ley de Fitts) |
| Región en vivo | Zona de la interfaz cuyos cambios se anuncian a tecnologías de asistencia (estado de conexión, progreso, resultado) | Criterio de accesibilidad WCAG 2.2 AA |

## 4. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Glosario UX inicial: referencia a términos de dominio de 00/02 sin duplicar y términos propios de presentación, generado por AG-03 |
