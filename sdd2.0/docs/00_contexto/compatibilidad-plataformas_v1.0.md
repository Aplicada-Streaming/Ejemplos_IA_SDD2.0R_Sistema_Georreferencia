# Compatibilidad de Plataformas

**Proyecto:** GeoVial
**Documento:** compatibilidad-plataformas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-31
**Autor:** Product Manager Senior (AG-00) + Mobile UX Lead
**Trazabilidad upstream:** PROJECT-README §1 (sub-proyectos), §12 (compatibilidad), §16 (restricciones); PROJECT-BRIEF §4, §9
**Trazabilidad downstream:** 09_devops

## 1. Resumen ejecutivo

Este documento declara las plataformas, sistemas operativos, runtimes y navegadores soportados por GeoVial en su versión 1, con su versión mínima y la justificación correspondiente.

Se genera de forma explícita por la presencia del sub-proyecto `mobile-app-maui` (App de captura en terreno) declarado en el README, no por el tipo dominante del proyecto. El tipo dominante es `web-monolith`, que por la regla de inclusión/exclusión por tipo (§2.2 de las reglas de la categoría) omitiría este documento; sin embargo, el sub-proyecto móvil tiene restricciones reales de plataforma (versión mínima de Android, prueba por conexión directa al dispositivo, exclusión de iOS y tablets) que deben quedar registradas para la categoría 09. Por eso el documento se produce.

GeoVial se compone de un front web administrativo y de revisión, un servicio de backend con base de datos, y una app móvil de captura. El front web soporta navegadores evergreen en sus dos últimas versiones; el backend y la base de datos corren en contenedores Linux; la app móvil soporta Android desde la versión 8.0 (API 26). iOS, tablets y la distribución por tiendas quedan fuera de alcance en la v1.

## 2. Matriz de compatibilidad

| Componente | Android | Navegadores evergreen | Contenedores Linux | Notas |
|---|---|---|---|---|
| App móvil de captura | Android 8.0 (API 26) mínimo | No aplica | No aplica | Sub-proyecto mobile-app-maui; prueba sobre dispositivo Android por conexión directa |
| Front web administrativo y de revisión | No aplica | Últimas 2 versiones de Chrome, Edge, Firefox y Safari | Empaquetado del front en contenedor | Render del lado del servidor; requiere conexión persistente con el servidor |
| Backend (servicio de aplicación y API) | No aplica | No aplica | Sí (contenedor de backend) | Almacenamiento local de archivos reside sobre el contenedor del backend |
| Base de datos | No aplica | No aplica | Sí (contenedor de base de datos) | Tres contenedores: front, backend y base de datos |

## 3. Restricciones de plataforma justificadas

| Componente | Plataforma | Versión mínima | Justificación |
|---|---|---|---|
| App móvil de captura | Android | 8.0 (API 26) | Versión mínima declarada por el equipo; cubre el parque de dispositivos esperado para el trabajo de campo. Sujeto a confirmación del cliente |
| Front web | Chrome, Edge, Firefox, Safari | Últimas 2 versiones (evergreen) | Soportar solo navegadores con actualización automática evita mantener compatibilidad con versiones obsoletas y reduce el costo de prueba |
| Backend y base de datos | Linux (contenedores) | Tres contenedores (front, backend, base de datos) | El entorno objetivo es contenerizado; el almacenamiento local de archivos se acopla al contenedor del backend salvo que se configure un backend externo |

## 4. Alternativas para plataformas no soportadas

| Plataforma no soportada | Estado en v1 | Justificación | Alternativa |
|---|---|---|---|
| iOS | Fuera de alcance | La depuración y prueba se plantea sobre dispositivo Android por conexión directa; iOS no entra en el ciclo de desarrollo de la v1 | Evaluar soporte en una versión 2.0; mientras tanto, usar dispositivos Android para la captura en terreno |
| Tablets | Fuera de alcance | La captura en campo se diseña para teléfono; las tablets no forman parte del parque de dispositivos objetivo de la v1 | Usar teléfonos Android compatibles; reevaluar tablets en una versión futura si surge la necesidad |
| Distribución por tiendas de aplicaciones | Fuera de alcance | El ciclo de desarrollo prueba la app por conexión directa al dispositivo; la publicación en tiendas no está cubierta en la v1 | Instalación directa en los dispositivos del organismo; evaluar publicación en una versión futura |
| Navegadores no evergreen (versiones obsoletas) | No soportado | Mantener compatibilidad con navegadores sin actualización automática eleva el costo sin valor para un organismo de uso interno | Indicar a los usuarios que actualicen a una versión evergreen soportada |

## 5. Estado de implementación por plataforma

| Componente | Plataforma | Estado |
|---|---|---|
| App móvil de captura | Android 8.0+ | Planificada (fases F2 y F4 del roadmap) |
| Front web | Navegadores evergreen | Planificada (fases F1, F3 y F5 del roadmap) |
| Backend y base de datos | Contenedores Linux | Planificada; el entorno contenerizado se valida hacia las fases finales (F6 del roadmap) |
| iOS / tablets / tiendas | — | Fuera de alcance en v1 |

A la fecha de este documento no hay implementación iniciada; los estados reflejan la planificación del roadmap, no avance verificado.

## 6. Trazabilidad downstream

- Upstream: PROJECT-README §1 (sub-proyecto mobile-app-maui que motiva este documento), §12 (matriz de compatibilidad: Android, navegadores evergreen, contenedores Linux), §16 (restricciones: prueba por conexión directa al dispositivo, entorno contenerizado validado tarde); PROJECT-BRIEF §4 (mapa en web y móvil), §9 (exclusión de plataformas distintas de Android).
- Downstream:
  - 09_devops: toma esta matriz para definir la matriz de sistema operativo, runtime y navegadores en la integración continua, los objetivos de empaquetado por contenedor y la estrategia de prueba sobre Android.

Nota de pendiente: queda por registrar la ADR de plataformas en la categoría 05 (arquitectura técnica), correspondiente a la fase C del flujo. Esta ADR formalizará la decisión de soporte de plataformas aquí declarada (Android 8.0+ como mínimo, navegadores evergreen, contenedores Linux; iOS, tablets y tiendas fuera de alcance v1).
