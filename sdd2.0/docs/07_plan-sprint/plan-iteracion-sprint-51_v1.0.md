# Plan de Iteración — Sprint 51

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-51_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-05-15
**Fecha fin:** 2028-05-26
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,0 SP (S48–S50); capacidad sugerida estricta 9 SP. Se compromete **ubicar una observación de la bandeja desde la app** (8 SP), la segunda mitad del trabajo de S50 (CU-05). Núcleo (puente WebView + HTML del mapa + cliente) en el gate.

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

En S50 el agente pasó a **ver** la bandeja sin georreferenciar, pero la ubicación seguía siendo sólo por la web. El objetivo es **cerrar el flujo en la app**: tocar una observación de la bandeja, elegir el punto sobre el mapa y ubicarla (CU-05), sin salir del teléfono. El backend ya soporta la ubicación manual (`POST /observaciones/{id}/ubicacion`); el trabajo es el **puente WebView↔app** (toque en el mapa → coordenada → backend), encapsulado en un núcleo testeable.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| US-BANDEJA-UBICAR | Historia | Ubicar una observación de la bandeja tocando el mapa, desde la app (CU-05) | Alta | 4 | Dev móvil (AG-08) | Cerrada |
| BT-PUENTE-MAPA | Tarea | Núcleo del puente: `MapaUbicacionHtml` (Leaflet + toque + esquema centinela) + `ParseadorMensajeUbicacion` (extrae la coordenada), en el gate | Alta | 3 | Dev fullstack (AG-09) | Cerrada |
| BT-UBICAR-CLIENTE | Tarea | `ClienteUbicacionManual` (POST de la coordenada), cubierto en el gate | Media | 1 | Dev móvil (AG-08) | Cerrada |

Total de puntos comprometidos: 8 SP. Núcleo en `GeoVial.Revision` (en el gate); UI en `GeoVial.Mobile`.

## 4. Alcance técnico

1. **Puente WebView → app (en el gate):** el HTML del mapa (`MapaUbicacionHtml`, Leaflet + OSM) escucha el toque, coloca/mueve un marcador y, al confirmar, **navega a un esquema centinela** `geovial-ubicar://place?lat=..&lon=..`. `ParseadorMensajeUbicacion.Intentar(url)` reconoce ese mensaje y extrae la coordenada (punto decimal invariante), validando el rango geográfico; devuelve `null` para cualquier otra navegación. Ambos puros y testeables, sin dependencia del WebView.
2. **Cliente (en el gate):** `ClienteUbicacionManual.UbicarAsync(observacionId, lat, lon)` postea al endpoint de ubicación manual y devuelve si el backend la aceptó.
3. **UI:** `MapaUbicacionPage` (modal) carga el HTML, **intercepta** la navegación al esquema centinela (`WebView.Navigating` → `e.Cancel`), parsea la coordenada, llama al cliente y cierra devolviendo el resultado. `BandejaPage`: al tocar una fila, abre la modal centrada en los marcadores existentes; si se ubica, recarga la bandeja.

## 5. Definition of Done aplicada

- El agente ubica una observación de la bandeja tocando el mapa, desde la app; al volver, la bandeja se actualiza (la observación ya no aparece).
- Núcleo (puente + parser + cliente) cubierto por pruebas en el gate; el puente cierra el círculo (lo que arma el HTML lo entiende el parser).
- Cobertura DoD respetada (Domain/Application líneas ≥80 %, ramas ≥70 %); `GeoVial.Revision` bien cubierto.
- El MAUI compila para `net10.0-android` con la modal de ubicación.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El puente WebView↔app es frágil/dependiente de plataforma | Alta | Alto | El contrato (esquema centinela + parser) vive en el gate y se testea cerrado; el WebView sólo intercepta y delega |
| Una navegación normal del mapa se confunde con el mensaje | Media | Medio | El parser sólo acepta el esquema `geovial-ubicar` y coordenadas en rango; el resto → `null` (se deja pasar) |
| El doble toque dispara dos POST | Media | Bajo | Guarda `_enviando` en la página: ignora mensajes mientras hay uno en curso |

## 7. Criterios de hecho del sprint

El Sprint 51 se considera completo cuando: el agente ubica desde la app una observación de la bandeja sobre el mapa, el núcleo del puente está cubierto en el gate, la cobertura DoD se mantiene, el MAUI compila, y se facilitan el sprint review y la retrospectiva.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Acción planificada en S50 (separar "ver" de "ubicar en el mapa") |
| CU | CU-05 (ubicación manual); RN-03 (fuente de ubicación) |
| Componentes | `GeoVial.Revision` (`MapaUbicacionHtml`, `ParseadorMensajeUbicacion`, `ClienteUbicacionManual`); `GeoVial.Mobile` (`MapaUbicacionPage`, `BandejaPage`) |
| Calidad | definition-of-done §1.4 (cobertura) |
| Tests previstos | `ParseadorMensajeUbicacionTests` (válido, no-mensaje, fuera de rango) + `MapaUbicacionHtmlTests` (leaflet/osm, centinela, toque) + `ClienteUbicacionManualTests` (POST ok/err) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Plan del Sprint 51 (ubicar desde la app): puente WebView↔app (`MapaUbicacionHtml` + `ParseadorMensajeUbicacion`) + `ClienteUbicacionManual` en el gate + `MapaUbicacionPage`/`BandejaPage`. Cierra el flujo de la bandeja (CU-05) iniciado en S50. Compromete 8 SP. Generado por AG-07 |
