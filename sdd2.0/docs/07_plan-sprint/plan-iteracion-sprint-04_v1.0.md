# Plan de Iteración — Sprint 04

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-04_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha inicio:** 2026-07-21
**Fecha fin:** 2026-08-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (2 dev backend, 1 dev frontend, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 31,7 SP; tope del 110 % = 35 SP. Se comprometen 24 SP.

## 2. Objetivo del sprint

Cerrar el ciclo de revisión: que un jefe de área revise un relevamiento sobre el mapa recorriendo sus marcadores con sus observaciones, fotos, comentarios y etiquetas, pueda comentar y etiquetar el contenido del marcador respetando el bloqueo de solo lectura, y que el equipo disponga de la provisión de credenciales para operar la jerarquía completa por la interfaz.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-23 | Backlog técnico | Provisión de credenciales de acceso de un usuario | Alta | 3 | Dev backend A | Pendiente |
| US-21 | Historia | Revisar el relevamiento sobre el mapa por marcadores | Alta | 8 | Dev backend B | Pendiente |
| US-15 | Historia | Gestionar comentarios y etiquetas del marcador | Alta | 8 | Dev backend A | Pendiente |
| US-22 | Historia | Recorrer el carrusel y navegar entre marcadores | Alta | 5 | Dev frontend | Pendiente |

Total de puntos comprometidos: 24 SP.

BT-23 es una tarea emergente de la acción recurrente de las retrospectivas (S01–S03): sin provisión de credenciales no se puede demostrar por interfaz a los roles distintos del usuario raíz. Se incorpora al backlog técnico de 06 (EP-01) como soporte de acceso.

US-15 se acota en este sprint a la gestión de comentarios y etiquetas del contenido del marcador (las fotos se incorporan por captura, Sprint 03). La exportación e importación del relevamiento (CU-08 §5.A/§5.B, EP-07) y el visor a pantalla completa (US-24) quedan fuera de este sprint.

## 4. Alcance técnico

Componentes que se construyen o modifican (sobre la arquitectura de 05, sin redefinirla):

1. BT-23 — Provisión de credenciales: establecer la credencial (nombre de usuario + clave con hash) de un usuario dado de alta, autorizada por quien lo administra (RN-01), con la clave hasheada con PBKDF2 (ADR-03). Habilita el inicio de sesión de jefes de área y agentes.
2. US-15 — Comentario y Etiqueta con sus cardinalidades (RC-04): un comentario pertenece a un marcador y se liga a cero o una foto; fotos y comentarios admiten varias etiquetas (muchos a muchos). Bloqueo de solo lectura tras el cierre (RN-05). Módulo de marcadores con CQRS ligero (ADR-01).
3. US-21 — Revisión sobre mapa (CU-08): query que devuelve los marcadores del relevamiento con sus observaciones, fotos, comentarios y etiquetas, y la bandeja sin georreferenciar, acotada por rol y área (RN-01). Solo lectura sobre datos consolidados.
4. US-22 — Carrusel y navegación (CU-09): el front recorre las fotos de un marcador y navega entre marcadores a partir de la vista consolidada de US-21.

La exportación/importación en archivo comprimido (CU-08 §5.A/§5.B) no entra en este sprint.

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- La cardinalidad foto/comentario/etiqueta (RC-04) se respeta: comentario ligado a 0..1 foto; etiquetas muchos a muchos.
- La revisión es de solo lectura sobre datos consolidados; comentar o etiquetar un relevamiento cerrado se rechaza (RN-05).
- La provisión de credenciales hashea la clave (nunca en claro) y la autoriza por jerarquía (RN-01).
- El ciclo capturar → revisar sobre mapa → comentar/etiquetar queda demostrable end-to-end en el sprint review, con login de un jefe de área provisto de credenciales.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El modelo muchos a muchos de etiquetas (RC-04) puede complicar el mapeo y las consultas de revisión | Media | Medio | Entidades de unión explícitas (FotoEtiqueta, ComentarioEtiqueta); pruebas de la query de revisión con marcador, fotos, comentarios y etiquetas |
| La provisión de credenciales toca la seguridad de acceso; un error abre o cierra el ingreso indebidamente | Media | Alto | Autorización por jerarquía reutilizando la regla de administración (BT-02); pruebas de alta de credencial y de login posterior |
| La query de revisión puede traer cargas grandes si arrastra todo el árbol del relevamiento | Baja | Medio | Acotar la vista al relevamiento y sus marcadores; sin lazy loading; proyección a DTO en la capa API |

## 7. Criterios de hecho del sprint

El Sprint 04 se considera completo cuando todas las US y BT comprometidas están terminadas según la DoD con sus pruebas verdes; la revisión sobre mapa con comentarios y etiquetas y el login de un jefe de área provisto de credenciales quedan demostrados end-to-end en el sprint review; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que avanzan | CU-08 (revisión sobre mapa por marcadores: US-21), CU-09 (gestión de comentarios/etiquetas y carrusel: US-15, US-22), CU-03 (provisión de credenciales: BT-23) |
| NB que avanzan | NB-04 (revisión centralizada sobre mapa), NB-01 (operación de la jerarquía con acceso completo) |
| ADRs que gobiernan | ADR-04 (mapas OSM + Leaflet), ADR-01 (CQRS ligero), ADR-03 (ROPC/JWT), ADR-09 (persistencia EF Core), ADR-14 (compliance) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Plan inicial del Sprint 04 (revisión sobre mapa, comentarios/etiquetas y provisión de credenciales). Compromete US-21, US-15 (acotada), US-22 y BT-23 (24 SP). Exportación/importación diferida a EP-07. Generado por AG-07 |
