using FluentAssertions;
using GeoVial.Domain;
using Xunit;

namespace GeoVial.UnitTests;

/// <summary>Reglas de dominio de detección y resolución de conflictos (CU-11, CU-12; RN-02).</summary>
public class ConflictoSyncTests
{
    private static readonly Guid Relevamiento = Guid.NewGuid();

    [Fact] // CU-11: un conflicto de radio nace pendiente con los dos marcadores
    public void MarcadoresEnRadio_nace_pendiente()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var c = ConflictoSync.MarcadoresEnRadio(Relevamiento, a, b);

        c.Tipo.Should().Be(TipoConflicto.MarcadoresEnRadio);
        c.EstadoResolucion.Should().Be(EstadoResolucionConflicto.Pendiente);
        c.EstaPendiente.Should().BeTrue();
        c.RelevamientoId.Should().Be(Relevamiento);
        var (ma, mb) = c.Marcadores();
        new[] { ma, mb }.Should().BeEquivalentTo(new[] { a, b });
    }

    [Fact] // RN-02 idempotencia: el par se ordena de forma canónica para detectar duplicados sin importar el orden
    public void RecursosInvolucrados_es_canonico_independiente_del_orden()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var uno = ConflictoSync.MarcadoresEnRadio(Relevamiento, a, b);
        var otro = ConflictoSync.MarcadoresEnRadio(Relevamiento, b, a);

        otro.RecursosInvolucrados.Should().Be(uno.RecursosInvolucrados);
    }

    [Fact] // CU-12: resolver lo deja resuelto con el decisor
    public void Resolver_lo_marca_resuelto_con_decisor()
    {
        var decisor = Guid.NewGuid();
        var c = ConflictoSync.MarcadoresEnRadio(Relevamiento, Guid.NewGuid(), Guid.NewGuid());

        c.Resolver(decisor);

        c.EstadoResolucion.Should().Be(EstadoResolucionConflicto.Resuelto);
        c.EstaPendiente.Should().BeFalse();
        c.DecisorUsuarioId.Should().Be(decisor);
    }
}

/// <summary>Ajuste de radio del relevamiento (CU-11 §5.A; RN-02, RN-05).</summary>
public class AjustarRadioTests
{
    private static readonly Guid Area = Guid.NewGuid();

    private static Relevamiento Crear() => Relevamiento.Crear("Obra 7", 15m, Area).Valor!;

    [Fact] // CU-11 §5.A: ajustar a un valor positivo cambia el radio
    public void Ajustar_a_valor_positivo_ok()
    {
        var r = Crear();
        r.AjustarRadio(25m).EsExito.Should().BeTrue();
        r.RadioAgrupacionMetros.Should().Be(25m);
    }

    [Fact] // RN-02: radio no positivo se rechaza
    public void Ajustar_a_valor_no_positivo_falla()
    {
        var r = Crear();
        r.AjustarRadio(0m).Codigo.Should().Be(CodigosError.RadioInvalido);
        r.RadioAgrupacionMetros.Should().Be(15m);
    }

    [Fact] // RN-05: sobre cerrado no se puede ajustar
    public void Ajustar_sobre_cerrado_falla()
    {
        var r = Crear();
        r.TransicionarA(EstadoRelevamiento.Revision);
        r.TransicionarA(EstadoRelevamiento.Cerrado);
        r.AjustarRadio(30m).Codigo.Should().Be(CodigosError.RelevamientoSoloLectura);
    }
}

/// <summary>Reasignación de contenido al unificar marcadores (CU-12 §5.A).</summary>
public class ReasignacionMarcadorTests
{
    [Fact] // la foto cambia de marcador
    public void Foto_reasignar_cambia_marcador()
    {
        var observacion = Guid.NewGuid();
        var origen = Guid.NewGuid();
        var destino = Guid.NewGuid();
        var foto = Foto.Crear(observacion, origen, tieneMetadatos: true, FuenteCoordenada.Metadatos, "f.jpg");
        foto.ReasignarMarcador(destino);
        foto.MarcadorId.Should().Be(destino);
    }

    [Fact] // el comentario cambia de marcador
    public void Comentario_reasignar_cambia_marcador()
    {
        var origen = Guid.NewGuid();
        var destino = Guid.NewGuid();
        var comentario = Comentario.Crear(origen, null, Guid.NewGuid(), "hola", new DateTime(2026, 6, 1)).Valor!;
        comentario.ReasignarMarcador(destino);
        comentario.MarcadorId.Should().Be(destino);
    }

    [Fact] // el marcador levanta su marca de conflicto
    public void Marcador_levanta_conflicto()
    {
        var m = Marcador.Crear(Guid.NewGuid(), new Coordenada(-34.6m, -58.4m));
        m.MarcarConflicto();
        m.EnConflicto.Should().BeTrue();
        m.LevantarConflicto();
        m.EnConflicto.Should().BeFalse();
    }
}
