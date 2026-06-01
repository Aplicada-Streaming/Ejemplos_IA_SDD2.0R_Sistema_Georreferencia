# Acuerdo de Equipo

**Proyecto:** GeoVial
**Documento:** acuerdo-equipo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-05-31
**Autor:** Product Manager Senior (AG-00) + Analista de Negocio Senior (AG-01)
**Trazabilidad upstream:** PROJECT-README §4, §10, §11; PROJECT-BRIEF §2
**Trazabilidad downstream:** 06_backlog-tecnico, 07_plan-sprint, 08_calidad_y_pruebas, 09_devops

## 1. Propósito

Este documento fija las convenciones de trabajo del equipo de cuatro personas que construye GeoVial: roles, ceremonias, acuerdos operativos, estrategia de ramas, convenciones de commits, herramientas y las referencias a la Definition of Done y a la Definition of Ready. Es obligatorio por tratarse de un equipo de más de dos personas. Los acuerdos se redactan como reglas operativas verificables, no como aspiraciones.

## 2. Equipo y roles

Se adopta Scrum con vertical slicing. El equipo es de cuatro personas. Los roles funcionales del organismo (jefe general, jefe de área) participan como interlocutores de negocio; los roles de construcción son del equipo de desarrollo.

| Rol | Responsabilidad principal |
|---|---|
| Product Owner | Representa al organismo ante el equipo; prioriza el backlog según el valor de negocio; valida los incrementos al cierre de cada sprint |
| Scrum Master / facilitador | Facilita las ceremonias, remueve impedimentos y vela por el cumplimiento de los acuerdos de este documento |
| Equipo de desarrollo (4 personas) | Construye y prueba los incrementos; cubre backend, front web, app móvil y librerías según el slice de cada sprint |
| Interlocutor de negocio (jefe general / jefe de área) | Aporta requisitos, valida la usabilidad y revisa los incrementos entregados |

Dado el tamaño del equipo, los roles de Product Owner y Scrum Master pueden recaer sobre integrantes que también desarrollan; la responsabilidad queda igualmente asignada de forma nominal por sprint.

## 3. Cadencia de ceremonias

| Ceremonia | Cuándo | Duración | Participantes | Notas |
|---|---|---|---|---|
| Planificación de sprint | Primer día del sprint | Hasta 2 h | Equipo completo + Product Owner | Se selecciona el slice del sprint según prioridad del backlog y la Definition of Ready |
| Daily | Cada día laboral | 15 min | Equipo de desarrollo | Qué se hizo, qué se hará, impedimentos |
| Revisión de sprint | Último día del sprint | Hasta 1 h | Equipo + Product Owner + interlocutor de negocio | Demostración del incremento funcional contra el criterio de transición de la fase |
| Retrospectiva | Último día del sprint, tras la revisión | Hasta 1 h | Equipo de desarrollo + Scrum Master | Acuerdos de mejora con responsable y seguimiento en la siguiente retro |
| Refinamiento del backlog | A mitad del sprint | Hasta 1 h | Equipo + Product Owner | Prepara ítems para que cumplan la Definition of Ready del próximo sprint |

La duración de los sprints se confirma en la categoría 07; se asume cadencia fija para todo el proyecto.

## 4. Acuerdos de trabajo

- Estrategia de ramas: GitHub Flow. La rama principal está protegida; todo cambio entra por una rama de feature mediante Pull Request. No se hace push directo a la rama principal.
- Code review: todo Pull Request requiere al menos una aprobación de otro integrante antes de fusionar. No se fusiona con la integración continua en rojo.
- Convenciones de commits: Conventional Commits sin excepciones. El versionado sigue SemVer 2.0.0; cualquier cambio que rompa la API pública de la librería de sincronización incrementa la versión mayor.
- Quality gates: la integración continua es bloqueante. La compilación debe pasar sin warnings tratados como error; la cobertura mínima exigida es de líneas igual o superior al 80% y de ramas igual o superior al 70%. Un Pull Request que no cumple los gates no se fusiona.
- Comunicación: el canal principal del equipo es el repositorio (issues y Pull Requests) para todo lo relativo al trabajo; las decisiones que afecten alcance o arquitectura se registran como documento o ADR, no solo en mensajería.
- SLA de respuesta: una solicitud de revisión de Pull Request se atiende dentro del mismo día laboral. Un impedimento bloqueante levantado en la daily se aborda el mismo día. Un comentario de revisión que no bloquea se responde antes del cierre del sprint.
- Documentación: cada incremento que cambia comportamiento actualiza la documentación correspondiente (categorías SDD afectadas) dentro del mismo Pull Request que introduce el cambio.
- Secretos: el archivo de variables de entorno queda fuera del control de versiones (con su plantilla de ejemplo versionada); los secretos de integración continua se gestionan en el almacén de secretos de la plataforma con rotación a 90 días.

## 5. Definition of Done

La Definition of Done detallada se define y mantiene en la categoría 08 (calidad y pruebas). A nivel de acuerdo de equipo se exige como mínimo: el incremento compila sin warnings tratados como error, pasa los gates de cobertura (líneas igual o superior al 80%, ramas igual o superior al 70%), tiene su Pull Request aprobado por al menos un par, su documentación actualizada y la demostración validada en la revisión de sprint contra el criterio de transición de la fase. Referencia: 08_calidad_y_pruebas.

## 6. Definition of Ready

La Definition of Ready detallada se define y mantiene en la categoría 06 (backlog técnico). A nivel de acuerdo de equipo, un ítem está listo para entrar a un sprint cuando tiene su valor de negocio claro, sus criterios de aceptación enunciados, sus dependencias identificadas y un tamaño estimado por el equipo. Referencia: 06_backlog-tecnico.

## 7. Herramientas

| Herramienta | Uso |
|---|---|
| GitHub | Alojamiento del monorepo, gestión de issues y Pull Requests, canal principal de trabajo |
| GitHub Actions | Integración continua y entrega continua; ejecución de los quality gates bloqueantes |
| GitHub Packages | Publicación de la librería de sincronización con canales de prelanzamiento y estable |
| GitHub Secrets | Gestión de secretos de la integración continua con rotación a 90 días |

## 8. Trazabilidad

- Upstream: PROJECT-README §4 (Scrum con vertical slicing y fases de delivery), §10 (GitHub Flow, Conventional Commits, SemVer, publicación en GitHub Packages), §11 (GitHub Actions, quality gates bloqueantes); PROJECT-BRIEF §2 (equipo de desarrollo e interlocutores de negocio).
- Downstream:
  - 06_backlog-tecnico: provee la Definition of Ready referenciada en §6.
  - 07_plan-sprint: aplica la cadencia de ceremonias (§3) y la planificación por slice.
  - 08_calidad_y_pruebas: provee la Definition of Done referenciada en §5 y los gates de cobertura.
  - 09_devops: implementa los gates bloqueantes (§4) y la gestión de secretos en la integración continua.
