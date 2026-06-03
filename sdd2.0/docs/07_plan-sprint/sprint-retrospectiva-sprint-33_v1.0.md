# Sprint Retrospectiva — Sprint 33

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-33_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Dependabot quedó configurado para los tres ecosistemas que GitHub entiende (NuGet/Actions/Docker) con poco esfuerzo y agrupación de minor/patch para no inundar de PRs.
- La lección más cara del proyecto (S29: cuatro bugs latentes en el primer release) quedó convertida en un checklist obligatorio, no en memoria tribal.
- Fuimos honestos sobre el límite real de Dependabot (no cubre Leaflet hand-vendored) en vez de asumir cobertura total.
- Cerró tres acciones reiteradas (Dependabot de S31/S32, tag preview de S29) en un solo sprint acotado.

## 2. Qué no salió bien

- `dependabot.yml` no se puede validar de punta a punta localmente; la confirmación real (que abra PRs, que el esquema sea válido) ocurre recién al hacer push y observarlo en GitHub.
- La validación con `-rc` quedó como checklist (proceso), no como gate automático: depende de que el Release manager lo siga; un check de CI que impida taggear stable sin un `-rc` previo sería más fuerte.
- El seguimiento de Leaflet sigue siendo manual; sin una alerta, puede quedar atrás en parches.

## 3. Qué probar

- Tras el primer ciclo, revisar la pestaña Dependabot: que abra los PRs esperados y ajustar agrupación/cadencia según el ruido real.
- Evaluar un check de CI que falle si se taggea un stable sin un `-rc` verde previo (automatizar la regla del checklist).
- Sumar un recordatorio/issue periódico para revisar la versión de Leaflet vendorizada.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Revisar el primer lote de PRs de Dependabot y ajustar agrupación/cadencia | AG-09 (DevOps) | 2027-10-02 | Pendiente |
| Evaluar automatizar la regla `-rc` antes de stable como check de CI | AG-09 (DevOps) | 2027-10-02 | Pendiente |
| Mejoras del mapa: Service Worker/caché de teselas (offline) y/o bundle de Leaflet en el móvil | AG-08 (móvil) | 2027-10-02 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 32 | Estado actual |
| --- | --- |
| Sumar las libs vendorizadas/CDN del mapa al seguimiento de versiones/seguridad | Parcial: Dependabot cubre NuGet/Actions/Docker; Leaflet (JS hand-vendored) queda con seguimiento manual documentado |
| Caché/Service Worker de teselas (offline) y bundle de Leaflet en el móvil | Pendiente (se reitera) |
| Mantenimiento post-release: atender alertas Dependabot/CVE por SLA | En curso (Dependabot ya configurado para abrir los PRs) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 33 (mantenimiento supply-chain): Dependabot configurado y validación `-rc` institucionalizada; 3 acciones nuevas y seguimiento de las de S32. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
