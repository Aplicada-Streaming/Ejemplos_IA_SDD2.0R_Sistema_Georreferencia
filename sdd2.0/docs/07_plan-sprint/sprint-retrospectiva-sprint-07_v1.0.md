# Sprint Retrospectiva — Sprint 07

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-07_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-01
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- Aplicar la acción de la retro anterior —combinar trabajo para acercarse al rango estable— subió el compromiso a 16 SP (vs 13 de los dos sprints previos) y se completó al 100 %, recuperando tracción.
- Mantener la abstracción `IAlmacenFotos` en `GeoVial.FileHosting` sin dependencias de proveedor dejó el SDK de AWS confinado a infraestructura: el dominio y la aplicación dependen solo del contrato.
- Inyectar `IAmazonS3` en `AlmacenS3` hizo testeable el backend S3 sin una cuenta real; el cliente sustituido verifica subida, recuperación y existencia, cumpliendo la métrica de ADR-08 sin red.
- Extender el contrato del empaquetado para transportar binarios no regresionó el round-trip del Sprint 06: las pruebas de ida y vuelta ahora verifican manifiesto y binarios juntos.

## 2. Qué no salió bien

- Tocar la firma de `IEmpaquetadorRelevamiento` obligó a actualizar las pruebas del Sprint 06; aunque el cambio fue acotado, confirma que consolidar contratos antes de tener todos los consumidores tiene costo. Conviene anticipar el transporte de binarios al diseñar un contrato de empaquetado.
- El backend local escribe en una carpeta relativa por defecto; en las pruebas de integración eso crea una carpeta de trabajo. Es inocuo, pero conviene parametrizar la ruta por entorno para no ensuciar el directorio.

## 3. Qué probar

- Sumar el sample `03-filehosting-backends` (verificación de despliegue contra S3 real) al pipeline de entrega, no al de PR, para validar el backend remoto sin frenar el ciclo de revisión.
- Al incorporar BT-19 (pipeline de imágenes), comprimir el binario antes de `GuardarAsync` para acotar el payload de exportación y sincronización; extender el round-trip para verificar el tamaño.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Parametrizar la ruta del backend local por entorno (evitar carpeta relativa en pruebas) | AG-08 | 2026-09-19 | Pendiente |
| Agregar la verificación contra S3 real (sample 03) al pipeline de entrega | AG-09 | 2026-09-19 | Pendiente |
| Refinar BT-19 (pipeline de imágenes) y EP-04 (sincronización) para los próximos sprints | AG-06 | 2026-09-19 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 06 | Estado actual |
| --- | --- |
| Integrar la librería de alojamiento (ADR-08) e incluir los binarios de fotos en el ZIP | Completada (BT-20 + cierre de BT-21 entregados en este sprint) |
| Combinar épica acotada + ítem de backlog para aprovechar la capacidad | Completada (se comprometieron 16 SP con buen resultado) |
| Configurar la cobertura del pipeline con limpieza previa | Pendiente (se reitera) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Retrospectiva del Sprint 07 con 3 acciones nuevas y seguimiento de las del Sprint 06 (alojamiento y compromiso ampliado completados). Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
