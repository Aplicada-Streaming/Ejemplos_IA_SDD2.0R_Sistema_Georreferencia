namespace GeoVial.Domain;

/// <summary>
/// Catálogo de códigos de error del dominio, alineado con los códigos declarados en los CU de 02
/// (CU-02, CU-03, CU-14) y las reglas RN-01, RN-06. Son estables y se mapean a Problem Details (RFC 7807)
/// en la capa API (ADR-11).
/// </summary>
public static class CodigosError
{
    // RN-01 / CU-03 / CU-14
    public const string AccesoNoAutorizado = "ACCESO_NO_AUTORIZADO";
    public const string AccesoDatoPersonalNoAutorizado = "ACCESO_DATO_PERSONAL_NO_AUTORIZADO";

    // CU-03
    public const string AreaInexistente = "AREA_INEXISTENTE";
    public const string AccionNoAuditada = "ACCION_NO_AUDITADA";

    // CU-02 / RN-06
    public const string CredencialesInvalidas = "CREDENCIALES_INVALIDAS";
    public const string OfflineNoHabilitado = "OFFLINE_NO_HABILITADO";
    public const string ReingresoSinMetodoSeguridad = "REINGRESO_SIN_METODO_SEGURIDAD";

    // Invariantes de dominio (RC-05)
    public const string NombreRequerido = "NOMBRE_REQUERIDO";
    public const string AreaRequerida = "AREA_REQUERIDA";
    public const string UsuarioInexistente = "USUARIO_INEXISTENTE";

    // Relevamientos (CU-01, CU-10; RN-02, RN-05)
    public const string IdentificacionRequerida = "IDENTIFICACION_REQUERIDA";
    public const string RadioInvalido = "RADIO_INVALIDO";
    public const string AgenteFueraDeArea = "AGENTE_FUERA_DE_AREA";
    public const string RelevamientoSoloLectura = "RELEVAMIENTO_SOLO_LECTURA";
    public const string TransicionInvalida = "TRANSICION_INVALIDA";
    public const string ReaperturaNoAutorizada = "REAPERTURA_NO_AUTORIZADA";
    public const string RelevamientoInexistente = "RELEVAMIENTO_INEXISTENTE";

    // Captura y georreferenciación (CU-04, CU-05; RN-02, RN-03)
    public const string ObservacionSinGeorreferencia = "OBSERVACION_SIN_GEORREFERENCIA";
    public const string FuenteUbicacionIncorrecta = "FUENTE_UBICACION_INCORRECTA";
    public const string ObservacionInexistente = "OBSERVACION_INEXISTENTE";
    public const string MarcadorInexistente = "MARCADOR_INEXISTENTE";
    public const string AgenteNoAsignado = "AGENTE_NO_ASIGNADO";

    // Provisión de credenciales (BT-23; ADR-03)
    public const string NombreUsuarioEnUso = "NOMBRE_USUARIO_EN_USO";
    public const string ClaveRequerida = "CLAVE_REQUERIDA";
    public const string NombreUsuarioRequerido = "NOMBRE_USUARIO_REQUERIDO";

    // Comentarios y etiquetas del marcador (CU-09; RC-04)
    public const string TextoRequerido = "TEXTO_REQUERIDO";
    public const string ComentarioInexistente = "COMENTARIO_INEXISTENTE";
    public const string FotoInexistente = "FOTO_INEXISTENTE";
    public const string EtiquetaRequerida = "ETIQUETA_REQUERIDA";

    // Resolución de conflictos (CU-11, CU-12; RN-02, RN-04)
    public const string ConflictoInexistente = "CONFLICTO_INEXISTENTE";
    public const string UnificacionNoAutorizada = "UNIFICACION_NO_AUTORIZADA";

    // Exportación e importación (CU-08 §5.A/§5.B; EP-07)
    public const string ArchivoExportacionInvalido = "ARCHIVO_EXPORTACION_INVALIDO";

    // Alojamiento de fotos (CU-04; ADR-08, BT-20)
    public const string ContenidoFotoRequerido = "CONTENIDO_FOTO_REQUERIDO";

    // Sincronización (CU-07; RN-04)
    public const string ConsolidacionInvalida = "CONSOLIDACION_INVALIDA";
    public const string ConflictoNoMarcado = "CONFLICTO_NO_MARCADO";
    public const string SincronizacionInterrumpida = "SINCRONIZACION_INTERRUMPIDA";
}
