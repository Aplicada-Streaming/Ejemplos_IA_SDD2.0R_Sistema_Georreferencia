# Matriz de cobertura de pruebas — GeoVial

**Proyecto:** GeoVial
**Documento:** matriz-cobertura-pruebas_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Ingeniero QA / SDET Senior (AG-08), Equipo SDD 2.0

## 1. Propósito y alcance

Documento bisagra de la categoría 08. Relaciona los catorce CU de 02 con los TC que cubren sus criterios Given-When-Then, los NFR numéricos de 05 con el test y el tooling de medición que validan su SLA, y las ocho RN de 02 con el TC que verifica su cumplimiento. Incluye además la tabla de cobertura por capa y los gaps con plan de remediación. Los TC referenciados están definidos en `casos-prueba-referenciales_v1.0.md`. El estado de los tests es `Pendiente` en toda la matriz porque aún no se ejecutaron (se implementan en los sprints de 07); la matriz se actualiza al cierre de cada sprint.

## 2. Tabla CU↔Tests

Cada CU con sus criterios Given-When-Then (resumidos del campo §8 de cada CU) y el TC que los cubre.

| CU | Criterio Given-When-Then | Test ID | Tipo | Estado |
| --- | --- | --- | --- | --- |
| CU-01 | Given jefe con dos agentes de su área, When crea relevamiento radio 15 m y asigna ambos, Then crea en recolección y registra dos asignaciones | TC-01 | Integration | Pendiente |
| CU-01 | Given jefe Zona Norte, When asigna agente de Zona Sur, Then `AGENTE_FUERA_DE_AREA` | TC-02 | Unit | Pendiente |
| CU-01 | Given relevamiento cerrado, When asigna agente, Then `RELEVAMIENTO_SOLO_LECTURA` | TC-03 | Unit | Pendiente |
| CU-02 | Given agente con dos relevamientos y método de seguridad, When inicia sesión con conexión, Then lista relevamientos y habilita offline | TC-04 | Integration | Pendiente |
| CU-02 | Given agente sin método de seguridad, When habilita offline / reingresa sin señal, Then `OFFLINE_NO_HABILITADO` / `REINGRESO_SIN_METODO_SEGURIDAD` | TC-05 | Unit | Pendiente |
| CU-03 | Given jefe general / jefe de área / área inexistente, When da de alta, Then alta auditada / `ACCESO_NO_AUTORIZADO` / `AREA_INEXISTENTE` | TC-06 | Integration | Pendiente |
| CU-04 | Given relevamiento radio 15 m sin/ con marcador, When toma foto con metadatos, Then crea o asocia marcador | TC-07 | Integration | Pendiente |
| CU-04 | Given foto sin metadatos, When registra, Then `OBSERVACION_SIN_GEORREFERENCIA` y deriva a manual | TC-08 | Unit | Pendiente |
| CU-05 | Given foto sin metadatos / con metadatos, When ubica manualmente / no ubica, Then asocia por manual / `FUENTE_UBICACION_INCORRECTA` / bandeja sin georreferenciar | TC-08, TC-22 | Unit | Pendiente |
| CU-06 | Given agente offline habilitado, When captura y completa jornada, Then guarda local, encola y conserva 100 sin pérdida | TC-09 | E2E | Pendiente |
| CU-07 | Given cola de 100 y conexión, When sincroniza, Then sube 100, baja actualizaciones y vacía cola | TC-10 | Integration | Pendiente |
| CU-07 | Given dos ediciones del mismo comentario, When consolida, Then prevalece la más reciente y marca conflicto | TC-12 | Unit | Pendiente |
| CU-07 | Given subida interrumpida, When reanuda, Then completa sin duplicar | TC-11, TC-13 | Integration | Pendiente |
| CU-08 | Given relevamiento en revisión con cinco marcadores, When jefe del área lo abre / otro área lo abre, Then muestra mapa+bandeja / `ACCESO_NO_AUTORIZADO` | TC-14 | Integration | Pendiente |
| CU-08 | Given relevamiento, When exporta, Then único archivo comprimido completo y auditado | TC-15 | Integration | Pendiente |
| CU-09 | Given marcador con fotos en recolección/cerrado, When agrega/recorre/quita, Then persiste y carrusel / navega / `RELEVAMIENTO_SOLO_LECTURA` | TC-16 | Component | Pendiente |
| CU-10 | Given relevamiento en recolección/cerrado, When transiciona/reabre, Then revisión auditada / `TRANSICION_INVALIDA` / reapertura levanta solo lectura | TC-17 | Unit | Pendiente |
| CU-11 | Given marcadores a 9/30/20 m con radio 15/25 m, When consulta conflictos, Then lista o no según radio sin unificar | TC-18 | Unit | Pendiente |
| CU-12 | Given dos marcadores en conflicto / en relevamiento cerrado, When unifica/separa/resuelve, Then fusiona o conserva y levanta marca / `RELEVAMIENTO_SOLO_LECTURA` | TC-19 | Integration | Pendiente |
| CU-13 | Given alta / registro de 3 meses / consulta del último año, When ejecuta/modifica/consulta, Then asienta inmutable / `AUDITORIA_INMUTABLE` / devuelve accesos ≥ 12 meses | TC-20 | Integration | Pendiente |
| CU-14 | Given jefe/agente, When accede a otra área / dato personal ajeno / su área, Then `ACCESO_NO_AUTORIZADO` / `ACCESO_DATO_PERSONAL_NO_AUTORIZADO` / concede | TC-21 | Unit | Pendiente |

Los catorce CU tienen al menos un TC que cubre sus criterios Given-When-Then. No hay CU huérfano de test.

## 3. Tabla NFR↔Tests

Cada NFR numérico de 05 §8 con su SLA, el test que lo valida y el tooling de medición. La disponibilidad se observa como SLO en 09, no como test unitario (regla §4.9, ejemplo de la categoría).

| NFR (05 §8) | SLA | Test | Tooling de medición |
| --- | --- | --- | --- |
| Operación sin conexión | ≥ 1 jornada (8 h) sin pérdida | TC-09 | .NET MAUI UI testing / Appium sobre Android (captura continua 8 h sobre SQLite local) |
| Tiempo de sincronización | ≤ 5 min para ≈100 observaciones con fotos | TC-10 | Telemetría de sync en test de integración (tiempo disparo → vaciado de cola) |
| Latencia API (lecturas administrativas) | p95 ≤ 500 ms | TC-24 | WebApplicationFactory + dataset sintético, medición de p95 |
| Detección de conectividad | Automática; sync se dispara sola al recuperar señal | TC-23 | UI móvil que corta y restablece red y verifica disparo automático |
| Disponibilidad del backend | SLO 99% en horario laboral | Métrica observada en 09 (health check + error budget) | Monitoreo de disponibilidad en 09; no es test unitario |
| Confiabilidad de georreferenciación | ≥ 95% de observaciones con coordenada válida automática | TC-07, TC-08 (más métrica de campo) | Verificación de fuente de coordenada = metadatos en captura; métrica observada en 09 |
| Tamaño de foto sincronizada | Compresión/redimensión para acotar el payload (límite a validar) | TC-10 (payload antes/después en pipeline de captura) | Medición de payload por foto en el pipeline de sync |

Cada NFR con objetivo numérico tiene un test asociado en la matriz; la disponibilidad y el ≥ 95% de georreferenciación se complementan con observación de SLO/métrica en 09 por su naturaleza estadística en producción.

## 4. Tabla RN↔Tests

Cada RN de 02 con el TC que verifica su cumplimiento.

| RN | Enunciado (resumen) | TC | Tipo |
| --- | --- | --- | --- |
| RN-01 | Jerarquía y autorización por rol y área | TC-21, TC-02, TC-06 | Unit / Integration |
| RN-02 | Identidad de marcador y radio de agrupación, sin unificación automática | TC-18, TC-07 | Unit |
| RN-03 | Prioridad de los metadatos de ubicación (EXIF → manual → sin georreferenciar) | TC-08, TC-22 | Unit |
| RN-04 | Última escritura prevalece con marca de conflicto | TC-12, TC-13 | Unit / Integration |
| RN-05 | Estados del relevamiento y solo lectura tras el cierre | TC-17, TC-03 | Unit |
| RN-06 | Método de seguridad del teléfono como precondición del modo offline | TC-05, TC-09 | Unit / E2E |
| RN-07 | Retención del registro de auditoría ≥ 12 meses, inmutable | TC-20 | Integration |
| RN-08 | Tratamiento de datos personales bajo Ley 25.326 (acceso acotado y registrado) | TC-21, TC-20 | Unit / Integration |

Las ocho RN tienen al menos un TC que verifica su cumplimiento. No hay RN huérfana de test.

## 5. Tabla de cobertura por capa

Umbrales mínimos de estrategia-testing §2 (`web-monolith`, §2.2 de la regla). Los valores observados se completan al ejecutar la suite en CI; en esta versión inicial figuran como `Pendiente` porque la suite aún no corrió.

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Dominio (entidades, reglas, estados) | Pendiente | Pendiente | — (no exigido v1) | 85 / 75 / — |
| Aplicación (handlers CQRS, servicios, autorización) | Pendiente | Pendiente | — | 80 / 70 / — |
| Infraestructura (EF Core, FileHosting, sync) | Pendiente | Pendiente | — | 70 / 60 / — |
| Presentación (Web Blazor, API) | Pendiente | Pendiente | — | 60 / 50 / — |

Gate global de CI bloqueante (sobre el agregado de capas con lógica): líneas ≥ 80%, branches ≥ 70% (PROJECT-README §9, §11). La cobertura se reporta por capa, no como número global único (anti-patrón regla §4.10).

## 6. Gaps identificados

| Gap | Estado | Plan de remediación |
| --- | --- | --- |
| Cobertura por capa sin valores observados | Esperado en v1.0 | Se completa al ejecutar la suite en CI; primera lectura al cierre del Sprint 00 (gates operativos) y consolidación en Sprint 01 |
| Límite numérico de tamaño de foto sincronizada no fijado | Supuesto a validar (README §13) | TC-10 mide el payload antes/después; el umbral concreto se fija cuando el cliente confirme el límite; entretanto se valida el mecanismo de compresión/redimensión |
| NFR de disponibilidad (SLO 99%) no es test unitario | Por diseño | Se observa como SLO en 09 (health check + error budget mensual); no se fuerza como test en 08 |
| Confiabilidad de georreferenciación ≥ 95% es métrica estadística de campo | Parcial en 08 | TC-07/TC-08 verifican la lógica de fuente de coordenada; el porcentaje agregado se observa como métrica en 09 con revisión mensual |
| UI móvil (TC-09, TC-23) depende de dispositivo Android por USB | Esperado (README §16) | Se ejecutan en banco con Android conectado; se mantienen acotados al 10% de la pirámide |
| Mutation testing no exigido en v1 para web-monolith | Aceptado por regla | Mejora opcional sobre el dominio; se evaluaría con ADR si se decide convertir en gate |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Matriz inicial con las tres tablas obligatorias (CU↔Tests para los 14 CU, NFR↔Tests para los NFR numéricos de 05 §8, RN↔Tests para las 8 RN), tabla de cobertura por capa y gaps con plan de remediación. Estado de tests `Pendiente` (suite no ejecutada). Generada por AG-08 |
