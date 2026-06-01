# ADR-13 — Compatibilidad de plataformas (Android 8.0+, navegadores evergreen, iOS fuera de v1)

**Proyecto:** GeoVial
**Documento:** ADR-13-compatibilidad-plataformas_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Despliegue

## 1. Contexto

El documento `compatibilidad-plataformas_v1.0.md` de la categoría 00 declara las plataformas soportadas y deja pendiente registrar la ADR de plataformas en la categoría 05 (nota de pendiente de §6 de ese documento). La app móvil de captura se prueba sobre Android por conexión directa al dispositivo; el front web soporta navegadores evergreen; el backend y la base de datos corren en tres contenedores Linux; iOS, tablets y la distribución por tiendas quedan fuera de v1 (PROJECT-README §12, §16; PROJECT-BRIEF §9). Esta ADR formaliza esa decisión. NFR asociado: disponibilidad SLO 99% en horario laboral.

## 2. Decisión

Se fija el soporte de plataformas de la v1:

- App móvil: Android 8.0 (API 26) como versión mínima.
- Front web: navegadores evergreen, últimas 2 versiones de Chrome, Edge, Firefox y Safari.
- Backend y base de datos: contenedores Linux (tres contenedores: front, backend, base de datos).
- Fuera de v1: iOS, tablets, distribución por tiendas de aplicaciones y navegadores no evergreen.

## 3. Estado

Aceptado el 2026-06-01. ADR de gobernanza que formaliza la decisión de `compatibilidad-plataformas_v1.0.md` (00).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Android 8.0+, evergreen, iOS fuera de v1 (elegido) | Acota el esfuerzo de prueba; cubre el parque esperado; coherente con la prueba por conexión directa Android | Sin soporte iOS ni tablets en v1 |
| Soportar Android + iOS desde v1 | Cobertura amplia de dispositivos | Duplica el esfuerzo de prueba y empaquetado; el ciclo prueba sobre Android por USB |
| Elevar la versión mínima de Android | Menos compatibilidad legada | Excluiría dispositivos del parque de campo esperado |

## 5. Consecuencias positivas

1. El esfuerzo de prueba se concentra en Android y navegadores evergreen, acotando el costo.
2. La matriz de plataformas queda fijada para la categoría 09 (CI, empaquetado por contenedor).
3. Coherencia con la operación de campo (teléfonos Android) y el entorno contenerizado.

## 6. Consecuencias negativas y trade-offs

1. iOS, tablets y tiendas quedan sin cobertura en v1; aceptado y reevaluable en una v2.0.
2. La versión mínima de Android es un supuesto a validar con el cliente (PROJECT-README §12).

## 7. Implementación

La matriz de compatibilidad de `compatibilidad-plataformas_v1.0.md` §2 gobierna la configuración de empaquetado. `GeoVial.Mobile` targetea Android 8.0+ (API 26). El front web declara el soporte evergreen. La categoría 09 toma esta matriz para la CI, la prueba sobre Android y los objetivos de contenedor.

## 8. Métricas de validación

- La app instala y opera en Android 8.0 (API 26) en el dispositivo de prueba.
- El front web funciona en las últimas 2 versiones de los cuatro navegadores evergreen.
- Disponibilidad del backend SLO 99% en horario laboral (health check del contenedor).

## 9. Referencias

- compatibilidad-plataformas_v1.0.md (00) §2, §3, §4, §6 (nota de pendiente).
- PROJECT-README §12, §16; PROJECT-BRIEF §9.
- Downstream: 09_devops.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. ADR de gobernanza que formaliza la compatibilidad de plataformas de 00 |
