# Wireframe — Mapa de revisión con pines (web admin)

**Proyecto:** GeoVial
**Documento:** wireframes-mapa-revision-web_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** UX/UI Designer + Frontend Lead (AG-03), Equipo SDD 2.0
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie web de revisión del jefe de área. Muestra los marcadores de un relevamiento ubicados sobre el mapa y la bandeja sin georreferenciar, para que el jefe recorra las observaciones, evalúe el estado de la obra y exporte o importe el relevamiento completo. Es la pantalla principal del flujo de revisión en escritorio (home funcional del jefe de área).

## 2. Layout

Escritorio (web):

```
+--------------------------------------------------------------+
| GeoVial | Relevamiento: "Puente Río 12"  [Estado: revisión]  |
|--------------------------------------------------------------|
| [Filtrar por etiqueta v] [Exportar] [Importar] [Conflictos(2)]|
|--------------------------------------------------------------|
| Panel lista (marcadores)   |        Mapa con pines           |
| --------------------------  |  ............................. |
| > M-01  3 obs  [grietas]    |   (o)        (o)               |
|   M-02  1 obs  [junta]      |       (o)                      |
|   M-03  5 obs  [pavimento]  |            (o)   (o)           |
|   ...                       |   .............................|
|----------------------------|                                 |
| Bandeja sin georreferenciar |  [+] crear / ubicar punto      |
| (2 observaciones)           |                                 |
+--------------------------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Encabezado de relevamiento | Identificar el relevamiento y su estado | Nombre, estado (recolección/revisión/cerrado) | Estado visible en todo momento (RN-05) |
| Mapa con pines | Ubicar los marcadores en el terreno | Marcadores por coordenada, agrupados si están cercanos | Zoom, desplazamiento, selección de pin; agrupamiento de pines cercanos |
| Panel lista de marcadores | Recorrer marcadores sin depender del mapa | Marcador, conteo de observaciones, etiquetas | Sincronizado con el mapa: seleccionar en uno resalta en el otro |
| Bandeja sin georreferenciar | Acceder a observaciones sin ubicación | Conteo y acceso a la lista | Abre las observaciones pendientes de ubicar (CU-05) |
| Filtro por etiqueta | Acotar observaciones mostradas | Etiquetas disponibles | Filtra mapa y lista (CU-08, flujo 5.C) |
| Acción exportar / importar | Resguardar o reconstruir el relevamiento completo | — | Genera o consume el archivo comprimido (CU-08, 5.A/5.B); muestra progreso |
| Acceso a conflictos | Saltar a la resolución de conflictos | Conteo de conflictos pendientes | Lleva a la superficie de resolución (CU-11, CU-12) |
| Crear / ubicar punto | Crear o ubicar un marcador desde la web | — | Coloca un pin en el mapa (relevamiento no cerrado) |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Seleccionar marcador | Clic en pin o en ítem de la lista | Abre el carrusel del marcador (ver wireframes-marcador-carrusel) | Marcador existente |
| Filtrar por etiqueta | Selección en el filtro | Mapa y lista muestran solo lo que coincide | Hay etiquetas en el relevamiento |
| Exportar relevamiento | Clic en exportar | Archivo comprimido único con datos, comentarios, etiquetas y fotos; acción auditada | Acceso por área (RN-01) |
| Importar relevamiento | Clic en importar y selección de archivo | Relevamiento reconstruido o rechazo con motivo | Archivo válido; acceso por área |
| Crear / ubicar punto | Clic en crear y clic en el mapa | Nuevo marcador en la coordenada elegida | Relevamiento no cerrado (RN-05) |
| Abrir conflictos | Clic en conflictos | Superficie de resolución de conflictos | Hay conflictos detectados (CU-11) |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Relevamiento sin marcadores | Mapa con mensaje orientativo y acceso a la bandeja sin georreferenciar |
| Cargando | Carga de marcadores | Skeleton del panel lista y del mapa |
| Con datos | Marcadores ubicados | Pines en el mapa y lista sincronizada; conteos visibles |
| Error | `ACCESO_NO_AUTORIZADO` o `ARCHIVO_EXPORTACION_INVALIDO` | Banner inline con causa y acción siguiente; los datos existentes no se alteran |
| Solo lectura | Relevamiento cerrado (RN-05) | Crear/ubicar e importar inhabilitados; revisión y exportación disponibles |
| Éxito | Exportación o importación completada | Confirmación; descarga o relevamiento reconstruido |

## 6. Versión móvil o responsive

- Superficie pensada para escritorio (revisión analítica del jefe de área). En anchos reducidos, el panel lista y el mapa se apilan: el mapa arriba y la lista debajo, conmutables por pestañas, sin perder el filtro ni los accesos a exportar/importar y conflictos.
- El agrupamiento de pines se vuelve más agresivo en pantallas chicas para mantener la legibilidad.

## 7. Notas de implementación

- Accesibilidad: operación completa por teclado del mapa, la lista, el filtro y las acciones; los pines exponen nombre y conteo como texto accesible (no solo color/forma); foco visible; cambios de estado (filtro aplicado, exportación lista) anunciados por región en vivo (WCAG 2.2 AA).
- Performance percibida: skeletons durante la carga; agrupamiento de pines para no renderizar todos de golpe; la exportación muestra progreso y no se presenta como instantánea.
- Internacionalización: distancias y radios en metros; coordenadas en grados decimales; el jefe opera sobre el mapa, no sobre números crudos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU origen | CU-08 (revisar sobre mapa, exportar/importar, filtrar); enlaza con CU-09 (carrusel) y CU-11/CU-12 (conflictos) |
| Reglas de negocio | RN-01, RN-05, RN-07, RN-08 |
| Marco experiencia-de-uso | §3.4 (flujo de revisión), §4.3 (estados del mapa), §5 (accesibilidad) |
| US a generar | a generar en 06 (revisión sobre mapa, exportar/importar, filtrar por etiqueta, crear/ubicar punto en web) |
| Tests previstos | a generar en 08 (revisión sobre mapa, exportación/importación, accesibilidad del mapa y de la lista) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Wireframe inicial del mapa de revisión web, generado por AG-03 a partir de CU-08 |
