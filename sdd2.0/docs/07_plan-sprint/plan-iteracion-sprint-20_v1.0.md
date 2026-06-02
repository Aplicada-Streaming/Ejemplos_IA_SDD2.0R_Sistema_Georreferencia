# Plan de Iteración — Sprint 20

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-20_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2027-03-09
**Fecha fin:** 2027-03-20
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas (10 días hábiles).
- Equipo: 4 integrantes (1 dev backend, 1 dev móvil, 1 dev fullstack, 1 QA part-time), `equipo_n: 4`.
- Unidad de estimación: story points (Fibonacci).
- Capacidad: promedio móvil de 3 sprints 8,7 SP (S17–S19); capacidad sugerida estricta 10 SP. Se compromete el endurecimiento E2E del MVP (8 SP): pruebas de integración del camino completo por HTTP que cierran la deuda señalada en las retrospectivas de los Sprints 15, 18 y 19. Todo el valor es testeable y vive en el gate (pruebas de integración).

| Rol | Integrantes | Horas disponibles | Factor de focus | Capacidad efectiva |
| --- | --- | --- | --- | --- |
| Dev backend | 1 | 60 | 0,72 | 43 h |
| Dev móvil | 1 | 60 | 0,70 | 42 h |
| Dev fullstack | 1 | 60 | 0,70 | 42 h |
| QA | 1 (part-time) | 30 | 0,65 | 19,5 h |

## 2. Objetivo del sprint

Endurecer el MVP con pruebas de integración del camino completo por HTTP, verificando de punta a punta los flujos centrales del producto sobre la API real (WebApplicationFactory + proveedor en memoria), con un helper que arma el escenario completo (área → agente con credencial → relevamiento asignado) que hasta ahora faltaba.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| BT-E2E | Tarea | Helper de escenario completo + pruebas E2E por HTTP de los flujos centrales | Alta | 8 | QA / Dev backend | Pendiente |

Total de puntos comprometidos: 8 SP. No agrega funcionalidad: consolida la confianza en el MVP cubriendo por HTTP los caminos que hasta ahora solo se verificaban a nivel de handler. Cierra las acciones de retro de los Sprints 15 (helper de escenario de captura), 18 y 19 (E2E del ciclo completo).

## 4. Alcance técnico

1. **Helper de escenario** (`tests/GeoVial.IntegrationTests`): siembra en el proveedor en memoria (vía el `IServiceScope` de la factory) un área, un agente de campo con credencial y un relevamiento asignado al agente, listo para capturar; devuelve los identificadores y las credenciales para autenticarse por HTTP.
2. **Pruebas E2E por HTTP**:
   - **Happy-path de captura**: login del agente → capturar una observación con coordenadas EXIF → la respuesta expone el `FotoId` y el marcador → subir el binario de la foto → descargarlo y verificar que coincide → la revisión muestra el marcador con su foto → comentar el marcador → la revisión refleja el comentario.
   - **Ubicación manual**: capturar sin coordenadas → la observación va a la bandeja sin georreferenciar → ubicar manualmente el punto → la revisión muestra el marcador resultante.
   - **Autorización**: un agente de otra área no puede capturar en el relevamiento (RN-01).

## 5. Definition of Done aplicada

Se aplica la Definition of Done canónica (`08_calidad_y_pruebas/definition-of-done_v1.0.md`). Criterios específicos:

- El helper arma el escenario completo y permite autenticarse como el agente por HTTP.
- El happy-path de captura se verifica de punta a punta por HTTP (captura → `FotoId` → subida → descarga → revisión → comentario).
- La ubicación manual de una observación sin georreferenciar se verifica por HTTP.
- La autorización por área se verifica por HTTP (agente ajeno → rechazado).
- Las pruebas son deterministas (escenario sembrado con identificadores únicos); el gate de cobertura se mantiene.

## 6. Riesgos del sprint y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| El proveedor en memoria se comparte entre pruebas (nombre fijo) | Media | Bajo | Cada escenario usa identificadores únicos; las pruebas consultan por sus propios ids, sin depender del estado global |
| Subir el binario depende del pipeline de imágenes | Baja | Bajo | El pipeline aloja tal cual los binarios que no son imágenes; la prueba sube y descarga bytes arbitrarios y compara |
| El login del agente requiere un método de seguridad | Baja | Bajo | El login con conexión solo verifica la credencial; el método de seguridad es para el reingreso offline (US-05) |

## 7. Criterios de hecho del sprint

El Sprint 20 se considera completo cuando el helper de escenario y las pruebas E2E por HTTP están verdes: el happy-path de captura, la ubicación manual y la autorización se verifican de punta a punta sobre la API; y se facilitan el sprint review y la retrospectiva con sus artefactos.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| US que consolidan | US-11, US-13, US-15, US-21 (verificación E2E de los flujos ya entregados) |
| CU que consolidan | CU-04, CU-05, CU-08, CU-09 |
| NB | NB-02, NB-04 |
| RN aplicadas | RN-01 (autorización), RN-03 (georreferenciación), RN-05 (solo lectura) |
| Calidad | definition-of-done §1 (pruebas E2E), estrategia-testing (pirámide de pruebas) |
| Tests previstos | integration/AT-04, AT-05, AT-08, AT-09 (camino completo por HTTP) |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Plan inicial del Sprint 20 (endurecimiento E2E del MVP): helper de escenario completo + pruebas de integración por HTTP de los flujos centrales (captura, ubicación manual, autorización). Compromete 8 SP. Cierra la deuda de E2E de las retros S15/S18/S19. Generado por AG-07 |
