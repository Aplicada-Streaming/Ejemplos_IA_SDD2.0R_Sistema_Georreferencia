# ADR-08 — Librería de alojamiento de archivos con backends configurables (local / S3 / otro)

**Proyecto:** GeoVial
**Documento:** ADR-08-libreria-alojamiento-backends-configurables_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-01
**Autor:** Arquitecto de Software Senior (AG-05), Equipo SDD 2.0
**Categoría:** Extensibilidad

## 1. Contexto

GeoVial almacena fotos asociadas a observaciones (CU-04, CU-09) y las incluye en la exportación/importación de relevamientos (CU-08). El almacenamiento local de archivos reside sobre el contenedor del backend, lo que lo acopla al ciclo de vida del contenedor (R-04 arquitectónico, PROJECT-README §16). El cliente requiere que el backend de alojamiento sea configurable por el usuario raíz: local, AWS S3 u otro (PROJECT-README §7, §1). Esto exige un punto de extensión con un contrato de backend estable.

## 2. Decisión

Se construye `GeoVial.FileHosting` como librería interna del backend con una abstracción de backend de almacenamiento (contrato de operaciones de subida, recuperación, borrado y referencia). Se proveen al menos dos implementaciones: almacenamiento local (sobre el contenedor del backend) y AWS S3. El usuario raíz selecciona y configura el backend activo por configuración. Nuevos backends se agregan implementando la abstracción y registrándolos, sin tocar el dominio.

## 3. Estado

Aceptado el 2026-06-01. Renumeración de `ADR-008` (PROJECT-README §15, decidido por el cliente) a `ADR-08`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Abstracción de backend con implementaciones registrables (elegido) | Configurable por el usuario raíz; desacopla el dominio del almacenamiento; extensible a nuevos backends | Requiere definir y mantener el contrato de backend |
| Almacenamiento local fijo en el contenedor | Trivial | No configurable; acopla los archivos al contenedor; incumple el requisito del cliente |
| Acceso directo a un SDK de nube en el dominio | Menos capas | Acopla el dominio a un proveedor; impide cambiar de backend sin reescribir |

## 5. Consecuencias positivas

1. El usuario raíz configura local, S3 u otro sin recompilar el dominio.
2. Desacopla el almacenamiento del ciclo de vida del contenedor backend cuando se usa S3 (mitiga R-04).
3. Punto de extensión documentado para terceros backends (extensibilidad).

## 6. Consecuencias negativas y trade-offs

1. La abstracción agrega una capa de indirección sobre el almacenamiento; aceptado por la configurabilidad exigida.
2. Hay que mantener el contrato de backend estable para no romper implementaciones existentes.

## 7. Implementación

`GeoVial.FileHosting` define la abstracción de backend; `GeoVial.Infrastructure` la integra. La base guarda la referencia del archivo, no el binario. La configuración del backend activo la fija el usuario raíz por configuración/secretos (PROJECT-README §8). Los puntos de extensión, el mecanismo de registro y el ejemplo se detallan en `extensibilidad_v1.0.md`, con el sample `samples/03-filehosting-backends` (referencia a 11).

## 8. Métricas de validación

- La misma operación de subida funciona contra backend local y contra S3 (sample 03-filehosting-backends).
- Cambiar el backend activo no requiere tocar el dominio ni el modelo de datos.
- Un nuevo backend se agrega implementando la abstracción y registrándolo.

## 9. Referencias

- PROJECT-README §1, §7, §8.
- CU-04, CU-08, CU-09.
- `extensibilidad_v1.0.md`, sample 03-filehosting-backends (11).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Decisión aceptada. Renumeración de ADR-008 a ADR-08 |
