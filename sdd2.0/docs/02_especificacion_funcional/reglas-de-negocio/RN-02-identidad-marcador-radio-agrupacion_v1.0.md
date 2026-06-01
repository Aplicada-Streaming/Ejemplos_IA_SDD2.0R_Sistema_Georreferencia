# RN-02 — Identidad de marcador y radio de agrupación

**Proyecto:** GeoVial
**Documento:** RN-02-identidad-marcador-radio-agrupacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Cada marcador geográfico tiene una identidad propia dentro de su relevamiento y una posición; dos marcadores cuyas posiciones disten entre sí menos que el radio de agrupación configurado para el relevamiento se consideran candidatos al mismo punto físico y nunca se unifican ni descartan de forma automática.

## 2. Justificación

Origen de negocio. El trabajo paralelo y sin conexión de varias cuadrillas produce puntos casi superpuestos que pueden referir a la misma obra. Unificar o descartar de forma silenciosa destruiría información válida; la decisión debe quedar en manos humanas (NB-05).

## 3. Ámbito de aplicación

Se evalúa al crear o reubicar un marcador, al sincronizar marcadores capturados sin conexión y al agrupar fotos en carga manual por radio. El radio de agrupación es un parámetro del relevamiento, configurable por el jefe de área.

## 4. Consecuencia si se viola

Si dos o más marcadores quedan dentro del radio, el sistema los señala como conflicto con el código `MARCADORES_EN_RADIO` y los deja pendientes de resolución manual; no los fusiona. Si un proceso intentara unificar o descartar marcadores sin decisión humana, la operación se rechaza con el código `UNIFICACION_NO_AUTORIZADA`.

## 5. CU afectados

CU-01, CU-04, CU-05, CU-07, CU-11, CU-12.

## 6. Pruebas que la verifican

- Dos marcadores a una distancia menor que el radio configurado se listan como conflicto y no se fusionan.
- Dos marcadores a una distancia mayor o igual que el radio no generan conflicto.
- Ningún proceso automático unifica ni descarta marcadores sin acción del jefe de área.
- Referencia a casos de prueba previstos en 08, suite de identidad y agrupación de marcadores.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-02 y NB-05 |
