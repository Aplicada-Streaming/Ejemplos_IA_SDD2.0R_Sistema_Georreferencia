# RN-08 — Tratamiento de datos personales bajo la Ley 25.326

**Proyecto:** GeoVial
**Documento:** RN-08-tratamiento-datos-personales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Los datos personales de los agentes y los registros del organismo solo son accesibles a los usuarios cuyo rol y área los habilitan, se tratan exclusivamente para los fines del relevamiento y su evaluación, y todo acceso a ellos queda sujeto a registro, en cumplimiento de la Ley 25.326 de Protección de Datos Personales.

## 2. Justificación

Origen regulatorio. La concentración de datos personales y de registros sensibles del organismo está sujeta a la Ley 25.326, que exige limitar el acceso, acotar la finalidad y poder demostrar el correcto tratamiento (NB-06).

## 3. Ámbito de aplicación

Se evalúa en cada acceso a datos personales de usuarios, en la exportación e importación de relevamientos que contienen esos datos, y en cualquier listado o consulta que los exponga. Aplica de forma transversal a toda la categoría.

## 4. Consecuencia si se viola

Un acceso a datos personales fuera del alcance del rol o del área se rechaza con el código `ACCESO_DATO_PERSONAL_NO_AUTORIZADO` y se registra. Un tratamiento ajeno a la finalidad del relevamiento se considera incumplimiento regulatorio y se bloquea con el código `FINALIDAD_NO_PERMITIDA`.

## 5. CU afectados

CU-03, CU-08, CU-13, CU-14.

## 6. Pruebas que la verifican

- Un usuario solo accede a datos personales de su área.
- Un acceso fuera del alcance del rol o del área queda bloqueado y registrado.
- La exportación de un relevamiento respeta el acotamiento de acceso por rol y área.
- Referencia a casos de prueba previstos en 08, suite de protección de datos personales.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-06 |
