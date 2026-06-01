# Estrategia de versionado — GeoVial

**Proyecto:** GeoVial
**Documento:** estrategia-versionado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Ingeniero DevOps Senior (AG-09), Equipo SDD 2.0
**Trazabilidad upstream:** PROJECT-README §10 (SemVer + Conventional Commits + MinVer + GitHub Flow); 05 (ADR-07, contratos-abstractions-sync §6); 08 (definition-of-done §1.4)
**Trazabilidad downstream:** pipeline-ci-cd_v1.0.md (calcula la versión y promueve por canal/ambiente); entornos-deploy_v1.0.md (canales preview/stable); guías de publicación; 10_developer_guide

Este documento es la bisagra entre el código (Conventional Commits, branching) y el artefacto publicado (SemVer, canales, deprecation). Lo consumen tanto los autores del repositorio como los consumidores de la librería de sincronización.

## 1. SemVer 2.0.0

Formato adoptado: `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILDMETADATA]`, conforme a SemVer 2.0.0 (PROJECT-README §10).

Reglas de incremento:

- MAJOR: cambio incompatible en una superficie pública versionada. Para la librería `GeoVial.Sync`, cualquier breaking change de la API pública de Abstractions (firma alterada, tipo público removido o cambiado) bumpea MAJOR (ADR-07, contratos-abstractions-sync §6). Para el monolito, un cambio incompatible del contrato REST `/api/v1/` bumpea MAJOR (el contrato se versiona además por URL, PROJECT-README §6).
- MINOR: funcionalidad retrocompatible. Para la librería, un método nuevo con default o un tipo nuevo. Para el monolito, un endpoint nuevo o un campo opcional.
- PATCH: corrección retrocompatible de comportamiento existente.
- PRERELEASE: sufijos `-alpha.N`, `-beta.N`, `-rc.N` para el canal preview.
- BUILDMETADATA: opcional, `+<sha-corto>` para trazabilidad; no afecta precedencia.

El monolito GeoVial y la librería `GeoVial.Sync` comparten el mismo esquema SemVer pero son líneas de versión independientes: el bump de uno no obliga al del otro.

## 2. Conventional Commits 1.0.0

Convención de mensajes de PROJECT-README §10, sin excepciones. Prefijos semánticos: `feat`, `fix`, `chore`, `docs`, `refactor`, `perf`, `test`, `build`, `ci`, `style`. El bump SemVer se deriva automáticamente del historial de commits desde el último tag.

| Tipo de cambio en Conventional Commits | Bump SemVer | Ejemplo |
| --- | --- | --- |
| `feat` | MINOR | `feat(sync): agregar política de reintento configurable a SyncOptions` → 1.2.3 → 1.3.0 |
| `fix` | PATCH | `fix(observaciones): corregir prioridad de metadatos cuando el EXIF es parcial` → 1.2.3 → 1.2.4 |
| `feat!` o `BREAKING CHANGE` en footer | MAJOR | `feat!(sync): renombrar ISyncEngine.RunAsync a SynchronizeAsync` → 1.2.3 → 2.0.0 |
| `refactor`, `perf`, `test`, `chore`, `docs`, `style`, `build`, `ci` | Ninguno | `refactor(infrastructure): extraer mapper de EF Core` → 1.2.3 → 1.2.3 |

El marcador `!` después del tipo/scope o el footer `BREAKING CHANGE:` son las dos formas admitidas de declarar un cambio mayor. El CHANGELOG (Keep a Changelog) se genera automáticamente desde estos commits y se publica en el release, evitando el anti-patrón "CHANGELOG ausente o no mantenido" (§4.8).

## 3. Herramienta de versionado

Herramienta: **MinVer** (o, como alternativa equivalente del runtime, Nerdbank.GitVersioning), declarada en PROJECT-README §10 y ADR-07.

- Cálculo: la versión se deriva de los tags Git con prefijo `v` (por ejemplo `v1.4.0`). MinVer recorre desde el commit actual hasta el último tag de versión y, en función de los Conventional Commits intermedios, calcula la versión efectiva del artefacto.
- Prefijo de tag: `v`. Configuración base en el proyecto: `MinVerTagPrefix=v`.
- Prerelease automático: los commits posteriores al último tag estable producen versiones prerelease (canal preview); el tag sin sufijo materializa el canal stable.
- Versión inicial: `1.0.0` para el monolito y para la librería de sincronización en su primer release estable.
- El versionado es automático: no se editan números de versión a mano, evitando el anti-patrón "versionado manual" (§4.8). La herramienta corre en STAGE-11 y STAGE-12 del pipeline (pipeline-ci-cd §1).

## 4. Branching

Modelo: **GitHub Flow** (PROJECT-README §10), alineado al acuerdo de equipo (00) y a la cadencia de sprints de 07.

- Rama principal `main` protegida: prohibido el push directo; todo cambio entra por Pull Request.
- Ramas de feature cortas `feature/<slug>` que se integran a `main` por PR.
- Reglas de protección de `main`: al menos una aprobación de revisión; el pipeline de CI debe estar verde (lint, build, tests, cobertura, SCA) antes del merge (DoD US: "el cambio se integró por PR a la rama protegida con revisión aprobada", definition-of-done §1.1).
- Merge por squash con un mensaje Conventional Commits que define el bump del próximo release.
- Los tags de release (`v<X.Y.Z>` y prerelease) se crean sobre `main` y disparan los stages de publicación/promoción.

## 5. Canales

GeoVial distingue dos planos. Los canales aplican al paquete de la librería; los ambientes (DEV/QA/STAGING/PROD) aplican al monolito desplegable y se documentan en `entornos-deploy_v1.0.md` (no son canales).

Canales del paquete `GeoVial.Sync` en GitHub Packages (ADR-07):

| Canal | Tags que lo alimentan | Semántica del sufijo | Promoción |
| --- | --- | --- | --- |
| preview (prerelease) | `v<X.Y.Z>-alpha.N`, `-beta.N`, `-rc.N` | `-alpha`: superficie inestable, en evolución; `-beta`: API congelada en evaluación; `-rc`: candidato a estable | Automática desde el tag prerelease |
| stable | `v<X.Y.Z>` sin sufijo | release de producción; API pública garantizada por SemVer | Aprobada por Release manager; requiere contract tests verdes (TC-26) y sin breaking change no señalado por MAJOR |

No se define canal LTS en v1; la librería tiene una única línea estable. Un eventual LTS se incorporaría con un ADR.

## 6. Deprecation policy

Aplica principalmente a la superficie pública de la librería `GeoVial.Sync` y, de forma análoga, al contrato REST `/api/v1/`.

- Anuncio: todo elemento a deprecar se marca en código como obsoleto (`[Obsolete]` con mensaje y versión de remoción prevista) y se anuncia en el CHANGELOG del release que introduce la deprecación.
- Ventana de gracia: un elemento deprecado de la API pública de la librería vive al menos dos MINOR antes de removerse; la remoción es un cambio MAJOR.
- Comunicación al consumidor: el CHANGELOG (Keep a Changelog) lista la deprecación bajo `Deprecated` y la remoción bajo `Removed`, con la versión de migración. Para el contrato REST, los endpoints deprecados exponen los headers `Deprecation` y `Sunset` en la respuesta (coherente con el versionado por URL de PROJECT-README §6).
- Breaking change: todo breaking change de la API pública de la librería bumpea MAJOR y se acompaña de una guía de migración en las release notes (ADR-07; riesgo RA-07 de arquitectura-solucion §9). El pipeline detecta el breaking change vía la herramienta de versionado y los contract tests (TC-26), conforme a la DoD de release (definition-of-done §1.4).

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Estrategia de versionado inicial de GeoVial: SemVer 2.0.0, Conventional Commits 1.0.0 con tabla de bump, MinVer/Nerdbank.GitVersioning como herramienta, GitHub Flow con `main` protegida y PR, canales preview/stable de la librería de sync (breaking de Abstractions → MAJOR) y deprecation policy con dos MINOR de gracia. Generada por AG-09 |
