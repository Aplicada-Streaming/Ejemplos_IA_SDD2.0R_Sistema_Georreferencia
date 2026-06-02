# Sprint Review — Sprint 12

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-12_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita literal del sprint goal de `plan-iteracion-sprint-12_v1.0.md`:

> Permitir que el agente recolecte observaciones sin conexión guardándolas localmente en la cola para sincronizar (US-16) y que la app, al recuperar señal, dispare la sincronización automáticamente avisando si había cambios pendientes (US-19), todo sobre la librería `GeoVial.Sync` y la cola SQLite del Sprint 11.

Veredicto: Cumplido.

Explicación corta: el `ColectorOffline` toma una observación capturada y la encola como cambio en la cola SQLite local, conservándola sin pérdida (100 capturas verificadas); sin espacio, la cola traduce el disco lleno a `AlmacenamientoLocalInsuficienteException` y conserva lo guardado (CA-03). El `CoordinadorAutoSync` se suscribe al monitor de conectividad y, al recuperar señal con un relevamiento activo, dispara la sincronización por el motor: si había cambios pendientes lo notifica, y si no, solo baja actualizaciones (CA-02). El formato del payload se centralizó en un factory compartido con el cliente REST, garantizando que lo encolado coincida con lo que consume `/sync`.

## 2. Demos realizadas

| ID | Tipo | Descripción de la demo | Feedback del Product Owner |
| --- | --- | --- | --- |
| US-16 | Historia | Capturar 100 observaciones sin señal: se encolan en SQLite y se conservan sin pérdida | Continuidad offline robusta |
| US-16 | Historia | Sin espacio local, el guardado responde `ALMACENAMIENTO_LOCAL_INSUFICIENTE` y conserva lo guardado | Manejo seguro del disco lleno |
| US-19 | Historia | Recuperar señal con cambios pendientes: dispara la sync automática y notifica que había pendientes | Sin sincronización manual |
| US-19 | Historia | Recuperar señal sin pendientes: solo baja actualizaciones, sin subir nada | Comportamiento correcto |

## 3. Feedback recibido

- El ciclo capturar sin señal → recuperar conexión → sincronizar automáticamente queda demostrable sobre la librería y la cola, cerrando la lógica de continuidad operativa (NB-03) en el cliente.
- Lo que resta para la app de campo completa es la UI de captura (foto/GPS/comentario) y la pantalla de resolución de conflictos, sobre esta base ya construida.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 13 |
| Puntos completados | 13 |
| Velocity efectiva | 13 |
| Ratio de completitud | 100 % |
| Defectos detectados durante el sprint | 0 |

Pruebas: 250 verdes (221 unitarias + 29 de integración), +8 respecto del Sprint 11. Cobertura: dominio 89,6 % líneas / 79,8 % branches; aplicación 89,9 % / 81,8 %; `GeoVial.Sync` 94,7 % / 94,0 %; `GeoVial.FileHosting` 100 % / 100 % (gate ≥ 80 % / ≥ 70 % cumplido). Build Release sin warnings tratados como error.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| US-16 | Historia | Aceptada (encolado offline + manejo de disco lleno; la UI de captura es de la app) |
| US-19 | Historia | Aceptada (coordinador de sync automática; el sensor de conectividad de plataforma vive en la cáscara MAUI) |

## 6. Carry-over al siguiente sprint

| ID | Tipo | Puntos | Motivo del traslado |
| --- | --- | --- | --- |
| — | — | 0 | Ningún ítem comprometido en el Sprint 12 se traslada. |

La implementación de plataforma `MonitorConectividadMaui` (sensor de red) y el cableado del colector y el coordinador se incorporaron a la cáscara `GeoVial.Mobile`, que sigue fuera de la solución/CI por el empaquetado del APK (Android SDK provisionado). La UI de captura de campo (foto/GPS), la pantalla de resolución de conflictos y la demo autónoma de sincronización (US-32) quedan en el backlog.

## 7. Decisiones tomadas durante el review

- Dar por cubierta la lógica de continuidad operativa del cliente (captura offline + sync automática) en `GeoVial.Sync`.
- Planificar la UI de captura de campo y la app demostrable sobre un agente con el Android SDK provisionado.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Sprint review del Sprint 12 (captura offline y sincronización automática por conectividad, US-16/US-19). Veredicto Cumplido, velocity 13, 0 carry-over. Generado por AG-07 a partir de `template-sprint-review_v1.0.md` |
