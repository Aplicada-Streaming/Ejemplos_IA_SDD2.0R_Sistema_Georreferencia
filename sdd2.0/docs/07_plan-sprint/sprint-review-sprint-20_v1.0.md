# Sprint Review — Sprint 20

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-20_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-20_v1.0.md`:

> Endurecer el MVP con pruebas de integración del camino completo por HTTP, verificando de punta a punta los flujos centrales del producto sobre la API real (WebApplicationFactory + proveedor en memoria), con un helper que arma el escenario completo (área → agente con credencial → relevamiento asignado) que hasta ahora faltaba.

Veredicto: Cumplido.

Explicación corta: un helper de escenario siembra en el proveedor en memoria (vía el `IServiceScope` de la factory) un área, un agente de campo con credencial y un relevamiento asignado, listo para capturar. Sobre esa base, tres pruebas E2E recorren los flujos centrales por HTTP: el happy-path de captura (login → capturar con EXIF → `FotoId` y marcador → subir el binario → descargarlo y comparar → la revisión muestra el marcador con su foto → comentar → la revisión refleja el comentario), la ubicación manual (captura sin GPS → bandeja → ubicar → la revisión muestra el marcador y la bandeja queda vacía) y la autorización (un agente de otra área no captura, RN-01). Cierra la deuda de E2E señalada en las retrospectivas de los Sprints 15, 18 y 19.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| E2E | Prueba | Happy-path de captura completo por HTTP (8 pasos, de login a comentario reflejado en la revisión) | Confianza de punta a punta |
| E2E | Prueba | Ubicación manual de una observación sin georreferenciar, verificada por HTTP | Flujo alternativo cubierto |
| E2E | Prueba | Un agente de otra área no captura (autorización por área) | Seguridad verificada E2E |

## 3. Feedback recibido

- Los flujos centrales del MVP quedan verificados de punta a punta sobre la API real, no solo a nivel de handler; sube la confianza para el primer release.
- El helper de escenario completo (que faltaba desde el arranque) habilita más pruebas E2E futuras a bajo costo.
- No se agregó funcionalidad: el sprint consolidó lo entregado, cerrando deuda de calidad de tres retros.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 8 |
| Puntos completados | 8 |
| Velocity efectiva | 8 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 302 verdes (269 unitarias + 33 de integración), +3 respecto del Sprint 19 (las tres E2E). El gate de cobertura de dominio/aplicación se mantiene (sin lógica nueva). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| BT-E2E | Tarea | Aceptada (helper de escenario completo + 3 pruebas E2E por HTTP de los flujos centrales) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 20 se traslada. |

El E2E del ciclo de edición en conflicto (sync con colisión → listar → confirmar) y el mapa interactivo quedan en el backlog.

## 7. Decisiones tomadas durante el review

- Sembrar el escenario completo vía el `DbContext` de la factory (más determinista que encadenar el alta por HTTP), autenticando luego por HTTP como el agente.
- Aprovechar que el pipeline aloja tal cual los binarios no-imagen para verificar la subida/descarga con bytes arbitrarios.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 20 (endurecimiento E2E del MVP: helper de escenario + 3 pruebas E2E por HTTP). Veredicto Cumplido, velocity 8, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
