# RN-01 — Jerarquía y autorización por rol y área

**Proyecto:** GeoVial
**Documento:** RN-01-jerarquia-autorizacion-por-rol_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Enunciado

Todo usuario pertenece a un único nivel de la jerarquía raíz → jefe general → jefe de área → agente de campo, y cada acción sobre el sistema solo es legítima si el nivel del usuario la autoriza y, cuando la acción recae sobre un relevamiento, un agente o una observación, si el recurso pertenece al área del usuario o a un área bajo su responsabilidad.

## 2. Justificación

Origen de negocio y regulatorio. La organización delega la recolección en más personas y necesita acotar quién puede crear, ver, modificar y cerrar cada recurso. El acotamiento de acceso por rol y área es además exigencia de la Ley 25.326 para el tratamiento de datos personales (NB-06).

## 3. Ámbito de aplicación

Se evalúa en cada operación que cree, lea, modifique o elimine usuarios, áreas, relevamientos, asignaciones, observaciones, marcadores, fotos, comentarios y etiquetas, así como en cada acceso autenticado. Momentos: alta y baja de usuarios, asignación de agentes, selección de relevamiento, recolección, revisión, transición de estados, resolución de conflictos y exportación o importación.

## 4. Consecuencia si se viola

El sistema rechaza la operación con el código `ACCESO_NO_AUTORIZADO`, no aplica ningún cambio y registra el intento en el registro de auditoría. Ningún recurso fuera del área del usuario se devuelve ni se lista.

## 5. CU afectados

CU-01, CU-02, CU-03, CU-04, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12, CU-13, CU-14.

## 6. Pruebas que la verifican

- Un jefe de área no accede a relevamientos de otra área (rechazo `ACCESO_NO_AUTORIZADO`).
- Un agente de campo no puede crear relevamientos ni dar de alta usuarios.
- Solo el jefe general da de alta o baja jefes de área; solo el usuario raíz da de alta al jefe general.
- Referencia a casos de prueba previstos en 08, suite de autorización por rol y área.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-01 y NB-06 |
