# Sprint Retrospectiva — Sprint 17

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-17_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-02
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- La navegación del carrusel quedó como lógica pura (`NavegadorRevision`) sobre los DTOs compartidos: 100 % de branches con pruebas simples, sin tocar la cáscara MAUI.
- El cliente de revisión reusa los contratos de `GeoVial.Shared` y el endpoint ya entregado en el Sprint 04; el sprint fue solo frente cliente, sin reabrir backend.
- El patrón núcleo testeable (`GeoVial.Revision`, en CI) + pantalla MAUI (fuera de CI) se aplicó por cuarta vez (S14–S17) con fricción mínima: ya es la forma estándar de sumar features móviles.
- El índice circular del carrusel (módulo con corrección de negativos) cubrió ambos sentidos y los bordes con una sola expresión.

## 2. Qué no salió bien

- La pantalla recorre los marcadores por coordenada y lista, no sobre un mapa interactivo: el control de mapas de MAUI sigue requiriendo la clave de proveedor no provisionada (deuda que se arrastra desde US-13).
- La descarga de la foto del marcador en foco no cachea: al volver a un marcador ya visitado se vuelve a descargar el binario.
- La edición sobre el marcador desde el móvil (agregar comentario/etiqueta, US-15 cliente) no entró: la revisión es de solo lectura en el cliente por ahora.

## 3. Qué probar

- Provisionar la clave del proveedor de mapas y situar los marcadores sobre un mapa interactivo, unificando la deuda de US-13 y US-21.
- Cachear en memoria las fotos ya descargadas del carrusel para no re-descargarlas al navegar hacia atrás.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Provisionar clave de mapas y mapa interactivo (unifica US-13 y US-21 sobre mapa) | AG-08 (móvil) | 2027-02-20 | Pendiente |
| Cachear las fotos descargadas del carrusel de revisión | AG-08 (móvil) | 2027-02-20 | Pendiente |
| Edición sobre el marcador desde el móvil (comentario/etiqueta, US-15 cliente) | AG-08 (móvil) | 2027-02-20 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 16 | Estado actual |
| --- | --- |
| Taggear `v1.0.0` y publicar el primer stable de `GeoVial.Sync` | Pendiente (release por tag con aprobación) |
| Firmar el paquete (supply-chain) antes de declararlo consumible en stable | Pendiente |
| Stage/test que verifique el contenido del `.nupkg` | Pendiente |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-02 | Retrospectiva del Sprint 17 con 3 acciones nuevas y seguimiento de las del Sprint 16 (release stable de la librería pendiente de tag/aprobación). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
