# NB-06 — Trazabilidad de acciones y protección de datos personales

| Campo | Valor |
| --- | --- |
| Proyecto | GeoVial |
| Documento | NB-06-trazabilidad-proteccion-datos-personales_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-05-31 |
| Autor | Analista de Negocio Senior (AG-01), Equipo SDD 2.0 |
| Trazabilidad upstream | PROJECT-BRIEF §10, §2; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-13, CU-14 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita poder dar cuenta de quién hizo qué y cuándo dentro del sistema, y proteger los datos personales de los agentes y los registros del organismo, porque el tratamiento de esa información está sujeto a la Ley 25.326 de Protección de Datos Personales. Al delegar la recolección en más personas y mover registros entre el campo y el organismo, la cantidad de datos personales y de acciones administrativas que se manejan crece, y con ella la exigencia de control.

El dolor concreto es que, sin trazabilidad ni control de acceso por rol, la organización no puede demostrar el correcto tratamiento de los datos ante una auditoría ni acotar quién accede a qué. Esta es una necesidad defensiva: no agrega una capacidad visible al usuario final, pero sin ella el organismo queda expuesto a incumplir su marco regulatorio y a perder la confianza en el resguardo de la información que recolecta y centraliza.

Si esta necesidad no se resuelve, el sistema concentra datos personales y registros sensibles sin capacidad de auditarlos ni de limitar su acceso, lo que constituye un riesgo legal y reputacional para el organismo titular.

## 2. Ejemplo de uso desde la perspectiva del negocio

El organismo recibe una consulta sobre el tratamiento de los datos personales de sus agentes y necesita demostrar quién accedió a esa información y qué acciones administrativas se realizaron en el último año. Sin la necesidad resuelta, no hay forma de reconstruir ese registro. Con la necesidad resuelta, el responsable consulta el registro de accesos y acciones, acota el acceso por rol y demuestra que cada agente solo ve lo que le corresponde según su área.

## 3. Impacto

- Permite demostrar el correcto tratamiento de datos personales ante una auditoría.
- Acota el acceso a la información según el rol y el área de cada usuario.
- Deja registro de las acciones administrativas y los accesos por un período definido.
- Si no se resuelve, el organismo queda expuesto a incumplir la Ley 25.326 y a riesgo reputacional.
- Refuerza la confianza interna en el resguardo de la información recolectada y centralizada.

## 4. Problema específico que resuelve

- No hay registro de quién accede a la información ni de qué acciones administrativas se ejecutan.
- No se puede demostrar el correcto tratamiento de datos personales ante una auditoría.
- Sin control de acceso por rol y área, cualquier usuario podría ver información que no le corresponde.
- No existe una política de retención del registro de accesos y acciones alineada al marco legal.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Cobertura de auditoría | Porcentaje de acciones administrativas y accesos registrados | 100 % | desde el lanzamiento |
| Retención del registro | Período de conservación del registro de accesos y acciones | ≥ 12 meses | desde el lanzamiento |
| Control de acceso por rol | Porcentaje de accesos fuera del alcance del rol o del área bloqueados | 100 % | desde el lanzamiento |
| Capacidad de respuesta a auditoría | Tiempo para reconstruir el historial de accesos de un dato solicitado | ≤ 1 día hábil | continuo |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Jefe general | Propietario | Responde por el cumplimiento de la Ley 25.326 ante el organismo |
| Equipo de desarrollo | Implementador | Construye el registro de accesos, la retención y el control de acceso por rol |
| Usuario raíz | Beneficiario | Configura el sistema y consulta el registro para resolver incoherencias |
| Agente de campo | Beneficiario | Es titular de datos personales protegidos y accede solo a lo que le corresponde |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-06 | CU-13 registrar accesos y acciones administrativas con retención | a generar |
| NB-06 | CU-14 aplicar autorización por rol y área en cada acceso | a generar |

## 8. Dependencias con otras NB

Depende de NB-01 (la jerarquía de usuarios y áreas es la base sobre la que se aplica el control de acceso). No es prerequisito de otras NB; las atraviesa transversalmente como necesidad defensiva.

## 9. Prioridad MoSCoW

Must Have. El tratamiento de datos personales bajo la Ley 25.326 es una obligación regulatoria; sin trazabilidad ni control de acceso el sistema no puede operar de forma legítima.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-31 | Versión inicial generada por AG-01 a partir de los intakes y la categoría 00 |
