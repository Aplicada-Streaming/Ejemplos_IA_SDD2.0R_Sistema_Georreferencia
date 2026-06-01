# CU-13 — Registrar accesos y acciones administrativas con retención

**Proyecto:** GeoVial
**Documento:** CU-13-registrar-accesos-acciones-administrativas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Registrar de forma inalterable cada acceso autenticado y cada acción administrativa, con su autor, momento y operación, y conservar ese registro por el período legal, para que el usuario raíz pueda demostrar el correcto tratamiento de datos ante una auditoría.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Usuario raíz | Primario | Consulta el registro de accesos y acciones para responder a auditorías |
| Sistema de auditoría | Sistema | Asienta cada evento de acceso y acción administrativa, lo conserva por el período legal y lo deja consultable |

## 3. Precondiciones

- El usuario raíz está autenticado con su rol vigente (RN-01).
- El sistema tiene en operación el registro de auditoría sobre las acciones administrativas (RN-07).

## 4. Flujo principal

1. Un usuario realiza un acceso autenticado o una acción administrativa (altas, bajas, asignaciones, transiciones, resoluciones, exportación o importación).
2. El sistema asienta el evento con autor, momento y operación en el registro de auditoría (RN-07).
3. El sistema conserva el registro por un período no menor a doce meses, sin permitir su alteración ni eliminación (RN-07).
4. El usuario raíz consulta el registro filtrando por usuario, recurso o rango de fechas.
5. El sistema devuelve los eventos que cumplen el filtro para reconstruir el historial solicitado.

## 5. Flujos alternativos

- 5.A Reconstrucción del historial de un dato. Disparador: una auditoría pide el historial de accesos a un dato personal. El sistema reúne todos los accesos registrados a ese dato dentro del período de retención (RN-08). Punto de retorno: paso 5.
- 5.B Acción no registrable. Disparador: una acción administrativa no puede asentarse en auditoría. El sistema rechaza la acción para no dejarla sin trazabilidad (RN-07). Punto de retorno: fin del flujo con rechazo.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `ACCION_NO_AUDITADA` | Una acción administrativa no pudo registrarse (RN-07) | Rechaza la acción para no dejarla sin trazabilidad |
| `AUDITORIA_INMUTABLE` | Se intenta alterar o eliminar un registro dentro del período de retención (RN-07) | Rechaza la alteración y conserva el registro original |
| `ACCESO_NO_AUTORIZADO` | Un usuario sin rol raíz intenta consultar el registro completo (RN-01) | Niega la consulta y registra el intento |

## 7. Postcondiciones

- Éxito: cada acceso y acción queda registrado, conservado e inalterable por el período legal, y el usuario raíz puede reconstruir el historial solicitado.
- Fallo: una acción no registrable se rechaza; ningún registro existente se altera ni elimina.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe general que da de alta un jefe de área | Se ejecuta el alta | El sistema asienta un registro con autor, momento y operación "alta de jefe de área" |
| CA-02 | Un registro de auditoría de hace tres meses | Un usuario intenta modificarlo | El sistema responde `AUDITORIA_INMUTABLE` y conserva el registro |
| CA-03 | Una auditoría que pide los accesos a un dato del último año | El usuario raíz consulta el registro por ese dato | El sistema devuelve todos los accesos registrados dentro de los doce meses |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-06 |
| Reglas de negocio aplicables | RN-01, RN-07, RN-08 |
| Historias de usuario a generar | US a generar en 06 (registro de accesos y acciones, retención, consulta del historial) |
| Componentes esperados | Módulo de auditoría y retención (referencia tentativa a 05) |
| Tests previstos | Suite de auditoría, inmutabilidad y retención (referencia tentativa a 08) |

## 10. Notas y supuestos

- El registro es defensivo: no agrega una capacidad visible al usuario final, pero es condición de operación legítima bajo la Ley 25.326.
- La retención mínima es de doce meses; un período mayor es admisible y no contradice esta especificación.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-06 |

## 13. Interacción multiusuario y concurrencia

El registro de auditoría recibe eventos de múltiples usuarios en paralelo; cada evento se asienta de forma independiente con su autor y momento, de modo que la secuencia temporal de acciones concurrentes sea reconstruible. La inmutabilidad (RN-07) garantiza que ningún usuario, ni siquiera concurrente, altere registros ya asentados.
