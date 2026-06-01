# Casos de prueba referenciales — GeoVial

**Proyecto:** GeoVial
**Documento:** casos-prueba-referenciales_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Ingeniero QA / SDET Senior (AG-08), Equipo SDD 2.0

## 1. Propósito y convenciones

Catálogo de casos de prueba referenciales (TC-XX) con trazabilidad explícita a CU, RN o NFR. Cada TC declara tipo (unit, integration, e2e, snapshot, contract), referencia upstream, setup, pasos Given-When-Then, expected output, actual output y status. El catálogo cubre los criterios de aceptación Given-When-Then de los catorce CU de 02 y las ocho RN, e incluye TC dedicados a idempotencia de la cola de sync, last-write-wins, georreferenciación EXIF→manual, autorización por rol y área, y retención de auditoría.

El campo "Actual output" se declara `pendiente` en todos los TC porque ninguno se ejecutó aún (los tests se implementan en los sprints de 07). El campo "Status" es `pendiente` por la misma razón. La matriz de cobertura (matriz-cobertura-pruebas) cruza estos TC con CU/RN/NFR y se actualizará a Verde/Rojo al cierre de cada sprint.

Numeración: TC-01 a TC-26, contigua, dos dígitos.

## 2. Catálogo de casos de prueba

### TC-01 crear-relevamiento-asignar-agentes-area

- Tipo: integration
- Cubre: CU-01 (CA-01); RN-01, RN-02, RN-05, RN-07
- Setup: jefe del área "Zona Norte" autenticado; dos agentes de campo dados de alta en "Zona Norte"; base efímera (Testcontainers) sembrada con el dataset sintético.
- Pasos:
  - Given un jefe del área "Zona Norte" con dos agentes de su área
  - When crea el relevamiento "Puente Río 12" con radio de agrupación 15 m y asigna ambos agentes
  - Then el sistema crea el relevamiento en estado recolección con radio 15 m y registra dos asignaciones y la acción en auditoría
- Expected output: relevamiento persistido en estado recolección, radio = 15 m, dos AsignaciónAgente; registro de auditoría con autor, momento y operación.
- Actual output: pendiente
- Status: pendiente

### TC-02 rechazar-asignacion-agente-fuera-de-area

- Tipo: unit
- Cubre: CU-01 (CA-02); RN-01
- Setup: jefe del área "Zona Norte"; un agente del área "Zona Sur"; puerto de repositorio mockeado.
- Pasos:
  - Given un jefe del área "Zona Norte"
  - When intenta asignar un agente del área "Zona Sur"
  - Then el sistema rechaza con `AGENTE_FUERA_DE_AREA` y no registra la asignación
- Expected output: excepción/resultado con código `AGENTE_FUERA_DE_AREA`; ninguna asignación creada.
- Actual output: pendiente
- Status: pendiente

### TC-03 rechazar-asignacion-relevamiento-cerrado

- Tipo: unit
- Cubre: CU-01 (CA-03); RN-05
- Setup: relevamiento "Camino 8" en estado cerrado.
- Pasos:
  - Given un relevamiento "Camino 8" cerrado
  - When el jefe intenta asignarle un agente
  - Then el sistema rechaza con `RELEVAMIENTO_SOLO_LECTURA`
- Expected output: código `RELEVAMIENTO_SOLO_LECTURA`; sin cambios.
- Actual output: pendiente
- Status: pendiente

### TC-04 iniciar-sesion-listar-relevamientos-habilitar-offline

- Tipo: integration
- Cubre: CU-02 (CA-01); RN-01, RN-06
- Setup: agente con dos relevamientos asignados y método de seguridad del teléfono configurado.
- Pasos:
  - Given un agente con dos relevamientos asignados y método de seguridad configurado
  - When inicia sesión con conexión
  - Then el sistema lista los dos relevamientos asignados y habilita el modo sin conexión
- Expected output: respuesta con los dos relevamientos asignados; flag de modo offline habilitado.
- Actual output: pendiente
- Status: pendiente

### TC-05 bloquear-offline-sin-metodo-seguridad

- Tipo: unit
- Cubre: CU-02 (CA-02, CA-03); RN-06
- Setup: agente sin método de seguridad configurado.
- Pasos:
  - Given un agente sin método de seguridad configurado en su primer inicio de sesión con conexión
  - When intenta habilitar la operación sin conexión, y luego intenta reingresar en terreno sin señal
  - Then el sistema responde `OFFLINE_NO_HABILITADO` al habilitar y `REINGRESO_SIN_METODO_SEGURIDAD` al reingresar, sin abrir sesión offline
- Expected output: códigos `OFFLINE_NO_HABILITADO` y `REINGRESO_SIN_METODO_SEGURIDAD`; modo offline no habilitado.
- Actual output: pendiente
- Status: pendiente

### TC-06 administrar-alta-baja-jerarquica-usuarios

- Tipo: integration
- Cubre: CU-03 (CA-01, CA-02, CA-03); RN-01, RN-07, RN-08
- Setup: jefe general autenticado; área "Zona Norte" existente; un jefe de área autenticado para el caso negativo.
- Pasos:
  - Given un jefe general autenticado
  - When da de alta un jefe de área asociado a "Zona Norte"; luego un jefe de área intenta dar de alta a otro jefe de área; luego el jefe general intenta dar de alta un agente en un área inexistente "Zona X"
  - Then el sistema crea el jefe de área vigente y lo audita; rechaza el segundo con `ACCESO_NO_AUTORIZADO`; rechaza el tercero con `AREA_INEXISTENTE`
- Expected output: alta válida persistida y auditada; códigos `ACCESO_NO_AUTORIZADO` y `AREA_INEXISTENTE` en los casos negativos.
- Actual output: pendiente
- Status: pendiente

### TC-07 capturar-observacion-georreferenciada-crear-asociar-marcador

- Tipo: integration
- Cubre: CU-04 (CA-01, CA-02); RN-02, RN-03, RN-05
- Setup: relevamiento en recolección con radio 15 m; en CA-02, un marcador existente y una foto a 8 m de él.
- Pasos:
  - Given un relevamiento en recolección con radio 15 m sin marcadores
  - When el agente toma una foto con metadatos de ubicación en un punto nuevo y, en otro escenario, una foto a 8 m de un marcador existente
  - Then en el primer caso crea un marcador en esa coordenada y asocia la observación; en el segundo asocia al marcador existente sin crear uno nuevo
- Expected output: marcador creado en coordenada nueva; observación asociada al marcador dentro del radio sin duplicar marcador.
- Actual output: pendiente
- Status: pendiente

### TC-08 georreferenciacion-exif-a-manual-a-bandeja

- Tipo: unit
- Cubre: CU-04 (CA-03), CU-05 (CA-01, CA-02, CA-03); RN-03
- Setup: foto sin metadatos de ubicación; foto con metadatos; relevamiento con radio 15 m y un marcador existente.
- Pasos:
  - Given una foto sin metadatos de ubicación
  - When se intenta registrar la observación, luego se coloca el punto a 5 m de un marcador existente, luego en otro escenario no se ubica el punto, y en otro se intenta ubicar manualmente una foto que sí trae metadatos
  - Then sin metadatos responde `OBSERVACION_SIN_GEORREFERENCIA` y deriva a ubicación manual; con ubicación manual a 5 m asocia al marcador con la coordenada manual; sin ubicar deriva a la bandeja sin georreferenciar; con metadatos presentes responde `FUENTE_UBICACION_INCORRECTA` y usa los metadatos (prioridad EXIF)
- Expected output: cadena de prioridad EXIF → manual → bandeja sin georreferenciar respetada; códigos `OBSERVACION_SIN_GEORREFERENCIA` y `FUENTE_UBICACION_INCORRECTA` en sus casos.
- Actual output: pendiente
- Status: pendiente

### TC-09 recolectar-sin-conexion-encolar-jornada-completa

- Tipo: e2e
- Cubre: CU-06 (CA-01, CA-02, CA-03); NFR operación sin conexión; RN-06
- Setup: dispositivo Android por USB; agente con modo offline habilitado y un relevamiento abierto; red deshabilitada.
- Pasos:
  - Given un agente con modo sin conexión habilitado y un relevamiento abierto, sin señal
  - When captura observaciones con foto, comentario y etiqueta y continúa una jornada laboral completa (≈8 h, ≈100 observaciones); además intenta reingresar sin método de seguridad válido
  - Then el sistema guarda cada observación localmente y la encola; conserva las 100 observaciones sin pérdida; y responde `REINGRESO_SIN_METODO_SEGURIDAD` al reingreso inválido sin habilitar la recolección
- Expected output: 100 RegistroCambioSync en la cola local SQLite, sin pérdida tras 8 h; reingreso inválido bloqueado.
- Actual output: pendiente
- Status: pendiente

### TC-10 sincronizar-subir-bajar-vaciar-cola

- Tipo: integration
- Cubre: CU-07 (CA-01); NFR tiempo de sincronización; RN-01, RN-02
- Setup: cola local con 100 cambios; backend efímero con relevamientos asignados; conexión recuperada.
- Pasos:
  - Given una cola con 100 cambios locales y conexión recuperada
  - When el sistema sincroniza
  - Then sube los 100 cambios, baja las actualizaciones de los relevamientos asignados y vacía la cola sin pérdida, en ≤ 5 minutos
- Expected output: estado central refleja los 100 cambios; cola vacía; tiempo total medido ≤ 5 min (NFR sincronización).
- Actual output: pendiente
- Status: pendiente

### TC-11 sincronizar-idempotencia-cola-reintento

- Tipo: integration
- Cubre: CU-07; RC-03 (idempotencia); RN-04
- Setup: cola con un cambio de identificador único conocido; backend efímero; se fuerza un reintento de subida del mismo identificador.
- Pasos:
  - Given un cambio encolado con identificador único ya subido y confirmado
  - When se reintenta subir el mismo identificador (replay)
  - Then el sistema no aplica el cambio dos veces y el estado central queda idéntico al de la primera aplicación
- Expected output: una sola aplicación del cambio; sin duplicación; recurso sin alteración adicional ante el replay.
- Actual output: pendiente
- Status: pendiente

### TC-12 consolidar-last-write-wins-marca-conflicto

- Tipo: unit
- Cubre: CU-07 (CA-02); RN-04
- Setup: dos ediciones del mismo comentario, una local con marca temporal más reciente que la central; reloj inyectado para fijar el orden.
- Pasos:
  - Given dos ediciones del mismo comentario, una local más reciente que la central
  - When el sistema consolida
  - Then prevalece la edición de marca temporal más reciente y el recurso queda marcado como conflicto (no hay descarte silencioso)
- Expected output: valor consolidado = edición más reciente; flag de conflicto persistente = verdadero.
- Actual output: pendiente
- Status: pendiente

### TC-13 sincronizar-reanudar-sin-duplicar-tras-corte

- Tipo: integration
- Cubre: CU-07 (CA-03); RC-03; RN-04
- Setup: subida en curso interrumpida por corte de señal a la mitad de la cola.
- Pasos:
  - Given una subida interrumpida por corte de señal a la mitad
  - When el sistema reanuda al recuperar conexión
  - Then completa la subida sin duplicar los cambios ya confirmados (`SINCRONIZACION_INTERRUMPIDA` manejado; cola conserva lo no confirmado)
- Expected output: todos los cambios aplicados exactamente una vez; cola final vacía; ninguna duplicación.
- Actual output: pendiente
- Status: pendiente

### TC-14 revisar-relevamiento-sobre-mapa

- Tipo: integration
- Cubre: CU-08 (CA-01, CA-03); RN-01, RN-08
- Setup: relevamiento "Puente Río 12" en revisión con cinco marcadores de "Zona Norte" y una bandeja sin georreferenciar; jefe de "Zona Norte" y jefe de "Zona Sur".
- Pasos:
  - Given un relevamiento en revisión con cinco marcadores de "Zona Norte"
  - When el jefe de "Zona Norte" lo abre sobre el mapa, y luego un jefe de "Zona Sur" intenta abrirlo
  - Then el sistema muestra los cinco marcadores y la bandeja sin georreferenciar al jefe autorizado, y responde `ACCESO_NO_AUTORIZADO` al de otra área
- Expected output: vista con cinco marcadores y bandeja para el autorizado; código `ACCESO_NO_AUTORIZADO` para el no autorizado.
- Actual output: pendiente
- Status: pendiente

### TC-15 exportar-importar-relevamiento-completo

- Tipo: integration
- Cubre: CU-08 (CA-02); RN-05, RN-07
- Setup: relevamiento de "Zona Norte" con datos, comentarios, etiquetas y fotos; backend de archivos local.
- Pasos:
  - Given un relevamiento de "Zona Norte"
  - When el jefe solicita exportarlo y luego reimportarlo
  - Then el sistema entrega un único archivo comprimido con datos, comentarios, etiquetas y fotos, registra la exportación en auditoría, y al reimportar reconstruye el relevamiento equivalente
- Expected output: un único archivo ZIP completo; auditoría de la exportación; relevamiento reconstruido equivalente tras importar.
- Actual output: pendiente
- Status: pendiente

### TC-16 gestionar-marcador-carrusel-fotos-comentarios-etiquetas

- Tipo: component
- Cubre: CU-09 (CA-01, CA-02, CA-03); RN-05
- Setup: componente Blazor de marcador (bUnit); marcador con tres/cinco fotos; relevamiento en recolección y, en otro caso, cerrado.
- Pasos:
  - Given un marcador con tres fotos en un relevamiento en recolección
  - When el agente agrega una cuarta foto con comentario y etiqueta, recorre el carrusel y navega al marcador siguiente, y en un relevamiento cerrado intenta quitar una foto
  - Then persiste foto/comentario/etiqueta y el carrusel muestra cuatro fotos; avanza por el carrusel y carga el marcador siguiente; en el cerrado responde `RELEVAMIENTO_SOLO_LECTURA` y conserva las fotos
- Expected output: carrusel renderiza cuatro fotos; navegación al siguiente marcador; código `RELEVAMIENTO_SOLO_LECTURA` sobre el cerrado.
- Actual output: pendiente
- Status: pendiente

### TC-17 transicionar-estados-relevamiento

- Tipo: unit
- Cubre: CU-10 (CA-01, CA-02, CA-03); RN-05
- Setup: relevamiento en recolección, otro en recolección para la transición inválida, otro cerrado para la reapertura.
- Pasos:
  - Given un relevamiento en recolección de "Zona Norte"
  - When el jefe lo pasa a revisión; luego intenta pasar de recolección directo a cierre; luego ejecuta la reapertura explícita sobre uno cerrado
  - Then deja el relevamiento en revisión y audita la transición; responde `TRANSICION_INVALIDA` conservando recolección; reabre a recolección levantando el solo lectura y audita la reapertura
- Expected output: transiciones válidas aplicadas y auditadas; `TRANSICION_INVALIDA` en el salto no permitido; reapertura explícita levanta el solo lectura.
- Actual output: pendiente
- Status: pendiente

### TC-18 detectar-marcadores-en-conflicto-por-radio

- Tipo: unit
- Cubre: CU-11 (CA-01, CA-02, CA-03); RN-02
- Setup: relevamiento con radio 15 m y pares de marcadores a 9 m, 30 m y 20 m.
- Pasos:
  - Given un relevamiento con radio 15 m y dos marcadores a 9 m
  - When el jefe consulta los conflictos; en otro escenario con marcadores a 30 m; en otro con marcadores a 20 m amplía el radio a 25 m
  - Then lista los de 9 m como conflicto sin unificarlos; no lista los de 30 m; tras ampliar el radio a 25 m lista los de 20 m como conflicto
- Expected output: detección correcta por radio en los tres escenarios; sin unificación automática.
- Actual output: pendiente
- Status: pendiente

### TC-19 resolver-conflictos-unificar-o-separar

- Tipo: integration
- Cubre: CU-12 (CA-01, CA-02, CA-03); RN-02, RN-04, RN-05, RN-07
- Setup: dos marcadores en un mismo radio listados como conflicto; en un caso, conflicto pendiente en relevamiento cerrado.
- Pasos:
  - Given dos marcadores en un mismo radio listados como conflicto
  - When el jefe decide unificarlos; en otro escenario decide mantenerlos separados; en otro intenta resolver un conflicto en un relevamiento cerrado
  - Then unifica las observaciones en un único marcador y levanta el conflicto; o conserva ambos y levanta la marca; o responde `RELEVAMIENTO_SOLO_LECTURA` sobre el cerrado
- Expected output: unificación o separación según decisión, marca de conflicto levantada y auditada; código `RELEVAMIENTO_SOLO_LECTURA` sobre el cerrado.
- Actual output: pendiente
- Status: pendiente

### TC-20 registrar-auditoria-inmutable-retencion

- Tipo: integration
- Cubre: CU-13 (CA-01, CA-02, CA-03); RN-07, RN-08
- Setup: jefe general que da de alta un jefe de área; registro de auditoría de hace tres meses; consulta por accesos a un dato del último año.
- Pasos:
  - Given un jefe general que da de alta un jefe de área
  - When se ejecuta el alta; luego un usuario intenta modificar un registro de hace tres meses; luego el usuario raíz consulta los accesos a un dato del último año
  - Then asienta un registro con autor, momento y operación "alta de jefe de área"; responde `AUDITORIA_INMUTABLE` y conserva el registro; devuelve todos los accesos dentro de los doce meses
- Expected output: registro de auditoría completo e inmutable; `AUDITORIA_INMUTABLE` ante intento de modificación; consulta retorna accesos del período ≥ 12 meses.
- Actual output: pendiente
- Status: pendiente

### TC-21 autorizar-acceso-por-rol-y-area

- Tipo: unit
- Cubre: CU-14 (CA-01, CA-02, CA-03); RN-01, RN-08
- Setup: jefe de "Zona Norte"; relevamiento de "Zona Sur" y de "Zona Norte"; agente de campo intentando ver datos personales de otro agente.
- Pasos:
  - Given un jefe de "Zona Norte"
  - When intenta abrir un relevamiento de "Zona Sur"; luego un agente intenta consultar datos personales de otro agente; luego el jefe accede a un relevamiento de "Zona Norte"
  - Then responde `ACCESO_NO_AUTORIZADO` y registra el intento; responde `ACCESO_DATO_PERSONAL_NO_AUTORIZADO` y registra el intento; concede el acceso al recurso de su área
- Expected output: bloqueo y registro de los accesos indebidos con sus códigos; concesión del acceso legítimo.
- Actual output: pendiente
- Status: pendiente

### TC-22 ubicar-punto-manual-fuente-ubicacion

- Tipo: unit
- Cubre: CU-05 (CA-01, CA-02); RN-02, RN-03, RN-05
- Setup: observación con foto sin metadatos en relevamiento con radio 15 m y un marcador existente; foto con metadatos cargada desde la web.
- Pasos:
  - Given una observación con foto sin metadatos en un relevamiento con radio 15 m
  - When el usuario coloca el punto a 5 m de un marcador existente; en otro escenario intenta ubicar manualmente una foto que sí trae metadatos
  - Then asocia la observación al marcador con la coordenada manual; y responde `FUENTE_UBICACION_INCORRECTA` usando los metadatos cuando estos existen
- Expected output: asociación por ubicación manual dentro del radio; prioridad de metadatos preservada con `FUENTE_UBICACION_INCORRECTA`.
- Actual output: pendiente
- Status: pendiente

### TC-23 detectar-conectividad-disparar-sync-automatica

- Tipo: e2e
- Cubre: NFR detección de conectividad; CU-07
- Setup: dispositivo Android por USB con cola de cambios pendiente; red cortada y luego restablecida.
- Pasos:
  - Given un dispositivo con cambios pendientes y sin señal
  - When se restablece la conexión
  - Then la app detecta la conexión automáticamente y dispara el pipeline de sincronización sin intervención manual
- Expected output: disparo automático del ciclo de sync al recuperar señal; cola sincronizada.
- Actual output: pendiente
- Status: pendiente

### TC-24 latencia-api-lecturas-administrativas-p95

- Tipo: integration
- Cubre: NFR latencia API (p95 ≤ 500 ms)
- Setup: backend efímero (WebApplicationFactory) con dataset sintético de revisión; carga sobre endpoints de lectura administrativa.
- Pasos:
  - Given un backend con dataset sintético de revisión sobre mapa
  - When se ejecuta una carga de lecturas administrativas sobre los endpoints de revisión
  - Then el percentil 95 del tiempo de respuesta es ≤ 500 ms
- Expected output: p95 medido ≤ 500 ms.
- Actual output: pendiente
- Status: pendiente

### TC-25 contrato-openapi-coincide-con-api

- Tipo: contract
- Cubre: CU-08, CU-14 (superficie REST); ADR-02, ADR-11
- Setup: backend efímero (WebApplicationFactory); documento OpenAPI 3.x generado.
- Pasos:
  - Given la API REST en ejecución
  - When se genera el documento OpenAPI y se compara con el versionado en el repo, y se provocan errores de dominio
  - Then el documento coincide con la implementación y los errores se devuelven en `application/problem+json` (RFC 7807) con códigos estables
- Expected output: OpenAPI sin desvíos respecto del versionado; respuestas de error en Problem Details con códigos estables.
- Actual output: pendiente
- Status: pendiente

### TC-26 filehosting-conformidad-backend

- Tipo: contract
- Cubre: extensibilidad (`GeoVial.FileHosting`); ADR-08; RN-05, RN-08
- Setup: backend de archivos local por defecto y backend S3 contra endpoint no productivo o doble compatible.
- Pasos:
  - Given un backend de almacenamiento que implementa la abstracción de `GeoVial.FileHosting`
  - When se ejecutan las operaciones Guardar, Recuperar, Eliminar y Existe, incluyendo Guardar dos veces la misma referencia lógica
  - Then todas las operaciones cumplen el contrato, Guardar es idempotente para una misma referencia, la referencia es opaca y estable, y Eliminar respeta el solo lectura del relevamiento cerrado (RN-05)
- Expected output: el mismo set de aserciones pasa idéntico para el backend local y el S3; Guardar idempotente; referencia estable.
- Actual output: pendiente
- Status: pendiente

## 3. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Catálogo inicial de 26 TC referenciales con setup, pasos Given-When-Then, expected, actual (pendiente) y status (pendiente); cubre los 14 CU, las 8 RN, idempotencia de la cola, last-write-wins, EXIF→manual, autorización por rol/área, retención de auditoría, NFR numéricos y conformidad de backends de archivos. Generado por AG-08 |
