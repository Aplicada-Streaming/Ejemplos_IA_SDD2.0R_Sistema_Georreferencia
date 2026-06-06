# Sprint Retrospectiva — Sprint 55

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-55_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-05
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- La revisión on-device (round 2) volvió a pagar: el usuario detectó que el modelo de relogueo era confuso y que S54 restauraba la sesión en silencio (hueco de seguridad). Se rearquitecturó según práctica de industria con un núcleo **puro y testeable** (`PoliticaArranque` 8 casos + `CoordinadorArranque` 5).
- Se cerró el hueco de S54 sin perder su beneficio: la vuelta de la cámara (la marca `IMarcadorCaptura` sobrevive a la muerte de proceso) entra sin re-pedir, pero cualquier otra reapertura exige el patrón.
- El tap del pin reusó el patrón de "esquema centinela" de S51 (otro `Parseador…` puro), y el "mostrar usuario" fue trivial. Buen apalancamiento de patrones ya probados.
- Antes de codear se **evaluó contra la doc** (el marcador se crea solo, CU-04/RN-02; lo de la spec es enriquecerlo, US-15) y contra prácticas de industria, y se consultó al PO las dos decisiones (alcance del marcador + semántica del logout). Evitó construir lo que no correspondía.

## 2. Qué no salió bien

- Quedó **deuda de diseño**: `CoordinadorReingreso` (online `/auth/reingreso`, S53) ya no se usa en la UI porque el desbloqueo offline (patrón + token persistido) lo reemplaza. Hay dos caminos de "volver a la sesión" y conviene consolidar (remover o repurposear el online) para no confundir.
- **Token persistido vencido offline**: tras desbloquear, si el JWT expiró, el primer request dará 401 hasta re-loguear online. Sin refresh token, es deuda real (afecta jornadas largas sin señal).
- La verificación on-device del flujo nuevo (patrón al reabrir, cancelar, vuelta de cámara, tap del pin) **depende del usuario**; el equipo sólo confirmó build + arranque sin crash + el núcleo en el gate.
- `SeguridadDispositivo` (marcador blando `metodo.configurado`/usuario) y `AlmacenTokenSecureStorage` guardan ambos el "usuario": hay redundancia de almacenamiento que conviene unificar.
- **Bug del puente WebView hallado on-device:** al tocar el pin daba `ERR_UNKNOWN_URL_SCHEME`. Causa: el `MapaWebViewClient` (que se setea para cachear teselas) **reemplaza** al client interno de MAUI, y con eso el evento `Navigating` —en el que se apoyaban el tap del pin (S55) y el "ubicar" (S51)— deja de dispararse. Se corrigió interceptando el esquema centinela **dentro** del `MapaWebViewClient` (`ShouldOverrideUrlLoading`) con un callback. Esto **también arregla el "ubicar" de S51**, que tenía el mismo defecto y nunca se había probado en dispositivo — confirma, una vez más, que la interacción WebView hay que verificarla on-device.

## 3. Qué probar

- On-device, de corrido: login con clave → cerrar app → reabrir → **patrón** → entra; cancelar el patrón → bloqueo → "usar usuario y clave"; sacar foto → volver **sin** patrón; "Cerrar sesión" → usuario+clave; tocar un pin → carrusel.
- Jornada offline larga: ver qué pasa cuando el token expira sin señal (decidir si hace falta refresh token).

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Confirmar on-device el modelo de relogueo + tap del pin | AG-05 (QA) | 2028-08-04 | En curso (con el usuario) |
| Consolidar los dos caminos de re-entrada (remover/repurposear `CoordinadorReingreso` online) | AG-09 (fullstack) | 2028-08-04 | Pendiente |
| Evaluar refresh token para jornadas offline largas | AG-06 (backend) | 2028-08-04 | Pendiente |
| Unificar el almacenamiento de "usuario recordado" (SeguridadDispositivo vs AlmacenToken) | AG-08 (móvil) | 2028-08-04 | Pendiente (limpieza) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 54 | Estado actual |
| --- | --- |
| Confirmar on-device camino positivo de restauración + reingreso con patrón | Reemplazado por el modelo nuevo de S55 (a confirmar on-device) |
| Verificar features de plataforma en dispositivo dentro del mismo sprint | Aplicado (se redeployó y verificó arranque; flujo interactivo con el usuario) |
| Evaluar persistir la captura en vuelo si se pierde la foto tras kill | Pendiente (condicional) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-05 | Retrospectiva del Sprint 55 (revisión on-device round 2): rearquitectura del relogueo (núcleo puro + coordinador), pin tocable y usuario visible. Deuda: consolidar los dos caminos de re-entrada y evaluar refresh token. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
