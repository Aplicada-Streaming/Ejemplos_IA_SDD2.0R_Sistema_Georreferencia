# Plan de Iteración — Sprint 03

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-03_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha inicio:** 2026-07-07
**Fecha fin:** 2026-07-18
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci); no se re-estima en el sprint.
- Capacidad: el promedio móvil de 3 sprints es 29,3 SP (velocidad-equipo §1). El tope por la regla del 110 % es 32 SP. Se comprometen 28 SP, dentro del tope. BT-05 y BT-06 son los componentes técnicos que materializan las US del slice y no se cuentan como puntos adicionales: su trabajo es el backend de US-11 a US-14.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 2 | 120 | 0,72 | 86 h |
| Dev frontend | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Entregar la captura georreferenciada de observaciones, de modo que una observación tome su coordenada de los metadatos de la foto como fuente primaria, se agrupe en un marcador del relevamiento cuando cae dentro del radio configurado y, si la foto no trae metadatos, se ubique manualmente o quede en la bandeja sin georreferenciar, todo sobre relevamientos en recolección, verificado por rol y área y auditado.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-05 | Backlog técnico | Módulo de observaciones y marcadores (CQRS) | Alta | (componente) | Dev backend A | Pendiente |
| BT-06 | Backlog técnico | Módulo de georreferenciación y prioridad de metadatos | Alta | (componente) | Dev backend A | Pendiente |
| US-11 | Historia | Capturar observación con georreferenciación automática | Alta | 8 | Dev backend B | Pendiente |
| US-12 | Historia | Agrupar observaciones en marcador por radio | Alta | 5 | Dev backend B | Pendiente |
| US-13 | Historia | Ubicar manualmente el punto de la observación | Alta | 5 | Dev backend A / Dev frontend | Pendiente |
| US-14 | Historia | Cargar fotos desde la web priorizando metadatos | Alta | 5 | Dev frontend | Pendiente |

Total de puntos comprometidos: 28 SP (US-11..14). BT-05 y BT-06 figuran como los componentes técnicos que estas US construyen; su esfuerzo ya está contenido en la estimación de las US, por lo que no suman puntos.

US-15 (gestión de marcador y carrusel, CU-09) no entra en este sprint: pertenece a la revisión sobre mapa (EP-05, NB-04) y se planifica con esa épica.

## 4. Alcance técnico

Componentes que se construyen, en orden de dependencias (sobre la arquitectura de 05, sin redefinirla):

1. BT-06 — Georreferenciación: un objeto de valor Coordenada con cálculo de distancia (sin tipo espacial nativo, modelo-datos-logico §2) y la prioridad de fuentes (RN-03): metadatos de la foto primero, luego ubicación manual, luego bandeja sin georreferenciar. Depende de BT-07.
2. BT-05 — Observaciones y marcadores con CQRS ligero (ADR-01): commands de captura y de ubicación manual y queries de listado, que crean o asocian el marcador por radio (RN-02), asientan la observación y su foto, y aplican el bloqueo de solo lectura (RN-05). Depende de BT-06 y de la persistencia de relevamientos (BT-10/BT-03 del Sprint 02).
3. US-11 y US-12 — Captura de la observación con coordenada de metadatos (RN-03) y agrupación por radio en marcador existente o nuevo (RN-02, CU-04 CA-01/CA-02).
4. US-13 — Ubicación manual del punto cuando la foto no trae metadatos; si los trae, `FUENTE_UBICACION_INCORRECTA` (RN-03, CU-05 CA-02); si no se ubica, bandeja sin georreferenciar (CU-05 CA-03).
5. US-14 — Carga de fotos desde la web priorizando metadatos (CU-05 §5.A), reutilizando la misma resolución de fuentes.

El cálculo de distancia para el radio se hace en la capa de dominio/aplicación (modelo-datos-logico §2). El alojamiento binario de las fotos (ADR-08, librería de archivos) no entra en este sprint: la foto persiste su referencia de archivo, no el binario.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Este plan no la redefine.

Criterios específicos del Sprint 03:

- La coordenada se deriva de los metadatos como fuente primaria (RN-03); la ubicación manual solo aplica cuando faltan.
- La agrupación por radio (RN-02) no unifica ni descarta marcadores de forma automática.
- La captura sobre un relevamiento cerrado se rechaza (RN-05); el agente debe estar asignado y en el área (RN-01).
- El slice queda demostrable end-to-end (capturar con metadatos → marcador; capturar sin metadatos → bandeja → ubicar manual) en el sprint review.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El cálculo de distancia geográfica sin tipo espacial puede introducir imprecisión en la agrupación por radio | Media | Medio | Fórmula de haversine encapsulada en el objeto de valor Coordenada con pruebas unitarias de casos límite (dentro y fuera del radio) |
| La resolución de fuentes (metadatos vs manual) puede dejar huecos si no se cubren todos los caminos de RN-03 | Media | Alto | Batería de pruebas de los tres caminos (metadatos, manual, bandeja) antes de exponer la API |
| El agregado de captura puede crecer si se incorpora la gestión de fotos/comentarios/etiquetas (US-15) | Media | Medio | US-15 queda fuera de este sprint; el slice se acota a la georreferenciación y la agrupación |

## 7. Criterios de hecho del sprint

El Sprint 03 se considera completo cuando todas las US comprometidas están terminadas según la DoD con sus pruebas verdes; la captura georreferenciada (con metadatos, sin metadatos con ubicación manual y bandeja sin georreferenciar) queda demostrada end-to-end en el sprint review; y se facilitan el sprint review y la retrospectiva con sus artefactos completados.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que avanzan | CU-04 (captura con georreferenciación automática y agrupación por radio: US-11, US-12), CU-05 (ubicación manual y prioridad de metadatos: US-13, US-14) |
| NB que avanzan | NB-02 (georreferenciación automática y confiable de las observaciones) |
| ADRs que gobiernan | ADR-04 (mapas OSM + Leaflet), ADR-01 (CQRS ligero), ADR-09 (persistencia EF Core), ADR-10 (separación de capas) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 03 (captura y georreferenciación). Compromete US-11, US-12, US-13 y US-14 (28 SP), con BT-05 y BT-06 como componentes técnicos. US-15 diferida a la épica de revisión. Generado por AG-07 |
