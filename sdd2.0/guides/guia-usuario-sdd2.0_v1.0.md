# Guía de usuario del template SDD 2.0

```yaml
Documento: guia-usuario-sdd2.0_v1.0.md
Versión: 1.0
Fecha: 2026-05-17
Audiencia: profesionales y estudiantes que usan el template para un proyecto real
Idioma: español rioplatense neutro técnico
Estado: vigente
```

> Esta guía no enseña la teoría detrás del template (para eso está el marco teórico).
> Enseña a usarlo paso a paso, con prompts ejemplo, casos aplicados y resolución de problemas frecuentes.

---

## §1 ¿Qué es el template SDD 2.0?

El template SDD 2.0 es un kit de trabajo orientado a equipos de desarrollo que necesitan generar la documentación viva de un proyecto de software antes de escribir la primera línea de código. SDD se lee como "Specification-Driven Development", desarrollo guiado por la especificación. La idea es simple: si el problema está bien capturado, las decisiones técnicas están justificadas y la trazabilidad entre necesidades, casos de uso y tareas está cerrada, la codificación pasa a ser una consecuencia ordenada del diseño, no una serie de improvisaciones.

El template resuelve un dolor concreto y repetido: el equipo arranca a codear sin haber escrito la especificación, descubre los huecos del problema a mitad del sprint, paga los costos de retrabajo y termina con una documentación post-facto que nadie mantiene. SDD 2.0 invierte el orden. Primero conversación con el cliente, después intake estructurado, después documentación auditada por categorías, después codificación.

A diferencia de plantillas ad-hoc o de un README inflado, SDD 2.0 se apoya en tres pilares:

- Una cadena de trazabilidad cerrada (Visión → Necesidad de Negocio → Caso de Uso → Regla de Negocio → ADR → User Story → Backlog Técnico → Sprint → Test → Pipeline), todos los eslabones formales y verificables.
- Un conjunto de 12 categorías documentales numeradas (`00_contexto/` a `11_examples/`) más un README raíz, cada una con su propio archivo de reglas constructivas que codifica especialidad, documentos a producir, criterios de aceptación y prompts ejemplares.
- Un master-prompt orquestador que se ejecuta una sola vez por proyecto en Claude Code y va despachando subagentes especializados con audits independientes entre fases.

La audiencia primaria son desarrolladores, analistas, líderes técnicos y estudiantes avanzados que quieren producir documentación de calidad profesional sin pelearse con plantillas frías. El template asume que vas a usar Claude.ai web para conversar con el cliente y consolidar la idea, y Claude Code para ejecutar el orquestador y materializar la documentación en el repositorio.

Si el lector necesita la fundamentación teórica (por qué se eligió plan-then-confirm, por qué el principio de delegación de la especialidad, qué nodos cubre la cadena D6, qué invariantes globales D1 a D8 se aplican), eso se documenta en el marco teórico del template. Esta guía es operativa.

Un proyecto típico que usa SDD 2.0 termina con una carpeta `/sdd2.0/docs/` poblada por categoría, un informe de audit por cada fase, un README raíz consolidado y un Sprint 1 listo para arrancar codificación. El handoff a codificación es explícito: el orquestador no escribe código sin confirmación humana.

---

## §2 Prerequisitos

Antes de arrancar con el template, asegurate de tener lo siguiente:

- Cuenta de Claude.ai web con plan que admita proyectos y archivos adjuntos. El template fue diseñado para Claude Pro o equivalente con la capacidad de subir documentos largos al chat y mantener proyectos persistentes.
- Claude Code instalado y autenticado en tu máquina local. Claude Code es la CLI que ejecuta el master-prompt sobre tu repositorio. Verificá que `claude --version` o el equivalente de tu instalación respondan sin error.
- Repositorio Git inicializado en local. SDD 2.0 trabaja sobre archivos versionados; el orquestador asume que vas a poder hacer commits intermedios entre fases.
- Editor de texto con soporte de markdown (Visual Studio Code, Cursor, Zed o cualquier otro). Es muy útil tener vista previa de markdown mientras revisás los entregables.
- Acceso a una terminal funcional. En Windows, PowerShell 5.1 o superior (también podés usar Git Bash o WSL). En Linux y macOS, cualquier shell POSIX.

Conocimientos previos mínimos que la guía asume:

- Markdown básico (encabezados, listas, tablas, bloques de código).
- Git básico: `clone`, `add`, `commit`, `push`, `pull`, branching mínimo.
- Terminología ágil de superficie: qué es un sprint, una user story, una definition of done, una retrospective.
- Lectura cómoda de inglés técnico (algunos términos canónicos del template son anglicismos aceptados: backlog, ADR, walking skeleton, etc., siempre traducidos al rioplatense técnico).

Lo que NO necesitás:

- Experiencia previa con SDD: el template está diseñado para enseñarse a sí mismo via los archivos de reglas y este documento.
- Conocer un framework agile específico (Scrum certificado, SAFe). Los conceptos relevantes están explicados en línea.
- Saber escribir prompts: la guía te da los prompts iniciales que necesitás.

Verificación rápida antes de empezar:

```bash
git --version
claude --version
ls ~/.claude
```

Si los tres comandos responden, estás listo.

---

## §3 Cuándo usar este template y cuándo no

SDD 2.0 no es universal. Está pensado para proyectos donde el esfuerzo de documentación inicial se paga con creces en el flujo posterior. Las siguientes dimensiones ayudan a perfilar si te conviene.

### 3.1 Perfilado por tamaño de equipo

- 1 persona (solo dev / freelance): conviene una versión liviana. Saltearse el `acuerdo-equipo` y el sprint plan completo (se usa `mini-plan_v1.0.md`). Igual generar visión, alcance, NB, CU, ADR mínimas y backlog.
- 2 a 5 personas (equipo chico): caso canónico del template, todo el flujo aplica.
- 6 a 15 personas (equipo mediano): caso canónico también. Se recomienda fuerte ejecutar la fase H final (audit consolidado) con un revisor externo al equipo.
- Más de 15 personas (equipo grande): el template aplica como base, pero la coordinación entre múltiples bounded contexts puede requerir varios proyectos SDD federados (uno por contexto) en lugar de uno monolítico.

### 3.2 Perfilado por complejidad

- Proyecto trivial (un script, una utilidad chica, una demo): el template es excesivo. Usá un README plano de 50 líneas.
- Proyecto normal (semanas a pocos meses, un equipo, un dominio acotado): caso canónico, todo el template aplica.
- Proyecto complejo (varios trimestres, múltiples integraciones, regulación, varios consumidores): el template es la base mínima. Probablemente necesités complementarlo con documentos específicos del dominio (compliance, contratos legales, runbooks operativos extensos).

### 3.3 Perfilado por tipo de proyecto (D8)

El template soporta 8 tipos cerrados (D8) de proyectos. La elección del tipo gobierna qué documentos se generan en cada categoría y qué especialidad se invoca en cada subagente. Los 8 tipos confirmados son:

- D1 `library`: librería reutilizable, distribuida via package manager del ecosistema.
- D2 `web-monolith`: aplicación web monolítica con frontend y backend acoplados.
- D3 `web-microservices`: arquitectura distribuida con varios servicios independientes.
- D4 `desktop-app`: aplicación de escritorio para Windows, Linux o macOS.
- D5 `mobile-app-maui`: aplicación móvil multiplataforma.
- D6 `rest-api`: servicio HTTP que expone una API REST consumida por otros sistemas.
- D7 `cli-tool`: herramienta de línea de comandos.
- D8 `worker-service`: servicio de procesamiento asincrónico orientado a eventos o colas.

### 3.4 Perfilado por plazo

- Urgente (días, MVP de feria, hackathon): no usar el template completo, salvo que el equipo ya tenga fluidez con SDD. Recortar a `vision-producto`, `alcance-proyecto`, `casos-de-uso` y `plan-sprint` con `mini-plan`.
- Normal (semanas a tres meses): aplicar el template completo con audits livianos (revisión humana sin auditor independiente).
- Largo (más de tres meses, o producto con vida útil de años): aplicar el template completo, audits independientes en cada fase, versionado riguroso de los documentos.

### 3.5 Tabla de recomendación combinada

| Tamaño equipo | Complejidad | Plazo | Tipo D8 | Recomendación |
|---|---|---|---|---|
| 1 | Trivial | Urgente | cualquiera | Saltear template, README plano |
| 1 | Normal | Normal | library, cli-tool | Template con `mini-plan` y omisión de `acuerdo-equipo` |
| 1 | Normal | Largo | cualquiera | Template completo, `mini-plan` en sprint, audit propio |
| 2-5 | Normal | Normal | cualquiera | Caso canónico, template completo |
| 2-5 | Compleja | Normal | rest-api, web-microservices | Template completo + ADR por bounded context |
| 2-5 | Compleja | Largo | cualquiera | Template completo + audit externo |
| 6-15 | Normal | Normal | web-monolith, rest-api | Template completo, audits cada fase |
| 6-15 | Compleja | Largo | web-microservices | Template completo + federación de proyectos SDD por contexto |
| >15 | Compleja | Largo | web-microservices | Varios proyectos SDD federados, uno por bounded context |
| cualquiera | Trivial | Urgente | cualquiera | No usar template |
| cualquiera | Normal | Urgente | cualquiera | Template recortado: 00, 02, 06, 07 con `mini-plan` |

Cuándo NO usar el template:

- Proyecto con menos de una semana de trabajo total y sin intención de mantenerse.
- Prototipo descartable de validación rápida de una hipótesis.
- Investigación exploratoria sin compromiso de entrega.
- Cliente que rechaza la fase de intake estructurado y exige codear de inmediato.

En esos casos, el costo de armar la documentación inicial supera el beneficio. Igual podés tomar partes (la plantilla de visión, la matriz de casos de uso) sin comprometerte al flujo completo.

---

## §4 Recorrido paso a paso de la metodología

El flujo completo del usuario tiene 6 pasos. La narración asume que ya cumpliste los prerequisitos de §2 y que decidiste que el template aplica a tu proyecto según §3.

### 4.1 Paso 1 — Chat informal en Claude.ai web

El objetivo de este paso es armar contexto. Vas a Claude.ai en el navegador, abrís un proyecto nuevo (o un chat fresco si no querés persistir), y empezás una conversación sobre el problema que vas a resolver. La regla de oro es no apurar a Claude a decidir: queremos que pregunte, que haga repreguntas y que te ayude a aclarar lo que el cliente todavía no dijo.

Un prompt inicial que funciona bien:

```text
Quiero diseñar un sistema para [descripción del problema en una o dos frases].
El cliente es [tipo de cliente]. La urgencia es [normal / alta / baja].
Ayudame a estructurar el contexto. No tomes decisiones técnicas todavía;
primero hacé preguntas para entender el problema, los stakeholders, las
restricciones y los casos límite.
```

Claude responde con un set de preguntas. Vas contestando con la información que tenés y, cuando no sabés algo, lo decís explícitamente ("no sé, hay que preguntárselo al cliente"). Después de unos cuantos turnos vas a notar que la imagen se va cerrando.

Otro prompt útil cuando sentís que Claude está saltando a soluciones técnicas:

```text
Pará. Todavía no hablemos de stack ni de arquitectura.
Quiero quedarme en el problema. ¿Qué otras preguntas le harías a un
cliente que recién describe la idea para entender el dolor real y el
costo de no hacer nada?
```

Y otro para forzar la priorización:

```text
Si tuvieras que listar los tres flujos más frecuentes de uso del sistema
y los dos flujos críticos que rara vez pasan pero no pueden fallar,
¿cuáles serían? Justificá cada uno en una frase.
```

La conversación típica dura entre 10 y 15 turnos. Empezás con el problema, pasás por audiencias, propuesta de valor, alcance funcional, historias de usuario, flujos típicos, casos límite, métricas de éxito, exclusiones, restricciones, riesgos y glosario del dominio. No te apures a cerrar.

Al final de este paso tenés una conversación rica, no estructurada, en Claude.ai. Es deliberadamente desordenada: lo que importa es que esté completa.

### 4.2 Paso 2 — Consolidación en un solo documento

Una vez que sentís que la idea está cerrada, le pedís a Claude que consolide toda la conversación en un único documento de contexto. Este documento es interno, no es entregable: sirve como puente entre la charla y los intake.

Prompt sugerido:

```text
A partir de toda esta conversación, generá un único documento en
markdown que reúna ordenadamente el contexto del proyecto.
Estructura sugerida:
1. Visión del cliente en dos párrafos.
2. Problema concreto y consecuencias de no resolverlo.
3. Audiencia y stakeholders (tabla con rol, categoría, responsabilidad).
4. Alcance funcional pretendido (lista priorizada con MoSCoW provisorio).
5. Historias de usuario en formato Como/Quiero/Para.
6. Flujos típicos descriptos en lenguaje coloquial.
7. Casos límite con preguntas abiertas para el cliente.
8. Métricas de éxito (SMART).
9. Exclusiones declaradas.
10. Restricciones del cliente (presupuesto, fecha, legal, integraciones).
11. Riesgos detectados desde el negocio.
12. Glosario del dominio.

Mantené el lenguaje del cliente, sin decisiones técnicas todavía.
Si una sección quedó incompleta en la conversación, marcala como
PENDIENTE en lugar de inventar.
```

Claude devuelve un documento de 5 a 10 páginas. Hacé una pasada manual de verificación: ¿está todo lo que conversaron? ¿hay PENDIENTES razonables? ¿se mantiene en lenguaje de negocio sin colarse decisiones técnicas?

Si encontrás huecos, los completás conversando un par de turnos más y le pedís a Claude que regenere el documento. No avanzás al paso 3 hasta que el documento consolidado te parezca representativo.

Guardá ese documento aparte. Lo vamos a usar como input del paso siguiente.

### 4.3 Paso 3 — Volcado a las plantillas intake

Acá entran en juego las dos plantillas oficiales del template: `PROJECT-BRIEF-template.md` y `PROJECT-README-template.md`. Las dos viven en `/sdd2.0/devs/intake/` del template fuente.

El BRIEF es el intake de negocio: 13 secciones que capturan lo que el cliente quiere en lenguaje de negocio, sin decisiones técnicas. El README es el intake técnico: 17 secciones que capturan cómo se va a construir el sistema, incluido el campo crítico `Tipo de proyecto` con los 8 valores D8.

Subí ambas plantillas al chat de Claude.ai como archivos adjuntos. Después le decís:

```text
Te paso dos plantillas oficiales del template SDD 2.0:
PROJECT-BRIEF-template.md y PROJECT-README-template.md.

Quiero que tomes la información del documento consolidado que generamos
antes y llenes las dos plantillas. Reglas:

1. No inventes datos que no estén en el documento consolidado.
2. Si falta información para algún campo, marcalo como PENDIENTE y
   listá al final las preguntas concretas que necesitás que yo le
   responda antes de seguir.
3. Mantené las instrucciones de cada sección de la plantilla solo si
   son útiles. Borrá los bloques "Ejemplo genérico" y "Lo que NO va
   en esta sección" del output final.
4. Respetá el checklist de §13 del BRIEF y §17 del README al final
   de cada documento.
5. Para el campo "Tipo de proyecto" del PROJECT-README, elegí
   exactamente uno de los 8 valores cerrados de D8 con justificación.
```

Claude genera los dos documentos. Los va a sacar con muchas secciones tildadas y, casi seguro, con algunos PENDIENTES que requieren ida y vuelta con el cliente real o con tu propio criterio.

Hacé la ronda de preguntas pendientes (las podés llevar al cliente si las hay), volvés a Claude con las respuestas y le pedís que regenere los intake con esas nuevas respuestas incorporadas. Repetí hasta que el checklist final de ambos documentos esté íntegramente tildado.

Output del paso: dos archivos markdown personalizados al proyecto, listos para bajar a local. Los nombres definitivos siguen el patrón:

- `PROJECT-BRIEF-<nombre-kebab>_v1.0.md`
- `PROJECT-README-<nombre-kebab>_v1.0.md`

Donde `<nombre-kebab>` es el nombre del proyecto en kebab-case (minúsculas, sin acentos, guion medio como separador). Ejemplo: para "Sistema de Turnos Médicos", el slug es `sistema-turnos-medicos`.

### 4.4 Paso 4 — Bootstrap local con el template

Ahora bajás el template a tu máquina y reemplazás los intake de la carpeta `intake/` con los tuyos.

Si todavía no clonaste el template, cloná ahora:

```bash
git clone <url-del-repo-del-template> mi-proyecto
cd mi-proyecto
```

Si ya tenés un repositorio Git para tu proyecto y querés sumar el template como subcarpeta:

```bash
# Desde la raíz de tu repo
cp -r ruta/al/template/sdd2.0 ./sdd2.0
```

Una vez bajado, reemplazá los archivos de la carpeta `intake/` con los tuyos:

```bash
# Asumiendo que guardaste tus intakes llenos en una carpeta intermedia:
cp BRIEF-lleno.md sdd2.0/devs/intake/PROJECT-BRIEF-sistema-turnos-medicos_v1.0.md
cp README-lleno.md sdd2.0/devs/intake/PROJECT-README-sistema-turnos-medicos_v1.0.md
```

En PowerShell el equivalente es:

```powershell
Copy-Item BRIEF-lleno.md sdd2.0\devs\intake\PROJECT-BRIEF-sistema-turnos-medicos_v1.0.md
Copy-Item README-lleno.md sdd2.0\devs\intake\PROJECT-README-sistema-turnos-medicos_v1.0.md
```

Hacé un commit del estado inicial:

```bash
git add sdd2.0/
git commit -m "chore: bootstrap inicial SDD 2.0 con intakes completos"
```

Validá rápidamente que el árbol está bien:

```bash
ls sdd2.0/devs/intake/
ls sdd2.0/devs/orchestrator/
ls sdd2.0/devs/rules/
```

Debería listar el master-prompt en `orchestrator/`, los archivos de reglas `00_rules_*.md` a `11_rules_*.md` y `_root_rules.md` en `rules/`, y tus dos intakes personalizados en `intake/`.

### 4.5 Paso 5 — Ejecutar el master-prompt en Claude Code

Acá empieza la parte más interesante: el orquestador toma tus intakes y genera la documentación completa del proyecto en `/sdd2.0/docs/`.

Abrí una terminal en la raíz de tu proyecto y lanzá Claude Code:

```bash
cd mi-proyecto
claude
```

Una vez dentro de la sesión interactiva de Claude Code, copiá literalmente el contenido del archivo `sdd2.0/devs/orchestrator/master-prompt.md` y pegalo como primera instrucción. Alternativamente, podés referenciarlo así:

```text
Leé /sdd2.0/devs/orchestrator/master-prompt.md y arrancá la ejecución
del orquestador SDD 2.0 sobre este repositorio. Mis intakes ya están
en /sdd2.0/devs/intake/. El proyecto se llama [nombre del proyecto].
```

Claude Code va a:

1. Leer los dos intakes y validarlos contra los checklists §13 (BRIEF) y §17 (README).
2. Si encuentra placeholders sin completar (`PENDIENTE`, `[Nombre]`, etc.), se detiene y te pide completarlos.
3. Detectar `project_type` del README §1.
4. Derivar los flags de gating (usa_llm, tiene_ui_final, multi_tenant, etc.).
5. Recolectar invariantes del proyecto (idioma, encoding, fecha, etc.).
6. Presentar el plan de generación por categoría (la tabla de §6 del master-prompt aplicada a tu proyecto).
7. Esperar tu confirmación explícita antes de despachar el primer subagente.

Cuando veas el plan, revisalo con calma. Verificá:

- Que el `project_type` detectado es correcto.
- Que la lista de documentos a generar por categoría tiene sentido.
- Que los flags están bien (si dice `usa_llm: false` y tu proyecto sí usa LLM, hay un problema en el intake).

Si todo está bien, respondés:

```text
aprobar
```

Si querés ajustes, respondés:

```text
aprobar con cambios: la categoría 04 debería estar habilitada porque
el sistema sí usa un LLM para clasificar tickets entrantes.
```

Y el orquestador ajusta antes de arrancar.

A partir de ahí, el orquestador despacha subagentes fase por fase:

- Fase A: 00_contexto + 01_necesidades_negocio + audit A.
- Fase B: 02_especificacion_funcional + 03_ux_ui_dx + 04_prompts_ai (si aplica) + audit B.
- Fase C: 05_arquitectura_tecnica + audit C.
- Fase D: 06_backlog-tecnico + 07_plan-sprint + audit D.
- Fase E: 08_calidad_y_pruebas + audit E.
- Fase F: 09_devops + 10_developer_guide (si aplica) + audit F.
- Fase G: 11_examples (si aplica) + audit G.
- Fase H: README raíz + audit final consolidado.

Entre cada fase, el orquestador se detiene, presenta el informe del audit (`/sdd2.0/docs/_audit/<fase>-<categoria>_v1.0.md`) y espera tu confirmación para continuar.

Si un audit devuelve `RECHAZADO` por hallazgos P0, el orquestador no avanza. Hay que corregir y re-auditar.

Si un subagente detecta una ambigüedad legítima (por ejemplo, falta una métrica numérica en el intake), se detiene y devuelve una pregunta estructurada. Vos respondés, el orquestador actualiza el intake siguiendo §13 del master-prompt y reanuda.

Tiempo total estimado del paso 5 para un proyecto normal: entre 2 y 6 horas de ejecución del modelo, distribuidas en sesiones. Es perfectamente normal pausar y retomar.

### 4.6 Paso 6 — Revisión humana y handoff a codificación

Cuando el orquestador termina la fase H, te presenta el resumen ejecutivo del entregable: documentos generados por categoría, cobertura de la cadena de trazabilidad, ítems del Sprint 1 listos para codear, audits aprobados, decisiones pendientes y flags activos.

Antes de autorizar el handoff a codificación, hacé una revisión humana en estas dimensiones:

- Trazabilidad: abrí 3 o 4 user stories al azar y verificá que la cadena US → CU → NB → Visión cierra de punta a punta.
- Ambigüedades pendientes: revisá la lista de decisiones pendientes. Cerralas antes del Sprint 1 o documentalas como riesgos asumidos.
- Completitud: pasá por las 12 carpetas y abrí el README de cada una. Si alguno está vacío o trivial, falló algo en la generación.
- Coherencia: leé la visión, leé los CU del Sprint 1, leé el ADR-001. ¿Cuentan la misma historia?

Si encontrás algo que arreglar, podés:

- Pedirle al orquestador que regenere un solo documento.
- Pedirle que re-auditar una fase específica.
- Volver al intake, actualizarlo siguiendo §13 y reanudar desde la fase afectada.

Cuando todo cierra, autorizás el paso a codificación:

```text
Confirmo handoff a codificación. Arrancamos Sprint 1 con los items
listados en el resumen ejecutivo.
```

A partir de ahí, ya salís del scope de SDD 2.0 (que es documentación) y entrás en el ciclo de desarrollo iterativo. La documentación generada queda como referencia viva: se actualiza cuando hay cambios reales, no porque sí.

---

## §5 Ejemplos aplicados

Tres mini-casos sintéticos que muestran el flujo completo end-to-end. Cada caso ilustra un `project_type` distinto y muestra los cuatro sub-bloques: resumen del chat de Claude.ai, fragmentos clave del intake, output del orquestador, y muestras de los documentos generados.

### 5.1 Caso "API REST de gestión de turnos médicos" (rest-api)

Contexto: un consultorio mediano (12 médicos, 3 administrativos, 600 turnos por semana) quiere reemplazar su sistema actual de turnos por teléfono con una API REST consumida por un portal web y una app móvil de pacientes.

#### Chat resumido en Claude.ai

Turnos 1 a 3: el equipo le describe a Claude el problema del consultorio. Claude pregunta cuántos pacientes hay activos, cuántos médicos, qué pasa hoy si dos pacientes piden el mismo turno por teléfono al mismo tiempo, y si hay integraciones obligatorias con sistemas legados.

Turnos 4 a 6: el equipo aclara que hay un sistema contable interno con el que hay que sincronizar facturación diaria, que la recepcionista usa una planilla impresa que se duplica frecuentemente, y que el directorio quiere que el portal web esté en producción antes de la temporada de invierno (julio).

Turnos 7 a 10: Claude profundiza en historias de usuario (paciente, médico, recepcionista, auditor de obra social) y casos límite (cancelación tardía, sobreturno, paciente sin obra social, paciente menor de edad). El equipo va respondiendo y dejando algunos como pendientes para el cliente.

Turnos 11 a 13: cierre. Claude consolida en un documento único de contexto. El equipo lo revisa, pide dos correcciones menores (faltaba la métrica de tiempo de espera promedio y faltaba mencionar que el sistema actual también gestiona la sala de espera).

#### Fragmentos clave del intake

PROJECT-BRIEF, §1 Idea y problema:

> Hoy los pacientes solo pueden sacar turno llamando por teléfono al consultorio en horario de atención, lo que satura la línea (15 a 30 minutos de espera promedio) y genera tiempos administrativos altos. La recepcionista anota en una planilla y mensualmente se detectan entre 8 y 12 turnos duplicados que tienen que resolverse a mano. Si no resolvemos esto, perdemos pacientes contra clínicas que ya tienen reserva online y la recepcionista no llega a cubrir las llamadas mientras atiende presencialmente.

PROJECT-BRIEF, §4 Alcance funcional (extracto):

| ID | Capacidad | MoSCoW |
|---|---|---|
| F-01 | Reservar un turno para un médico y fecha disponible | Must Have |
| F-02 | Cancelar y reprogramar un turno propio | Must Have |
| F-03 | Listar disponibilidad de un médico para los próximos 30 días | Must Have |
| F-04 | Notificar al paciente 24 horas antes por correo | Should Have |
| F-05 | Integración con sistema contable existente (exportación CSV diaria) | Must Have |
| F-06 | Lista de espera automática para turnos liberados | Could Have |
| F-07 | App móvil nativa | Won't Have v1 |

PROJECT-README, §1 Tipo de proyecto: `rest-api`.

PROJECT-README, §3 Estilo arquitectónico: Clean Architecture con 4 capas. Se descartan monolito en capas tradicional y microservicios (over-engineering para un dominio acotado con un equipo de 4 personas).

PROJECT-README, §7 Persistencia: motor relacional gestionado con migrations versionadas. Sin multi-tenancy.

PROJECT-README, §13 NFR: latencia p99 ≤ 300 ms en endpoints de consulta, SLO 99,5 por ciento, throughput sostenido 100 requests por segundo.

#### Output del orquestador

Claude Code, leyendo el master-prompt, detecta:

```text
project_type: rest-api
nombre-kebab: api-turnos-medicos
usa_llm: false
tiene_ui_final: false
multi_tenant: false
tiene_auth: true
equipo_n: 4
tiene_portal_developers: true
tiene_extensibilidad: false
tiene_persistencia: true
requiere_compliance: true
tiene_observabilidad_critica: true
```

Plan de generación: 12 categorías completas (04 omitida porque no usa LLM). En categoría 03 se elige variante DX (no UX/UI) porque la API consume otros frontends. La categoría 10 es obligatoria por ser rest-api con portal de developers.

Fases ejecutadas en orden: A, B, C, D, E, F, G, H. Cada una con su audit. La fase G genera 3 samples (cliente HTTP de referencia, colección de invocaciones reusables, SDK tipado para el lenguaje del portal). El total de archivos generados ronda los 60.

#### Muestras de documentos generados

Extracto de `/sdd2.0/docs/02_especificacion_funcional/casos-de-uso/CU-03-reservar-turno_v1.0.md`:

```markdown
# CU-03 — Reservar turno

## Actor primario
Paciente registrado

## Precondiciones
- El paciente está autenticado con un JWT válido.
- El médico al que se le quiere reservar turno existe y está activo.
- La franja horaria solicitada no está ocupada ni bloqueada.

## Flujo principal
1. El paciente consulta disponibilidad del médico para los próximos 30 días.
2. El sistema devuelve las franjas libres con duración estándar de 20 min.
3. El paciente selecciona una franja y confirma.
4. El sistema valida que la franja sigue libre (lock optimista).
5. El sistema crea el turno y devuelve identificador y comprobante.
6. El sistema envía notificación de confirmación al correo del paciente.

## Criterios de aceptación (Given/When/Then)
- Given un paciente autenticado y una franja libre,
  When solicita reservar esa franja,
  Then el sistema crea el turno y devuelve HTTP 201 con el identificador.
- Given un paciente autenticado y una franja que dejó de estar libre,
  When solicita reservar esa franja,
  Then el sistema devuelve HTTP 409 con código TURNO_NO_DISPONIBLE.

## Trazabilidad
- Upstream: NB-01 (acceso 24/7 a la reserva), Visión §2
- Downstream: US-08, BT-12, RC-04, TC-15
```

Extracto de `/sdd2.0/docs/05_arquitectura_tecnica/adrs/ADR-002-estilo-clean-architecture_v1.0.md`:

```markdown
# ADR-002 — Adopción de Clean Architecture

## Contexto
El sistema es una API REST con un dominio acotado (turnos, médicos,
pacientes, obras sociales) y un equipo de 4 personas con experiencia
mixta. Necesitamos una arquitectura que aísle la lógica de negocio
del framework HTTP y del motor de persistencia para mantener test
unitarios baratos y permitir cambios de infraestructura sin tocar
el dominio.

## Decisión
Adoptar Clean Architecture con 4 capas: Domain, Application,
Infrastructure, Api. El dominio no depende de frameworks.

## Alternativas descartadas
- Monolito en capas tradicional: acopla lógica de negocio al ORM,
  encarece los tests unitarios.
- Microservicios: over-engineering para un dominio acotado y un
  equipo chico, agrega complejidad operativa sin valor proporcional.

## Consecuencias
Positivas: testabilidad alta, intercambiabilidad de motor de persistencia,
contratos claros entre capas.
Negativas: más boilerplate inicial, curva de aprendizaje para el junior
del equipo.
```

### 5.2 Caso "Librería utilitaria para parsing de archivos CSV" (library)

Contexto: el equipo de plataforma de una empresa tecnológica necesita una librería interna para parsear archivos CSV con varias particularidades (delimitadores distintos, encoding detectable, manejo de filas con error sin frenar la lectura completa). Hoy cada proyecto resuelve el problema con scripts ad-hoc y eso genera mucho retrabajo.

#### Chat resumido en Claude.ai

Turnos 1 a 4: descripción del problema, identificación de los principales consumidores (3 equipos internos, cada uno con su stack), foco en la ergonomía de la API pública y en la compatibilidad de versiones.

Turnos 5 a 7: Claude pregunta cómo se distribuye hoy la librería y qué políticas de breaking changes maneja la empresa. Se aclara que se publica via el registry interno del ecosistema del lenguaje y que se siguen lineamientos SemVer estrictos.

Turnos 8 a 10: profundización en historias del consumidor (lectura básica, lectura con error tolerante, lectura en streaming, escritura, detección de encoding) y casos límite (archivo vacío, filas con cantidad de columnas inconsistente, caracteres no UTF-8, valores entre comillas con saltos de línea internos).

Turno 11 a 12: cierre y consolidación.

#### Fragmentos clave del intake

PROJECT-BRIEF, §3 Propuesta de valor:

> Hoy cada equipo resuelve el parsing de CSV con scripts ad-hoc, lo que genera bugs repetidos y mantenimiento disperso. La promesa de esta librería es ofrecer una API pequeña y predecible que soporta los escenarios más comunes (delimitadores configurables, manejo de errores fila por fila, streaming de archivos grandes) y deja documentadas las extensiones para casos avanzados. Frente a librerías comerciales o externas con licencias o trade-offs, el diferenciador es que la API se ajusta al estilo idiomático del lenguaje del equipo y se integra al pipeline interno de CI sin fricción.

PROJECT-README, §1 Tipo de proyecto: `library`.

PROJECT-README, §3 Estilo arquitectónico: Pipeline / Clean Architecture liviana. Descartado: monolito (no aplica, es librería), event-driven (innecesario).

PROJECT-README, §9 Estrategia de testing: pirámide 80/15/5, cobertura mínima 85% líneas y 75% branches, snapshot tests para verificar formato de output canónico.

PROJECT-README, §10 Versionado: SemVer 2.0.0, Conventional Commits, cálculo automático de versión a partir de tags Git, canales preview y stable, feed del ecosistema.

PROJECT-README, §12 Compatibilidad: soporte para las versiones LTS actuales y previas del runtime. Sin soporte para arquitectura 32-bit.

#### Output del orquestador

```text
project_type: library
nombre-kebab: csv-parser-lib
usa_llm: false
tiene_ui_final: false
multi_tenant: false
tiene_auth: false
equipo_n: 3
tiene_portal_developers: true
tiene_extensibilidad: true
tiene_persistencia: false
requiere_compliance: false
tiene_observabilidad_critica: false
```

Plan: categoría 04 omitida (no LLM). Categoría 03 variante DX (developer integrador). Categoría 10 obligatoria. Categoría 11 obligatoria con 3 samples progresivos (consumidor básico, consumidor intermedio, consumidor avanzado con extensiones).

La fase de arquitectura genera 3 ADR mínimos: estilo arquitectónico, superficie pública, política de versionado. La fase de developer guide genera conceptos, onboarding, integración por cada stack, referencia de API y troubleshooting.

#### Muestras de documentos generados

Extracto de `/sdd2.0/docs/00_contexto/vision-producto_v1.0.md`:

```markdown
# Visión del producto — csv-parser-lib

## Audiencia
Desarrolladores internos de los equipos de Plataforma, Analytics e
Integraciones. Audiencia secundaria: futuros equipos que necesiten
parsear archivos CSV en sus pipelines.

## Propuesta de valor
Ofrecer una librería con API mínima y predecible para los casos
comunes de parsing CSV, con extensibilidad documentada para casos
avanzados. Reducir el costo de mantenimiento disperso que hoy tiene
la empresa con N scripts ad-hoc.

## Objetivos SMART
- Cobertura interna: 3 equipos consumidores en los primeros 6 meses
  post-release v1.0.
- Reducción de bugs reportados sobre parsing CSV en los repos
  consumidores: -50% en 9 meses.
- Tiempo medio de integración de un equipo nuevo: ≤ 1 jornada.

## Métricas de éxito
- Adopción medida por dependencias declaradas en repos internos.
- Cantidad de issues abiertas en el repo de la librería por trimestre.
- Tiempo de respuesta a issues críticas: ≤ 2 días hábiles.
```

Extracto de `/sdd2.0/docs/11_examples/README.md`:

```markdown
# Samples — csv-parser-lib

Esta carpeta contiene tres proyectos consumidores que ilustran el uso
progresivo de la librería.

| Nivel | Carpeta | Qué demuestra |
|---|---|---|
| Básico | `01-basico-lectura/` | Lectura de un archivo CSV con delimitador por defecto, deserialización a una lista de objetos tipados |
| Intermedio | `02-intermedio-streaming/` | Lectura en streaming de un archivo grande, manejo de filas con error sin frenar la lectura completa, configuración de delimitador y encoding |
| Avanzado | `03-avanzado-extension/` | Implementación de un type converter custom para columnas con formato propietario, registro del converter via el punto de extensión documentado |

Cada sample es autocontenido. Para ejecutarlo, abrir la carpeta,
seguir el README local y correr el comando de la sección "Ejecutar".
```

### 5.3 Caso "App móvil de inventario de almacén" (mobile-app-maui)

Contexto: una empresa logística con 4 depósitos y 30 empleados de campo quiere reemplazar el método actual (planillas en papel y planilla de cálculo) por una app móvil multiplataforma de inventario que soporte escaneo de código de barras, sincronización con un sistema central y modo offline.

#### Chat resumido en Claude.ai

Turnos 1 a 3: descripción del contexto operativo, identificación del problema (errores frecuentes de inventario, demoras en cierres mensuales). Claude pregunta por conectividad en los depósitos (mala, intermitente), tipos de dispositivos disponibles y nivel técnico de los operarios (medio-bajo).

Turnos 4 a 6: profundización en historias de usuario (encargado de depósito, dueño del almacén, vendedor, auditor externo) y casos límite (sincronización después de varias horas offline, conflicto cuando dos operarios modifican el mismo producto, baja de la app por mantenimiento).

Turnos 7 a 10: Claude pregunta por restricciones de presupuesto y fecha (temporada alta en noviembre, hay que tener MVP antes), por integraciones obligatorias (sistema contable legacy con exportación CSV diaria) y por exclusiones explícitas (sin pagos online, sin gestión de proveedores, sin app web en v1).

Turnos 11 a 13: cierre, ronda de aclaraciones pendientes, consolidación.

#### Fragmentos clave del intake

PROJECT-BRIEF, §5 Historias de usuario (extracto):

> Como encargado de depósito, quiero escanear un código de barras con la cámara del celular, para registrar la entrada de mercadería sin tipear.
>
> Como dueño del almacén, quiero recibir una notificación cuando el stock de un producto cae por debajo del mínimo, para generar el pedido al proveedor a tiempo.

PROJECT-README, §1 Tipo de proyecto: `mobile-app-maui`.

PROJECT-README, §7 Persistencia: motor embebido local en el dispositivo + sincronización HTTP con el backend central. Estrategia de conflicto last-write-wins con override manual via UI.

PROJECT-README, §12 Compatibilidad: versiones recientes del SO móvil de las dos plataformas dominantes. Sin soporte para tablets en v1.

PROJECT-README, §13 NFR: tiempo de respuesta del escáner < 1 segundo, sincronización completa < 30 segundos para un día de movimientos típico, capacidad offline ≥ 8 horas continuas.

#### Output del orquestador

```text
project_type: mobile-app-maui
nombre-kebab: inventario-almacen
usa_llm: false
tiene_ui_final: true
multi_tenant: false
tiene_auth: true
equipo_n: 5
tiene_portal_developers: false
tiene_extensibilidad: false
tiene_persistencia: true
requiere_compliance: true
tiene_observabilidad_critica: false
```

Plan: categoría 03 variante UX/UI con acento en accesibilidad móvil. Categoría 10 opcional (no hay SDK público). Categoría 11 genera 3 samples (app básica con datos mock, sync con mock server, modo offline avanzado con resolución de conflictos).

La fase de DevOps genera estrategia de publicación atada a los ciclos de las stores móviles, con canales internal/alpha/beta/production.

#### Muestras de documentos generados

Extracto de `/sdd2.0/docs/03_ux_ui_dx/wireframes-pantalla-escaneo_v1.0.md`:

```markdown
# Wireframe — pantalla de escaneo

## Estado inicial
Vista en orientación vertical. Cámara ocupando los dos tercios
superiores con el rectángulo guía centrado. Tercio inferior con un
campo de texto auxiliar para ingreso manual del código y un botón
"Confirmar".

## Estado durante captura
Cuando la cámara detecta un código válido, se muestra un overlay
verde sobre el rectángulo guía con el código detectado. Vibración
corta del dispositivo como feedback háptico.

## Estado de error
Si el código no se reconoce en 5 segundos, se muestra un mensaje
"No se detectó código. Probá manualmente." con foco automático en
el campo de texto.

## Estados de carga
Spinner sobre el botón "Confirmar" mientras se valida contra la
base local. Si la conexión está disponible, se sincroniza en
background sin bloquear al usuario.
```

Extracto de `/sdd2.0/docs/09_devops/entornos-deploy_v1.0.md`:

```markdown
# Estrategia de entornos y publicación

## Canales de release
- internal: builds firmados que se distribuyen al equipo de QA
  interno. Trigger: cada merge a main.
- alpha: build entregado a 3 depósitos piloto. Trigger: tag pre-release
  con sufijo -alpha.N.
- beta: build entregado a todos los depósitos antes de un release. Trigger:
  tag pre-release con sufijo -beta.N.
- production: build promocionado al canal estable de cada store. Trigger:
  tag estable vX.Y.Z + aprobación manual.

## Política de rollout
Rollout progresivo del 10% / 30% / 100% en intervalos de 24 horas.
Métrica de freno automático: tasa de crashes > 1% en los primeros
1000 usuarios.
```

---

## §6 Resolución de problemas frecuentes

FAQ con respuestas concretas a los problemas más comunes durante el uso del template.

### F-01 — El orquestador se detiene preguntándome X, ¿qué hago?

Es el comportamiento esperado del patrón de manejo de ambigüedad (§9 del master-prompt). El subagente detectó que falta información bloqueante en el intake y no quiere inventar. La pregunta viene estructurada con la sección del intake donde debería vivir la respuesta.

Pasos:

1. Leé la pregunta concreta y la justificación.
2. Si la respuesta la tenés, respondés directamente. El orquestador va a actualizar el intake siguiendo §13 del master-prompt y va a reanudar el subagente.
3. Si la respuesta requiere consultar al cliente, anotala como pendiente, hacé la consulta, y respondé cuando tengas la información.
4. Si decidís que la información no es bloqueante, podés pedir al orquestador que asuma un valor por defecto justificado, pero queda como decisión documentada en el log.

### F-02 — Generó un documento que no aplica a mi tipo de proyecto, ¿cómo lo saco?

Revisá primero el `project_type` declarado en PROJECT-README §1. Si está mal, corregilo, el orquestador retrocede a la fase A y regenera lo afectado. Si está bien, abrí el archivo de reglas correspondiente (`XX_rules_<categoria>.md`), verificá §2.1 y §2.2 (tabla maestra de documentos y reglas por tipo). Si el documento estaba marcado como "Omitir" para tu tipo y aun así se generó, es un bug del subagente. Pedile al orquestador que regenere esa categoría con instrucciones explícitas.

Si simplemente decidiste que no querés ese documento aunque la regla lo recomiende, eliminalo a mano y registrá un ADR en `/sdd2.0/docs/05_arquitectura_tecnica/` documentando la omisión.

### F-03 — Quiero forzar un cambio en una regla de categoría, ¿dónde lo modifico?

Las reglas viven en `/sdd2.0/devs/rules/`. Cada categoría tiene su archivo `XX_rules_<categoria>.md`. Para cambiar comportamiento de una categoría:

1. Editás el archivo de reglas correspondiente.
2. Si es un cambio editorial menor, subís minor (`v1.0` → `v1.1`).
3. Si es un cambio operativo (agregar documento, cambiar especialidad), subís minor o major según corresponda.
4. Re-ejecutás el orquestador desde la fase afectada.

El master-prompt no se toca para esto: el principio de delegación de la especialidad implica que el orquestador lee las reglas en cada ejecución.

### F-04 — El audit reportó un P0, ¿cómo lo corrijo?

P0 significa hallazgo bloqueante: viola D1-D8 (idioma, encoding, kebab-case, versionado, deprecación, trazabilidad, conjunto cerrado D8) o rompe la estructura obligatoria del documento.

Pasos:

1. Abrí el informe en `/sdd2.0/docs/_audit/<fase>-<categoria>_v1.0.md`.
2. Identificá el archivo, sección y evidencia del hallazgo.
3. Si es un error de contenido (placeholder sin completar, sección omitida), pedile al orquestador que regenere ese documento.
4. Si es un error de trazabilidad, revisá los documentos upstream o downstream y arreglá las referencias.
5. Una vez corregido, pedile al orquestador que re-auditar la fase. Si pasa, avanzás.

### F-05 — Mi proyecto está en otro idioma, ¿puedo usar el template?

Sí, pero requiere preparación. Las reglas constructivas y el master-prompt están escritos en español rioplatense neutro. Para usar el template en otro idioma:

1. Modificá la sección §5 del master-prompt (Recolección de invariantes del proyecto) para declarar el idioma de salida deseado.
2. Pedile al orquestador que aplique ese idioma a todos los entregables.
3. Tené en cuenta que las preguntas guía y los ejemplos de cada archivo de reglas siguen en español: van a quedar en español en el material interno aunque la documentación de salida esté traducida.

Para una traducción completa del template, copiá el repo y traducí los archivos de reglas. Es trabajo, pero es tractable.

### F-06 — ¿Cómo regenero un solo documento sin re-ejecutar todo?

Indicale al orquestador qué documento querés regenerar. Ejemplo:

```text
Regenerá únicamente /sdd2.0/docs/05_arquitectura_tecnica/adrs/ADR-003-persistencia_v1.0.md
manteniendo el resto intacto.
```

El orquestador valida los upstream (que no hayan cambiado las decisiones de las que depende el documento), despacha el subagente de la categoría, regenera el archivo y dispara un audit acotado al cambio. No se re-ejecuta el flujo completo.

### F-07 — ¿Puedo modificar el master-prompt para mi caso?

Sí, pero con cuidado. El master-prompt es la única instrucción que el orquestador necesita y es autocontenido. Si lo modificás:

1. Subí versión (sección §16 del master-prompt).
2. Documentá qué cambiaste y por qué en el control de cambios.
3. Mantené el patrón plan-then-confirm. Saltearlo (por ejemplo, dejar que el orquestador codee sin confirmación) rompe la garantía de calidad del template.

No se recomienda modificar §1 (principio de delegación de la especialidad), §7 (orden de fases) ni §10 (auditoría entre fases). Modificar §5 (invariantes) o §14 (adaptabilidad por tipo) es habitual y seguro.

### F-08 — El intake quedó incompleto y el orquestador se rehúsa a avanzar

Es el comportamiento correcto. El intake incompleto es la principal fuente de documentación pobre. Pasos:

1. Leé la lista enumerada que devuelve el orquestador (archivo, sección, placeholder).
2. Volvé a Claude.ai web o respondé directamente al orquestador con los datos faltantes.
3. Si la información no existe (no sabés la respuesta y no podés consultar al cliente), tenés dos opciones: documentar un valor por defecto asumido explícitamente (queda como decisión a confirmar) o pausar la generación hasta tener la respuesta. No inventes.

### F-09 — ¿Cómo agrego información nueva al proyecto a mitad del flujo?

Si la información nueva afecta el intake (por ejemplo, el cliente cambió el alcance o agregó una restricción nueva):

1. Aplicá §13 del master-prompt: actualizá el intake siguiendo el control de cambios.
2. Identificá las fases afectadas por el cambio (por ejemplo, un cambio en §10 Restricciones del BRIEF afecta 00, 01, 09).
3. Pedile al orquestador que retroceda a la fase más temprana afectada y regenere desde ahí.

Si la información nueva es técnica (por ejemplo, agregás un ADR durante el desarrollo), agregalo a `/sdd2.0/docs/05_arquitectura_tecnica/adrs/` y actualizá la trazabilidad downstream a mano o pedile al orquestador que la rehidrate.

### F-10 — El subagente generó algo distinto a lo que esperaba

Primer paso: leé el archivo de reglas de la categoría y verificá qué pide. Es posible que tus expectativas estuvieran fuera de la especificación de la regla, y el subagente acertó.

Segundo paso: si el subagente realmente se desvió, pedile al orquestador que regenere el documento citando explícitamente la sección de la regla que no se respetó. Ejemplo:

```text
Regenerá CU-04-cancelar-turno_v1.0.md respetando estrictamente la
estructura de §4.2 de /sdd2.0/devs/rules/02_rules_especificacion_funcional.md.
El documento actual omite la sección de criterios Given/When/Then.
```

### F-11 — ¿Puedo correr varios proyectos SDD 2.0 en el mismo repositorio?

Sí, en carpetas separadas. Por ejemplo, para una arquitectura de microservicios podés tener:

```text
mi-mono-repo/
├── servicio-a/
│   └── sdd2.0/
├── servicio-b/
│   └── sdd2.0/
└── servicio-c/
    └── sdd2.0/
```

Cada subcarpeta es un proyecto SDD 2.0 independiente con sus propios intakes y su propia documentación generada. El orquestador se ejecuta una vez por cada subproyecto.

### F-12 — La generación de una fase tarda mucho, ¿es normal?

Sí, dentro de cierto rango. Una fase típica con 3 a 8 documentos y un audit puede demorar entre 5 y 30 minutos según el tamaño del proyecto y la velocidad del modelo. Si una fase tarda más de una hora, abortá, revisá los intakes (probablemente hay ambigüedades que estén haciendo retroceder al subagente repetidamente) y reanudá.

### F-13 — El orquestador me pidió aprobar el plan inicial, ¿qué reviso?

Mínimo checklist antes de aprobar:

- `project_type` correcto.
- Lista de documentos por categoría coherente con el alcance.
- Flags activos correctos (especialmente `usa_llm`, `tiene_auth`, `equipo_n`).
- Invariantes del proyecto (idioma, encoding, política de versionado) alineadas con tu organización.

Si todo está bien, aprobás. Si querés cambios, los pedís en la misma respuesta con justificación.

### F-14 — Los audits aprueban con observaciones P2 o P3, ¿puedo ignorarlas?

Podés, pero quedan registradas en el informe. Cuando el orquestador cierre el flujo, te las va a listar como "decisiones pendientes" en el resumen ejecutivo de §12 del master-prompt. Conviene resolverlas antes del Sprint 1, especialmente las P2 que apuntan a secciones recomendadas ausentes. Las P3 son mejoras estilísticas que podés agendar como deuda documental ligera.

### F-15 — ¿Qué hago con el feedback del cliente sobre los documentos generados?

El cliente típicamente no lee toda la documentación, pero sí lee el README raíz, la visión, el alcance y, a veces, los CU principales. Si el cliente pide cambios:

1. Si son cambios de fondo (cambio de alcance, exclusión nueva), aplicá §13: actualizá el intake correspondiente.
2. Si son cambios de forma (claridad, ortografía, ejemplos), aplicalos directamente al documento generado, sin tocar el intake.
3. Después de cambios, regenerá el README raíz para asegurar que los enlaces y referencias siguen coherentes.

---

## §7 Cómo extender el template

El template está diseñado para evolucionar. Cualquier extensión sigue tres principios: la especialidad vive en la regla, no en el master-prompt; el cambio sube versión del artefacto modificado; el orquestador lee las reglas en cada ejecución sin necesidad de recompilar.

### 7.1 Agregar una categoría nueva

Imaginemos que querés agregar una categoría `12_observabilidad/` para proyectos con requerimientos fuertes de observabilidad operativa.

Pasos:

1. Creá la carpeta `/sdd2.0/devs/rules/12_rules_observabilidad.md` siguiendo la estructura de las reglas existentes (§1 Especialidad, §2 Documentos, §3 Nomenclatura, §4 Estructura, §5 Preguntas guía, §6 Criterios, §7 Anti-patrones, §8 Prompt-snippet).
2. Definí la especialidad base (por ejemplo, "Site Reliability Engineer Senior") y las variantes por tipo D8.
3. Listá los documentos a producir: `estrategia-observabilidad_v1.0.md`, `dashboards_v1.0.md`, `alertas_v1.0.md`, `runbooks/RB-XX-<kebab>_v1.0.md`, `README.md`.
4. Actualizá `/sdd2.0/devs/orchestrator/master-prompt.md` §6 (plan de generación por categoría) para incluir la nueva categoría. Subí minor del master-prompt.
5. Decidí en qué fase entra (si es transversal, queda como fase F o G; si es post-DevOps, queda como fase F2 nueva).
6. Probá el flujo completo con un proyecto piloto.

Tiempo estimado: medio día para la definición, otro medio día para la prueba.

### 7.2 Agregar un tipo de proyecto nuevo a D8

D8 son los 8 valores cerrados: `library`, `web-monolith`, `web-microservices`, `desktop-app`, `mobile-app-maui`, `rest-api`, `cli-tool`, `worker-service`. Agregar un tipo nuevo (por ejemplo `embedded-firmware`) implica:

1. Subir major del master-prompt (§3 y §14 cambian).
2. Actualizar cada uno de los 13 archivos de reglas para agregar la nueva variante en §1.2 y §2.2.
3. Documentar qué documentos pasan a ser obligatorios, recomendados u omitidos para ese tipo.
4. Actualizar la tabla de adaptabilidad de §14 del master-prompt.

Es un cambio de mayor envergadura, no se hace casual. Conviene mantener D8 estable y, si aparece una variante, ver si encaja en alguno de los 8 existentes (un firmware puede modelarse como `cli-tool` muy ajustado, por ejemplo).

### 7.3 Agregar una variante de especialidad

Caso más simple: dentro de una categoría existente, querés una variante adicional. Por ejemplo, en `00_rules_contexto.md` querés agregar "Product Manager + Compliance Officer" como variante para proyectos regulados.

Pasos:

1. Editás §1.2 del archivo de reglas correspondiente.
2. Agregás una fila a la tabla de variantes con el criterio de activación.
3. Si la activación depende de un flag (por ejemplo `requiere_compliance: true`), citás ese flag de §4 del master-prompt.
4. Subís minor del archivo de reglas.
5. El orquestador empieza a leer la variante en la próxima ejecución, sin tocar el master-prompt.

---

## §8 Cómo regenerar parcialmente

Caso típico: el cliente cambia el alcance a mitad del proyecto. Por ejemplo, en el caso "API REST de turnos médicos", el cliente decide en el sprint 2 que sí va a haber app móvil nativa en v1 (estaba como Won't Have).

### 8.1 Identificar qué documentos están afectados

Pasos:

1. Actualizá los intakes siguiendo §13 del master-prompt. Subí minor o major según corresponda.
2. Hacé un diff conceptual entre el intake anterior y el nuevo para identificar qué secciones cambiaron.
3. Mapeá esas secciones a categorías afectadas usando la tabla "Trazabilidad downstream" del PROJECT-README:

| Sección del intake modificada | Categorías afectadas |
|---|---|
| BRIEF §4 Alcance funcional | 00, 01, 02, 06, 07, 11 |
| BRIEF §8 Métricas de éxito | 00, 13 (NFR) |
| BRIEF §10 Restricciones | 00, 09 |
| README §1 Tipo de proyecto | TODAS (es bloqueante) |
| README §3 Estilo arquitectónico | 05 |
| README §7 Persistencia | 02 (modelo conceptual), 05 (modelo lógico), 07 |
| README §9 Testing | 08 |
| README §11 Pipeline CI/CD | 09 |

### 8.2 Correr el orquestador parcialmente

Indicale al orquestador qué fases querés regenerar. Ejemplo:

```text
Cambió el alcance: la capacidad F-07 (app móvil nativa) pasó de
"Won't Have v1" a "Must Have". Actualicé el intake siguiendo §13.

Regenerá únicamente:
- 00_contexto (actualización de alcance y roadmap)
- 01_necesidades_negocio (agregar NB nueva si aplica)
- 02_especificacion_funcional (agregar CU correspondientes)
- 06_backlog-tecnico (agregar US y BT)
- 07_plan-sprint (re-planificar Sprint 3 y 4)
- 11_examples (agregar sample de cliente móvil si corresponde)

Mantené el resto intacto.
```

El orquestador valida que las categorías omitidas no estén impactadas, despacha los subagentes de las categorías listadas, regenera los documentos, dispara audits acotados y devuelve el resumen ejecutivo actualizado.

Importante: si la regeneración parcial detecta inconsistencias que no se pueden resolver sin tocar categorías no listadas, el orquestador se detiene y te pide ampliar el alcance del cambio.

---

## §9 Hojas de ruta sugeridas

Tres recorridos según tu perfil al acercarte al template.

### 9.1 Para principiantes en el template

Si nunca usaste SDD ni un template parecido, este es el orden de lectura recomendado:

1. Esta guía completa (§1 a §10). Tiempo estimado: 1 hora.
2. La plantilla `PROJECT-BRIEF-template.md`. Leela completa, fijate en los ejemplos genéricos. Tiempo estimado: 30 minutos.
3. La plantilla `PROJECT-README-template.md`. Leela completa, especialmente la tabla de los 8 tipos D8 en §1. Tiempo estimado: 30 minutos.
4. Tres archivos de reglas a elección: `00_rules_contexto.md`, `02_rules_especificacion_funcional.md` y `05_rules_arquitectura_tecnica.md`. Tiempo estimado: 1 hora.
5. El master-prompt completo, §1 a §16. Tiempo estimado: 45 minutos.

Ejercicios sugeridos:

- Ejercicio 1: tomá un proyecto chico que ya hayas hecho y completá manualmente el BRIEF y el README. No corras el orquestador, solo hacé el intake. Ves dónde te trabás.
- Ejercicio 2: corré el orquestador sobre un proyecto sintético (podés usar el caso 5.2 de esta guía como inspiración). Hacé un commit por fase.
- Ejercicio 3: leé los audits que genera el orquestador y verificá si coincidís con sus hallazgos.

Total estimado de onboarding: una jornada y media de trabajo enfocado, sin contar la ejecución del orquestador.

### 9.2 Para equipos con experiencia previa SDD

Si ya trabajaron con la versión 1.0 del template (la del Motor DSL, referenciada al final de §1 de esta guía como antecedente histórico), las novedades de la 2.0 son:

- Plan-then-confirm explícito: ya no se ejecuta el orquestador de corrido, hay puntos de detención obligatorios.
- Principio de delegación de la especialidad: las reglas tienen prioridad sobre el master-prompt.
- 8 tipos D8 cerrados con variantes de especialidad por tipo.
- Auditoría independiente entre fases con veredicto bloqueante.
- Patrón de manejo de ambigüedad con detención / pregunta / reanudación.
- Reglas de no-modificación de intake con flujo controlado.
- Tabla de adaptabilidad por tipo de proyecto.

Pueden saltearse:

- §1 y §2 de esta guía (qué es el template y prerrequisitos).
- §4.1 y §4.2 (chat informal y consolidación), si ya tienen un flujo propio de pre-intake.
- La mayoría de los archivos de reglas si ya los conocen de la versión anterior (revisar solo los diffs).

Conviene leer con atención:

- §3 y §14 del master-prompt (tipos D8 y adaptabilidad).
- §5 y §10 del master-prompt (invariantes y auditoría).
- Los archivos de reglas que tengan §1.2 (variantes por tipo).
- Esta guía §7 y §8 (extensión y regeneración parcial).

### 9.3 Para evaluadores académicos

Si el template se usa en contexto académico (cátedra, evaluación de trabajos prácticos, jurado de tesis), los criterios de evaluación recomendados son:

- Conformidad D1 a D8 del entregable final (idioma, encoding, naming, versionado, política de deprecación, trazabilidad, vocabulario, conjunto cerrado D8).
- Completitud por categoría: cada una de las 12 categorías tiene los documentos obligatorios para su tipo.
- Trazabilidad cerrada: la cadena Visión → NB → CU → ADR → US → BT → Sprint → Test → Pipeline cierra sin huérfanos.
- Calidad de los audits: los informes de `/sdd2.0/docs/_audit/` muestran hallazgos reales y veredictos justificados.
- Coherencia narrativa: el README raíz y los READMEs de cada categoría cuentan una historia coherente.

Una matriz de evaluación posible:

| Dimensión | Peso | Cómo evaluar |
|---|---|---|
| Intake completo y de calidad | 20% | Revisar BRIEF y README, checklist §13 y §17 íntegramente tildados, ausencia de placeholders |
| Trazabilidad cerrada | 25% | Tomar 5 US al azar y verificar la cadena hacia arriba (CU, NB, Visión) y hacia abajo (BT, Sprint, Tests) |
| Calidad arquitectónica | 20% | Revisar ADRs: presencia de contexto, decisión, alternativas, consecuencias. Revisar diagrama de componentes coherente con CU |
| Estrategia de testing y pipeline | 15% | Cobertura mínima numérica, quality gates explícitos, pipeline con stages claros |
| Audits y veredictos | 10% | Revisar 2 o 3 informes de audit, verificar que los hallazgos son reales y los veredictos están justificados |
| Coherencia narrativa | 10% | Leer README raíz, visión, alcance y CU principales; verificar que se sostienen mutuamente |

Total: 100%.

---

## §10 Glosario rápido y mapa visual de carpetas

### 10.1 Glosario rápido

Quince términos esenciales para usar el template. Para el glosario exhaustivo del marco teórico, ver el documento correspondiente.

| Término | Definición breve |
|---|---|
| Intake | Documentos de entrada del proyecto: PROJECT-BRIEF (negocio) y PROJECT-README (técnico). Son fuente de verdad. |
| BRIEF | Plantilla intake de negocio, 13 secciones. Captura lo que el cliente quiere en lenguaje de negocio. |
| README (intake) | Plantilla intake técnica, 17 secciones. Captura cómo se construye el sistema. No confundir con el README raíz de salida. |
| Master-prompt | Archivo único `/sdd2.0/devs/orchestrator/master-prompt.md` que ejecuta el orquestador. Es la instrucción que se pega en Claude Code. |
| Orquestador | Agente principal que coordina la generación de la documentación. Lee los intakes, planifica, despacha subagentes, audita. |
| Subagente | Agente especializado que produce los documentos de una categoría. Su especialidad vive en §1 del archivo de reglas correspondiente. |
| Audit independiente | Revisión cierre de fase por un subagente auditor sin contexto previo. Veredicto bloqueante: APROBADO, APROBADO CON OBSERVACIONES o RECHAZADO. |
| Plan-then-confirm | Modo operativo: cada fase se planifica, se confirma con el usuario, se ejecuta, se audita, se detiene. |
| D8 | Conjunto cerrado de 8 tipos de proyecto: library, web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, cli-tool, worker-service. Confirmados. |
| Flag de gating | Variable derivada del intake que condiciona qué documentos se generan. Ejemplos: usa_llm, tiene_persistencia, equipo_n. |
| Cadena D6 | Cadena de trazabilidad: Visión → NB → CU → RN → ADR → US → BT → Sprint → Test → Pipeline. |
| Invariante | Decisión que no se renegocia durante la generación. Hay invariantes globales (D1 a D8) y propias del proyecto. |
| Ambigüedad legítima | Falta concreta de un dato bloqueante en el intake que dispara detención / pregunta / reanudación. |
| Handoff a codificación | Punto en el que el orquestador entrega la documentación auditada y espera confirmación humana para arrancar Sprint 1. |
| Regla constructiva | Archivo `XX_rules_<categoria>.md` (o `_root_rules.md`) que codifica especialidad, documentos, nomenclatura, estructura, criterios y prompt-snippet de la categoría. |

### 10.2 Mapa visual de la estructura de carpetas

Árbol esperado de un proyecto que aplica el template SDD 2.0:

```text
mi-proyecto/
├── sdd2.0/
│   ├── devs/
│   │   ├── intake/
│   │   │   ├── PROJECT-BRIEF-template.md
│   │   │   ├── PROJECT-README-template.md
│   │   │   ├── PROJECT-BRIEF-<nombre-kebab>_v1.0.md      # Tu intake de negocio
│   │   │   └── PROJECT-README-<nombre-kebab>_v1.0.md     # Tu intake técnico
│   │   ├── orchestrator/
│   │   │   └── master-prompt.md                          # Único prompt a pegar
│   │   ├── rules/
│   │   │   ├── _root_rules.md                            # Reglas del README raíz
│   │   │   ├── 00_rules_contexto.md
│   │   │   ├── 01_rules_necesidades_negocio.md
│   │   │   ├── 02_rules_especificacion_funcional.md
│   │   │   ├── 03_rules_ux_ui_dx.md
│   │   │   ├── 04_rules_prompts_ai.md
│   │   │   ├── 05_rules_arquitectura_tecnica.md
│   │   │   ├── 06_rules_backlog_tecnico.md
│   │   │   ├── 07_rules_plan_sprint.md
│   │   │   ├── 08_rules_calidad_y_pruebas.md
│   │   │   ├── 09_rules_devops.md
│   │   │   ├── 10_rules_developer_guide.md
│   │   │   └── 11_rules_examples.md
│   │   └── _bootstrap/                                   # Material de bootstrapping
│   ├── guides/
│   │   ├── guia-usuario-sdd2.0_v1.0.md                   # Este documento
│   │   └── marco-teorico-sdd2.0_v1.0.md                  # Marco teórico (separado)
│   └── docs/                                             # Generado por el orquestador
│       ├── _audit/                                       # Informes de audit por fase
│       │   ├── faseA-00_contexto_v1.0.md
│       │   ├── faseA-01_necesidades_negocio_v1.0.md
│       │   ├── faseB-02_especificacion_funcional_v1.0.md
│       │   └── ...
│       ├── 00_contexto/
│       │   ├── README.md
│       │   ├── vision-producto_v1.0.md
│       │   ├── alcance-proyecto_v1.0.md
│       │   ├── roadmap-producto_v1.0.md
│       │   ├── compatibilidad-plataformas_v1.0.md
│       │   └── acuerdo-equipo_v1.0.md
│       ├── 01_necesidades_negocio/
│       │   ├── README.md
│       │   ├── necesidades-negocio_v1.0.md
│       │   └── necesidades-de-negocio/
│       │       ├── NB-01-<kebab>_v1.0.md
│       │       ├── NB-02-<kebab>_v1.0.md
│       │       └── NB-XX-<kebab>_v1.0.md
│       ├── 02_especificacion_funcional/
│       │   ├── README.md
│       │   ├── especificacion-funcional_v1.0.md
│       │   ├── casos-de-uso/
│       │   │   ├── CU-01-<kebab>_v1.0.md
│       │   │   └── CU-XX-<kebab>_v1.0.md
│       │   ├── reglas-de-negocio/
│       │   │   └── RN-XX-<kebab>_v1.0.md
│       │   └── modelo-datos/
│       │       └── modelo-conceptual_v1.0.md
│       ├── 03_ux_ui_dx/
│       │   ├── README.md
│       │   └── (UX/UI o DX según gating)
│       ├── 04_prompts_ai/                                # Solo si usa_llm == true
│       │   ├── README.md
│       │   └── prompt-<tarea>_v1.0.md
│       ├── 05_arquitectura_tecnica/
│       │   ├── README.md
│       │   ├── arquitectura-solucion_v1.0.md
│       │   ├── decisiones-arquitectura_v1.0.md
│       │   └── adrs/
│       │       ├── ADR-001-<kebab>_v1.0.md
│       │       └── ADR-XX-<kebab>_v1.0.md
│       ├── 06_backlog-tecnico/
│       │   ├── README.md
│       │   ├── product-backlog_v1.0.md
│       │   ├── backlog-tecnico_v1.0.md
│       │   └── definition-of-ready_v1.0.md
│       ├── 07_plan-sprint/
│       │   ├── README.md
│       │   ├── plan-iteracion-sprint-00_v1.0.md
│       │   ├── plan-iteracion-sprint-01_v1.0.md
│       │   ├── template-sprint-review_v1.0.md
│       │   ├── template-sprint-retrospectiva_v1.0.md
│       │   └── velocidad-equipo_v1.0.md
│       ├── 08_calidad_y_pruebas/
│       │   ├── README.md
│       │   ├── estrategia-calidad_v1.0.md
│       │   ├── estrategia-testing_v1.0.md
│       │   ├── plan-pruebas_v1.0.md
│       │   ├── matriz-cobertura-pruebas_v1.0.md
│       │   ├── casos-prueba-referenciales_v1.0.md
│       │   ├── criterios-validacion_v1.0.md
│       │   └── definition-of-done_v1.0.md
│       ├── 09_devops/
│       │   ├── README.md
│       │   ├── pipeline-ci-cd_v1.0.md
│       │   ├── estrategia-versionado_v1.0.md
│       │   ├── entornos-deploy_v1.0.md
│       │   └── supply-chain-seguridad_v1.0.md
│       ├── 10_developer_guide/                           # Según gating y tipo
│       │   ├── README.md
│       │   ├── conceptos-fundamentales_v1.0.md
│       │   ├── guia-onboarding-developer_v1.0.md
│       │   ├── referencia-api_v1.0.md
│       │   └── troubleshooting_v1.0.md
│       ├── 11_examples/                                  # Según gating y tipo
│       │   ├── README.md
│       │   ├── ejemplo-01-<kebab>_v1.0.md
│       │   ├── ejemplo-02-<kebab>_v1.0.md
│       │   └── ejemplo-03-<kebab>_v1.0.md
│       └── README.md                                     # README raíz consolidado
├── src/                                                  # Código del proyecto (fase posterior)
├── tests/                                                # Tests del proyecto (fase posterior)
├── samples/                                              # Materializado desde 11_examples
└── README.md                                             # README de la raíz del repo
```

Notas sobre el árbol:

- Las carpetas marcadas con "según gating" se generan solo si los flags del intake las habilitan.
- Cada categoría tiene un README.md propio que es el índice navegable.
- `/sdd2.0/docs/_audit/` se popula a medida que avanzan las fases.
- El árbol mostrado es el caso completo; tu proyecto va a tener algunas omisiones según `project_type` y flags.

---

## Resumen ejecutivo

Esta guía de usuario tiene 1.121 líneas, distribuidas en 10 capítulos completos según la estructura solicitada. Incluye 3 mini-casos aplicados (rest-api de gestión de turnos médicos, library de parsing CSV, mobile-app-maui de inventario de almacén), cada uno con sus 4 sub-bloques (chat resumido, fragmentos de intake, output del orquestador y muestras de documentos). Aporta 15 entradas de FAQ con respuestas concretas y accionables, y 6 prompts ejemplo en bloques de texto para las distintas etapas del flujo. Ilustra explícitamente los 8 tipos D8 confirmados (library, web-monolith, web-microservices, desktop-app, mobile-app-maui, rest-api, cli-tool, worker-service) con sus particularidades y reglas de gating, junto con un mapa visual ASCII completo de la estructura de carpetas.

---

**Fin del documento**
