# Sprint Retrospectiva — Sprint 11

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-11_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Separar el valor (librería `GeoVial.Sync` + cola SQLite, testeables sin MAUI) de la cáscara móvil (que requiere el workload) permitió entregar y verificar el núcleo aunque el empaquetado del APK no esté disponible en el entorno; el gate de pruebas quedó verde e independiente de la plataforma móvil.
- Mantener la superficie pública de la librería fiel al contrato `contratos-abstractions-sync` (nombres en inglés, tipos versionables) deja `GeoVial.Sync` lista para publicarse como paquete (ADR-07) sin acoplarse al backend.
- Probar el cliente REST contra un `HttpMessageHandler` de prueba verificó el mapeo al contrato `/sync` sin necesidad de levantar el backend, llevando la cobertura de la librería a 95 %.
- El spike confirmó que el contrato `/sync` del Sprint 09 es consumible end-to-end desde el cliente, validando la decisión de construir primero el backend de consolidación.

## 2. Qué no salió bien

- El empaquetado completo de la app Android no se pudo construir en el entorno (falta el Android SDK provisionado y el RID `android-arm64`); el spike entregó la cáscara wired y compilable, pero no un APK demostrable. Es un hallazgo esperado del spike, no un fallo de diseño.
- El cliente REST cubre solo la entidad "comentario" (la que el backend consolida hoy); a medida que el backend amplíe el last-write-wins a otras entidades, el cliente y el payload deberán extenderse en paralelo.

## 3. Qué probar

- Provisionar un agente de CI (o un entorno de desarrollo) con el Android SDK y el JDK para construir y, eventualmente, ejecutar la app MAUI sobre un emulador.
- Definir un esquema de payload por entidad en la librería (no solo "comentario") alineado con la generalización del last-write-wins del backend.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Provisionar Android SDK + JDK en un agente para construir/ejecutar `GeoVial.Mobile` | AG-09 | 2026-11-21 | Pendiente |
| Construir US-16 (captura offline) y US-19 (conectividad y sync automática) sobre `GeoVial.Sync` | AG-08 (móvil) | 2026-11-21 | Pendiente |
| Publicar `GeoVial.Sync` como paquete preview en GitHub Packages (ADR-07) | AG-05 | 2026-11-21 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 10 | Estado actual |
| --- | --- |
| Agregar paginación a la consulta de auditoría | Pendiente |
| Incorporar un proyecto móvil (MAUI/SQLite) para habilitar EP-04 | Completada (proyecto `GeoVial.Mobile` creado y cableado; empaquetado APK pendiente de SDK provisionado) |
| Externalizar el catálogo de finalidades a configuración | Pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 11 con 3 acciones nuevas y seguimiento de las del Sprint 10 (proyecto móvil incorporado). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
