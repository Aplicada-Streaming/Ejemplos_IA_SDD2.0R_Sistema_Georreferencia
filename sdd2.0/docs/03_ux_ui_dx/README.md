# 03 UX / UI / DX — GeoVial

Índice navegable de la categoría 03. Punto de entrada para revisores (AG-05, AG-06, AG-08). Tipo de proyecto: web-monolith. Variante principal: UX/UI (el sistema tiene UI rica: mapa, pines, carrusel). Adición DX: el portal de developers de la librería de sincronización, activado por el flag `tiene_portal_developers: true`.

Mínimos §2.2 (web-monolith): experiencia-de-uso obligatorio; al menos 4 wireframes (login, home, flujo principal, error). Acá se generan 6 wireframes por la cobertura de los CU con interacción humana significativa.

## Marco de experiencia (variante UX/UI)

| Artefacto | Documento | Variante | Propósito | Estado |
| --- | --- | --- | --- | --- |
| Experiencia de uso | [experiencia-de-uso_v1.0.md](experiencia-de-uso_v1.0.md) | UX/UI | Marco de experiencia: audiencia, principios (Nielsen y leyes UX), flujos, estados/feedback, accesibilidad WCAG 2.2 AA, i18n, performance percibida, errores | Propuesto |
| Glosario UX | [glosario-ux_v1.0.md](glosario-ux_v1.0.md) | UX/UI | Terminología de presentación; referencia los términos de dominio de 00/02 sin duplicarlos | Propuesto |

## Wireframes (variante UX/UI, 6 superficies)

| Wireframe | Documento | CU que materializa | Estado |
| --- | --- | --- | --- |
| Login y relogueo en campo | [wireframes-login-relogueo_v1.0.md](wireframes-login-relogueo_v1.0.md) | CU-02 (login online y relogueo offline con método de seguridad, RN-06) | Propuesto |
| Mapa de revisión web | [wireframes-mapa-revision-web_v1.0.md](wireframes-mapa-revision-web_v1.0.md) | CU-08 (revisión sobre mapa, exportar/importar, filtrar) | Propuesto |
| Marcador con carrusel | [wireframes-marcador-carrusel_v1.0.md](wireframes-marcador-carrusel_v1.0.md) | CU-09 (carrusel, comentarios, etiquetas, navegación) | Propuesto |
| Captura móvil offline | [wireframes-captura-movil_v1.0.md](wireframes-captura-movil_v1.0.md) | CU-04 y CU-06 (captura georreferenciada, modo sin conexión) | Propuesto |
| Gestión de relevamientos | [wireframes-gestion-relevamientos_v1.0.md](wireframes-gestion-relevamientos_v1.0.md) | CU-01 y CU-10 (crear/asignar, transición de estados) | Propuesto |
| Resolución de conflictos web | [wireframes-resolucion-conflictos-web_v1.0.md](wireframes-resolucion-conflictos-web_v1.0.md) | CU-11 y CU-12 (detectar y resolver conflictos) | Propuesto |

Cada wireframe incluye las 9 secciones de §4.2.1 y los estados mínimos (vacío, cargando, con datos, error), más "sin conexión" en las superficies móviles y "solo lectura" donde aplica RN-05.

## Adición DX (variante DX)

| Artefacto | Documento | Variante | Propósito | Estado |
| --- | --- | --- | --- | --- |
| Portal de developers | [dx-portal-developers_v1.0.md](dx-portal-developers_v1.0.md) | DX | Portal de documentación de la librería de sincronización publicada para reuso por terceros (Diátaxis, páginas obligatorias, accesibilidad, métricas) | Propuesto |

Nota sobre el alcance DX en 03: la única adición DX en esta categoría es el portal de developers, habilitada por el flag `tiene_portal_developers`. El onboarding completo y la referencia detallada del integrador se cubren en la categoría 10; los ejemplos ejecutables (samples), en la 11. Acá se especifica el portal, no se lo reemplaza.

## Trazabilidad de la categoría

- Upstream: 00 (vision-producto_v1.0.md persona objetivo y visión; alcance-proyecto_v1.0.md capacidades), 02 (CU-01, CU-02, CU-04, CU-05, CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12; RN-01 a RN-08).
- Downstream: 05 (capa de presentación web y móvil; contrato de la librería de sincronización), 06 (US con criterios de aceptación de ergonomía), 08 (snapshot, accesibilidad WCAG 2.2 AA, UI), 10 (developer guide), 11 (samples).
