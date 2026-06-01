# CU-03 — Administrar la jerarquía de usuarios y áreas

**Proyecto:** GeoVial
**Documento:** CU-03-administrar-jerarquia-usuarios-areas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que un usuario administrador del sistema dé de alta y de baja usuarios según la jerarquía raíz → jefe general → jefe de área → agente de campo, manteniendo coherente la pertenencia a áreas.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador de jerarquía | Primario | Da de alta y de baja usuarios en el nivel que su rol le permite administrar |
| Sistema de usuarios | Sistema | Valida el nivel jerárquico, persiste el alta o baja, asocia el área y registra la acción en auditoría |

## 3. Precondiciones

- El usuario administrador está autenticado y su rol jerárquico está vigente.
- El nivel que pretende administrar es el inmediato inferior al suyo: el usuario raíz administra al jefe general, el jefe general a los jefes de área, el jefe de área a los agentes de su área.

## 4. Flujo principal

1. El administrador solicita dar de alta un usuario indicando su nivel jerárquico y, si corresponde, su área.
2. El sistema valida que el administrador puede administrar ese nivel (RN-01).
3. El sistema valida que un jefe de área y sus agentes quedan asociados a un área existente.
4. El sistema da de alta al usuario y lo deja vigente.
5. El sistema registra el alta en el registro de auditoría (RN-07) y confirma la operación.

## 5. Flujos alternativos

- 5.A Baja de un usuario. Disparador: el administrador solicita dar de baja un usuario de su nivel administrable. El sistema marca al usuario como no vigente, conserva sus datos personales bajo tratamiento regulado (RN-08) y la información que produjo, y registra la baja en auditoría. Punto de retorno: paso 5.
- 5.B Intervención del usuario raíz por incoherencia. Disparador: el usuario raíz interviene para resolver una incoherencia de jerarquía o de área. El sistema permite la corrección dentro del alcance del rol raíz y la registra en auditoría. Punto de retorno: paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `ACCESO_NO_AUTORIZADO` | El administrador intenta administrar un nivel que no le corresponde (RN-01) | Rechaza la operación y registra el intento |
| `AREA_INEXISTENTE` | Se intenta asociar un jefe de área o un agente a un área que no existe | Rechaza el alta e indica el área inválida |
| `ACCION_NO_AUDITADA` | Una acción administrativa no pudo registrarse en auditoría (RN-07) | Rechaza la operación para no dejar acciones sin trazabilidad |

## 7. Postcondiciones

- Éxito: el usuario queda dado de alta o de baja en el nivel correcto y, si corresponde, asociado a su área; la acción queda auditada.
- Fallo: no se altera la jerarquía ni la pertenencia a áreas; el intento queda registrado cuando corresponde.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe general autenticado | Da de alta un jefe de área asociado al área "Zona Norte" | El sistema crea al jefe de área vigente, asociado a "Zona Norte", y registra el alta en auditoría |
| CA-02 | Un jefe de área autenticado | Intenta dar de alta a otro jefe de área | El sistema rechaza con el código `ACCESO_NO_AUTORIZADO` |
| CA-03 | Un jefe general | Intenta dar de alta un agente asociado a un área inexistente "Zona X" | El sistema rechaza con el código `AREA_INEXISTENTE` |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01, RN-07, RN-08 |
| Historias de usuario a generar | US a generar en 06 (alta y baja jerárquica, asociación a área, intervención raíz) |
| Componentes esperados | Módulo de usuarios y jerarquía (referencia tentativa a 05) |
| Tests previstos | Suite de alta/baja jerárquica y pertenencia a áreas (referencia tentativa a 08) |

## 10. Notas y supuestos

- El alta del jefe general la realiza el usuario raíz; el primer eslabón de la cadena se configura en la puesta en marcha del sistema.
- La baja conserva la información producida por el usuario y sus datos personales bajo el tratamiento de RN-08; no se borran registros históricos.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-01 |

## 13. Interacción multiusuario y concurrencia

Las altas y bajas son operaciones administrativas centralizadas en la web. Si dos administradores del mismo nivel modifican el mismo usuario de forma concurrente, prevalece la última escritura confirmada (coherente con RN-04) y ambas acciones quedan registradas en auditoría, de modo que la secuencia de cambios sea reconstruible.
