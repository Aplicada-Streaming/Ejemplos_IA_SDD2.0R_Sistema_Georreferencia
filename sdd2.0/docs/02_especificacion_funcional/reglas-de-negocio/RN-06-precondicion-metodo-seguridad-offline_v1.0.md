# RN-06 — Método de seguridad del teléfono como precondición del modo sin conexión

**Proyecto:** GeoVial
**Documento:** RN-06-precondicion-metodo-seguridad-offline_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

El modo de operación sin conexión de un agente de campo solo queda habilitado si, durante un inicio de sesión con conexión, el agente tiene configurado el método de seguridad del teléfono que se exige para el reingreso en terreno; sin ese método configurado, el sistema no habilita la operación sin conexión.

## 2. Justificación

Origen de negocio y regulatorio. El primer inicio de sesión requiere conexión; en terreno el reingreso se hace con el método de seguridad del teléfono. Condicionar el modo sin conexión a ese método protege el acceso a los datos personales y a los registros que quedan guardados localmente (NB-03, NB-06).

## 3. Ámbito de aplicación

Se evalúa en el primer inicio de sesión con conexión, en cada reingreso en terreno y al habilitar la recolección sin conexión. Define la precondición que separa un dispositivo apto de uno no apto para operar sin señal.

## 4. Consecuencia si se viola

Si el agente intenta habilitar la operación sin conexión sin el método de seguridad configurado, el sistema lo deniega con el código `OFFLINE_NO_HABILITADO` y solicita configurar el método durante un inicio de sesión con conexión. Un reingreso en terreno sin el método configurado se rechaza con el código `REINGRESO_SIN_METODO_SEGURIDAD`.

## 5. CU afectados

CU-02, CU-06.

## 6. Pruebas que la verifican

- Un agente sin método de seguridad configurado no habilita la operación sin conexión.
- Un agente con método configurado en un inicio de sesión con conexión habilita la operación sin conexión.
- El reingreso en terreno exige el método de seguridad del teléfono.
- Referencia a casos de prueba previstos en 08, suite de habilitación del modo sin conexión.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-03 y NB-06 |
