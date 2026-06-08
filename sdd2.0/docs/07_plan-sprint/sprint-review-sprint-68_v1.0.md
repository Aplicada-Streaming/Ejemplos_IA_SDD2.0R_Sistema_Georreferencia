# Sprint Review — Sprint 68

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-68_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-68_v1.0.md`:

> Que el backend no pueda arrancar inseguro en producción y que exponga su salud para orquestación.

Veredicto: Cumplido.

Explicación corta: primer sprint de la épica **Hardening de producción**, de **alcance pleno (8 SP)** — rompe la racha de nueve sprints acotados. **Guardrails de configuración:** el núcleo `GuardrailsProduccion` (gate) valida al arrancar y, en `Production`, **falla rápido** si la base sería en memoria (sin cadena de conexión) o si la clave JWT es la de desarrollo (pública) o muy corta; en Development/tests es permisivo. **Health checks:** `/health` (liveness, el proceso responde) y `/health/ready` (readiness, la base es alcanzable vía `ChequeoBaseDeDatos`), anónimos, para sondas de contenedor/balanceador. `appsettings.Production.json` queda **sin secretos**: la cadena y la clave se proveen por entorno/secret manager, y el arranque las exige.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| HARD-CONFIG | Seguridad | En producción sin cadena/clave segura, el arranque falla rápido (test verde) | No corre inseguro |
| HARD-CONFIG | Compatibilidad | Development arranca igual que antes (login `campo1` → 200) | Sin regresión |
| HARD-HEALTH | Operación | `/health` → Healthy 200; `/health/ready` → Healthy 200 (base SQL alcanzable) | Listo para orquestación |

## 3. Feedback recibido

- El fail-fast cierra el agujero de correr en producción con la base en memoria o la clave de desarrollo: errores típicos de despliegue que ahora se cazan al arrancar.
- Los health checks habilitan sondas de liveness/readiness para contenedores y balanceadores.
- Mantener los secretos fuera del repo (sólo por entorno) es la base del resto del hardening.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados | 0 |

Pruebas: **510** (463 unitarias + 47 de integración), **+12**: 9 unitarias de `GuardrailsProduccion` + 3 de integración (`SaludEArranqueTests`: liveness, readiness, fail-fast en producción). Cobertura DoD sin regresión (núcleo nuevo cubierto en `GeoVial.Application`). Verificado en vivo: `/health` y `/health/ready` Healthy contra SQL Server real; Development arranca y loguea bien.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| HARD-CONFIG | Historia | Aceptada (guardrails fail-fast) |
| HARD-HEALTH | Historia | Aceptada (liveness + readiness) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| — | 0 | Sin traslados. |

**Épica Hardening de producción — arrancada.** Sprint 1 de N: config segura + health. Próximos candidatos: observabilidad (logging estructurado + correlación de request), endurecimiento de errores (handler global), HTTPS/headers de seguridad, contenedor productivo, rate limiting.

## 7. Decisiones tomadas

- El guardrail es estricto **sólo** en `Production` (Development/Staging/tests permisivos): no rompe el flujo de desarrollo ni el gate, pero blinda el despliegue real.
- Liveness y readiness separados: `/health` no chequea dependencias (el proceso vive); `/health/ready` sí (la base). Permite sondas distintas.
- `appsettings.Production.json` sin secretos: la cadena de conexión y la clave JWT vienen por variable de entorno / secret manager (`ConnectionStrings__GeoVial`, `Jwt__ClaveSecreta`).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Review del Sprint 68 (hardening de producción #1: guardrails fail-fast + health checks). Cumplido, **velocity 8** (alcance pleno, rompe la racha de acotados), 0 carry-over, 510 pruebas (+12). Arranca la épica de hardening. Generado por AG-07 |
