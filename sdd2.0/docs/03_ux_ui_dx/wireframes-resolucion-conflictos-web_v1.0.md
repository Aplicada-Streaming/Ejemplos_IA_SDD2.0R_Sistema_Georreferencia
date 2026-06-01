# Wireframe — Resolución de conflictos (web admin)

**Proyecto:** GeoVial
**Documento:** wireframes-resolucion-conflictos-web_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** UX/UI Designer + Frontend Lead (AG-03), Equipo SDD 2.0
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie web del jefe de área para revisar la lista de conflictos de un relevamiento y resolverlos por decisión humana: unificar o mantener separados marcadores dentro de un mismo radio, y dirimir ediciones en conflicto consolidadas por última escritura. El sistema nunca unifica ni descarta de forma automática (RN-02); aquí concentra la complejidad de la consolidación, lejos de la captura móvil.

## 2. Layout

Escritorio (web):

```
+--------------------------------------------------------------+
| Conflictos — "Puente Rio 12"                Pendientes: 3    |
|--------------------------------------------------------------|
| Marcadores en un mismo radio (2)                             |
| --------------------------------------------------------     |
| [ M-04 ]  +--mini mapa--+   [ M-07 ]                         |
| 3 obs       (o)   (o)         2 obs                          |
| Distancia: 9 m  (radio 15 m)                                 |
| ( ) Unificar en uno   ( ) Mantener separados                |
| [ Aplicar decision ]                                         |
| --------------------------------------------------------     |
| Ediciones en conflicto (1)                                   |
| --------------------------------------------------------     |
| Comentario de M-02                                           |
| Version vigente (mas reciente):  "Fisura ... ampliada"      |
| Version alternativa registrada:  "Fisura ..."              |
| ( ) Mantener vigente   ( ) Usar alternativa                 |
| [ Aplicar decision ]                                         |
+--------------------------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Encabezado de conflictos | Identificar el relevamiento y los pendientes | Nombre del relevamiento y conteo de conflictos | Conteo se actualiza al resolver |
| Sección marcadores en un mismo radio | Resolver cercanía dentro del radio (RN-02) | Marcadores, conteo de observaciones, distancia y radio, mini mapa | Permite unificar o mantener separados; nunca decide solo |
| Sección ediciones en conflicto | Dirimir choques por última escritura (RN-04) | Versión vigente y alternativa registrada | Permite mantener la vigente o usar la alternativa |
| Control de decisión | Capturar la elección del jefe | Opciones mutuamente excluyentes | Aplicar habilitado solo al elegir una opción |
| Ajuste de radio (acceso) | Reevaluar la cercanía con otro radio | Radio actual | Cambiar el radio reevalúa la lista (CU-11, 5.A) |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Unificar marcadores | Elegir unificar y aplicar | Observaciones fusionadas en un marcador; conflicto resuelto; auditado | Relevamiento no cerrado (RN-05); decisión humana (RN-02) |
| Mantener separados | Elegir mantener y aplicar | Ambos marcadores se conservan; marca de conflicto levantada | Relevamiento no cerrado |
| Dirimir edición | Elegir versión y aplicar | La versión elegida queda vigente; conflicto resuelto; auditado | Relevamiento no cerrado (RN-04) |
| Ajustar radio | Cambiar el radio | La lista de marcadores en conflicto se reevalúa | Radio admisible (RN-02) |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Sin conflictos pendientes | Ícono neutro y mensaje "No hay conflictos pendientes" |
| Cargando | Detección o aplicación en curso | Skeleton de la lista o carga en el botón aplicar |
| Con datos | Conflictos listados | Lista separada por tipo (mismo radio / ediciones), con su conteo |
| Error | `RELEVAMIENTO_SOLO_LECTURA`, `CONFLICTO_INEXISTENTE`, `ACCESO_NO_AUTORIZADO`, `RADIO_INVALIDO`, `UNIFICACION_NO_AUTORIZADA` | Banner inline con causa y acción siguiente (ver experiencia-de-uso §8) |
| Solo lectura | Relevamiento cerrado (RN-05) | Aplicar inhabilitado; aviso de reabrir para resolver |
| Éxito | Conflicto resuelto | El ítem se marca resuelto y baja de la lista; conteo decrece |

## 6. Versión móvil o responsive

- Superficie de escritorio (decisión analítica del jefe de área). En anchos reducidos cada conflicto se presenta como una tarjeta a página completa: primero el contexto (mini mapa o versiones), luego las opciones y la acción de aplicar, sin perder la separación por tipo de conflicto.

## 7. Notas de implementación

- Accesibilidad: las opciones de decisión son controles agrupados con nombre accesible y operables por teclado; la distinción entre tipos de conflicto se da con encabezado y texto, no solo por color; el resultado de la resolución se anuncia por región en vivo; foco visible (WCAG 2.2 AA).
- Performance percibida: skeleton durante la detección; aplicar muestra carga y confirma; la lista se actualiza sin recargar toda la pantalla.
- Internacionalización: distancias y radio en metros; versiones de texto en español, mostradas completas para comparar sin truncar.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU origen | CU-11 (detectar y listar conflictos), CU-12 (resolver conflictos desde la web) |
| Reglas de negocio | RN-01, RN-02 (radio), RN-04 (última escritura), RN-05 (solo lectura), RN-07 (auditoría) |
| Marco experiencia-de-uso | §3.6 (resolución de conflictos), §4.5 (estados de conflictos), §8 (errores) |
| US a generar | a generar en 06 (lista de conflictos, unificar/separar marcadores, dirimir ediciones, ajuste de radio) |
| Tests previstos | a generar en 08 (detección por radio, resolución de conflictos, solo lectura tras cierre, accesibilidad) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Wireframe inicial de resolución de conflictos web, generado por AG-03 a partir de CU-11 y CU-12 |
