# Entornos de despliegue y canales de distribución — GeoVial

**Proyecto:** GeoVial
**Documento:** entornos-deploy_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0
**Trazabilidad upstream:** 05 (arquitectura-solucion §5 despliegue de tres contenedores, §8 NFR de disponibilidad y latencia, ADR-07, ADR-08, ADR-14); PROJECT-README §7, §8, §11, §16
**Trazabilidad downstream:** pipeline-ci-cd_v1.0.md; guías de publicación; 11_examples (referencia los canales)

## 0. Dos planos independientes

GeoVial maneja dos cosas que no se confunden (anti-patrón "confundir publicación con despliegue", §4.8):

1. Despliegue de servicio: el monolito desplegable (tres contenedores front/backend/db) se promueve por los ambientes **DEV / QA / STAGING / PROD**.
2. Publicación de paquete: la librería de sincronización `GeoVial.Sync` se distribuye por los canales **preview / stable** de GitHub Packages (ADR-07). Un canal no es un ambiente: publicar un paquete preview no despliega nada.

Las secciones siguientes tratan ambos planos por separado.

## 1. Lista de ambientes y canales

### 1.1 Ambientes de despliegue del monolito (tres contenedores)

Topología por ambiente: contenedor front (`GeoVial.Web`), contenedor backend (`GeoVial.Api` + capas + `GeoVial.FileHosting`, con el almacenamiento local de fotos sobre este contenedor) y contenedor base de datos (SQL Server) (arquitectura-solucion §5; PROJECT-README §7, §16).

| Ambiente | Propósito | URL o destino | Aprobador | SLA / ventana | NFR de 05 referenciado |
| --- | --- | --- | --- | --- | --- |
| DEV | Validación de integración continua del último merge a `main` | entorno de integración interno del organismo | Automático | — | — |
| QA | Verificación funcional y de NFR de integración antes de promover | entorno QA interno | QA lead | hasta verde de la suite de integración (TC-24 latencia, TC-10 sync) | latencia API p95 ≤ 500 ms; tiempo de sync ≤ 5 min |
| STAGING | Soak en entorno equivalente al productivo; validación de NFR | entorno STAGING equivalente a PROD | Release manager | ventana de soak antes de PROD | disponibilidad SLO 99% observada; latencia p95 ≤ 500 ms |
| PROD | Operación real del organismo | entorno productivo del organismo titular | Release manager + aprobación de negocio (jefe general) | SLO disponibilidad 99% en horario laboral (error budget mensual) | disponibilidad 99% (arquitectura-solucion §8) |

Desarrollo local: previo a DEV, la codificación y prueba se hacen íntegramente local (SQL Server local, servicios locales) y la app móvil se valida sobre Android por USB en modo desarrollador (PROJECT-README §11, §16). El entorno contenerizado se valida recién hacia las fases finales (trade-off PROJECT-README §16); por eso DEV es el primer ambiente contenerizado.

### 1.2 Canales de distribución del paquete de la librería de sync

| Canal | Destino | Criterio de uso | Aprobador | SLA |
| --- | --- | --- | --- | --- |
| preview (prerelease) | feed de GitHub Packages, versiones `-alpha/-beta/-rc` | evaluación temprana del reuso; demo MAUI autónoma (samples/02-sync-maui-demo) | Automático | — |
| stable | feed de GitHub Packages, versiones `v<X.Y.Z>` | consumo en producción por otros proyectos del organismo | Release manager | — |

Los NFR de disponibilidad/latencia de 05 no aplican a los canales del paquete: un paquete no es un servicio con SLA de tiempo de respuesta. Su garantía es de compatibilidad de API (SemVer, estrategia-versionado §1 y §6), no de disponibilidad de runtime.

## 2. Provisión (IaC)

- Herramienta declarativa: **Docker Compose** como definición de infraestructura de los tres contenedores por ambiente, versionada en el repo (`scripts/` e infra del repo, PROJECT-README §5, §11). Es el nivel de IaC que el alcance v1 del proyecto requiere: la infraestructura objetivo son tres contenedores Linux fijos (arquitectura-solucion §5), no un clúster orquestado (microservicios descartados, arquitectura-solucion §2).
- Layout: un archivo de composición base más un override por ambiente (`compose.dev`, `compose.qa`, `compose.staging`, `compose.prod`) que parametriza variables y secretos sin duplicar la topología.
- Política de cambios de infraestructura: todo cambio del compose entra por PR a `main` (revisión aprobada, GitHub Flow). El equivalente al "plan antes de apply" es la revisión del diff del compose en el PR y la validación en DEV antes de promover. Una eventual migración a un orquestador (Kubernetes, Helm) requeriría un ADR, ya que cambia el tipo de despliegue declarado.

## 3. Configuración por ambiente (12-factor)

La configuración vive en variables de entorno o archivos referenciados, nunca en código (12-factor; arquitectura-solucion §7 "Configuración y secretos"). En desarrollo se usa `.env` fuera de git con `.env.example` versionado (PROJECT-README §8).

Mapa de variables por ambiente (no exhaustivo; los valores sensibles van por secretos, §4):

| Variable | DEV | QA | STAGING | PROD | Origen |
| --- | --- | --- | --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Development | Staging | Staging | Production | runtime |
| `ConnectionStrings__Sql` | SQL Server local/contenedor DEV | SQL QA | SQL STAGING | SQL PROD | secreto |
| `FileHosting__Backend` | local | local | local | local o S3 (configurable por usuario raíz, ADR-08) | config |
| `FileHosting__LocalPath` | volumen del contenedor backend | ídem | ídem | ídem | config |
| `Jwt__Issuer` / `Jwt__Audience` | valores DEV | QA | STAGING | PROD | config |
| `Jwt__SigningKey` | — (secreto) | secreto | secreto | secreto | secreto |
| `Auditoria__RetencionMeses` | 12 | 12 | 12 | 12 | config (Ley 25.326, RN-07) |
| `Logging__LogLevel__Default` | Debug | Information | Information | Warning | config |

El almacenamiento local de fotos reside sobre el contenedor del backend en todos los ambientes salvo que el usuario raíz configure un backend externo S3 u otro (ADR-08, extensibilidad). La retención de auditoría se fija en 12 meses en todos los ambientes por compliance (Ley 25.326, ADR-14).

## 4. Secretos

- Gestor: **GitHub Secrets** para los secretos de CI/CD (PROJECT-README §8); en producción, el **secret store gestionado del entorno de deploy** del organismo (arquitectura-solucion §7).
- Desarrollo: `.env` fuera de git con `.env.example` versionado (solo claves, sin valores). El `.gitignore` excluye `.env`. Prohibido el commit de secretos (anti-patrón "secretos en commit", §4.8); el pipeline incluye scan de secretos en commits (supply-chain-seguridad §5).
- Rotación: **90 días** para los secretos de CI (PROJECT-README §8). Los tokens de aplicación siguen su propia política: access token de vida corta (≈60 min) + refresh token, con el refresh en móvil condicionado al método de seguridad del teléfono (PROJECT-README §8, ADR-03).
- Scopes mínimos: el token de publicación a GitHub Packages usa `write:packages` / `read:packages`; el de publicación de imágenes usa el scope de escritura del registry (detalle en las guías de publicación). Sin tokens con scope amplio innecesario.
- Auditoría: el acceso a secretos y a datos personales queda registrado de forma inmutable con retención ≥ 1 año (Ley 25.326, RN-07, RN-08, ADR-14).

## 5. Promoción

### 5.1 Promoción entre ambientes (monolito)

Integrada con el pipeline (pipeline-ci-cd §4.1). Cada transición tiene trigger, prerequisitos y aprobador, con registro auditable:

- DEV: automática en merge a `main`.
- DEV → QA: tag `-rc.N`; aprobador QA lead; prerequisito DoD de sprint (definition-of-done §1.3).
- QA → STAGING: aprobación manual del Release manager; prerequisito NFR de integración verdes.
- STAGING → PROD: tag estable `v<X.Y.Z>` + aprobación del Release manager y de negocio (jefe general); prerequisito DoD de release completa (definition-of-done §1.4) y criterios-validacion §2–§5. La aprobación a PROD es un gate humano explícito (anti-patrón "promotion sin aprobador humano para PROD", §4.8) y queda registrada en auditoría (≥ 1 año, Ley 25.326).

### 5.2 Promoción entre canales (paquete)

- preview: automática desde tags prerelease.
- preview → stable: aprobación del Release manager; prerequisito contract tests verdes (TC-26) y sin breaking change no señalado por MAJOR (estrategia-versionado §5, §6). Detalle en `guia-publicacion-paquete-github-packages_v1.0.md`.

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Entornos y canales iniciales de GeoVial: ambientes DEV/QA/STAGING/PROD para el monolito de tres contenedores con aprobador y SLA/ventana por ambiente, y canales preview/stable para el paquete de la librería de sync, tratados como planos independientes. IaC con Docker Compose por ambiente, configuración 12-factor, secretos en GitHub Secrets con rotación a 90 días y promoción con aprobador y registro auditable. Generado por AG-09 |
