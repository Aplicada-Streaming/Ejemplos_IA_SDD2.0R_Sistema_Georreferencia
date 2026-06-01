# Contexto del Producto — GeoVial (categoría 00)

**Proyecto:** GeoVial
**Carpeta:** /sdd2.0/docs/00_contexto/
**Estado de la sección:** Propuesto
**Fecha:** 2026-05-31
**Autor:** Product Manager Senior (AG-00) + Analista de Negocio Senior (AG-01)

Esta carpeta reúne los documentos de contexto de GeoVial. Es el inicio de la cadena de trazabilidad: no tiene documentos upstream generados; sus insumos son el PROJECT-BRIEF y el PROJECT-README del intake. Alimenta a las categorías 01, 02, 03, 05, 06, 07, 08, 09 y 11.

## Documentos de la sección

| Orden de lectura | Documento | Propósito | Estado |
|---|---|---|---|
| 1 | [vision-producto_v1.0.md](vision-producto_v1.0.md) | Por qué existe GeoVial, audiencia, propuesta de valor, visión a 3 años, objetivos SMART, métricas, riesgos y glosario del dominio | Propuesto |
| 2 | [alcance-proyecto_v1.0.md](alcance-proyecto_v1.0.md) | Qué entra y qué no entra en la v1, supuestos, restricciones y criterios de aceptación del proyecto | Propuesto |
| 3 | [roadmap-producto_v1.0.md](roadmap-producto_v1.0.md) | Fases de delivery como hitos con criterios de transición verificables y matriz fase-épica-sprint-release | Propuesto |
| 4 | [compatibilidad-plataformas_v1.0.md](compatibilidad-plataformas_v1.0.md) | Plataformas, sistemas operativos, navegadores y versiones mínimas soportadas; alternativas para lo no soportado | Propuesto |
| 5 | [acuerdo-equipo_v1.0.md](acuerdo-equipo_v1.0.md) | Roles, ceremonias, acuerdos operativos, ramas, commits y herramientas del equipo | Propuesto |

Orden de lectura sugerido: visión, alcance, roadmap, compatibilidad y acuerdo de equipo. La visión y el alcance fijan el qué y el porqué en lenguaje de negocio; el roadmap ordena la construcción; compatibilidad y acuerdo de equipo aportan el marco de plataformas y de proceso.

## Stakeholders del proyecto

| Rol | Categoría | Responsabilidad principal |
|---|---|---|
| Usuario raíz (administrador técnico del sistema) | Propietario | Configura el sistema, da de alta al jefe general e interviene para resolver incoherencias |
| Jefe general (responsable funcional del organismo) | Propietario | Administra el alta y baja de los jefes de área; aprueba el brief y la visión |
| Jefe de área (jefe de área administrativa de vialidad) | Implementador / Beneficiario | Crea relevamientos, gestiona agentes de su área, evalúa la información y cierra relevamientos |
| Agente de campo (relevador / cuadrilla técnica) | Beneficiario | Recolecta comentarios y fotos en terreno, etiqueta, comenta y administra fotos de un marcador |
| Área central de evaluación | Beneficiario | Recibe la información recolectada para confeccionar informes rutinarios |
| Equipo de desarrollo (SDD 2.0, 4 personas) | Implementador | Construye y mantiene el sistema |

## Nota de inclusión de compatibilidad-plataformas

El tipo dominante del proyecto es `web-monolith`. Por la regla de inclusión/exclusión por tipo (§2.2 de las reglas de la categoría 00), un `web-monolith` omitiría `compatibilidad-plataformas` salvo soporte a navegadores legacy. Aquí, en cambio, el documento sí se genera: el proyecto declara el sub-proyecto `mobile-app-maui` (App de captura en terreno, PROJECT-README §1) con restricciones reales de plataforma (Android 8.0 mínimo, prueba por conexión directa al dispositivo, exclusión de iOS, tablets y distribución por tiendas en la v1). Esa multiplicidad de artefactos, modelada como sub-proyecto, justifica la inclusión por aplicación de §3.4.

Ningún documento de la sección fue omitido: el tipo y los flags activan los cinco documentos más este README.

## Pendiente registrado

Queda pendiente registrar la ADR de plataformas en la categoría 05 (arquitectura técnica), correspondiente a la fase C del flujo. Esa ADR formalizará la decisión de soporte de plataformas declarada en `compatibilidad-plataformas_v1.0.md`.
