# Extensibilidad — Librería de alojamiento de archivos — GeoVial

**Proyecto:** GeoVial
**Documento:** extensibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Trazabilidad upstream:** ADR-08; CU-04, CU-08, CU-09; PROJECT-README §7, §14

## 1. Objetivo

Documenta los puntos de extensión de la librería de alojamiento de archivos `GeoVial.FileHosting`, que permite al usuario raíz configurar el backend de almacenamiento de fotos: local sobre el contenedor del backend, AWS S3 u otro disponible. La extensibilidad la justifica ADR-08 (`tiene_extensibilidad: true`, PROJECT-README §1).

## 2. Punto de extensión: backend de almacenamiento

El punto de extensión es la abstracción de backend de almacenamiento. Toda implementación provee las operaciones mínimas sobre archivos de foto:

| Operación | Responsabilidad |
| --- | --- |
| Guardar | Persiste el binario de la foto y devuelve una referencia estable |
| Recuperar | Devuelve el binario a partir de su referencia |
| Eliminar | Quita el binario por referencia (respetando la solo lectura del relevamiento cerrado, RN-05) |
| Existe | Verifica la presencia de un archivo por referencia |

La base de datos persiste únicamente la referencia (`Foto.ReferenciaArchivo`, modelo lógico §1.7), nunca el binario. El dominio no conoce el backend concreto: depende del puerto definido en `GeoVial.FileHosting` (Clean Architecture, ADR-10).

## 3. Contrato del backend

Una implementación de backend cumple el contrato de la abstracción de almacenamiento:

- Es idempotente en Guardar para una misma referencia lógica.
- Devuelve una referencia opaca y estable que la base persiste.
- No filtra detalles del proveedor al dominio (la referencia es agnóstica del backend).
- Reporta errores de almacenamiento de forma tipada para que la capa de aplicación los traduzca a Problem Details (ADR-11).

Implementaciones provistas en v1: almacenamiento local (sobre el contenedor del backend) y AWS S3 (ADR-08).

## 4. Mecanismo de registro y configuración

- El usuario raíz selecciona el backend activo por configuración (PROJECT-README §7, §8); los secretos del backend (por ejemplo credenciales de S3) viven en el secret store del entorno, no en git.
- El backend activo se resuelve por inyección de dependencias al iniciar el backend: la configuración determina qué implementación de la abstracción se registra.
- Un nuevo backend se incorpora implementando la abstracción de almacenamiento y registrándolo en la configuración de DI, sin modificar el dominio ni el modelo de datos.

## 5. Ejemplo de extensión

El sample `samples/03-filehosting-backends` (referencia a 11) muestra la misma operación de subida de una foto contra el backend local y contra AWS S3, demostrando que cambiar el backend activo no altera el código de dominio ni el modelo de datos. Sirve de plantilla para implementar un tercer backend: implementar la abstracción, registrarlo por configuración y reusar el mismo flujo de subida.

## 6. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU que lo consumen | CU-04 (guardar foto), CU-08 (exportar/importar con fotos), CU-09 (carrusel) |
| RN aplicables | RN-05 (solo lectura tras cierre), RN-08 (acceso acotado a datos) |
| ADR que lo justifica | ADR-08 |
| Ejemplo de extensión | samples/03-filehosting-backends (11) |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Extensibilidad inicial: punto de extensión de backend de almacenamiento, contrato, registro por configuración del usuario raíz y ejemplo. Generado por AG-05 |
