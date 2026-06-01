# RN-07 — Retención del registro de auditoría

**Proyecto:** GeoVial
**Documento:** RN-07-retencion-auditoria_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Toda acción administrativa y todo acceso autenticado quedan registrados con su autor, su momento y la operación realizada, y ese registro se conserva por un período no menor a doce meses, durante el cual permanece consultable y no puede ser alterado ni eliminado.

## 2. Justificación

Origen regulatorio. La Ley 25.326 obliga al organismo a dar cuenta del tratamiento de los datos personales que maneja. El registro inalterable con retención mínima permite demostrar quién hizo qué y cuándo ante una auditoría (NB-06).

## 3. Ámbito de aplicación

Se evalúa en cada acción administrativa (altas, bajas, asignaciones, transiciones de estado, resoluciones de conflicto, exportación e importación) y en cada acceso autenticado. Define la obligación de asentar el evento y de conservarlo.

## 4. Consecuencia si se viola

Si una acción administrativa o un acceso no se registra, la operación se considera no trazable y se rechaza con el código `ACCION_NO_AUDITADA`. Todo intento de alterar o eliminar un registro de auditoría dentro del período de retención se rechaza con el código `AUDITORIA_INMUTABLE`.

## 5. CU afectados

CU-01, CU-02, CU-03, CU-08, CU-10, CU-12, CU-13.

## 6. Pruebas que la verifican

- Cada acción administrativa deja un registro con autor, momento y operación.
- Un registro de auditoría no puede modificarse ni eliminarse dentro de los doce meses.
- El historial de accesos de un dato se reconstruye dentro del período de retención.
- Referencia a casos de prueba previstos en 08, suite de auditoría y retención.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-06 |
