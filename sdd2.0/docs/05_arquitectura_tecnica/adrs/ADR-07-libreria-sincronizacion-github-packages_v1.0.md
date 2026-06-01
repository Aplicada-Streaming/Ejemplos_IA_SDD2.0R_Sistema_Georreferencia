# ADR-07 — Librería de sincronización como paquete en GitHub Packages

**Proyecto:** GeoVial
**Documento:** ADR-07-libreria-sincronizacion-github-packages_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Extensibilidad

## 1. Contexto

La lógica de sincronización offline-first (subir cambios locales, bajar actualizaciones, consolidar last-write-wins, marcar conflictos) es reutilizable más allá de GeoVial. El cliente requiere que la librería de sincronización se publique como paquete para evaluarla y reutilizarla en otros proyectos, e incluya una demo MAUI autónoma (PROJECT-README §1, §10, §14). Esto exige una superficie pública estable y versionada. Motivan: CU-06, CU-07; RN-04.

## 2. Decisión

Se construye `GeoVial.Sync` como librería independiente con una superficie pública estable (capa Abstractions) y se publica en GitHub Packages con canales preview (prerelease) y stable. El versionado sigue SemVer 2.0.0: cualquier breaking change de la API pública bumpea MAJOR. La librería incluye una demo MAUI autónoma (`samples/02-sync-maui-demo`) ajena al sistema para evaluar el reuso.

## 3. Estado

Aceptado el 2026-06-01. Renumeración de `ADR-007` (PROJECT-README §15, decidido por el cliente) a `ADR-07`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Librería publicada en GitHub Packages con Abstractions estable (elegido) | Reuso en otros proyectos; versionado SemVer; demo autónoma | Disciplina de compatibilidad de la API pública; costo de publicación |
| Código de sincronización embebido en el monolito/móvil sin publicar | Más simple a corto plazo | Imposible reutilizarlo; incumple el requisito explícito del cliente |
| Servicio de sincronización separado | Aislamiento | Sobredimensionado; el cliente pide una librería, no un servicio |

## 5. Consecuencias positivas

1. La sincronización se reutiliza en otros proyectos del organismo (requisito del cliente).
2. La capa Abstractions fija un contrato estable y versionado (contratos-abstractions-sync).
3. La demo MAUI autónoma permite evaluar la librería sin el resto del sistema.

## 6. Consecuencias negativas y trade-offs

1. Mantener compatibilidad de la API pública impone disciplina; un breaking change obliga a MAJOR.
2. La publicación agrega un stage de empaquetado al pipeline (PROJECT-README §11).

## 7. Implementación

`GeoVial.Sync` expone su superficie pública en Abstractions, documentada en `contratos-abstractions-sync_v1.0.md`. Se publica en GitHub Packages con MinVer/Nerdbank.GitVersioning calculando la versión desde Conventional Commits (PROJECT-README §10). El stage 4 del pipeline empaqueta la librería. La demo vive en `samples/02-sync-maui-demo` (referencia a 11).

## 8. Métricas de validación

- Paquete publicado en GitHub Packages con canal preview y stable.
- Un breaking change detectado bumpea MAJOR (verificable en CI por la herramienta de versionado).
- La demo MAUI autónoma sincroniza contra un mock server (alta local, sync, estado de cola, resolución básica).

## 9. Referencias

- PROJECT-README §1, §10, §14.
- CU-06, CU-07; RN-04.
- `contratos-abstractions-sync_v1.0.md`, ADR-05, ADR-06.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Decisión aceptada. Renumeración de ADR-007 a ADR-07 |
