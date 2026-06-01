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
}
