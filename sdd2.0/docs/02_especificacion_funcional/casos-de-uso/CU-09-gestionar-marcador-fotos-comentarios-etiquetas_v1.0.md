# CU-09 — Gestionar un marcador y recorrer sus fotos y comentarios en carrusel

**Proyecto:** GeoVial
**Documento:** CU-09-gestionar-marcador-fotos-comentarios-etiquetas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-01
**Autor:** Analista Funcional senior (AG-02), Equipo SDD 2.0

## 1. Propósito

Permitir gestionar el contenido de un marcador —agregar y quitar fotos, comentar, etiquetar y recorrer las fotos en un carrusel— y navegar entre marcadores, para enriquecer y revisar la observación de un punto.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo o jefe de área | Primario | Administra fotos, comentarios y etiquetas del marcador y recorre el carrusel |
| Sistema de marcadores | Sistema | Persiste fotos, comentarios y etiquetas, presenta el carrusel y permite navegar entre marcadores |

## 3. Precondiciones

- Existe un marcador dentro de un relevamiento no cerrado para gestionar contenido (RN-05); un relevamiento cerrado admite solo lectura.
- El usuario tiene acceso al relevamiento según su rol y área (RN-01).

## 4. Flujo principal

1. El usuario abre un marcador del relevamiento.
2. El sistema muestra las fotos del marcador en un carrusel, con sus comentarios y etiquetas.
3. El usuario agrega o quita fotos del marcador.
4. El usuario comenta una foto o el marcador y aplica o quita etiquetas a fotos o comentarios.
5. El usuario recorre las fotos en el carrusel y navega al marcador siguiente o anterior.
6. El sistema persiste los cambios y mantiene la navegación entre marcadores.

## 5. Flujos alternativos

- 5.A Quitar una foto. Disparador: el usuario elimina una foto del marcador. El sistema la quita junto con sus comentarios y etiquetas asociados y conserva el resto del marcador. Punto de retorno: paso 2.
- 5.B Navegación entre marcadores. Disparador: el usuario pasa al marcador siguiente o anterior. El sistema carga el marcador destino con su carrusel. Punto de retorno: paso 2.
- 5.C Revisión de solo lectura. Disparador: el relevamiento está cerrado (RN-05). El sistema muestra el carrusel y permite recorrer y navegar, pero no agregar ni quitar contenido. Punto de retorno: paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| `RELEVAMIENTO_SOLO_LECTURA` | Se intenta modificar contenido de un marcador de un relevamiento cerrado (RN-05) | Rechaza la modificación y mantiene la vista de solo lectura |
| `MARCADOR_INEXISTENTE` | Se intenta abrir o navegar a un marcador que no existe en el relevamiento | Informa que el marcador no existe y vuelve a la vista del relevamiento |
| `ACCESO_NO_AUTORIZADO` | El usuario no tiene acceso al relevamiento por rol o área (RN-01) | Niega el acceso y registra el intento |

## 7. Postcondiciones

- Éxito: el marcador refleja las fotos, comentarios y etiquetas gestionados; el usuario pudo recorrer el carrusel y navegar entre marcadores.
- Fallo: no se modifica el contenido del marcador; se mantiene la vista de solo lectura cuando corresponde.

## 8. Criterios de aceptación

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un marcador con tres fotos en un relevamiento en recolección | El agente agrega una cuarta foto con un comentario y una etiqueta | El sistema persiste la foto, el comentario y la etiqueta, y el carrusel muestra cuatro fotos |
| CA-02 | Un marcador con cinco fotos | El usuario recorre el carrusel y navega al marcador siguiente | El sistema avanza por las cinco fotos y carga el marcador siguiente con su carrusel |
| CA-03 | Un marcador en un relevamiento cerrado | El usuario intenta quitar una foto | El sistema responde `RELEVAMIENTO_SOLO_LECTURA` y conserva las fotos |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-01, RN-05 |
| Historias de usuario a generar | US a generar en 06 (administrar fotos, comentar, etiquetar, carrusel y navegación) |
| Componentes esperados | Módulo de marcadores y carrusel (referencia tentativa a 05) |
| Tests previstos | Suite de gestión de marcador, carrusel y navegación (referencia tentativa a 08) |

## 10. Notas y supuestos

- El detalle visual del carrusel, el visor a pantalla completa y el zoom pertenecen a 03; aquí se define qué contenido gestiona el marcador y la navegación funcional entre marcadores.
- Una foto puede o no estar ligada a un comentario, conforme al glosario del dominio.

## 11. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-01 | Versión inicial generada por AG-02 a partir de NB-04 |

## 13. Interacción multiusuario y concurrencia

Varios usuarios pueden gestionar el mismo marcador, en campo y en web. Las ediciones concurrentes del contenido del marcador se consolidan por última escritura al sincronizar (RN-04), quedando marcadas como conflicto cuando los cambios son incompatibles, para resolución desde la web (CU-12).
