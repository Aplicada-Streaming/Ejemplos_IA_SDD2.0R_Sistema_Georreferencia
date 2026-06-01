# 06 Backlog técnico — GeoVial

**Proyecto:** GeoVial
**Tipo de proyecto:** web-monolith
**Estimación adoptada:** Fibonacci (1, 2, 3, 5, 8, 13, 21)
**Fecha:** 2026-06-01
**Autor:** Scrum Master / Agile Coach (AG-06), Equipo SDD 2.0

Índice navegable del backlog de GeoVial para revisores (AG-02, AG-05, AG-07, AG-08).

## Artefactos

- [product-backlog_v1.0.md](product-backlog_v1.0.md) — objetivos y MVP, épicas EP-01 a EP-09, historias por épica, métricas y refinamiento.
- [backlog-tecnico_v1.0.md](backlog-tecnico_v1.0.md) — épicas técnicas EP-T1 a EP-T6, 22 BT inline y matriz BT↔US↔CU.
- [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md) — DoR vigente (7 criterios US, 5 BT, excepciones y aprobador).
- [historias-usuario/](historias-usuario/) — 32 archivos individuales US-01 a US-32.

## Conteo y decisión inline-vs-individual

- US: 32 → superan el umbral de 20, por lo que viven en archivos individuales bajo `historias-usuario/` (§3.3).
- BT: 22 → por debajo del umbral de 30, por lo que viven inline en `backlog-tecnico_v1.0.md` (§3.3).
- Épicas de producto: 9 (EP-01 a EP-09). Épicas técnicas: 6 (EP-T1 a EP-T6).
- IDs de dos dígitos uniformes en US, BT y EP, sin mezclar esquemas de numeración entre artefactos.

## Épicas vigentes

| EP | Nombre | NB origen |
| --- | --- | --- |
| EP-01 | Jerarquía, usuarios y acceso | NB-01 |
| EP-02 | Relevamientos y asignación | NB-01, NB-04 |
| EP-03 | Captura y georreferenciación | NB-02, NB-04 |
| EP-04 | Sincronización offline | NB-03 |
| EP-05 | Revisión sobre mapa | NB-04 |
| EP-06 | Resolución de conflictos | NB-05 |
| EP-07 | Exportación e importación | NB-04 |
| EP-08 | Auditoría y datos personales | NB-06 |
| EP-09 | Librería de sincronización publicada | NB-03 |

## US Must del MVP

US-01, US-02, US-04, US-05 (EP-01); US-06, US-07, US-09 (EP-02); US-11, US-12, US-13, US-14, US-15 (EP-03); US-16, US-17, US-18, US-19 (EP-04); US-21, US-22 (EP-05); US-27, US-28 (EP-07); US-29, US-30, US-31 (EP-08). Total: 23 US Must.

## BT prioritarias

BT-09 (scaffolding y capas), BT-07 (persistencia EF Core), BT-18 (API REST + OpenAPI + Problem Details), BT-08 (auth y autorización), BT-12 (auditoría inmutable), BT-13/BT-14/BT-15 (móvil, cola y motor de sync). Son las BT que habilitan el walking skeleton y el núcleo Must del MVP.

## DoR vigente

[definition-of-ready_v1.0.md](definition-of-ready_v1.0.md): 7 criterios verificables para US y 5 para BT, con excepciones para spikes y US Could dependientes de 03. Aprobador: AG-06. La DoR habla de cuándo empezar; la Definition of Done (cuándo terminar) vive en 08.

## Distribución MoSCoW

| Prioridad | US | Porcentaje |
| --- | --- | --- |
| Must | 23 | 71,9 % |
| Should | 6 | 18,8 % |
| Could | 3 | 9,4 % |
