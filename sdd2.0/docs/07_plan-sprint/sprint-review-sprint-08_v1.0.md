# Sprint Review — Sprint 08

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-08_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-08_v1.0.md`:

> Acotar el tamaño de las fotos comprimiéndolas y redimensionándolas al subirlas (sin perder la georreferenciación, que ya vive en el dominio), y cerrar la revisión sobre el mapa permitiendo al jefe de área filtrar las observaciones por etiquetas y abrir una foto a pantalla completa con zoom, todo sobre el alojamiento de fotos ya integrado.

Veredicto: Cumplido.

Explicación corta: la subida de una foto pasa por el pipeline de imágenes (redimensión hacia abajo a un máximo configurable + recodificación JPEG) antes de alojarse, acotando el payload sin afectar la georreferenciación (que ya está asentada en el dominio). La revisión acepta un filtro por etiquetas que muestra solo las observaciones coincidentes y lo indica cuando no hay coincidencias; y una foto se abre a pantalla completa con zoom y al cerrar vuelve al carrusel, con la descarga del binario autorizada por área (RN-01). Con esto queda cerrada la épica de revisión sobre mapa (EP-05).

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| BT-19 | Backlog técnico | Subir una foto grande: el binario alojado queda redimensionado (≤ 1920 px) y recomprimido | Payload acotado, orientación correcta |
| US-23 | Historia | Filtrar la revisión por "fisura": solo aparecen las observaciones con esa etiqueta; filtro sin coincidencias avisa | Filtrado claro y útil |
| US-24 | Historia | Abrir una foto a pantalla completa, acercar/alejar con zoom y cerrar volviendo al carrusel | Inspección visual cómoda |
| US-24 | Historia | La descarga del binario de la foto la autoriza el área (RN-01) | Acotamiento respetado |

## 3. Feedback recibido

- Con el pipeline de imágenes y el cierre de EP-05, la revisión sobre mapa queda completa (marcadores, carrusel, etiquetas, filtrado, visor) y las fotos viajan acotadas.
- Próximos focos sugeridos: la sincronización del cliente móvil (EP-04) y la consulta del historial de auditoría con retención (US-30, EP-08).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 11 |
| Puntos completados | 11 |
| Velocity efectiva | 11 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 205 verdes (182 unitarias + 23 de integración), +13 respecto del Sprint 07. Cobertura: dominio 89,9 % líneas / 80,4 % branches; aplicación 89,3 % / 81,4 %; `GeoVial.FileHosting` 100 % / 100 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-19 | Backlog técnico | Aceptada |
| US-23 | Historia | Aceptada |
| US-24 | Historia | Aceptada (capacidad funcional de ampliar y hacer zoom; el detalle fino de gestos pertenece a 03) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 08 se traslada. |

EP-05 (revisión sobre mapa) queda cerrada por completo (US-21/22/23/24). El pipeline de imágenes del cliente móvil y la sincronización (EP-04: US-16/17/18/19) y la consulta de auditoría (EP-08: US-30) continúan en el backlog.

## 7. Decisiones tomadas durante el review

- Dar por cerrada la épica EP-05 (revisión sobre mapa).
- Fijar la librería de imágenes en SkiaSharp (MIT) tras descartar ImageSharp por vulnerabilidad conocida en la línea de licencia permisiva.
- Priorizar EP-04 (sincronización) y US-30 (consulta de auditoría) para los próximos sprints.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 08 (pipeline de imágenes y cierre de EP-05: filtrado y visor). Veredicto Cumplido, velocity 11, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
