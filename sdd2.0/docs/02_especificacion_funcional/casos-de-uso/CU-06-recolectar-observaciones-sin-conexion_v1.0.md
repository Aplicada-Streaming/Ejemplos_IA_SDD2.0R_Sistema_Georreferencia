# CU-06 — Recolectar observaciones en modo sin conexión

**Proyecto:** GeoVial
**Documento:** CU-06-recolectar-observaciones-sin-conexion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que un agente de campo recolecte observaciones durante una jornada completa sin conexión, guardando todo localmente y registrando cada cambio en una cola para sincronizarlo después.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Recolecta observaciones en terreno sin señal |
| Sistema de captura local | Sistema | Guarda las observaciones localmente y registra cada cambio en la cola de sincronización |

## 3. Precondiciones

- El agente habilitó el modo sin conexión en un inicio de sesión con conexión, con su método de seguridad del teléfono configurado (RN-06).
- El agente tiene seleccionado un relevamiento asignado no cerrado (RN-05).

## 4. Flujo principal

1. El agente reingresa en terreno con el método de seguridad del teléfono (RN-06).
2. El agente captura observaciones con fotos, comentarios y etiquetas (CU-04, CU-05), sin conexión.
3. El sistema asigna la coordenada de cada observación y la guarda localmente.
4. El sistema registra cada cambio (alta o edición de observación, marcador, foto, comentario o etiqueta) en la cola de sincronización con su tipo de operación, entidad y marca temporal.
5. El agente continúa recolectando durante la jornada sin necesidad de conexión.

## 5. Flujos alternativos

- 5.A Observación sin georreferenciar en campo. Disparador: una foto capturada sin señal no trae metadatos de ubicación y el agente no ubica el punto. El sistema la deriva a la bandeja sin georreferenciar local (RN-03) y la encola igual para sincronizar. Punto de retorno: paso 4.
- 5.B Edición de una observación ya capturada. Disparador: el agente modifica comentarios o etiquetas de una observación local. El sistema actualiza el registro local y encola la edición con su marca temporal. Punto de retorno: paso 4.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `REINGRESO_SIN_METODO_SEGURIDAD` | El agente intenta reingresar sin el método de seguridad del teléfono (RN-06) | Bloquea el reingreso y la operación sin conexión |
| `ALMACENAMIENTO_LOCAL_INSUFICIENTE` | El dispositivo no tiene espacio para guardar la observación | Informa la falta de espacio y conserva lo ya guardado y encolado |
| `RELEVAMIENTO_SOLO_LECTURA` | El relevamiento seleccionado fue cerrado antes de capturar (RN-05) | Rechaza la captura e indica el cierre del relevamiento |

## 7. Postcondiciones

- Éxito: las observaciones quedan guardadas localmente y encoladas para sincronizar, con su georreferenciación o en la bandeja sin georreferenciar.
- Fallo: no se reingresa sin el método de seguridad; lo ya guardado y encolado se conserva ante falta de espacio.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente con modo sin conexión habilitado y un relevamiento abierto, sin señal | Captura una observación con foto, comentario y etiqueta | El sistema la guarda localmente y registra el cambio en la cola de sincronización |
| CA-02 | Un agente sin método de seguridad válido en terreno | Intenta reingresar sin señal | El sistema responde `REINGRESO_SIN_METODO_SEGURIDAD` y no habilita la recolección |
| CA-03 | Un agente que recolectó 100 observaciones sin conexión durante la jornada | Continúa una jornada laboral completa sin señal | El sistema conserva las 100 observaciones encoladas, sin pérdida |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-03, RN-05, RN-06 |
| Historias de usuario a generar | US a generar en 06 (recolección sin conexión, cola de cambios local, bandeja sin georreferenciar local) |
| Componentes esperados | Módulo de captura local y cola de sincronización (referencia tentativa a 05) |
| Tests previstos | Suite de operación sin conexión y encolado de cambios (referencia tentativa a 08) |

## 10. Notas y supuestos

- Se soporta al menos una jornada laboral completa sin conexión; no hay límite duro de observaciones más allá del almacenamiento del dispositivo.
- El subir y bajar de cambios pertenece a CU-07; este CU cubre solo la recolección y el encolado local.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-03 |

## 13. Interacción multiusuario y concurrencia

Cada agente recolecta de forma aislada en su dispositivo; no hay interacción en línea con otros agentes durante la recolección sin conexión. La interacción concurrente se materializa recién al sincronizar (CU-07), donde la coexistencia de cambios incompatibles se resuelve por última escritura con marca de conflicto (RN-04).
