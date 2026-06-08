# Plan de Iteración — Sprint 63

**Proyecto:** GeoVial
**Documento:** plan-iteracion-sprint-63_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha inicio:** 2028-10-30
**Fecha fin:** 2028-11-10
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Información general

- Duración: 2 semanas. Equipo: 4 (`equipo_n: 4`). Estimación en story points.
- Capacidad: promedio móvil 5,0 SP (S60–S62). Se agrupan los **P2 de pulido** de la auditoría UX en un solo sprint (5 SP); UI sin núcleo nuevo ni backend.

## 2. Objetivo del sprint

**Saldar los P2 de la auditoría UX móvil** (`evaluacion-ux-mobile_v1.0.md`), cerrados ya los 4 P1 (S59-S62). Bundle de pulido seguro (presentación/comportamiento, sin tocar dominio/datos/sync):

- **H-10** — el contenido se carga **al entrar** a la solapa, sin esperar un toque a "Cargar"/"Recargar" (faltaba en *Revisión*; *Mapa* y *Bandeja* ya lo hacían).
- **H-12** — "Cerrar sesión" (acción destructiva) pasa al **overflow** del toolbar, para evitar el toque accidental.
- **H-13** — **contraste** de los placeholders del login (gris claro por defecto < 4.5:1).
- **H-11** — el **disparador de foto** ("Tomar foto") como acción primaria destacada (objetivo grande, Ley de Fitts); "Elegir foto" como secundaria.

## 3. Historias y tareas comprometidas

| ID | Tipo | Descripción | Prioridad | Estimación | Asignado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| P2-PULIDO-UX | Tarea | Bundle de P2 de la auditoría: H-10, H-12, H-13, H-11 | Media | 5 | Dev móvil (AG-08) | Cerrada |

Total: 5 SP (UI de pulido; sin núcleo nuevo ni backend).

## 4. Alcance técnico

- **H-10 (`RevisionPage`):** se extrae `CargarRevisionAsync()` y se llama desde `OnCargar` y desde un nuevo `OnAppearing` (carga una vez si aún no hay datos; el botón "Cargar revisión" se conserva para refrescar). De paso, el error de carga pasa a mensaje llano (experiencia §8), sin el detalle de la excepción.
- **H-12 (`MainPage`):** el `ToolbarItem` "Cerrar sesión" se crea con `Order = ToolbarItemOrder.Secondary` (overflow).
- **H-13 (`LoginPage`):** `PlaceholderColor = #595959` (~7:1 sobre blanco) en los `Entry` de usuario y clave.
- **H-11 (`CapturaPage.xaml`):** "📷 Tomar foto" con `FontSize=20`, `Bold`, `HeightRequest=64` (primario); "Elegir foto de la galería" como acción secundaria (texto chico, sin fondo).
- **Sin cambios de backend/Application/Domain. Sin núcleo nuevo** (UI declarativa + un refactor de carga).

## 5. Definition of Done aplicada

- *Revisión* muestra los datos al entrar a la solapa, sin tocar "Cargar revisión"; el botón sigue para refrescar a mano.
- "Cerrar sesión" no está en la barra principal (va al overflow); los placeholders del login tienen contraste suficiente; el disparador de foto es la acción primaria destacada.
- Suite del gate verde (sin cambios; UI de pulido); cobertura DoD sin regresión.
- El MAUI compila y se verifica on-device.

## 6. Riesgos y mitigaciones

| Riesgo | Prob. | Impacto | Mitigación |
| --- | --- | --- | --- |
| Auto-cargar Revisión al reentrar pisa una edición reciente | Baja | Bajo | Carga una vez (`_nav is null`); las ediciones refrescan con `RecargarAsync` |
| El overflow esconde demasiado "Cerrar sesión" | Baja | Bajo | Sigue accesible en el menú de 3 puntos; es lo buscado para una acción destructiva |

## 7. Criterios de hecho del sprint

Completo cuando: Revisión auto-carga; logout en overflow; placeholders con contraste; disparador prominente; la suite del gate verde; el MAUI compila y se verifica on-device; y se facilitan review y retro.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Origen | Auditoría UX móvil `evaluacion-ux-mobile_v1.0.md`, hallazgos **P2** (H-10, H-11, H-12, H-13) |
| CU/UX | experiencia-de-uso §2 (Doherty, prevención de errores), §5 (WCAG 2.2 AA: contraste, tamaño de objetivo) |
| Componentes | `GeoVial.Mobile` (`RevisionPage`, `MainPage`, `LoginPage`, `CapturaPage`) |
| Calidad | definition-of-done §1.4 |
| Tests | Sin núcleo nuevo (UI); verificación on-device |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-07 | Plan del Sprint 63 (P2 de la auditoría UX: H-10 auto-carga, H-12 logout al overflow, H-13 contraste de placeholders, H-11 disparador prominente). UI de pulido, sin núcleo ni backend. 5 SP. Generado por AG-07 |
