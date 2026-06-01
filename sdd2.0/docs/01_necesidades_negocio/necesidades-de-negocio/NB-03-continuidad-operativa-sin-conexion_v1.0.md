# NB-03 — Continuidad operativa en terreno sin conexión con sincronización confiable

| Campo | Valor |
| --- | --- |
| Proyecto | GeoVial |
| Documento | NB-03-continuidad-operativa-sin-conexion_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-05-31 |
| Autor | Analista de Negocio Senior (AG-01), Equipo SDD 2.0 |
| Trazabilidad upstream | PROJECT-BRIEF §3, §6, §8, §11; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-06, CU-07 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita que el personal de campo pueda trabajar una jornada completa en zonas sin cobertura y que después nada de lo recolectado se pierda. Las obras de infraestructura vial suelen estar en lugares sin señal, y hoy la dependencia de conexión y de soportes sueltos hace que las observaciones se traspapelen o no lleguen nunca al área que evalúa.

El dolor concreto tiene dos caras. Primero, la continuidad: si la herramienta exige conexión permanente, el relevamiento se interrumpe justo donde más se necesita. Segundo, la confianza en el traspaso: lo que se capturó sin señal tiene que llegar íntegro y a tiempo cuando el dispositivo recupera conexión, sin que el agente tenga que recordar pasos manuales ni arriesgar la pérdida del trabajo de toda una jornada si el dispositivo se daña o se extravía.

Si esta necesidad no se resuelve, el relevamiento de campo no es viable en el escenario real del organismo, se pierden observaciones antes de consolidarse y la base de datos queda incompleta e intermitente.

## 2. Ejemplo de uso desde la perspectiva del negocio

Una cuadrilla pasa toda la mañana relevando un puente en una zona sin señal y registra decenas de observaciones con sus fotos. En el método actual, parte de ese trabajo queda en notas y fotos sueltas que después hay que volcar a mano y a veces se pierden. Con la necesidad resuelta, la cuadrilla trabaja la jornada completa sin conexión y, al volver a una zona con señal, todo lo recolectado se traspasa solo y queda disponible para el área que evalúa en pocos minutos.

## 3. Impacto

- Hace viable el relevamiento en el escenario real de obras sin cobertura.
- Reduce el riesgo de pérdida de observaciones capturadas antes de consolidarse.
- Evita el retrabajo de volcar a mano lo registrado en campo.
- Si no se resuelve, el trabajo de campo es intermitente y la base de datos queda incompleta.
- Acota el tiempo entre la recolección en terreno y la disponibilidad de los datos para evaluar.

## 4. Problema específico que resuelve

- La conexión es intermitente o nula en las obras, lo que interrumpe la recolección si se exige estar en línea.
- Lo capturado sin señal puede perderse si el dispositivo se daña o se extravía antes del traspaso.
- El traspaso manual de lo registrado es lento, propenso a olvidos y a duplicación de esfuerzo.
- No hay garantía de que el traspaso ocurra al menos una vez por jornada de trabajo.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Autonomía sin conexión | Duración de operación de campo sin conexión soportada | ≥ 1 jornada laboral (8 h) | desde el lanzamiento |
| Eficiencia del traspaso | Tiempo de sincronización de una jornada típica de campo | ≤ 5 minutos | 3 meses post-lanzamiento |
| Integridad del traspaso | Porcentaje de observaciones capturadas sin conexión que se sincronizan sin pérdida | 100 % | continuo, revisión mensual |
| Frecuencia de resguardo | Porcentaje de jornadas con al menos una sincronización completada | ≥ 95 % | 6 meses post-lanzamiento |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Jefe general | Propietario | Aprueba la prioridad de habilitar el trabajo de campo en zonas sin cobertura |
| Equipo de desarrollo | Implementador | Construye la operación sin conexión y el mecanismo de sincronización confiable |
| Agente de campo | Beneficiario | Trabaja la jornada sin señal y valida que no pierde lo recolectado |
| Jefe de área | Beneficiario | Recibe a tiempo y completo lo recolectado para poder evaluar |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-03 | CU-06 recolectar observaciones en modo sin conexión | a generar |
| NB-03 | CU-07 sincronizar cambios locales al recuperar conexión | a generar |

## 8. Dependencias con otras NB

Depende de NB-01 (define quién recolecta) y de NB-02 (lo que se sincroniza son observaciones georreferenciadas). Es prerequisito de NB-05, que resuelve los conflictos que surgen al sincronizar.

## 9. Prioridad MoSCoW

Must Have. Sin operación sin conexión y sincronización confiable, el relevamiento no es viable en el terreno real del organismo.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-05-31 | Versión inicial generada por AG-01 a partir de los intakes y la categoría 00 |
