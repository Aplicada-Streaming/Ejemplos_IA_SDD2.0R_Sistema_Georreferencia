# Ejemplo 03 — Alojamiento de archivos con backends configurables

**Proyecto:** GeoVial
**Documento:** ejemplo-03-filehosting-backends_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-01
**Autor:** Developer Advocate / Sample Engineer Senior (AG-11), Equipo SDD 2.0
**Nivel:** Avanzado
**Ubicación del código:** `/samples/03-filehosting-backends/`

## 1. Objetivo del sample

Demostrar el punto de extensión de la librería de alojamiento de archivos `GeoVial.FileHosting`: la misma operación de subida de una foto se ejecuta contra un backend de almacenamiento local y contra AWS S3 (u otro), sin cambiar el código que invoca la subida. Al ejecutarlo, el desarrollador entiende cómo el backend activo se resuelve por configuración e inyección de dependencias, y obtiene una plantilla para implementar un tercer backend. Es el sample que ejercita el punto de extensión documentado en `extensibilidad_v1.0.md` (05).

## 2. Nivel

Avanzado. Es el sample de punto de extensión principal. No depende de los samples 01 ni 02 (cambia de librería: aquí es alojamiento de archivos, no sincronización), pero asume comodidad con inyección de dependencias y configuración por entorno. Demuestra una capacidad que ningún sample anterior cubría: intercambiar la implementación concreta de un puerto sin tocar el código de dominio ni el modelo de datos.

## 3. Prerequisites

| Requisito | Versión mínima | Cómo obtenerlo |
| --- | --- | --- |
| SDK de .NET | .NET 9 o superior (PROJECT-README §2) | Instalar el SDK desde el canal oficial de .NET. Verificar con `dotnet --info`. |
| Docker (contenedores Linux) | Engine reciente con soporte Linux containers (PROJECT-README §12) | Para levantar el emulador de S3 local. Verificar con `docker version`. |
| Emulador de S3 local | Compatible con la API S3 (por ejemplo MinIO) | Se levanta vía `docker compose` con el archivo incluido en el sample. Evita credenciales reales de AWS. |
| Credenciales del backend S3 | Apuntan al emulador local | Provistas en `.env.example` del sample; se copian a `.env` (fuera de git, PROJECT-README §8). |

El backend local no requiere Docker: escribe sobre el sistema de archivos del propio sample (almacenamiento local sobre el contenedor del backend, PROJECT-README §7). Docker solo es necesario para el backend S3 emulado.

## 4. Cómo correrlo

1. Entrar a la carpeta del sample: `cd samples/03-filehosting-backends`.
2. Copiar la configuración: `cp .env.example .env` y levantar el emulador de S3: `docker compose up -d`.
3. Subir la foto contra el backend local: `dotnet run -- --backend local`.
4. Subir la misma foto contra el backend S3: `dotnet run -- --backend s3`.
5. Comparar ambas salidas con §6: la operación es idéntica y solo cambia la referencia devuelta.

## 5. Estructura del código

```text
03-filehosting-backends/
├── README.md                       # Explica el punto de extensión y el orden de ejecución
├── 03-filehosting-backends.csproj   # Proyecto de consola, referencia a GeoVial.FileHosting
├── Program.cs                      # Lee --backend, resuelve el backend activo por DI y sube la foto
├── appsettings.json                # Selección del backend activo (local | s3)
├── .env.example                    # Credenciales del backend S3 (apuntan al emulador local)
├── docker-compose.yml              # Emulador de S3 local
├── data/
│   └── foto-ejemplo.jpg            # Foto de ejemplo a subir
└── tests/
    └── FileHostingBackendsTests.cs # Verifica que la misma subida funciona en ambos backends
```

`Program.cs` lee el backend pedido, lo registra por inyección de dependencias según `appsettings.json` (mecanismo de §4 de `extensibilidad_v1.0.md`) y ejecuta la misma operación Guardar de la abstracción de backend de almacenamiento. La base persistiría solo la referencia opaca devuelta (`Foto.ReferenciaArchivo`, modelo lógico §1.7), nunca el binario. El código de subida no conoce el backend concreto.

## 6. Qué esperar

Subida contra el backend local (`dotnet run -- --backend local`):

```text
Backend activo: local
Subiendo data/foto-ejemplo.jpg ...
Guardado OK. Referencia: local://fotos/foto-ejemplo.jpg
Existe? True
```

Subida contra el backend S3 (`dotnet run -- --backend s3`):

```text
Backend activo: s3
Subiendo data/foto-ejemplo.jpg ...
Guardado OK. Referencia: s3://geovial-demo/fotos/foto-ejemplo.jpg
Existe? True
```

La operación de subida es idéntica en ambos casos: lo único que cambia es la referencia opaca devuelta por cada backend. Esto evidencia que cambiar el backend activo no altera el código de dominio ni el modelo de datos.

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado esperado |
| --- | --- | --- |
| Implementar un tercer backend | Crear una clase que implemente la abstracción de almacenamiento y registrarla en `appsettings.json` | El nuevo backend funciona con el mismo código de subida, sin tocar el dominio (plantilla de `extensibilidad_v1.0.md` §5) |
| Recuperar y eliminar la foto | Llamar a las operaciones Recuperar y Eliminar tras la subida | El binario se recupera por su referencia y se elimina respetando la solo lectura del relevamiento cerrado (RN-05) |
| Probar idempotencia de Guardar | Subir la misma referencia lógica dos veces | El backend no duplica el binario (Guardar idempotente, `extensibilidad_v1.0.md` §3) |
| Cambiar el backend activo por configuración | Editar `appsettings.json` en lugar de pasar `--backend` | La selección por configuración resuelve el mismo backend sin recompilar |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-04 | Caso de uso | Materializa la persistencia del binario de la foto de una observación contra el backend activo. |
| CU-08 | Caso de uso | El alojamiento configurable sustenta la exportación/importación de relevamientos con sus fotos. |
| CU-09 | Caso de uso | Las fotos del carrusel se recuperan por su referencia desde el backend activo. |
| ADR-08 | Decisión arquitectónica | Implementa la librería de alojamiento con backends configurables (local / S3 / otro). |
| RN-05 | Regla de negocio | La operación Eliminar respeta la solo lectura del relevamiento cerrado. |
| `extensibilidad_v1.0.md` (05) | Punto de extensión | Ejercita la abstracción de backend de almacenamiento, su contrato y el registro por configuración. |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial. Sample avanzado del punto de extensión de backends de almacenamiento: misma subida contra backend local y S3. Generado por AG-11. |
