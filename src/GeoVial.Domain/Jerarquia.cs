namespace GeoVial.Domain;

/// <summary>
/// Reglas de administración jerárquica (BT-02). Cada nivel solo administra el inmediato inferior:
/// raíz → jefe general, jefe general → jefes de área, jefe de área → agentes de su propia área
/// (CU-03 §3 precondición, RN-01).
/// </summary>
public static class Jerarquia
{
    public static RolJerarquico? NivelInmediatoInferior(RolJerarquico rol) => rol switch
    {
        RolJerarquico.Raiz => RolJerarquico.JefeGeneral,
        RolJerarquico.JefeGeneral => RolJerarquico.JefeArea,
        RolJerarquico.JefeArea => RolJerarquico.AgenteCampo,
        _ => null,
    };

    /// <summary>
    /// Determina si <paramref name="administrador"/> puede administrar un alta/baja del rol
    /// <paramref name="rolObjetivo"/> sobre el área <paramref name="areaObjetivo"/>.
    /// </summary>
    public static bool PuedeAdministrar(Usuario administrador, RolJerarquico rolObjetivo, Guid? areaObjetivo)
    {
        if (!administrador.EstadoVigencia)
        {
            return false;
        }

        if (NivelInmediatoInferior(administrador.Rol) != rolObjetivo)
        {
            return false;
        }

        // El jefe de área solo administra agentes de su propia área (RN-01).
        if (administrador.Rol == RolJerarquico.JefeArea)
        {
            return areaObjetivo is not null && areaObjetivo == administrador.AreaId;
        }

        return true;
    }
}
