# Estrategia de testing — GeoVial

**Proyecto:** GeoVial
**Documento:** estrategia-testing_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Ingeniero QA / SDET Senior (AG-08), Equipo SDD 2.0

## 1. Pirámide de testing deseada

GeoVial es de tipo D8 `web-monolith` (PROJECT-README §1), por lo que adopta la pirámide clásica de §2.2 de la regla, coincidente con PROJECT-README §9.

| Nivel | Qué cubre | Tooling | Porcentaje objetivo |
| --- | --- | --- | --- |
| Unit | Lógica de dominio y aplicación aislada: jerarquía y autorización, estados y transiciones, agrupación por radio, prioridad de metadatos, consolidación last-write-wins, idempotencia de la cola | xUnit + FluentAssertions | 70 % |
| Integration | Interacción entre componentes con persistencia real e infraestructura: API REST + EF Core + SQL Server, contrato OpenAPI, exportación/importación, ciclo de sincronización contra backend | WebApplicationFactory + Testcontainers (SQL Server) | 20 % |
| Componente / E2E (UI) | Componentes Blazor del front web y flujos críticos de captura y sincronización en la app móvil | bUnit (componentes Blazor); .NET MAUI UI testing / Appium sobre Android (UI móvil) | 10 % |

Justificación contra las dos degeneraciones de la pirámide:

- Contra la pirámide invertida (e2e pesado). La lógica de mayor riesgo de GeoVial es algorítmica y verificable sin infraestructura: la autorización por rol y área (RN-01), las transiciones de estado (RN-05), la agrupación por radio (RN-02), la prioridad de metadatos (RN-03) y la consolidación last-write-wins con idempotencia (RN-04, RC-03). Resolverla en unit tests da diagnóstico rápido y suite estable; cargar ese peso en e2e produciría una suite lenta y frágil, justo donde la app móvil sobre Android por USB (PROJECT-README §16) es el nivel más costoso de automatizar.
- Contra la pirámide aplanada (cobertura cuantitativa sin distinguir capas). No basta un porcentaje global: un 80% podría esconder dominio poco cubierto detrás de getters triviales. Por eso la cobertura se reporta por capa (§2) con umbrales diferenciados, y la trazabilidad a CU/RN/NFR (matriz-cobertura-pruebas) prevalece sobre el número.

## 2. Cobertura mínima por capa

Umbrales de §2.2 para `web-monolith` (80% aplicación, 70% infraestructura, 60% presentación), extendidos al dominio que, en Clean Architecture (ADR-01, ADR-10), es la capa más rica y testeable sin infraestructura. El gate global de CI (líneas ≥ 80%, branches ≥ 70%, PROJECT-README §9/§11) es el piso transversal; los pisos por capa son adicionales y no se pueden bajar sin ADR.

| Capa | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- |
| Dominio (entidades, reglas, estados) | ≥ 85 | ≥ 75 | — (no exigido en v1) | 85 / 75 / — |
| Aplicación (handlers CQRS, servicios, autorización) | ≥ 80 | ≥ 70 | — | 80 / 70 / — |
| Infraestructura (EF Core, FileHosting, sync) | ≥ 70 | ≥ 60 | — | 70 / 60 / — |
| Presentación (Web Blazor, API) | ≥ 60 | ≥ 50 | — | 60 / 50 / — |

Mutation testing no se exige en v1 para `web-monolith` (la regla solo lo fija como piso para `library`); se deja como mejora opcional sobre el dominio sin convertirlo en gate. El gate de CI global (líneas ≥ 80%, branches ≥ 70%) se sigue evaluando sobre el agregado de las capas con lógica (dominio + aplicación + infraestructura).

## 3. Tooling

Frameworks reales del proyecto, decisión firme de PROJECT-README §9.

| Nivel / propósito | Framework | Alcance |
| --- | --- | --- |
| Unit | xUnit + FluentAssertions | Dominio y aplicación: jerarquía, estados, reglas de marcadores/observaciones, consolidación e idempotencia |
| Integration | WebApplicationFactory + Testcontainers (SQL Server) | API REST + persistencia real EF Core; contrato OpenAPI; export/import; ciclo de sync contra backend efímero |
| Componente | bUnit | Componentes Blazor del front web (mapa, carrusel, formularios de administración) |
| UI móvil | .NET MAUI UI testing / Appium sobre Android | Flujos críticos de captura georreferenciada y de sincronización en dispositivo Android por USB |
| Cobertura | Coverlet + reporte por capa | Cálculo de líneas/branches por capa para los gates de CI |

## 4. BDD

Los criterios de aceptación Given-When-Then de los CU de 02 (campo `## 8. Criterios de aceptación` de cada CU-XX) son la fuente de las especificaciones de comportamiento. Cada criterio CA de un CU se traduce a un TC referencial (casos-prueba-referenciales) con sus pasos expresados Given-When-Then, y se materializa como test xUnit con nombre que conserva la intención del escenario. No se introduce un runner `.feature` separado en v1: los escenarios viven como tests xUnit nombrados por escenario, lo que mantiene una sola suite y un solo reporte de cobertura. Si en una versión futura se incorpora un runner de especificación ejecutable, los `.feature` vivirían junto a `tests/` y referenciarían el mismo catálogo de TC.

## 5. Mocks y fixtures

- Política de aislamiento. El dominio se prueba sin dobles porque no depende de infraestructura (Clean Architecture). En la capa de aplicación se mockean los puertos (repositorios, `GeoVial.FileHosting`, reloj para marcas temporales de last-write-wins) para mantener los unit tests deterministas. En integración no se mockea la persistencia: se usa SQL Server real efímero vía Testcontainers (§7).
- Reuso y versionado. Builders y fixtures compartidos viven en un proyecto de soporte de tests (`tests/` del monorepo, PROJECT-README §5) y se versionan con el código; se prohíbe duplicar el armado de entidades en cada test (anti-duplicación). Los builders cubren Usuario/Área con rol, Relevamiento con estado y radio, Marcador con posición, Observación con foto y metadatos, y RegistroCambioSync con identificador único.
- Reloj y aleatoriedad controlados. La marca temporal que gobierna last-write-wins (RN-04) se inyecta mediante un reloj falso para que el test fije cuál escritura es la más reciente; ningún test depende de la hora real ni del orden de ejecución.

## 6. Datos de prueba

- Origen: dataset sintético de relevamientos y fotos de ejemplo. No se usan datos de producción reales (datos personales bajo Ley 25.326, RN-08); todo dato personal en tests es ficticio. Este dataset coincide con el que el cliente pide generar en la fase de recolección (PROJECT-README §4 fase 5 y §9: "generación de datos de testing y previsualización manual en la fase de recolección") y con el dataset sintético de los samples (PROJECT-README §14).
- Contenido del dataset: relevamientos en los tres estados (recolección, revisión, cerrado); marcadores con posiciones que ejercitan el radio de agrupación (dentro y fuera del radio); fotos con metadatos de ubicación y fotos sin metadatos (para EXIF→manual→bandeja sin georreferenciar); ediciones concurrentes con marcas temporales para last-write-wins; lote de ≈100 observaciones con fotos para el NFR de sincronización.
- Versionado y regeneración: el dataset se versiona junto al código en `tests/`; su generación es un script reproducible y determinista (semilla fija) invocable localmente y en CI, alineado con la fase de recolección del cliente. Regenerarlo requiere PR con justificación (coherente con la política de snapshots de la regla §4.10).

## 7. Ambiente de testing

- Aislamiento entre tests. Cada test de integración corre contra una instancia de base de datos efímera y limpia; el estado no se comparte entre tests ni depende del orden de ejecución.
- Base efímera con contenedores. La integración usa Testcontainers con SQL Server (PROJECT-README §9); cada corrida levanta y descarta el contenedor, garantizando reproducibilidad local y en CI. En desarrollo local se admite el fallback documentado a una instancia SQL Server local compartida si el contenedor no levanta (plan-iteracion-sprint-00 §6).
- Variables de entorno y secretos. Configuración por `.env` fuera de git con `.env.example` versionado en desarrollo; en CI, los secretos viven en el gestor de secretos de la plataforma (PROJECT-README §8). Los tests usan exclusivamente secretos no productivos; ninguna credencial real de S3 ni del organismo entra en la suite.
- UI móvil. Los flujos de captura y sincronización se prueban sobre dispositivo Android conectado por USB en modo desarrollador (PROJECT-README §11, §16); iOS y tablets quedan fuera de alcance en v1 (ADR-13).
- Backends de archivos. Los contract tests de `GeoVial.FileHosting` corren contra el backend local por defecto; el backend S3 se prueba contra un doble compatible con S3 o un endpoint no productivo, sin credenciales reales (ver guia-testing-extensibilidad).

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Estrategia de testing inicial: pirámide 70/20/10 justificada, cobertura por capa con umbrales numéricos, tooling real (xUnit+FluentAssertions, WebApplicationFactory+Testcontainers, bUnit, MAUI UI/Appium), BDD desde los Given-When-Then de los CU, mocks/fixtures, dataset sintético y ambiente efímero. Generada por AG-08 |
