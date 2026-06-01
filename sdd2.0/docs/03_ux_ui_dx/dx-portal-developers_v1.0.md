# Portal de developers — Librería de sincronización GeoVial

**Proyecto:** GeoVial
**Documento:** dx-portal-developers_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** DX Lead (AG-03, variante DX), Equipo SDD 2.0
**Variante:** DX
**Trazabilidad upstream:** PROJECT-README §1 (sub-proyecto librería de sincronización, flag tiene_portal_developers), §14 (samples); alcance-proyecto_v1.0.md; CU-07 (sincronización) como contrato funcional de la superficie publicada
**Trazabilidad downstream:** 10_developer_guide (onboarding y referencia detallada del integrador), 11_examples (samples ejecutables), 05_arquitectura_tecnica (contrato de la librería)

## 0. Encuadre

Este documento especifica el portal de documentación para developers de la librería de sincronización de GeoVial, que se publica para que terceros la reutilicen en otros proyectos. La activación responde al flag `tiene_portal_developers: true`. Acá se especifica el portal (su estructura, navegación, páginas, accesibilidad y métricas), no su contenido detallado: el onboarding completo y la referencia fina del integrador viven en la categoría 10; los ejemplos ejecutables viven en 11. El portal es la puerta de entrada que enlaza esos materiales.

## 1. Audiencia y objetivos del portal

Audiencia: developer integrador que evalúa la librería de sincronización para reutilizarla en un proyecto propio, ajeno a GeoVial. Llega con experiencia en aplicaciones que necesitan operar sin conexión y sincronizar después; conoce el patrón de cola de cambios y resolución de conflictos, pero no conoce la librería. Busca decidir rápido si le sirve y, si le sirve, llegar al primer resultado sin fricción.

Objetivos del portal:

- Permitir que el integrador entienda en pocos minutos qué resuelve la librería (subir cambios locales, bajar actualizaciones, marcar conflictos por última escritura) y si encaja en su caso.
- Llevarlo del aterrizaje al primer resultado exitoso por el camino más corto (quick-start), apoyándose en el sample de demostración publicado.
- Servir como índice navegable hacia la documentación de los cuatro modos Diátaxis y hacia el changelog y el estado de publicación.
- Sostener la confianza: dejar claro el versionado (cualquier cambio incompatible de la API pública sube la versión mayor) y el estado de cada release.

No-objetivos: el portal no reemplaza la guía de onboarding detallada (categoría 10) ni los ejemplos (categoría 11); no documenta el sistema GeoVial completo, solo la librería publicada.

## 2. Estructura de información según Diátaxis

| Modo | Orientación | Qué cubre para la librería | Dónde vive |
| --- | --- | --- | --- |
| Tutorial | Aprendizaje | Recorrido guiado de extremo a extremo: del aterrizaje a sincronizar un set de registros de ejemplo contra el backend de prueba | Categoría 10 (guía de onboarding del integrador); el portal lo enlaza como punto de entrada |
| How-to | Tarea | Recetas concretas: encolar un cambio, disparar la sincronización al recuperar conexión, leer el estado de la cola, atender un conflicto marcado por última escritura | Categoría 10; el portal agrupa los how-to por tarea |
| Reference | Información | Descripción de la superficie pública de la librería: operaciones de sincronización, modelo de la cola de cambios, marcas de conflicto, opciones de configuración | Categoría 10 (referencia detallada); el portal expone la página reference como índice |
| Explanation | Comprensión | Por qué primero se sube lo local y luego se baja lo remoto; por qué la resolución es por última escritura con marca de conflicto; el modelo de idempotencia por identificador único | Categoría 10; el portal enlaza la explicación del modelo de sincronización |

El portal no duplica el contenido de cada modo: lo organiza y lo enlaza de forma explícita, de modo que el integrador pueda moverse entre aprender, hacer, consultar y comprender sin perderse.

## 3. Navegación principal y búsqueda

- Navegación primaria persistente con las entradas: Inicio (landing), Quick-start, Tutorial, How-to, Reference, Explicación, Changelog, Estado.
- Navegación secundaria contextual dentro de Reference y How-to (índice lateral por operación o por tarea).
- Búsqueda global del portal sobre todo el contenido publicado, con resultados que indican a qué modo Diátaxis pertenece cada acierto.
- Migas de pan para ubicar la página actual dentro de la jerarquía.
- Enlace visible al repositorio del paquete publicado y al sample de demostración.

## 4. Páginas obligatorias

| Página | Propósito | Contenido mínimo |
| --- | --- | --- |
| Landing | Comunicar qué resuelve la librería y a quién sirve | Propuesta de valor en una frase, cuándo usarla y cuándo no, enlaces a quick-start y reference |
| Quick-start | Llevar al primer resultado exitoso reproducible | Pasos mínimos: instalar el paquete, apuntar al backend de prueba, encolar un cambio y sincronizarlo; snippet ejecutable (ver §5) |
| Reference | Servir de índice de la superficie pública | Listado de operaciones de sincronización, modelo de la cola y de las marcas de conflicto, opciones de configuración; remite a la referencia detallada de la categoría 10 |
| Changelog | Comunicar la evolución y la compatibilidad | Entradas por versión con SemVer; marca explícita de cambios incompatibles que suben la versión mayor; canales preview y stable |
| Status | Comunicar el estado de publicación | Última versión stable y preview publicadas, estado del paquete y disponibilidad del sample de demostración |

## 5. Ejemplos ejecutables y sandbox

- El quick-start se apoya en el sample de demostración publicado junto a la librería: una app de demostración autónoma, ajena al sistema GeoVial, que permite evaluar la librería en otros proyectos. El sample cubre alta de registros locales, sincronización contra un backend de prueba, visualización del estado de la cola y resolución básica de conflictos.
- El portal enlaza ese sample como ejemplo ejecutable de referencia y como punto de partida del integrador; el detalle del sample y su materialización viven en la categoría 11. El portal no entra en el stack ni en el detalle de implementación del sample.
- Existen además ejemplos de menor alcance (un consumidor mínimo que sincroniza un set de registros de ejemplo contra un backend de prueba), enlazados desde el quick-start para el primer resultado.
- Verificación del quick-start: antes de cada publicación, el snippet del quick-start y el arranque del sample se ejecutan manualmente para confirmar que producen el primer resultado exitoso (alta de un registro local y su sincronización visible en el estado de la cola). Un quick-start que no corre se considera bloqueante.

## 6. Accesibilidad del portal (WCAG 2.2 AA)

Compromiso explícito: el portal cumple WCAG 2.2 nivel AA como piso mínimo. Criterios prioritarios:

- Contraste de texto 4.5:1 y de componentes 3:1, incluido el contenido de los bloques de código y sus controles.
- Operación completa por teclado de la navegación, la búsqueda y los bloques de código (incluido copiar el snippet); foco visible y orden de foco lógico.
- Estructura semántica de encabezados y puntos de referencia para navegar por lectores de pantalla; los bloques de código exponen su lenguaje y son anunciables.
- Alternativas textuales para diagramas (por ejemplo, el orden de la sincronización subir-luego-bajar); no depender solo del color para distinguir estados del changelog (estable, preview, incompatible).
- Resultados de búsqueda y cambios de página anunciados por región en vivo.

## 7. Métricas de uso del portal

| Métrica | Definición | Objetivo | Cómo se mide |
| --- | --- | --- | --- |
| TTFS (time-to-first-success) | Tiempo desde el aterrizaje hasta correr el quick-start con éxito | ≤ 5 minutos | Pruebas con cinco developers integradores y telemetría opcional con consentimiento |
| TTFV (time-to-first-value) | Tiempo hasta sincronizar un caso propio del integrador con la librería | ≤ 1 hora | Encuesta a integradores de adopción reciente |
| Tasa de salida en quick-start | Porcentaje que abandona antes del primer resultado exitoso | ≤ 20% | Analítica de navegación del portal con consentimiento |
| Cobertura de búsqueda | Porcentaje de búsquedas con al menos un resultado útil | Alta, en mejora continua | Registro de consultas sin resultado (con consentimiento) |
| Tráfico por modo Diátaxis | Distribución de visitas entre tutorial, how-to, reference y explicación | Equilibrio coherente con el recorrido del integrador | Analítica de páginas por modo |

La telemetría es opcional y con consentimiento, coherente con el tratamiento de datos del proyecto.

## 8. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Especificación inicial del portal de developers de la librería de sincronización, generada por AG-03 (variante DX) por el flag tiene_portal_developers |
