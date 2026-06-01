# Wireframe — Captura móvil en terreno (offline)

**Proyecto:** GeoVial
**Documento:** wireframes-captura-movil_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** UX/UI Designer + Frontend Lead (AG-03), Equipo SDD 2.0
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie de captura en terreno para el agente de campo: tomar una foto con georreferenciación automática, ver y mover el pin o recentrar por posicionamiento, y operar sin conexión guardando todo localmente. Es la pantalla más crítica del producto en campo y la principal mitigación del riesgo de baja adopción (R-04): captura rápida, una acción primaria, estado sin conexión siempre visible.

## 2. Layout

Móvil portrait, modo captura:

```
+----------------------------------+
| (cinta) Sin conexión · 12 pend.  |
|----------------------------------|
|                                  |
|        Mapa con tu posicion      |
|            y los pines           |
|     (o)        (x = vos)         |
|          (o)        (o)          |
|                                  |
|         [ Centrar por GPS ]      |
|----------------------------------|
|                                  |
|            (  O  )  <- tomar foto|
|                                  |
| [ Bandeja sin georref. (2) ]     |
+----------------------------------+
```

Móvil portrait, tras tomar la foto (georreferenciación automática):

```
+----------------------------------+
| (cinta) Sin conexión · 13 pend.  |
|----------------------------------|
|   Foto tomada — ubicada por      |
|   metadatos en el mapa           |
|   +--------------------------+   |
|   |        miniatura         |   |
|   +--------------------------+   |
|   Comentario: [............]     |
|   Etiquetas: [grietas][+]        |
|                                  |
|   [ Guardar observacion ]        |
|   [ Mover pin en el mapa ]       |
+----------------------------------+
```

Móvil portrait, foto sin ubicación (RN-03):

```
+----------------------------------+
| Esta foto no trae ubicacion.     |
|                                  |
| [ Ubicar punto en el mapa ]      |
| [ Dejar sin georreferenciar ]    |
+----------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Cinta de estado | Hacer visible conexión y pendientes (Nielsen: visibilidad del estado) | Sin conexión / sincronizando; conteo de pendientes | Persistente; cambia con la conectividad |
| Mapa con posición y pines | Ubicar al agente y los marcadores ya capturados | Posición actual y pines del relevamiento | Mover pin, recentrar por posicionamiento |
| Disparador de foto | Acción primaria de captura | — | Objetivo táctil grande y central (Ley de Fitts); captura y dispara la georreferenciación |
| Centrar por GPS | Recentrar el mapa en la posición actual | — | Un toque recentra sin recargar el mapa |
| Comentario y etiquetas | Enriquecer la observación | Texto y etiquetas | Agregar comentario y etiquetas a la foto/observación |
| Mover pin | Ajustar la ubicación del marcador | Pin actual | Arrastre del pin sobre el mapa |
| Bandeja sin georreferenciar | Acceder a observaciones sin ubicación | Conteo local | Abre las observaciones pendientes de ubicar (CU-05) |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Tomar foto | Toque en el disparador | Foto capturada y coordenada resuelta por metadatos (RN-03); marcador creado o asociado por radio (RN-02) | Relevamiento abierto en recolección (RN-05) |
| Recentrar por posicionamiento | Toque en centrar por GPS | Mapa centrado en la posición actual | — |
| Mover pin | Arrastre del pin | Coordenada del marcador ajustada | Relevamiento no cerrado |
| Ubicar punto sin metadatos | Toque en ubicar punto en el mapa | Coordenada manual asentada y marcador creado/asociado (CU-05) | Foto sin ubicación |
| Dejar sin georreferenciar | Toque en dejar sin georreferenciar | Observación derivada a la bandeja sin georreferenciar local, encolada igual | Foto sin ubicación |
| Guardar observación | Toque en guardar | Observación asentada localmente y encolada; contador de pendientes +1 | Captura válida |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Relevamiento abierto sin observaciones | Mapa centrado y disparador destacado; mensaje "Tomá la primera foto" |
| Cargando | Posicionamiento o foto procesándose | Indicador "Ubicando…" breve sobre el disparador |
| Con datos | Observaciones capturadas en la jornada | Pines en el mapa y contador de pendientes |
| Error | `OBSERVACION_SIN_GEORREFERENCIA` o `ALMACENAMIENTO_LOCAL_INSUFICIENTE` | Aviso inline con acción siguiente; lo ya guardado se conserva |
| Sin conexión | Sin señal en terreno (estado central) | Cinta persistente de sin conexión con conteo de pendientes |
| Éxito | Observación guardada localmente | Confirmación sutil y pin nuevo en el mapa |

## 6. Versión móvil o responsive

- Superficie nativa de móvil en portrait; es el orden de captura natural sosteniendo el teléfono con una mano. No se diseña variante de escritorio: la captura es exclusiva de la app móvil.
- En landscape el mapa se ensancha y los controles de captura se reubican al lateral dominante, manteniendo el disparador al alcance del pulgar.
- Objetivos táctiles amplios y separados para reducir errores con guantes o en movimiento (Ley de Fitts).

## 7. Notas de implementación

- Accesibilidad: el disparador y los controles tienen nombre accesible y tamaño de objetivo acorde a 2.2; el estado sin conexión y el resultado de guardado se anuncian por región en vivo; no se depende solo del color para distinguir sin conexión, pendiente y guardado (se acompaña con ícono y texto); foco visible (WCAG 2.2 AA).
- Performance percibida: confirmación optimista local de la captura por debajo del umbral percibido (Ley de Doherty); la coordenada y el encolado se resuelven sin bloquear la siguiente foto; recentrar por posicionamiento sin recargar el mapa.
- Internacionalización: textos en español; distancias en metros; coordenadas resueltas automáticamente, el agente no ingresa números.
- Modo sin conexión: el guardado y el encolado son íntegramente locales; la sincronización ocurre al recuperar señal (ver experiencia-de-uso §4.2 y CU-07), no en esta pantalla.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU origen | CU-04 (captura georreferenciada), CU-06 (recolección sin conexión); deriva a CU-05 (ubicación manual) y al carrusel de CU-09 |
| Reglas de negocio | RN-02 (radio), RN-03 (prioridad de metadatos), RN-05 (estados), RN-06 (precondición del modo sin conexión) |
| Marco experiencia-de-uso | §3.2 (captura), §4.1 (estados de captura), §7 (performance percibida) |
| US a generar | a generar en 06 (captura georreferenciada, mover pin, centrar por posicionamiento, bandeja sin georreferenciar local) |
| Tests previstos | a generar en 08 (captura georreferenciada y agrupación por radio, operación sin conexión, accesibilidad táctil) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Wireframe inicial de la captura móvil offline, generado por AG-03 a partir de CU-04 y CU-06 |
