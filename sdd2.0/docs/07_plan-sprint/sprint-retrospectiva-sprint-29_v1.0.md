# Sprint Retrospectiva — Sprint 29

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-29_v1.0.md
**Versión:** 1.1
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- `v1.0.0` stable quedó **publicado y firmado**: paquete al canal stable de GitHub Packages (firma + SBOM verificados en el pipeline) y las tres imágenes a GHCR (`:latest`, firmadas + SBOM atestado).
- Validar con tags preview `rc.1`–`rc.4` fue la decisión clave: encontró y permitió corregir **cuatro** bugs de pipeline sin tocar el stable. Cada rc dio una señal concreta (log persistido) para el fix siguiente.
- Diagnósticos sólidos: NU5026 reproducido localmente borrando `bin/obj`; el cuelgue de firma aislado comparando paquete (1 job, OK) vs imágenes (3 jobs concurrentes, cuelgue) desde el log del paquete.
- Los fixes quedaron quirúrgicos (workflows/script; el `.csproj` no cambió, el gate sigue verde) y dejan el pipeline reproducible para próximos releases.

## 2. Qué no salió bien

- **Cuatro** bugs de pipeline llegaron al release porque **ningún tag había ejercitado nunca los workflows** (el repo no tenía tags): ambos corrieron por primera vez en `v1.0.0`.
- NU5026 (pack) y el flag del SBOM (`-j`→`-F Json`) son fallos que un tag preview rutinario habría detectado mucho antes.
- La firma cosign keyless se colgó de entrada y el `timeout-minutes` del runner **no** mató el proceso (espera de red): un cuelgue puede no fallar solo; hizo falta serializar la firma para resolver la causa real (saturación de sigstore por concurrencia).
- El release tomó cinco corridas (1 stable fallida + 4 rc) hasta quedar verde: mucho ida y vuelta que un preview rutinario en cada sprint de pipeline habría amortizado.

## 3. Qué probar

- Institucionalizar el tag preview `-rc.N` como paso obligatorio antes de todo stable (y ante cualquier cambio de workflow de publicación): es la única forma de ejercitar un workflow que sólo corre en tag.
- Para firmas keyless contra el sigstore public-good, firmar **en serie** (no en matriz paralela) y acotar las ceremonias keyless por step (sign+attest en pipeline; verify en promoción/consumo).
- Para "pasa local / falla en CI", reproducir siempre sobre un árbol limpio (borrar `bin/obj`) antes de concluir.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Resolver el cuelgue de la firma cosign keyless de imágenes | AG-09 (DevOps) | 2027-07-24 | Completada (causa: 3 firmas concurrentes saturan sigstore → `max-parallel: 1`) |
| Publicar `v1.0.0` stable verificando paquete + imágenes (firma + SBOM) | AG-09 (Release manager) | 2027-07-24 | Completada (publicado tras validar con `rc.1`–`rc.4`) |
| Institucionalizar la validación con tag preview `-rc` antes de cada stable | AG-09 (DevOps) | 2027-08-07 | Pendiente (aplicado en este sprint; queda formalizarlo en la guía/checklist) |
| Gestionar la clave de proveedor de mapas para habilitar el mapa interactivo | AG-08 (móvil) | 2027-08-07 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 28 | Estado actual |
| --- | --- |
| Publicar v1.0.0 (tag) y validar paquete + imágenes (firma + SBOM) post-publish | En curso (tag creado; imágenes publicadas; paquete re-disparado tras corregir NU5026) |
| Gestionar la clave de proveedor de mapas para el mapa interactivo | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva inicial del Sprint 29 (release v1.0.0) con el estado parcial del primer disparo. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
| 1.1 | 2026-06-03 | Actualizada al cierre: `v1.0.0` publicado y firmado tras validar el pipeline con `rc.1`–`rc.4` (cuatro bugs de pipeline corregidos); acciones de cuelgue de firma y publicación completadas. Por AG-07 |
