namespace GeoVial.Domain;

/// <summary>
/// Relevamiento: tarea de registro de observaciones sobre una obra (modelo-datos-logico §1.3).
/// Agregado raíz que contiene sus asignaciones de agentes. Gobierna la máquina de estados (RN-05)
/// y el radio de agrupación positivo (RN-02). Cerrado ⇒ solo lectura.
/// </summary>
public sealed class Relevamiento
{
    private readonly List<AsignacionAgente> _asignaciones = new();

    public Guid RelevamientoId { get; private set; }
    public string IdentificacionObra { get; private set; }
    public EstadoRelevamiento Estado { get; private set; }
    public decimal RadioAgrupacionMetros { get; private set; }
    public Guid AreaId { get; private set; }

    public IReadOnlyList<AsignacionAgente> Asignaciones => _asignaciones;

    public bool EsSoloLectura => Estado == EstadoRelevamiento.Cerrado;

    // ctor para materialización del ORM
    private Relevamiento()
    {
        IdentificacionObra = string.Empty;
    }

    private Relevamiento(Guid relevamientoId, string identificacionObra, decimal radio, Guid areaId)
    {
        RelevamientoId = relevamientoId;
        IdentificacionObra = identificacionObra;
        Estado = EstadoRelevamiento.Recoleccion;
        RadioAgrupacionMetros = radio;
        AreaId = areaId;
    }

    /// <summary>Crea un relevamiento en estado recolección (CU-01); valida identificación y radio > 0 (RN-02).</summary>
    public static Resultado<Relevamiento> Crear(string identificacionObra, decimal radioAgrupacionMetros, Guid areaId)
    {
        if (string.IsNullOrWhiteSpace(identificacionObra))
        {
            return Resultado<Relevamiento>.Fallo(CodigosError.IdentificacionRequerida);
        }

        if (radioAgrupacionMetros <= 0)
        {
            return Resultado<Relevamiento>.Fallo(CodigosError.RadioInvalido);
        }

        return Resultado<Relevamiento>.Exito(
            new Relevamiento(Guid.NewGuid(), identificacionObra.Trim(), radioAgrupacionMetros, areaId));
    }

    /// <summary>
    /// Transición de estado (RN-05). Solo recolección → revisión y revisión → cierre.
    /// El intento de cerrado → recolección por la vía normal exige la reapertura explícita.
    /// </summary>
    public Resultado TransicionarA(EstadoRelevamiento destino)
    {
        if (Estado == EstadoRelevamiento.Cerrado && destino == EstadoRelevamiento.Recoleccion)
        {
            return Resultado.Fallo(CodigosError.ReaperturaNoAutorizada);
        }

        var valido = (Estado, destino) switch
        {
            (EstadoRelevamiento.Recoleccion, EstadoRelevamiento.Revision) => true,
            (EstadoRelevamiento.Revision, EstadoRelevamiento.Cerrado) => true,
            _ => false,
        };

        if (!valido)
        {
            return Resultado.Fallo(CodigosError.TransicionInvalida);
        }

        Estado = destino;
        return Resultado.Exito();
    }

    /// <summary>Ajusta el radio de agrupación del relevamiento (CU-11 §5.A); valida que sea positivo (RN-02).</summary>
    public Resultado AjustarRadio(decimal nuevoRadioMetros)
    {
        if (EsSoloLectura)
        {
            return Resultado.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        if (nuevoRadioMetros <= 0)
        {
            return Resultado.Fallo(CodigosError.RadioInvalido);
        }

        RadioAgrupacionMetros = nuevoRadioMetros;
        return Resultado.Exito();
    }

    /// <summary>Reapertura explícita del jefe de área: cerrado → recolección (RN-05, CU-10 §5.A).</summary>
    public Resultado Reabrir()
    {
        if (Estado != EstadoRelevamiento.Cerrado)
        {
            return Resultado.Fallo(CodigosError.TransicionInvalida);
        }

        Estado = EstadoRelevamiento.Recoleccion;
        return Resultado.Exito();
    }

    /// <summary>
    /// Asigna un agente de campo del área del relevamiento (CU-01, RN-01). Reactiva la asignación si
    /// el agente ya estuvo asignado. Rechaza si el relevamiento está cerrado (RN-05) o el agente es de
    /// otra área o no es agente de campo (RN-01).
    /// </summary>
    public Resultado AsignarAgente(Usuario agente)
    {
        if (EsSoloLectura)
        {
            return Resultado.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        if (agente.Rol != RolJerarquico.AgenteCampo || agente.AreaId != AreaId)
        {
            return Resultado.Fallo(CodigosError.AgenteFueraDeArea);
        }

        var existente = _asignaciones.FirstOrDefault(a => a.AgenteUsuarioId == agente.UsuarioId);
        if (existente is null)
        {
            _asignaciones.Add(new AsignacionAgente(RelevamientoId, agente.UsuarioId));
        }
        else
        {
            existente.Reactivar();
        }

        return Resultado.Exito();
    }

    /// <summary>Quita (baja lógica) la asignación vigente de un agente (CU-01 §5.A reasignación).</summary>
    public Resultado QuitarAgente(Guid agenteUsuarioId)
    {
        if (EsSoloLectura)
        {
            return Resultado.Fallo(CodigosError.RelevamientoSoloLectura);
        }

        _asignaciones.FirstOrDefault(a => a.AgenteUsuarioId == agenteUsuarioId && a.Vigente)?.Desactivar();
        return Resultado.Exito();
    }

    public IReadOnlyList<Guid> AgentesVigentes() =>
        _asignaciones.Where(a => a.Vigente).Select(a => a.AgenteUsuarioId).ToList();
}
