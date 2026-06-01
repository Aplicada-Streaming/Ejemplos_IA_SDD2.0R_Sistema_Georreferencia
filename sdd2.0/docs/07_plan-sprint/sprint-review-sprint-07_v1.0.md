# Sprint Review — Sprint 07

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-07_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-07_v1.0.md`:

> Integrar la librería de alojamiento de archivos de fotos con backends configurables por el usuario raíz (local y AWS S3), de modo que la base persista solo la referencia y el binario viva en el backend activo, y completar la exportación/importación para que el archivo ZIP transporte los binarios de las fotos y los restaure al importar, dejando el archivo realmente autocontenido.

Veredicto: Cumplido.

Explicación corta: `GeoVial.FileHosting` expone la abstracción del backend (Guardar/Recuperar/Eliminar/Existe) con la implementación local; AWS S3 vive en infraestructura sobre el SDK, con cliente inyectable y testeable; el usuario raíz selecciona el backend por configuración y la base persiste solo la referencia. La subida de contenido persiste el binario y asienta la referencia devuelta, autorizada por área (RN-01), respetando el solo-lectura (RN-05) y auditada (RN-07). La exportación incluye los binarios en el ZIP y la importación los restaura en el backend activo remapeando las referencias, dejando el archivo autocontenido.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| BT-20 | Backlog técnico | La misma subida de foto funciona contra el backend local y contra S3 (cliente S3 testeable); cambiar el backend no toca el dominio | Configurabilidad demostrada |
| BT-20 | Backlog técnico | La base persiste solo la referencia; el binario vive en el backend activo | Desacople correcto |
| BT-21 (cierre) | Backlog técnico | Subir el contenido de una foto, exportar el relevamiento y reimportarlo: el binario viaja en el ZIP y se restaura | Archivo autocontenido |
| BT-21 (cierre) | Backlog técnico | Subir contenido sobre relevamiento cerrado se rechaza (RN-05); un jefe de otra área no sube (RN-01) | Acotamiento respetado |

## 3. Feedback recibido

- Con el archivo de exportación autocontenido (manifiesto + binarios) y el alojamiento configurable, el resguardo y traspaso del relevamiento queda completo end-to-end.
- Próximos focos sugeridos: el pipeline de imágenes (compresión/redimensión, BT-19) para acotar el payload, y la sincronización del cliente móvil (EP-04).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 16 |
| Puntos completados | 16 |
| Velocity efectiva | 16 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 192 verdes (172 unitarias + 20 de integración), +16 respecto del Sprint 06. Cobertura: dominio 89,9 % líneas / 80,4 % branches; aplicación 88,9 % / 80,6 %; `GeoVial.FileHosting` 100 % / 100 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-20 | Backlog técnico | Aceptada |
| BT-21 (cierre) | Backlog técnico | Aceptada (binarios en el ZIP; el componente de exportación/importación queda completo con fotos) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 07 se traslada. |

El pipeline de imágenes (compresión/redimensión, BT-19), el carrusel a pantalla completa (US-24), el filtrado por etiquetas (US-23) y la sincronización del cliente móvil (EP-04) continúan en el backlog. La validación de la subida contra una cuenta S3 real (sample 03-filehosting-backends) queda como verificación de despliegue; en el sprint se cubre con un cliente S3 inyectado y testeable.

## 7. Decisiones tomadas durante el review

- Dar por cerrado el alojamiento configurable (ADR-08) y el archivo de exportación autocontenido.
- Priorizar BT-19 (pipeline de imágenes) y EP-04 (sincronización) para los próximos sprints.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Sprint review del Sprint 07 (alojamiento de fotos con backends configurables y binarios en el ZIP). Veredicto Cumplido, velocity 16, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
