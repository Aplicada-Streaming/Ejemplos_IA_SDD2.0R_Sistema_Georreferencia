# Sprint Retrospectiva — Sprint 67

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-67_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- **H-14 entregado y reusó núcleo.** El anuncio por región en vivo reusa el texto de `PresentacionCintaConexion` (S59) y se dispara sólo al cambiar el estado; cambio chico, enfocado y de valor real en terreno (el agente se entera del estado sin mirar la pantalla).
- **Se cortó a tiempo con H-07.** En vez de seguir invirtiendo en código de plataforma frágil que pelea contra el framework, se reconoció la limitación, se **revirtió** y se documentó. Mejor diferir honesto que envalentonar un fix que no se sostiene.
- **Casi toda la auditoría cerrada.** P1 (S59-S62) + P2 (S63) + H-14 (S67) hechos; sólo resta H-07, y no por alcance sino por una restricción de MAUI Shell.

## 2. Qué no salió bien

- **H-07 no se pudo cerrar.** Se intentaron dos vías de plataforma (forzar `LabelVisibilityMode=labeled`; igualar la apariencia activa/inactiva de la `BottomNavigationView`, re-aplicadas en cada layout) y **ninguna** sostuvo el cambio: el *appearance tracker* del Shell de MAUI re-pisa esos ajustes al navegar. La verificación on-device confirmó que la pestaña activa seguía mostrando "Captu…". Una corrección limpia exige un *renderer* de Shell propio o pestañas con ícono → diferido.
- **Se quemaron 4 ciclos de build** peleando con el framework por un detalle cosmético antes de reconocer la limitación y revertir. Lección: poner un tope de intentos a los fixes de plataforma frágiles y, si no ceden, diferir con el hallazgo.
- **Verificación de accesibilidad limitada.** H-14 sólo se confirma por construcción + arranque sin fallos; la verificación real con **TalkBack** queda como prueba manual (no automatizable en el gate ni observable por captura).

## 3. Qué probar

- On-device: las cinco pestañas muestran su etiqueta completa (sin "…"), incluida la seleccionada.
- Con TalkBack activado: al pasar de "al día" a "sin conexión"/"sincronizando", el lector anuncia el nuevo estado.

## 4. Acciones concretas

| Acción | Responsable | Fecha | Estado |
| --- | --- | --- | --- |
| **H-07 con enfoque correcto**: renderer de Shell propio o pestañas con ícono (no customizar la `BottomNavigationView` por fuera) | AG-08 | 2029-02-02 | Diferido |
| Prueba manual de accesibilidad con TalkBack (anuncios H-14 + recorrido por lector) | AG-08 | 2029-02-02 | Pendiente |
| Script de reset/arranque del entorno de desarrollo | AG-09 | 2029-02-02 | Pendiente (reiterado de S64) |
| Definir la próxima épica de alcance pleno | AG-07 | 2029-02-02 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 66 | Estado |
| --- | --- |
| Pasada de accesibilidad/UI dedicada (H-14 + H-07) | **Parcial**: H-14 hecho; H-07 diferido (limitación de MAUI Shell) |
| Script de reset/arranque del entorno de desarrollo | Pendiente (reiterado) |
| Volver al alcance pleno con una épica | Pendiente (única vía: el backlog chico está agotado salvo H-07) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Retro del Sprint 67 (pasada de accesibilidad). H-14 hecho; H-07 diferido (el tab bar de MAUI Shell re-pisa los ajustes de la `BottomNavigationView`; se revirtió el código frágil tras 4 ciclos). Lección: tope de intentos a fixes de plataforma frágiles. Acción: H-07 con renderer propio + definir la próxima épica. Generada por AG-07 |
