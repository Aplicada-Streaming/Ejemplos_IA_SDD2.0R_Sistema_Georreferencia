# README — Necesidades de Negocio (categoría 01) — GeoVial

Esta sección reúne las necesidades de negocio (NB) de GeoVial. El punto de entrada formal es el índice maestro [necesidades-negocio_v1.0.md](necesidades-negocio_v1.0.md); cada NB vive en un archivo propio dentro de [necesidades-de-negocio/](necesidades-de-negocio/). Este README aporta navegación rápida, mapa de dependencias, orden de lectura y RACI.

## 1. Tabla de NB

| NB-XX | Título | Impacto | Prioridad | Estado | Enlace |
| --- | --- | --- | --- | --- | --- |
| NB-01 | Delegación de la recolección en personal no experto | Libera al experto y escala la cantidad de obras relevadas | Must | Propuesto | [archivo](necesidades-de-negocio/NB-01-delegacion-recoleccion-personal-no-experto_v1.0.md) |
| NB-02 | Georreferenciación automática y confiable | Asegura una base de ubicaciones confiable para evaluar | Must | Propuesto | [archivo](necesidades-de-negocio/NB-02-georreferenciacion-automatica-confiable_v1.0.md) |
| NB-03 | Continuidad operativa sin conexión con sincronización confiable | Hace viable el relevamiento en zonas sin cobertura | Must | Propuesto | [archivo](necesidades-de-negocio/NB-03-continuidad-operativa-sin-conexion_v1.0.md) |
| NB-04 | Revisión y evaluación centralizada sobre mapa | Acelera la evaluación y la confección de informes | Must | Propuesto | [archivo](necesidades-de-negocio/NB-04-revision-centralizada-sobre-mapa_v1.0.md) |
| NB-05 | Consistencia de datos ante duplicados y conflictos | Preserva la confianza en la base ante trabajo paralelo | Should | Propuesto | [archivo](necesidades-de-negocio/NB-05-consistencia-datos-ante-conflictos_v1.0.md) |
| NB-06 | Trazabilidad de acciones y protección de datos personales | Habilita el cumplimiento de la Ley 25.326 | Must | Propuesto | [archivo](necesidades-de-negocio/NB-06-trazabilidad-proteccion-datos-personales_v1.0.md) |

## 2. Mapa de dependencias

| NB | Depende de | Es prerequisito de |
| --- | --- | --- |
| NB-01 | — | NB-02, NB-03, NB-06 |
| NB-02 | NB-01 | NB-03, NB-04, NB-05 |
| NB-03 | NB-01, NB-02 | NB-04, NB-05 |
| NB-04 | NB-02, NB-03 | — |
| NB-05 | NB-03, NB-02 | — |
| NB-06 | NB-01 | — (atraviesa el resto como necesidad transversal) |

El grafo es acíclico y ninguna NB supera tres dependencias.

## 3. Orden de lectura sugerido

1. NB-01 — necesidad raíz; define quién recolecta y por qué.
2. NB-02 — qué dato de ubicación debe quedar confiable en cada observación.
3. NB-03 — cómo se sostiene la recolección sin conexión y cómo se traspasa.
4. NB-04 — cómo se evalúa de forma centralizada lo recolectado.
5. NB-05 — cómo se mantiene consistente la base ante conflictos.
6. NB-06 — cómo se controla y audita el tratamiento de los datos.

## 4. RACI breve

R: responsable de ejecutar. A: rinde cuentas y aprueba. C: consultado. I: informado.

| NB | Responsable (R) | Aprobador (A) | Consultado (C) | Informado (I) |
| --- | --- | --- | --- | --- |
| NB-01 | Equipo de desarrollo | Jefe general | Jefe de área | Agente de campo |
| NB-02 | Equipo de desarrollo | Jefe general | Jefe de área | Agente de campo |
| NB-03 | Equipo de desarrollo | Jefe general | Agente de campo | Jefe de área |
| NB-04 | Equipo de desarrollo | Jefe general | Jefe de área | Área central de evaluación |
| NB-05 | Equipo de desarrollo | Jefe general | Jefe de área | Usuario raíz |
| NB-06 | Equipo de desarrollo | Jefe general | Usuario raíz | Agente de campo |

## 5. Notas

- Propietario del documento: Analista de Negocio Senior (AG-01). Revisión y validación: AG-00 (alineación a visión y alcance) y AG-02 (trazabilidad a CU).
- Cantidad de NB: 6. La justificación de fusiones y particiones está en la §5 del índice maestro.
