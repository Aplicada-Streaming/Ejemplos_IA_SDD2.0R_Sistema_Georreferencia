# CU-14 — Aplicar autorización por rol y área en cada acceso

**Proyecto:** GeoVial
**Documento:** CU-14-aplicar-autorizacion-rol-area_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Garantizar que cada acceso a un recurso del sistema se autorice según el rol jerárquico del usuario y su área, bloqueando y registrando todo acceso fuera de alcance, como caso de uso transversal de control de acceso y de manejo uniforme de los errores de autorización.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Usuario autenticado | Primario | Intenta acceder a un recurso del sistema |
| Sistema de autorización | Sistema | Evalúa el rol y el área, concede o niega el acceso y registra los accesos fuera de alcance |

## 3. Precondiciones

- El usuario está autenticado con un rol jerárquico vigente (RN-01).
- El recurso solicitado tiene una pertenencia de área conocida.

## 4. Flujo principal

1. El usuario solicita acceder a un recurso (relevamiento, observación, marcador, usuario, registro de auditoría u otro).
2. El sistema determina el rol del usuario y el área del recurso.
3. El sistema evalúa si el rol autoriza la acción y si el recurso pertenece al área del usuario o a un área bajo su responsabilidad (RN-01).
4. Si la autorización procede, el sistema concede el acceso al recurso.
5. Si no procede, el sistema niega el acceso de forma uniforme y registra el intento (RN-08).

## 5. Flujos alternativos

- 5.A Acceso fuera del área. Disparador: el recurso pertenece a un área distinta de la del usuario. El sistema niega el acceso con `ACCESO_NO_AUTORIZADO` y registra el intento (RN-08). Punto de retorno: fin del flujo con rechazo.
- 5.B Acceso a dato personal fuera de alcance. Disparador: el usuario intenta acceder a un dato personal que su rol o área no habilitan. El sistema lo bloquea con `ACCESO_DATO_PERSONAL_NO_AUTORIZADO` y lo registra (RN-08). Punto de retorno: fin del flujo con rechazo.
- 5.C Manejo uniforme de errores de autorización. Disparador: cualquier CU produce un rechazo de autorización. El sistema centraliza la respuesta de error con su código, sin filtrar información del recurso protegido. Punto de retorno: el CU que originó el acceso.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `ACCESO_NO_AUTORIZADO` | El rol o el área no habilitan la acción (RN-01) | Niega el acceso de forma uniforme y registra el intento |
| `ACCESO_DATO_PERSONAL_NO_AUTORIZADO` | Acceso a un dato personal fuera del alcance del rol o del área (RN-08) | Bloquea el acceso al dato y registra el intento |
| `FINALIDAD_NO_PERMITIDA` | Tratamiento de un dato personal ajeno a la finalidad del relevamiento (RN-08) | Bloquea la operación |

## 7. Postcondiciones

- Éxito: el usuario accede solo a los recursos que su rol y área habilitan.
- Fallo: el acceso fuera de alcance se niega de forma uniforme y queda registrado; ningún dato protegido se expone.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de la "Zona Norte" | Intenta abrir un relevamiento de la "Zona Sur" | El sistema responde `ACCESO_NO_AUTORIZADO` y registra el intento |
| CA-02 | Un agente de campo | Intenta consultar datos personales de otro agente | El sistema responde `ACCESO_DATO_PERSONAL_NO_AUTORIZADO` y registra el intento |
| CA-03 | Un jefe de la "Zona Norte" | Accede a un relevamiento de la "Zona Norte" | El sistema concede el acceso al relevamiento |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-06 |
| Reglas de negocio aplicables | RN-01, RN-08 |
| Historias de usuario a generar | US a generar en 06 (autorización por rol y área, bloqueo y registro de accesos fuera de alcance, manejo uniforme de errores) |
| Componentes esperados | Módulo transversal de autorización (referencia tentativa a 05) |
| Tests previstos | Suite de autorización por rol y área y de errores de acceso (referencia tentativa a 08) |

## 10. Notas y supuestos

- Este CU es transversal: lo invocan todos los demás CU para autorizar el acceso, y centraliza el manejo uniforme de los errores de autorización que se repiten en los flujos. Reconciliación respecto del mapeo de NB-06, que preveía CU-14 para "aplicar autorización por rol y área en cada acceso"; el manejo transversal de errores de acceso se absorbe aquí sin alterar ese mapeo, en lugar de crear un CU adicional fuera de la numeración prevista.
- El mecanismo concreto de evaluación de la autorización pertenece a 05; aquí se define la regla funcional de quién accede a qué.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-06; absorbe el manejo transversal de errores de autorización |

## 13. Interacción multiusuario y concurrencia

La autorización se evalúa en cada acceso, de forma independiente por usuario y sesión; varios usuarios concurrentes acceden cada uno solo a su ámbito de rol y área (RN-01). Un cambio de jerarquía o de área (CU-03) aplicado de forma concurrente rige sobre los accesos posteriores a su consolidación, sin afectar retroactivamente accesos ya concedidos.
