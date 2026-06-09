# Sprint Retrospectiva — Sprint 73

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-73_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-09
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Página chica reusando todo.** El tablero reusó el `GeoVialApiCliente` (login + token + llamadas) y el patrón de las páginas existentes; el único código nuevo de backend fue un método de cliente de una línea efectiva.
- **Cierra la historia del resumen.** El dato de S72 (verificado en vivo) ahora se ve en el front administrativo, sin curl. Reporting tiene su primera pantalla.
- **Sin regresión en el gate.** Al ser UI web, el gate (543) no se tocó; el endpoint que la página consume ya estaba cubierto en S72.

## 2. Qué no salió bien

- **Incidente de tooling: el SDK de .NET en bash quedó en 8.0.** Al cambiar el día, el entorno de la shell bash se refrescó y su `dotnet` pasó a resolver el SDK 8.0.406 (que no soporta `net10.0`), aunque el SDK 10.0 sigue instalado y el `dotnet` de PowerShell lo usa bien. Los builds .NET se hicieron por PowerShell. **Acción:** documentar/forzar el `dotnet` net10 en bash (o usar PowerShell para builds) para no perder tiempo.
- **Verificación de UI limitada.** La página se verifica por build + que la ruta sirva; la interacción completa (login → elegir → resumen) usa el circuito Blazor y no se automatiza en el gate. Una prueba bUnit del componente cubriría el render; queda como mejora.

## 3. Qué probar

- En `/tablero`: login como `jefe.norte`, elegir "Puente Río 12" → ver 2 marcadores / 2 observaciones / 2 fotos y "Carlos → 2" en productividad.
- Elegir otro relevamiento sin acceso (si se pudiera) → mensaje de error, sin datos.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Fijar el `dotnet` net10 en el entorno bash (o estandarizar builds por PowerShell) | AG-09 | 2029-04-27 | Pendiente |
| Reporting #3: resumen **por área** (todos los relevamientos) + exportes (CSV/PDF) | AG-08 | 2029-04-27 | Planificado |
| Reporting #4: mapa de calor de observaciones + gráficos | AG-08 | — | Backlog |
| Prueba bUnit del componente `Tablero` (render del resumen) | AG-08 | — | Backlog |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 72 | Estado |
| --- | --- |
| Reporting #2: UI web del tablero | **Hecho** (este sprint) |
| Reporting #3: resumen por área + exportes | Planificado (próximo) |
| Conteo de fotos/comentarios en la base (optimización) | Backlog |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-09 | Retro del Sprint 73 (reporting #2: UI web del tablero). Bien: página chica reusando el cliente, cierra la historia del resumen. Incidente: SDK de .NET en bash quedó en 8.0 (se usó PowerShell). Verificación de UI Blazor limitada. Generada por AG-07 |
