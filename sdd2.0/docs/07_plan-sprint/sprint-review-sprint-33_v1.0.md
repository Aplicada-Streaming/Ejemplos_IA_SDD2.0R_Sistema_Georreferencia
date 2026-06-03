# Sprint Review — Sprint 33

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-33_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-33_v1.0.md`:

> Endurecer el mantenimiento post-release de la cadena de suministro: (1) configurar **Dependabot** para que actualice y alerte automáticamente las dependencias del repo —NuGet, GitHub Actions e imágenes base de Docker—; y (2) **institucionalizar** la validación con un tag preview `-rc` antes de cada release stable.

Veredicto: Cumplido.

Explicación corta: se agregó `.github/dependabot.yml` con tres ecosistemas —`nuget` (raíz, agrupando minor/patch para reducir ruido), `github-actions` y `docker` (los tres Dockerfiles: `src/GeoVial.Api`, `src/GeoVial.Web`, `infra/db`)— en cadencia semanal, con límite de PRs y etiquetas. Se documentó explícitamente que Leaflet (vendorizado/CDN) no lo cubre Dependabot y se sigue a mano, para no dar falsa sensación de cobertura. Se creó `09_devops/checklist-release`, un checklist de release genérico que **exige validar el pipeline con un tag preview `-rc` antes del stable**, convirtiendo en regla escrita la lección del Sprint 29 (cuatro bugs latentes que sólo aparecieron al disparar un tag). Y se actualizó `supply-chain-seguridad` a v1.4 con §4 (dependency scanning) implementado. Sin cambios de backend ni dominio; el gate se mantiene.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| EP-09 | Supply-chain | `dependabot.yml` declara NuGet + Actions + Docker (×3) con cadencia semanal | Dependencias vigiladas automáticamente |
| EP-09 | Proceso | Checklist de release con validación `-rc` obligatoria antes del stable | Releases sin sorpresas, regla escrita |
| EP-09 | Documentación | `supply-chain-seguridad §4` implementado; nota del seguimiento manual de Leaflet | Cobertura real, sin falsos supuestos |

## 3. Feedback recibido

- Dependabot cierra el anti-patrón de dependencias sin vigilancia para los ecosistemas que GitHub entiende (NuGet/Actions/Docker).
- Convertir la lección de S29 en un checklist obligatorio evita repetir el aprendizaje doloroso en el próximo release.
- Ser explícito sobre lo que Dependabot **no** cubre (Leaflet hand-vendored) evita una falsa sensación de seguridad.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 325 verdes (288 unitarias + 37 de integración), sin cambio respecto del Sprint 32: sprint de DevOps/proceso sin lógica de dominio nueva, por lo que el gate de cobertura se mantiene. Verificación específica: estructura del `dependabot.yml` según el esquema de Dependabot (GitHub lo valida al hacer push y reporta en la pestaña Dependabot) y suite .NET verde. Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-SUPPLY-MANT | Tarea | Aceptada (Dependabot configurado para NuGet/Actions/Docker; checklist de release con `-rc`; supply-chain §4 a v1.4) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 33 se traslada. |

Backlog restante: mejoras opcionales del mapa (Service Worker/caché de teselas para offline real, bundle de Leaflet en el móvil) y mantenimiento continuo (atender los PRs/alertas que abra Dependabot por SLA de CVE).

## 7. Decisiones tomadas durante el review

- Agrupar minor/patch de NuGet en un PR para reducir el ruido de Dependabot; los major van por separado.
- Hacer del tag preview `-rc` un paso **obligatorio** del checklist de release (no opcional), por la lección de S29.
- Documentar el seguimiento manual de Leaflet en lugar de fingir cobertura de Dependabot.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Sprint review del Sprint 33 (mantenimiento supply-chain: Dependabot + checklist de release con `-rc`). Veredicto Cumplido, velocity 8, 0 carry-over, 325 pruebas verdes. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
