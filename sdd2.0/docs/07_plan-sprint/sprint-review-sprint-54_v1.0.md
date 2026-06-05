# Sprint Review — Sprint 54

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-54_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-54_v1.0.md`:

> La app se "resetea" al volver de la cámara […] hay que persistir la sesión para que sobreviva a la recreación del proceso. […] Falta un "ojito" para mostrar/ocultar la clave.

Veredicto: Cumplido.

Explicación corta: en la verificación on-device se vio que, al volver de la cámara, la app rebotaba al login porque Android mata el proceso y la sesión vivía sólo en memoria. Se agregó **persistencia de sesión** (`IAlmacenTokenSesion` + `ServicioSesion.RestaurarAsync`, implementado con SecureStorage), restaurada al arrancar: ahora la app vuelve a las solapas. Se sumó el **"ojito"** en el login y, durante la sesión, se corrigió el **biométrico** para aceptar **patrón/PIN** del dispositivo (no sólo huella), más fiel a RN-06.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| BUG-CAMARA-RESET | Bug fix | Tras recrear el proceso, la app restaura la sesión y abre las solapas (no el login) | Ya no expulsa al agente al sacar una foto |
| US-LOGIN-OJITO | Funcionalidad | Botón 👁/🙈 que muestra/oculta la clave | Menos errores de tipeo en el teléfono |
| BUG-BIO-PATRON | Bug fix | El reingreso acepta el **patrón** del teléfono (este equipo no tiene huella enrolada) | RN-06 funciona con el método real del dispositivo |

## 3. Feedback recibido

- La sesión de verificación on-device cumplió su propósito: sacó a la luz dos defectos reales (reseteo por la cámara y biométrico que no aceptaba patrón) que el gate no podía ver.
- La persistencia se diseñó con un núcleo testeable (`IAlmacenTokenSesion` + restauración) y SecureStorage como implementación; los tests previos no se tocaron (parámetro opcional).
- Honesto: el objetivo es **no perder la sesión** (no rebotar al login). Si en un kill por memoria muy agresivo la **foto en vuelo** se pierde, re-tomarla funciona y el agente sigue logueado; persistir la captura en curso queda fuera de alcance.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 1 (build: `BiometricManager.Authenticators`/`SetDeviceCredentialAllowed` por API — resuelto con valores de plataforma y piso API 29) |

Pruebas: **437** (394 unitarias + 43 de integración), +3 unitarias en `ServicioSesionTests`: la sesión persistida se restaura tras recrear el proceso; cerrar sesión limpia la persistencia; sin almacén no rompe (compatibilidad). Cobertura del gate: Domain 88,5 % / 80,5 %; Application 87,6 % / 76,3 % (umbral 80 % / 70 %); `GeoVial.Sync` a 94,8 % / 88,6 %. **Verificado on-device:** deploy + arranque limpio y la ruta de restauración (recreación de proceso → sin crash). El camino positivo completo (login → matar proceso → vuelve a las solapas) y el reingreso con patrón se validan con el usuario en el dispositivo.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BUG-CAMARA-RESET | Bug | Aceptado (persistencia + restauración de sesión) |
| US-LOGIN-OJITO | Historia | Aceptada (mostrar/ocultar clave) |
| BUG-BIO-PATRON | Bug | Aceptado (biométrico acepta patrón/PIN) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 54 se traslada. |

Pendiente: confirmar con el usuario el camino positivo on-device (login → kill → vuelve a solapas) y el reingreso con patrón. Además, datos de prueba documentados en `DATOS-DE-PRUEBA.md` (raíz).

## 7. Decisiones tomadas durante el review

- Persistir la sesión en SecureStorage detrás de una abstracción (`IAlmacenTokenSesion`) para mantener el núcleo testeable.
- Restaurar en el arranque en un hilo del pool para no bloquear la UI ni arriesgar deadlock.
- Biométrico: aceptar credencial del dispositivo (patrón/PIN) con piso API 29; usar valores de plataforma de los autenticadores (binding no expone `BiometricManager.Authenticators`).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Sprint review del Sprint 54 (fixes on-device: reseteo por cámara → persistencia de sesión; ojito; biométrico con patrón/PIN). Veredicto Cumplido, velocity 8, 0 carry-over, 437 pruebas (+3); verificado deploy/arranque/restauración on-device. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
