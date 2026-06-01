# Plan de Iteración — Sprint 07

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-07_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2026-09-01
**Fecha fin:** 2026-09-12
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 16,7 SP. Siguiendo la acción de la retro del Sprint 06 (combinar trabajo para acercarse al rango estable de 24–28 SP de los sprints de módulo completo), se comprometen 16 SP. El margen se reserva por la incorporación de una dependencia externa (AWS SDK) y por tocar el contrato del empaquetado, ya consolidado en el Sprint 06.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 120 | 0,72 | 86 h |
| Dev frontend | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Integrar la librería de alojamiento de archivos de fotos con backends configurables por el usuario raíz (local y AWS S3), de modo que la base persista solo la referencia y el binario viva en el backend activo, y completar la exportación/importación para que el archivo ZIP transporte los binarios de las fotos y los restaure al importar, dejando el archivo realmente autocontenido.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-20 | Backlog técnico | Librería de alojamiento `GeoVial.FileHosting` con backends configurables (local / S3) | Alta (Must) | 8 | Dev backend A | Pendiente |
| BT-21 (cierre) | Backlog técnico | Binarios de fotos en el ZIP de exportación/importación y subida de contenido de foto | Alta (Must) | 8 | Dev backend B / Dev frontend | Pendiente |

Total de puntos comprometidos: 16 SP. BT-20 materializa ADR-08 (extensibilidad); el cierre de BT-21 completa la parte de binarios que el Sprint 06 dejó explícitamente diferida (manifiesto sin binarios). Ambos cierran la necesidad de resguardo/traspaso con fotos (NB-04) y el alojamiento configurable (PROJECT-README §7).

## 4. Alcance técnico

Componentes que se construyen o modifican (sobre la arquitectura de 05, sin redefinirla):

1. BT-20 — Librería de alojamiento (ADR-08, extensibilidad): se construye `GeoVial.FileHosting` con la abstracción del backend de almacenamiento de fotos (`IAlmacenFotos`: Guardar/Recuperar/Eliminar/Existe) y la implementación local sobre el sistema de archivos. La implementación AWS S3 vive en `GeoVial.Infrastructure` (que integra el alojamiento, ADR-08 §7) sobre el SDK de AWS, inyectada con un cliente `IAmazonS3` testeable. El usuario raíz selecciona el backend activo por configuración (`Almacen:Backend` = `Local` | `S3`); la base persiste solo la referencia (`Foto.ReferenciaArchivo`, modelo lógico §1.7), nunca el binario. Por defecto, sin configuración, se usa el backend local.
2. BT-21 (cierre) — Binarios en el ZIP y subida de contenido (CU-04, CU-08): se agrega la subida del binario de una foto (`POST /api/v1/fotos/{id}/contenido`, multipart), que persiste el binario en el backend activo y asienta la referencia devuelta en la foto, autorizado por área (RN-01), respetando el solo-lectura (RN-05) y auditado (RN-07). La exportación recupera los binarios del backend y los incluye en el ZIP (`fotos/<referencia>`); la importación restaura los binarios en el backend activo y remapea las referencias. El contrato del empaquetado (`IEmpaquetadorRelevamiento`) pasa a transportar manifiesto + binarios.

El contrato del backend es estable y agnóstico del proveedor (extensibilidad §3): la referencia es opaca, la base solo la persiste, y los errores de almacenamiento se reportan de forma tipada para traducirlos a Problem Details (ADR-11). El pipeline de imágenes (compresión/redimensión, BT-19) y el carrusel a pantalla completa (US-24) quedan fuera de este sprint.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- La misma operación de subida funciona contra el backend local y contra S3 (con cliente S3 testeable); cambiar el backend activo no toca el dominio ni el modelo de datos.
- La base persiste solo la referencia de la foto; el binario vive en el backend activo.
- La exportación incluye los binarios de las fotos en el ZIP y la importación los restaura; el ciclo exportar → importar conserva los binarios (round-trip verificado).
- La subida de contenido respeta el acotamiento por área (RN-01) y el solo-lectura del relevamiento cerrado (RN-05), y se audita (RN-07).

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Acoplar el dominio o la aplicación al SDK de AWS | Media | Alto | La abstracción `IAlmacenFotos` vive en `GeoVial.FileHosting` sin dependencias de proveedor; el SDK de AWS solo entra en `GeoVial.Infrastructure`; la aplicación depende del contrato, no del backend |
| No poder probar S3 sin una cuenta/servicio | Alta | Medio | `AlmacenS3` recibe `IAmazonS3` por inyección; las pruebas usan un cliente S3 falso que verifica las llamadas de subida/recuperación sin red |
| Tocar el contrato del empaquetado (consolidado en S06) puede regresionar el round-trip | Media | Alto | Extender `IEmpaquetadorRelevamiento` para transportar binarios manteniendo el manifiesto; pruebas de round-trip que verifican manifiesto y binarios juntos |
| Archivos grandes en memoria al exportar/importar | Baja | Medio | Límite de tamaño en la subida; alcance de fotos por relevamiento acotado; el pipeline de compresión (BT-19) llega en un sprint posterior |

## 7. Criterios de hecho del sprint

El Sprint 07 se considera completo cuando BT-20 está terminada según la DoD (abstracción + backend local + backend S3 testeable + selección por configuración) y el cierre de BT-21 incluye los binarios en el ZIP con su round-trip verde; el ciclo subir contenido → exportar → importar queda demostrado de punta a punta en el sprint review; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| BT que avanzan | BT-20 (librería de alojamiento), BT-21 (cierre de binarios en el ZIP) |
| CU que avanzan | CU-04 (guardar foto), CU-08 (exportar/importar con fotos) |
| NB que avanzan | NB-04 (resguardo y traspaso del relevamiento completo, con fotos) |
| ADRs que gobiernan | ADR-08 (alojamiento con backends configurables), ADR-10 (capas), ADR-11 (Problem Details), ADR-02 (API REST) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 07 (alojamiento de fotos con backends configurables y binarios en el ZIP). Compromete BT-20 y el cierre de BT-21 (16 SP). Materializa ADR-08 y deja el archivo de exportación autocontenido; el pipeline de imágenes (BT-19) queda fuera. Generado por AG-07 |
