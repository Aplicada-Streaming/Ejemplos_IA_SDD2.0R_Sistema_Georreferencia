# US-28 — Importar el relevamiento completo desde un archivo

**Proyecto:** GeoVial
**Documento:** US-28-importar-relevamiento-completo-archivo_v1.0.md
**Versión:** 1.0
**Estado:** Ready
**Fecha:** 2026-06-01
**Autor:** Equipo GeoVial (AG-06)
**Épica:** EP-07 Exportación e importación
**Prioridad MoSCoW:** Must
**Estimación:** 8 SP (Fibonacci)

## 1. Historia

Como jefe de área, quiero importar un relevamiento completo desde un único archivo, para reconstruirlo en el sistema cuando lo recibo resguardado o desde otra instancia.

## 2. Contexto

El alcance Must del BRIEF incluye importar un relevamiento completo en un único archivo. CU-08 (flujo 5.B) reconstruye el relevamiento con observaciones, marcadores, fotos, comentarios y etiquetas, valida la pertenencia de área (RN-01) y la coherencia del manifiesto, y registra la importación (RN-07). Es la contraparte de la exportación (US-27).

## 3. Criterios de aceptación

- Given un archivo de relevamiento completo y coherente de la "Zona Norte", When el jefe de "Zona Norte" lo importa, Then el sistema reconstruye el relevamiento con sus marcadores, fotos, comentarios y etiquetas, y registra la importación.
- Given un archivo que no es un relevamiento completo y coherente, When el jefe intenta importarlo, Then el sistema responde `ARCHIVO_EXPORTACION_INVALIDO` y no altera los datos existentes.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-04 |
| CU cubiertos | CU-08 |
| BT derivadas | BT-07, BT-09, BT-18, BT-21 |
| Tests previstos | acceptance/AT-08-importacion |

## 5. Prioridad y estimación

Must: la importación completa el flujo de resguardo y traspaso del alcance Must. 8 SP (Fibonacci): validación del manifiesto, reconstrucción de entidades y binarios, autorización por área y auditoría.

## 6. DoR check

- [x] Criterios de aceptación en Given/When/Then
- [x] Estimada en SP (Fibonacci)
- [x] CU relacionado identificado (CU-08)
- [x] Reglas de negocio identificadas (RN-01, RN-07, RN-08)
- [x] Dependencia con US-27 (formato de archivo) declarada
- [x] Valor para el rol explícito

## 7. Notas y supuestos

La importación rechaza archivos incoherentes sin tocar datos existentes. El formato del archivo es el mismo que produce US-27.
