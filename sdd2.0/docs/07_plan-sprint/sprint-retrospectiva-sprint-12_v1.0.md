# Sprint Retrospectiva — Sprint 12

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-12_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Centralizar el formato del payload de comentario en `ChangeRecordFactory` (compartido con el cliente REST) eliminó el riesgo de divergencia entre lo que se encola y lo que consume `/sync`; el round-trip quedó garantizado por construcción.
- Exponer el núcleo del coordinador de sync como un método awaitable (`SincronizarSiCorrespondeAsync`) hizo testeable el disparo automático sin depender del `async void` del manejador de evento; las pruebas verifican tanto el método como el disparo por el evento de conectividad.
- Traducir `SQLITE_FULL` a una excepción tipada en la cola dejó el manejo de disco lleno (US-16 CA-03) en el lugar correcto y testeable a nivel del contrato del colector.
- La librería `GeoVial.Sync` mantuvo cobertura alta (94 %) y siguió independiente del workload MAUI, confirmando que separar el núcleo de la cáscara fue la decisión correcta.

## 2. Qué no salió bien

- La sincronización automática asume un único relevamiento activo (la cola sube todos sus cambios a ese relevamiento); cuando un agente trabaje varios relevamientos en una jornada, la cola y el motor deberán segmentar los cambios por relevamiento.
- La verificación de US-16 CA-03 se hace a nivel del contrato (una cola que lanza la excepción), no contra un disco realmente lleno; la traducción de `SQLITE_FULL` en la cola SQLite queda sin una prueba de integración con un dispositivo sin espacio.

## 3. Qué probar

- Extender la cola y el motor para etiquetar cada cambio con su relevamiento, de modo que la sincronización suba cada cambio a su relevamiento correspondiente en una sola pasada.
- Sumar una prueba de integración (o de campo) que llene el almacenamiento local y verifique el `ALMACENAMIENTO_LOCAL_INSUFICIENTE` real, cuando exista el agente con Android SDK provisionado.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Segmentar la cola/motor por relevamiento para jornadas con varios relevamientos | AG-05 | 2026-12-05 | Pendiente |
| Construir la UI de captura de campo (foto/GPS/comentario) en `GeoVial.Mobile` | AG-08 (móvil) | 2026-12-05 | Pendiente |
| Provisionar el Android SDK en un agente para construir/ejecutar la app y probar disco lleno | AG-09 | 2026-12-05 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 11 | Estado actual |
| --- | --- |
| Provisionar Android SDK + JDK en un agente para construir/ejecutar `GeoVial.Mobile` | Pendiente (se reitera) |
| Construir US-16 (captura offline) y US-19 (conectividad y sync automática) sobre `GeoVial.Sync` | Completada (entregadas en este sprint) |
| Publicar `GeoVial.Sync` como paquete preview en GitHub Packages | Pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 12 con 3 acciones nuevas y seguimiento de las del Sprint 11 (US-16/US-19 completadas). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
