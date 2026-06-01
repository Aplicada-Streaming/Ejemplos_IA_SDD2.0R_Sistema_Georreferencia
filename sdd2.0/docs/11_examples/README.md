# Ejemplos ejecutables — GeoVial

**Proyecto:** GeoVial
**Documento:** README.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Developer Advocate / Sample Engineer Senior (AG-11), Equipo SDD 2.0

---

## 1. Propósito de la carpeta

Esta carpeta (`/docs/11_examples/`) documenta los samples ejecutables del proyecto GeoVial: proyectos autocontenidos que se clonan, se corren en un entorno limpio y se modifican como punto de partida. Cada markdown explicativo describe un sample; la materialización en código vive en `/samples/<carpeta>/` del repositorio y se gobierna desde §5.X del PROJECT-README.

A diferencia de la categoría 10 (developer guide), que explica conceptos y guías intercaladas con snippets, la categoría 11 produce proyectos completos que se compilan, se ejecutan y se observan. La regla operativa es: 10 explica, 11 ejecuta.

Los samples cubren la superficie pública de los dos sub-proyectos de tipo `library` declarados en PROJECT-README §1: la librería de sincronización `GeoVial.Sync` (cuya superficie está contractualizada en `contratos-abstractions-sync_v1.0.md` de 05) y la librería de alojamiento de archivos `GeoVial.FileHosting` (cuyo punto de extensión está en `extensibilidad_v1.0.md` de 05).

### 1.1 Decisión de gating

La categoría 11 se genera para GeoVial por dos motivos combinados, registrados en PROJECT-README §1:

- El proyecto declara al menos un sub-proyecto de tipo `library` (la librería de sincronización publicada como paquete y la librería de alojamiento de archivos). Para `library`, la categoría 11 es obligatoria según §2.1 de la regla constructiva `11_rules_examples.md`.
- El flag `tiene_portal_developers: true` (PROJECT-README §1) refuerza la generación: la librería de sincronización se publica para reuso externo, lo que activa las categorías 10 y 11.

El tipo dominante D8 es `web-monolith`, para el cual la categoría 11 sería solo recomendada. La presencia del sub-proyecto `library` y del portal de developers la promueve a generada. Se aplica la variante `library` de §1.2 de la regla: apps consumidoras progresivas que invocan la librería publicada, cada sample mostrando un nivel distinto de su superficie pública.

## 2. Tabla maestra de samples

| Sample | Nivel | Tiempo de setup | CU ilustrados | Ubicación en /samples |
| --- | --- | --- | --- | --- |
| `ejemplo-01-sync-basico_v1.0.md` | Básico | < 5 min | CU-06, CU-07 | `/samples/01-sync-basico/` |
| `ejemplo-02-demo-movil-autonoma_v1.0.md` | Intermedio | 10-15 min | CU-06, CU-07, CU-12 | `/samples/02-sync-maui-demo/` |
| `ejemplo-03-filehosting-backends_v1.0.md` | Avanzado | 15-20 min | CU-04, CU-08, CU-09 | `/samples/03-filehosting-backends/` |

La correspondencia entre cada markdown y su carpeta es 1:1, coherente con la matriz de `/samples` de PROJECT-README §5 y §5.X. La progresión es por nivel (básico, intermedio, avanzado) reforzada con un slug por capacidad, nunca por entidad del dominio del producto.

## 3. Convenciones de los samples

- Autocontenidos: cada sample se ejecuta en un entorno limpio sin depender de servicios externos no triviales. Los backends que el sample necesita se materializan como mocks en memoria o como contenedores locales declarados en sus prerequisites.
- Ejecutables en ≤ 5 pasos: cada sample llega a su primera ejecución exitosa en cinco pasos copiables como máximo (§6 de la regla, anti-patrón de fricción de adopción).
- Nivel declarado: cada markdown declara su nivel explícito en §2, con justificación respecto al sample anterior.
- Trazabilidad obligatoria: cada markdown enlaza en §8 al menos un CU, ADR, RN, RC o NFR de 02/05.
- Output esperado: cada markdown documenta en §6 el output exacto que el desarrollador verá (texto de consola o archivo generado).
- Programación contra contratos: los snippets usan exclusivamente la superficie pública documentada en `referencia-api_v1.0.md` (10) y `contratos-abstractions-sync_v1.0.md` (05). No se invoca ninguna implementación concreta interna.
- Compatibilidad: los prerequisites declaran versiones mínimas alineadas con PROJECT-README §12 (Android 8.0+ para el sample móvil, contenedores Linux para el backend de archivos, .NET 9 o superior como runtime base).

## 4. Cómo agregar un sample nuevo

1. Decidir qué capacidad nueva demuestra el sample respecto a los existentes (la regla prohíbe samples que dupliquen `/src` sin valor demostrativo).
2. Elegir el slug por nivel o por capacidad, en kebab-case lowercase, sin token de framework ni entidad de dominio del producto (§3.1 de la regla). Numerar de forma correlativa.
3. Crear el markdown explicativo `ejemplo-XX-<kebab-progresion>_v1.0.md` con las nueve secciones obligatorias de §4.2 de la regla y la cabecera de §4.1 (con Nivel y Ubicación del código).
4. Crear la carpeta ejecutable `/samples/XX-<kebab-progresion>/` con su README propio, su código y sus tests de verificación del output esperado.
5. Registrar el sample en la tabla maestra de §2 de este README.
6. Verificar los 14 criterios de aceptación de §6 de `11_rules_examples.md` antes de dar el sample por vigente.

El template y la normativa de redacción están en `11_rules_examples.md`. La numeración refleja un orden de lectura recomendado de menor a mayor complejidad (§3.2 de la regla).

## 5. Vínculo con 10 (developer guide) y 05 (arquitectura)

Cada sample materializa lo que 10 explica y respeta lo que 05 contractualiza:

| Sample | Guía de 10 que lo acompaña | Contrato/punto de extensión de 05 que ejercita |
| --- | --- | --- |
| `01-sync-basico` | `guia-onboarding-developer_v1.0.md`, `conceptos-fundamentales_v1.0.md` | `contratos-abstractions-sync_v1.0.md` (superficie pública de `GeoVial.Sync`) |
| `02-sync-maui-demo` | `guia-integracion-aplicacion-movil_v1.0.md`, `referencia-api_v1.0.md` | `contratos-abstractions-sync_v1.0.md` (cola durable, conectividad, conflictos) |
| `03-filehosting-backends` | `referencia-api_v1.0.md` (como referencia de patrón de puerto) | `extensibilidad_v1.0.md` (punto de extensión de backend de almacenamiento) |

## 6. Validación de ejecutabilidad en CI

La ejecutabilidad de los samples no es opcional: para tipo `library` la regla exige un pipeline CI que compile y ejecute los samples (§6 de `11_rules_examples.md`). En GeoVial, el pipeline de `pipeline-ci-cd_v1.0.md` de 09 (GitHub Actions) compila la solución y empaqueta la librería de sincronización en STAGE-11 (`dotnet pack src/GeoVial.Sync`), con el gate de build sin warnings tratados como error (PROJECT-README §11, stages 2 y 4). Los samples consumen exactamente la superficie pública versionada de ese paquete; cualquier breaking change que rompa un sample se detecta como fallo de compilación en el pipeline. La verificación periódica de que los samples siguen siendo ejecutables se apoya en ese pipeline.

---

## Referencias cruzadas

- `11_rules_examples.md` — regla constructiva de esta categoría (normativa).
- `PROJECT-README-geovial_v1.0.md` §1, §5, §5.X, §12, §14 — sub-proyectos, estructura de repo, materialización de `/samples`, compatibilidad y estrategia de samples.
- `02_especificacion_funcional/casos-de-uso/` — CU que cada sample ilustra.
- `05_arquitectura_tecnica/contratos-abstractions-sync_v1.0.md` y `extensibilidad_v1.0.md` — superficie pública y punto de extensión.
- `10_developer_guide/` — guías conceptuales que cada sample materializa.
- `09_devops/pipeline-ci-cd_v1.0.md` — pipeline que valida la ejecutabilidad de los samples.

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | README inicial de la categoría 11: propósito, decisión de gating (library + tiene_portal_developers), tabla maestra de tres samples, convenciones, cómo agregar un sample, vínculo con 10 y 05, y validación de ejecutabilidad en CI. Generado por AG-11. |
