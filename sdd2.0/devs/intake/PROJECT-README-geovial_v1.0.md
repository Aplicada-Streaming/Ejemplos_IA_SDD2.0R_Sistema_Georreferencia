# PROJECT-README — GeoVial

| Campo | Valor |
|---|---|
| Nombre del proyecto | GeoVial |
| Repositorio | `https://github.com/<organizacion>/geovial` — (Supuesto a validar: URL real) |
| Lead técnico | Arquitecto de software (AG-05) — Equipo SDD 2.0 |
| Documento | `PROJECT-README-geovial_v1.0.md` |
| Versión | 1.0 |
| Fecha | 2026-05-31 |
| Stack principal | C# / .NET — Blazor Interactive Server (web) + .NET MAUI (móvil) |
| Estado | En revisión |

> Este documento captura cómo se va a construir el sistema descrito en PROJECT-BRIEF.
> Contiene decisiones técnicas de stack, arquitectura, testing, CI/CD, samples.
> NO repite las necesidades del cliente: las asume conocidas vía PROJECT-BRIEF.

> **Nota de procedencia.** Los valores marcados **(Supuesto a validar)** son propuestas de
> ingeniería que precisan datos del cliente o confirmación de equipo. El resto son
> decisiones técnicas firmes tomadas por los roles AG-05, AG-08 y AG-09.

---

## §1 Tipo de proyecto

**Tipo seleccionado (D8): `web-monolith`.**

GeoVial es una solución multi-artefacto, y SDD 2.0 representa exactamente este caso con el mecanismo de §1: se elige el **tipo dominante** y se declaran los demás como **sub-proyectos** en §5. La multiplicidad no se pierde, se modela.

El artefacto dominante es un back-end monolítico que, junto a un front web administrativo en Blazor Interactive Server, concentra la administración, la revisión y la confección de informes. Ese mismo backend expone una API REST consumida también por la app móvil. Se elige `web-monolith` por sobre `rest-api` porque el tipo `web-monolith` incluye como obligatoria la categoría `03_ux_ui_dx` (el sistema tiene UX rica: mapa, pines, carrusel), mientras que `rest-api` la deja solo como recomendada. La superficie de API queda cubierta dentro del monolito.

Sub-proyectos declarados (cada uno con su clasificación D8):

| Sub-proyecto | Tipo D8 | Notas |
|---|---|---|
| App de captura en terreno | `mobile-app-maui` | .NET MAUI con páginas Blazor integradas + MudBlazor; persistencia local SQLite; modo offline |
| Librería de sincronización | `library` | Publicable como paquete en repositorio GitHub; incluye demo MAUI autónoma para reuso |
| Librería de alojamiento de archivos | `library` | Interna del backend; backends configurables (local / S3 / otro); no se publica como paquete |

**Flags de gating (para §4 del master-prompt):**

```text
project_type: web-monolith
nombre-kebab: geovial
usa_llm: false
tiene_ui_final: true
multi_tenant: false          # un único organismo; las "áreas" son ámbitos de autorización, no tenants
tiene_auth: true
equipo_n: 4                  # (Supuesto a validar)
tiene_portal_developers: true # la librería de sync se publica para reuso → activa 10 y 11
tiene_extensibilidad: true   # la librería de archivos tiene backends configurables
tiene_persistencia: true
requiere_compliance: true    # datos personales bajo Ley 25.326 (ver BRIEF §10)
tiene_observabilidad_critica: false # uso interno; observabilidad estándar, no crítica en v1
```

---

## §2 Stack tecnológico

| Componente | Tecnología | Versión | Justificación |
|---|---|---|---|
| Lenguaje / plataforma | C# / .NET | .NET 9 o superior, LTS vigente — (Supuesto a validar) | Definido por el cliente |
| Front web | Blazor Interactive Server + MudBlazor | MudBlazor 7.x — (Supuesto a validar) | Definido por el cliente; render server-side |
| App móvil | .NET MAUI con Blazor integrado + MudBlazor | Igual a .NET de base | Definido por el cliente |
| Autenticación | Flujo ROPC con bearer token JWT | — | Definido por el cliente |
| Base de datos backend | SQL Server | SQL Server 2022 — (Supuesto a validar) | Definido por el cliente |
| Persistencia local móvil | SQLite | sqlite-net-pcl | Definido por el cliente; soporta offline |
| Mapas | OpenStreetMap + Leaflet | Leaflet 1.9.x — (Supuesto a validar) | Definido por el cliente; mismo recurso en web y móvil |

Licencias: todas las dependencias deben ser MIT/Apache/BSD compatibles con uso en organismo público; sin GPL en componentes distribuidos.

---

## §3 Estilo arquitectónico

**Estilo adoptado: monolito modular con Clean Architecture.**

Capas Domain, Application, Infrastructure y Web/API, con módulos por contexto: usuarios y jerarquía, relevamientos, observaciones y marcadores, sincronización, alojamiento de archivos. **Se adopta CQRS ligero** (separación de commands y queries vía mediador) solo en el módulo de relevamientos/observaciones, que es el de dominio más rico; el resto usa servicios de aplicación directos.

Alternativas descartadas:

- **Microservicios:** sobredimensionado para el tamaño del problema y del equipo (≈4 devs); el cliente pide explícitamente backend monolítico e infraestructura de tres contenedores.
- **Monolito en capas tradicional acoplado al ORM:** dificulta las pruebas unitarias del dominio, requeridas desde la fase 2.

Esta decisión se materializa como ADR-001 en el Sprint 0.

---

## §4 Esquema de descomposición y delivery

Se adopta **vertical slicing** dentro de un flujo Scrum, como indica el cliente, para tener avances entregables y funcionales al cerrar cada sprint. Cada fase queda funcional respecto de la anterior, se prueba automáticamente para su ajuste y luego pasa a revisión del usuario.

El primer slice end-to-end (jerarquía y manejo de usuarios sobre backend + front + base de datos) entrega valor demostrable: se prueba el alta/baja según jerarquía y los logueos. Las fases del cliente se mapean así:

1. Scaffolding (walking skeleton): creación de todos los proyectos.
2. Backend + front + base de datos: jerarquía y manejo de usuarios, con pruebas unitarias.
3. App móvil: jerarquía y manejo de usuarios, alineada al backend/front.
4. Relevamientos (jefe de área): alta, baja, visualización y creación de marcadores; asignación de agentes.
5. Recolección (agente de campo): visualización y recolección, con datos de testing y previsualización manual.
6. Cierre: puntos restantes para finalizar el desarrollo.

Fase final adicional: scripts para generar las imágenes de los servicios.

---

## §5 Estructura de repositorio propuesta

Monorepo único. La librería de sincronización vive en el monorepo y se publica como paquete; su demo MAUI autónoma va en `/samples`.

```text
geovial/
├── src/
│   ├── GeoVial.Domain/              # Entidades: Relevamiento, Observación, Marcador, Usuario, Etiqueta
│   ├── GeoVial.Application/         # Casos de uso, validaciones, jerarquía y estados (CQRS en relevamientos)
│   ├── GeoVial.Infrastructure/      # EF Core + SQL Server, integración con librería de archivos
│   ├── GeoVial.Api/                 # API REST, autenticación ROPC/JWT, OpenAPI
│   ├── GeoVial.Web/                 # Front Blazor Interactive Server + MudBlazor (admin/revisión)
│   ├── GeoVial.Mobile/              # Sub-proyecto: app de captura (.NET MAUI + Blazor + MudBlazor)
│   ├── GeoVial.Shared/              # Modelos y DTOs compartidos
│   ├── GeoVial.Sync/                # Sub-proyecto librería: sincronización (paquete)
│   └── GeoVial.FileHosting/         # Sub-proyecto librería: alojamiento de archivos (local/S3/otros)
├── tests/
│   ├── GeoVial.UnitTests/
│   ├── GeoVial.IntegrationTests/
│   ├── GeoVial.ComponentTests/      # bUnit para componentes Blazor
│   └── GeoVial.UiTests/             # UI testing móvil
├── samples/
│   ├── 01-sync-basico/              # Consumidor mínimo de la librería de sincronización
│   ├── 02-sync-maui-demo/           # Demo MAUI autónoma de la librería de sincronización (requerida)
│   └── 03-filehosting-backends/     # Ejemplo local / S3 / otros
├── docs/                            # Categorías SDD 00-11 generadas por el orquestador
├── devs/
│   └── intake/                      # PROJECT-BRIEF-geovial, PROJECT-README-geovial
├── scripts/                         # build-*.bat, publish-*.bat y scripts de imágenes
├── PROJECT-README-geovial_v1.0.md
└── README.md                        # Apuntador corto al PROJECT-README
```

### §5.X Materialización de `/samples`

| Sample | Nivel | Contenido |
|---|---|---|
| `01-sync-basico` | Básico | Consumidor mínimo de la librería de sincronización contra un backend mock |
| `02-sync-maui-demo` | Intermedio | App de demostración MAUI **ajena al sistema** que evalúa la librería de sincronización para reutilizarla en otros proyectos (requerimiento explícito del cliente) |
| `03-filehosting-backends` | Avanzado | Ejemplo del soporte de alojamiento configurable: local, AWS S3 u otro disponible |

---

## §6 Comunicación e integración

El backend expone una **API REST (JSON)** consumida por el front web Blazor y la app móvil MAUI. Autenticación ROPC → JWT bearer.

| Aspecto | Decisión |
|---|---|
| Protocolo | HTTP/REST con JSON; errores con Problem Details (RFC 7807) |
| Contrato | OpenAPI 3.x versionado en el repo, generado desde la API |
| Versionado de API | Por URL: `/api/v1/` |
| Sincronización móvil | Subir cambios locales y luego bajar actualizaciones de los relevamientos del agente |
| Resolución de conflictos | **Last-write-wins** a nivel de campo, con marca de conflicto resoluble manualmente desde la web; conflictos de marcadores en un mismo radio decididos por el jefe de área |
| Exportación / importación | Relevamiento completo (datos, comentarios, etiquetas y fotos) en un único archivo ZIP |

---

## §7 Persistencia

| Capa | Motor | Notas |
|---|---|---|
| Backend | SQL Server, acceso vía EF Core | Base de datos central; en desarrollo, SQL Server local |
| App móvil | SQLite | Persistencia interna que habilita offline y la cola de cambios pendientes de sincronizar |
| Archivos (fotos) | Librería de alojamiento | Backends configurables por el usuario raíz: local, AWS S3 u otro; el almacenamiento local reside sobre el contenedor del backend |

Modelo de datos lógico (resumen): `Relevamiento (1) → (N) Observación`; `Observación (N) → (1) Marcador`; `Marcador (1) → (N) Foto` y `(N) Comentario`; `Foto/Comentario (N) → (N) Etiqueta`. La cola local SQLite registra cada cambio con tipo de operación, entidad, timestamp y estado de sincronización. El detalle fino se desarrolla en `02_especificacion_funcional/modelo-datos/`.

---

## §8 Seguridad y autenticación

- Flujo **ROPC con bearer token JWT** para las APIs REST; claims `sub` y `role`.
- **Primer login con internet**; en el campo, el relogueo se hace con los métodos de seguridad del teléfono (PIN/biometría), requisito para habilitar el modo offline.
- Jerarquía de roles con autorización por rol: usuario raíz, jefe general, jefe de área, agentes de campo.
- **Manejo de secretos:** `.env` fuera de git (con `.env.example` versionado) en desarrollo; secretos de CI en GitHub Secrets con rotación a 90 días; en producción, secret store gestionado del entorno de deploy.
- **Tokens:** access token de vida corta (≈60 min) + refresh token; en móvil el refresh se condiciona al método de seguridad del teléfono.
- **Auditoría:** registro de accesos y acciones administrativas con retención mínima de 1 año (alineado a Ley 25.326).

---

## §9 Estrategia de testing

Pirámide: 70% unitarios, 20% integración, 10% componente/UI.

| Nivel | Framework | Alcance |
|---|---|---|
| Unitarios | xUnit + FluentAssertions | Dominio y aplicación (jerarquía, estados, reglas de marcadores/observaciones) |
| Integración | WebApplicationFactory + Testcontainers (SQL Server) | API + persistencia real |
| Componente | bUnit | Componentes Blazor del front web |
| UI móvil | .NET MAUI UI testing / Appium sobre Android | Flujos críticos de captura y sincronización |

**Cobertura mínima (gate de CI bloqueante): líneas ≥ 80%, branches ≥ 70%.** Generación de datos de testing y previsualización manual en la fase de recolección, como pide el cliente.

---

## §10 Estrategia de versionado y release

- **SemVer 2.0.0 + Conventional Commits** sin excepciones.
- Cálculo automático de versión con **MinVer** (o Nerdbank.GitVersioning).
- Branching **GitHub Flow** (rama principal protegida + ramas de feature con PR).
- La **librería de sincronización** se publica en **GitHub Packages** con canales preview (prerelease) y stable; cualquier breaking change de su API pública bumpea MAJOR.

---

## §11 Pipeline CI/CD

Plataforma: **GitHub Actions** (el repositorio vive en GitHub). En desarrollo local se trabaja con SQL Server local, servicios locales y la app móvil sobre Android por USB en modo desarrollador. Se crean scripts BAT (`build-*.bat`, `publish-*.bat`) invocables por el propio proceso de codificación y ajuste, y al final scripts para generar las imágenes de los servicios.

Stages del pipeline (los marcados como gate son bloqueantes):

1. Restore.
2. Build (**gate:** compila sin warnings tratados como error).
3. Tests unitarios + integración (**gate:** cobertura líneas ≥ 80%, branches ≥ 70%).
4. Empaquetado de la librería de sincronización (NuGet/GitHub Packages).
5. Build de imágenes Docker (front, backend, base de datos).
6. Publish (paquetes e imágenes).

Infraestructura objetivo: tres contenedores — front-end, backend y base de datos — con el almacenamiento local de archivos sobre el contenedor del backend.

---

## §12 Compatibilidad

| Componente | Plataforma | Versión mínima |
|---|---|---|
| App móvil | Android | Android 8.0 (API 26) — (Supuesto a validar) |
| Front web | Navegadores evergreen | Últimas 2 versiones de Chrome, Edge, Firefox y Safari |
| Backend / base de datos | Contenedores Linux | Front, backend y base de datos en tres contenedores |

iOS y tablets quedan **fuera de alcance en v1** (coherente con la prueba sobre Android por USB).

---

## §13 NFR (requerimientos no funcionales)

Métricas **propuestas por ingeniería (a validar con el cliente)**:

| NFR | Objetivo |
|---|---|
| Operación offline | ≥ 1 jornada laboral completa (8 h) sin conexión |
| Tiempo de sincronización | ≤ 5 minutos para una jornada típica (≈100 observaciones con fotos) |
| Latencia API (lecturas administrativas) | p95 ≤ 500 ms |
| Detección de conectividad | Automática; la sincronización se dispara sola al recuperar señal |
| Disponibilidad del backend | SLO 99% en horario laboral |
| Tamaño de foto sincronizada | Compresión/redimensión para acotar el payload de sincronización — (Supuesto a validar: límites) |

---

## §14 Demo / samples

- **Requerido por el cliente:** la librería de sincronización incluye un ejemplo de demostración MAUI ajeno al sistema, que permite evaluar la librería para reutilizarla en otros proyectos. Alcance: alta de registros locales, sincronización con un mock server, visualización del estado de la cola y resolución básica de conflictos.
- `01-sync-basico`: consumidor mínimo que sincroniza un set de registros de ejemplo.
- `03-filehosting-backends`: misma operación de subida de archivos contra backend local y contra S3.

Datos de demostración: dataset sintético de relevamientos y fotos de ejemplo incluido en cada sample.

---

## §15 Pre-ADR (decisiones técnicas a formalizar como ADR)

| Pre-ADR | Decisión | Estado |
|---|---|---|
| ADR-001 | Estilo arquitectónico: monolito modular con Clean Architecture + CQRS ligero | Propuesto |
| ADR-002 | Backend monolítico que expone API REST a web y móvil | Decidido por el cliente |
| ADR-003 | Autenticación ROPC + JWT bearer con refresh condicionado al método de seguridad del teléfono | Propuesto |
| ADR-004 | Mapas con OpenStreetMap + Leaflet en web y móvil | Decidido por el cliente |
| ADR-005 | SQLite + cola de cambios para soporte offline en móvil | Propuesto |
| ADR-006 | Resolución de conflictos last-write-wins con override manual desde la web | Propuesto |
| ADR-007 | Librería de sincronización como paquete en GitHub Packages | Decidido por el cliente |
| ADR-008 | Librería de alojamiento de archivos con backends configurables (local / S3 / otro) | Decidido por el cliente |

---

## §16 Restricciones y trade-offs

- Desarrollo y prueba **íntegramente local** durante codificación (SQL Server local, servicios locales). Consecuencia: el entorno contenerizado se valida recién hacia las fases finales.
- Prueba de la app móvil **solo sobre Android por USB**. Consecuencia: otras plataformas y la distribución por stores quedan sin cobertura en el ciclo de desarrollo.
- **Tres contenedores** con almacenamiento local sobre el contenedor del backend. Consecuencia: el almacenamiento local acopla los archivos al ciclo de vida del contenedor del backend salvo que se configure un backend externo (S3 u otro).
- **Blazor Interactive Server** en el front web. Consecuencia: requiere conexión persistente con el servidor para la interactividad del front administrativo.
- **Last-write-wins** en sincronización. Consecuencia: puede descartar cambios concurrentes; se compensa con la marca de conflicto y la resolución manual desde la web.

---

## §17 Checklist de completitud técnica

- [x] Tipo de proyecto seleccionado entre los 8 valores de D8 (§1): `web-monolith` con sub-proyectos declarados.
- [x] Tabla de implicancias por tipo verificada contra el tipo elegido (§1).
- [x] Stack tecnológico declara lenguaje, versión, runtime y dependencias core con justificación (§2).
- [x] Estilo arquitectónico elegido y justificado contra al menos dos alternativas descartadas (§3).
- [x] Estrategia de descomposición garantiza valor demostrable end-to-end en el primer sprint (§4).
- [x] Estructura de repositorio publicada como árbol `tree` completo y coherente con tipo y estilo (§5).
- [x] Subsección §5.X describe materialización de `/samples` según tipo D8.
- [x] Estrategia de testing declara cobertura mínima numérica de líneas y de branches (§9).
- [x] Estrategia de versionado adopta SemVer 2.0.0 y Conventional Commits con herramienta automatizada (§10).
- [x] Pipeline CI/CD enumera stages con quality gates explícitos y bloqueantes (§11).
- [x] Compatibilidad declara SO, runtimes y versiones mínimas (§12).
- [x] NFR expresados con métricas numéricas (§13).
- [x] Estructura de `/samples` describe al menos un sample por nivel de complejidad (§14).
- [x] Decisiones pre-ADR de §15 tienen justificación documentada y alternativas evaluadas.
- [x] Trade-offs aceptados documentados con su contrapartida (§16).
- [x] Control de cambios actualizado con la versión y fecha del documento.

---

## Apéndice — Supuestos a validar con el cliente / equipo

1. URL real del repositorio (cabecera).
2. Tamaño del equipo (`equipo_n`, §1) — usado para la decisión de estilo.
3. Versiones exactas de .NET, MudBlazor, SQL Server y Leaflet (§2).
4. Versión mínima de Android (§12).
5. Targets de NFR: tiempo de sincronización, latencia, disponibilidad y límites de tamaño de foto (§13).

---

## Trazabilidad downstream

| Sección del README | Categoría SDD destino |
|---|---|
| §1 Tipo de proyecto | Todas las categorías 00-11 |
| §2 Stack | `05_arquitectura_tecnica/` |
| §3 Estilo arquitectónico | `05_arquitectura_tecnica/` (ADR-001) |
| §4 Descomposición y delivery | `07_plan-sprint/` |
| §5 Estructura de repositorio | `05_arquitectura_tecnica/`, `10_developer_guide/` |
| §6 Comunicación e integración | `05_arquitectura_tecnica/` (contratos, OpenAPI) |
| §7 Persistencia | `02_especificacion_funcional/modelo-datos/`, `05_arquitectura_tecnica/` |
| §8 Seguridad | `09_devops/`, `05_arquitectura_tecnica/` |
| §9 Testing | `08_calidad_y_pruebas/` |
| §10 Versionado y release | `09_devops/` |
| §11 Pipeline CI/CD | `09_devops/` |
| §12 Compatibilidad | `00_contexto/` |
| §13 NFR | `00_contexto/`, `08_calidad_y_pruebas/` |
| §14 Samples | `11_examples/` |
| §15 Pre-ADR | `05_arquitectura_tecnica/` |
| §16 Restricciones | `00_contexto/` |

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | 2026-05-31 | README técnico inicial generado a partir de `geovial-requerimientos.md` | Equipo SDD 2.0 |
| 1.0 | 2026-05-31 | Resolución de bloqueantes: tipo D8 firme + flags de gating (§1), versiones de stack (§2), CQRS y modularización (§3), monorepo (§5), OpenAPI y conflictos (§6), modelo de datos y cola (§7), secretos/tokens/auditoría (§8), cobertura y frameworks (§9), versionado (§10), pipeline (§11), compatibilidad (§12), NFR numéricos (§13), samples (§14), ADRs ampliados (§15); estado a "En revisión"; checklist §17 tildado | Equipo SDD 2.0 (AG-05 / AG-08 / AG-09) |

---

**Fin del documento**
