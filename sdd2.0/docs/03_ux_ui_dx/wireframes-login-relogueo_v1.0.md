# Wireframe — Inicio de sesión y relogueo en campo

**Proyecto:** GeoVial
**Documento:** wireframes-login-relogueo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** UX/UI Designer + Frontend Lead (AG-03), Equipo SDD 2.0
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie de acceso de GeoVial en web y móvil. El usuario inicia sesión la primera vez con conexión y, en el caso del agente de campo, reingresa en terreno sin señal con el método de seguridad del teléfono. La pantalla cumple además la precondición de habilitar el modo sin conexión (RN-06): sin el método de seguridad configurado, no se habilita el trabajo offline.

## 2. Layout

Login inicial con conexión (móvil, portrait):

```
+----------------------------------+
|            GeoVial               |
|                                  |
|   [ Usuario .................. ] |
|   [ Contraseña ............... ] |
|                                  |
|   [      Iniciar sesión       ] |
|                                  |
|   (i) El primer ingreso         |
|       necesita conexión.         |
+----------------------------------+
| (cinta) Estado: conectado        |
+----------------------------------+
```

Paso de configuración del método de seguridad (móvil, tras login si falta):

```
+----------------------------------+
|  Configurá el método de          |
|  seguridad del teléfono          |
|                                  |
|  Sin esto no podés trabajar      |
|  sin conexión en el campo.       |
|                                  |
|  [  Configurar método        ]  |
|  [  Más tarde (sin offline)  ]  |
+----------------------------------+
```

Relogueo en terreno sin conexión (móvil, portrait):

```
+----------------------------------+
| (cinta) Sin conexión             |
|----------------------------------|
|        Reingreso en campo        |
|                                  |
|   Validá con el método de        |
|   seguridad del teléfono         |
|                                  |
|   [   Validar e ingresar    ]    |
|                                  |
|   Último relevamiento:           |
|   "Puente Río 12"                |
+----------------------------------+
```

Login web (escritorio, jefe de área / administración):

```
+--------------------------------------------------+
|  GeoVial — Acceso                                |
|                                                  |
|     [ Usuario ............................. ]    |
|     [ Contraseña .......................... ]    |
|     [               Iniciar sesión        ]      |
|                                                  |
|     Estado del sistema: conectado                |
+--------------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Campo usuario / contraseña | Capturar credenciales para el primer ingreso con conexión | Texto ingresado (contraseña enmascarada) | Validación de presencia; envío deshabilitado si faltan |
| Botón iniciar sesión | Disparar la autenticación | — | Deshabilitado hasta completar campos; muestra estado de carga al enviar |
| Aviso de método de seguridad | Explicar y guiar la configuración del método del teléfono (RN-06) | Mensaje y consecuencia | Lleva a configurar; permite continuar sin offline |
| Botón validar e ingresar (campo) | Reingreso sin conexión con el método de seguridad | Nombre del último relevamiento | Invoca el método de seguridad del teléfono; abre el último relevamiento |
| Cinta de estado de conexión | Hacer visible si hay o no conexión (Nielsen: visibilidad del estado) | Conectado / sin conexión | Persistente; cambia con la conectividad |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Iniciar sesión con conexión | Toque/clic en iniciar sesión | Sesión activa y lista de relevamientos asignados | Hay conexión; credenciales válidas |
| Configurar método de seguridad | Toque en configurar método | Se habilita el modo sin conexión | Primer ingreso con conexión |
| Reingresar en campo | Toque en validar e ingresar | Abre el último relevamiento en modo sin conexión | Método de seguridad configurado (RN-06) |
| Posponer configuración | Toque en "Más tarde" | Sesión activa sin modo sin conexión habilitado | El agente acepta no trabajar offline por ahora |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Formulario sin datos | Campos vacíos, botón de ingreso deshabilitado |
| Cargando | Autenticación o validación en curso | Indicador de carga en el botón; campos bloqueados |
| Con datos | Credenciales o último relevamiento presentes | Botón habilitado; nombre del último relevamiento visible en el reingreso |
| Error | `CREDENCIALES_INVALIDAS`, `OFFLINE_NO_HABILITADO`, `REINGRESO_SIN_METODO_SEGURIDAD` | Mensaje en lenguaje llano con acción siguiente (ver experiencia-de-uso §8) |
| Sin conexión | Sin señal al reingresar en campo | Cinta de sin conexión; se ofrece reingreso por método de seguridad, no login con credenciales |
| Éxito | Acceso concedido | Transición a la lista de relevamientos o al último relevamiento abierto |

## 6. Versión móvil o responsive

- El login y el reingreso son superficies primarias en móvil portrait; los campos y botones ocupan el ancho útil al alcance del pulgar (Ley de Fitts).
- En web (escritorio) solo aplica el login con credenciales; no existe reingreso sin conexión, porque el modo sin conexión es exclusivo de la captura móvil.
- La cinta de estado de conexión se fija arriba en móvil y se mantiene visible al desplazar.

## 7. Notas de implementación

- Accesibilidad: etiquetas semánticas en cada campo, nombre accesible en los botones, foco visible y orden de foco lógico; el cambio de estado de conexión se anuncia por región en vivo; el error se asocia al campo o se anuncia, no solo por color (WCAG 2.2 AA).
- Performance percibida: el botón muestra carga inmediata al enviar; el reingreso por método de seguridad responde sin recargar la app.
- Internacionalización: textos en español; el microcopy del método de seguridad se dimensiona con holgura para no truncar.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU origen | CU-02 (iniciar sesión y seleccionar relevamiento; relogueo en terreno como flujos 5.A y 5.B) |
| Reglas de negocio | RN-01, RN-06, RN-07 |
| Marco experiencia-de-uso | §3.1 (flujo de acceso), §4.1, §8 (errores de acceso) |
| US a generar | a generar en 06 (login con conexión, configuración del método de seguridad, relogueo sin conexión) |
| Tests previstos | a generar en 08 (acceso, habilitación del modo sin conexión, relogueo en campo, accesibilidad del formulario) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Wireframe inicial de login y relogueo en campo, generado por AG-03 a partir de CU-02 y RN-06 |
