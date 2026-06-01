# Conceptos fundamentales — Librería de sincronización GeoVial

**Proyecto:** GeoVial
**Documento:** conceptos-fundamentales_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Technical Writer / Developer Advocate Senior (AG-10), Equipo SDD 2.0
**Tipo Diátaxis:** Explanation
**Audiencia:** Developer integrador que reutiliza la librería de sincronización en otros proyectos
**Nivel:** Medio
**Tiempo estimado de lectura:** 14 min

---

## 1. Concepto central

La librería de sincronización de GeoVial (`GeoVial.Sync`) resuelve un único problema: una aplicación que trabaja sin conexión y necesita converger sus datos con un backend cuando recupera la red. Recibe cambios producidos localmente (altas y ediciones), los encola de forma durable, y al recuperar conexión los sube en orden al backend y baja las actualizaciones remotas. Ante ediciones incompatibles del mismo recurso, consolida por última escritura (last-write-wins) y deja marcado el conflicto para que se resuelva después; nunca descarta un cambio en silencio.

En una frase: la librería transforma una secuencia de cambios locales encolados en un estado consolidado contra un backend, reportando los conflictos que la consolidación no puede dirimir por sí sola.

La librería es agnóstica del dominio de su consumidor. GeoVial la usa para sincronizar relevamientos, marcadores, fotos y comentarios capturados en terreno, pero la superficie pública trabaja sobre un registro de cambio genérico (`ChangeRecord`), de modo que cualquier aplicación con un modelo de "cambios encolados que convergen contra un servidor" puede reutilizarla.

## 2. Modelo mental

El flujo principal tiene tres etapas: encolar localmente, subir lo local, bajar lo remoto. La sincronización siempre sube antes de bajar, porque ese orden es una decisión del dominio (sube primero lo capturado en terreno, luego incorpora lo que cambió en el servidor mientras el dispositivo estaba sin red).

```text
 [Consumidor]                 [GeoVial.Sync]                       [Backend]
      |                              |                                  |
  1.  | Enqueue(ChangeRecord) ------>| IChangeQueue (cola durable)      |
      |                              |   ordena por Timestamp           |
      |                              |                                  |
  2.  |                  IConnectivityMonitor: recupera conexión        |
      |                              |                                  |
  3.  | Synchronize(options) ------->| ISyncEngine                      |
      |                              |  a) sube pendientes ------------>| ISyncBackendClient.Push
      |                              |  b) consolida last-write-wins    |
      |                              |  c) baja actualizaciones <-------| ISyncBackendClient.Pull
      |                              |  d) reporta conflictos --------->| IConflictReporter
      |                              |  e) confirma y vacía la cola     |
  4.  | <----------- SyncResult { Confirmed, Conflicts, Updates }       |
```

| Concepto | Qué es | Ejemplo en GeoVial |
| --- | --- | --- |
| Cola de cambios local | Almacén durable y ordenado de los cambios producidos sin red, identificados de forma única para idempotencia. La modela `IChangeQueue` sobre `ChangeRecord`. | Cada foto, comentario o etiqueta capturada en terreno se encola como un `ChangeRecord` en SQLite local. |
| Subir locales | Primera fase del pipeline: enviar al backend los cambios encolados, en orden de marca temporal. | Al recuperar señal, la app sube las observaciones de la jornada antes de mirar el servidor. |
| Bajar remotos | Segunda fase: traer las actualizaciones de los recursos que el consumidor tiene asignados. | La app baja las últimas actualizaciones de los relevamientos asignados al agente. |
| Consolidación last-write-wins | Criterio determinista para resolver choques de campo: prevalece el cambio de `Timestamp` más reciente; el recurso queda marcado como conflicto. | Dos cuadrillas editan el mismo comentario offline; gana la edición más reciente y queda marcado el conflicto. |
| Conflicto | Resultado que la consolidación no dirime sola y que requiere decisión humana posterior. Se expone vía `IConflictReporter` como `ConflictInfo`. | Marcadores que quedan dentro de un mismo radio; o ediciones de campo en choque. |
| Idempotencia por `ChangeId` | Garantía de que reintentar la subida de un cambio ya aplicado no lo aplica dos veces. | Una sincronización que se cortó a la mitad se reanuda sin duplicar lo ya confirmado. |

## 3. Decisiones de diseño relevantes para el consumidor

Las decisiones siguientes afectan cómo se usa la librería, no su implementación interna. Cada una cita su ADR de origen en la arquitectura (05).

| Decisión | Por qué importa al consumidor | ADR fuente (05) |
| --- | --- | --- |
| Cola de cambios durable con identificador único de idempotencia | El consumidor debe asignar (o dejar que la librería asigne) un `ChangeId` por cambio; reintentar una subida con el mismo `ChangeId` es seguro y no duplica. La durabilidad implica que los cambios sobreviven al cierre de la app antes de sincronizar. | ADR-05 |
| Last-write-wins a nivel de campo con marca de conflicto | El consumidor no obtiene merge automático ni resolución silenciosa: ante un choque gana la marca temporal más reciente y el recurso queda señalado como conflicto. Resolver el conflicto es responsabilidad del consumidor (en GeoVial, desde la web). | ADR-06 |
| Superficie pública estable en la capa Abstractions, versionada con SemVer | El consumidor programa contra interfaces (`ISyncEngine`, `IChangeQueue`, etc.), no contra implementaciones concretas. Un breaking change de esa superficie bumpea MAJOR; los cambios aditivos bumpean MINOR. La implementación interna puede cambiar sin afectar al consumidor. | ADR-07 |

El detalle completo de cada decisión vive en la arquitectura (05); aquí solo se documenta el efecto visible para quien consume la librería.

## 4. Vocabulario

Subconjunto crítico para entender la librería. El vocabulario canónico y completo está en [glosario-tecnico_v1.0.md](glosario-tecnico_v1.0.md).

| Término | Definición operativa | Ejemplo |
| --- | --- | --- |
| `change-record` | Unidad de cambio encolada: identificador, tipo de operación, entidad, referencia, marca temporal y payload. | Un alta de comentario capturada offline. |
| `change-queue` | Cola local durable y ordenada de `change-record` pendientes de sincronizar. | La cola SQLite que drena al recuperar señal. |
| `sync-engine` | Orquestador del pipeline subir-consolidar-bajar, idempotente por `ChangeId`. | El componente que dispara la sincronización al recuperar conexión. |
| `last-write-wins` | Criterio de consolidación: prevalece el cambio de marca temporal más reciente. | Resolución de dos ediciones del mismo campo. |
| `conflict-info` | Conflicto reportado para resolución posterior: de campo o de marcadores en un mismo radio. | Dos marcadores casi superpuestos listados como conflicto. |

## 5. Qué NO hace la librería

Delimita la responsabilidad del consumidor frente a la de la librería, para evitar expectativas falsas.

| La librería NO hace | Responsabilidad del consumidor |
| --- | --- |
| No persiste el modelo de dominio del consumidor. Solo persiste la cola de `ChangeRecord`. | El consumidor mantiene su propio almacén de entidades (en GeoVial, SQLite local). |
| No resuelve los conflictos: los detecta y los reporta. | El consumidor decide cómo y dónde resolverlos (en GeoVial, el jefe de área desde la web, CU-12). |
| No hace merge automático por campo (no es CRDT ni operacional). Aplica last-write-wins. | El consumidor asume el trade-off de last-write-wins; los cambios concurrentes descartados quedan marcados, no perdidos. |
| No implementa el transporte concreto contra el backend. Define el puerto `ISyncBackendClient`. | El consumidor (o GeoVial) provee la implementación REST contra el backend objetivo. |
| No gestiona autenticación ni sesión. | El consumidor inyecta credenciales/token válidos al cliente de backend antes de sincronizar. |
| No comprime ni redimensiona binarios (fotos). | El consumidor acota el payload antes de encolar el cambio. |
| No garantiza orden entre dispositivos distintos. Garantiza idempotencia por `ChangeId` y orden por `Timestamp` dentro de una cola. | El consumidor asume convergencia eventual entre dispositivos. |

---

## Referencias cruzadas

- [contratos-abstractions-sync_v1.0.md](../05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md) — superficie pública que estos conceptos describen (05).
- [ADR-05-sqlite-cola-cambios-offline_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-05-sqlite-cola-cambios-offline_v1.0.md) — decisión de cola durable (05).
- [ADR-06-conflictos-last-write-wins-override-manual_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-06-conflictos-last-write-wins-override-manual_v1.0.md) — decisión de consolidación (05).
- [ADR-07-libreria-sincronizacion-github-packages_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-07-libreria-sincronizacion-github-packages_v1.0.md) — publicación y versionado (05).
- [CU-07-sincronizar-cambios-locales_v1.0.md](../02_especificacion_funcional/casos-de-uso/CU-07-sincronizar-cambios-locales_v1.0.md) — caso de uso del pipeline de sincronización (02).
- [referencia-api_v1.0.md](referencia-api_v1.0.md) y [glosario-tecnico_v1.0.md](glosario-tecnico_v1.0.md) — referencia y vocabulario (10).

## Control de cambios

| Versión | Fecha | Cambio | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Concepto central, modelo mental, decisiones de diseño (ADR-05/06/07), vocabulario y delimitación de responsabilidades. | AG-10 |
