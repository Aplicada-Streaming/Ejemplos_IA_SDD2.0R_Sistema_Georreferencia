# Developer guide — Librería de sincronización GeoVial

**Proyecto:** GeoVial
**Documento:** README.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Technical Writer / Developer Advocate Senior (AG-10), Equipo SDD 2.0
**Audiencia:** Developer integrador que reutiliza la librería de sincronización en otros proyectos

---

Esta carpeta es la developer guide de la **librería de sincronización publicada** `GeoVial.Sync`, no del sistema GeoVial completo. La audiencia es el developer que consume la librería desde su propia aplicación para reutilizar la lógica de sincronización offline-first (subir cambios locales, bajar actualizaciones, consolidar last-write-wins, marcar conflictos).

## Decisión de gating de la categoría 10

GeoVial tiene tipo dominante D8 `web-monolith` (PROJECT-README §1), para el cual la categoría 10 sería **opcional** según §1.2 y §2.2 de `10_rules_developer_guide.md`. Sin embargo, la categoría **se genera** porque concurren dos condiciones:

- El proyecto declara el sub-proyecto `library` (la librería de sincronización `GeoVial.Sync`), que se publica como paquete en GitHub Packages para reuso por terceros (PROJECT-README §1, §5, §10, §14; ADR-07).
- El flag de gating `tiene_portal_developers=true` (PROJECT-README §1) habilita la superficie pública para developers.

Para el sub-proyecto `library`, la categoría 10 es **obligatoria** (§1.2). La variante de especialidad activada es **Technical Writer + SDK Documentation Lead**. El alcance documental se restringe a la superficie pública de la librería (`contratos-abstractions-sync_v1.0.md` de 05); no documenta el monolito web ni la app móvil completa. Esta decisión de gating queda registrada también en `decisiones-proyecto` del intake.

## Índice de artefactos

| Documento | Tipo Diátaxis | Nivel | Para qué |
| --- | --- | --- | --- |
| [conceptos-fundamentales_v1.0.md](conceptos-fundamentales_v1.0.md) | Explanation | Medio | Entender el modelo mental: cola local, subir-bajar, last-write-wins, qué NO hace la librería |
| [guia-onboarding-developer_v1.0.md](guia-onboarding-developer_v1.0.md) | Tutorial | Básico | De cero al primer éxito: Hello world, primer caso real contra mock, puente a la integración |
| [guia-integracion-aplicacion-movil_v1.0.md](guia-integracion-aplicacion-movil_v1.0.md) | How-to | Medio | Integrar la librería en una aplicación móvil offline, paso a paso |
| [referencia-api_v1.0.md](referencia-api_v1.0.md) | Reference | Avanzado | Firma exacta de tipos, métodos, eventos y excepciones (paridad con el contrato de 05) |
| [troubleshooting_v1.0.md](troubleshooting_v1.0.md) | How-to (diagnóstico) | Medio | Resolver errores frecuentes con diagnóstico paso a paso |
| [glosario-tecnico_v1.0.md](glosario-tecnico_v1.0.md) | Reference | Básico | Vocabulario canónico del consumidor |

## Orden de lectura recomendado

1. `guia-onboarding-developer` (Básico) — empezá acá; te deja con la librería funcionando.
2. `conceptos-fundamentales` (Medio) — entendé por qué funciona así antes de integrar.
3. `guia-integracion-aplicacion-movil` (Medio) — integración real en tu aplicación.
4. `referencia-api` (Avanzado) — consultá la firma exacta cuando la necesites.
5. `troubleshooting` y `glosario-tecnico` — consulta puntual, en cualquier momento.

## Prerequisitos

- SDK de .NET (LTS vigente del proyecto, .NET 9 o superior).
- Acceso al paquete `GeoVial.Sync` en GitHub Packages (token de lectura de paquetes; ADR-07).
- Un almacén local en tu aplicación y un backend HTTP de sincronización.

## Quick-start

```bash
dotnet new console -n SyncQuickStart && cd SyncQuickStart
dotnet add package GeoVial.Sync
```

```csharp
using GeoVial.Sync.Abstractions;

IChangeQueue queue = SyncFactory.CreateInMemoryQueue();
await queue.EnqueueAsync(new ChangeRecord(
    Guid.NewGuid().ToString(), OperationType.Create, "Nota", "nota-001",
    DateTimeOffset.UtcNow, """{ "texto": "hola" }"""));

ISyncEngine engine = SyncFactory.CreateEngine(queue, backend);
SyncResult result = await engine.SynchronizeAsync(new SyncOptions(batchSize: 50));
Console.WriteLine($"Confirmados: {result.Confirmed.Count}, conflictos: {result.Conflicts.Count}");
```

Seguí con la [guía de onboarding completa](guia-onboarding-developer_v1.0.md).

---

## Referencias cruzadas

- [contratos-abstractions-sync_v1.0.md](../05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md) — superficie pública documentada (05).
- [ADR-07-libreria-sincronizacion-github-packages_v1.0.md](../05_arquitectura_tecnica/adrs/ADR-07-libreria-sincronizacion-github-packages_v1.0.md) — publicación y gating del sub-proyecto library (05).
- [README de 05](../05_arquitectura_tecnica/README.md) — índice de la arquitectura (05).

## Control de cambios

| Versión | Fecha | Cambio | Autor |
| --- | --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Índice con nivel y orden de lectura, prerequisitos, quick-start y registro de la decisión de gating de la categoría 10. | AG-10 |
