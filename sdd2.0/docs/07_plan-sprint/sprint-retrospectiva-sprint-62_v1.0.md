# Sprint Retrospectiva — Sprint 62

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-62_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-07
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Se cerró el backlog de P1 de la auditoría UX** (H-05, H-02, H-04, H-01/H-03 en S59–S62). La app móvil pasó de pestañas inconexas a una experiencia GIS de campo: estado de conexión persistente, posición por GPS, carrusel deslizable y captura sobre el mapa.
- **Reuso compuesto.** El mapa de la captura sale de tres piezas ya en el gate: `MapaRevisionHtml` (pines), `ScriptUbicacionDispositivo`+`UbicacionDispositivo` (S60) y `CacheTeselasDisco`/`MapaWebViewClient` (S38). El sprint fue glue.
- **Riesgo acotado en la pantalla crítica.** El mapa se trató como **contexto** (un `catch` evita que un fallo de carga tumbe la captura), sin tocar el flujo de captura ya probado.
- **Verificación on-device con datos reales.** Se reusaron los 2 marcadores sembrados en S61: la captura mostró ambos pines.

## 2. Qué no salió bien

- **Integración parcial de H-01.** El mapa de la captura es de sólo lectura (contexto); fijar la coordenada arrastrando el pin sobre **este** mapa (la visión completa del wireframe) no se hizo: la captura sigue usando EXIF / map-pick (S56) / manual. Es una decisión de alcance, pero conviene nombrarla para no dar por "100%" lo que es el núcleo de H-01.
- **Dos WebViews de mapa más.** Ya van varias superficies con el mismo armado de WebView (Mapa, map-pick, revisión, ahora captura). Se reitera la deuda de un **control de mapa compartido**.
- **Sin tests nuevos.** Correcto (reusa núcleos), pero la superficie de UI no cubierta por el gate sigue creciendo; depende de la verificación on-device.

## 3. Qué probar

- On-device: la captura muestra el mapa con los pines del relevamiento y "📍 Mi ubicación" recentra; tomar/elegir foto y encolar siguen funcionando con el mapa presente.
- Sin conexión la primera vez: el mapa puede no cargar, pero la captura igual encola (el mapa es contexto).

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Verificar on-device la captura sobre el mapa (con y sin conexión) | AG-05 (QA) | 2028-11-10 | En curso (con el usuario) |
| Control de mapa compartido (un solo armado de WebView reusable) | AG-08 | 2028-11-10 | Pendiente (deuda reiterada) |
| Evaluar fijar la coordenada de la captura desde el pin del mapa embebido (evolución de H-01) | Equipo | 2028-11-10 | Pendiente |
| Atacar los P2 de la auditoría (carga al entrar, disparador central, logout en overflow, contraste) | Equipo | 2028-11-10 | Planificado |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 61 | Estado |
| --- | --- |
| Verificar on-device el swipe con fotos reales | En curso (con el usuario) |
| Set de datos de demo con marcadores + fotos | Parcial (se sembraron marcadores por API en S61; faltan binarios) |
| Último P1: H-01/H-03 | **Hecho** (este sprint, en su núcleo) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-07 | Retro del Sprint 62 (H-01/H-03: mapa en la pantalla de captura). Cierra los 4 P1 de la auditoría UX reusando núcleos del gate. A mejorar: integración completa de H-01 (coordenada desde el pin embebido), control de mapa compartido. Siguen los P2. Generada por AG-07 |
