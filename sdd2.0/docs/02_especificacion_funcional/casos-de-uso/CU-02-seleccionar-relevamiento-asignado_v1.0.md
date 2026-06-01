# CU-02 — Iniciar sesión y seleccionar un relevamiento asignado para recolectar

**Proyecto:** GeoVial
**Documento:** CU-02-seleccionar-relevamiento-asignado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que un agente de campo inicie sesión y elija uno de los relevamientos que le fueron asignados para comenzar la recolección, habilitando además el modo sin conexión cuando corresponde.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Inicia sesión, configura el método de seguridad y elige el relevamiento a recolectar |
| Sistema de acceso | Sistema | Autentica, valida el método de seguridad del teléfono, lista los relevamientos asignados y registra el acceso |

## 3. Precondiciones

- El agente de campo está dado de alta y tiene rol vigente.
- El primer inicio de sesión se realiza con conexión.
- El reingreso en terreno se realiza con el método de seguridad del teléfono (RN-06).

## 4. Flujo principal

1. El agente de campo solicita iniciar sesión con sus credenciales.
2. El sistema autentica al agente y registra el acceso en el registro de auditoría (RN-07).
3. Si es un inicio de sesión con conexión, el sistema verifica que el agente tenga configurado el método de seguridad del teléfono; si lo tiene, habilita el modo sin conexión (RN-06).
4. El sistema lista únicamente los relevamientos asignados al agente dentro de su área (RN-01).
5. El agente selecciona uno de los relevamientos asignados que no esté cerrado.
6. El sistema abre el relevamiento para recolección y confirma que el agente puede comenzar a capturar observaciones.

## 5. Flujos alternativos

- 5.A Reingreso en terreno sin conexión. Disparador: el agente vuelve a entrar en terreno sin señal. El sistema valida el método de seguridad del teléfono (RN-06) y, si es correcto, abre el último relevamiento seleccionado en modo sin conexión. Punto de retorno: paso 6.
- 5.B Configuración del método de seguridad. Disparador: en el primer inicio de sesión con conexión el agente no tiene el método configurado. El sistema solicita configurarlo y, una vez configurado, habilita el modo sin conexión. Punto de retorno: paso 4.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `CREDENCIALES_INVALIDAS` | Las credenciales no coinciden con un usuario vigente | Rechaza el acceso, no abre sesión y registra el intento |
| `OFFLINE_NO_HABILITADO` | El agente intenta habilitar el modo sin conexión sin método de seguridad configurado (RN-06) | Deniega la habilitación y solicita configurar el método con conexión |
| `REINGRESO_SIN_METODO_SEGURIDAD` | Reingreso en terreno sin el método de seguridad del teléfono (RN-06) | Rechaza el reingreso y bloquea la operación sin conexión |

## 7. Postcondiciones

- Éxito: el agente tiene una sesión activa, ve sus relevamientos asignados y tiene uno abierto para recolectar; el modo sin conexión queda habilitado si el método de seguridad está configurado.
- Fallo: no se abre sesión ni se habilita el modo sin conexión; el intento queda registrado.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente con dos relevamientos asignados y método de seguridad configurado | Inicia sesión con conexión | El sistema lista los dos relevamientos asignados y habilita el modo sin conexión |
| CA-02 | Un agente sin método de seguridad configurado, en su primer inicio de sesión con conexión | Intenta habilitar la operación sin conexión | El sistema responde `OFFLINE_NO_HABILITADO` y solicita configurar el método |
| CA-03 | Un agente en terreno sin señal y sin método de seguridad válido | Intenta reingresar | El sistema responde `REINGRESO_SIN_METODO_SEGURIDAD` y no abre la sesión sin conexión |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01, RN-06, RN-07 |
| Historias de usuario a generar | US a generar en 06 (inicio de sesión, relogueo en terreno, selección de relevamiento) |
| Componentes esperados | Módulo de acceso y selección de relevamiento (referencia tentativa a 05) |
| Tests previstos | Suite de acceso, relogueo y habilitación sin conexión (referencia tentativa a 08) |

## 10. Notas y supuestos

- El inicio de sesión y el relogueo en terreno con el método de seguridad del teléfono se modelan dentro de este CU como precondición y flujos alternativos, en lugar de un CU de login independiente, porque el negocio los describe como el acto de entrar a recolectar. Reconciliación respecto del mapeo de NB-01, que preveía CU-02 para "seleccionar un relevamiento asignado para recolectar"; el login se absorbe sin alterar ese mapeo y sin entrar en el mecanismo técnico de autenticación.
- El método de seguridad del teléfono es un concepto del dominio del cliente; el stack que lo implementa pertenece a 05.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-01 |

## 13. Interacción multiusuario y concurrencia

Varios agentes pueden tener abierto el mismo relevamiento de forma simultánea; cada uno recolecta sus observaciones y la consolidación de cambios concurrentes se rige por RN-04 al sincronizar. Una nueva asignación o reasignación realizada por el jefe de área (CU-01) se refleja en la lista del agente en su próximo inicio de sesión con conexión.
