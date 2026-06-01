# 02 Especificación funcional — GeoVial

Índice navegable de la categoría 02. Punto de entrada para revisores (AG-03, AG-05, AG-08). El índice maestro con la matriz NB → CU → RN → US es [especificacion-funcional_v1.0.md](especificacion-funcional_v1.0.md).

Tipo de proyecto: web-monolith. Mínimos §2.2: 8 CU, RN obligatorias, modelo conceptual obligatorio, RC si el modelo supera 10 entidades.

## Casos de uso (14 vigentes)

| CU | Documento | Propósito | Estado |
| --- | --- | --- | --- |
| CU-01 | [crear-relevamiento-asignar-agentes](casos-de-uso/CU-01-crear-relevamiento-asignar-agentes_v1.0.md) | Crear relevamiento y asignar o reasignar agentes | Propuesto |
| CU-02 | [seleccionar-relevamiento-asignado](casos-de-uso/CU-02-seleccionar-relevamiento-asignado_v1.0.md) | Iniciar sesión y elegir relevamiento asignado | Propuesto |
| CU-03 | [administrar-jerarquia-usuarios-areas](casos-de-uso/CU-03-administrar-jerarquia-usuarios-areas_v1.0.md) | Alta y baja jerárquica de usuarios y áreas | Propuesto |
| CU-04 | [capturar-observacion-georreferenciacion-automatica](casos-de-uso/CU-04-capturar-observacion-georreferenciacion-automatica_v1.0.md) | Captura con asignación automática de coordenadas | Propuesto |
| CU-05 | [ubicar-manualmente-punto](casos-de-uso/CU-05-ubicar-manualmente-punto_v1.0.md) | Ubicación manual y bandeja sin georreferenciar | Propuesto |
| CU-06 | [recolectar-observaciones-sin-conexion](casos-de-uso/CU-06-recolectar-observaciones-sin-conexion_v1.0.md) | Recolección sin conexión y encolado de cambios | Propuesto |
| CU-07 | [sincronizar-cambios-locales](casos-de-uso/CU-07-sincronizar-cambios-locales_v1.0.md) | Subir local y bajar remoto al recuperar conexión | Propuesto |
| CU-08 | [revisar-relevamiento-sobre-mapa](casos-de-uso/CU-08-revisar-relevamiento-sobre-mapa_v1.0.md) | Revisión sobre mapa y exportar/importar | Propuesto |
| CU-09 | [gestionar-marcador-fotos-comentarios-etiquetas](casos-de-uso/CU-09-gestionar-marcador-fotos-comentarios-etiquetas_v1.0.md) | Gestión de marcador, carrusel y navegación | Propuesto |
| CU-10 | [transicionar-estados-relevamiento](casos-de-uso/CU-10-transicionar-estados-relevamiento_v1.0.md) | Transición de estados y reapertura explícita | Propuesto |
| CU-11 | [detectar-marcadores-en-conflicto](casos-de-uso/CU-11-detectar-marcadores-en-conflicto_v1.0.md) | Detección de conflictos por radio configurable | Propuesto |
| CU-12 | [resolver-conflictos-sincronizacion-web](casos-de-uso/CU-12-resolver-conflictos-sincronizacion-web_v1.0.md) | Resolución de conflictos desde la web | Propuesto |
| CU-13 | [registrar-accesos-acciones-administrativas](casos-de-uso/CU-13-registrar-accesos-acciones-administrativas_v1.0.md) | Registro de accesos y acciones con retención | Propuesto |
| CU-14 | [aplicar-autorizacion-rol-area](casos-de-uso/CU-14-aplicar-autorizacion-rol-area_v1.0.md) | Autorización por rol y área (transversal) | Propuesto |

## Reglas de negocio (8 vigentes)

| RN | Documento | Propósito | Estado |
| --- | --- | --- | --- |
| RN-01 | [jerarquia-autorizacion-por-rol](reglas-de-negocio/RN-01-jerarquia-autorizacion-por-rol_v1.0.md) | Autorización por rol y área | Propuesto |
| RN-02 | [identidad-marcador-radio-agrupacion](reglas-de-negocio/RN-02-identidad-marcador-radio-agrupacion_v1.0.md) | Identidad de marcador y radio | Propuesto |
| RN-03 | [prioridad-exif-georreferenciacion](reglas-de-negocio/RN-03-prioridad-exif-georreferenciacion_v1.0.md) | Prioridad de metadatos de ubicación | Propuesto |
| RN-04 | [last-write-wins-marca-conflicto](reglas-de-negocio/RN-04-last-write-wins-marca-conflicto_v1.0.md) | Última escritura con marca de conflicto | Propuesto |
| RN-05 | [estados-relevamiento-solo-lectura-tras-cierre](reglas-de-negocio/RN-05-estados-relevamiento-solo-lectura-tras-cierre_v1.0.md) | Estados y solo lectura tras cierre | Propuesto |
| RN-06 | [precondicion-metodo-seguridad-offline](reglas-de-negocio/RN-06-precondicion-metodo-seguridad-offline_v1.0.md) | Método de seguridad como precondición offline | Propuesto |
| RN-07 | [retencion-auditoria](reglas-de-negocio/RN-07-retencion-auditoria_v1.0.md) | Retención del registro de auditoría | Propuesto |
| RN-08 | [tratamiento-datos-personales](reglas-de-negocio/RN-08-tratamiento-datos-personales_v1.0.md) | Tratamiento de datos personales (Ley 25.326) | Propuesto |

## Modelo de datos

| Artefacto | Documento | Estado |
| --- | --- | --- |
| Modelo conceptual | [modelo-conceptual_v1.0.md](modelo-datos/modelo-conceptual_v1.0.md) | Propuesto |

Conteo de entidades del modelo conceptual: **12** (Usuario, Área, Relevamiento, AsignaciónAgente, Observación, Marcador, Foto, Comentario, Etiqueta, RegistroCambioSync, ConflictoSync, RegistroAuditoría).

Decisión sobre RC: como el modelo conceptual **supera las 10 entidades** (12 > 10), por §2.2 (web-monolith) se generan reglas conceptuales del modelo. Se generaron **7 RC**.

## Reglas conceptuales del modelo (7 vigentes)

| RC | Documento | Propósito | Estado |
| --- | --- | --- | --- |
| RC-01 | [identidad-marcador-radio](modelo-datos/reglas-conceptuales-de-modelo/RC-01-identidad-marcador-radio_v1.0.md) | Identidad de marcador y radio | Propuesto |
| RC-02 | [integridad-observacion-marcador](modelo-datos/reglas-conceptuales-de-modelo/RC-02-integridad-observacion-marcador_v1.0.md) | Integridad observación → marcador | Propuesto |
| RC-03 | [unicidad-identificador-cola-sync](modelo-datos/reglas-conceptuales-de-modelo/RC-03-unicidad-identificador-cola-sync_v1.0.md) | Unicidad del identificador en la cola de sync | Propuesto |
| RC-04 | [cardinalidad-foto-comentario-etiqueta](modelo-datos/reglas-conceptuales-de-modelo/RC-04-cardinalidad-foto-comentario-etiqueta_v1.0.md) | Cardinalidad foto/comentario/etiqueta | Propuesto |
| RC-05 | [valores-rol-jerarquico](modelo-datos/reglas-conceptuales-de-modelo/RC-05-valores-rol-jerarquico_v1.0.md) | Valores del rol jerárquico | Propuesto |
| RC-06 | [estados-transiciones-relevamiento](modelo-datos/reglas-conceptuales-de-modelo/RC-06-estados-transiciones-relevamiento_v1.0.md) | Estados y transiciones del relevamiento | Propuesto |
| RC-07 | [integridad-asignacion-agente-area](modelo-datos/reglas-conceptuales-de-modelo/RC-07-integridad-asignacion-agente-area_v1.0.md) | Integridad de la asignación de agente al área | Propuesto |

## Cobertura NB → CU

NB-01 → CU-01, CU-02, CU-03. NB-02 → CU-04, CU-05. NB-03 → CU-06, CU-07. NB-04 → CU-08, CU-09, CU-10. NB-05 → CU-11, CU-12. NB-06 → CU-13, CU-14. Cobertura bidireccional completa: sin NB huérfana ni CU huérfano.
