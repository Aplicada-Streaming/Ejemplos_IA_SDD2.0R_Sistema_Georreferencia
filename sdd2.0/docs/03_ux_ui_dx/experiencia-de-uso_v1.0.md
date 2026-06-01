# Experiencia de uso — GeoVial

**Proyecto:** GeoVial
**Documento:** experiencia-de-uso_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** UX/UI Designer + Frontend Lead (AG-03), Equipo SDD 2.0
**Variante:** UX/UI
**Trazabilidad upstream:** vision-producto_v1.0.md (persona objetivo, §4 visión, §8 R-04); alcance-proyecto_v1.0.md (§4 capacidades); especificacion-funcional_v1.0.md (CU-01, CU-02, CU-04, CU-05, CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12; RN-01 a RN-08)
**Trazabilidad downstream:** wireframes de esta misma categoría; 05_arquitectura_tecnica (capa de presentación web y móvil); 06_backlog-tecnico (US con criterios de aceptación de ergonomía); 08_calidad_y_pruebas (snapshot, accesibilidad, UI)

## 1. Audiencia y contexto de uso

GeoVial tiene dos personas objetivo principales, una en terreno y otra en escritorio, más roles administrativos secundarios. La experiencia se diseña para la tensión entre ambos contextos: captura rápida con una sola mano y poca atención disponible en campo, frente a revisión analítica y prolongada sobre mapa en oficina.

| Persona | Contexto físico | Contexto emocional | Frecuencia y duración | Implicancias de diseño |
| --- | --- | --- | --- | --- |
| Agente de campo (primaria, captura) | A la intemperie, de pie o en movimiento, sosteniendo el teléfono en una mano, sol directo, guantes posibles, conectividad intermitente o nula | Apurado, expuesto al clima, poca paciencia para pasos largos; baja especialización técnica (riesgo R-04 de adopción) | Por jornada completa de campo (hasta 8 horas sin conexión), captura repetida punto a punto | Objetivos táctiles grandes, mínima cantidad de pasos por foto, estado sin conexión siempre visible, recuperación sin perder datos, vocabulario llano |
| Jefe de área (primaria, revisión) | Escritorio con monitor amplio, conexión persistente, sesión larga | Concentrado, comparativo, busca evaluar y armar informes | Por relevamiento, sesiones largas de revisión y decisión | Mapa como eje, densidad de información controlable, navegación por teclado, filtros, comparación de fotos en carrusel |
| Jefe general / usuario raíz (secundaria, administración) | Escritorio, conexión persistente | Puntual, administrativo | Esporádica (altas, bajas, intervención sobre incoherencias) | Formularios claros, confirmaciones explícitas en acciones jerárquicas e irreversibles |

El contexto de uso dominante para el diseño de la app móvil es "captura de pie, sin conexión, sin tiempo"; el de la web es "revisión sentada, conectada, con tiempo". Cada superficie se prioriza para su contexto y no se fuerza la paridad innecesaria entre ambas.

## 2. Principios de diseño

### 2.1 Heurísticas de Nielsen aplicadas

| Heurística de Nielsen | Aplicación en GeoVial | Verificación |
| --- | --- | --- |
| Visibilidad del estado del sistema | Indicador de conexión persistente en móvil (sin conexión / sincronizando / sincronizado); contador de cambios pendientes en la cola; estado del relevamiento (recolección / revisión / cerrado) visible en web y móvil | Inspección heurística por dos revisores sobre cada wireframe |
| Correspondencia entre el sistema y el mundo real | Vocabulario del dominio del cliente (relevamiento, marcador, observación, etiqueta, bandeja sin georreferenciar); el mapa representa el terreno; el pin representa el punto físico | Revisión contra el glosario de 02 y glosario-ux_v1.0.md |
| Control y libertad del usuario | El agente puede mover el pin, recentrar por posicionamiento, quitar una foto antes de sincronizar; el jefe puede deshacer un filtro y reabrir un relevamiento cerrado con acción explícita | Pruebas de flujo con retroceso en cada wireframe |
| Prevención de errores | El botón de habilitar modo sin conexión queda inhabilitado hasta configurar el método de seguridad del teléfono (RN-06); confirmación antes de cerrar un relevamiento (acción que deja todo de solo lectura, RN-05); confirmación antes de unificar marcadores (RN-02) | Casos de prueba de estados deshabilitados en 08 |
| Reconocer antes que recordar | Lista de relevamientos asignados visible al iniciar; miniaturas en el carrusel; etiquetas como fichas seleccionables en vez de texto libre a memorizar | Inspección heurística |
| Flexibilidad y eficiencia de uso | Centrar por posicionamiento en un toque para el agente experimentado; navegación por teclado y filtros por etiqueta para el jefe; atajos de navegación entre marcadores | Pruebas con usuario novato y avanzado |
| Estética y diseño minimalista | En móvil, una acción primaria por pantalla (tomar foto); el resto de controles, secundarios y agrupados | Revisión de jerarquía visual en wireframes |
| Ayudar a reconocer, diagnosticar y recuperarse de errores | Mensajes con causa y acción siguiente, nunca códigos crudos al usuario final (ver §8); la observación sin ubicación va a una bandeja recuperable, no se pierde | Catálogo de mensajes de §8 verificado contra los códigos de los CU |
| Ayuda y documentación | Microcopy contextual en los puntos de fricción (qué es el radio, por qué se pide configurar el método de seguridad); el portal de developers cubre el reuso de la librería de sincronización (dx-portal-developers_v1.0.md) | Revisión de microcopy por AG-10 |

### 2.2 Leyes UX relevantes

| Ley UX | Aplicación en GeoVial | Justificación |
| --- | --- | --- |
| Ley de Fitts | Objetivos táctiles grandes y ubicados al alcance del pulgar en la captura móvil; el disparador de foto es el objetivo más grande y central | El agente opera con una mano, en movimiento y con posible uso de guantes; objetivos chicos elevan la tasa de error en campo |
| Ley de Hick | En móvil se reduce el número de decisiones por pantalla a una acción primaria; las opciones secundarias se agrupan u ocultan tras un menú | Menos opciones simultáneas acortan el tiempo de decisión en un contexto con poca atención disponible (mitiga R-04) |
| Ley de Jakob | El mapa con pines, el carrusel de fotos y el patrón de etiquetas siguen convenciones ya conocidas de apps de mapas y galerías | Personal poco especializado adopta más rápido patrones que ya conoce de otras aplicaciones |
| Ley de Miller | La navegación entre marcadores y el carrusel no exigen recordar más de unos pocos elementos a la vez; el conteo de fotos y de pendientes se muestra, no se memoriza | Reduce la carga cognitiva en sesiones largas de revisión y en captura apurada |
| Ley de Tesler (conservación de la complejidad) | La complejidad de la resolución de conflictos se concentra en la web, en el jefe de área, y se mantiene fuera de la captura móvil | El agente de campo no decide unificaciones; esa complejidad irreducible vive donde hay tiempo y criterio (RN-02, RN-04) |
| Ley de Doherty (umbral de respuesta) | Feedback inmediato a la captura de foto y a la colocación del pin, por debajo de medio segundo percibido; las operaciones largas (sincronización, exportación) muestran progreso | Mantener la respuesta por debajo del umbral sostiene el ritmo de captura y la sensación de control |

## 3. Flujos clave

Cada flujo se ancla en uno o más CU de 02 y declara disparador, pasos, fricción anticipada y salida.

### 3.1 Iniciar sesión y habilitar el campo sin conexión (CU-02, RN-06)

- Disparador: el agente abre la app por primera vez con conexión, o reingresa en terreno sin señal.
- Pasos: ingresa credenciales con conexión; el sistema verifica el método de seguridad del teléfono; si falta, lo guía a configurarlo; al estar configurado, habilita el modo sin conexión; muestra la lista de relevamientos asignados; el agente elige uno no cerrado. En terreno, el reingreso usa el método de seguridad y abre el último relevamiento.
- Fricción anticipada: el agente no entiende por qué se le exige configurar el método de seguridad. Mitigación: microcopy que explica que sin ese método no se puede trabajar sin conexión, con un botón directo a la configuración.
- Salida: sesión activa, relevamiento abierto, modo sin conexión habilitado o claramente bloqueado con la razón.

### 3.2 Capturar una observación georreferenciada en campo (CU-04, CU-06, RN-03)

- Disparador: el agente llega a un punto de la obra y quiere registrarlo.
- Pasos: toma una foto; el sistema resuelve la coordenada desde los metadatos de la foto y crea o asocia el marcador según el radio; el agente agrega comentario y etiquetas; confirma. Todo se guarda localmente y se encola.
- Fricción anticipada: la foto no trae ubicación; el agente no sabe qué hacer. Mitigación: se lo deriva a ubicar el punto en el mapa o a dejar la observación en la bandeja sin georreferenciar, sin perderla.
- Salida: observación asentada con coordenada o derivada a la bandeja sin georreferenciar; contador de pendientes actualizado.

### 3.3 Sincronizar al recuperar señal (CU-07, RN-04, RN-02)

- Disparador: el dispositivo recupera conexión.
- Pasos: el sistema detecta la conexión y avisa que hay pendientes; sube primero los cambios locales, baja luego las actualizaciones de los relevamientos asignados; marca los choques como conflicto sin unificar nada.
- Fricción anticipada: la sincronización se corta a la mitad; el agente teme perder datos. Mitigación: la cola conserva lo no confirmado y reanuda sin duplicar; el mensaje lo dice explícitamente.
- Salida: cola vacía de lo sincronizado, conflictos señalados para la web, sin pérdida.

### 3.4 Revisar un relevamiento sobre el mapa (CU-08, CU-09)

- Disparador: el jefe de área abre un relevamiento para evaluarlo.
- Pasos: ve los marcadores ubicados y la bandeja sin georreferenciar; selecciona un marcador; recorre su carrusel de fotos con comentarios y etiquetas; navega entre marcadores; filtra por etiqueta; exporta el relevamiento completo si lo necesita.
- Fricción anticipada: muchos marcadores superpuestos en el mapa. Mitigación: agrupamiento visual de pines cercanos y un panel lista sincronizado con el mapa.
- Salida: el jefe completó su evaluación y, si correspondía, obtuvo el archivo exportado.

### 3.5 Gestionar relevamientos y agentes (CU-01, CU-10)

- Disparador: el jefe de área organiza el trabajo de su área.
- Pasos: crea un relevamiento con su obra y su radio de agrupación; asigna o reasigna agentes de su área; transiciona el estado del relevamiento (recolección → revisión → cierre) y lo reabre con acción explícita cuando hace falta.
- Fricción anticipada: cerrar por error y dejar todo de solo lectura. Mitigación: confirmación explícita del cierre con su consecuencia descrita.
- Salida: relevamiento creado o avanzado de estado, agentes asignados, acción auditada.

### 3.6 Resolver conflictos desde la web (CU-11, CU-12, RN-02, RN-04)

- Disparador: tras una sincronización quedan conflictos.
- Pasos: el jefe abre la lista de conflictos; para marcadores en un mismo radio decide unificar o mantener separados; para ediciones en conflicto confirma la versión que prevalece; el sistema aplica la decisión y levanta la marca.
- Fricción anticipada: el jefe no distingue qué tipo de conflicto está resolviendo. Mitigación: la lista separa visualmente "marcadores en un mismo radio" de "ediciones en conflicto", cada uno con su acción propia.
- Salida: conflicto resuelto por decisión humana, base consistente, resolución auditada.

## 4. Estados y feedback

Estados mínimos por superficie clave. El estado sin conexión es central en las superficies móviles. Convención: cada superficie expone vacío, cargando, con datos, error, sin conexión (donde aplica) y éxito.

### 4.1 Captura móvil (CU-04, CU-06)

| Estado | Condición que lo produce | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Vacío | Relevamiento abierto sin observaciones aún | Mapa centrado con pin de posición y disparador de foto destacado | "Tomá la primera foto para registrar un punto" |
| Cargando | Posicionamiento resolviéndose o foto procesándose | Indicador breve sobre el disparador | "Ubicando…" |
| Con datos | Observaciones capturadas en la jornada | Pines en el mapa y contador de pendientes | "12 observaciones, 12 sin sincronizar" |
| Error | La foto no trae ubicación (OBSERVACION_SIN_GEORREFERENCIA) o no hay espacio (ALMACENAMIENTO_LOCAL_INSUFICIENTE) | Aviso inline sobre la observación afectada | "Esta foto no trae ubicación. Ubicá el punto en el mapa o dejala en la bandeja sin georreferenciar." |
| Sin conexión | Sin señal en terreno | Cinta persistente de estado sin conexión | "Sin conexión. Tus cambios se guardan y se sincronizan al recuperar señal." |
| Éxito | Observación asentada localmente | Confirmación sutil y pin nuevo en el mapa | "Observación guardada" |

### 4.2 Sincronización (CU-07)

| Estado | Condición | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Vacío | Cola sin pendientes | Ícono neutro de sincronizado | "Todo sincronizado" |
| Cargando | Sincronización en curso | Barra de progreso con conteo subido/bajado | "Subiendo 12 de 100…" |
| Con datos | Pendientes encolados a la espera de señal | Contador de pendientes | "100 cambios pendientes de sincronizar" |
| Error | Consolidación fallida (CONSOLIDACION_INVALIDA) o corte (SINCRONIZACION_INTERRUMPIDA) | Banner con reintento | "Se interrumpió la sincronización. No se perdió nada; reintentá cuando tengas señal." |
| Sin conexión | Señal perdida durante o antes de sincronizar | Cinta de estado sin conexión | "Esperando señal para sincronizar" |
| Éxito | Cola vaciada de lo sincronizado | Confirmación y marca temporal | "Sincronizado. Quedaron 2 conflictos para resolver en la web." |

### 4.3 Mapa de revisión web (CU-08)

| Estado | Condición | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Vacío | Relevamiento sin marcadores | Mapa con mensaje orientativo y acceso a la bandeja sin georreferenciar | "Este relevamiento todavía no tiene marcadores" |
| Cargando | Carga de marcadores | Skeleton del panel lista y del mapa | "Cargando marcadores…" |
| Con datos | Marcadores ubicados | Pines en el mapa y panel lista sincronizado | Conteo de marcadores y de observaciones |
| Error | Acceso denegado (ACCESO_NO_AUTORIZADO) o importación inválida (ARCHIVO_EXPORTACION_INVALIDO) | Banner inline | "No tenés acceso a este relevamiento" / "El archivo no es un relevamiento completo y coherente" |
| Éxito | Exportación lista | Confirmación con descarga | "Relevamiento exportado" |

### 4.4 Carrusel de marcador (CU-09)

| Estado | Condición | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Vacío | Marcador sin fotos | Marco neutro de marcador vacío | "Este marcador no tiene fotos todavía" |
| Cargando | Fotos cargándose | Skeleton de miniaturas | "Cargando fotos…" |
| Con datos | Fotos con comentarios y etiquetas | Carrusel con miniaturas y posición actual | "Foto 3 de 8" |
| Error | Marcador inexistente (MARCADOR_INEXISTENTE) | Aviso y retorno al relevamiento | "Ese marcador ya no existe en el relevamiento" |
| Solo lectura | Relevamiento cerrado (RELEVAMIENTO_SOLO_LECTURA) | Controles de edición ocultos o inhabilitados | "Relevamiento cerrado: solo lectura" |
| Éxito | Cambio persistido | Confirmación sutil | "Cambios guardados" |

### 4.5 Resolución de conflictos web (CU-11, CU-12)

| Estado | Condición | Feedback visual | Feedback textual |
| --- | --- | --- | --- |
| Vacío | Sin conflictos pendientes | Ícono neutro | "No hay conflictos pendientes" |
| Cargando | Detección en curso | Skeleton de lista | "Buscando conflictos…" |
| Con datos | Conflictos listados | Lista separada por tipo (mismo radio / ediciones) | Conteo por tipo |
| Error | Solo lectura (RELEVAMIENTO_SOLO_LECTURA) o ya resuelto (CONFLICTO_INEXISTENTE) | Banner inline | "El relevamiento está cerrado; reabrilo para resolver" / "Ese conflicto ya fue resuelto" |
| Éxito | Conflicto resuelto | Ítem se marca resuelto y baja de la lista | "Conflicto resuelto" |

## 5. Accesibilidad

Compromiso explícito: WCAG 2.2 nivel AA como piso mínimo en web y en las páginas integradas de la app móvil. Las versiones anteriores de WCAG solo se mencionarían como evolución histórica; el objetivo de cumplimiento es 2.2 AA.

Criterios prioritarios:

- Contraste de texto 4.5:1 y de componentes de interfaz y datos esenciales 3:1, incluido el texto sobre el mapa y sobre fotos (caption con fondo legible).
- Foco visible y orden de foco lógico en todos los controles, en web y en la captura móvil; el foco nunca queda atrapado en el carrusel ni en los modales.
- Operación completa por teclado en la web (mapa, panel lista, carrusel, filtros, resolución de conflictos); objetivo táctil mínimo adecuado en móvil (Ley de Fitts en §2.2).
- Etiquetas semánticas y nombres accesibles en todos los formularios (login, alta de relevamiento, asignación de agentes) y en los controles del mapa y del carrusel.
- Alternativas textuales: cada foto admite y muestra su comentario y etiquetas como texto asociado; los pines del mapa exponen nombre y conteo de observaciones como texto accesible, no solo color.
- Anuncios de cambios dinámicos por región en vivo: estado de conexión, progreso y resultado de la sincronización, confirmación de guardado y aparición de conflictos.
- No depender solo del color para distinguir estados (sin conexión, conflicto, solo lectura): se acompaña con ícono y texto.
- Objetivo de tamaño y espaciado de controles acorde a 2.2 (criterio de tamaño de objetivo) en los controles críticos de campo.

## 6. Internacionalización

- Idioma soportado: español rioplatense neutro técnico, único idioma de la audiencia (un organismo en una sola región lingüística). No se declara estrategia multilenguaje, coherente con la nota de alcance-proyecto_v1.0.md.
- Dirección de lectura: izquierda a derecha.
- Expansión de texto: aunque hay un solo idioma, los contenedores de microcopy y de etiquetas se diseñan con holgura para no truncar comentarios largos del agente; las etiquetas largas se muestran completas o con elipsis recuperable.
- Formato de fecha: YYYY-MM-DD en datos y marcas temporales de sincronización y auditoría; en presentación al usuario se admite formato local con día, mes y año sin ambigüedad.
- Formato de número: separador decimal con coma en presentación de distancias y radios (por ejemplo, radio de agrupación en metros), coherente con la región.
- Formato de coordenadas: las coordenadas se presentan en grados decimales con precisión suficiente para ubicar el punto; el dato crudo no se exige al usuario de campo, que opera sobre el mapa, no sobre números.
- Unidades: distancias y radios en metros.

## 7. Performance percibida

| Acción | Tiempo máximo tolerable percibido | Técnica |
| --- | --- | --- |
| Toma de foto y confirmación de guardado local | Inmediato, por debajo de medio segundo percibido (Ley de Doherty) | Confirmación optimista local; el encolado y la coordenada se resuelven sin bloquear la siguiente captura |
| Colocación o movimiento del pin en el mapa | Inmediato | Respuesta directa al gesto; recentrar por posicionamiento sin recargar el mapa |
| Carga del mapa de revisión con marcadores | Hasta unos pocos segundos | Skeleton del panel y del mapa; agrupamiento de pines para no renderizar todo de golpe |
| Avance en el carrusel de fotos | Inmediato | Miniaturas precargadas; la foto siguiente se prepara mientras se ve la actual |
| Sincronización de una jornada típica | Operación larga con progreso visible; objetivo de negocio ≤ 5 minutos | Barra de progreso con conteo; nunca bloquea la app; se puede seguir trabajando |
| Exportación del relevamiento completo | Operación larga con progreso | Indicador de progreso y confirmación al finalizar; no se presenta como instantánea |

Criterios de animación: animaciones breves y funcionales (transición de carrusel, aparición de confirmaciones); se respeta la preferencia de movimiento reducido del sistema; ninguna animación bloquea la captura ni la revisión.

## 8. Errores y recuperación

Principio: el usuario final ve mensajes en lenguaje llano con causa y acción siguiente; los códigos de los CU (por ejemplo `OFFLINE_NO_HABILITADO`) son identificadores internos para 06 y 08, no texto de cara al usuario. Tono respetuoso, sin culpar a la persona.

| Código interno (CU) | Superficie | Mensaje al usuario (tono y acción) | Vía de recuperación |
| --- | --- | --- | --- |
| `CREDENCIALES_INVALIDAS` (CU-02) | Login web/móvil | "No pudimos validar tus datos. Revisá usuario y contraseña e intentá de nuevo." | Reintento; el intento queda registrado |
| `OFFLINE_NO_HABILITADO` (CU-02) | Login móvil | "Para trabajar sin conexión, configurá el método de seguridad del teléfono." | Botón directo a la configuración del método |
| `REINGRESO_SIN_METODO_SEGURIDAD` (CU-02, CU-06) | Reingreso en campo | "Necesitás el método de seguridad del teléfono para reingresar en el campo." | Configurar el método en un inicio con conexión |
| `OBSERVACION_SIN_GEORREFERENCIA` (CU-04, CU-05) | Captura móvil / carga web | "Esta foto no trae ubicación. Ubicá el punto en el mapa o dejala en la bandeja sin georreferenciar." | Ubicar el pin (CU-05) o derivar a la bandeja sin georreferenciar |
| `FUENTE_UBICACION_INCORRECTA` (CU-05) | Carga web | "Esta foto ya trae su ubicación; se usa esa y no hace falta ubicarla a mano." | El sistema usa los metadatos automáticamente |
| `RELEVAMIENTO_SOLO_LECTURA` (CU-04, CU-09, CU-10, CU-12) | Web y móvil | "Este relevamiento está cerrado, solo se puede consultar. Para editarlo, reabrilo." | Reapertura explícita por el jefe de área (CU-10) |
| `ALMACENAMIENTO_LOCAL_INSUFICIENTE` (CU-06) | Captura móvil | "El teléfono se quedó sin espacio. Lo ya guardado está a salvo; liberá espacio para seguir." | Liberar espacio; lo encolado se conserva |
| `CONSOLIDACION_INVALIDA` / `SINCRONIZACION_INTERRUMPIDA` (CU-07) | Sincronización | "Se interrumpió la sincronización. No se perdió nada; reintentá cuando tengas señal." | Reanudación sin duplicar; reintento automático y manual |
| `ACCESO_NO_AUTORIZADO` (varios) | Web | "No tenés acceso a este relevamiento." | Cambiar de relevamiento; el intento queda registrado |
| `ARCHIVO_EXPORTACION_INVALIDO` (CU-08) | Importación web | "El archivo no es un relevamiento completo y coherente; no se importó nada." | Reintentar con un archivo válido; los datos existentes no se tocan |
| `TRANSICION_INVALIDA` / `REAPERTURA_NO_AUTORIZADA` (CU-10) | Estados web | "No se puede pasar a ese estado desde el actual." / "La reapertura requiere tu acción explícita." | Elegir transición válida; confirmar la reapertura |
| `UNIFICACION_NO_AUTORIZADA` (CU-11) | Conflictos web | "Los marcadores no se unifican solos; vos decidís si unificar o mantener separados." | El jefe decide en CU-12 |
| `CONFLICTO_INEXISTENTE` (CU-12) | Conflictos web | "Ese conflicto ya fue resuelto." | Refrescar la lista de conflictos |

Handoff humano: cuando un conflicto excede al jefe de área, el usuario raíz interviene para resolver la incoherencia (CU-12, flujo 5.B); el camino de escalamiento se nombra en el mensaje cuando aplica.

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Agente de campo poco especializado (captura) y jefe de área (revisión sobre mapa), de vision-producto_v1.0.md §2 |
| CU origen | CU-01, CU-02, CU-04, CU-05, CU-06, CU-07, CU-08, CU-09, CU-10, CU-11, CU-12 (02_especificacion_funcional) |
| Reglas de negocio relevantes | RN-01 (autorización), RN-02 (radio), RN-03 (prioridad de metadatos), RN-04 (última escritura), RN-05 (estados/solo lectura), RN-06 (método de seguridad), RN-07 (auditoría), RN-08 (datos personales) |
| Wireframes asociados | wireframes-login-relogueo_v1.0.md, wireframes-mapa-revision-web_v1.0.md, wireframes-marcador-carrusel_v1.0.md, wireframes-captura-movil_v1.0.md, wireframes-gestion-relevamientos_v1.0.md, wireframes-resolucion-conflictos-web_v1.0.md |
| US a generar | a generar en 06 (login y relogueo, captura georreferenciada, sincronización, revisión sobre mapa, carrusel y navegación, gestión de relevamientos y estados, resolución de conflictos) |
| Tests previstos | a generar en 08 (snapshot de estados por superficie, accesibilidad WCAG 2.2 AA, UI móvil de captura y sincronización, UI web de revisión y conflictos) |

## 10. Notas y supuestos

- El sistema es de un único idioma (español); no se diseña selector de idioma ni soporte RTL, coherente con la nota de internacionalización de alcance-proyecto_v1.0.md.
- El detalle visual fino (paleta, tipografía, tokens) no se fija acá: pertenece a 05 o al design system. Esta sección define experiencia, estados y comportamiento.
- El método de seguridad del teléfono, la georreferenciación, el carrusel, las etiquetas, el archivo comprimido y el radio son conceptos del dominio del cliente y se nombran como tales; no se nombra el stack que los implementa.
- Los valores marcados como "Supuesto a validar" en los intakes ya están resueltos aguas arriba; esta sección no reabre supuestos bloqueantes.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Marco de experiencia inicial UX/UI para GeoVial, generado por AG-03 a partir de 00 y 02 |
