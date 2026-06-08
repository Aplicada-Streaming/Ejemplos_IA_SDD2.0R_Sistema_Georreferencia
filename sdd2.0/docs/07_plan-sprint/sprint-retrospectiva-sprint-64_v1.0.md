# Sprint Retrospectiva — Sprint 64

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-64_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **Entorno sobre base real sin romper el gate.** El riesgo central —que la cadena de SQL en `Development` arrastrara a las pruebas de integración (que hacen `UseEnvironment("Development")`) contra SQL Server— se atajó con `FabricaPruebas`, que fuerza InMemory. El gate quedó verde corriendo en memoria (489).
- **Seed idempotente en código.** `SeedDesarrollo` reemplaza al script externo `seed-datos-prueba.py` (que duplicaba sobre base persistente) y deja siempre el set limpio de DATOS-DE-PRUEBA.md; seguro de re-ejecutar en cada arranque.
- **Diagnóstico preciso del blanco de Captura.** Con captura reducida (para no exceder el límite del lector de imágenes) + dump de la jerarquía se confirmó que el WebView quedaba en blanco por falta de altura definida (Leaflet `height:100%`), no por datos ni por red.

## 2. Qué no salió bien

- **Deuda heredada de S62 (regresión).** El mapa en Captura nunca había renderizado bien (fila `Auto`); la verificación on-device de S62 lo dio por bueno ("2 pines") probablemente con datos/tamaño distintos. Recordatorio: verificar el layout también con relevamiento **sin marcadores**.
- **Base local contaminada.** La base `GeoVial` traía basura de pruebas manuales previas (usuarios `ag11761`, `pedro_66122`, relevamiento "TRIGGER bug", etc.), que obligó a recrearla. Conviene un script de reset de la base de desarrollo.
- **Reorganización de ramas.** Los cambios se hicieron por error sobre la rama de S63 (PR #78 abierto); hubo que stashearlos, mergear #78 y rebasar S64 a main. Recordatorio: arrancar cada sprint desde `main` con la rama nueva creada de entrada.

## 3. Qué probar

- Backend: reiniciar el backend **no** pierde datos ni los duplica; `campo1` mantiene sus 2 relevamientos.
- On-device: la pestaña Captura muestra el mapa (no en blanco) incluso con un relevamiento sin marcadores; "Tomar foto" queda a la vista.
- Gate: corre sin SQL Server (en memoria) y queda verde.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Script de reset de la base de desarrollo (drop + recrear limpia) | AG-09 | 2028-12-08 | Pendiente |
| Script único de arranque on-device (backend SQL + adb reverse + login) | AG-09 | 2028-12-08 | Pendiente (reiterado) |
| Verificar layouts móviles también con datos vacíos (sin marcadores) | AG-08 | 2028-12-08 | Planificado |
| Pasada de accesibilidad/UI dedicada: H-14 (live regions) + H-07 (tabs) | AG-08 | 2028-12-08 | Pendiente (reiterado de S63) |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 63 | Estado |
| --- | --- |
| Script único de verificación on-device | Pendiente (reiterado) |
| Pasada de accesibilidad/UI dedicada (H-14 + H-07) | Pendiente (reiterado) |
| Evolución de H-01 (coordenada desde el pin embebido) | Pendiente |
| Control de mapa compartido | Pendiente (reiterado) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Retro del Sprint 64 (base real + seed idempotente + gate aislado; fix del mapa de Captura). Bien: gate intacto vía `FabricaPruebas`, seed en código. Fricciones: regresión heredada de S62, base contaminada, reorganización de ramas. Generada por AG-07 |
