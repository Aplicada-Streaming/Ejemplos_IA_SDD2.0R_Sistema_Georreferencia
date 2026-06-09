using FluentAssertions;
using GeoVial.Application.Reportes;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>
/// Resumen de actividad de un relevamiento (reporting): cuenta marcadores/conflictos/observaciones/bandeja/
/// fotos/comentarios y arma la productividad por agente.
/// </summary>
public class CalculadoraResumenRelevamientoTests
{
    private static readonly Guid AreaId = Guid.NewGuid();
    private static readonly Guid AgenteA = Guid.NewGuid();
    private static readonly Guid AgenteB = Guid.NewGuid();
    private static readonly DateTime T0 = new(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static Relevamiento CrearRel() => Relevamiento.Crear("Obra X", 15m, AreaId).Valor!;

    [Fact] // totales: marcadores, conflictos, observaciones, bandeja, fotos y comentarios
    public void Cuenta_los_totales()
    {
        var rel = CrearRel();
        var m1 = Marcador.Crear(rel.RelevamientoId, new Coordenada(-34.60m, -58.40m));
        var m2 = Marcador.Crear(rel.RelevamientoId, new Coordenada(-34.61m, -58.41m));
        m2.MarcarConflicto();
        var obs = new List<Observacion>
        {
            Observacion.Georreferenciada(rel.RelevamientoId, AgenteA, T0, m1.MarcadorId),
            Observacion.Georreferenciada(rel.RelevamientoId, AgenteA, T0, m2.MarcadorId),
            Observacion.EnBandejaSinGeorreferenciar(rel.RelevamientoId, AgenteB, T0),
        };

        var resumen = CalculadoraResumenRelevamiento.Calcular(
            rel, new[] { m1, m2 }, obs, totalFotos: 5, totalComentarios: 3,
            new Dictionary<Guid, string> { [AgenteA] = "Carlos", [AgenteB] = "Ana" });

        resumen.IdentificacionObra.Should().Be("Obra X");
        resumen.Marcadores.Should().Be(2);
        resumen.MarcadoresEnConflicto.Should().Be(1);
        resumen.Observaciones.Should().Be(3);
        resumen.ObservacionesSinGeorreferenciar.Should().Be(1);
        resumen.Fotos.Should().Be(5);
        resumen.Comentarios.Should().Be(3);
    }

    [Fact] // productividad por agente, de mayor a menor, con nombres resueltos
    public void Productividad_por_agente_ordenada_desc()
    {
        var rel = CrearRel();
        var obs = new List<Observacion>
        {
            Observacion.EnBandejaSinGeorreferenciar(rel.RelevamientoId, AgenteA, T0),
            Observacion.EnBandejaSinGeorreferenciar(rel.RelevamientoId, AgenteA, T0),
            Observacion.EnBandejaSinGeorreferenciar(rel.RelevamientoId, AgenteB, T0),
        };

        var resumen = CalculadoraResumenRelevamiento.Calcular(
            rel, Array.Empty<Marcador>(), obs, 0, 0,
            new Dictionary<Guid, string> { [AgenteA] = "Carlos", [AgenteB] = "Ana" });

        resumen.Productividad.Should().HaveCount(2);
        resumen.Productividad[0].Nombre.Should().Be("Carlos");
        resumen.Productividad[0].Observaciones.Should().Be(2);
        resumen.Productividad[1].Nombre.Should().Be("Ana");
        resumen.Productividad[1].Observaciones.Should().Be(1);
    }

    [Fact] // un agente sin nombre conocido queda como "(desconocido)"
    public void Agente_sin_nombre_es_desconocido()
    {
        var rel = CrearRel();
        var obs = new[] { Observacion.EnBandejaSinGeorreferenciar(rel.RelevamientoId, AgenteA, T0) };

        var resumen = CalculadoraResumenRelevamiento.Calcular(rel, Array.Empty<Marcador>(), obs, 0, 0, new Dictionary<Guid, string>());

        resumen.Productividad.Should().ContainSingle().Which.Nombre.Should().Be("(desconocido)");
    }

    [Fact] // relevamiento nulo → ArgumentNullException
    public void Relevamiento_nulo_falla()
    {
        var crear = () => CalculadoraResumenRelevamiento.Calcular(
            null!, Array.Empty<Marcador>(), Array.Empty<Observacion>(), 0, 0, new Dictionary<Guid, string>());

        crear.Should().Throw<ArgumentNullException>();
    }
}
