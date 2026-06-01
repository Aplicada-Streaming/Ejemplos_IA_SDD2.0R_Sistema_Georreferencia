# CU-05 — Ubicar manualmente el punto cuando falta la posición de la foto

**Proyecto:** GeoVial
**Documento:** CU-05-ubicar-manualmente-punto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que un usuario habilitado ubique sobre el mapa el punto de una observación cuya foto no trae metadatos de ubicación, priorizando los metadatos cuando existan, y dejar la observación en la bandeja sin georreferenciar cuando no se ubica.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo o jefe de área | Primario | Ubica el punto de la observación sobre el mapa o la deja sin georreferenciar |
| Sistema de georreferenciación | Sistema | Verifica los metadatos, agrupa por radio, asienta la coordenada manual o deriva a la bandeja sin georreferenciar |

## 3. Precondiciones

- Existe una observación con foto sin coordenada resuelta dentro de un relevamiento no cerrado (RN-05).
- El relevamiento tiene definido su radio de agrupación (RN-02).

## 4. Flujo principal

1. El sistema detecta que la foto carece de metadatos de ubicación y solicita ubicar el punto (RN-03).
2. El usuario coloca el punto sobre el mapa en el lugar de la observación.
3. El sistema verifica si la coordenada manual cae dentro del radio de agrupación de un marcador existente (RN-02).
4. Si cae, asocia la observación a ese marcador; si no, crea un marcador nuevo en la coordenada indicada.
5. El sistema asienta la coordenada manual en la observación y confirma la georreferenciación.

## 5. Flujos alternativos

- 5.A Carga manual desde la web priorizando metadatos. Disparador: el jefe de área carga manualmente una foto que sí trae metadatos de ubicación. El sistema usa esos metadatos como fuente primaria (RN-03) y solo pide ubicación manual si faltan. Punto de retorno: paso 3.
- 5.B La observación queda sin georreferenciar. Disparador: el usuario no ubica el punto. El sistema deriva la observación a la bandeja sin georreferenciar del relevamiento, conservando su foto, comentarios y etiquetas. Punto de retorno: fin del flujo.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `OBSERVACION_SIN_GEORREFERENCIA` | La foto no trae metadatos y el usuario no ubica el punto (RN-03) | Deriva la observación a la bandeja sin georreferenciar |
| `FUENTE_UBICACION_INCORRECTA` | Se intenta ubicar manualmente una foto que trae metadatos de ubicación válidos (RN-03) | Rechaza la ubicación manual y usa los metadatos como fuente primaria |
| `RELEVAMIENTO_SOLO_LECTURA` | Se intenta georreferenciar sobre un relevamiento cerrado (RN-05) | Rechaza la operación |

## 7. Postcondiciones

- Éxito: la observación queda con coordenada (manual o de metadatos) y asociada a un marcador.
- Fallo: la observación queda en la bandeja sin georreferenciar o no se modifica si el relevamiento está cerrado.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una observación con foto sin metadatos de ubicación en un relevamiento con radio 15 m | El usuario coloca el punto a 5 m de un marcador existente | El sistema asocia la observación a ese marcador con la coordenada manual |
| CA-02 | Una foto cargada desde la web que sí trae metadatos de ubicación | El jefe de área intenta ubicarla manualmente | El sistema responde `FUENTE_UBICACION_INCORRECTA` y usa los metadatos |
| CA-03 | Una observación con foto sin metadatos | El usuario no ubica el punto | El sistema deriva la observación a la bandeja sin georreferenciar con el código `OBSERVACION_SIN_GEORREFERENCIA` |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-02, RN-03, RN-05 |
| Historias de usuario a generar | US a generar en 06 (ubicación manual, carga manual priorizando metadatos, bandeja sin georreferenciar) |
| Componentes esperados | Módulo de georreferenciación y bandeja sin georreferenciar (referencia tentativa a 05) |
| Tests previstos | Suite de ubicación manual y prioridad de metadatos (referencia tentativa a 08) |

## 10. Notas y supuestos

- La bandeja sin georreferenciar es una agrupación lógica de observaciones pendientes de ubicación dentro de un relevamiento, no un relevamiento aparte.
- El concepto de metadatos de ubicación de la foto (EXIF) y el de radio pertenecen al lenguaje del cliente; el detalle de su lectura y cálculo se define en 05.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-02 |

## 13. Interacción multiusuario y concurrencia

La ubicación manual puede realizarse desde la web por el jefe de área mientras agentes recolectan en campo. Si dos usuarios ubican el punto de la misma observación de forma concurrente, prevalece la última escritura confirmada (RN-04) y, si la consolidación deja dos marcadores dentro de un mismo radio, se listan como conflicto (RN-02).
