# Guía de testing de extensibilidad — GeoVial

**Proyecto:** GeoVial
**Documento:** guia-testing-extensibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Ingeniero QA / SDET Senior (AG-08), Equipo SDD 2.0
**Trazabilidad upstream:** `05_arquitectura_tecnica/extensibilidad_v1.0.md`; ADR-08; CU-04, CU-08, CU-09; RN-05, RN-08

## 1. Propósito

GeoVial expone un punto de extensión interno: la librería de alojamiento de archivos `GeoVial.FileHosting`, cuyos backends de almacenamiento de fotos son configurables por el usuario raíz (local sobre el contenedor del backend, AWS S3 u otro), según `extensibilidad_v1.0.md` de 05 y ADR-08 (`tiene_extensibilidad: true`, PROJECT-README §1). Esta guía define cómo se prueba esa extensibilidad sin modificar el núcleo: una batería de contract tests común a todos los backends, sus fixtures y el procedimiento para que un nuevo backend demuestre su conformidad. La librería de sincronización `GeoVial.Sync` también publica una API estable cuya conformidad se prueba por contrato (§6).

## 2. Qué se prueba y qué no

- Se prueba: que toda implementación de la abstracción de backend cumple el contrato de almacenamiento (Guardar, Recuperar, Eliminar, Existe), que Guardar es idempotente para una misma referencia lógica, que la referencia devuelta es opaca y estable, que los errores se reportan de forma tipada (para traducirse a Problem Details, ADR-11) y que Eliminar respeta el solo lectura del relevamiento cerrado (RN-05).
- No se prueba en estos tests: el dominio de GeoVial (se prueba en sus propios unit/integration) ni detalles internos del proveedor; el contrato es agnóstico del backend (`extensibilidad_v1.0.md` §3). El dominio nunca conoce el backend concreto: depende del puerto de `GeoVial.FileHosting` (Clean Architecture, ADR-10).

## 3. Contract tests por backend

El mecanismo central es una batería de contract tests parametrizada por backend: un único conjunto de aserciones que se ejecuta idéntico contra cada implementación. Materializa el TC-26 (`filehosting-conformidad-backend`) de `casos-prueba-referenciales`.

| Aserción del contrato | Verifica |
| --- | --- |
| Guardar y luego Recuperar devuelve el mismo binario | Persistencia y recuperación íntegra |
| Guardar dos veces la misma referencia lógica no duplica ni corrompe | Idempotencia de Guardar (`extensibilidad_v1.0.md` §3) |
| La referencia devuelta es opaca, estable y persistible | Agnosticismo de proveedor; la base guarda solo la referencia |
| Existe responde verdadero tras Guardar y falso tras Eliminar | Coherencia de Existe/Eliminar |
| Eliminar sobre foto de relevamiento cerrado se rechaza | Solo lectura tras cierre (RN-05) |
| Error de almacenamiento se reporta de forma tipada | Traducción a Problem Details en aplicación (ADR-11) |
| Recuperar una referencia inexistente reporta error tipado, no excepción cruda | Manejo de error de borde |

La misma clase de contract tests se ejecuta contra el backend local y contra el backend S3; ambos deben pasar el set idéntico. Tipo de test: contract (estrategia-testing §3, tooling de integración).

## 4. Fixtures

- Fixture de archivo de foto: binario sintético de tamaño representativo (no datos reales; RN-08), versionado en `tests/` junto al dataset sintético (estrategia-testing §6).
- Fixture de referencia de relevamiento cerrado: estado mínimo que permite probar la aserción de solo lectura (RN-05) sin levantar todo el dominio.
- Fixtures compartidos y reutilizados entre backends para evitar duplicación (estrategia-testing §5); ningún fixture depende del orden de ejecución.

## 5. Ambiente por backend

- Backend local: corre por defecto en CI; usa un directorio temporal aislado por test que se descarta al finalizar.
- Backend S3: se prueba contra un endpoint no productivo o un doble compatible con la API de S3; los secretos viven en el secret store del entorno, nunca en git (PROJECT-README §8; `extensibilidad_v1.0.md` §4). No se usan credenciales reales del organismo.
- Aislamiento: cada corrida del contract test parte de un estado limpio; el backend activo se resuelve por inyección de dependencias según la configuración (`extensibilidad_v1.0.md` §4), igual que en ejecución real.

## 6. Cómo un nuevo backend prueba su conformidad

Para incorporar un tercer backend sin modificar el núcleo (ADR-08, `extensibilidad_v1.0.md` §5):

1. Implementar la abstracción de almacenamiento de `GeoVial.FileHosting` (Guardar, Recuperar, Eliminar, Existe).
2. Registrar la nueva implementación en la configuración de DI, sin tocar el dominio ni el modelo de datos.
3. Parametrizar la batería de contract tests de §3 con el nuevo backend: el nuevo backend debe pasar el mismo set de aserciones idéntico que el local y el S3, incluyendo idempotencia de Guardar y respeto del solo lectura (RN-05).
4. Reusar el flujo de subida del sample `samples/03-filehosting-backends` (11) como plantilla, que demuestra la misma operación de subida contra local y S3; el nuevo backend se agrega al sample para evidenciar que cambiar el backend activo no altera el código de dominio.

El criterio de aceptación de conformidad de un nuevo backend es: la batería de contract tests pasa completa para ese backend, sin modificar las aserciones existentes. Si una aserción no se cumple, el backend no es conforme y no se habilita por configuración.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Punto de extensión | Backend de almacenamiento de `GeoVial.FileHosting` (`extensibilidad_v1.0.md` de 05) |
| ADR que lo justifica | ADR-08 (backends configurables) |
| CU que lo consumen | CU-04 (guardar foto), CU-08 (export/import con fotos), CU-09 (carrusel) |
| RN aplicables | RN-05 (solo lectura tras cierre), RN-08 (acceso acotado a datos) |
| TC asociado | TC-26 (filehosting-conformidad-backend) en casos-prueba-referenciales |
| Sample de extensión | samples/03-filehosting-backends (11) |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Guía inicial de testing de extensibilidad: contract tests parametrizados por backend de `GeoVial.FileHosting`, fixtures, ambiente por backend y procedimiento de conformidad de un nuevo backend sin modificar el núcleo. Referencia a extensibilidad_v1.0.md de 05 y al sample de 11. Generada por AG-08 |
