# Supply chain y seguridad — GeoVial

**Proyecto:** GeoVial
**Documento:** supply-chain-seguridad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0
**Trazabilidad upstream:** 05 (arquitectura-solucion §7 configuración/secretos, §8 NFR, ADR-14 compliance Ley 25.326, RA-08); PROJECT-README §2 (licencias), §8 (secretos/auditoría); 08 (criterios-validacion §5)
**Trazabilidad downstream:** pipeline-ci-cd_v1.0.md (stages SCA/SBOM/firma); guías de publicación

Política de cadena de suministro de GeoVial alineada a SLSA, NIST SSDF (SP 800-218) y OWASP SCVS. Cubre los dos tipos de artefacto del proyecto: imágenes Docker del monolito (front/backend/db) y el paquete de la librería de sincronización en GitHub Packages. Refuerza el compliance de la Ley 25.326 (datos personales, auditoría ≥ 1 año) donde aplica.

## 1. SBOM

- Formato: **CycloneDX** (JSON).
- Generador: herramienta CycloneDX para .NET (genera el SBOM del paquete y de cada ensamblado) y generador de SBOM de imagen de contenedor para las tres imágenes Docker.
- Cobertura: un SBOM por artefacto publicado — uno por imagen (front, backend, db) y uno por el paquete `GeoVial.Sync`.
- Generación y publicación: automática en STAGE-09 del pipeline (pipeline-ci-cd §1). El SBOM se adjunta al release de GitHub de forma permanente (anti-patrón "falta de SBOM", §4.8).
- Firma del propio SBOM: el SBOM se firma con cosign junto al artefacto (STAGE-10), de modo que el consumidor pueda verificar su integridad.

## 2. Firma

- Herramienta: **cosign (sigstore)** en modo keyless con el OIDC de GitHub Actions; no se gestionan llaves privadas de larga vida.
- Alcance: se firman las tres imágenes Docker y el paquete de la librería, más sus SBOM.
- Transparency log: la firma se registra en el transparency log público de sigstore (Rekor); el bundle de verificación se adjunta al release.
- Verificación por consumidores: documentada en las guías de publicación (`cosign verify ...`); la verificación de firma es prerrequisito para promover entre ambientes (imágenes) y para declarar el paquete consumible (anti-patrón "falta de firma del artefacto", §4.8). La firma corre en el stage final, antes del publish (STAGE-10 → STAGE-13/14).

## 3. SLSA

- Nivel objetivo v1: **SLSA Build L2**. Se cumple porque el build corre en una plataforma hosteada (GitHub Actions), genera procedencia (provenance) firmada por artefacto y los artefactos se firman y adjuntan al release. La procedencia liga cada artefacto al commit, al workflow y al runner que lo produjo.
- Criterios cumplidos en L2: build con servicio gestionado; procedencia generada automáticamente y firmada; artefactos firmados (cosign).
- Plan de elevación a L3: aislar el build con runners reforzados y procedencia no falsificable por el propio job (separación entre el paso que construye y el que firma), y restringir los secretos de firma al paso de firma. Se evalúa post-v1 mediante un ADR cuando el organismo lo requiera.

## 4. Dependency scanning

- Tooling de SCA: **Dependabot** para actualización y alerta de dependencias del repositorio (NuGet, imágenes base de Docker, acciones de GitHub Actions), complementado por el escáner de SCA del pipeline (STAGE-08).
- Frecuencia: en cada PR y push (STAGE-08), más el escaneo programado de Dependabot sobre `main` y un escaneo periódico de las imágenes ya publicadas.
- Política de licencias: las dependencias deben ser MIT/Apache/BSD; sin GPL en componentes distribuidos (PROJECT-README §2; criterios-validacion §5). El escaneo verifica la conformidad de licencias además de las CVE.
- Política por severidad de vulnerabilidad de dependencia (gate de STAGE-08):

| Severidad | Acción en el pipeline |
| --- | --- |
| Crítica | Bloquea el PR y el release; remediación inmediata (ver SLA §6) |
| Alta | Bloquea el release salvo excepción registrada por ADR con plan de remediación |
| Media | No bloquea; se agenda como BT en el backlog de 06 |
| Baja | Seguimiento; se atiende en mantenimiento programado |

## 5. SAST y DAST

- SAST: análisis estático con los analizadores Roslyn y un motor de SAST de código (por ejemplo CodeQL sobre el repo GitHub) en STAGE-01 (lint/análisis). Criterio de bloqueo de PR: 0 hallazgos nuevos de severidad alta/crítica respecto de la línea base; 0 warnings nuevos del análisis estático (criterios-validacion §5). Incluye scan de secretos en commits para impedir el commit de credenciales (anti-patrón "secretos en commit", §4.8; entornos-deploy §4).
- DAST: análisis dinámico sobre el backend desplegado en QA/STAGING (entorno equivalente al productivo), ejercitando la API REST `/api/v1/` autenticada con ROPC/JWT. Verifica controles de autorización por rol y área (RN-01, RN-08) y manejo de errores Problem Details (ADR-11). Criterio de bloqueo: 0 hallazgos de severidad alta/crítica sin excepción registrada antes de promover a PROD.
- Compliance: tanto SAST como DAST prestan atención especial al acceso a datos personales (Ley 25.326): el DAST verifica que el acceso no autorizado a datos personales se rechace y quede auditado (RN-08, ADR-14).

## 6. Política de CVE

SLA de remediación por severidad, aplicable a vulnerabilidades en dependencias, imágenes y código propio:

| Severidad | SLA de remediación | Comunicación al consumidor |
| --- | --- | --- |
| Crítica | ≤ 48 h hasta fix publicado o mitigación | Aviso inmediato en release notes y CHANGELOG; delist de la versión afectada del paquete si aplica (guia-publicacion-paquete §4) |
| Alta | ≤ 7 días | Nota en CHANGELOG del siguiente release; PATCH dedicado si bloquea |
| Media | ≤ 30 días | CHANGELOG del release planificado |
| Baja | ≤ 90 días o mantenimiento programado | CHANGELOG |

- Ventana entre detección y publicación de fix: para CVE crítica de una dependencia del paquete publicado, se prioriza el PATCH y el delist de la versión afectada; para el monolito, se prioriza el re-deploy de la versión corregida o el rollback (pipeline-ci-cd §5).
- Auditoría y compliance Ley 25.326: toda remediación que afecte el acceso o la retención de datos personales queda registrada en auditoría inmutable con retención ≥ 1 año (RN-07, ADR-14). El riesgo RA-08 de arquitectura-solucion §9 (incumplimiento de la Ley 25.326) se mitiga con autorización por rol/área, auditoría inmutable ≥ 12 meses y minimización de datos; esta política de CVE es parte de esa mitigación en la capa de supply chain.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Política de supply chain inicial de GeoVial: SBOM CycloneDX por artefacto firmado, firma cosign keyless con transparency log, SLSA Build L2 objetivo con plan de elevación a L3, dependency scanning con Dependabot y política por severidad, SAST/DAST con criterios de bloqueo y política de CVE con SLA por severidad. Refuerzo de compliance Ley 25.326 (auditoría ≥ 1 año, datos personales). Generada por AG-09 |
| 1.1 | 2026-06-02 | §2 (firma) implementada para el paquete de la librería `GeoVial.Sync` (Sprint 22): el workflow `publish-sync.yml` firma el `.nupkg` con cosign keyless (OIDC de Actions, `id-token: write`), lo verifica con `cosign verify-blob` antes de publicar y adjunta el bundle de verificación. La firma de las imágenes Docker y los SBOM sigue pendiente de los stages de imagen. Por AG-09 |
