# US-05 — Relogueo en terreno con método de seguridad

**Proyecto:** GeoVial
**Documento:** US-05-relogueo-terreno-metodo-seguridad_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-01 Jerarquía, usuarios y acceso
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero volver a entrar a la app en terreno usando el método de seguridad del teléfono, para retomar la recolección sin señal sin tener que reautenticarme contra el servidor.

## 2. Contexto

NB-03 exige continuidad operativa sin conexión. CU-02 (flujo 5.A) y RN-06 fijan que el relogueo en terreno se hace con el método de seguridad del teléfono, y que el modo sin conexión solo se habilita si ese método está configurado en el primer login con conexión. Sin esta historia, un agente sin señal no podría reabrir la app y se perdería la jornada de campo.

## 3. Criterios de aceptación

- Given un agente con método de seguridad configurado, en terreno sin señal, When reingresa validando el método de seguridad del teléfono, Then el sistema abre el último relevamiento seleccionado en modo sin conexión.
- Given un agente sin método de seguridad válido, en terreno sin señal, When intenta reingresar, Then el sistema responde `REINGRESO_SIN_METODO_SEGURIDAD` y no abre la sesión sin conexión.
- Given un agente en su primer login con conexión sin método configurado, When intenta habilitar la operación sin conexión, Then el sistema responde `OFFLINE_NO_HABILITADO` y solicita configurar el método.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-02 |
| BT derivadas | BT-08, BT-13, BT-18 |
| Tests previstos | acceptance/AT-02-relogueo-offline |

## 5. Prioridad y estimación

Must: la continuidad de la jornada de campo sin conexión depende del relogueo seguro. 5 SP (Fibonacci): involucra el método de seguridad del dispositivo y la precondición de habilitación offline.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-02)
- [x] Reglas de negocio identificadas (RN-06, RN-01)
- [x] Dependencia con US-04 (login inicial) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El refresh del token en móvil se condiciona al método de seguridad del teléfono; el detalle técnico vive en las BT (ADR-03). El método de seguridad es del dominio del cliente.
