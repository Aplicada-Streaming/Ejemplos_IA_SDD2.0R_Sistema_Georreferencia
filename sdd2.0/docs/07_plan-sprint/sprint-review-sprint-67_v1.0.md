# Sprint Review — Sprint 67

**Proyecto:** GeoVial
**Documento:** sprint-review-sprint-67_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-08
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Objetivo del sprint y resultado

Cita del sprint goal de `plan-iteracion-sprint-67_v1.0.md`:

> Cerrar los dos últimos hallazgos de la auditoría UX móvil, ambos de accesibilidad/UI: H-07 (truncado de etiquetas de pestañas) y H-14 (anuncios por región en vivo).

Veredicto: **Parcial** — H-14 cumplido; H-07 investigado y **diferido** (limitación de MAUI Shell).

Explicación corta: **H-14** — el estado de sincronización ahora se **anuncia** por el lector de pantalla al **cambiar** (región en vivo, WCAG 4.1.3), no sólo se muestra: el agente en terreno se entera de "sin conexión"/"sincronizando"/"pendiente" sin mirar la pantalla. **H-07** — se intentó corregir el truncado de la pestaña seleccionada ("Captu…") por customización de plataforma (forzar `LabelVisibilityMode=labeled` e igualar la apariencia activa/inactiva de la `BottomNavigationView`), pero el **appearance tracker del Shell de MAUI re-pisa** esos ajustes al navegar; una corrección limpia exige un *renderer* de Shell propio (o pestañas con ícono), desproporcionado para un detalle de etiqueta. Se **revirtió** el código de plataforma frágil y se **difiere** H-07 con el hallazgo documentado. Honesto: el backlog de la auditoría UX queda con **P1 + P2 + H-14 hechos** y **H-07 diferido** (limitación de framework, no de una línea). Sin núcleo nuevo ni backend.

## 2. Demos realizadas

| ID | Tipo | Descripción | Feedback |
| --- | --- | --- | --- |
| H-14 | Accesibilidad | Al cambiar el estado de conexión, el lector de pantalla lo anuncia (región en vivo) | WCAG 2.2 §4.1.3 |
| H-07 | Investigación | Truncado de la pestaña activa: el tab bar de MAUI Shell re-pisa los ajustes de etiqueta de la `BottomNavigationView` → diferido (necesita renderer propio) | Limitación documentada |

## 3. Feedback recibido

- El truncado de pestañas era un detalle visible que daba sensación de inacabado; queda prolijo.
- Anunciar el estado de conexión es clave en terreno (el agente suele no estar mirando la pantalla).
- Cierra el último tramo de la auditoría UX: el informe queda 100 % ejecutado.

## 4. Métricas del sprint

| Métrica | Valor |
| --- | --- |
| Puntos comprometidos | 5 (H-14 + H-07) |
| Puntos completados | 3 (H-14 + investigación de H-07) |
| Velocity efectiva | 3 |
| Ratio de completitud | 60 % |
| Defectos detectados | 0 |

Pruebas: **498** (454 unitarias + 44 de integración), **sin nuevas**: es accesibilidad de UI sin núcleo nuevo. El gate no se ve afectado (los archivos tocados son de `GeoVial.Mobile`, fuera del gate Domain/Application). El MAUI compila (`net10.0-android`, arm64) y se redeployó. **H-14 verificado por construcción** (compila y llama a `SemanticScreenReader.Announce` al cambiar el estado) y por arranque sin fallos —la verificación con lector de pantalla (TalkBack) queda como prueba manual de accesibilidad—. **H-07 verificado on-device como NO resuelto** por la vía de plataforma (la captura seguía mostrando "Captu…" pese a forzar el modo de etiquetas) → se revirtió y se difirió.

## 5. Items completados vs comprometidos

| ID | Tipo | Estado final |
| --- | --- | --- |
| H-14 | Historia | Aceptada (anuncios por región en vivo) |
| H-07 | Historia | **Diferida** (limitación de MAUI Shell; necesita renderer propio o pestañas con ícono) |

## 6. Carry-over

| ID | Puntos | Motivo |
| --- | --- | --- |
| H-07 | 2 | Reclasificado: no es un fix de plataforma chico; requiere un renderer de Shell propio. Se difiere como ítem aparte (no se re-compromete tal cual) |

**Backlog de la auditoría UX:** P1 (S59-S62) + P2 (S63) + **H-14** (S67) hechos; **H-07** diferido como limitación de framework. Es el único hallazgo de la auditoría que queda, y no por alcance sino por una restricción de MAUI Shell.

## 7. Decisiones tomadas

- H-07: tras intentar forzar `LabelVisibilityMode=labeled` + igualar apariencias activa/inactiva (re-aplicado en cada layout), el tab bar de MAUI Shell siguió re-pisando los ajustes. Decisión: **revertir** la customización de plataforma (frágil y con costo de perf por el listener de layout) y **diferir** H-07; no insistir con código que pelea contra el framework por un detalle cosmético.
- H-14 anuncia **sólo al cambiar** el estado (no en cada render) para no ser ruidoso; se reusa el texto de `PresentacionCintaConexion`.
- La verificación con lector de pantalla (TalkBack) se documenta como prueba manual de accesibilidad (no automatizable en el gate).

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-08 | Review del Sprint 67 (pasada de accesibilidad). **Parcial**: H-14 (anuncios por región en vivo) cumplido; H-07 (truncado de pestañas) investigado y **diferido** (el tab bar de MAUI Shell re-pisa los ajustes de la `BottomNavigationView`; necesita renderer propio). Velocity 3, 498 pruebas (sin nuevas; accesibilidad de UI). Generado por AG-07 |
