# US-04 — Iniciar sesión y seleccionar un relevamiento asignado

**Proyecto:** GeoVial
**Documento:** US-04-iniciar-sesion-seleccionar-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-01 Jerarquía, usuarios y acceso
**Prioridad MoSCoW:** Must
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero iniciar sesión con conexión y elegir uno de los relevamientos que me asignaron, para empezar a recolectar en la obra correcta sin ver trabajo que no me corresponde.

## 2. Contexto

NB-01 delega la recolección en agentes; cada agente debe poder entrar y ubicar su trabajo. CU-02 describe el inicio de sesión y la selección del relevamiento asignado, listando solo los relevamientos del agente dentro de su área (RN-01). Sin esta historia el agente no puede acceder a su tarea de campo.

## 3. Criterios de aceptación

- Given un agente con dos relevamientos asignados y método de seguridad configurado, When inicia sesión con conexión, Then el sistema lista los dos relevamientos asignados y habilita el modo sin conexión.
- Given un agente autenticado, When intenta abrir un relevamiento que no le fue asignado o de otra área, Then el sistema no lo lista ni permite abrirlo.
- Given credenciales que no corresponden a un usuario vigente, When el agente intenta iniciar sesión, Then el sistema responde `CREDENCIALES_INVALIDAS` y registra el intento.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-02 |
| BT derivadas | BT-08, BT-09, BT-13, BT-18 |
| Tests previstos | acceptance/AT-02-login-seleccion |

## 5. Prioridad y estimación

Must: es la puerta de entrada del agente a la recolección. 5 SP (Fibonacci): incluye autenticación, listado acotado por asignación y área, y habilitación del modo sin conexión.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-02)
- [x] Reglas de negocio identificadas (RN-01, RN-06, RN-07)
- [x] Sin dependencias bloqueantes más allá de la jerarquía (US-01)
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El primer inicio de sesión se realiza con conexión; el relogueo en terreno sin señal se cubre en US-05. El método de seguridad del teléfono es un concepto del dominio; su implementación vive en las BT.
