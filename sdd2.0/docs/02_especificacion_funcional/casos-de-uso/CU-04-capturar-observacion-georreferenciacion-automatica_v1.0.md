# CU-04 — Capturar una observación con asignación automática de coordenadas

**Proyecto:** GeoVial
**Documento:** CU-04-capturar-observacion-georreferenciacion-automatica_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que un agente de campo registre una observación tomando una foto, de modo que el sistema le asigne automáticamente la coordenada geográfica y cree o asocie su marcador, sin carga manual de la ubicación.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Toma la foto y registra la observación con sus comentarios y etiquetas |
| Sistema de captura | Sistema | Resuelve la coordenada al tomar la foto, crea o asocia el marcador y asienta la observación en el relevamiento |

## 3. Precondiciones

- El agente tiene una sesión activa y un relevamiento abierto en estado recolección (RN-05).
- El relevamiento tiene definido su radio de agrupación (RN-02).

## 4. Flujo principal

1. El agente toma una foto en el punto de la obra que quiere registrar.
2. El sistema resuelve la coordenada de captura a partir de los metadatos de ubicación de la foto como fuente primaria (RN-03).
3. El sistema verifica si existe un marcador del relevamiento dentro del radio de agrupación de esa coordenada (RN-02).
4. Si existe, asocia la observación a ese marcador; si no, crea un marcador nuevo en esa coordenada.
5. El agente agrega comentarios y etiquetas a la foto o a la observación.
6. El sistema asienta la observación, su foto, sus comentarios y sus etiquetas en el relevamiento y confirma el registro.

## 5. Flujos alternativos

- 5.A Varias fotos en el mismo punto. Disparador: el agente toma varias fotos dentro del radio de agrupación. El sistema asocia todas las observaciones al mismo marcador. Punto de retorno: paso 6.
- 5.B Coordenada fuera de cualquier marcador existente. Disparador: la coordenada resuelta no cae dentro del radio de ningún marcador. El sistema crea un marcador nuevo. Punto de retorno: paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `OBSERVACION_SIN_GEORREFERENCIA` | La foto no trae metadatos de ubicación y no se resuelve la coordenada (RN-03) | Deriva la observación a la ubicación manual del punto (CU-05) o a la bandeja sin georreferenciar |
| `RELEVAMIENTO_SOLO_LECTURA` | Se intenta capturar sobre un relevamiento cerrado (RN-05) | Rechaza la captura e indica que el relevamiento está cerrado |
| `ACCESO_NO_AUTORIZADO` | El agente no está asignado al relevamiento o no pertenece al área (RN-01) | Rechaza la captura y registra el intento |

## 7. Postcondiciones

- Éxito: la observación queda asentada con su coordenada, asociada a un marcador del relevamiento, con sus fotos, comentarios y etiquetas.
- Fallo: no se asienta la observación, o queda derivada a ubicación manual o a la bandeja sin georreferenciar; no se modifica un relevamiento cerrado.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento en recolección con radio 15 m sin marcadores | El agente toma una foto con metadatos de ubicación en un punto nuevo | El sistema crea un marcador en esa coordenada y asocia la observación |
| CA-02 | Un marcador existente y una foto tomada a 8 m de él, con radio 15 m | El agente registra la observación | El sistema asocia la observación al marcador existente, sin crear uno nuevo |
| CA-03 | Una foto sin metadatos de ubicación | El agente intenta registrar la observación | El sistema responde `OBSERVACION_SIN_GEORREFERENCIA` y deriva a ubicación manual del punto |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-01, RN-02, RN-03, RN-05 |
| Historias de usuario a generar | US a generar en 06 (captura de foto georreferenciada, comentarios y etiquetas) |
| Componentes esperados | Módulo de observaciones y marcadores (referencia tentativa a 05) |
| Tests previstos | Suite de captura georreferenciada y agrupación por radio (referencia tentativa a 08) |

## 10. Notas y supuestos

- La georreferenciación y el radio de agrupación son conceptos del dominio del cliente; el mecanismo de obtención de la coordenada se define en 05.
- La continuidad de la captura cuando no hay señal se detalla en CU-06; este CU describe la captura con independencia del estado de conexión.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-02 |

## 13. Interacción multiusuario y concurrencia

Dos agentes pueden capturar observaciones sobre el mismo relevamiento al mismo tiempo y crear marcadores cercanos. Si al sincronizar quedan dos marcadores dentro de un mismo radio, no se unifican de forma automática (RN-02): se listan como conflicto para que el jefe de área decida (CU-11, CU-12).
