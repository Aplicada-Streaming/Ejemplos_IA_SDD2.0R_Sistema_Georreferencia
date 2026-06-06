# Sprint Review — Sprint 57

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-57_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-57_v1.0.md`:

> El objetivo es que el agente, parado en un marcador del carrusel, pueda agregar una foto —tomándola con la cámara o eligiéndola del catálogo/galería— en la posición de ese marcador, siempre que el relevamiento no esté cerrado (RN-05).

Veredicto: Cumplido.

Explicación corta: en el carrusel de revisión (al que ya se llega tocando el pin del mapa, S55) hay un botón **"📷 Agregar foto al marcador"** que ofrece **cámara o galería** y suma la foto **en la posición del marcador en foco**. La clave de diseño: "agregar foto a un marcador" se modela como **una captura en la coordenada exacta del marcador**, así la agrupación por radio del backend (RN-02) la asocia a ese mismo marcador — **sin tocar dominio, sin endpoints nuevos, sin fotos huérfanas**. El núcleo `ArmadorFotoMarcador` (en el gate) **fuerza la coordenada del marcador y descarta el EXIF**, de modo que una foto del catálogo tomada en otro lugar igual cae en *este* marcador. Reusa la cola de capturas offline (S42) y el patrón de cámara (S55, con el flag para no rebotar al login). El cierre del relevamiento (RN-05) lo aplica el backend de captura existente.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-15-FOTO-MOVIL | Funcionalidad | Mapa → tocar pin → carrusel → "Agregar foto" → cámara → la foto aparece bajo ese marcador | Es lo que faltaba: sumar fotos sobre el marcador en terreno |
| US-15-FOTO-MOVIL | Funcionalidad | Misma vía con **galería**: la foto del catálogo cae en el marcador aunque su EXIF sea de otro lado | Útil para sumar fotos o gráficos adicionales |
| BT-FOTO-MARCADOR-NUCLEO | Núcleo | `ArmadorFotoMarcador` fuerza la coordenada del marcador (test) y respeta el rango | Bien aislado y probado |

## 3. Feedback recibido

- El flujo materializa el pedido del usuario (tocar el marcador y sumarle fotos en esa posición y/o imágenes del dispositivo) **dentro de la spec**: el marcador no se "coloca" en el móvil (eso es función web); en el móvil se enriquece un marcador existente agregando capturas en su coordenada (US-15 vía RN-02).
- Reuso fuerte: cero backend, cero dominio. Se aprovechó la tubería de captura offline (S42) y el patrón de cámara (S55).
- La spec **no distingue** "foto de cámara" de "imagen de galería" (la entidad es una sola, `Foto`): por eso agregar desde el catálogo es legítimo y entra por el mismo camino.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: **466** (423 unitarias + 43 de integración), **+9 unitarias** del núcleo `ArmadorFotoMarcador` (coordenada forzada, binario vacío, rango inválido, nombre por defecto). La suite del gate queda verde y la cobertura DoD se mantiene: Domain 88,5/80,5, Application 87,6/76,5, `GeoVial.Sync` 94,5/**90,1** (mejora de ramas por el núcleo nuevo bien cubierto), `GeoVial.Revision` 97,7/93,1, `GeoVial.CapturaCampo` 88,9/86,2. El MAUI compila (`net10.0-android`, arm64) y se redeployó al moto g42 (smoke-test de arranque OK; verificación interactiva del flujo con el usuario).

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-15-FOTO-MOVIL | Historia | Aceptada (agregar foto cámara/galería a un marcador en su posición, RN-05) |
| BT-FOTO-MARCADOR-NUCLEO | Tarea | Aceptada (`ArmadorFotoMarcador` en el gate + tests) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido se traslada. |

Pendiente: verificación on-device interactiva del flujo completo (cámara/galería → la foto aparece en el marcador; relevamiento cerrado → rechazo; offline → cola). Posible próximo paso (no comprometido): **quitar** una foto de un marcador (US-15 lo contempla, pero requiere endpoint nuevo + método de dominio); y, si se decide extender la spec, llevar al móvil el "crear punto colocándolo en el mapa" (hoy función web).

## 7. Decisiones tomadas durante el review

- "Agregar foto a un marcador" = **captura en la coordenada del marcador** (RN-02), no un endpoint nuevo ni una `Foto` huérfana: mantiene intacto el modelo (RC-04) y reusa toda la tubería existente.
- El armado **descarta el EXIF** de la foto elegida y usa la coordenada del marcador (decisión explícita, cubierta por test).
- **Quitar** foto queda fuera de alcance este sprint (necesita backend); se documenta como candidato.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Sprint review del Sprint 57 (US-15/CU-09 móvil: agregar foto a un marcador). Veredicto Cumplido, velocity 8, 0 carry-over, 466 pruebas (+9 unitarias del núcleo `ArmadorFotoMarcador`); sin cambios de backend (agrupación por radio RN-02); MAUI redeployado. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
