# US-10 — Reabrir un relevamiento cerrado

**Proyecto:** GeoVial
**Documento:** US-10-reabrir-relevamiento-cerrado_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-02 Relevamientos y asignación
**Prioridad MoSCoW:** Should
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero reabrir explícitamente un relevamiento cerrado, para corregir o completar información cuando hace falta sin perder la trazabilidad del cierre.

## 2. Contexto

El BRIEF (§7 caso 5) fija que un relevamiento cerrado queda de solo lectura y que reabrirlo requiere una acción explícita del jefe de área. CU-10 (flujo 5.A) modela la transición cerrado → recolección solo ante esa acción explícita. Sin ella, un error detectado tras el cierre quedaría sin remedio.

## 3. Criterios de aceptación

- Given un relevamiento cerrado, When el jefe ejecuta la acción explícita de reapertura, Then el sistema lo pasa a recolección, levanta el solo lectura y registra la reapertura en auditoría.
- Given un relevamiento cerrado, When un proceso intenta reabrirlo sin la acción explícita del jefe, Then el sistema responde `REAPERTURA_NO_AUTORIZADA` y lo mantiene cerrado.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-10 |
| BT derivadas | BT-04, BT-07, BT-09, BT-18 |
| Tests previstos | acceptance/AT-10-reapertura |

## 5. Prioridad y estimación

Should: el MVP cierra relevamientos; la reapertura cubre la corrección posterior, valiosa pero no bloqueante. 3 SP (Fibonacci): extiende la máquina de estados de US-09 con una transición controlada.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-10)
- [x] Reglas de negocio identificadas (RN-05, RN-07)
- [x] Dependencia con US-09 declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La reapertura es un Should que depende de la máquina de estados de US-09; se prioriza después del cierre básico.
