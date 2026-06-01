# Wireframe — Gestión de relevamientos y estados (web admin)

**Proyecto:** GeoVial
**Documento:** wireframes-gestion-relevamientos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** UX/UI Designer + Frontend Lead (AG-03), Equipo SDD 2.0
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie web del jefe de área para crear un relevamiento, fijar su radio de agrupación, asignar o reasignar agentes de su área y hacer avanzar el relevamiento por sus estados (recolección → revisión → cierre), con reapertura explícita. Organiza el trabajo del área y delega la recolección en personal no experto.

## 2. Layout

Escritorio (web), lista y alta de relevamiento:

```
+--------------------------------------------------------------+
| GeoVial | Relevamientos — Area "Zona Norte"   [+ Nuevo]      |
|--------------------------------------------------------------|
| Relevamiento        | Estado       | Agentes | Acciones      |
|---------------------|--------------|---------|---------------|
| Puente Rio 12       | revision     |  2      | [Ver][Estado] |
| Camino 8            | recoleccion  |  3      | [Ver][Estado] |
| Puente Sur          | cerrado      |  1      | [Ver][Reabrir]|
+--------------------------------------------------------------+

Alta / edicion de relevamiento (panel o modal):

+--------------------------------------------------+
| Nuevo relevamiento                               |
| Identificacion: [ Puente Rio 12 ............. ]  |
| Obra: ( ) Puente  ( ) Camino                     |
| Radio de agrupacion: [ 15 ] m                    |
|--------------------------------------------------|
| Asignar agentes del area:                        |
|  [x] Agente A   [x] Agente B   [ ] Agente C      |
|--------------------------------------------------|
| [ Crear relevamiento ]   [ Cancelar ]            |
+--------------------------------------------------+

Transicion de estado (confirmacion):

+--------------------------------------------------+
| Estado actual: revision                          |
| Pasar a: [ cerrado v ]                            |
| Al cerrar, el relevamiento queda de solo lectura.|
| [ Confirmar cierre ]   [ Cancelar ]              |
+--------------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Lista de relevamientos del área | Ver y operar los relevamientos del área | Nombre, estado, conteo de agentes, acciones | Solo muestra relevamientos del área del jefe (RN-01) |
| Formulario de alta/edición | Crear el relevamiento y fijar el radio | Identificación, obra (puente/camino), radio en metros | Valida y persiste; crea en estado recolección (RN-05) |
| Selector de agentes | Asignar o reasignar agentes del área | Agentes del área con su estado de asignación | Solo permite agentes del área (RN-01); reasignar conserva lo recolectado |
| Control de estado | Avanzar el relevamiento de estado | Estado actual y transiciones válidas | Ofrece solo transiciones permitidas (RN-05); confirma cierre |
| Acción reabrir | Reapertura explícita de un cerrado | — | Disponible solo en cerrado; requiere acción explícita (RN-05) |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Crear relevamiento | Clic en crear | Relevamiento en estado recolección con su radio; acción auditada | Jefe de área autenticado (RN-01) |
| Asignar agentes | Selección de agentes y guardar | Asignaciones registradas | Agentes del área del jefe (RN-01) |
| Reasignar agentes | Cambio de selección sobre un relevamiento existente | Asignaciones actualizadas; observaciones previas conservadas | Relevamiento en recolección o revisión |
| Avanzar de estado | Selección de transición y confirmación | Nuevo estado aplicado; cierre deja solo lectura; auditado | Transición válida (RN-05) |
| Reabrir relevamiento | Clic en reabrir y confirmación | Pasa a recolección, levanta solo lectura, auditado | Relevamiento cerrado; acción explícita (RN-05) |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | El área no tiene relevamientos | Mensaje orientativo y acción "Nuevo relevamiento" |
| Cargando | Carga de la lista o guardado | Skeleton de la lista o carga en el botón |
| Con datos | Relevamientos del área presentes | Lista con estado y conteo de agentes |
| Error | `ACCESO_NO_AUTORIZADO`, `AGENTE_FUERA_DE_AREA`, `RELEVAMIENTO_SOLO_LECTURA`, `TRANSICION_INVALIDA`, `REAPERTURA_NO_AUTORIZADA` | Aviso inline con causa y acción siguiente (ver experiencia-de-uso §8) |
| Solo lectura | Relevamiento cerrado (RN-05) | Asignación y edición inhabilitadas; disponible reabrir y exportar |
| Éxito | Creación, asignación o transición aplicada | Confirmación; la lista refleja el nuevo estado/conteo |

## 6. Versión móvil o responsive

- Superficie de escritorio (tarea administrativa del jefe de área). En anchos reducidos la tabla de relevamientos pasa a tarjetas apiladas con estado y acciones; el alta y la transición se presentan como pantallas a página completa en lugar de modal.

## 7. Notas de implementación

- Accesibilidad: formularios con etiquetas semánticas y nombres accesibles; el control de estado es operable por teclado; la confirmación de cierre describe su consecuencia (prevención de errores) y se anuncia por región en vivo; el estado no se comunica solo por color (WCAG 2.2 AA).
- Performance percibida: skeleton de la lista; guardado con confirmación inmediata; las acciones jerárquicas no se presentan como instantáneas sin confirmación.
- Internacionalización: radio en metros; estados y obra en español.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU origen | CU-01 (crear relevamiento y asignar/reasignar agentes), CU-10 (transición de estados y reapertura) |
| Reglas de negocio | RN-01, RN-02 (radio), RN-05 (estados/solo lectura), RN-07 (auditoría) |
| Marco experiencia-de-uso | §3.5 (gestión), §4 (estados), §8 (errores) |
| US a generar | a generar en 06 (alta de relevamiento, asignación/reasignación, transición de estados, reapertura explícita) |
| Tests previstos | a generar en 08 (creación y asignación, transición de estados, reapertura, accesibilidad de formularios) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Wireframe inicial de gestión de relevamientos y estados, generado por AG-03 a partir de CU-01 y CU-10 |
