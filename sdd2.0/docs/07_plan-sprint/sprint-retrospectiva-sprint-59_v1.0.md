# Sprint Retrospectiva — Sprint 59

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-59_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-06
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **La auditoría UX alimentó el backlog.** El primer hallazgo P1 que se atacó (H-05) salió derecho del informe `evaluacion-ux-mobile_v1.0.md`; el ciclo "auditar → priorizar → implementar" funcionó.
- **Reuso del núcleo de S48.** `MonitorSincronizacion` + `ResumenSincronizacion.Calcular` (ya en el gate) se aplicaron a 4 pantallas sin tocar lógica de sync; el sprint fue una vista reusable + un pequeño núcleo de presentación.
- **Accesibilidad desde el diseño.** Ícono distinto por estado (no sólo color, WCAG 1.4.1) + descripción semántica, cubierto por test (`Cada_estado_tiene_icono_distinto`).
- **Componente reusable.** `CintaConexionView` encapsula la suscripción y el pintado; cada página la usa con una línea (`Cinta.Vincular(monitor)`), lo que mantuvo los cambios por página mínimos.

## 2. Qué no salió bien

- **Cuatro páginas heterogéneas.** Dos son XAML (Captura, Revisión) y dos code-only (Mapa, Bandeja); hubo que reestructurar el layout de cada una (Grid Auto/*). Un layout base compartido (una `ContentPage` con cinta + slot) evitaría repetir el andamiaje en el futuro.
- **Verificación on-device costosa.** El relogueo (biométrico → cancelar → bloqueado → clave) y el manejo de capturas por adb (rutas `/sdcard` mangleadas por git-bash, `MSYS_NO_PATHCONV`, `screencap` 0 bytes en ventanas seguras) consumieron tiempo. Conviene un script de captura/login reutilizable.
- **Persistencia "casi" global.** La cinta está en las 4 solapas de trabajo, pero no es un overlay único de la app; si se agregan solapas habrá que vincularla otra vez. Un `Shell`/layout base lo haría una sola vez.

## 3. Qué probar

- On-device: en cada solapa, cortar señal → la cinta vira a rojo "⚠ Sin conexión" y vuelve a verde al reconectar; con capturas en cola, muestra "● N por sincronizar" (ámbar).
- Accesibilidad: el lector de pantalla anuncia "Estado de sincronización: …".

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| Verificar on-device la cinta en las 4 solapas (online/pendiente/offline) | AG-05 (QA) | 2028-09-29 | En curso (con el usuario) |
| Evaluar un layout base / Shell con la cinta única para toda la app | AG-08 | 2028-09-29 | Pendiente |
| Próximo P1 de la auditoría: H-02 (permiso de ubicación + Centrar-GPS) | Equipo | 2028-09-29 | Planificado |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 58 | Estado |
| --- | --- |
| Verificar on-device "quitar foto" | En curso (con el usuario) |
| Script único parar→test→relanzar→seed del backend | Pendiente (se reitera; ver §2) |
| Confirmar con el PO las decisiones de cascada de borrado | Pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-06 | Retro del Sprint 59 (H-05: cinta de conexión persistente). Bien: reuso de S48, accesibilidad por diseño, componente reusable. A mejorar: layout base compartido y un script de login/captura on-device. Próximo P1: H-02. Generada por AG-07 |
