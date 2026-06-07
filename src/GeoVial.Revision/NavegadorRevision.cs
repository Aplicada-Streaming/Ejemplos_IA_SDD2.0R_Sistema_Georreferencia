using GeoVial.Shared;

namespace GeoVial.Revision;

/// <summary>
/// Navegación del carrusel de revisión (US-22): recorre los marcadores de un relevamiento y, dentro de cada
/// marcador, sus fotos. La navegación es circular (el siguiente del último vuelve al primero). Al cambiar de
/// marcador se reinicia la foto actual. Un relevamiento sin marcadores o un marcador sin fotos no rompen la
/// navegación: el actual queda en <c>null</c>.
/// </summary>
public sealed class NavegadorRevision
{
    private readonly IReadOnlyList<RevisionMarcadorDto> _marcadores;

    public NavegadorRevision(RevisionRelevamientoDto revision) => _marcadores = revision.Marcadores;

    public int CantidadMarcadores => _marcadores.Count;
    public bool HayMarcadores => _marcadores.Count > 0;
    public int IndiceMarcador { get; private set; }
    public int IndiceFoto { get; private set; }

    /// <summary>Marcador en foco, o null si el relevamiento no tiene marcadores.</summary>
    public RevisionMarcadorDto? MarcadorActual => HayMarcadores ? _marcadores[IndiceMarcador] : null;

    /// <summary>Foto en foco del marcador actual, o null si el marcador no tiene fotos.</summary>
    public RevisionFotoDto? FotoActual
    {
        get
        {
            var marcador = MarcadorActual;
            return marcador is { Fotos.Count: > 0 } ? marcador.Fotos[IndiceFoto] : null;
        }
    }

    public void SiguienteMarcador() => MoverMarcador(+1);

    public void AnteriorMarcador() => MoverMarcador(-1);

    public void SiguienteFoto() => MoverFoto(+1);

    public void AnteriorFoto() => MoverFoto(-1);

    /// <summary>
    /// Avance del carrusel por gesto lateral (H-04, wireframes-marcador-carrusel §6): pasa a la foto siguiente del
    /// marcador y, si era la última (o el marcador no tiene fotos), <b>cruza al marcador siguiente</b> (su primera
    /// foto). Circular sobre todo el relevamiento. Complementa los botones, no los reemplaza.
    /// </summary>
    public void Avanzar()
    {
        var marcador = MarcadorActual;
        if (marcador is null)
        {
            return;
        }

        if (marcador.Fotos.Count > 0 && IndiceFoto < marcador.Fotos.Count - 1)
        {
            IndiceFoto++;
            return;
        }

        SiguienteMarcador(); // cruza al siguiente marcador (reinicia la foto en 0)
    }

    /// <summary>
    /// Retroceso del carrusel por gesto lateral (H-04): va a la foto anterior y, si era la primera (o el marcador
    /// no tiene fotos), <b>cruza al marcador anterior</b> y se posiciona en su <b>última</b> foto. Circular.
    /// </summary>
    public void Retroceder()
    {
        var marcador = MarcadorActual;
        if (marcador is null)
        {
            return;
        }

        if (IndiceFoto > 0)
        {
            IndiceFoto--;
            return;
        }

        AnteriorMarcador(); // cruza al marcador anterior (foto en 0)
        var nuevo = MarcadorActual;
        if (nuevo is { Fotos.Count: > 0 })
        {
            IndiceFoto = nuevo.Fotos.Count - 1; // última foto del marcador anterior
        }
    }

    /// <summary>
    /// Posiciona el carrusel en el marcador indicado (reiniciando la foto en foco), para restaurar la posición
    /// tras recargar la revisión. Devuelve false si el marcador ya no existe (no cambia la posición).
    /// </summary>
    public bool IrAlMarcador(Guid marcadorId)
    {
        for (var i = 0; i < _marcadores.Count; i++)
        {
            if (_marcadores[i].MarcadorId == marcadorId)
            {
                IndiceMarcador = i;
                IndiceFoto = 0;
                return true;
            }
        }

        return false;
    }

    private void MoverMarcador(int paso)
    {
        if (!HayMarcadores)
        {
            return;
        }

        IndiceMarcador = Circular(IndiceMarcador + paso, _marcadores.Count);
        IndiceFoto = 0; // al cambiar de marcador se reinicia la foto en foco
    }

    private void MoverFoto(int paso)
    {
        var marcador = MarcadorActual;
        if (marcador is null || marcador.Fotos.Count == 0)
        {
            return;
        }

        IndiceFoto = Circular(IndiceFoto + paso, marcador.Fotos.Count);
    }

    // Índice circular en [0, n): el módulo con corrección de negativos cierra el carrusel.
    private static int Circular(int indice, int n) => ((indice % n) + n) % n;
}
