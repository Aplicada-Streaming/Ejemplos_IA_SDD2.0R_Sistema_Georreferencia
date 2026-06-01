# US-16 — Recolectar observaciones sin conexión

**Proyecto:** GeoVial
**Documento:** US-16-recolectar-observaciones-sin-conexion_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-04 Sincronización offline
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como agente de campo, quiero recolectar observaciones durante una jornada completa sin conexión, para poder trabajar en zonas sin cobertura sin perder nada de lo que registro.

## 2. Contexto

NB-03 exige continuidad operativa sin conexión por al menos una jornada laboral (NFR 8 h). CU-06 guarda cada observación localmente mientras el agente trabaja sin señal. Es lo que una planilla y un álbum de fotos no resuelven; sin esta historia, la captura de campo dependería de la cobertura.

## 3. Criterios de aceptación

- Given un agente con modo sin conexión habilitado y un relevamiento abierto, sin señal, When captura una observación con foto, comentario y etiqueta, Then el sistema la guarda localmente.
- Given un agente que recolecta sin señal durante una jornada laboral completa, When captura 100 observaciones, Then el sistema las conserva localmente sin pérdida.
- Given un dispositivo sin espacio de almacenamiento, When el agente intenta guardar una observación, Then el sistema responde `ALMACENAMIENTO_LOCAL_INSUFICIENTE` y conserva lo ya guardado.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-03 |
| CU cubiertos | CU-06 |
| BT derivadas | BT-13, BT-14, BT-19 |
| Tests previstos | acceptance/AT-06-recoleccion-offline |

## 5. Prioridad y estimación

Must: el offline es una capacidad nuclear del producto. 8 SP (Fibonacci): persistencia local durable, captura sin red y manejo de almacenamiento.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-06)
- [x] Reglas de negocio identificadas (RN-06, RN-05, RN-03)
- [x] Dependencia con US-05 (relogueo) y US-11 (captura) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

El encolado de cambios para sincronizar se cubre en US-17 y el subir/bajar en US-18. Una observación sin georreferenciar en campo se deriva a la bandeja local (US-13) y se encola igual.
