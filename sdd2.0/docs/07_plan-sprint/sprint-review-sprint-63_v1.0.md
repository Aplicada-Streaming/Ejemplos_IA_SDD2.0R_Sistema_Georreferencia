# Sprint Review — Sprint 63

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-63_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-07
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-63_v1.0.md`:

> Saldar los P2 de la auditoría UX móvil […]: H-10 (cargar al entrar), H-12 (logout al overflow), H-13 (contraste de placeholders), H-11 (disparador de foto prominente).

Veredicto: Cumplido.

Explicación corta: se cerró el **bundle de P2** de la auditoría UX, con lo que el backlog de la auditoría (P1 + P2) queda **saldado**. *Revisión* ahora **carga al entrar** a la solapa (sin tocar "Cargar revisión"; *Mapa* y *Bandeja* ya lo hacían); "Cerrar sesión" pasó al **overflow** del toolbar (acción destructiva, evita el toque accidental); los **placeholders** del login tienen contraste suficiente (WCAG 1.4.3); y el **disparador de foto** ("📷 Tomar foto") es la acción primaria destacada (Ley de Fitts), con "Elegir foto de la galería" como secundaria. Todo es presentación/comportamiento de UI, sin tocar dominio/datos/sync.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| H-10 | Comportamiento | Entrar a *Revisión* muestra los datos sin tocar "Cargar revisión" (verificado on-device: "Marcador 1/2" al entrar) | Menos fricción |
| H-12 | Prevención de errores | "Cerrar sesión" en el overflow (3 puntos), no en la barra | Menos toques accidentales |
| H-13 | Accesibilidad | Placeholders del login con contraste suficiente | WCAG 1.4.3 |
| H-11 | Ergonomía | "📷 Tomar foto" grande y primario; galería secundaria | Disparador acorde a la Ley de Fitts |

## 3. Feedback recibido

- Cargar al entrar cierra una fricción reiterada (Doherty): el agente no tiene que descubrir un botón "Cargar".
- Mover una acción destructiva (logout) al overflow es prevención de errores estándar.
- El disparador de foto, ahora primario, refuerza la jerarquía de la pantalla de captura (una acción primaria).

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 |
| Puntos completados | 5 |
| Velocity efectiva | 5 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **489** (445 unitarias + 44 de integración), **sin nuevas**: es UI de pulido sin núcleo nuevo (como S56/S62). La suite del gate sigue verde y la cobertura sin regresión. El MAUI compila (`net10.0-android`, arm64) y se redeployó al moto g42; verificación on-device de H-10 (Revisión auto-carga) confirmada; H-11/H-12/H-13 son propiedades declarativas aplicadas por construcción.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| P2-PULIDO-UX | Tarea | Aceptada (H-10, H-12, H-13, H-11) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

**Backlog de la auditoría UX: saldado.** Los 4 P1 (S59-S62) y los P2 (S63) están cerrados. Quedan, como evolución (no hallazgos de la auditoría): fijar la coordenada de la captura desde el pin del mapa embebido (evolución de H-01); un control de mapa compartido (deuda técnica reiterada); H-14 (anuncios por región en vivo) y H-07 (truncado de etiquetas de pestañas), que requieren más trabajo y se dejan para una pasada de accesibilidad/UI dedicada.

## 7. Decisiones tomadas

- *Revisión* carga **una vez** al entrar (no recarga en cada reaparición) para no pisar ediciones; el botón "Cargar revisión" se conserva para refresco manual.
- "Cerrar sesión" al overflow vía `Order=Secondary` (no se elimina; sigue accesible).
- H-14 (live regions) y H-07 (truncado de tabs) se difieren a una pasada de accesibilidad/UI dedicada (no son cambios de una línea).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-07 | Review del Sprint 63 (P2 de la auditoría UX: H-10/H-12/H-13/H-11). Cumplido, velocity 5, 0 carry-over, 489 pruebas (sin nuevas; UI de pulido). Salda el backlog de la auditoría UX (P1 + P2). Generado por AG-07 |
