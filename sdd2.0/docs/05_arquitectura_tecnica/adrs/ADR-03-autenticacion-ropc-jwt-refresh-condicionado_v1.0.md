# ADR-03 — Autenticación ROPC + JWT bearer con refresh condicionado al método de seguridad del teléfono

**Proyecto:** GeoVial
**Documento:** ADR-03-autenticacion-ropc-jwt-refresh-condicionado_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Seguridad

## 1. Contexto

GeoVial tiene una jerarquía de roles (raíz, jefe general, jefe de área, agente de campo) y autoriza cada acción por rol y área (RN-01, CU-14). El primer inicio de sesión del agente requiere internet; en terreno el reingreso se hace con los métodos de seguridad del teléfono (PIN/biometría), que son precondición para habilitar el modo sin conexión (RN-06, CU-02, CU-06). El cliente fijó el flujo ROPC con bearer token JWT (PROJECT-README §8). La protección del acceso a los datos personales que quedan guardados localmente está sujeta a la Ley 25.326 (RN-08).

## 2. Decisión

Se adopta el flujo ROPC (Resource Owner Password Credentials) que emite un access token JWT bearer de vida corta (≈60 min) con claims `sub` y `role`, más un refresh token. En la app móvil, el uso del refresh token se condiciona a que el agente complete el método de seguridad del teléfono; sin ese método configurado en un login con conexión, no se habilita el modo sin conexión ni el refresh en terreno.

## 3. Estado

Aceptado el 2026-06-01. Renumeración de `ADR-003` (PROJECT-README §15, estado original Propuesto) a `ADR-03`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| ROPC + JWT con refresh condicionado (elegido) | Fijado por el cliente; simple para un organismo con gestión interna de credenciales; condiciona offline a RN-06 | ROPC no es el flujo recomendado para terceros; acopla credenciales al cliente |
| Authorization Code + PKCE | Flujo recomendado por OAuth para apps | Requiere navegador/IdP externo; excede el alcance interno y la operación de campo sin conexión |
| Sesión por cookie server-side sin JWT | Simple para el front Blazor | No sirve a la app móvil ni a la API REST consumida por dos clientes; no soporta el relogueo offline |

## 5. Consecuencias positivas

1. Un mismo esquema de token sirve a la API consumida por web y móvil.
2. El claim `role` habilita la autorización por rol y área en el módulo transversal (RN-01).
3. El condicionamiento del refresh al método de seguridad del teléfono materializa RN-06 y protege los datos locales (RN-08).

## 6. Consecuencias negativas y trade-offs

1. ROPC expone las credenciales al cliente; aceptado por ser un organismo con gestión interna y sin federación de identidad en v1.
2. El access token de vida corta obliga a refrescar; en terreno el refresh depende del método de seguridad, lo que puede bloquear a un agente que no lo configuró (comportamiento deseado por RN-06).

## 7. Implementación

`GeoVial.Api` emite y valida los JWT; el módulo de acceso y selección (CU-02) gestiona login, relogueo y habilitación offline. El access token dura ≈60 min; el refresh en móvil se valida contra el método de seguridad del teléfono. Errores: `OFFLINE_NO_HABILITADO` y `REINGRESO_SIN_METODO_SEGURIDAD` (RN-06), `ACCESO_NO_AUTORIZADO` (RN-01). Secretos de firma gestionados por el secret store del entorno (PROJECT-README §8).

## 8. Métricas de validación

- Un agente sin método de seguridad configurado no habilita el modo sin conexión (suite de habilitación offline, 08).
- El token expira a ≈60 min y el refresh respeta la condición del método de seguridad.
- Todo acceso autenticado queda auditado (RN-07, ADR-14).

## 9. Referencias

- PROJECT-README §8 (seguridad y autenticación).
- RN-01, RN-06, RN-08; CU-02, CU-06, CU-14.
- ADR-14 (compliance), ADR-05 (offline).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Decisión aceptada. Renumeración de ADR-003 a ADR-03 |
