# US-01 — Administrar alta y baja jerárquica de usuarios

**Proyecto:** GeoVial
**Documento:** US-01-administrar-alta-baja-jerarquica-usuarios_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-01 Jerarquía, usuarios y acceso
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como administrador de jerarquía (usuario raíz, jefe general o jefe de área), quiero dar de alta y de baja usuarios del nivel inmediato inferior al mío, para habilitar y deshabilitar a las personas que operan el sistema sin romper la cadena de responsabilidad.

## 2. Contexto

NB-01 exige delegar la recolección en personal no experto, lo que requiere una jerarquía operable de usuarios (raíz → jefe general → jefe de área → agente de campo). CU-03 describe el alta y la baja según el nivel administrable. Sin esta historia no hay forma de incorporar agentes ni jefes de área, y la delegación no puede arrancar.

## 3. Criterios de aceptación

- Given un jefe general autenticado, When da de alta un jefe de área asociado al área "Zona Norte", Then el sistema lo crea vigente, asociado a "Zona Norte", y registra el alta en auditoría.
- Given un jefe de área autenticado, When intenta dar de alta a otro jefe de área, Then el sistema rechaza con `ACCESO_NO_AUTORIZADO` y no crea el usuario.
- Given un jefe general, When da de baja un jefe de área, Then el sistema lo marca como no vigente, conserva sus datos y la información que produjo, y registra la baja en auditoría.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-01 |
| CU cubiertos | CU-03 |
| BT derivadas | BT-01, BT-02, BT-07, BT-09, BT-12, BT-18 |
| Tests previstos | acceptance/AT-03-alta-baja-jerarquica |

## 5. Prioridad y estimación

Must: sin altas jerárquicas no existen los actores del sistema. 8 SP por Planning Poker (Fibonacci): toca el dominio de jerarquía, validación de nivel administrable y auditoría obligatoria de cada acción.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-03)
- [x] Reglas de negocio identificadas (RN-01, RN-07, RN-08)
- [x] Sin dependencias bloqueantes para arrancar
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El alta del jefe general la realiza el usuario raíz en la puesta en marcha del sistema. La baja conserva la información histórica bajo el tratamiento de RN-08 (no se borran registros). El mecanismo concreto de persistencia y autorización vive en las BT, no en esta US.
