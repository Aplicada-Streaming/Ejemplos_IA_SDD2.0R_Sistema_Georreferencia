# CU-08 — Revisar un relevamiento sobre el mapa por marcadores

**Proyecto:** GeoVial
**Documento:** CU-08-revisar-relevamiento-sobre-mapa_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir que un jefe de área revise un relevamiento sobre el mapa, recorriendo sus marcadores con sus observaciones agrupadas, y que pueda exportarlo o importarlo como unidad completa para resguardarlo o compartirlo con el área central.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Revisa el relevamiento sobre el mapa y exporta o importa el relevamiento completo |
| Sistema de revisión | Sistema | Muestra los marcadores sobre el mapa, agrupa las observaciones y produce o consume el archivo de exportación |
| Área central de evaluación | Secundario | Recibe el relevamiento exportado para confeccionar informes |

## 3. Precondiciones

- El jefe de área está autenticado y el relevamiento pertenece a su área (RN-01).
- El relevamiento está en estado revisión, recolección o cerrado (la revisión es de solo lectura sobre datos consolidados).

## 4. Flujo principal

1. El jefe de área abre un relevamiento de su área sobre el mapa.
2. El sistema muestra los marcadores del relevamiento ubicados según sus coordenadas y la bandeja sin georreferenciar.
3. El jefe selecciona un marcador y el sistema agrupa y muestra sus observaciones con fotos, comentarios y etiquetas.
4. El jefe recorre los marcadores y las observaciones para evaluar el estado de la obra.
5. El jefe confecciona sus informes rutinarios a partir de la vista consolidada (acción externa al sistema).

## 5. Flujos alternativos

- 5.A Exportar el relevamiento completo. Disparador: el jefe solicita exportar el relevamiento. El sistema produce un único archivo comprimido con datos, comentarios, etiquetas y fotos, respetando el acotamiento de acceso por rol y área (RN-08), y registra la exportación en auditoría (RN-07). Punto de retorno: paso 4.
- 5.B Importar un relevamiento completo. Disparador: el jefe importa un archivo comprimido de relevamiento. El sistema reconstruye el relevamiento con sus observaciones, marcadores, fotos, comentarios y etiquetas, valida la pertenencia de área (RN-01) y registra la importación en auditoría (RN-07). Punto de retorno: paso 2.
- 5.C Filtrar por etiquetas. Disparador: el jefe filtra fotos y observaciones por una o más etiquetas. El sistema muestra solo las observaciones que coinciden. Punto de retorno: paso 3.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `ACCESO_NO_AUTORIZADO` | El relevamiento no pertenece al área del jefe (RN-01) | Niega el acceso y registra el intento |
| `ARCHIVO_EXPORTACION_INVALIDO` | El archivo importado no es un relevamiento completo y coherente | Rechaza la importación e indica el motivo, sin alterar datos existentes |
| `ACCION_NO_AUDITADA` | La exportación o importación no pudo registrarse en auditoría (RN-07) | Rechaza la operación para no dejarla sin trazabilidad |

## 7. Postcondiciones

- Éxito: el jefe recorre el relevamiento consolidado sobre el mapa; al exportar obtiene un archivo único; al importar el relevamiento queda reconstruido; las acciones quedan auditadas.
- Fallo: no se entrega ni se reconstruye el relevamiento; el intento queda registrado cuando corresponde.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento "Puente Río 12" en revisión con cinco marcadores de la "Zona Norte" | El jefe de "Zona Norte" lo abre sobre el mapa | El sistema muestra los cinco marcadores ubicados y la bandeja sin georreferenciar |
| CA-02 | Un relevamiento de la "Zona Norte" | El jefe solicita exportarlo | El sistema entrega un único archivo comprimido con datos, comentarios, etiquetas y fotos, y registra la exportación |
| CA-03 | Un relevamiento de la "Zona Sur" | Un jefe de la "Zona Norte" intenta abrirlo | El sistema responde `ACCESO_NO_AUTORIZADO` |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-01, RN-05, RN-07, RN-08 |
| Historias de usuario a generar | US a generar en 06 (revisión sobre mapa, exportar e importar relevamiento, filtrar por etiquetas) |
| Componentes esperados | Módulo de revisión sobre mapa y exportación/importación (referencia tentativa a 05) |
| Tests previstos | Suite de revisión sobre mapa y de exportación/importación (referencia tentativa a 08) |

## 10. Notas y supuestos

- La exportación e importación del relevamiento completo en un único archivo comprimido (Must Have del alcance) se modela como flujos alternativos 5.A y 5.B de este CU, porque es la operación con la que el jefe de área resguarda o comparte lo revisado, con el mismo actor primario y la misma autorización por área. Reconciliación respecto del mapeo de NB-04, que preveía CU-08 para "revisar un relevamiento sobre el mapa por marcadores"; la exportación e importación se absorben sin alterar ese mapeo ni partir el CU, y quedan registradas en la matriz del índice.
- El archivo comprimido es un concepto del dominio del cliente; su formato físico se define en 05.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-04; absorbe exportación e importación por reconciliación de cobertura |

## 13. Interacción multiusuario y concurrencia

La revisión se hace sobre datos consolidados; varios jefes de área revisan en paralelo relevamientos de áreas distintas sin interferencia (RN-01). Si durante la revisión llegan nuevas sincronizaciones de agentes (CU-07), la vista refleja el estado consolidado más reciente, y los choques quedan marcados como conflicto a resolver (CU-12).
