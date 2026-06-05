namespace GeoVial.Domain;

/// <summary>
/// Autorización por rol y área (RN-01, CU-14). Raíz y jefe general acceden a todas las áreas;
/// jefe de área y agente solo a la suya. La evaluación es por acceso, no segmenta físicamente la base.
/// </summary>
public static class Autorizacion
{
    /// <summary>
    /// Indica si <paramref name="usuario"/> puede acceder a un recurso perteneciente a
    /// <paramref name="areaRecurso"/> (null = recurso sin área, p. ej. catálogo global).
    /// </summary>
    public static bool PuedeAccederArea(Usuario usuario, Guid? areaRecurso)
    {
        if (!usuario.EstadoVigencia)
        {
            return false;
        }

        return usuario.Rol switch
        {
            RolJerarquico.Raiz => true,
            RolJerarquico.JefeGeneral => true,
            RolJerarquico.JefeArea => areaRecurso is not null && areaRecurso == usuario.AreaId,
            RolJerarquico.AgenteCampo => areaRecurso is not null && areaRecurso == usuario.AreaId,
            _ => false,
        };
    }

    /// <summary>
    /// Indica si el usuario ve recursos de todas las áreas (raíz y jefe general). Permite filtrar el listado
    /// en la base —área propia para el resto— sin duplicar la lógica de rol fuera de este módulo (RN-01).
    /// </summary>
    public static bool AccedeATodasLasAreas(Usuario usuario) =>
        usuario.EstadoVigencia && usuario.Rol is RolJerarquico.Raiz or RolJerarquico.JefeGeneral;

    /// <summary>
    /// Acceso a datos personales de otro usuario (CU-14 §5.B). Raíz y jefe general pueden;
    /// el jefe de área solo a usuarios de su área; un agente solo a sus propios datos (RN-08).
    /// </summary>
    public static bool PuedeAccederDatoPersonal(Usuario solicitante, Usuario objetivo)
    {
        if (!solicitante.EstadoVigencia)
        {
            return false;
        }

        if (solicitante.UsuarioId == objetivo.UsuarioId)
        {
            return true;
        }

        return solicitante.Rol switch
        {
            RolJerarquico.Raiz => true,
            RolJerarquico.JefeGeneral => true,
            RolJerarquico.JefeArea => objetivo.AreaId is not null && objetivo.AreaId == solicitante.AreaId,
            _ => false,
        };
    }
}
