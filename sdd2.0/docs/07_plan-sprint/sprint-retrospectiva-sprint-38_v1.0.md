# Sprint Retrospectiva — Sprint 38

**Proyecto:** GeoVial
**Documento:** sprint-retrospectiva-sprint-38_v1.0.md
**Versión:** 1.0
**Estado:** Cerrado
**Fecha:** 2026-06-03
**Autor:** Scrum Master (AG-07), Equipo SDD 2.0

## 1. Qué salió bien

- El patrón núcleo-testeable volvió a pagar: `CacheTeselasDisco` (caché en disco acotada) quedó en el gate y la lógica del offline se verificó sin depender del dispositivo.
- Reusar `MapaTeselas.EsUrlDeTesela` mantuvo un único contrato de "qué es una tesela" entre web (S34) y móvil; el render y el caché comparten criterio.
- Se pudo verificar con un **dispositivo real**: la app con el `WebViewClient` propio se desplegó y arrancó sin crash, no sólo compiló.
- Cerró una acción que se arrastraba desde S32 (mapa al móvil) y S34/S35/S37 (offline de teselas).

## 2. Qué no salió bien

- El comportamiento de runtime del interceptor (servir teselas cacheadas estando sin red) no se prueba automáticamente: requiere navegar el mapa online, cortar la red y recargar; eso es prueba manual, no del gate.
- El `WebViewClient` propio reemplaza al de MAUI; para esta página de mapa estático es seguro, pero es un punto a vigilar si la página gana navegación/eventos.
- La descarga de teselas en `ShouldInterceptRequest` es sincrónica (en hilo de trabajo); aceptable acá, pero conviene revisarlo si el volumen crece.

## 3. Qué probar

- Prueba manual guiada en el dispositivo: cargar el mapa con red, activar modo avión, reabrir el mapa y confirmar que las teselas de la zona ya vista se muestran.
- Considerar un límite por tamaño en bytes (además de por cantidad de archivos) si las teselas pesan dispar.
- Evaluar unificar la caché web (Service Worker) y móvil (disco) bajo una misma política documentada.

## 4. Acciones concretas

| Acción | Responsable | Fecha compromiso | Estado |
| --- | --- | --- | --- |
| Prueba manual de offline en el dispositivo (modo avión) y registrar el resultado | AG-05 (QA) | 2027-12-11 | Pendiente |
| Prueba de `AlmacenS3` contra LocalStack | AG-05 (QA) | 2027-12-11 | Pendiente |
| Unificar la URL/atribución de teselas también en `mapaRevision.js` (web) | AG-08 (fullstack) | 2027-12-11 | Pendiente |

## 5. Seguimiento de acciones del sprint anterior

| Acción del Sprint 37 | Estado actual |
| --- | --- |
| Habilitar "Allow auto-merge" y branch protection en el repo | Pendiente (acto del admin; se reitera) |
| Verificar el auto-merge con el próximo PR de Dependabot | Pendiente (al próximo lote) |
| Mejoras del mapa: offline de teselas en el móvil | Completada (entregada en este sprint) |

## Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-03 | Retrospectiva del Sprint 38 (offline de teselas en el móvil): núcleo testeable + verificación en dispositivo real; 3 acciones nuevas. Generada por AG-07 a partir de `template-sprint-retrospectiva_v1.0.md` |
