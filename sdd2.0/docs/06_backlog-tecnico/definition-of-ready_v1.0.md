# Definition of Ready — GeoVial

**Proyecto:** GeoVial
**Documento:** definition-of-ready_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Scrum Master / Agile Coach (AG-06), Equipo SDD 2.0
**Trazabilidad upstream:** product-backlog_v1.0.md, backlog-tecnico_v1.0.md
**Trazabilidad downstream:** 07_plan-sprint (filtro de entrada al Sprint Planning); 08_calidad_y_pruebas (la Definition of Done vive en 08)

La Definition of Ready (DoR) define cuándo un ítem está listo para entrar a un sprint, no cuándo está terminado. El filtro de salida —cuándo un ítem se considera `Done`— es la Definition of Done y vive en la categoría 08; esta DoR no la duplica ni la solapa. Una US o BT que no cumple su DoR no entra al Sprint Planning.

## 1. Criterios DoR para US

Una US está Ready cuando cumple todos estos criterios verificables (sí/no):

1. La historia está redactada como `Como [rol], quiero [acción], para [valor]` con el valor para el rol explícito y no vacío.
2. Tiene al menos un CU relacionado declarado en la trazabilidad (sin US huérfana de CU).
3. Tiene criterios de aceptación en formato Given/When/Then, con al menos dos escenarios si es Must o Should (un happy path y un edge case).
4. Está estimada en story points con la técnica Fibonacci declarada en el product-backlog.
5. Las reglas de negocio (RN) o conceptuales (RC) que la condicionan están identificadas.
6. Sus dependencias con otras US o BT están declaradas y ninguna es bloqueante para iniciarla en el sprint.
7. Cabe en un sprint (atributo Small de INVEST); si supera 13 SP en refinamiento, está descompuesta.

## 2. Criterios DoR para BT

Una BT está Ready cuando cumple todos estos criterios técnicos (sí/no):

1. Tiene fuente upstream declarada: una ADR, un componente o un contrato de 05 (o un CU/NB cuando aplica). Sin fuente upstream no entra.
2. Declara al menos una US consumidora, o se justifica como infraestructura compartida con una ADR explícita.
3. Tiene criterios de aceptación técnicos verificables (compila, los tests pasan, el contrato se respeta, la deuda queda saldada).
4. Tiene declaradas sus dependencias (BT o US previas) y su tipo (feature, spike, refactor, devops o docs).
5. Está estimada en story points con la técnica Fibonacci; si es spike, declara su caja temporal explícita.

## 3. Excepciones admitidas

- Spike exploratorio: una BT de tipo spike puede entrar sin todos los criterios técnicos cerrados (por ejemplo, sin criterios de aceptación de implementación), siempre que declare su caja temporal y el objetivo de la indagación. El resultado del spike alimenta el refinamiento de las US o BT que dependen de él.
- US Could en estado `Borrador` con un criterio dependiente de 03 (por ejemplo, canal de aviso de US-20 o gestos de zoom de US-24): pueden ingresar a refinamiento, pero no a Sprint Planning, hasta resolver ese criterio con 03.

Toda excepción se documenta en la US o BT correspondiente y la aprueba el rol indicado en §4.

## 4. Aprobador

- El Scrum Master (AG-06) es el responsable de validar que un ítem cumple la DoR antes de incorporarlo al Sprint Planning, y de aprobar las excepciones de §3.
- La trazabilidad US↔CU la firma el Analista Funcional (AG-02); la fuente upstream de cada BT la valida el Arquitecto (AG-05); la verificabilidad de los criterios de aceptación la revisa QA (AG-08). Estas revisiones son acotadas; la titularidad de la DoR y la decisión final de "Ready" es del AG-06.

## 5. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | DoR inicial: 7 criterios para US, 5 para BT, excepciones (spike, Could dependiente de 03) y aprobador. Generada por AG-06 |
